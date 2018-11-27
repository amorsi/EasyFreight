using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace EasyFreight.DAL
{
    public class CashOutExpenseHelper
    {
        public static CashInVm GetCashReceiptForExpense(int receiptId = 0)
        {
            CashInVm cashVm;
            if (receiptId != 0)
            {
                cashVm = CashOutHelper.FillCashVmForReceiptView(receiptId);
                return cashVm;
            }
            cashVm = new CashInVm();
            cashVm.CashType = "cashout";
            cashVm.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashOut, false);
            cashVm.PaymentTermId = 1;
            cashVm.CurrencyId = 1;

            CashOutExpense cashOutExpense = new CashOutExpense();
            cashVm.CashOutReceiptExpenses.Add(cashOutExpense);


            return cashVm;
        }

        internal static string AddEditExpenseCashReceipt(CashInVm cashInVmObj)
        {
            string isSaved = "true";

            AccountingEntities db = new AccountingEntities();
            int receiptId = cashInVmObj.ReceiptId;
            CashOutReceipt cashDbObj;
            if (receiptId == 0)
                cashDbObj = new CashOutReceipt();
            else
            {
                cashDbObj = db.CashOutReceipts.Include("CashOutReceiptExpenses")
                    .Where(x => x.ReceiptId == receiptId).FirstOrDefault();

                //Delete expenses list .. will insert it again
                var invList = cashDbObj.CashOutReceiptExpenses.ToList();
                foreach (var item in invList)
                {
                    cashDbObj.CashOutReceiptExpenses.Remove(item);
                }

            }

            Mapper.CreateMap<CashInVm, CashOutReceipt>()
              .ForMember(x => x.CashOutReceiptInvs, y => y.Ignore())
              .ForMember(x => x.CashOutReceiptChecks, y => y.Ignore());

            Mapper.CreateMap<CashOutExpense, CashOutReceiptExpense>();
            Mapper.Map(cashInVmObj, cashDbObj);

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

                    AddExpenseReceiptToTrans(cashInVmObj);

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

        private static void AddExpenseReceiptToTrans(CashInVm cashInVmObj)
        {
            string creditAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(cashInVmObj.CurrencyId, "Currency", "CurrencyId");
            if (string.IsNullOrEmpty(creditAccId))
            {
                string parentAccountId = ((int)AccountingChartEnum.Cash).ToString();
                //Add new accountId to the chart
                creditAccId = AccountingChartHelper
                    .AddAccountToChart(cashInVmObj.CurrencySign, cashInVmObj.CurrencySign, parentAccountId);
                AccountingChartHelper.AddAccountIdToObj(creditAccId, "Currency", cashInVmObj.CurrencyId, "CurrencyId");
            }


            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId(),
                TransactionName = "Cash Out Receipt Number " + cashInVmObj.ReceiptCode,
                TransactionNameAr = "ايصال صرف نقدية رقم " + cashInVmObj.ReceiptCode
            };

            AccountingEntities db = new AccountingEntities();
            var expensesLibList = db.ExpenseLibs.Select(x => new { x.ExpenseId, x.AccountId }).ToList();

            AccTransactionDetailVm accTransDetDebit;
            //Loop through expenses for debit accounts
            foreach (var item in cashInVmObj.CashOutReceiptExpenses)
            {
                string debitAccId = expensesLibList.Where(x => x.ExpenseId == item.ExpenseId).FirstOrDefault().AccountId;
                accTransDetDebit = new AccTransactionDetailVm()
                {
                    AccountId = debitAccId,
                    DebitAmount = item.PaidAmount.Value,
                    CurrencyId = cashInVmObj.CurrencyId,
                    CreditAmount = 0
                };
                accTrans.AccTransactionDetails.Add(accTransDetDebit);

            }

            // Add Cash as credit 
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

        internal static JObject GetExpensesCashOutJson(System.Web.Mvc.FormCollection form)
        {
            AccountingEntities db = new AccountingEntities();
            //Get Only cash out receipts for expenses
            var receiptIds = db.CashOutReceiptExpenses.Select(x => x.ReceiptId).Distinct();


            var receiptList = (from c in db.CashOutReceipts
                              join u in db.AspNetUserAccs
                              on c.CreateBy equals u.Id
                              join cu in db.CurrencyAccs
                              on c.CurrencyId equals cu.CurrencyId
                              where receiptIds.Contains(c.ReceiptId) && c.IsDeleted == false
                              select new
                              {
                                  c.ReceiptId,
                                  c.ReceiptDate,
                                  c.ReceiptCode,
                                  c.ReceiptAmount,
                                  c.Notes,
                                  c.CreateDate,
                                  c.CreateBy,
                                  cu.CurrencySign,
                                  u.UserName
                              }).ToList();

            if(!string.IsNullOrEmpty(form["ReceiptDateStart"]))
            {
                DateTime date = DateTime.Parse(form["ReceiptDateStart"]);
                receiptList = receiptList.Where(x => x.ReceiptDate >= date).ToList();
            }

            if (!string.IsNullOrEmpty(form["ReceiptDateEnd"]))
            {
                DateTime date = DateTime.Parse(form["ReceiptDateEnd"]);
                receiptList = receiptList.Where(x => x.ReceiptDate <= date).ToList();
            }

            if (!string.IsNullOrEmpty(form["CreateDateStart"]))
            {
                DateTime date = DateTime.Parse(form["CreateDateStart"]);
                receiptList = receiptList.Where(x => x.CreateDate >= date).ToList();
            }

            if (!string.IsNullOrEmpty(form["CreateDateEnd"]))
            {
                DateTime date = DateTime.Parse(form["CreateDateEnd"]);
                receiptList = receiptList.Where(x => x.CreateDate <= date).ToList();
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
                pJTokenWriter.WriteValue(item.Notes);

                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("UserName");
                pJTokenWriter.WriteValue(item.UserName);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static List<CashOutExpense> GetExpensesListForReceipt(int receiptId)
        {
            AccountingEntities db = new AccountingEntities();

            List<CashOutExpense> cashOutExpenseList = new List<CashOutExpense>();
            CashOutExpense cashOutExpense;
            var receiptExp = db.CashOutReceiptExpenses.Include("ExpenseLib").Include("CashOutReceipt")
                .Where(x => x.ReceiptId == receiptId)
                .Select(x => new { x.ExpenseLib.ExpenseNameEn,x.PaidAmount,x.CashOutReceipt.Currency.CurrencySign }).ToList();

            foreach (var item in receiptExp)
            {
                cashOutExpense = new CashOutExpense();
                cashOutExpense.CurrencySign = item.CurrencySign;
                cashOutExpense.ExpenseName = item.ExpenseNameEn;
                cashOutExpense.PaidAmount = item.PaidAmount;

                cashOutExpenseList.Add(cashOutExpense);
            }

            return cashOutExpenseList;
        }
    }
}