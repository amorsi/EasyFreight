using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;

namespace EasyFreight.DAL
{
    public static class APInvoiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operId"></param>
        /// <param name="invId"></param>
        /// <param name="invFor">if = 1  Invoice for Carrier .. if 2 invoice for Contractor .. if 3 custom clearance</param>
        /// <param name="forEdit"></param>
        /// <returns></returns>
        public static InvoiceVm GetInvoiceInfo(int hbId = 0, int invId = 0, byte invFor = 1, bool forEdit = true)
        {
            InvoiceVm invoiceVm = new InvoiceVm(invFor);
            AccountingEntities db = new AccountingEntities();

            if (invId != 0)
            {
                InvoiceAP invDb = db.InvoiceAPs.Include("InvoiceDetailAPs").Include("InvoiceTotalAPs")
                    .Where(x => x.InvoiceId == invId && x.IsDeleted == null).FirstOrDefault();
                Mapper.CreateMap<InvoiceAP, InvoiceVm>()
                    .IgnoreAllNonExisting();

                Mapper.Map(invDb, invoiceVm);

                Mapper.CreateMap<InvoiceDetailAP, InvoiceDetailVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<InvoiceTotalAP, InvoiceTotalVm>().IgnoreAllNonExisting();

                InvoiceDetailVm invDetVm;
                foreach (var item in invDb.InvoiceDetailAPs)
                {
                    invDetVm = new InvoiceDetailVm();
                    Mapper.Map(item, invDetVm);
                    invoiceVm.InvoiceDetails.Add(invDetVm);
                }

                InvoiceTotalVm invTotalVm;
                foreach (var item in invDb.InvoiceTotalAPs)
                {
                    invTotalVm = new InvoiceTotalVm();
                    Mapper.Map(item, invTotalVm);
                    invoiceVm.InvoiceTotals.Add(invTotalVm);
                }


                invoiceVm.PaymentTermName = invDb.PaymentTerm.PaymentTermEn;
                invoiceVm.CurrencySign = invDb.Currency.CurrencySign;
            }
            else
                invoiceVm.InvoiceCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.APInvoice, false);

            if (hbId == 0)
                hbId = invoiceVm.HouseBillId.Value;

            OperationsEntities db1 = new OperationsEntities();
            var hbInfo = db1.HouseBillViews.Where(x => x.HouseBillId == hbId).FirstOrDefault();

            Mapper.CreateMap<HouseBillView, InvoiceVm>()
            .ForMember(x => x.CreateBy, y => y.Ignore())
            .ForMember(x => x.CreateDate, y => y.Ignore())
            .IgnoreAllNonExisting();
            Mapper.Map(hbInfo, invoiceVm);

            if (invFor == 1) //Carrier
            {
                invoiceVm.ContractorId = null;
                invoiceVm.SupplierName = hbInfo.CarrierNameEn;
                invoiceVm.CustomerName = hbInfo.CarrierNameEn;
            }
            else if (invFor == 2)
            {
                var contractorInfo = db1.TruckingOrdersViews
                    .Where(x => x.HouseBillId == hbId)
                    .Select(x => new { x.ContractorId, x.ContractorNameEn })
                    .FirstOrDefault();
                if (contractorInfo != null)
                {
                    invoiceVm.CarrierId = null;
                    invoiceVm.SupplierName = contractorInfo.ContractorNameEn;
                    invoiceVm.ContractorId = contractorInfo.ContractorId;
                    invoiceVm.CustomerName = contractorInfo.ContractorNameEn;
                }
            }

            //Get Cost list 
            var operCostObj = AccountingHelper.GetOperationCost(invoiceVm.OperationId, invoiceVm.HouseBillId.Value);

            var operCostList = operCostObj.OperationCostAccVms;
            var operCostTotalList = operCostObj.OperationCostTotalAccVms;

            // GetHbInvTotal(0, ref operCostList, ref operCostTotalList);

            invoiceVm.OperationCostAccVms = operCostList;
            invoiceVm.OperationCostTotalAccVms = operCostTotalList;

            if (forEdit)
            {
                //Get prev invoice details for this HB
                // adding condation  [x.Invoice.IsDeleted==null] get undeleted invoices
                var prevInvDetails = db.InvoiceDetailAPs
                    .Where(x => x.InvoiceAP.HouseBillId == hbId && x.InvoiceAP.IsDeleted == null)
                    .Select(x => new { x.CostFkId, x.FkType })
                    .ToList();

                InvoiceDetailVm invDetailVm;
                //invFor == 1 ..get operation cost only
                //invFor == 2 ..get trucking cost only
                List<int> usedCost = invoiceVm.InvoiceDetails.Where(x => x.FkType == invFor).Select(x => x.CostFkId).ToList();
                usedCost.AddRange(prevInvDetails.Where(x => x.FkType == invFor).Select(x => x.CostFkId).ToList());
                var newCosts = operCostList.Where(x => !usedCost.Contains(x.CostFkId) && x.FkType == invFor).ToList();

                foreach (var item in newCosts)
                {
                    invDetailVm = new InvoiceDetailVm()
                    {

                        CostFkId = item.CostFkId,
                        CostName = item.CostName,
                        ExchangeRate = 1,
                        FkType = item.FkType,
                        InvDetailId = 0,
                        InvoiceAmount = item.NetRate,
                        InvoiceId = invoiceVm.InvoiceId,
                        MainAmount = item.NetRate,
                        MainCurrencyId = item.CurrencyIdSelling,
                        MainCurrSign = item.CurrencySignSelling
                    };

                    invoiceVm.InvoiceDetails.Add(invDetailVm);
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
                Dictionary<int, string> operCost = new Dictionary<int, string>();
                Dictionary<int, string> truckCost = new Dictionary<int, string>();
                Dictionary<int, string> ccCost = new Dictionary<int, string>();
                if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.OperationCost))
                    operCost = ListCommonHelper.GetOperationCostList();
                else if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.TruckingCost))
                    truckCost = ListCommonHelper.GetTruckingCostNameList();
                else if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.CCCost))
                    ccCost = ListCommonHelper.GetCustClearCostList();
                //Get cost name for invoice items
                foreach (var item in invoiceVm.InvoiceDetails)
                {
                    item.CostName = invoiceVm.OperationCostAccVms
                        .Where(x => x.CostFkId == item.CostFkId && x.FkType == item.FkType)
                        .FirstOrDefault().CostName;

                }
            }

            return invoiceVm;
        }

        internal static string AddEditInvoice(InvoiceVm invoiceVm)
        {
            string isSaved = "true";

            AccountingEntities db = new AccountingEntities();
            int invoiceId = invoiceVm.InvoiceId;
            byte invFor = invoiceVm.InvoiceType;
            //  int hbId = invoiceVm.HouseBillId;

            InvoiceAP invDb;
            if (invoiceId == 0)
                invDb = new InvoiceAP();
            else
            {
                invDb = db.InvoiceAPs.Include("InvoiceDetailAPs").Include("InvoiceTotalAPs")
                    .Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
                //Delete Invoice details and totals .. will insert it again
                var invDbTotals = invDb.InvoiceTotalAPs;
                var invDbDetails = invDb.InvoiceDetailAPs;
                foreach (var item in invDbDetails)
                {
                    invDb.InvoiceDetailAPs.Remove(item);
                }
                foreach (var item in invDbTotals)
                {
                    invDb.InvoiceTotalAPs.Remove(item);
                }
            }


            Mapper.CreateMap<InvoiceVm, InvoiceAP>()
                .ForMember(x => x.InvoiceTotalAPs, y => y.Ignore())
                .ForMember(x => x.InvoiceDetailAPs, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(invoiceVm, invDb);

            InvoiceDetailAP invDetail;
            Mapper.CreateMap<InvoiceDetailVm, InvoiceDetailAP>().IgnoreAllNonExisting();
            foreach (var item in invoiceVm.InvoiceDetails)
            {
                if (item.IsSelected == true)
                {
                    invDetail = new InvoiceDetailAP();
                    Mapper.Map(item, invDetail);
                    invDb.InvoiceDetailAPs.Add(invDetail);
                }
            }

            InvoiceTotalAP invTotalDb;
            foreach (var item in invoiceVm.InvoiceTotals)
            {
                invTotalDb = new InvoiceTotalAP()
                {
                    CurrencyId = item.CurrencyId,
                    TotalAmount = item.TotalAmount,
                    CurrencySign = item.CurrencySign

                };

                invDb.InvoiceTotalAPs.Add(invTotalDb);
            }



            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    //Add shipper or consignee to accounting chart
                    string creditAccountId = "";
                    if (invFor == 1) //carrier
                    {
                        creditAccountId = AccountingChartHelper
                            .GetAccountIdByPkAndTbName(invoiceVm.CarrierId.Value, "Carrier", "CarrierId");
                        if (string.IsNullOrEmpty(creditAccountId))
                            creditAccountId = AccountingChartHelper.AddCarrierToChart(invoiceVm.CarrierId.Value);
                    }
                    else if (invFor == 2) //Contractor
                    {
                        creditAccountId = AccountingChartHelper
                                .GetAccountIdByPkAndTbName(invoiceVm.ContractorId.Value, "Contractor", "ContractorId");
                        if (string.IsNullOrEmpty(creditAccountId))
                            creditAccountId = AccountingChartHelper.AddContractorToChart(invoiceVm.ContractorId.Value);
                    }
                    else if (invFor == 3) //Custom Clearance 
                    {
                        creditAccountId = AccountingChartHelper.GetCCCashDepAccountId(invoiceVm.OperationId);

                        if (!string.IsNullOrEmpty(creditAccountId))
                            invDb.InvStatusId = 4; //paid with cash deposit
                        else // In this case there was no cash deposit paid .. I added an account to sotre
                            //any credit amount for custom clearance
                            creditAccountId = ((int) AccountingChartEnum.CustomClearanceSupplier).ToString();

                    }

                    if (invoiceId == 0)
                    {
                        invDb.InvoiceCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.APInvoice, true);
                        db.InvoiceAPs.Add(invDb);
                    }

                    db.SaveChanges();

                    invoiceVm.InvoiceId = invDb.InvoiceId;
                    invoiceVm.InvoiceCode = invDb.InvoiceCode;

                    //Change HB status
                    if (invoiceId == 0)
                    {
                        //Add invoice to accounting transactions table
                        if (!string.IsNullOrEmpty(creditAccountId))
                            AddAPInvToTransTable(creditAccountId, invoiceVm);

                        //if (invFor == 3) //Custom Clearance 
                        if (invFor == 3) //will link the cash out receipts with same currency to this invoice as it is paid
                        {
                            int operId = invoiceVm.OperationId;
                            int savedInvId = invoiceVm.InvoiceId;
                            int currId = invoiceVm.InvCurrencyId;
                            decimal invAmount = invoiceVm.InvoiceTotals.FirstOrDefault().TotalAmount;
                            //Get cashout receiptIds
                            var cashoutReceiptIds = db.CashOutReceipts.Include("CashOutReceiptInvs")
                                .Where(x => x.OperationId == operId && x.CurrencyId == currId)
                                .ToList();

                            CashOutReceiptInv cashOutReceiptInv;
                            decimal prevPaidFromReceipt, receiptAmount, amountToPay = 0;

                            foreach (var item in cashoutReceiptIds)
                            {
                                receiptAmount = item.ReceiptAmount;

                                //Get what paid before by this receipt
                                prevPaidFromReceipt = item.CashOutReceiptInvs.Sum(x => x.PaidAmount);
                                amountToPay = receiptAmount - prevPaidFromReceipt;
                                if (amountToPay <= 0) //This receipt total amount is used by another invoices
                                    continue;

                                if (invAmount <= 0) //This Invoice is total paid
                                    break;

                                if (amountToPay > invAmount)
                                    amountToPay = invAmount;

                                //Add cash out receipt invoice
                                cashOutReceiptInv = new CashOutReceiptInv();
                                cashOutReceiptInv.ReceiptId = item.ReceiptId;
                                cashOutReceiptInv.InvoiceId = savedInvId;
                                cashOutReceiptInv.PaidAmount = amountToPay;

                                db.CashOutReceiptInvs.Add(cashOutReceiptInv);

                                invAmount = invAmount - amountToPay;
                            }

                            db.SaveChanges();
                        }
                        // HouseBillHelper.ChangeHBStatus(invDb.HouseBillId, (byte)StatusEnum.InvoiceIssued);
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

        private static void AddAPInvToTransTable(string creditAccountId, InvoiceVm invoiceVm)
        {
            string debitAccId = "";
            byte invFor = invoiceVm.InvoiceType;

            if (invFor == 1) //carrier
                debitAccId = ((int)AccountingChartEnum.CarrierCostOfSales).ToString();
            else if (invFor == 2) //Contractor
                debitAccId = ((int)AccountingChartEnum.TruckingCostOfSales).ToString();
            else if (invFor == 3) //Custom Clearance
                debitAccId = ((int)AccountingChartEnum.CCCostOfSales).ToString();


            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId(),
                TransactionName = "Invoice Number " + invoiceVm.InvoiceCode,
                TransactionNameAr = "فاتورة رقم " + invoiceVm.InvoiceCode
            };


            AccTransactionDetailVm accTransDetDebit = new AccTransactionDetailVm()
            {
                AccountId = debitAccId,
                CreditAmount = 0,
                CurrencyId = invoiceVm.InvCurrencyId,
                DebitAmount = invoiceVm.InvoiceTotals.FirstOrDefault().TotalAmount
            };
            accTrans.AccTransactionDetails.Add(accTransDetDebit);

            AccTransactionDetailVm accTransDetCredit = new AccTransactionDetailVm()
            {
                AccountId = creditAccountId,
                CreditAmount = invoiceVm.InvoiceTotals.FirstOrDefault().TotalAmount,
                CurrencyId = invoiceVm.InvCurrencyId,
                DebitAmount = 0
            };
            accTrans.AccTransactionDetails.Add(accTransDetCredit);

            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, "InvoiceAP", invoiceVm.InvoiceId, "InvoiceId");


        }

        internal static JObject GetInvListJson(System.Web.Mvc.FormCollection form)
        {
            AccountingEntities db = new AccountingEntities();
            APInvoiceView invViewDb = new APInvoiceView();
            string where = CommonHelper.AdvancedSearch<APInvoiceView>(form, invViewDb);
            if (string.IsNullOrEmpty(where))
                where = "1 = 1"; //instead of make if condition

            where = where + " and IsDeleted = null ";

            var invoiceList = db.APInvoiceViews.Where(where.ToString()).ToList();
            //change this to cashOut when done
            List<int> invIds = invoiceList.Select(x => x.InvoiceId).ToList();
            var cashInvDb = db.CashOutReceiptInvs
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

                pJTokenWriter.WritePropertyName("OperationId");
                pJTokenWriter.WriteValue(item.OperationId);

                pJTokenWriter.WritePropertyName("InvoiceCode");
                pJTokenWriter.WriteValue(item.InvoiceCode);

                pJTokenWriter.WritePropertyName("APInvoiceCode");
                pJTokenWriter.WriteValue(item.APInvoiceCode);

                pJTokenWriter.WritePropertyName("DueDate");
                pJTokenWriter.WriteValue(item.DueDate.Value.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("InvoiceDate");
                pJTokenWriter.WriteValue(item.InvoiceDate.ToString("dd/MM/yyyy"));

                switch (item.InvoiceType)
                {
                    case 1:
                        pJTokenWriter.WritePropertyName("SupplierName");
                        pJTokenWriter.WriteValue(item.CarrierNameEn);

                        pJTokenWriter.WritePropertyName("InvForImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-ship'></i>");

                        break;

                    case 2:
                        pJTokenWriter.WritePropertyName("SupplierName");
                        pJTokenWriter.WriteValue(item.ContractorNameEn);

                        pJTokenWriter.WritePropertyName("InvForImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-truck'></i>");
                        break;
                    case 3:
                        pJTokenWriter.WritePropertyName("SupplierName");
                        pJTokenWriter.WriteValue("Custom Clearance");

                        pJTokenWriter.WritePropertyName("InvForImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-cubes'></i>");
                        break;
                }

                pJTokenWriter.WritePropertyName("OrderFromImg");
                switch (item.OrderFrom)
                {
                    case 1:
                        pJTokenWriter.WriteValue("<i class='fa fa-level-up'></i>");
                        break;
                    case 2:
                        pJTokenWriter.WriteValue("<i class='fa fa-level-down'></i>");
                        break;
                }

                pJTokenWriter.WritePropertyName("OperationCode");
                pJTokenWriter.WriteValue(item.OperationCode);

                //pJTokenWriter.WritePropertyName("HouseBL");
                //pJTokenWriter.WriteValue(item.HouseBL);



                pJTokenWriter.WritePropertyName("TotalAmount");
                pJTokenWriter.WriteValue(item.TotalAmount + " (" + item.CurrencySign + ")");

                collectedAmount = cashInvDb.Where(x => x.Key == item.InvoiceId)
                   .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                amountDue = item.TotalAmount - collectedAmount;


                pJTokenWriter.WritePropertyName("AmountDue"); //Modify this after make payment
                pJTokenWriter.WriteValue(amountDue + " (" + item.CurrencySign + ")");

                pJTokenWriter.WritePropertyName("InvStatusId");
                pJTokenWriter.WriteValue(item.InvStatusId);

                pJTokenWriter.WritePropertyName("InvFor");
                pJTokenWriter.WriteValue(item.InvoiceType);

                pJTokenWriter.WritePropertyName("InvStatusName");
                pJTokenWriter.WriteValue(item.InvStatusNameEn);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        internal static List<InvoiceVm> GetInvoiceListForOper(int operId)
        {
            AccountingEntities db = new AccountingEntities();
            OperationsEntities db1 = new OperationsEntities();
            List<InvoiceVm> invVmList = new List<InvoiceVm>();
            List<InvoiceAP> invDbList = new List<InvoiceAP>();
            if (operId != 0)
                invDbList = db.InvoiceAPs
                    .Include("InvoiceDetailAPs")
                    .Include("InvoiceTotalAPs")
                    .Include("InvStatusLib").Where(x => x.OperationId == operId && x.IsDeleted == null).ToList();

            Mapper.CreateMap<InvoiceDetailAP, InvoiceDetailVm>();
            Mapper.CreateMap<InvoiceTotalAP, InvoiceTotalVm>();

            Mapper.CreateMap<InvoiceAP, InvoiceVm>()
                .ForMember(x => x.InvoiceDetails, y => y.MapFrom(c => c.InvoiceDetailAPs))
                .ForMember(x => x.InvoiceTotals, y => y.MapFrom(c => c.InvoiceTotalAPs));
            Mapper.Map(invDbList, invVmList);




            List<int> invIds = invVmList.Select(x => x.InvoiceId).ToList();
            var cashInvDb = db.CashOutReceiptInvs
                .Where(x => invIds.Contains(x.InvoiceId))
                .GroupBy(x => x.InvoiceId)
                .ToList();

            var apInvView = db.APInvoiceViews.Where(x => x.OperationId == operId)
                .Select(x => new
                {
                    x.InvoiceType,
                    x.CarrierNameEn,
                    x.ContractorNameEn,
                    x.InvoiceId
                }).ToList();

            foreach (var item in invVmList)
            {

                if (item.InvoiceType == 1)
                    item.CustomerName = apInvView.Where(x => x.InvoiceId == item.InvoiceId).FirstOrDefault().CarrierNameEn;
                else if (item.InvoiceType == 2)
                    item.CustomerName = apInvView.Where(x => x.InvoiceId == item.InvoiceId).FirstOrDefault().ContractorNameEn;
                else if (item.InvoiceType == 3)
                    item.CustomerName = "Custom Clearance";
                item.InvStatusName = invDbList.Where(x => x.InvoiceId == item.InvoiceId)
                    .FirstOrDefault().InvStatusLib.InvStatusNameEn;

                decimal collectedAmount = cashInvDb.Where(x => x.Key == item.InvoiceId)
                     .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                item.AmountDue = item.InvoiceTotals.FirstOrDefault().TotalAmount - collectedAmount;
            }

            return invVmList;
        }

        internal static string Delete(int invId, string deleteReason)
        {
            string isSaved = "false";

            if (invId == 0)
                return isSaved;

            AccountingEntities db = new AccountingEntities();

            InvoiceAP invDb = new InvoiceAP();
            invDb = db.InvoiceAPs
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
                var invTotal = db.InvoiceTotalAPs.Where(x => x.InvoiceId == invId).FirstOrDefault();

                try
                {

                    db.SaveChanges();

                    db.InvoiceTotalAPs.Remove(invTotal); 

                    if (transID.HasValue)
                        AccountingHelper.DeleteTransaction(transID.Value);


                    isSaved = "true";
                }
                catch
                {
                    isSaved = "false";
                }
            }

            return isSaved;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="operId"></param>
        /// <param name="invId"></param>
        /// <param name="invFor">if = 1  Invoice for Carrier .. if 2 invoice for Contractor .. if 3 custom clearance</param>
        /// <param name="forEdit"></param>
        /// <returns></returns>
        public static InvoiceVm GetInvoiceInfoFullOperation(int operId = 0, int invId = 0, byte invFor = 1, bool forEdit = true)
        {
            InvoiceVm invoiceVm = new InvoiceVm(invFor);
            AccountingEntities db = new AccountingEntities();

            if (invId != 0)
            {
                InvoiceAP invDb = db.InvoiceAPs.Include("InvoiceDetailAPs").Include("InvoiceTotalAPs")
                    .Where(x => x.InvoiceId == invId && x.IsDeleted==null).FirstOrDefault();
                Mapper.CreateMap<InvoiceAP, InvoiceVm>()
                    .IgnoreAllNonExisting();

                Mapper.Map(invDb, invoiceVm);

                Mapper.CreateMap<InvoiceDetailAP, InvoiceDetailVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<InvoiceTotalAP, InvoiceTotalVm>().IgnoreAllNonExisting();

                InvoiceDetailVm invDetVm;
                foreach (var item in invDb.InvoiceDetailAPs)
                {
                    invDetVm = new InvoiceDetailVm();
                    Mapper.Map(item, invDetVm);
                    invoiceVm.InvoiceDetails.Add(invDetVm);
                }

                InvoiceTotalVm invTotalVm;
                foreach (var item in invDb.InvoiceTotalAPs)
                {
                    invTotalVm = new InvoiceTotalVm();
                    Mapper.Map(item, invTotalVm);
                    invoiceVm.InvoiceTotals.Add(invTotalVm);
                }


                invoiceVm.PaymentTermName = invDb.PaymentTerm.PaymentTermEn;
                invoiceVm.CurrencySign = invDb.Currency.CurrencySign;
            }
            else
                invoiceVm.InvoiceCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.APInvoice, false);

            if (operId == 0)
                operId = invoiceVm.OperationId;

            OperationsEntities db1 = new OperationsEntities();
            var hbInfo = db1.OperationViews.Where(x => x.OperationId == operId).FirstOrDefault();

            Mapper.CreateMap<OperationView, InvoiceVm>()
            .ForMember(x => x.CreateBy, y => y.Ignore())
            .ForMember(x => x.CreateDate, y => y.Ignore())
            .IgnoreAllNonExisting();
            Mapper.Map(hbInfo, invoiceVm);

            if (invFor == 1) //Carrier
            {
                invoiceVm.ContractorId = null;
                invoiceVm.SupplierName = hbInfo.CarrierNameEn;
                invoiceVm.CustomerName = hbInfo.CarrierNameEn;
            }
            else if (invFor == 2)
            {
                var contractorInfo = db1.TruckingOrdersViews
                    .Where(x => x.OperationId == operId)
                    .Select(x => new { x.ContractorId, x.ContractorNameEn })
                    .FirstOrDefault();
                if (contractorInfo != null)
                {
                    invoiceVm.CarrierId = null;
                    invoiceVm.SupplierName = contractorInfo.ContractorNameEn;
                    invoiceVm.ContractorId = contractorInfo.ContractorId;
                    invoiceVm.CustomerName = contractorInfo.ContractorNameEn;
                }
            }
            else if (invFor == 3)
            {
                invoiceVm.SupplierName = "Custom Clearance";
                invoiceVm.CustomerName = "Custom Clearance";
            }

            //Get Cost list 
            var operCostObj = AccountingHelper.GetOperationCost(invoiceVm.OperationId, 0);

            var operCostList = operCostObj.OperationCostAccVms;
            var operCostTotalList = operCostObj.OperationCostTotalAccVms;

            // GetHbInvTotal(0, ref operCostList, ref operCostTotalList);

            invoiceVm.OperationCostAccVms = operCostList;
            invoiceVm.OperationCostTotalAccVms = operCostTotalList;

            if (forEdit)
            {
                //Get prev invoice details for this HB
                var prevInvDetails = db.InvoiceDetailAPs
                    .Where(x => x.InvoiceAP.OperationId == operId && x.InvoiceAP.IsDeleted==null)
                    .Select(x => new { x.CostFkId, x.FkType })
                    .ToList();

                InvoiceDetailVm invDetailVm;
                //invFor == 1 ..get operation cost only
                //invFor == 2 ..get trucking cost only
                //invFor == 3 ..get custom clearance cost only
                List<int> usedCost = invoiceVm.InvoiceDetails.Where(x => x.FkType == invFor).Select(x => x.CostFkId).ToList();
                usedCost.AddRange(prevInvDetails.Where(x => x.FkType == invFor).Select(x => x.CostFkId).ToList());
                var newCosts = operCostList.Where(x => !usedCost.Contains(x.CostFkId) && x.FkType == invFor).ToList();

                foreach (var item in newCosts.Where(x => x.NetRate > 0))
                {
                    invDetailVm = new InvoiceDetailVm()
                    {

                        CostFkId = item.CostFkId,
                        CostName = item.CostName,
                        ExchangeRate = 1,
                        FkType = item.FkType,
                        InvDetailId = 0,
                        InvoiceAmount = item.NetRate,
                        InvoiceId = invoiceVm.InvoiceId,
                        MainAmount = item.NetRate,
                        MainCurrencyId = item.CurrencyId,
                        MainCurrSign = item.CurrencySign
                    };

                    invoiceVm.InvoiceDetails.Add(invDetailVm);
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
                Dictionary<int, string> operCost = new Dictionary<int, string>();
                Dictionary<int, string> truckCost = new Dictionary<int, string>();
                Dictionary<int, string> ccCost = new Dictionary<int, string>();
                if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.OperationCost))
                    operCost = ListCommonHelper.GetOperationCostList();
                else if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.TruckingCost))
                    truckCost = ListCommonHelper.GetTruckingCostNameList();
                else if (invoiceVm.InvoiceDetails.Any(x => x.FkType == (byte)CostType.CCCost))
                    ccCost = ListCommonHelper.GetCustClearCostList();
                //Get cost name for invoice items
                foreach (var item in invoiceVm.InvoiceDetails)
                {
                    item.CostName = invoiceVm.OperationCostAccVms
                        .Where(x => x.CostFkId == item.CostFkId && x.FkType == item.FkType)
                        .FirstOrDefault().CostName;

                }
            }

            return invoiceVm;
        }
    }
}