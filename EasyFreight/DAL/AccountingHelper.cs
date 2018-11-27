using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using AutoMapper;
using System.Transactions;
using System.Data.Entity.Validation;
using Newtonsoft.Json.Linq;

namespace EasyFreight.DAL
{
    public static class AccountingHelper
    {
        /// <summary>
        /// Get complete operation cost .. if has many HB .. will get the sum foreach on
        /// The will contains operation, trucking, custom clearance cost
        /// </summary>
        /// <param name="operationId">operation Id</param>
        /// <returns>OperationCostAccMainVm</returns>
        public static OperationCostAccMainVm GetOperationCost(int operationId, int hbId = 0, string langCode = "en")
        {
            OperationCostAccMainVm operCostMainVm;
            OperationsEntities db = new OperationsEntities();
            if (hbId == 0)
            {
                operCostMainVm = db.OperationViews
               .Where(x => x.OperationId == operationId)
               .Select(x => new OperationCostAccMainVm
               {
                   BookingNumber = x.BookingNumber,
                   CarrierName = x.CarrierNameEn,
                   FromPort = x.FromPort,
                   ConsigneeName = x.ConsigneeNameEn,
                   MBL = x.MBL,
                   OperationCode = x.OperationCode,
                   OperationId = x.OperationId,
                   ShipperName = x.ShipperNameEn,
                   ToPort = x.ToPort,
                   VesselName = x.VesselName,
                   AgentName = x.AgentNameEn,
                   OrderFrom = x.OrderFrom == 1 ? "Export" : "Import"

               }).FirstOrDefault();

                var hbList = db.HouseBills.Where(x => x.OperationId == operationId).Select(x => x.HouseBL).ToArray();
                operCostMainVm.HouseBL = string.Join(" , ", hbList);
            }
            else
            {
                operCostMainVm = db.HouseBillViews
                    .Where(x => x.HouseBillId == hbId)
                    .Select(x => new OperationCostAccMainVm
                    {
                        BookingNumber = x.BookingNumber,
                        CarrierName = x.CarrierNameEn,
                        FromPort = x.FromPort,
                        ConsigneeName = x.CarrierNameEn,
                        MBL = x.MBL,
                        OperationCode = x.OperationCode,
                        OperationId = x.OperationId,
                        ShipperName = x.ShipperNameEn,
                        ToPort = x.ToPort,
                        VesselName = x.VesselName,
                        AgentName = x.AgentNameEn,
                        OrderFrom = x.OrderFrom == 1 ? "Export" : "Import",
                        HouseBL = x.HouseBL

                    }).FirstOrDefault();
            }


            operCostMainVm.ContainerSummary = OperationHelper.GetContainersSummary(operCostMainVm.OperationId);

            dynamic operCostList;
            //Get operationcost
            if (hbId == 0)
            {
                operCostList = db.OperationCosts.Include("OperationCostLib").Include("Currency")
                 .Where(x => x.OperationId == operationId)
                 .GroupBy(g => new
                 {
                     g.OperationCostLib.OperCostNameEn,
                     g.Currency.CurrencySign,
                     g.IsAgentCost,
                     g.CurrencyId,
                     g.OperCostLibId,
                     g.CurrencyIdSelling,
                     g.HouseBillId,
                     sellingCurrSign = g.Currency1.CurrencySign
                 })
                 .Select(g => new
                 {
                     OperCostNameEn = g.Key.OperCostNameEn,
                     OperationCostNet = g.Sum(x => x.OperationCostNet),
                     OperationCostSelling = g.Sum(x => x.OperationCostSelling),
                     CurrencySign = g.Key.CurrencySign,
                     IsAgentCost = g.Key.IsAgentCost,
                     CurrencyId = g.Key.CurrencyId,
                     OperCostId = g.Key.OperCostLibId,
                     CurrencyIdSelling = g.Key.CurrencyIdSelling,
                     HouseBillId=g.Key.HouseBillId,
                     CurrencySignSelling = g.Key.sellingCurrSign
                 })
                 .ToList();
            }
            else
            {
                //Will not get agent cost .. check with operation
                operCostList = db.OperationCosts.Include("OperationCostLibs")
                    .Include("Currency")
                    .Include("Currency1")
                  .Where(x => x.HouseBillId == hbId && x.IsAgentCost == false)
                  .Select(g => new
                  {
                      OperCostNameEn = g.OperationCostLib.OperCostNameEn,
                      OperationCostNet = g.OperationCostNet,
                      OperationCostSelling = g.OperationCostSelling,
                      CurrencySign = g.Currency.CurrencySign,
                      IsAgentCost = g.IsAgentCost,
                      CurrencyId = g.CurrencyId,
                      OperCostId = g.OperCostId,
                      CurrencyIdSelling = g.CurrencyIdSelling,
                      CurrencySignSelling = g.Currency1.CurrencySign,
                      HouseBillId = g.HouseBillId

                  })
                  .ToList();
            }

            OperationCostAccVm operCostAccVm;
            int i = 1;
            foreach (var item in operCostList)
            {
                operCostAccVm = new OperationCostAccVm();
                operCostAccVm.Id = i;
                operCostAccVm.CostName = item.OperCostNameEn;
                operCostAccVm.CurrencySign = item.CurrencySign;
                operCostAccVm.IsAgent = item.IsAgentCost;
                operCostAccVm.NetRate = Math.Round(item.OperationCostNet, 2);
                operCostAccVm.SellingRate = Math.Round(item.OperationCostSelling, 2);
                operCostAccVm.CurrencyId = item.CurrencyId;
                operCostAccVm.FkType = Convert.ToByte(CostType.OperationCost);
                operCostAccVm.CostFkId = item.OperCostId;
                operCostAccVm.CurrencyIdSelling = item.CurrencyIdSelling;
                operCostAccVm.CurrencySignSelling = item.CurrencySignSelling;
                operCostAccVm.HouseBillId = item.HouseBillId;

                i = i + 1;
                operCostMainVm.OperationCostAccVms.Add(operCostAccVm);
            }

            //Get trucking cost
            dynamic truckingCost;
            if (hbId == 0)
            {
                EasyFreightEntities db1 = new EasyFreightEntities();
                truckingCost = db1.TruckingOrderCosts.Include("TruckingCostLibs").Include("Currency")
                   .Where(x => x.TruckingOrderDetail.TruckingOrder.OperationId == operationId)
                   .GroupBy(g => new { g.TruckingCostLib.TruckingCostName, g.Currency.CurrencySign, g.CurrencyId, g.TruckingCostId })
                   .Select(g => new
                   {
                       NetRate = g.Sum(x => x.TruckingCostNet),
                       SellingRate = g.Sum(x => x.TruckingCostSelling),
                       CostName = g.Key.TruckingCostName,
                       CurrencySign = g.Key.CurrencySign,
                       CurrencyId = g.Key.CurrencyId,
                       TruckingOrderCostId = g.Key.TruckingCostId

                   }).ToList();
            }
            else
            {
                EasyFreightEntities db1 = new EasyFreightEntities();
                truckingCost = db1.TruckingOrderCosts.Include("TruckingCostLibs").Include("Currency")
                   .Where(x => x.TruckingOrderDetail.TruckingOrder.HouseBillId == hbId)
                   .Select(g => new
                   {
                       NetRate = g.TruckingCostNet,
                       SellingRate = g.TruckingCostSelling,
                       CostName = g.TruckingCostLib.TruckingCostName,
                       CurrencySign = g.Currency.CurrencySign,
                       CurrencyId = g.CurrencyId,
                       TruckingOrderCostId = g.TruckingOrderCostId

                   }).ToList();
            }


            foreach (var item in truckingCost)
            {
                operCostAccVm = new OperationCostAccVm();
                operCostAccVm.Id = i;
                operCostAccVm.CostName = item.CostName;
                operCostAccVm.CurrencySign = item.CurrencySign;
                operCostAccVm.NetRate = Math.Round(item.NetRate, 2);
                operCostAccVm.SellingRate = Math.Round(item.SellingRate, 2);
                operCostAccVm.CurrencyId = item.CurrencyId;
                operCostAccVm.FkType = Convert.ToByte(CostType.TruckingCost);
                operCostAccVm.CostFkId = item.TruckingOrderCostId;
                operCostAccVm.CurrencyIdSelling = item.CurrencyId;
                operCostAccVm.CurrencySignSelling = item.CurrencySign;
                i = i + 1;
                operCostMainVm.OperationCostAccVms.Add(operCostAccVm);
            }

            //Get CC cost
            dynamic ccCost;
            if (hbId == 0)
            {
                ccCost = db.CustomClearanceDetails.Include("CustomClearanceCostLibs").Include("Currency")
               .Where(x => x.CustomClearanceOrder.OperationId == operationId)
               .GroupBy(g => new { g.CustomClearanceCostLib.CostNameEn, g.Currency.CurrencySign, g.CurrencyId, g.CCCostId })
               .Select(g => new
               {
                   NetRate = g.Sum(x => x.CCCostNet),
                   SellingRate = g.Sum(x => x.CCCostSelling),
                   CostName = g.Key.CostNameEn,
                   CurrencySign = g.Key.CurrencySign,
                   CurrencyId = g.Key.CurrencyId,
                   CCDetailsId = g.Key.CCCostId
               });
            }
            else
            {
                ccCost = db.CustomClearanceDetails.Include("CustomClearanceCostLibs").Include("Currency")
               .Where(x => x.CustomClearanceOrder.HouseBillId == hbId)
               .Select(g => new
               {
                   NetRate = g.CCCostNet,
                   SellingRate = g.CCCostSelling,
                   CostName = g.CustomClearanceCostLib.CostNameEn,
                   CurrencySign = g.Currency.CurrencySign,
                   CurrencyId = g.CurrencyId,
                   CCDetailsId = g.CCDetailsId
               });
            }


            foreach (var item in ccCost)
            {
                operCostAccVm = new OperationCostAccVm();
                operCostAccVm.Id = i;
                operCostAccVm.CostName = item.CostName;
                operCostAccVm.CurrencySign = item.CurrencySign;
                operCostAccVm.NetRate = Math.Round(item.NetRate, 2);
                operCostAccVm.SellingRate = Math.Round(item.SellingRate, 2);
                operCostAccVm.CurrencyId = item.CurrencyId;
                operCostAccVm.FkType = Convert.ToByte(CostType.CCCost);
                operCostAccVm.CostFkId = item.CCDetailsId;
                operCostAccVm.CurrencyIdSelling = item.CurrencyId;
                operCostAccVm.CurrencySignSelling = item.CurrencySign;
                i = i + 1;
                operCostMainVm.OperationCostAccVms.Add(operCostAccVm);
            }

            //Totals
            var totalList = operCostMainVm.OperationCostAccVms.GroupBy(g => new { g.CurrencySign })
                .Select(g => new
                {
                    CurrencySign = g.Key.CurrencySign,
                    TotalNetRate = g.Sum(x => x.NetRate),
                    CurrencyId = g.Where(x => x.CurrencySign == g.Key.CurrencySign).FirstOrDefault().CurrencyId
                }).ToList();

            var totalListSelling = operCostMainVm.OperationCostAccVms.GroupBy(g => g.CurrencySignSelling)
                .Select(g => new
                {
                    CurrencySign = g.Key,
                    TotalSellingRate = g.Sum(x => x.SellingRate),
                    TotalAgentRate = g.Where(x => x.IsAgent).Sum(x => x.SellingRate),
                    CurrencyId = g.Where(x => x.CurrencySignSelling == g.Key).FirstOrDefault().CurrencyIdSelling
                }).ToList();

            Dictionary<int,string> usedCurrency = new Dictionary<int,string>();
            usedCurrency = totalList.Select(c=> new {key = c.CurrencyId ,value=c.CurrencySign}).Distinct().ToDictionary(x=>x.key,x=> x.value) ;
            foreach (var item in totalListSelling)
	        {
                if(!usedCurrency.ContainsKey(item.CurrencyId))
                 usedCurrency.Add(item.CurrencyId,item.CurrencySign);		 
            }           
            // usedCurrency.(totalListSelling.Select(c => c.CurrencyId).Distinct().ToList());
            //usedCurrency = usedCurrency.Distinct().ToList();

             
            OperationCostTotalAccVm _operTotalsVm;
            foreach (var item in usedCurrency)
            {
                var sellingItem = totalListSelling.Where(x => x.CurrencyId == item.Key).FirstOrDefault();
                var netItem = totalList.Where(x => x.CurrencyId == item.Key).FirstOrDefault();

                _operTotalsVm = new OperationCostTotalAccVm();
                _operTotalsVm.CurrencySign = netItem == null ? item.Value : netItem.CurrencySign;
                _operTotalsVm.TotalNetRate = netItem == null ? 0 : netItem.TotalNetRate;
                _operTotalsVm.TotalSellingRate = sellingItem == null ? 0 : sellingItem.TotalSellingRate;
                _operTotalsVm.TotalAgentRate = sellingItem == null ? 0 : sellingItem.TotalAgentRate;
                _operTotalsVm.CurrencyId = netItem == null ? item.Key : netItem.CurrencyId;

                operCostMainVm.OperationCostTotalAccVms.Add(_operTotalsVm);

            }


            //OperationCostTotalAccVm operTotalsVm;
            //int currId;
            //foreach (var item in totalList)
            //{
            //    currId = item.CurrencyId;
            //    var sellingItem = totalListSelling.Where(x => x.CurrencyId == currId).FirstOrDefault();
            //    operTotalsVm = new OperationCostTotalAccVm();
            //    operTotalsVm.CurrencySign = item.CurrencySign;
            //    operTotalsVm.TotalNetRate = item.TotalNetRate;
            //    operTotalsVm.TotalSellingRate = sellingItem == null ? 0 : sellingItem.TotalSellingRate;
            //    operTotalsVm.TotalAgentRate = sellingItem  == null ? 0 : sellingItem.TotalAgentRate;
            //    operTotalsVm.CurrencyId = item.CurrencyId;

            //    operCostMainVm.OperationCostTotalAccVms.Add(operTotalsVm);
            //}

            return operCostMainVm;
        }

        /// <summary>
        /// Add accounting transaction the transaction table
        /// </summary>
        /// <param name="transVm">AccTransactionVm</param>
        /// <returns>The Id for the added trans</returns>
        public static int AddTransaction(AccTransactionVm transVm)
        {
            int transId = 0;
            AccountingEntities db = new AccountingEntities();

            AccTransaction transDb = new AccTransaction();

            Mapper.CreateMap<AccTransactionVm, AccTransaction>().IgnoreAllNonExisting();
            Mapper.CreateMap<AccTransactionDetailVm, AccTransactionDetail>().IgnoreAllNonExisting();
            Mapper.Map(transVm, transDb);

            db.AccTransactions.Add(transDb);

            try
            {
                db.SaveChanges();
                transId = transDb.TransId;
            }
            catch
            {
                throw;
            }
            return transId;
        }

        public static OpenBalanceVm GetOpenBalanceInfo(string tbName, int libId, string pkName)
        {
            OpenBalanceVm balanceVm = new OpenBalanceVm();
            balanceVm.TbName = tbName;
            balanceVm.LibItemId = libId;
            balanceVm.CreateDate = DateTime.Now;
            balanceVm.IsCreditAgent = false;

            string accountId = "";
            accountId = AccountingChartHelper.GetAccountIdByPkAndTbName(libId, tbName, pkName);

            AccountingEntities db = new AccountingEntities();

            //Get Transaction details for open balance
            var accTanDet = db.AccTransactionDetails.Include("AccTransaction")
                .Where(x => x.AccountId == accountId && x.AccTransaction.TransactionName == "open balance").ToList();

            if (accTanDet.Count > 0)
                balanceVm.TransId = accTanDet.FirstOrDefault().TransId;

            int? bankAccCurrId = null;
            //For bank Acc will get only the currency for this bank
            if (tbName == "BankAccount")
            {
                bankAccCurrId = db.BankAccounts.Where(x => x.BankAccId == libId).FirstOrDefault().CurrencyId;
            }
            //Get Currency List
            // Dictionary<int, string> currList = ListCommonHelper.GetCurrencyList();
            List<CurrencyAcc> currList;
            if (bankAccCurrId == null)
                currList = db.CurrencyAccs.ToList();
            else
                currList = db.CurrencyAccs.Where(x => x.CurrencyId == bankAccCurrId).ToList();
            OpenBalanceDetailVm openBalanceDet;

            foreach (var item in currList)
            {
                openBalanceDet = new OpenBalanceDetailVm();
                openBalanceDet.CurrencyId = item.CurrencyId;
                openBalanceDet.CurrencySign = item.CurrencySign;
                openBalanceDet.CurrencyAccountId = item.AccountId;
                openBalanceDet.LibItemId = libId;
                //check if has open balance for this currency
                var openAccTran = accTanDet.Where(x => x.CurrencyId == item.CurrencyId).FirstOrDefault();
                if (openAccTran != null)
                {
                    openBalanceDet.TransDetailId = openAccTran.TransDetailId;
                    openBalanceDet.CreditAmount = openAccTran.CreditAmount;
                    openBalanceDet.DebitAmount = openAccTran.DebitAmount;
                    openBalanceDet.TransId = openAccTran.TransId;
                }

                balanceVm.OpenBalanceDetails.Add(openBalanceDet);
            }

            return balanceVm;
        }

        //public static OpenBalanceVm GetOpenBalanceForCash()
        //{
        //    OpenBalanceVm balanceVm = new OpenBalanceVm();
        //    balanceVm.TbName = "Currency";
        //    balanceVm.LibItemId = 0;
        //    balanceVm.CreateDate = DateTime.Now;
        //    balanceVm.IsCreditAgent = false;

        //    AccountingEntities db = new AccountingEntities();

        //    //Get Currency List
        //    var currList = db.CurrencyAccs.ToList();
        //    OpenBalanceDetailVm openBalanceDet;

        //    foreach (var item in currList)
        //    {
        //        string accountId = AccountingChartHelper.GetAccountIdByPkAndTbName(item.CurrencyId, "Currency", "CurrencyId");
        //        //Get Transaction details for open balance
        //        var accTanDet = db.AccTransactionDetails.Include("AccTransaction")
        //            .Where(x => x.AccountId == accountId && x.AccTransaction.TransactionName == "open balance").ToList();
        //        openBalanceDet = new OpenBalanceDetailVm();
        //        openBalanceDet.CurrencyId = item.CurrencyId;
        //        openBalanceDet.CurrencySign = item.CurrencySign;
        //        openBalanceDet.CurrencyAccountId = item.AccountId;
        //        //check if has open balance for this currency
        //        var openAccTran = accTanDet.Where(x => x.CurrencyId == item.CurrencyId).FirstOrDefault();
        //        if (openAccTran != null)
        //        {
        //            openBalanceDet.TransDetailId = openAccTran.TransDetailId;
        //            openBalanceDet.CreditAmount = openAccTran.CreditAmount;
        //            openBalanceDet.DebitAmount = openAccTran.DebitAmount;
        //        }

        //        balanceVm.OpenBalanceDetails.Add(openBalanceDet);
        //    }



        //    return balanceVm;
        //}

        public static string AddEditOpenBalance(OpenBalanceVm openBalanceVm)
        {
            string isSaved = "true";

            string accountId = "";
            string tbName, pkName; int libId; bool isCreditAgent = false;

            libId = openBalanceVm.LibItemId;
            tbName = openBalanceVm.TbName;
            pkName = openBalanceVm.PkName;
            if (openBalanceVm.IsCreditAgent != null)
                isCreditAgent = openBalanceVm.IsCreditAgent.Value;

            //Get AccountId
            accountId = AccountingChartHelper.GetAccountIdByPkAndTbName(libId, tbName, pkName);
            if (string.IsNullOrEmpty(accountId))
            {
                switch (tbName)
                {
                    case "Agent":
                        byte agentType;
                        if (isCreditAgent == false)
                            agentType = 1; //debit note A/R
                        else
                            agentType = 2; //Credit note A/P

                        accountId = AccountingChartHelper.AddAgentToChart(libId, agentType);
                        break;
                    case "Carrier":
                        accountId = AccountingChartHelper.AddCarrierToChart(libId);
                        break;
                    case "Contractor":
                        accountId = AccountingChartHelper.AddContractorToChart(libId);
                        break;
                    case "Shipper":
                        accountId = AccountingChartHelper.AddShipperToChart(libId);
                        break;
                    case "Consignee":
                        accountId = AccountingChartHelper.AddConsigneeToChart(libId);
                        break;
                    case "Currency":
                        accountId = AccountingChartHelper.AddCashToChart(libId);
                        break;
                    case "BankAccount":
                        accountId = AccountingChartHelper.AddBankAccountToChart(0, libId);
                        break;
                    case "AccountingChart":
                        accountId = AccountingChartHelper.AddBankAccountToChart(0, libId);
                        break;

                }

                AccountingChartHelper.AddAccountIdToObj(accountId, tbName, libId, pkName);
            }

            openBalanceVm.AccountId = accountId;

            int transId = openBalanceVm.TransId;
            AccTransaction accTran;

            AccountingEntities db = new AccountingEntities();

            if (transId != 0)
            {
                accTran = db.AccTransactions.Include("AccTransactionDetails").Where(x => x.TransId == transId).FirstOrDefault();
                //delete all tran details and add it later
                foreach (var item in accTran.AccTransactionDetails.ToList())
                {
                    db.AccTransactionDetails.Remove(item);
                }
            }
            else
            {
                accTran =
                    new AccTransaction()
                    {
                        CreateBy = AdminHelper.GetCurrentUserId(),
                        CreateDate = DateTime.Now,
                        TransactionName = "open balance"

                    };
            }

            Mapper.CreateMap<OpenBalanceDetailVm, AccTransactionDetail>();
            AccTransactionDetail accTranDetail;
            foreach (var item in openBalanceVm.OpenBalanceDetails.Where(x => x.DebitAmount != null || x.CreditAmount != null))
            {
                accTranDetail = new AccTransactionDetail();

                item.AccountId = accountId;
                Mapper.Map(item, accTranDetail);
                accTran.AccTransactionDetails.Add(accTranDetail);
            }

            if (transId == 0)
                db.AccTransactions.Add(accTran);

            try
            {
                db.SaveChanges();
            }

            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.Message;
            }
            return isSaved;
        }


        /// <summary>
        /// Delete accounting transaction the transaction table
        /// </summary>
        /// <param name="transID"></param>
        /// <returns></returns>
        public static bool DeleteTransaction(int transID)
        {
            bool isDeleted = false;
            AccountingEntities db = new AccountingEntities();

            AccTransaction transDb = new AccTransaction();
            List<AccTransactionDetail> transDetailDb = new List<AccTransactionDetail>();

            transDb = db.AccTransactions.Where(x => x.TransId == transID).FirstOrDefault();
            if (transDb != null)
            {
                transDetailDb = db.AccTransactionDetails.Where(x => x.TransId == transID).ToList();
                try
                {
                    db.AccTransactions.Remove(transDb);
                    foreach (var item in transDetailDb)
                    {
                        db.AccTransactionDetails.Remove(item);
                    }
                    db.SaveChanges();
                    isDeleted = true;
                }
                catch
                {
                    isDeleted = false;
                }
            }
            return isDeleted;
        }

        public static void GetCCCashDepositCostList(int operationId)
        {
            var operCostObj = AccountingHelper.GetOperationCost(operationId, 0);
            var operCostList = operCostObj.OperationCostAccVms;
            var operCostTotalList = operCostObj.OperationCostTotalAccVms;
            Dictionary<int, string> ccCost = ListCommonHelper.GetCustClearCostList();
        }


        internal static JObject GetOpenBalanceListJson( )
        {
            var openBalanceList = OpenBalancePaymentModels.Instance.GetOpenBalancePaymentVm();
            
            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();

            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in openBalanceList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("AccountId");
                pJTokenWriter.WriteValue(item.AccountId);

                pJTokenWriter.WritePropertyName("CurrencyId");
                pJTokenWriter.WriteValue(item.CurrencyId);

                pJTokenWriter.WritePropertyName("DebitAmount");
                pJTokenWriter.WriteValue(item.DebitAmount);//item.IsCredit==1?item.CreditAmount: item.DebitAmount );

                pJTokenWriter.WritePropertyName("CreditAmount");
                pJTokenWriter.WriteValue(item.CreditAmount);//item.IsCredit == 1 ? item.DebitAmount : item.CreditAmount);

                pJTokenWriter.WritePropertyName("Amount");
                pJTokenWriter.WriteValue(item.DebitAmount - item.CreditAmount);

                //if (item.IsCredit == 1)
                //{
                //    tempAmount = item.DebitAmount - item.CreditAmount;
                //    pJTokenWriter.WriteValue(tempAmount); //pJTokenWriter.WriteValue(item.Amount * -1);
                //}
                //else
                //{
                //    tempAmount = item.CreditAmount - item.DebitAmount  ;
                //    pJTokenWriter.WriteValue(tempAmount); //pJTokenWriter.WriteValue(item.Amount);
                //}

                pJTokenWriter.WritePropertyName("IsCredit");
                pJTokenWriter.WriteValue(item.IsCredit);

                pJTokenWriter.WritePropertyName("AccountNameEn");
                pJTokenWriter.WriteValue(item.AccountNameEn );

                pJTokenWriter.WritePropertyName("AccountNameAr");
                pJTokenWriter.WriteValue(item.AccountNameAr);

                pJTokenWriter.WritePropertyName("CurrencyName");
                pJTokenWriter.WriteValue(item.CurrencyName);

                pJTokenWriter.WritePropertyName("CurrencySign");
                pJTokenWriter.WriteValue(item.CurrencySign); 


                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static OpenBalancePaymentVm Get_OpenBalanceObject(string accid, int cid)
        {
             return OpenBalancePaymentModels.Instance.Get_OpenBalanceObject(accid, cid);
         }

    }
}