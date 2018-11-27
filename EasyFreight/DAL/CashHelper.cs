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
using System.Linq.Dynamic;
using Humanizer;

namespace EasyFreight.DAL
{
    public static class CashHelper
    {
        public static CashInVm GetCashReceiptInfo(int invId, int agNoteId = 0, int cashReceiptId = 0)
        {
            CashInVm cashVm;
            if (cashReceiptId != 0)
            {
                cashVm = FillCashVmForReceiptView(cashReceiptId);
                cashVm.CashType = "cashin";
                return cashVm;
            }

            AccountingEntities db = new AccountingEntities();

            if (invId != 0) //Invoice
            {
                cashVm = db.InvoiceViews.Where(x => x.InvoiceId == invId)
                .Select(x => new CashInVm
                {
                    ShipperId = x.ShipperId,
                    ConsigneeId = x.ConsigneeId,
                    CustomerName = x.OrderFrom == 1 ? x.ShipperNameEn : x.ConsigneeNameEn,
                    OrderFrom = x.OrderFrom 
                }).FirstOrDefault();
            }
            else //invId == 0 .. so agNoteId will not be 0 .. collect agent note
            {
                cashVm = db.AgentNoteViews.Where(x => x.AgentNoteId == agNoteId)
                    .Select(x => new CashInVm
                    {
                        AgentId = x.AgentId,
                        CustomerName = x.AgentNameEn,
                        OrderFrom = x.OrderFrom
                    }).FirstOrDefault();
            }

            if (cashVm == null)
                cashVm = new CashInVm();

            cashVm.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashIn, false);
            CashInCheckVm cashCheckVm = new CashInCheckVm();
            cashVm.CashInReceiptChecks.Add(cashCheckVm);

            if (agNoteId != 0) //Cash Receipt for Agent Note
            {
                FillCashVmForAgNote(agNoteId, ref cashVm);
                cashVm.CashType = "cashin";
                return cashVm;
            }

            //Cash Receipt for Invoice

            //Get invoice list for this customer
            List<CashInInvoiceVm> cashInvList = new List<CashInInvoiceVm>();
            List<InvoiceView> invList = new List<InvoiceView>();
            if (cashVm.OrderFrom == 1) //add condation for undeleted invoices [x.IsDeleted == null]
                invList = db.InvoiceViews
                    .Where(x => x.ShipperId == cashVm.ShipperId && (x.InvStatusId != 4 && x.InvStatusId != 1) && x.IsDeleted==null).ToList();
            else //add condation for undeleted invoices [x.IsDeleted == null]
                invList = db.InvoiceViews
                    .Where(x => x.ConsigneeId == cashVm.ConsigneeId && (x.InvStatusId != 4 && x.InvStatusId != 1) && x.IsDeleted == null).ToList();

            Mapper.CreateMap<InvoiceView, CashInInvoiceVm>().IgnoreAllNonExisting();
            Mapper.Map(invList, cashInvList);


            cashVm.CashInReceiptInvs = cashInvList;
            List<int> invIds = cashInvList.Select(x => x.InvoiceId).ToList();
            var cashInvDb = db.CashInReceiptInvs
                .Where(x => invIds.Contains(x.InvoiceId))
                .GroupBy(x => x.InvoiceId)
                .ToList();

            foreach (var item in cashVm.CashInReceiptInvs)
            {
                item.CollectedAmount = cashInvDb.Where(x => x.Key == item.InvoiceId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                item.AmountDue = item.TotalAmount - item.CollectedAmount;
            }

            cashVm.CashType = "cashin";


            return cashVm;
        }

        /// <summary>
        /// Get Cash Receipt info for Agent Note ..Not like invoices .. 
        /// Will allow only to collect from one agent note only .. not for all notes for that agent
        /// </summary>
        /// <param name="agNoteId"></param>
        /// <param name="cashVm"></param>
        private static void FillCashVmForAgNote(int agNoteId, ref CashInVm cashVm)
        {
            AccountingEntities db = new AccountingEntities();
            List<CashInInvoiceVm> cashAgNoteList = new List<CashInInvoiceVm>();
            List<AgentNoteView> agNoteList = new List<AgentNoteView>();

            AgentNoteView agv = new AgentNoteView(); 
            agv = db.AgentNoteViews
                .Where(x => x.AgentNoteId == agNoteId).FirstOrDefault();

            //agNoteList = db.AgentNoteViews
            //    .Where(x => x.AgentNoteId == agNoteId && (x.InvStatusId != 4 && x.InvStatusId != 1)  ).ToList();

            agNoteList = db.AgentNoteViews
               .Where(x => x.AgentId == agv.AgentId && (x.InvStatusId != 4 && x.InvStatusId != 1)).ToList();

            Mapper.CreateMap<AgentNoteView, CashInInvoiceVm>().IgnoreAllNonExisting();
            Mapper.Map(agNoteList, cashAgNoteList);

            cashVm.CashInReceiptInvs = cashAgNoteList;

            List<int> invIds = agNoteList.Select(x => x.AgentNoteId).ToList();
            var cashInvDb = db.CashInReceiptAgNotes
                .Where(x => invIds.Contains(x.AgentNoteId))
                .GroupBy(x => x.AgentNoteId)
                .ToList();

            foreach (var item in cashVm.CashInReceiptInvs)
            {
                item.CollectedAmount = cashInvDb.Where(x => x.Key == item.AgentNoteId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                item.AmountDue = item.TotalAmount - item.CollectedAmount;
            }

            cashVm.CashType = "cashin";
        }

        private static CashInVm FillCashVmForReceiptView(int receiptId)
        {
            CashInVm cashVm = new CashInVm();
            AccountingEntities db = new AccountingEntities();
            EasyFreightEntities db1 = new EasyFreightEntities();
            var cashDb = db.CashInReceipts
                            .Include("PaymentTerm")
                            .Include("Currency").Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            Mapper.CreateMap<CashInReceipt, CashInVm>()
                .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
                .ForMember(x => x.CashInReceiptInvs, y => y.Ignore())
                .IgnoreAllNonExisting();

            Mapper.Map(cashDb, cashVm);

            cashVm.CurrencySign = cashDb.Currency.CurrencySign;
            cashVm.PaymentTermName = cashDb.PaymentTerm.PaymentTermEn;
            //Get customer Name
            int custId;
            if (cashVm.ShipperId != null)
            {
                custId = cashVm.ShipperId.Value;
                cashVm.CustomerName = db1.Shippers.Where(x => x.ShipperId == custId).FirstOrDefault().ShipperNameEn;
            }
            else if (cashVm.ConsigneeId != null)
            {
                custId = cashVm.ConsigneeId.Value;
                cashVm.CustomerName = db1.Consignees.Where(x => x.ConsigneeId == custId).FirstOrDefault().ConsigneeNameEn;
            }
            else if (cashVm.AgentId != null)
            {
                custId = cashVm.AgentId.Value;
                cashVm.CustomerName = db1.Agents.Where(x => x.AgentId == custId).FirstOrDefault().AgentNameEn;
            }

            //for bank
            if (cashVm.PaymentTermId == (byte)PaymentTermEnum.BankCashDeposit)
            {
                int bankAccId = cashVm.BankAccId.Value;
                var bankInfo = db.BankAccounts.Include("Bank").Where(x => x.BankAccId == bankAccId).FirstOrDefault();
                cashVm.BankDetailsVm = new BankDetailsVm()
                {
                    AccountName = bankInfo.AccountName,
                    AccountNumber = bankInfo.AccountNumber,
                    BankNameEn = bankInfo.Bank.BankNameEn,
                    BankAddressEn = bankInfo.Bank.BankAddressEn
                };
            }

            else if (cashVm.PaymentTermId == (byte)PaymentTermEnum.Check)
            {
                CashInCheckVm cashCheckVm = new CashInCheckVm();
                var cashCheckDb = db.CashInReceiptChecks.Where(x => x.ReceiptId == receiptId).FirstOrDefault();
                Mapper.CreateMap<CashInReceiptCheck, CashInCheckVm>().IgnoreAllNonExisting();
                Mapper.Map(cashCheckDb, cashCheckVm);
                cashCheckVm.BankNameEn = db.Banks.Where(x => x.BankId == cashCheckVm.BankId).Select(s => s.BankNameEn).FirstOrDefault();
                cashVm.CashInReceiptChecks.Add(cashCheckVm);
            }

                //kamal
            else if (cashVm.PaymentTermId == (byte)PaymentTermEnum.CashToBank)
            {
                CashInCheckVm cashCheckVm = new CashInCheckVm();
                var cashCheckDb = db.CashInReceiptChecks.Where(x => x.ReceiptId == receiptId).FirstOrDefault();
                Mapper.CreateMap<CashInReceiptCheck, CashInCheckVm>().IgnoreAllNonExisting();
                Mapper.Map(cashCheckDb, cashCheckVm);  
                cashCheckVm.BankNameEn = db.Banks.Where(x => x.BankId == cashCheckVm.BankId).Select(s => s.BankNameEn).FirstOrDefault();
                cashVm.CashInReceiptChecks.Add(cashCheckVm);
            }


            //Get Invoice or agent notes codes list 
            if (cashVm.AgentId != null)
            {

            }
            else
            {
                var invlist = db.CashInReceiptInvs.Include("Invoice")
                    .Where(x => x.ReceiptId == receiptId).Select(x => x.Invoice.InvoiceCode).ToArray();

                cashVm.InvCodeListString = string.Join(" - ", invlist);

                var invId = db.CashInReceiptInvs.Include("Invoice")
                    .Where(x => x.ReceiptId == receiptId).Select(x => x.Invoice.InvoiceId).FirstOrDefault(); 

                InvoiceVm invVmList = new  InvoiceVm ();
                 Invoice  invDbList = new  Invoice ();

                invDbList = db.Invoices.Include("InvStatusLib").Where(x => x.InvoiceId == invId).Where(a => a.IsDeleted == null).FirstOrDefault();

                Mapper.CreateMap<Invoice, InvoiceVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<InvoiceDetail, InvoiceDetailVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<InvoiceTotal, InvoiceTotalVm>().IgnoreAllNonExisting();

                Mapper.Map(invDbList, invVmList); 
                cashVm.InvsVm = invVmList;

            }
            cashVm.CashType = "cashin";
            return cashVm;
        }

        internal static string AddEditCashReceipt(CashInVm cashInVmObj, out int savedReceiptId, bool addToTrans = true)
        {
            string isSaved = "true";

            AccountingEntities db = new AccountingEntities();

            int receiptId = cashInVmObj.ReceiptId;
            savedReceiptId = receiptId;

            #region collect from Ar Deposit
            //1- check if balance if collected from deposit
            if (cashInVmObj.PaymentTermId == (byte)PaymentTermEnum.FromDeposit)
            {
                var balance = CashDepositBalanceHelper.GetDepositBalance(cashInVmObj.ShipperId, cashInVmObj.ConsigneeId, cashInVmObj.CurrencyId);
                if (balance <= 0)
                {
                    return "false .. This client has no deposit balance ";
                }
                else
                {
                    //balance > 0 .. check if will be enough 
                    if (balance - cashInVmObj.ReceiptAmount < 0)
                        return "false .. The deposit balance is not enough to collect this amount";
                }
            }
            #endregion

            CashInReceipt cashDbObj;
            if (receiptId == 0)
                cashDbObj = new CashInReceipt();
            else
            {
                cashDbObj = db.CashInReceipts.Include("CashInReceiptChecks")
                    .Include("CashInReceiptInvs")
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

                //Delete invoice list .. will insert it again
                var invList = cashDbObj.CashInReceiptInvs.ToList();
                foreach (var item in invList)
                {
                    cashDbObj.CashInReceiptInvs.Remove(item);
                }

                //Delete check list .. will insert it again
                var checkList = cashDbObj.CashInReceiptChecks.ToList();
                foreach (var item in checkList)
                {
                    cashDbObj.CashInReceiptChecks.Remove(item);
                }
            }

            Mapper.CreateMap<CashInVm, CashInReceipt>()
                .ForMember(x => x.CashInReceiptInvs, y => y.Ignore())
                .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(cashInVmObj, cashDbObj);

            CashInReceiptCheck cashCheckDb;
            foreach (var item in cashInVmObj.CashInReceiptChecks)
            {
                if (!string.IsNullOrEmpty(item.CheckNumber))
                {
                    cashCheckDb = new CashInReceiptCheck();
                    Mapper.CreateMap<CashInCheckVm, CashInReceiptCheck>().IgnoreAllNonExisting();
                    Mapper.Map(item, cashCheckDb);

                    cashDbObj.CashInReceiptChecks.Add(cashCheckDb);
                }
            }

            if (cashInVmObj.CashInReceiptInvs.Count > 0)
            {
                if (cashInVmObj.AgentId == null) //Cash Receipt for invoice
                {
                    //Add Receipt invoices
                    CashInReceiptInv cashInvDb;
                    foreach (var item in cashInVmObj.CashInReceiptInvs.Where(x => x.InvoiceId != 0))
                    {
                        if (item.IsSelected)
                        {
                            cashInvDb = new CashInReceiptInv();
                            Mapper.CreateMap<CashInInvoiceVm, CashInReceiptInv>().IgnoreAllNonExisting();
                            item.CashInReceipt = null;
                            Mapper.Map(item, cashInvDb);

                            cashDbObj.CashInReceiptInvs.Add(cashInvDb);
                        }
                    }
                }
                else //Cash Receipt for Agent Note
                {
                    //Add Receipt Agent Notes
                    CashInReceiptAgNote cashAgNoteDb;
                    foreach (var item in cashInVmObj.CashInReceiptInvs)
                    {
                        if (item.IsSelected)
                        {
                            cashAgNoteDb = new CashInReceiptAgNote();
                            Mapper.CreateMap<CashInInvoiceVm, CashInReceiptAgNote>().IgnoreAllNonExisting();
                            item.CashInReceipt = null;
                            Mapper.Map(item, cashAgNoteDb);

                            cashDbObj.CashInReceiptAgNotes.Add(cashAgNoteDb);
                        }
                    }
                }
            }


            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {

                    if (cashInVmObj.PaymentTermId == (byte)PaymentTermEnum.FromDeposit)
                    {
                        //reduce client deposit balance
                        CashDepositBalanceHelper.AddEditArDepBalance(cashInVmObj.ShipperId, cashInVmObj.ConsigneeId,
                            cashInVmObj.CurrencyId, -cashInVmObj.ReceiptAmount.Value, db);
                    }
                    if (receiptId == 0)
                    {
                        cashDbObj.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashIn, true);
                        db.CashInReceipts.Add(cashDbObj);
                    }

                    db.SaveChanges();

                    cashInVmObj.ReceiptId = cashDbObj.ReceiptId;
                    cashInVmObj.ReceiptCode = cashDbObj.ReceiptCode;
                    savedReceiptId = cashDbObj.ReceiptId;

                    #region Add To Transaction Table
                    if (addToTrans)
                    {
                        //Add shipper or consignee to accounting chart
                        string creditAccountId = "";
                        if (cashInVmObj.AgentId == null && cashInVmObj.OrderFrom != 0) //Cash Receipt for invoice
                        {
                            creditAccountId = InvoiceHelper.GetAccountId(cashInVmObj.OrderFrom, cashInVmObj.ShipperId,
                            cashInVmObj.ConsigneeId);
                        }
                        else if (cashInVmObj.AgentId != null) //Cash Receipt for Agent Note
                        {
                            creditAccountId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.AgentId.Value, "Agent", "AgentId");
                            if (string.IsNullOrEmpty(creditAccountId))
                                creditAccountId = AccountingChartHelper.AddAgentToChart(cashInVmObj.AgentId.Value, 1); //AgentNoteType = 1 .. Debit Agent
                        }
                        else if (!string.IsNullOrEmpty(cashInVmObj.PartnerAccountId)) //Partners Drawing
                            creditAccountId = cashInVmObj.PartnerAccountId;
                        else if (cashInVmObj.OrderFrom == 0) //CC cash sattelment
                            creditAccountId = AccountingChartHelper.GetCCCashDepAccountId(cashInVmObj.OperationId.Value);

                        if (receiptId == 0 && !string.IsNullOrEmpty(creditAccountId))
                        {
                            //Add invoice to accounting transactions table
                            AddReceiptToTransTable(creditAccountId, cashInVmObj);

                            foreach (var item in cashInVmObj.CashInReceiptInvs)
                            {
                                if (item.IsSelected)
                                {
                                    if (item.InvoiceId != 0) //Cash Receipt for invoice
                                    {
                                        //Change Invoice status
                                        if (item.AmountDue == 0)
                                            InvoiceHelper.ChangeInvStatus(item.InvoiceId, InvStatusEnum.Paid);
                                        else if (item.CollectedAmount != 0 && item.AmountDue != 0)
                                            InvoiceHelper.ChangeInvStatus(item.InvoiceId, InvStatusEnum.PartiallyPaid);
                                    }
                                    else if (item.AgentNoteId != 0) //Cash Receipt for Agent Note
                                    {
                                        //Change Agent Note status
                                        if (item.AmountDue == 0)
                                            AgentNoteHelper.ChangeAgNoteStatus(item.AgentNoteId, InvStatusEnum.Paid);
                                        else if (item.CollectedAmount != 0 && item.AmountDue != 0)
                                            AgentNoteHelper.ChangeAgNoteStatus(item.AgentNoteId, InvStatusEnum.PartiallyPaid);
                                    }

                                }
                            }
                        }
                    }

                    #endregion

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

        internal static void AddReceiptToTransTable(string creditAccId, CashInVm cashInVmObj,
            string mainTbName = "CashInReceipt", string pkName = "ReceiptId",bool IsOpenBalancePayment=false)
        {
            //Check Payment type
            string debitAccId = "";
            byte paymentType = cashInVmObj.PaymentTermId;
            switch (paymentType)
            {
                case 1: //cash
                    debitAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.CurrencyId, "Currency", "CurrencyId");
                    if (string.IsNullOrEmpty(debitAccId))
                    {
                        string parentAccountId = ((int)AccountingChartEnum.Cash).ToString();
                        //Add new accountId to the chart
                        debitAccId = AccountingChartHelper
                            .AddAccountToChart(cashInVmObj.CurrencySign, cashInVmObj.CurrencySign, parentAccountId);
                        AccountingChartHelper.AddAccountIdToObj(debitAccId, "Currency", cashInVmObj.CurrencyId, "CurrencyId");
                    }
                    break;
                case 3: // Bank Cash Deposit
                    int bankAccountId = cashInVmObj.BankAccId.Value;
                    int bankId = cashInVmObj.BankId.Value;
                    debitAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(bankAccountId, "BankAccount", "BankAccId");
                    if (string.IsNullOrEmpty(debitAccId))
                    {
                        debitAccId = AccountingChartHelper.AddBankAccountToChart(bankId, bankAccountId);
                    }
                    break;
                case 4: //Check
                    debitAccId = ((int)AccountingChartEnum.NotesReceivable).ToString();
                    break;
                case 5: //From Cash Deposit
                    debitAccId = ((int)AccountingChartEnum.DepositRevenues).ToString();
                    break;
                default:
                    break;
            }

            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId(),
                TransactionName = "Cash In Receipt Number " + cashInVmObj.ReceiptCode + " " + mainTbName + (IsOpenBalancePayment ? " pay open balance " : ""),
                TransactionNameAr = "ايصال استلام نقدية رقم " + cashInVmObj.ReceiptCode 
            };


            AccTransactionDetailVm accTransDetDebit = new AccTransactionDetailVm()
            {
                AccountId = debitAccId,
                CreditAmount = 0,
                CurrencyId = cashInVmObj.CurrencyId,
                DebitAmount = cashInVmObj.ReceiptAmount.Value
            };

            accTrans.AccTransactionDetails.Add(accTransDetDebit);

            AccTransactionDetailVm accTransDetCredit = new AccTransactionDetailVm()
            {
                AccountId = creditAccId,
                CreditAmount = cashInVmObj.ReceiptAmount.Value,
                CurrencyId = cashInVmObj.CurrencyId,
                DebitAmount = 0
            };
            accTrans.AccTransactionDetails.Add(accTransDetCredit);
            if (creditAccId == ((int)AccountingChartEnum.DepositRevenues).ToString())
            {
                //Get Account ID 
                byte isexport = (cashInVmObj.ShipperId != null) ? (byte)1 : (byte)2;
                string newcreditAccId = InvoiceHelper.GetAccountId(isexport, cashInVmObj.ShipperId, cashInVmObj.ConsigneeId);
                string newdebitAccId = ((int)AccountingChartEnum.DepositRevenues).ToString();
                AccTransactionDetailVm accTransDetDebit2 = new AccTransactionDetailVm()
                {
                    AccountId = newdebitAccId,
                    CreditAmount = 0,
                    CurrencyId = cashInVmObj.CurrencyId,
                    DebitAmount = cashInVmObj.ReceiptAmount.Value 
                };
                accTrans.AccTransactionDetails.Add(accTransDetDebit2);

                AccTransactionDetailVm accTransDetCredit2 = new AccTransactionDetailVm()
                {
                    AccountId = newcreditAccId,
                    CreditAmount = cashInVmObj.ReceiptAmount.Value,
                    CurrencyId = cashInVmObj.CurrencyId,
                    DebitAmount = 0 
                     
                };
                accTrans.AccTransactionDetails.Add(accTransDetCredit2); 
            }

            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, mainTbName, cashInVmObj.ReceiptId, pkName);
        }

        public static List<CashInInvoiceVm> GetCashInvList(int invId)
        {
            List<CashInInvoiceVm> cashInVmList = new List<CashInInvoiceVm>();
            AccountingEntities db = new AccountingEntities();
            var cashDbList = db.CashInReceiptInvs
                .Include("CashInReceipt")
                .Include("CashInReceipt.PaymentTerm")
                .Include("CashInReceipt.Currency")
                .Where(x => x.InvoiceId == invId).ToList();

            Mapper.CreateMap<CashInReceiptInv, CashInInvoiceVm>().IgnoreAllNonExisting();
            Mapper.CreateMap<CashInReceipt, CashInVm>()
                .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
                .IgnoreAllNonExisting();

            Mapper.Map(cashDbList, cashInVmList);

            foreach (var item in cashInVmList)
            {
                var cashDbObj = cashDbList
                    .Where(x => x.ReceiptId == item.ReceiptId && x.InvoiceId == item.InvoiceId).FirstOrDefault();
                item.CurrencySign = cashDbObj.CashInReceipt.Currency.CurrencySign;
                item.CashInReceipt.PaymentTermName = cashDbObj.CashInReceipt.PaymentTerm.PaymentTermEn;
                item.CashInReceipt.CashType = "cashin";
            }

            return cashInVmList;
        }

        public static List<CashInInvoiceVm> GetCashAgNoteList(int agNoteId)
        {
            List<CashInInvoiceVm> cashInVmList = new List<CashInInvoiceVm>();
            AccountingEntities db = new AccountingEntities();
            var cashDbList = db.CashInReceiptAgNotes
                .Include("CashInReceipt")
                .Include("CashInReceipt.PaymentTerm")
                .Include("CashInReceipt.Currency")
                .Where(x => x.AgentNoteId == agNoteId).ToList();

            Mapper.CreateMap<CashInReceiptAgNote, CashInInvoiceVm>().IgnoreAllNonExisting();
            Mapper.CreateMap<CashInReceipt, CashInVm>()
                .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
                .IgnoreAllNonExisting();

            Mapper.Map(cashDbList, cashInVmList);

            foreach (var item in cashInVmList)
            {
                var cashDbObj = cashDbList
                    .Where(x => x.ReceiptId == item.ReceiptId && x.AgentNoteId == item.AgentNoteId).FirstOrDefault();
                item.CurrencySign = cashDbObj.CashInReceipt.Currency.CurrencySign;
                item.CashInReceipt.PaymentTermName = cashDbObj.CashInReceipt.PaymentTerm.PaymentTermEn;
                item.CashInReceipt.CashType = "cashin";
            }

            return cashInVmList;
        }


        internal static string DeleteCashInReceipt(int receiptId, int invID, string deleteReason)
        {
            return CashInOutReceiptHelper.CashInReceipt_Delete(receiptId, invID, 0, deleteReason) ? "true" : "false";

        }
        /*
        internal static string DeleteCashInReceipt(int receiptId, int invID, string deleteReason)
        {

            string isSaved = "false";

            if (receiptId == 0)
                return isSaved;

            AccountingEntities db = new AccountingEntities();

            CashInReceipt cashInDb = new CashInReceipt();
            cashInDb = db.CashInReceipts
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            CashInReceiptCheck cashInCheckDb = new CashInReceiptCheck();
            cashInCheckDb = db.CashInReceiptChecks
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            CashInReceiptInv cashInInvkDb = new CashInReceiptInv();
            cashInInvkDb = db.CashInReceiptInvs
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();



            int? transID = null;
            if (cashInDb.TransId != null)
                transID = cashInDb.TransId.Value;
            cashInDb.IsDeleted = true;
            cashInDb.DeleteReason = deleteReason;
            cashInDb.DeletedBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
            cashInDb.TransId = null;


            if (cashInCheckDb != null)
                db.CashInReceiptChecks.Remove(cashInCheckDb);

            if (cashInInvkDb != null)
                db.CashInReceiptInvs.Remove(cashInInvkDb);

            try
            {
                db.SaveChanges();

                if (transID.HasValue)
                    AccountingHelper.DeleteTransaction(transID.Value);

                // CashInVm cashInVmObj = GetCashReceiptInfo(invID);

                List<CashInInvoiceVm> cashInVmObj = GetCashInvList(invID);

                if (invID != 0)
                {
                    if (cashInVmObj.Count == 0)
                        InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.Approved);
                    else
                    {

                        Invoice invDb = db.Invoices.Where(x => x.InvoiceId == invID).FirstOrDefault();

                        decimal InvoiceTotals = invDb.InvoiceTotals.Sum(s => s.TotalAmount);

                        decimal? totalPaidAmount = cashInVmObj.Sum(s => s.PaidAmount);

                        if (totalPaidAmount == null)
                            InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.Approved);
                        else
                        {
                            if (totalPaidAmount == InvoiceTotals)
                                InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.Paid);
                            if (totalPaidAmount < InvoiceTotals)
                                InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.PartiallyPaid);
                        }
                    }
                }

                isSaved = "true";
            }
            catch
            {
                isSaved = "false";
            }



            return isSaved;

        }
        */
        internal static CashInVm GetCashReceiptInfoForSettlement(int operId, decimal receiptAmount = 0)
        {
            CashInVm cashVm = new CashInVm();

            OperationsEntities db = new OperationsEntities();
            var operObj = db.Operations.Where(x => x.OperationId == operId).FirstOrDefault();

            //Mapper.CreateMap<OperationView, CashInVm>().IgnoreAllNonExisting();
            //Mapper.Map(operObj, cashVm);
            cashVm.OperationId = operObj.OperationId;
            cashVm.OperationCode = operObj.OperationCode;

            cashVm.CashType = "cashin";
            cashVm.CustomerName = "Custom Clearance Settlement";
            cashVm.PaymentTermId = 1;
            cashVm.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashIn, false);

            CashInCheckVm cashCheck = new CashInCheckVm();
            cashVm.CashInReceiptChecks.Add(cashCheck);

            var ccCashDepList = CashOutHelper.GetCCCashDepositList(operId);

            //  decimal depositTotal = ccCashDepList.Sum(x => x.ReceiptAmount.Value);
            var depositTotals = ccCashDepList.GroupBy(x => x.CurrencySign).Select(x => new
            {
                currSign = x.Key,
                totalByCurr = x.Sum(y => y.ReceiptAmount)
            });

            //Get AP invoices
            var apInvList = APInvoiceHelper.GetInvoiceListForOper(operId);

            var ccInvList = apInvList.Where(x => x.InvoiceType == 3).ToList();

            // decimal cCInvTotal = ccInvList.Sum(x => x.InvoiceDetails.Sum(c => c.InvoiceAmount));

            var currList = ListCommonHelper.GetCurrencyList();
            var apInvTotals = apInvList.Where(x => x.InvoiceType == 3).GroupBy(x => x.InvCurrencyId)
            .Select(x => new { curr = x.Key, total = x.Sum(y => y.InvoiceDetails.Sum(c => c.InvoiceAmount)) });

            //Get any collcted back deposits
            var depositSattl = CashHelper.GetCashSettlementForOper(operId);

            CashInInvoiceVm cashInv;
            decimal depositTotal = 0;
            foreach (var item in apInvTotals)
            {
                string currSign = currList.Where(x => x.Key == item.curr).FirstOrDefault().Value;
                var checkDepositByCurr = depositTotals.Where(x => x.currSign == currSign).FirstOrDefault();
                if (checkDepositByCurr != null)
                {
                    if (depositSattl.Keys.Contains(currSign)) //deposit was paid back by cash in
                        depositTotal = checkDepositByCurr.totalByCurr.Value - depositSattl[currSign];
                    else
                        depositTotal = checkDepositByCurr.totalByCurr.Value;


                }

                if (depositTotal - item.total > 0) //has some value to collect back
                {
                    cashInv = new CashInInvoiceVm();
                    cashInv.InvoiceCode = "000";
                    cashInv.CurrencyId = item.curr.ToString();
                    cashInv.CurrencySign = currSign;
                    cashInv.TotalAmount = depositTotal - item.total;
                    cashInv.AmountDue = depositTotal - item.total;
                    cashInv.PaidAmount = depositTotal - item.total; //Amount to collect
                    cashInv.IsSelected = true;
                    cashVm.CashInReceiptInvs.Add(cashInv);

                }

            }

            cashVm.Notes = "Custom Clearance Cash Deposit Settlement";

            if (receiptAmount != 0)
                cashVm.ReceiptAmount = receiptAmount;
            //  cashVm.CurrencyId = ccInvList.FirstOrDefault().InvCurrencyId;

            return cashVm;
        }

        internal static Dictionary<string, decimal> GetCashSettlementForOper(int operId)
        {
            Dictionary<string, decimal> cashSettlAmount = null;
            //decimal cashSettlAmount = 0;
            AccountingEntities db = new AccountingEntities();
            var cashInList = db.CashInReceipts.Where(x => x.OperationId == operId
                && x.AgentId == null && x.ShipperId == null && x.ConsigneeId == null).ToList();
            if (cashInList != null)
            {
                cashSettlAmount = new Dictionary<string, decimal>();
                cashSettlAmount = cashInList.GroupBy(x => x.Currency.CurrencySign)
                .Select(x => new { curr = x.Key, total = x.Sum(y => y.ReceiptAmount) }).ToDictionary(x => x.curr, x => x.total);
            }

            return cashSettlAmount;
        }

        internal static JObject GetCashReceiptsJson(System.Web.Mvc.FormCollection form, string cashType)
        {
            AccountingEntities db = new AccountingEntities();

            dynamic receiptList;
            if (cashType == "cashin")
            {
                CashInReceiptView invViewDb = new CashInReceiptView();
                string where = CommonHelper.AdvancedSearch<CashInReceiptView>(form, invViewDb);
                if (string.IsNullOrEmpty(where))
                    where = "1 = 1"; //instead of make if condition

                where = where + " AND IsDeleted == false";

                receiptList = db.CashInReceiptViews.Where(where.ToString())
                            .Select(c => new
                            {
                                c.ReceiptId,
                                c.ReceiptDate,
                                c.ReceiptCode,
                                c.ReceiptAmount,
                                c.Notes,
                                c.CreateDate,
                                ReceiptFor1 = c.ShipperNameEn,
                                ReceiptFor2 = c.ConsigneeNameEn,
                                ReceiptFor3 = c.AgentNameEn,
                                c.CreateBy,
                                c.CurrencySign,
                                c.UserName,
                                c.PaymentTermEn
                            }).ToList();
            }
            else
            {
                CashOutReceiptView invViewDb = new CashOutReceiptView();
                string where = CommonHelper.AdvancedSearch<CashOutReceiptView>(form, invViewDb);
                if (string.IsNullOrEmpty(where))
                    where = "1 = 1"; //instead of make if condition

                where = where + " AND IsDeleted == false";

                receiptList = db.CashOutReceiptViews.Where(where.ToString())
                            .Select(c => new
                            {
                                c.ReceiptId,
                                c.ReceiptDate,
                                c.ReceiptCode,
                                c.ReceiptAmount,
                                c.Notes,
                                c.CreateDate,
                                ReceiptFor1 = c.CarrierNameEn,
                                ReceiptFor2 = c.ContractorNameEn,
                                ReceiptFor3 = c.AgentNameEn,
                                c.CreateBy,
                                c.CurrencySign,
                                c.UserName,
                                c.PaymentTermEn
                            }).ToList();

            }



            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in receiptList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("ReceiptId");
                pJTokenWriter.WriteValue(item.ReceiptId);

                pJTokenWriter.WritePropertyName("ReceiptCode");
                pJTokenWriter.WriteValue(item.ReceiptCode);

                pJTokenWriter.WritePropertyName("ReceiptDate");
                pJTokenWriter.WriteValue(item.ReceiptDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("ReceiptAmount");
                pJTokenWriter.WriteValue(item.ReceiptAmount.ToString() + " " + item.CurrencySign);

                pJTokenWriter.WritePropertyName("Notes");
                if (item.Notes == null)
                    pJTokenWriter.WriteValue("");
                else
                    pJTokenWriter.WriteValue(item.Notes);

                pJTokenWriter.WritePropertyName("ReceiptFor");
                if (!string.IsNullOrEmpty(item.ReceiptFor1))
                    pJTokenWriter.WriteValue(item.ReceiptFor1);
                else if (!string.IsNullOrEmpty(item.ReceiptFor2))
                    pJTokenWriter.WriteValue(item.ReceiptFor2);
                else if (!string.IsNullOrEmpty(item.ReceiptFor3))
                    pJTokenWriter.WriteValue(item.ReceiptFor3);
                else
                    pJTokenWriter.WriteValue("");



                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("UserName");
                pJTokenWriter.WriteValue(item.UserName);

                pJTokenWriter.WritePropertyName("PaymentTermEn");
                pJTokenWriter.WriteValue(item.PaymentTermEn);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static string GetNumberToWords(decimal someNumber, string currancy)
        {
            string comma = " and ";
            string pound =" Pound ";
            string penc = " pence ";
            string end = " only "; 
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us");
            string[] nums = someNumber.ToString().Split('.');

            if (currancy == "EGP")
            {
                culture = System.Globalization.CultureInfo.CreateSpecificCulture("ar-eg");
                comma = " و ";
                 pound = " جنيها ";
                penc = " قرش ";
                end = " فقط لاغير  "; 
            }
            if (currancy == "EUR")
            {
                comma = " and ";
                pound = " Euro ";
             }


            return int.Parse(nums[0]).ToWords(culture) + pound + ((nums.Length > 1 && int.Parse(nums[1]) > 0) ? (comma + int.Parse(nums[1]).ToWords(culture) +end) : end);
        }


        public static CashInVm GetCashReceiptOpenBalance(string accid, int cid, int cashReceiptId = 0)
        {
            CashInVm cashVm;
            OpenBalancePaymentVm openBalanceVm;

            if (cashReceiptId != 0)
            {
                cashVm = FillCashVmForReceiptView(cashReceiptId);
                cashVm.CashType = "cashin";
                return cashVm;
            }

           
            if (accid != "0") //Account ID
            {
                openBalanceVm = OpenBalancePaymentModels.Instance.Get_OpenBalanceObject(accid, cid);
                cashVm = new CashInVm();
                if (openBalanceVm != null)
                {
                    int accountType = 0; // 1= carrier / 2= contractor / 3= agent / 4=shipper / 5 = consignee / 6 = partner
                    int Id = 0;

                    if (accid.StartsWith("283"))
                    {
                        Id = int.Parse(accid);
                        accountType = 6;
                        cashVm.PartnerAccountId = accid;
                        cashVm.PartnerName = openBalanceVm.AccountNameEn;
                    }
                    else
                    {
                        OpenBalancePaymentModels.Instance.Get_AccountTypeID(accid, out Id, out accountType);
                        switch (accountType)
                        {
                            case 1:
                                cashVm.CarrierId = Id;
                                break;
                            case 2:
                                cashVm.ContractorId = Id;
                                break;
                            case 3:
                                cashVm.AgentId = Id;
                                break;
                            case 4:
                                cashVm.ShipperId = Id;
                                break;
                            case 5:
                                cashVm.ConsigneeId = Id;
                                break;
                        }
                    }
                    
                    cashVm.CustomerName = openBalanceVm.AccountNameEn;
                    cashVm.ReceiptAmount = openBalanceVm.Amount < 0 ? openBalanceVm.Amount * -1 : openBalanceVm.Amount;
                    cashVm.CurrencyId = openBalanceVm.CurrencyId;

                    if (openBalanceVm.IsCredit == 1 && openBalanceVm.Amount>0)
                    cashVm.CashType = "cashin";
                    if (openBalanceVm.IsCredit == 1 && openBalanceVm.Amount < 0)
                        cashVm.CashType = "cashout";

                    if (openBalanceVm.IsCredit == 0 && openBalanceVm.Amount > 0)
                        cashVm.CashType = "cashin";
                    if (openBalanceVm.IsCredit == 0 && openBalanceVm.Amount < 0)
                        cashVm.CashType = "cashout";

                    
                    cashVm.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashIn, false);
                    CashInCheckVm cashCheckVm = new CashInCheckVm();
                    cashVm.CashInReceiptChecks.Add(cashCheckVm);

                    return cashVm;
                }
            }
            else
                cashVm = new CashInVm();

            return cashVm;
        }

        internal static string AddEditOpenCashReceipt(CashInVm cashInVmObj, out int savedReceiptId, bool addToTrans = true)
        {
            string isSaved = "true";

            AccountingEntities db = new AccountingEntities();

            int receiptId = cashInVmObj.ReceiptId;
            savedReceiptId = receiptId;

       

            CashInReceipt cashDbObj;
            if (receiptId == 0)
                cashDbObj = new CashInReceipt();
            else
            {
                cashDbObj = db.CashInReceipts.Include("CashInReceiptChecks")
                    .Include("CashInReceiptInvs")
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

                //Delete invoice list .. will insert it again
                var invList = cashDbObj.CashInReceiptInvs.ToList();
                foreach (var item in invList)
                {
                    cashDbObj.CashInReceiptInvs.Remove(item);
                }

                //Delete check list .. will insert it again
                var checkList = cashDbObj.CashInReceiptChecks.ToList();
                foreach (var item in checkList)
                {
                    cashDbObj.CashInReceiptChecks.Remove(item);
                }
            }

            Mapper.CreateMap<CashInVm, CashInReceipt>()
                .ForMember(x => x.CashInReceiptInvs, y => y.Ignore())
                .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(cashInVmObj, cashDbObj);

            CashInReceiptCheck cashCheckDb;
            foreach (var item in cashInVmObj.CashInReceiptChecks)
            {
                if (!string.IsNullOrEmpty(item.CheckNumber))
                {
                    cashCheckDb = new CashInReceiptCheck();
                    Mapper.CreateMap<CashInCheckVm, CashInReceiptCheck>().IgnoreAllNonExisting();
                    Mapper.Map(item, cashCheckDb);

                    cashDbObj.CashInReceiptChecks.Add(cashCheckDb);
                }
            }


            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (receiptId == 0)
                    {
                        cashDbObj.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashIn, true);
                        db.CashInReceipts.Add(cashDbObj);
                    }

                    db.SaveChanges();

                    cashInVmObj.ReceiptId = cashDbObj.ReceiptId;
                    cashInVmObj.ReceiptCode = cashDbObj.ReceiptCode;
                    savedReceiptId = cashDbObj.ReceiptId;

                    #region Add To Transaction Table
                    if (addToTrans)
                    {
                        string creditAccountId = "";

                        if (cashInVmObj.ShipperId != null)
                            creditAccountId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.ShipperId.Value, "Shipper", "ShipperId");
                        if (cashInVmObj.CarrierId != null)
                            creditAccountId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.CarrierId.Value, "Carrier", "CarrierId");
                        if (cashInVmObj.ContractorId != null)
                            creditAccountId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.ContractorId.Value, "Contractor", "ContractorId");
                        if (cashInVmObj.AgentId != null)
                            creditAccountId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.AgentId.Value, "Agent", "AgentId");
                        if (cashInVmObj.ConsigneeId != null)
                            creditAccountId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.ConsigneeId.Value, "Consignee", "ConsigneeId");
                         
                        //AccTransaction accTran = new AccTransaction()
                        //{
                        //    CreateBy = AdminHelper.GetCurrentUserId(),
                        //    CreateDate = DateTime.Now,
                        //    TransactionName = "pay open balance"
                        //};

                        //AccTransactionDetail accTranDetail = new AccTransactionDetail();

                        //accTranDetail.AccountId = creditAccountId;
                        //accTranDetail.CreditAmount = cashInVmObj.ReceiptAmount.Value;
                        //accTranDetail.CurrencyId = cashInVmObj.CurrencyId;

                        //accTran.AccTransactionDetails.Add(accTranDetail);

                        //db.AccTransactions.Add(accTran);

                        //db.SaveChanges();

                        AddReceiptToTransTable(creditAccountId, cashInVmObj,"CashInReceipt","ReceiptId",true);
                    }

                    #endregion

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
    
    
    }
}