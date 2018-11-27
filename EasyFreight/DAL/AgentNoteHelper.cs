using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using AutoMapper;
using System.Data.Entity.Validation;
using System.Transactions;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic;

namespace EasyFreight.DAL
{
    public static class AgentNoteHelper
    {
        public static AgentNoteVm GetAgentNoteInfo(int operId, byte noteType, int agNoteId = 0, bool forEdit = true)
        {
            AgentNoteVm agNoteVm = new AgentNoteVm(noteType);
            OperationsEntities db1 = new OperationsEntities();


            AccountingEntities db = new AccountingEntities();

            AgentNote agNoteDb;

            if (agNoteId != 0)
            {
                agNoteDb = db.AgentNotes.Where(x => x.AgentNoteId == agNoteId).FirstOrDefault();
                Mapper.CreateMap<AgentNote, AgentNoteVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<AgentNoteDetail, AgentNoteDetailVm>().IgnoreAllNonExisting();
                Mapper.Map(agNoteDb, agNoteVm);

                operId = agNoteDb.OperationId;

                //agNoteVm.FromPort = operationInfo.FromPort;
                //agNoteVm.ToPort = operationInfo.ToPort;
            }
            else
            {
                agNoteDb = new AgentNote();
                agNoteVm.AgentNoteCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.AgentNote, false);

            }

            var operationInfo = db1.OperationViews.Where(x => x.OperationId == operId).FirstOrDefault();

            Mapper.CreateMap<OperationView, AgentNoteVm>()
                    .ForMember(x => x.CreateBy, y => y.Ignore())
                    .ForMember(x => x.CreateDate, y => y.Ignore())
                    .IgnoreAllNonExisting();
            Mapper.Map(operationInfo, agNoteVm);

            if (operationInfo.OrderFrom == 1)
                agNoteVm.CustomerName = operationInfo.ShipperNameEn;
            else
                agNoteVm.CustomerName = operationInfo.ConsigneeNameEn;

            agNoteVm.AgentName = operationInfo.AgentNameEn;

            var operCostObj = AccountingHelper.GetOperationCost(agNoteVm.OperationId, 0);

            var agentCostList = operCostObj.OperationCostAccVms.Where(x => x.IsAgent).ToList();
            var agentCostTotalList = operCostObj.OperationCostTotalAccVms.Where(x => x.TotalAgentRate != 0).ToList();

            var operCostList = db1.OperationCosts.Include("OperationCostLib").Include("Currency")
                  .Where(x => x.OperationId == operId && x.IsAgentCost == true).ToList();

            agNoteVm.ContainerSummary = operCostObj.ContainerSummary;

            agNoteVm.OperationCostAccVms = agentCostList;
            agNoteVm.OperationCostTotalAccVms = agentCostTotalList;
            agNoteVm.HouseBillId = agentCostList.FirstOrDefault().HouseBillId;
            if (agentCostList.Count > 0)
            {
                agNoteVm.CurrencyId = agentCostList.FirstOrDefault().CurrencyId;
                agNoteVm.CurrencySign = agentCostList.FirstOrDefault().CurrencySign;
            }
            

            if (forEdit)
            {
                //Get prev agent note details for this operation
                List<int> usedCost = db.AgentNoteDetails
                    .Where(x => x.AgentNote.OperationId == operId)
                    .Select(x => x.OperCostId)
                    .ToList();
                var newCosts = operCostList.Where(x => !usedCost.Contains(x.OperCostId)).ToList();

                AgentNoteDetailVm agentNoteDetailVm;
                foreach (var item in newCosts)
                {
                    agentNoteDetailVm = new AgentNoteDetailVm()
                    {
                        AgentNoteDetailId = 0,
                        CostName = item.OperationCostLib.OperCostNameEn,
                        CurrencyId = item.CurrencyId,
                        CurrencySign = item.Currency.CurrencySign,
                        ExchangeRate = 1,
                        MainAmount = agNoteVm.AgentNoteType == 1 ? item.OperationCostSelling : item.OperationCostNet,
                        MainCurrencyId = item.CurrencyId,
                        MainCurrencySign = item.Currency.CurrencySign,
                        OperCostId = item.OperCostId,
                        OperCostLibId = item.OperCostLibId

                    };

                    agNoteVm.AgentNoteDetails.Add(agentNoteDetailVm);
                }
            }
            else
            {
                foreach (var item in agNoteVm.AgentNoteDetails)
                {
                    item.CostName = operCostList
                        .Where(x => x.OperCostId == item.OperCostId).FirstOrDefault().OperationCostLib.OperCostNameEn;
                }
            }
            return agNoteVm;

        }

        internal static string AddEditAgentNote(AgentNoteVm agentNoteVm)
        {
            string isSaved = "true";
            AccountingEntities db = new AccountingEntities();
            AgentNote agNoteDb;
            int agNoteId = agentNoteVm.AgentNoteId;

            if (agNoteId == 0)
                agNoteDb = new AgentNote();
            else
                agNoteDb = db.AgentNotes.Where(x => x.AgentNoteId == agNoteId).FirstOrDefault();

            Mapper.CreateMap<AgentNoteVm, AgentNote>()
                .ForMember(x => x.AgentNoteDetails, y => y.Ignore())
                .IgnoreAllNonExisting();

            Mapper.Map(agentNoteVm, agNoteDb);

            AgentNoteDetail agNoteDetail;
            Mapper.CreateMap<AgentNoteDetailVm, AgentNoteDetail>().IgnoreAllNonExisting();

            foreach (var item in agentNoteVm.AgentNoteDetails)
            {
                if (item.IsSelected)
                {
                    agNoteDetail = new AgentNoteDetail();
                    Mapper.Map(item, agNoteDetail);
                    agNoteDb.AgentNoteDetails.Add(agNoteDetail);
                }
            }

            using (TransactionScope transaction = new TransactionScope())
            {

                try
                {
                    string agentAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(agNoteDb.AgentId, "Agent", "AgentId");
                    if (string.IsNullOrEmpty(agentAccId))
                        agentAccId = AccountingChartHelper.AddAgentToChart(agNoteDb.AgentId, agNoteDb.AgentNoteType);


                    if (agNoteId == 0)
                    {
                        agNoteDb.AgentNoteCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.AgentNote, true);
                        db.AgentNotes.Add(agNoteDb);
                    }

                    db.SaveChanges();

                    if (agNoteId == 0)
                    {
                        agentNoteVm.AgentId = agNoteDb.AgentId;
                        agentNoteVm.AgentNoteCode = agNoteDb.AgentNoteCode;
                        //Add invoice to accounting transactions table
                        AddAgentToTransTable(agentAccId, agentNoteVm);

                        
                        OperationHelper.ChangeOperationStatus(agentNoteVm.OperationId, (byte)StatusEnum.InvoiceIssued);
                    }
                    isSaved = "true" + agNoteDb.AgentNoteId;
                    transaction.Complete();
                }
                catch (DbEntityValidationException e)
                {
                    isSaved = "false " + e.Message;
                    AdminHelper.LastIdRemoveOne(PrefixForEnum.AgentNote);
                }
                catch (Exception e)
                {
                    isSaved = "false " + e.Message;
                    AdminHelper.LastIdRemoveOne(PrefixForEnum.AgentNote);
                }
            }


            return isSaved;
        }

        private static void AddAgentToTransTable(string agentAccId, AgentNoteVm agentNoteVm)
        {
            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId(),
                TransactionName = "Agent Note Number " + agentNoteVm.AgentNoteCode,
                TransactionNameAr = "فاتورة agent رقم " + agentNoteVm.AgentNoteCode
            };

            AccTransactionDetailVm accTransDetDebit;
            AccTransactionDetailVm accTransDetCredit;

            if (agentNoteVm.AgentNoteType == 1) // Debit .. A/R deb-- SoldServices cr .. same like invoice
            {
                accTransDetDebit = new AccTransactionDetailVm()
                {
                    AccountId = agentAccId,
                    CreditAmount = 0,
                    CurrencyId = agentNoteVm.CurrencyId,
                    DebitAmount = agentNoteVm.TotalAmount

                };
                accTrans.AccTransactionDetails.Add(accTransDetDebit);

                accTransDetCredit = new AccTransactionDetailVm()
                {
                    AccountId = ((int)AccountingChartEnum.SoldServices).ToString(),
                    CreditAmount = agentNoteVm.TotalAmount,
                    CurrencyId = agentNoteVm.CurrencyId,
                    DebitAmount = 0

                };
                accTrans.AccTransactionDetails.Add(accTransDetCredit);

            }
            else //Credit not .. CarrierCostOfSales deb -- A/P cr
            {
                accTransDetDebit = new AccTransactionDetailVm()
                {
                    AccountId = ((int)AccountingChartEnum.CarrierCostOfSales).ToString(),
                    CreditAmount = 0,
                    CurrencyId = agentNoteVm.CurrencyId,
                    DebitAmount = agentNoteVm.TotalAmount

                };
                accTrans.AccTransactionDetails.Add(accTransDetDebit);

                accTransDetCredit = new AccTransactionDetailVm()
                {
                    AccountId = agentAccId,
                    CreditAmount = agentNoteVm.TotalAmount,
                    CurrencyId = agentNoteVm.CurrencyId,
                    DebitAmount = 0

                };
                accTrans.AccTransactionDetails.Add(accTransDetCredit);
            }

            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, "AgentNote", agentNoteVm.AgentNoteId, "AgentNoteId");

        }

        public static List<AgentNoteVm> GetAgentNoteList(int operId)
        {
            AccountingEntities db = new AccountingEntities();
            List<AgentNoteVm> agentNoteList = new List<AgentNoteVm>();
            var agNoteDb = db.AgentNotes.Where(x => x.OperationId == operId).ToList();
            Mapper.CreateMap<AgentNote, AgentNoteVm>()
                .ForMember(x => x.AgentNoteDetails, y => y.Ignore())
                .IgnoreAllNonExisting();

            Mapper.Map(agNoteDb, agentNoteList);
            //Get agent Name
            List<int> agentIds = agentNoteList.Select(x => x.AgentId).ToList();
            EasyFreightEntities db1 = new EasyFreightEntities();
            var agentInfo = db1.Agents.Where(x => agentIds.Contains(x.AgentId))
                .Select(x => new { x.AgentId, x.AgentNameEn }).ToList();
            // update to show status 24/01/2017
            var invStatusLib = db.InvStatusLibs.Select(a => a).ToList(); 

            foreach (var item in agentNoteList)
            {
                item.AgentName = agentInfo.Where(x => x.AgentId == item.AgentId).FirstOrDefault().AgentNameEn;
                // update to show status 24/01/2017
                item.InvStatusName = invStatusLib.Where(x => x.InvStatusId == item.InvStatusId)
                   .FirstOrDefault().InvStatusNameEn;
                if (item.AgentNoteType == 1) //Debit .. will collect
                    item.InvStatusName = item.InvStatusName.Replace("Paid", "Collected"); 
            }

            return agentNoteList;
        }

        internal static JObject GetAgNoteListJson(System.Web.Mvc.FormCollection form)
        {
            AccountingEntities db = new AccountingEntities();
            AgentNoteView invViewDb = new AgentNoteView();
            string where = CommonHelper.AdvancedSearch<AgentNoteView>(form, invViewDb);
            if (string.IsNullOrEmpty(where))
                where = "1 = 1"; //instead of make if condition
            var agNoteList = db.AgentNoteViews.Where(where.ToString()).ToList();
            //fi moseba hena :D .. This table schema should be changed to Indentity PK and two FK 
            //From Invoice or From Agent Note .. The other option is to create another table for Agent Note collection :)

            List<int> invIds = agNoteList.Select(x => x.AgentNoteId).ToList();
            var cashInvDb = db.CashInReceiptAgNotes
                .Where(x => invIds.Contains(x.AgentNoteId))
                .GroupBy(x => x.AgentNoteId)
                .ToList();

            var cashOutInvDb = db.CashOutReceiptAgNotes
                .Where(x => invIds.Contains(x.AgentNoteId))
                .GroupBy(x => x.AgentNoteId)
                .ToList();

            decimal collectedAmount, amountDue;

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in agNoteList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("AgentNoteId");
                pJTokenWriter.WriteValue(item.AgentNoteId);

                pJTokenWriter.WritePropertyName("OperationId");
                pJTokenWriter.WriteValue(item.OperationId);

                pJTokenWriter.WritePropertyName("AgentNoteCode");
                pJTokenWriter.WriteValue(item.AgentNoteCode);

                pJTokenWriter.WritePropertyName("OperationCode");
                pJTokenWriter.WriteValue(item.OperationCode);

                pJTokenWriter.WritePropertyName("AgentNoteType"); //1 Debit  2 Credit
                pJTokenWriter.WriteValue(item.AgentNoteType);

                pJTokenWriter.WritePropertyName("DueDate");
                pJTokenWriter.WriteValue(item.DueDate.Value.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("AgentNoteDate");
                pJTokenWriter.WriteValue(item.AgentNoteDate.ToString("dd/MM/yyyy"));

                switch (item.OrderFrom)
                {
                    case 1:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-up'></i>");

                        break;
                    case 2:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-down'></i>");

                        break;
                }

                pJTokenWriter.WritePropertyName("MBL");
                pJTokenWriter.WriteValue(item.MBL);

                pJTokenWriter.WritePropertyName("AgentName");
                pJTokenWriter.WriteValue(item.AgentNameEn);

                pJTokenWriter.WritePropertyName("TotalAmount");
                pJTokenWriter.WriteValue(item.TotalAmount + " (" + item.CurrencySign + ")");
                if (item.AgentNoteType == 1) //Debit .. Get prev collected from cash in
                {
                    collectedAmount = cashInvDb.Where(x => x.Key == item.AgentNoteId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                    amountDue = item.TotalAmount - collectedAmount;
                }
                else //Credit .. Get prev Paid from Cash out
                {
                    collectedAmount = cashOutInvDb.Where(x => x.Key == item.AgentNoteId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                    amountDue = item.TotalAmount - collectedAmount;
                }



                pJTokenWriter.WritePropertyName("AmountDue"); //Modify this after make payment
                pJTokenWriter.WriteValue(amountDue + " (" + item.CurrencySign + ")");

                pJTokenWriter.WritePropertyName("InvStatusName");
                if (item.AgentNoteType == 1) //Debit .. will collect
                    pJTokenWriter.WriteValue(item.InvStatusNameEn.Replace("Paid", "Collected"));
                else
                    pJTokenWriter.WriteValue(item.InvStatusNameEn);

                pJTokenWriter.WritePropertyName("InvStatusId");
                pJTokenWriter.WriteValue(item.InvStatusId);


                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;

        }

        internal static void ChangeAgNoteStatus(int agNoteId, InvStatusEnum invStatusEnum)
        {
            byte invStatusId = (byte)invStatusEnum;
            AccountingEntities db = new AccountingEntities();
            var invDb = db.AgentNotes.Where(x => x.AgentNoteId == agNoteId).FirstOrDefault();
            invDb.InvStatusId = invStatusId;

            db.SaveChanges();

        }

        internal static string DeleteAgentInReceipt(int receiptId, int agnid, string deleteReason)
        {
            return CashInOutReceiptHelper.CashInReceipt_Delete(receiptId, 0, agnid, deleteReason) ? "true" : "false";

        }

        internal static string DeleteAgentOutReceipt(int receiptId, int agnid, string deleteReason)
        {
            return CashInOutReceiptHelper.CashOutReceipt_Delete(receiptId, 0, agnid, deleteReason) ? "true" : "false";

        }

    }
}