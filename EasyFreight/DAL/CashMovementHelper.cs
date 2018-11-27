using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace EasyFreight.DAL
{
    public static class CashMovementHelper
    {
        static string CurrentUserId = AdminHelper.GetCurrentUserId();

        public static string SaveCurrencyExchange(CurrencyExchangeVm currVm)
        {
            string result = "true";
            //create cash In object
            CashInVm cashIn = new CashInVm();
            cashIn.CashType = "cashin";
            cashIn.CreateBy = CurrentUserId;
            cashIn.CreateDate = DateTime.Now;
            cashIn.CurrencyId = currVm.NewCurrencyId;
            cashIn.Notes = currVm.Notes;
            cashIn.PaymentTermId = (byte)PaymentTermEnum.CurrencyExchange; //CurrencyExchange
            cashIn.ReceiptAmount = currVm.NewAmount;
            cashIn.ReceiptDate = DateTime.Now;

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    int cashInReceiptId, cashOutReceiptId;
                    CashHelper.AddEditCashReceipt(cashIn,out cashInReceiptId, false);
                    cashIn.CashType = "cashout";
                    cashIn.CurrencyId = currVm.CurrentCurrencyId;
                    cashIn.ReceiptAmount = currVm.CurrentAmount;
                    cashIn.ReceiptId = 0;
                    cashIn.ReceiptCode = "";
                    CashOutHelper.AddEditCashReceipt(cashIn, out cashOutReceiptId, false);

                    AddReceiptToTransTable(currVm, cashInReceiptId, cashOutReceiptId);

                    transaction.Complete();
                }
                catch (DbEntityValidationException e)
                {
                    result = "false " + e.Message;
                }
                catch (Exception e)
                {
                    result = "false " + e.Message;
                }
            }
            return result;
        }

        private static void AddReceiptToTransTable(CurrencyExchangeVm currVm, int cashInReceiptId, int cashOutReceiptId)
        {
            string creditAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(currVm.CurrentCurrencyId, "Currency", "CurrencyId");
            string debitAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(currVm.NewCurrencyId, "Currency", "CurrencyId");

            decimal creditAmount = currVm.CurrentAmount;
            decimal debitAmount = currVm.NewAmount;


            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = CurrentUserId,
                TransactionName = "Currency Exchange",
                TransactionNameAr = "تحويل عملة"
            };

            AccTransactionDetailVm accTransDetDebit = new AccTransactionDetailVm()
            {
                AccountId = debitAccId,
                CreditAmount = 0,
                CurrencyId = currVm.NewCurrencyId,
                DebitAmount = debitAmount
            };
            accTrans.AccTransactionDetails.Add(accTransDetDebit);

            AccTransactionDetailVm accTransDetCredit = new AccTransactionDetailVm()
            {
                AccountId = creditAccId,
                CreditAmount = currVm.CurrentAmount,
                CurrencyId = currVm.CurrentCurrencyId,
                DebitAmount = 0
            };
            accTrans.AccTransactionDetails.Add(accTransDetCredit);

            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, "CashInReceipt", cashInReceiptId, "ReceiptId");

            AccountingChartHelper.AddTransIdToObj(transId, "CashOutReceipt", cashOutReceiptId, "ReceiptId");
        }


        public static string SaveTransfer(CashBankTransferVm currVm)
        {
            string result = "true";
            //create cash In object

             
                CashInVm cashIn = new CashInVm();
                cashIn.CashType = "cashin";
                cashIn.CreateBy = CurrentUserId;
                cashIn.CreateDate = DateTime.Now;
                cashIn.CurrencyId = currVm.CurrentCurrencyId;
                cashIn.Notes = currVm.Notes;
                cashIn.PaymentTermId = currVm.IsCashToBank ? (byte)PaymentTermEnum.CashToBank : (byte)PaymentTermEnum.BankToCash;
                cashIn.ReceiptAmount = currVm.CurrentAmount;
                cashIn.ReceiptDate = DateTime.Now;

                var bankAcc = BankHelper.GetBankAccountInfo(currVm.BankAccId);
                cashIn.BankId = bankAcc.BankId;
                cashIn.BankAccId = bankAcc.BankAccId;

                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        int cashInReceiptId, cashOutReceiptId;
                        CashHelper.AddEditCashReceipt(cashIn, out cashInReceiptId, false);
                        cashIn.CashType = "cashout";
                        cashIn.CurrencyId = currVm.CurrentCurrencyId;
                        cashIn.ReceiptAmount = currVm.CurrentAmount;
                        cashIn.ReceiptId = 0;
                        cashIn.ReceiptCode = "";
                        cashIn.BankId = null;
                        cashIn.BankAccId = null;
                        CashOutHelper.AddEditCashReceipt(cashIn, out cashOutReceiptId, false);

                        AddReceiptToTransTableTransfer(currVm, cashInReceiptId, cashOutReceiptId);

                        transaction.Complete();
                    }
                    catch (DbEntityValidationException e)
                    {
                        result = "false " + e.Message;
                    }
                    catch (Exception e)
                    {
                        result = "false " + e.Message;
                    }
                 
            }
             
            
            return result;
        }

        private static void AddReceiptToTransTableTransfer(CashBankTransferVm currVm, int cashInReceiptId, int cashOutReceiptId)
        {
            string creditAccId, debitAccId;

            creditAccId = currVm.IsCashToBank ?
                AccountingChartHelper.GetAccountIdByPkAndTbName(currVm.CurrentCurrencyId, "Currency", "CurrencyId") :
                currVm.AccountId ;
           debitAccId = currVm.IsCashToBank ? currVm.AccountId :
               AccountingChartHelper.GetAccountIdByPkAndTbName(currVm.CurrentCurrencyId, "Currency", "CurrencyId");

            decimal amount = currVm.CurrentAmount;



            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = CurrentUserId,
                TransactionName = currVm.IsCashToBank ? "Cash to bank transfer" : "Bank to cash transfer",
                TransactionNameAr = currVm.IsCashToBank ? "تحويل من الخزينة الى البنك" : "تحويل من البنك للخزينة"
            };

            AccTransactionDetailVm accTransDetDebit = new AccTransactionDetailVm()
            {
                AccountId = debitAccId,
                CreditAmount = 0,
                CurrencyId = currVm.CurrentCurrencyId,
                DebitAmount = amount
            };
            accTrans.AccTransactionDetails.Add(accTransDetDebit);

            AccTransactionDetailVm accTransDetCredit = new AccTransactionDetailVm()
            {
                AccountId = creditAccId,
                CreditAmount = currVm.CurrentAmount,
                CurrencyId = currVm.CurrentCurrencyId,
                DebitAmount = 0
            };
            accTrans.AccTransactionDetails.Add(accTransDetCredit);

            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, "CashInReceipt", cashInReceiptId, "ReceiptId");

            AccountingChartHelper.AddTransIdToObj(transId, "CashOutReceipt", cashOutReceiptId, "ReceiptId");
        }
    
    }
}