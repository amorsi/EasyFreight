using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace EasyFreight.DAL
{
    public static class CashOutHelper
    {
        public static CashInVm GetCashReceiptInfo(int invId, int agNoteId = 0, int cashReceiptId = 0,
            int operationId = 0, decimal receiptAmount = 0)
        {
            CashInVm cashVm;
            if (cashReceiptId != 0)
            {
                cashVm = FillCashVmForReceiptView(cashReceiptId);
                return cashVm;
            }

            AccountingEntities db = new AccountingEntities();

            if (invId != 0) //Invoice
            {
                cashVm = db.APInvoiceViews.Where(x => x.InvoiceId == invId)
                .Select(x => new CashInVm
                {
                    CarrierId = x.CarrierId,
                    ContractorId = x.ContractorId,
                    CustomerName = x.InvoiceType == 1 ? x.CarrierNameEn : x.ContractorNameEn,
                    OrderFrom = x.OrderFrom,
                    InvoiceType = x.InvoiceType
                }).FirstOrDefault();
            }
            else if (operationId != 0) //operationId != 0 .. so it will be cash deposit for CC
            {
                OperationsEntities db1 = new OperationsEntities();
                cashVm = db1.Operations.Where(x => x.OperationId == operationId).Select(x => new CashInVm
                {
                    OperationId = x.OperationId,
                    OperationCode = x.OperationCode,
                    Notes = "Custom Clerance Cash Deposit For Operation " + x.OperationCode
                }).FirstOrDefault();

                if (receiptAmount != 0)
                {
                    cashVm.ReceiptAmount = receiptAmount;
                    cashVm.PaymentTermId = 1;
                }


                cashVm.CCCashDepositVmList = GetCCCashDepositList(operationId);

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


            cashVm.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashOut, false);
            CashInCheckVm cashCheckVm = new CashInCheckVm();
            cashVm.CashInReceiptChecks.Add(cashCheckVm);

            if (agNoteId != 0) //Cash Receipt for Agent Note
            {
                FillCashVmForAgNote(agNoteId, ref cashVm);
                cashVm.CashType = "cashout";
                return cashVm;
            }

            if (operationId != 0) //operationId != 0 .. so it will be cash deposit for CC
            {
                cashVm.CashType = "cashout";
                return cashVm;
            }
            //Cash Receipt for Invoice

            //Get invoice list for this customer
            List<CashInInvoiceVm> cashInvList = new List<CashInInvoiceVm>();
            List<APInvoiceView> invList = new List<APInvoiceView>();
            if (cashVm.InvoiceType == 1)
                invList = db.APInvoiceViews
                    .Where(x => x.CarrierId == cashVm.CarrierId && (x.InvStatusId != 4 && x.InvStatusId != 1)).ToList();
            else if (cashVm.InvoiceType == 2)
                invList = db.APInvoiceViews
                    .Where(x => x.ContractorId == cashVm.ContractorId && (x.InvStatusId != 4 && x.InvStatusId != 1)).ToList();
            else if (cashVm.InvoiceType == 3) // Cutom clearance .. will get all not paid invoices 
                //as there is only one grouped credit account for Cutom clearance suppliers .. 22/11/2016
                invList = db.APInvoiceViews
                    .Where(x => x.InvoiceType == 3 && (x.InvStatusId != 4 && x.InvStatusId != 1)).ToList();

            Mapper.CreateMap<APInvoiceView, CashInInvoiceVm>().IgnoreAllNonExisting();
            Mapper.Map(invList, cashInvList);


            cashVm.CashInReceiptInvs = cashInvList;
            List<int> invIds = cashInvList.Select(x => x.InvoiceId).ToList();
            var cashInvDb = db.CashOutReceiptInvs
                .Where(x => invIds.Contains(x.InvoiceId))
                .GroupBy(x => x.InvoiceId)
                .ToList();

            foreach (var item in cashVm.CashInReceiptInvs)
            {
                item.CollectedAmount = cashInvDb.Where(x => x.Key == item.InvoiceId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                item.AmountDue = item.TotalAmount - item.CollectedAmount;
            }


            cashVm.CashType = "cashout";

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
            agNoteList = db.AgentNoteViews
                .Where(x => x.AgentNoteId == agNoteId && (x.InvStatusId != 4 && x.InvStatusId != 1)).ToList();
            Mapper.CreateMap<AgentNoteView, CashInInvoiceVm>().IgnoreAllNonExisting();
            Mapper.Map(agNoteList, cashAgNoteList);

            cashVm.CashInReceiptInvs = cashAgNoteList;

            List<int> invIds = agNoteList.Select(x => x.AgentNoteId).ToList();
            var cashInvDb = db.CashOutReceiptAgNotes
                .Where(x => invIds.Contains(x.AgentNoteId))
                .GroupBy(x => x.AgentNoteId)
                .ToList();

            foreach (var item in cashVm.CashInReceiptInvs)
            {
                item.CollectedAmount = cashInvDb.Where(x => x.Key == item.AgentNoteId)
                    .Select(x => x.Sum(y => y.PaidAmount)).FirstOrDefault();
                item.AmountDue = item.TotalAmount - item.CollectedAmount;
            }
            cashVm.CashType = "cashout";

        }

        internal static CashInVm FillCashVmForReceiptView(int receiptId)
        {
            CashInVm cashVm = new CashInVm();
            AccountingEntities db = new AccountingEntities();
            EasyFreightEntities db1 = new EasyFreightEntities();
            var cashDb = db.CashOutReceipts
                            .Include("PaymentTerm")
                            .Include("Currency").Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            Mapper.CreateMap<CashOutReceipt, CashInVm>()
                .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
                .ForMember(x => x.CashInReceiptInvs, y => y.Ignore())
                .ForMember(x => x.CashOutReceiptExpenses, y => y.Ignore())
                .IgnoreAllNonExisting();

            Mapper.Map(cashDb, cashVm);

            cashVm.CurrencySign = cashDb.Currency.CurrencySign;
            cashVm.PaymentTermName = cashDb.PaymentTerm.PaymentTermEn;
            //Get customer Name
            int custId;
            if (cashVm.CarrierId != null)
            {
                custId = cashVm.CarrierId.Value;
                cashVm.CustomerName = db1.Carriers.Where(x => x.CarrierId == custId).FirstOrDefault().CarrierNameEn;
            }
            else if (cashVm.ContractorId != null)
            {
                custId = cashVm.ConsigneeId.Value;
                cashVm.CustomerName = db1.Contractors.Where(x => x.ContractorId == custId).FirstOrDefault().ContractorNameEn;
            }
            else if (cashVm.AgentId != null)
            {
                custId = cashVm.AgentId.Value;
                cashVm.CustomerName = db1.Agents.Where(x => x.AgentId == custId).FirstOrDefault().AgentNameEn;
            }

            //for bank
            if (cashVm.PaymentTermId == (byte)PaymentTermEnum.BankCashDeposit)
            {
                if (cashVm.BankAccId != null)
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
            }

            else if (cashVm.PaymentTermId == (byte)PaymentTermEnum.Check)
            {
                CashInCheckVm cashCheckVm = new CashInCheckVm();
                var cashCheckDb = db.CashOutReceiptChecks.Where(x => x.ReceiptId == receiptId).FirstOrDefault();
                Mapper.CreateMap<CashOutReceiptCheck, CashInCheckVm>().IgnoreAllNonExisting();
                Mapper.Map(cashCheckDb, cashCheckVm);
                cashVm.CashInReceiptChecks.Add(cashCheckVm);
            }

                //kamal
            else if (cashVm.PaymentTermId == (byte)PaymentTermEnum.CashToBank)
            {
                CashInCheckVm cashCheckVm = new CashInCheckVm();
                var cashCheckDb = db.CashOutReceiptChecks.Where(x => x.ReceiptId == receiptId).FirstOrDefault();
                Mapper.CreateMap<CashOutReceiptCheck, CashInCheckVm>().IgnoreAllNonExisting();
                Mapper.Map(cashCheckDb, cashCheckVm);
                cashVm.CashInReceiptChecks.Add(cashCheckVm);
            }



            //Get Invoice or agent notes codes list 
            if (cashVm.AgentId != null)
            {

            }
            else
            {
                var invlist = db.CashOutReceiptInvs.Include("InvoiceAP")
                    .Where(x => x.ReceiptId == receiptId).Select(x => x.InvoiceAP.InvoiceCode).ToArray();

                cashVm.InvCodeListString = string.Join(" - ", invlist);
            }

            //Check if for expenses
            var cashOutExp = db.CashOutReceiptExpenses.Include("ExpenseLib").Where(x => x.ReceiptId == receiptId).ToList();
            if (cashOutExp.Count() > 0)
            {
                List<CashOutExpense> cashOutExpenseList = new List<CashOutExpense>();
                CashOutExpense cashOutExpense;
                foreach (var item in cashOutExp)
                {
                    cashOutExpense = new CashOutExpense();
                    cashOutExpense.CurrencySign = cashVm.CurrencySign;
                    cashOutExpense.ExpenseName = item.ExpenseLib.ExpenseNameEn;
                    cashOutExpense.PaidAmount = item.PaidAmount;

                    cashOutExpenseList.Add(cashOutExpense);
                }
                cashVm.CashOutReceiptExpenses = cashOutExpenseList;
            }

            cashVm.CashType = "cashout";
            //Get Created by user name


            return cashVm;
        }

        internal static string AddEditCashReceipt(CashInVm cashInVmObj, out int savedReceiptId, bool addToTrans = true)
        {
            string isSaved = "true";

            AccountingEntities db = new AccountingEntities();
            int receiptId = cashInVmObj.ReceiptId;
            savedReceiptId = receiptId;
            CashOutReceipt cashDbObj;
            if (receiptId == 0)
                cashDbObj = new CashOutReceipt();
            else
            {
                cashDbObj = db.CashOutReceipts.Include("CashOutReceiptChecks")
                    .Include("CashOutReceiptInvs")
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

                //Delete invoice list .. will insert it again
                var invList = cashDbObj.CashOutReceiptInvs.ToList();
                foreach (var item in invList)
                {
                    cashDbObj.CashOutReceiptInvs.Remove(item);
                }

                //Delete check list .. will insert it again
                var checkList = cashDbObj.CashOutReceiptChecks.ToList();
                foreach (var item in checkList)
                {
                    cashDbObj.CashOutReceiptChecks.Remove(item);
                }
            }

            Mapper.CreateMap<CashInVm, CashOutReceipt>()
                .ForMember(x => x.CashOutReceiptInvs, y => y.Ignore())
                .ForMember(x => x.CashOutReceiptChecks, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(cashInVmObj, cashDbObj);

            CashOutReceiptCheck cashCheckDb;
            foreach (var item in cashInVmObj.CashInReceiptChecks)
            {
                if (!string.IsNullOrEmpty(item.CheckNumber))
                {
                    cashCheckDb = new CashOutReceiptCheck();
                    Mapper.CreateMap<CashInCheckVm, CashOutReceiptCheck>().IgnoreAllNonExisting();
                    Mapper.Map(item, cashCheckDb);

                    cashDbObj.CashOutReceiptChecks.Add(cashCheckDb);
                }
            }

            if (cashInVmObj.OperationId != null) //CC Cash Deposit
            {
                CashOutCCCashDeposit cashDeposit = new CashOutCCCashDeposit();
                cashDeposit.OperationId = cashInVmObj.OperationId.Value;
                cashDeposit.ReceiptId = cashInVmObj.ReceiptId;
                cashDbObj.CashOutCCCashDeposits.Add(cashDeposit);
            }

            else if (cashInVmObj.AgentId == null) //Cash Receipt for invoice
            {
                //Add Receipt invoices
                CashOutReceiptInv cashInvDb;
                foreach (var item in cashInVmObj.CashInReceiptInvs)
                {
                    if (item.IsSelected)
                    {
                        cashInvDb = new CashOutReceiptInv();
                        Mapper.CreateMap<CashInInvoiceVm, CashOutReceiptInv>().IgnoreAllNonExisting();
                        item.CashInReceipt = null;
                        Mapper.Map(item, cashInvDb);

                        cashDbObj.CashOutReceiptInvs.Add(cashInvDb);
                    }
                }
            }
            else //Cash Receipt for Agent Note
            {
                //Add Receipt Agent Notes
                CashOutReceiptAgNote cashAgNoteDb;
                foreach (var item in cashInVmObj.CashInReceiptInvs)
                {
                    if (item.IsSelected)
                    {
                        cashAgNoteDb = new CashOutReceiptAgNote();
                        Mapper.CreateMap<CashInInvoiceVm, CashOutReceiptAgNote>().IgnoreAllNonExisting();
                        item.CashInReceipt = null;
                        Mapper.Map(item, cashAgNoteDb);

                        cashDbObj.CashOutReceiptAgNotes.Add(cashAgNoteDb);
                    }
                }
            }


            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {


                    if (receiptId == 0)
                    {
                        cashDbObj.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashOut, true);
                        db.CashOutReceipts.Add(cashDbObj);
                    }

                    db.SaveChanges();

                    cashInVmObj.ReceiptId = cashDbObj.ReceiptId;
                    cashInVmObj.ReceiptCode = cashDbObj.ReceiptCode;

                    savedReceiptId = cashInVmObj.ReceiptId;

                    #region Add To Transaction Table
                    if (addToTrans)
                    {
                        //Add shipper or consignee to accounting chart
                        string debitAccountId = "";
                        if (cashInVmObj.OperationId != null) //CC Cash Deposit
                        {
                            debitAccountId = AccountingChartHelper.GetCCCashDepAccountId(cashInVmObj.OperationId.Value);
                            if (string.IsNullOrEmpty(debitAccountId))
                                debitAccountId = AccountingChartHelper.AddCCCashDepositToChart(cashInVmObj.OperationId.Value);
                        }
                        else if (!string.IsNullOrEmpty(cashInVmObj.PartnerAccountId)) //Partner Drawing
                            debitAccountId = cashInVmObj.PartnerAccountId;
                        else if (cashInVmObj.AgentId == null) //Cash Receipt for invoice
                        {
                            if (cashInVmObj.InvoiceType == 1) //carrier
                            {
                                debitAccountId = AccountingChartHelper
                                .GetAccountIdByPkAndTbName(cashInVmObj.CarrierId.Value, "Carrier", "CarrierId");
                                if (string.IsNullOrEmpty(debitAccountId))
                                    debitAccountId = AccountingChartHelper.AddCarrierToChart(cashInVmObj.CarrierId.Value);
                            }
                            else if (cashInVmObj.InvoiceType == 2) //contractor
                            {
                                debitAccountId = AccountingChartHelper
                                    .GetAccountIdByPkAndTbName(cashInVmObj.ContractorId.Value, "Contractor", "ContractorId");
                                if (string.IsNullOrEmpty(debitAccountId))
                                    debitAccountId = AccountingChartHelper.AddContractorToChart(cashInVmObj.ContractorId.Value);
                            }
                            else if (cashInVmObj.InvoiceType == 3) //custom clearance .. 22/11/2016
                            {
                                debitAccountId = ((int)AccountingChartEnum.CustomClearanceSupplier).ToString();
                            }
                        }
                        else //Cash Receipt for Agent Note
                        {
                            debitAccountId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.AgentId.Value, "Agent", "AgentId");
                            if (string.IsNullOrEmpty(debitAccountId))
                                debitAccountId = AccountingChartHelper.AddAgentToChart(cashInVmObj.AgentId.Value, 2); //AgentNoteType = 2 .. Credit Agent
                        }

                        if (receiptId == 0)
                        {
                            //Add invoice to accounting transactions table

                            AddReceiptToTransTable(debitAccountId, cashInVmObj);

                            foreach (var item in cashInVmObj.CashInReceiptInvs)
                            {
                                if (item.IsSelected)
                                {
                                    if (item.InvoiceId != 0) //Cash Receipt for invoice
                                    {
                                        //Change Invoice status
                                        if (item.AmountDue == 0)
                                            InvoiceHelper.ChangeInvStatus(item.InvoiceId, InvStatusEnum.Paid, true);
                                        else if (item.CollectedAmount != 0 && item.AmountDue != 0)
                                            InvoiceHelper.ChangeInvStatus(item.InvoiceId, InvStatusEnum.PartiallyPaid, true);
                                    }
                                    else //Cash Receipt for Agent Note
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

        internal static void AddReceiptToTransTable(string debitAccountId, CashInVm cashInVmObj, bool IsOpenBalancePayment = false)
        {
            //Check Payment type
            string creditAccId = "";
            byte paymentType = cashInVmObj.PaymentTermId;
            switch (paymentType)
            {
                case 1: //cash
                    creditAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.CurrencyId, "Currency", "CurrencyId");
                    if (string.IsNullOrEmpty(creditAccId))
                    {
                        string parentAccountId = ((int)AccountingChartEnum.Cash).ToString();
                        //Add new accountId to the chart
                        creditAccId = AccountingChartHelper
                            .AddAccountToChart(cashInVmObj.CurrencySign, cashInVmObj.CurrencySign, parentAccountId);
                        AccountingChartHelper.AddAccountIdToObj(creditAccId, "Currency", cashInVmObj.CurrencyId, "CurrencyId");
                    }
                    break;
                case 3: // Bank Cash Deposit
                    int bankAccountId = cashInVmObj.BankAccId.Value;
                    int bankId = cashInVmObj.BankId.Value;
                    creditAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(bankAccountId, "BankAccount", "BankAccId");
                    if (string.IsNullOrEmpty(creditAccId))
                    {
                        creditAccId = AccountingChartHelper.AddBankAccountToChart(bankId, bankAccountId);
                    }
                    break;
                case 4: //Check
                    creditAccId = ((int)AccountingChartEnum.NotesPayable).ToString();
                    break;
                default:
                    break;
            }

            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId(),
                TransactionName = "Cash Out Receipt Number " + cashInVmObj.ReceiptCode + (IsOpenBalancePayment ? " pay open balance " : ""),
                TransactionNameAr = "ايصال صرف نقدية رقم " + cashInVmObj.ReceiptCode
            };

            AccTransactionDetailVm accTransDetDebit = new AccTransactionDetailVm()
            {
                AccountId = debitAccountId,
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


            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, "CashOutReceipt", cashInVmObj.ReceiptId, "ReceiptId");
        }

        public static List<CashInInvoiceVm> GetCashInvList(int invId)
        {
            List<CashInInvoiceVm> cashInVmList = new List<CashInInvoiceVm>();
            AccountingEntities db = new AccountingEntities();
            var cashDbList = db.CashOutReceiptInvs
                .Include("CashOutReceipt")
                .Include("CashOutReceipt.PaymentTerm")
                .Include("CashOutReceipt.Currency")
                .Where(x => x.InvoiceId == invId).ToList();

            Mapper.CreateMap<CashOutReceiptInv, CashInInvoiceVm>()
                .IgnoreAllNonExisting();
            Mapper.CreateMap<CashOutReceipt, CashInVm>()
                .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
                .IgnoreAllNonExisting();

            Mapper.Map(cashDbList, cashInVmList);

            foreach (var item in cashInVmList)
            {
                var cashDbObj = cashDbList
                    .Where(x => x.ReceiptId == item.ReceiptId && x.InvoiceId == item.InvoiceId).FirstOrDefault();
                Mapper.Map(cashDbObj.CashOutReceipt, item.CashInReceipt);
                item.CurrencySign = cashDbObj.CashOutReceipt.Currency.CurrencySign;
                item.CashInReceipt.PaymentTermName = cashDbObj.CashOutReceipt.PaymentTerm.PaymentTermEn;
                item.CashInReceipt.CashType = "cashout";
            }

            return cashInVmList;
        }

        public static List<CashInInvoiceVm> GetCashAgNoteList(int agNoteId)
        {
            List<CashInInvoiceVm> cashInVmList = new List<CashInInvoiceVm>();
            AccountingEntities db = new AccountingEntities();
            var cashDbList = db.CashOutReceiptAgNotes
                .Include("CashOutReceipt")
                .Include("CashOutReceipt.PaymentTerm")
                .Include("CashOutReceipt.Currency")
                .Where(x => x.AgentNoteId == agNoteId).ToList();

            Mapper.CreateMap<CashOutReceiptAgNote, CashInInvoiceVm>().IgnoreAllNonExisting();

            Mapper.Map(cashDbList, cashInVmList);

            Mapper.CreateMap<CashOutReceipt, CashInVm>();

            foreach (var item in cashInVmList)
            {
                int receiptId = item.ReceiptId;
                Mapper.Map(cashDbList.Where(x => x.ReceiptId == receiptId).FirstOrDefault().CashOutReceipt, item.CashInReceipt);
                var cashDbObj = cashDbList
                    .Where(x => x.ReceiptId == item.ReceiptId && x.AgentNoteId == item.AgentNoteId).FirstOrDefault();
                item.CurrencySign = cashDbObj.CashOutReceipt.Currency.CurrencySign;
                item.CashInReceipt.PaymentTermName = cashDbObj.CashOutReceipt.PaymentTerm.PaymentTermEn;
            }

            return cashInVmList;
        }

        public static List<CCCashDepositVm> GetCCCashDepositList(int operationId)
        {
            AccountingEntities db = new AccountingEntities();
            List<CCCashDepositVm> cashDepositVmList = new List<CCCashDepositVm>();
            var cashDepositDbList = db.CashOutCCCashDeposits.Include("CashOutReceipt")
                .Where(x => x.OperationId == operationId)
                .Select(x => new
                {
                    x.OperationId,
                    x.ReceiptId,
                    x.CashOutReceipt.ReceiptAmount,
                    x.CashOutReceipt.Currency.CurrencySign,
                    x.CashOutReceipt.CreateDate,
                    x.CashOutReceipt.ReceiptCode
                }).ToList();

            CCCashDepositVm cashDepositVm;
            foreach (var item in cashDepositDbList)
            {
                cashDepositVm = new CCCashDepositVm()
                {
                    CreateDate = item.CreateDate,
                    CurrencySign = item.CurrencySign,
                    OperationId = item.OperationId,
                    ReceiptId = item.ReceiptId,
                    ReceiptAmount = item.ReceiptAmount,
                    ReceiptCode = item.ReceiptCode
                };

                cashDepositVmList.Add(cashDepositVm);

            }
            return cashDepositVmList;
        }


        internal static string DeleteCashOutReceipt(int receiptId, int invID, string deleteReason)
        {
            return CashInOutReceiptHelper.CashOutReceipt_Delete(receiptId, invID, 0, deleteReason) ? "true" : "false";
        }
        internal static string DeleteCashOutReceipt(int receiptId,  string deleteReason)
        {
            return CashInOutReceiptHelper.CashOutReceipt_Delete(receiptId, 0, 0, deleteReason) ? "true" : "false";
        }
        /*
        internal static string DeleteCashOutReceipt(int receiptId, int invID, string deleteReason)
        {

            string isSaved = "false";

            if (receiptId == 0)
                return isSaved;

            AccountingEntities db = new AccountingEntities();

            CashOutReceipt cashOutDb = new CashOutReceipt();
            cashOutDb = db.CashOutReceipts
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            CashOutReceiptCheck cashOutCheckDb = new CashOutReceiptCheck();
            cashOutCheckDb = db.CashOutReceiptChecks
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            CashOutReceiptInv cashOutInvkDb = new CashOutReceiptInv();
            cashOutInvkDb = db.CashOutReceiptInvs
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();



            int? transID = null;
            if (cashOutDb.TransId != null)
                transID = cashOutDb.TransId.Value;
            cashOutDb.IsDeleted = true;
            cashOutDb.DeleteReason = deleteReason;
            cashOutDb.DeletedBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
            cashOutDb.TransId = null;


            if (cashOutCheckDb != null)
                db.CashOutReceiptChecks.Remove(cashOutCheckDb);

            if (cashOutInvkDb != null)
                db.CashOutReceiptInvs.Remove(cashOutInvkDb);

            try
            {
                db.SaveChanges();

                if (transID.HasValue)
                    AccountingHelper.DeleteTransaction(transID.Value);

                List<CashInInvoiceVm> cashOutVmObj = GetCashInvList(invID);

                if (invID != 0)
                {
                    if (cashOutVmObj.Count == 0)
                        InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.Approved, true);
                    else
                    {

                        InvoiceAP invDb = db.InvoiceAPs.Where(x => x.InvoiceId == invID).FirstOrDefault();

                        decimal InvoiceTotals = invDb.InvoiceTotalAPs.Sum(s => s.TotalAmount);

                        decimal? totalPaidAmount = cashOutVmObj.Sum(s => s.PaidAmount);

                        if (totalPaidAmount == null)
                            InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.Approved, true);
                        else
                        {
                            if (totalPaidAmount == InvoiceTotals)
                                InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.Paid, true);
                            if (totalPaidAmount < InvoiceTotals)
                                InvoiceHelper.ChangeInvStatus(invID, InvStatusEnum.PartiallyPaid, true);
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

        internal static string AddEditOpenCashReceipt(CashInVm cashInVmObj, out int savedReceiptId, bool addToTrans = true)
        {
            string isSaved = "true";

            AccountingEntities db = new AccountingEntities();



            int receiptId = cashInVmObj.ReceiptId;
            savedReceiptId = receiptId;



            CashOutReceipt cashDbObj;
            if (receiptId == 0)
                cashDbObj = new CashOutReceipt();
            else
            {
                cashDbObj = db.CashOutReceipts.Include("CashOutReceiptChecks")
                    .Include("CashOutReceiptInvs")
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

                //Delete invoice list .. will insert it again
                var invList = cashDbObj.CashOutReceiptInvs.ToList();
                foreach (var item in invList)
                {
                    cashDbObj.CashOutReceiptInvs.Remove(item);
                }

                //Delete check list .. will insert it again
                var checkList = cashDbObj.CashOutReceiptChecks.ToList();
                foreach (var item in checkList)
                {
                    cashDbObj.CashOutReceiptChecks.Remove(item);
                }
            }

            Mapper.CreateMap<CashInVm, CashOutReceipt>()
              .ForMember(x => x.CashOutReceiptInvs, y => y.Ignore())
              .ForMember(x => x.CashOutReceiptChecks, y => y.Ignore())
              .IgnoreAllNonExisting();
            Mapper.Map(cashInVmObj, cashDbObj);

            CashOutReceiptCheck cashCheckDb;
            foreach (var item in cashInVmObj.CashInReceiptChecks)
            {
                if (!string.IsNullOrEmpty(item.CheckNumber))
                {
                    cashCheckDb = new CashOutReceiptCheck();
                    Mapper.CreateMap<CashInCheckVm, CashOutReceiptCheck>().IgnoreAllNonExisting();
                    Mapper.Map(item, cashCheckDb);

                    cashDbObj.CashOutReceiptChecks.Add(cashCheckDb);
                }
            }
            
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                { 
                    if (receiptId == 0)
                    {
                        cashDbObj.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashOut, true);
                        db.CashOutReceipts.Add(cashDbObj);
                    }

                    db.SaveChanges();

                    cashInVmObj.ReceiptId = cashDbObj.ReceiptId;
                    cashInVmObj.ReceiptCode = cashDbObj.ReceiptCode;

                    savedReceiptId = cashInVmObj.ReceiptId;

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
                        //accTranDetail.DebitAmount = cashInVmObj.ReceiptAmount.Value;
                        //accTranDetail.CurrencyId = cashInVmObj.CurrencyId;

                        //accTran.AccTransactionDetails.Add(accTranDetail);

                        //db.AccTransactions.Add(accTran);

                        //db.SaveChanges();

                        AddReceiptToTransTable(creditAccountId, cashInVmObj,true);
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