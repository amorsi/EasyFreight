using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.ViewModel;
using EasyFreight.Models;
using AutoMapper;
using System.Data.Entity.Validation;
using System.Transactions;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic;

namespace EasyFreight.DAL
{
    public static class InvoiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hbId"></param>
        /// <param name="operId"></param>
        /// <param name="invId"></param>
        /// <param name="invType">0 all currency, 1 EGP only, 2 other currency</param>
        /// <returns></returns>
        public static InvoiceVm GetInvoiceInfo(int hbId = 0, int operId = 0, int invId = 0, byte invType = 0)
        {
            InvoiceVm invoiceVm = new InvoiceVm(invType);
            OperationsEntities db1 = new OperationsEntities();
            var hbInfo = db1.HouseBillViews.Where(x => x.HouseBillId == hbId).FirstOrDefault();

            //Get saved invoice for HB with same invoice type .. if any
            AccountingEntities db = new AccountingEntities();
            var savedInv = db.Invoices.Where(x => x.HouseBillId == hbId && x.InvoiceType == invType).FirstOrDefault();
            if (savedInv != null)
                invId = savedInv.InvoiceId;

            //If invoice id != 0 ... will fill InvoiceVm from Invoice table
            if (invId != 0)
            {
                Invoice invDb = db.Invoices.Where(x => x.InvoiceId == invId).FirstOrDefault();
                Mapper.CreateMap<Invoice, InvoiceVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<InvoiceTotal, InvoiceTotalVm>().IgnoreAllNonExisting();
                Mapper.Map(invDb, invoiceVm);

                invoiceVm.FromPort = hbInfo.FromPort;
                invoiceVm.ToPort = hbInfo.ToPort;
                invoiceVm.PaymentTermName = invDb.PaymentTerm.PaymentTermEn;
                invType = invDb.InvoiceType;
            }
            else
                invoiceVm.InvoiceCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.AccountingInvoice, false);

            Mapper.CreateMap<HouseBillView, InvoiceVm>()
                    .ForMember(x => x.CreateBy, y => y.Ignore())
                    .ForMember(x => x.CreateDate, y => y.Ignore())
                    .IgnoreAllNonExisting();
            Mapper.Map(hbInfo, invoiceVm);

            if (hbInfo.OrderFrom == 1)
            {
                invoiceVm.CustomerName = hbInfo.ShipperNameEn;
                invoiceVm.ConsigneeId = null;
            }
            else
            {
                invoiceVm.CustomerName = hbInfo.ConsigneeNameEn;
                invoiceVm.ShipperId = null;
            }

            //Get Cost list 
            var operCostObj = AccountingHelper.GetOperationCost(invoiceVm.OperationId, invoiceVm.HouseBillId.Value);

            var operCostList = operCostObj.OperationCostAccVms;
            var operCostTotalList = operCostObj.OperationCostTotalAccVms;

            GetHbInvTotal(invType, ref operCostList, ref operCostTotalList);

            invoiceVm.OperationCostAccVms = operCostList;
            invoiceVm.OperationCostTotalAccVms = operCostTotalList;

            return invoiceVm;
        }

        private static void GetHbInvTotal(int invType, ref IList<OperationCostAccVm> operCostList,
            ref IList<OperationCostTotalAccVm> operCostTotalList)
        {
            if (invType == 0)
            {
                //return the same 
            }
            else if (invType == 1)
            {
                operCostList = operCostList.Where(x => x.CurrencyId == 1).ToList();
                operCostTotalList = operCostTotalList.Where(x => x.CurrencyId == 1).ToList(); ;
            }
            else if (invType == 2)
            {
                operCostList = operCostList.Where(x => x.CurrencyId != 1).ToList();
                operCostTotalList = operCostTotalList.Where(x => x.CurrencyId != 1).ToList(); ;
            }
        }

        internal static string AddEditInvoice(InvoiceVm invoiceVm)
        {
            string isSaved = "true";

            AccountingEntities db = new AccountingEntities();
            int invoiceId = invoiceVm.InvoiceId;
            byte invoiceType = invoiceVm.InvoiceType;
           // int hbId = invoiceVm.HouseBillId;
            
            Invoice invDb;
            if (invoiceId == 0)
                invDb = new Invoice();
            else
            {
                invDb = db.Invoices.Include("InvoiceDetails").Include("InvoiceTotals")
                    .Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
                //Delete Invoice details and totals .. will insert it again
                var invDbTotals = invDb.InvoiceTotals;
                var invDbDetails = invDb.InvoiceDetails;
                foreach (var item in invDbDetails)
                {
                    invDb.InvoiceDetails.Remove(item);
                }
                foreach (var item in invDbTotals)
                {
                    invDb.InvoiceTotals.Remove(item);
                }
            }


            Mapper.CreateMap<InvoiceVm, Invoice>()
                .ForMember(x => x.InvoiceTotals, y => y.Ignore())
                .ForMember(x => x.InvoiceDetails, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(invoiceVm, invDb);

            InvoiceDetail invDetail;
            Mapper.CreateMap<InvoiceDetailVm, InvoiceDetail>().IgnoreAllNonExisting();
            foreach (var item in invoiceVm.InvoiceDetails)
            {
                if (item.IsSelected == true)
                {
                    invDetail = new InvoiceDetail();
                    Mapper.Map(item, invDetail);
                    invDb.InvoiceDetails.Add(invDetail);
                }
            }

            InvoiceTotal invTotalDb;
            foreach (var item in invoiceVm.InvoiceTotals)
            {
                invTotalDb = new InvoiceTotal()
                {
                    CurrencyId = item.CurrencyId,
                    TotalAmount = item.TotalAmount,
                    CurrencySign = item.CurrencySign,
                    TaxDepositAmount = item.TaxDepositAmount,
                    TotalBeforeTax = item.TotalBeforeTax,
                    VatTaxAmount = item.VatTaxAmount

                };

                invDb.InvoiceTotals.Add(invTotalDb);
            }



            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    //Add shipper or consignee to accounting chart
                    string accountId = GetAccountId(invoiceVm.OrderFrom, invoiceVm.ShipperId, invoiceVm.ConsigneeId);

                    if (invoiceId == 0)
                    {
                        invDb.InvoiceCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.AccountingInvoice, true);
                        db.Invoices.Add(invDb);
                    }

                    db.SaveChanges();

                    invoiceVm.InvoiceId = invDb.InvoiceId;
                    invoiceVm.InvoiceCode = invDb.InvoiceCode;


                    //Change HB status
                    if (invoiceId == 0)
                    {
                        //Add invoice to accounting transactions table
                        AddInvToTransTable(accountId, invoiceVm);

                        HouseBillHelper.ChangeHBStatus(invDb.HouseBillId, (byte)StatusEnum.InvoiceIssued);
                        OperationHelper.ChangeOperationStatus(invDb.OperationId, (byte)StatusEnum.InvoiceIssued);
                    }

                    transaction.Complete();
                }
                catch (DbEntityValidationException e)
                {
                    isSaved = "false " + e.Message;

                    //AdminHelper.LastIdRemoveOne(PrefixForEnum.AccountingInvoice);
                }
                catch (Exception e)
                {
                    isSaved = "false " + e.Message;
                    //AdminHelper.LastIdRemoveOne(PrefixForEnum.AccountingInvoice);
                }
            }

            return isSaved;
        }

        internal static List<InvoiceLightVm> GetInvListForHb(int hbId)
        {
            AccountingEntities db2 = new AccountingEntities();

            var invList = db2.Invoices.Where(x => x.HouseBillId == hbId)
                .Select(x => new InvoiceLightVm
                {
                    InvoiceCode = x.InvoiceCode,
                    InvoiceId = x.InvoiceId,
                    InvoiceDate = x.InvoiceDate,
                    InvoiceType = x.InvoiceType,
                    HouseBillId = x.HouseBillId
                })
                .ToList();

            //Get Cost list 
            var operCostObj = AccountingHelper.GetOperationCost(0, hbId);

            var operCostList = operCostObj.OperationCostAccVms;


            foreach (var item in invList)
            {
                var operCostTotalList = operCostObj.OperationCostTotalAccVms;
                GetHbInvTotal(item.InvoiceType, ref operCostList, ref operCostTotalList);
                item.OperationCostTotalAccVms = operCostTotalList;
            }
            return invList;

        }

        
        /// <summary>
        /// Add Invoice amounts to the accounting transactions table
        /// </summary>
        /// <param name="clientAccId">shipeer/ consignee account Id</param>
        /// <param name="invVm">Invoice Vm obj</param>
        private static void AddInvToTransTable(string clientAccId, InvoiceVm invoiceVm)
        {
            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId(),
                TransactionName = "Invoice Number " + invoiceVm.InvoiceCode,
                TransactionNameAr = "فاتورة رقم " + invoiceVm.InvoiceCode
                
            };

            //Get Cost list 
          //  var operCostObj = AccountingHelper.GetOperationCost(invoiceVm.OperationId, invoiceVm.HouseBillId);

            var operCostList = invoiceVm.InvoiceDetails;
            var operCostTotalList = invoiceVm.InvoiceTotals;

            //GetHbInvTotal(invoiceVm.InvoiceType, ref operCostList, ref operCostTotalList);

            //Tax Deposit AccountId
            string taxDepositAccId = "";
            if (operCostTotalList.FirstOrDefault().TaxDepositAmount != 0)
                taxDepositAccId = GetTaxDepositAccountId(invoiceVm.OrderFrom, invoiceVm.ShipperId, invoiceVm.ConsigneeId);

            AccTransactionDetailVm accTransDetDebit;
            AccTransactionDetailVm accTransDetCredit;
            AccTransactionDetailVm accTransDetVAT;
            foreach (var item in operCostTotalList)
            {
                accTransDetDebit = new AccTransactionDetailVm();
                accTransDetDebit.AccountId = clientAccId;
                accTransDetDebit.DebitAmount = item.TotalAmount;
                accTransDetDebit.CurrencyId = item.CurrencyId;


                accTransDetCredit = new AccTransactionDetailVm();
                accTransDetCredit.AccountId = ((int)AccountingChartEnum.SoldServices).ToString();
                accTransDetCredit.CreditAmount = item.TotalBeforeTax;
                accTransDetCredit.CurrencyId = item.CurrencyId;

                accTrans.AccTransactionDetails.Add(accTransDetDebit);
                accTrans.AccTransactionDetails.Add(accTransDetCredit);

                if(item.VatTaxAmount != 0)
                {
                    accTransDetVAT = new AccTransactionDetailVm();
                    accTransDetVAT.AccountId = ((int)AccountingChartEnum.VAT).ToString();
                    accTransDetVAT.CreditAmount = item.VatTaxAmount;
                    accTransDetVAT.CurrencyId = item.CurrencyId;
                    accTrans.AccTransactionDetails.Add(accTransDetVAT);
                }

                if(!string.IsNullOrEmpty(taxDepositAccId))
                {
                    AccTransactionDetailVm accTransTaxDepDebit = new AccTransactionDetailVm();
                    accTransTaxDepDebit.AccountId = taxDepositAccId;
                    accTransTaxDepDebit.DebitAmount = item.TaxDepositAmount;
                    accTransTaxDepDebit.CurrencyId = item.CurrencyId;
                    accTrans.AccTransactionDetails.Add(accTransTaxDepDebit);
                }
            }

            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, "Invoice", invoiceVm.InvoiceId, "InvoiceId");
        }

        internal static List<InvoiceVm> GetInvoiceListForOper(int operId, int hbId = 0)
        {
            AccountingEntities db = new AccountingEntities();
            OperationsEntities db1 = new OperationsEntities();
            List<InvoiceVm> invVmList = new List<InvoiceVm>();
            List<Invoice> invDbList = new List<Invoice>();

            //adding condaition  [.Where(a => a.IsDeleted == null)] to get undeleted invoices
            if (operId != 0)
                invDbList = db.Invoices.Include("InvStatusLib").Where(x => x.OperationId == operId).Where(a => a.IsDeleted == null).ToList();
            else
                invDbList = db.Invoices.Include("InvStatusLib").Where(a => a.IsDeleted == null).ToList();

            Mapper.CreateMap<Invoice, InvoiceVm>().IgnoreAllNonExisting();
            Mapper.CreateMap<InvoiceDetail, InvoiceDetailVm>().IgnoreAllNonExisting();
            Mapper.CreateMap<InvoiceTotal, InvoiceTotalVm>().IgnoreAllNonExisting();
            Mapper.Map(invDbList, invVmList);

            List<HouseBillView> hbList = new List<HouseBillView>();
            if (operId != 0)
                hbList = db1.HouseBillViews.Where(x => x.OperationId == operId).ToList();
            else
            {
                List<int> hbIds = invDbList.Select(x => x.HouseBillId).ToList();
                hbList = db1.HouseBillViews.Where(x => hbIds.Contains(x.HouseBillId)).ToList();
            }

            Mapper.CreateMap<HouseBillView, InvoiceVm>()
                    .ForMember(x => x.CreateBy, y => y.Ignore())
                    .ForMember(x => x.CreateDate, y => y.Ignore())
                    .IgnoreAllNonExisting();

            List<int> invIds = invVmList.Select(x => x.InvoiceId).ToList();
            var cashInvDb = db.CashInReceiptInvs
                .Where(x => invIds.Contains(x.InvoiceId))
                .GroupBy(x => x.InvoiceId)
                .ToList();

            foreach (var item in invVmList)
            {
                var hb = hbList.Where(x => x.HouseBillId == item.HouseBillId).FirstOrDefault();
                Mapper.Map(hb, item);

                if (item.OrderFrom == 1)
                    item.CustomerName = hb.ShipperNameEn;
                else
                    item.CustomerName = hb.ConsigneeNameEn;

                item.InvStatusName = invDbList.Where(x => x.InvoiceId == item.InvoiceId)
                    .FirstOrDefault().InvStatusLib.InvStatusNameEn;

               decimal collectedAmount = cashInvDb.Where(x => x.Key == item.InvoiceId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
               item.AmountDue = item.InvoiceTotals.FirstOrDefault().TotalAmount - collectedAmount;
            }

            return invVmList;
        }


        internal static JObject GetInvListJson(System.Web.Mvc.FormCollection form)
        {
            AccountingEntities db = new AccountingEntities();
            InvoiceView invViewDb = new InvoiceView();
            string where = CommonHelper.AdvancedSearch<InvoiceView>(form, invViewDb);
            if (string.IsNullOrEmpty(where))
                where = "1 = 1"; //instead of make if condition 

            where = where + " and IsDeleted = null ";

            var invoiceList = db.InvoiceViews.Where(where.ToString()).ToList();

            List<int> invIds = invoiceList.Select(x => x.InvoiceId).ToList();
            var cashInvDb = db.CashInReceiptInvs
                .Where(x => invIds.Contains(x.InvoiceId))
                .GroupBy(x => x.InvoiceId)
                .ToList();
            decimal collectedAmount, amountDue;
            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in invoiceList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("InvoiceId");
                pJTokenWriter.WriteValue(item.InvoiceId);

                pJTokenWriter.WritePropertyName("HouseBillId");
                pJTokenWriter.WriteValue(item.HouseBillId);

                pJTokenWriter.WritePropertyName("InvoiceCode");
                pJTokenWriter.WriteValue(item.InvoiceCode);

                pJTokenWriter.WritePropertyName("DueDate");
                pJTokenWriter.WriteValue(item.DueDate.Value.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("InvoiceDate");
                pJTokenWriter.WriteValue(item.InvoiceDate.ToString("dd/MM/yyyy"));

                switch (item.OrderFrom)
                {
                    case 1:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-up'></i>");

                        pJTokenWriter.WritePropertyName("ClientName");
                        pJTokenWriter.WriteValue(item.ShipperNameEn);
                        break;
                    case 2:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-down'></i>");

                        pJTokenWriter.WritePropertyName("ClientName");
                        pJTokenWriter.WriteValue(item.ConsigneeNameEn);
                        break;
                }

                pJTokenWriter.WritePropertyName("OperationCode");
                pJTokenWriter.WriteValue(item.OperationCode);

                pJTokenWriter.WritePropertyName("HouseBL");
                pJTokenWriter.WriteValue(item.HouseBL);

                

                pJTokenWriter.WritePropertyName("TotalAmount");
                pJTokenWriter.WriteValue(item.TotalAmount + " (" + item.CurrencySign + ")");

                 collectedAmount = cashInvDb.Where(x => x.Key == item.InvoiceId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                 amountDue = item.TotalAmount - collectedAmount;


                pJTokenWriter.WritePropertyName("AmountDue"); //Modify this after make payment
                pJTokenWriter.WriteValue(amountDue + " (" + item.CurrencySign + ")");

                pJTokenWriter.WritePropertyName("InvStatusId");
                pJTokenWriter.WriteValue(item.InvStatusId);

                pJTokenWriter.WritePropertyName("InvStatusName");
                pJTokenWriter.WriteValue(item.InvStatusNameEn.Replace("Paid", "Collected"));

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static InvoiceVm GetInvoiceInfoNew(int hbId = 0, int invId = 0, bool forEdit = true)
        {
            InvoiceVm invoiceVm = new InvoiceVm(0);
            

            AccountingEntities db = new AccountingEntities();

            if (invId != 0)
            {
                Invoice invDb = db.Invoices.Where(x => x.InvoiceId == invId).FirstOrDefault();
                Mapper.CreateMap<Invoice, InvoiceVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<InvoiceDetail, InvoiceDetailVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<InvoiceTotal, InvoiceTotalVm>().IgnoreAllNonExisting();
                Mapper.Map(invDb, invoiceVm);

                //invoiceVm.FromPort = hbInfo.FromPort;
                //invoiceVm.ToPort = hbInfo.ToPort;
                invoiceVm.PaymentTermName = invDb.PaymentTerm.PaymentTermEn;
                invoiceVm.CurrencySign = invDb.Currency.CurrencySign;
                //invType = invDb.InvoiceType;

                //Add Tax part
               //decimal totalBeforeTax = invoiceVm.InvoiceDetails.Sum(x => x.InvoiceAmount);
               // invoiceVm.InvoiceTotals.FirstOrDefault().TotalBeforeTax = totalBeforeTax;
               // invoiceVm.InvoiceTotals.FirstOrDefault().TaxDepositAmount = totalBeforeTax - invoiceVm.InvoiceTotals.FirstOrDefault().TotalAmount;

            }
            else
                invoiceVm.InvoiceCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.AccountingInvoice, false);

            if (hbId == 0)
                hbId = invoiceVm.HouseBillId.Value;

            OperationsEntities db1 = new OperationsEntities();
            var hbInfo = db1.HouseBillViews.Where(x => x.HouseBillId == hbId).FirstOrDefault();

            Mapper.CreateMap<HouseBillView, InvoiceVm>()
            .ForMember(x => x.CreateBy, y => y.Ignore())
            .ForMember(x => x.CreateDate, y => y.Ignore())
            .IgnoreAllNonExisting();
            Mapper.Map(hbInfo, invoiceVm);

            if (hbInfo.OrderFrom == 1)
            {
                invoiceVm.CustomerName = hbInfo.ShipperNameEn;
                invoiceVm.ConsigneeId = null;
            }
            else
            {
                invoiceVm.CustomerName = hbInfo.ConsigneeNameEn;
                invoiceVm.ShipperId = null;
            }

            //Get Cost list 
            var operCostObj = AccountingHelper.GetOperationCost(invoiceVm.OperationId, invoiceVm.HouseBillId.Value);

            var operCostList = operCostObj.OperationCostAccVms;
            var operCostTotalList = operCostObj.OperationCostTotalAccVms;

            GetHbInvTotal(0, ref operCostList, ref operCostTotalList);

            invoiceVm.OperationCostAccVms = operCostList;
            invoiceVm.OperationCostTotalAccVms = operCostTotalList ;

            
            

            if (forEdit)
            {
                //Get prev invoice details for this HB
                // adding condation  [x.Invoice.IsDeleted==null] get undeleted invoices
                var prevInvDetails = db.InvoiceDetails
                    .Where(x => x.Invoice.HouseBillId == hbId && x.Invoice.IsDeleted==null).OrderBy(x=>x.ItemOrder)
                    .Select(x => new { x.CostFkId,x.FkType })
                    .ToList();
                InvoiceDetailVm invDetailVm;
                //Add other costs that are not inserted in inv details table 
                for (int i = 1; i < 4; i++) // loop for the three cost types
                {
                    List<int> usedCost = invoiceVm.InvoiceDetails.Where(x => x.FkType == i).Select(x => x.CostFkId).ToList();
                    usedCost.AddRange(prevInvDetails.Where(x => x.FkType == i).Select(x => x.CostFkId).ToList());
                    var newCosts = operCostList.Where(x => !usedCost.Contains(x.CostFkId) && x.FkType == i).ToList();
                    foreach (var item in newCosts)
                    {
                        invDetailVm = new InvoiceDetailVm()
                        {
                            
                            CostFkId = item.CostFkId,
                            CostName = item.CostName,
                            ExchangeRate = 1,
                            FkType = item.FkType,
                            InvDetailId = 0,
                            InvoiceAmount = item.SellingRate,
                            InvoiceId = invoiceVm.InvoiceId,
                            MainAmount = item.SellingRate,
                            MainCurrencyId = item.CurrencyIdSelling,
                            MainCurrSign = item.CurrencySignSelling
                        };

                        invoiceVm.InvoiceDetails.Add(invDetailVm);
                    }

                }

                if (invoiceVm.InvoiceTotals.Count == 0)
                {
                    InvoiceTotalVm invTotal = new InvoiceTotalVm()
                    {
                        CurrencyId = invoiceVm.InvCurrencyId,
                        CurrencySign = invoiceVm.CurrencySign,
                        InvoiceId = invoiceVm.InvoiceId,
                        TotalAmount = 0
                    };

                    invoiceVm.InvoiceTotals.Add(invTotal);
                }
            }
            else
            {
                Dictionary<int,string> operCost = new Dictionary<int,string>();
                Dictionary<int,string> truckCost = new Dictionary<int,string>();
                Dictionary<int, string> ccCost = new Dictionary<int, string>();
                if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.OperationCost))
                    operCost = ListCommonHelper.GetOperationCostList();
                if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.TruckingCost))
                    truckCost = ListCommonHelper.GetTruckingCostNameList();
                if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.CCCost))
                    ccCost = ListCommonHelper.GetCustClearCostList();
                //Get cost name for invoice items
                foreach (var item in invoiceVm.InvoiceDetails)
                {
                    item.CostName = invoiceVm.OperationCostAccVms
                        .Where(x => x.CostFkId == item.CostFkId && x.FkType == item.FkType)
                        .FirstOrDefault().CostName;
                    
                }
            }
            invoiceVm.InvoiceDetails = invoiceVm.InvoiceDetails.OrderBy(z => z.ItemOrder).ToList();
            return invoiceVm;

        }

        /// <summary>
        /// Get the shipper/ consignee account Id .. if has non .. add new account to the chart
        /// and update the accountId in shipper/ consignee table
        /// </summary>
        /// <param name="orderFrom"></param>
        /// <param name="shipperId"></param>
        /// <param name="consigneeId"></param>
        /// <returns></returns>
        internal static string GetAccountId(byte orderFrom, int? shipperId, int? consigneeId)
        {
            string accountNameEn = "", accountNameAr = "", accountId = "";
            if (orderFrom == 1) //export
            {
                var shipper = ShipperHelper.GetShipperInfo(shipperId.Value);
                accountId = shipper.AccountId;
                if (string.IsNullOrEmpty(accountId))
                {
                    accountNameEn = shipper.ShipperNameEn;
                    accountNameAr = string.IsNullOrEmpty(shipper.ShipperNameAr) ? accountNameEn : shipper.ShipperNameAr;
                    string parentAccountId = ((int)AccountingChartEnum.AccountsRecievable).ToString();
                    //Add new accountId to the chart
                    accountId = AccountingChartHelper.AddAccountToChart(accountNameEn, accountNameAr, parentAccountId);
                    //update AccountId column shipper table 
                    AccountingChartHelper.AddAccountIdToObj(accountId, "shipper", shipper.ShipperId, "ShipperId");
                }
            }
            else if (orderFrom == 2) //import
            {
                var consignee = ConsigneeHelper.GetConsigneeInfo(consigneeId.Value);
                accountId = consignee.AccountId;
                if (string.IsNullOrEmpty(accountId))
                {
                    accountNameEn = consignee.ConsigneeNameEn;
                    accountNameAr = string.IsNullOrEmpty(consignee.ConsigneeNameAr) ? accountNameEn : consignee.ConsigneeNameAr;
                    string parentAccountId = ((int)AccountingChartEnum.AccountsRecievable).ToString();
                    //Add new accountId to the chart
                    accountId = AccountingChartHelper.AddAccountToChart(accountNameEn, accountNameAr, parentAccountId);
                    //update AccountId column shipper table 
                    AccountingChartHelper.AddAccountIdToObj(accountId, "consignee", consignee.ConsigneeId, "ConsigneeId");
                }
            }

            return accountId;
        }


        internal static string GetTaxDepositAccountId(byte orderFrom, int? shipperId, int? consigneeId)
        {
            string accountNameEn = "", accountNameAr = "", accountId = "";
            if (orderFrom == 1) //export
            {
                var shipper = ShipperHelper.GetShipperInfo(shipperId.Value);
                accountId = shipper.TaxDepositAccountId;
                if (string.IsNullOrEmpty(accountId))
                {
                    accountNameEn = shipper.ShipperNameEn + " Tax Deposit";
                    accountNameAr = string.IsNullOrEmpty(shipper.ShipperNameAr) ? accountNameEn  : shipper.ShipperNameAr +  " ضريبة الخصم ";
                    string parentAccountId = ((int)AccountingChartEnum.TaxDepositDebit).ToString();
                    //Add new accountId to the chart
                    accountId = AccountingChartHelper.AddAccountToChart(accountNameEn, accountNameAr, parentAccountId);
                    //update AccountId column shipper table 
                    AccountingChartHelper.AddAccountIdToObj(accountId, "shipper", shipper.ShipperId, "ShipperId", "TaxDepositAccountId");
                }
            }
            else if (orderFrom == 2) //import
            {
                var consignee = ConsigneeHelper.GetConsigneeInfo(consigneeId.Value);
                accountId = consignee.TaxDepositAccountId;
                if (string.IsNullOrEmpty(accountId))
                {
                    accountNameEn = consignee.ConsigneeNameEn + " Tax Deposit";
                    accountNameAr = string.IsNullOrEmpty(consignee.ConsigneeNameAr) ? accountNameEn + " Tax Deposit" : consignee.ConsigneeNameAr + " ضريبة الخصم ";
                    string parentAccountId = ((int)AccountingChartEnum.TaxDepositDebit).ToString();
                    //Add new accountId to the chart
                    accountId = AccountingChartHelper.AddAccountToChart(accountNameEn, accountNameAr, parentAccountId);
                    //update AccountId column shipper table 
                    AccountingChartHelper.AddAccountIdToObj(accountId, "consignee", consignee.ConsigneeId, "ConsigneeId", "TaxDepositAccountId");
                }
            }

            return accountId;
        }

        internal static void ChangeInvStatus(int invId, InvStatusEnum invStatusEnum,bool isOut = false)
        {
            byte invStatusId = (byte)invStatusEnum;
            AccountingEntities db = new AccountingEntities();
            if (isOut)
            {
                var invDb = db.InvoiceAPs.Where(x => x.InvoiceId == invId).FirstOrDefault();
                invDb.InvStatusId = invStatusId;
            }
            else
            {
                var invDb = db.Invoices.Where(x => x.InvoiceId == invId).FirstOrDefault();
                invDb.InvStatusId = invStatusId;
            }
            

            db.SaveChanges();

        }

        internal static string Delete(int invId, string deleteReason)
        {
            string isSaved = "false";

            if (invId == 0)
                return isSaved;

            AccountingEntities db = new AccountingEntities();

            Invoice invDb = new Invoice();
            invDb = db.Invoices
                    .Where(x => x.InvoiceId == invId).FirstOrDefault();

            int? transID = null;
            if (invDb.TransId != null)
                transID = invDb.TransId.Value;  

            if (invDb.InvStatusId == 1 || invDb.InvStatusId == 2)
            {
                invDb.IsDeleted = true;
                invDb.DeleteReason = deleteReason;
                invDb.DeletedBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId(); 
                invDb.TransId = null;

                // invoice total
                var invTotal = db.InvoiceTotals.Where(x => x.InvoiceId == invId).FirstOrDefault();
                
                try
                { 
                    db.SaveChanges();

                    db.InvoiceTotals.Remove(invTotal); 

                    if (transID.HasValue  )
                        AccountingHelper.DeleteTransaction(transID.Value);

                    #region Update Operation Status...
                    try
                    {
                        var provInvs = db.Invoices
                     .Where(x => x.OperationId == invDb.OperationId && x.IsDeleted == null && x.InvoiceId != invDb.InvoiceId).ToList();
                        if (provInvs.Count == 0)
                        {
                            OperationsEntities opdb = new OperationsEntities();
                            var operation = opdb.Operations.Where(z => z.OperationId == invDb.OperationId).FirstOrDefault();
                            operation.StatusId = 3;
                            opdb.SaveChanges();
                        }
                    }
                    catch { } 
                    #endregion

                    isSaved = "true";
                }
                catch
                {
                    isSaved = "false";
                }
                
            }

            return isSaved;

        }

    }
}