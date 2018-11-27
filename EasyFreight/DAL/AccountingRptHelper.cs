using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace EasyFreight.DAL
{
    public static class AccountingRptHelper
    {
        public static AccountSummaryMain FillAccountSummaryMain(out Dictionary<string, decimal> topViewTotal)
        {
            AccountSummaryMain accountSummaryMain = new AccountSummaryMain();
            accountSummaryMain.ArSummary = GetAccSummaryByParentAccId("171").Take(5).ToList();
            //accountSummaryMain.ApSummary = GetAccSummaryByParentAccId("281", true).Take(5).ToList();
            accountSummaryMain.AgentSummary = GetAccSummaryByParentAccId("178").Take(5).ToList();
            accountSummaryMain.CashInBankSummary = GetBankListRpt("193").ToList();
            accountSummaryMain.ApCarrierSummary = GetAccSummaryByParentAccId("2811", true).Take(5).ToList();
            accountSummaryMain.ApContractorSummary = GetAccSummaryByParentAccId("2812", true).Take(5).ToList();

            accountSummaryMain.TreasurySummary = GetAccSummaryByParentAccId("194").Take(5).ToList();

            //Get Total Collected
            AccountingEntities db = new AccountingEntities();

            accountSummaryMain.TotalCollectedByCurrency = db.CashInReceipts.Include("Currency")
                .Where(x => x.IsDeleted == false)
                .GroupBy(x => x.Currency.CurrencySign)
                .Select(x => new { CurrSign = x.Key, Amount = x.Sum(y => y.ReceiptAmount) })
                .ToDictionary(x => x.CurrSign, x => x.Amount);

            accountSummaryMain.TotalPaidByCurrency = db.CashOutReceipts.Include("Currency")
                .Where(x => x.IsDeleted == false)
                .GroupBy(x => x.Currency.CurrencySign)
                .Select(x => new { CurrSign = x.Key, Amount = x.Sum(y => y.ReceiptAmount) })
                .ToDictionary(x => x.CurrSign, x => x.Amount);

            //Fill Balance Top View Report
            accountSummaryMain.TopViewBalance = GetTopViewBalanceRpt();

            topViewTotal = GetTopBalanceTotals(accountSummaryMain.TopViewBalance);

            return accountSummaryMain;
        }

        #region Account By Parent Account Number
        public static List<AccountSummary> GetAccSummaryByParentAccId(string parentAccId,
            bool isCreditAccount = false, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<AccountSummary> accSummary = new List<AccountSummary>();
            ReportsAccountingEntities db = new ReportsAccountingEntities();
            var spResult = db.GetAccountSummaryByParentAccId(parentAccId, fromDate, toDate).ToList();

            Mapper.CreateMap<GetAccountSummaryByParentAccId_Result, AccountSummary>();
            Mapper.Map(spResult, accSummary);

            foreach (var item in accSummary)
            {
                if (isCreditAccount)
                    item.DiffAmount = item.CreditAmount - item.DebitAmout;
                else
                    item.DiffAmount = item.DebitAmout - item.CreditAmount;
            }


            return accSummary.OrderByDescending(x => x.DiffAmount).ToList();
        }

        public static JObject GetAccSummaryByParentAccIdJObj(string parentAccId, bool isCreditAccount = false,
          DateTime? fromDate = null, DateTime? toDate = null, int bId = 0)
        {
            List<AccountSummary> accSummary = GetAccSummaryByParentAccId(parentAccId, isCreditAccount, fromDate, toDate);

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            List<string> Ids = new List<string>();
            bool isFound = false;

            //Cash in Bank .. get for specific bank ID
            if (parentAccId == "193" && bId != 0)
            {
                AccountingEntities db = new AccountingEntities();

                var accIdsForBank = db.BankAccounts.Where(x => x.BankId == bId).Select(x => x.AccountId).ToList();

                accSummary = accSummary.Where(x => accIdsForBank.Contains(x.AccountId)).ToList();
            }

            foreach (var item in accSummary)
            {

                isFound = Ids.FindIndex(s => s == item.AccountId) > -1;
                if (!isFound)
                {
                    Ids.Add(item.AccountId);
                    pJTokenWriter.WriteStartObject();

                    pJTokenWriter.WritePropertyName("AccountId");
                    pJTokenWriter.WriteValue(item.AccountId);

                    pJTokenWriter.WritePropertyName("AccountNameEn");
                    pJTokenWriter.WriteValue(item.AccountNameEn);

                    foreach (string cur in Enum.GetNames(typeof(UsedCurrencies)))
                    {
                        pJTokenWriter.WritePropertyName(cur);
                        // pJTokenWriter.WriteValue(accSummary.Where(x=>x.AccountId == .item.AccountId && x.CurrencySign == cur).FirstOrDefault()?.DiffAmount.ToString("N"));


                        if (accSummary.Where(x => x.AccountId == item.AccountId && x.CurrencySign == cur).FirstOrDefault() != null)
                            pJTokenWriter.WriteValue(accSummary.Where(x => x.AccountId == item.AccountId && x.CurrencySign == cur).FirstOrDefault().DiffAmount.ToString("N"));
                        else
                            pJTokenWriter.WriteValue(0.0.ToString("N"));
                    }

                    //pJTokenWriter.WritePropertyName("DiffAmount");
                    //pJTokenWriter.WriteValue(item.DiffAmount.ToString("N"));

                    //pJTokenWriter.WritePropertyName("CurrencySign");
                    //pJTokenWriter.WriteValue(item.CurrencySign);

                    pJTokenWriter.WriteEndObject();
                }
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        /// <summary>
        /// Get Account Summary Grouped By Currency .. Used to fill summary table at page top
        /// </summary>
        /// <param name="parentAccId"></param>
        /// <param name="isCreditAccount"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>AccountSummary object</returns>
        public static AccountSummary GetAccountSummaryGrouped(string parentAccId, bool isCreditAccount = false,
            DateTime? fromDate = null, DateTime? toDate = null, int bId = 0)
        {
            AccountSummary accountSummary = new AccountSummary();

            var accSummaryList = GetAccSummaryByParentAccId(parentAccId, isCreditAccount, fromDate, toDate);

            AccountingEntities db = new AccountingEntities();

            accountSummary.AccountNameEn = db.AccountingCharts.Where(x => x.AccountId == parentAccId).FirstOrDefault().AccountNameEn;

            //Cash in Bank .. get for specific bank ID
            if(parentAccId == "193" && bId != 0)
            {
               var accIdsForBank = db.BankAccounts.Where(x => x.BankId == bId).Select(x => x.AccountId).ToList();

                accSummaryList = accSummaryList.Where(x => accIdsForBank.Contains(x.AccountId)).ToList();
            }
            

            if (accSummaryList.Count > 0)
            {
                var grouped = accSummaryList.GroupBy(x => x.CurrencySign)
                    .Select(x => new
                    {
                        Currency = x.Key,
                        DebitSum = x.Sum(y => y.DebitAmout),
                        CreditSum = x.Sum(y => y.CreditAmount)
                    }).ToList();

                List<AccountSummaryTotal> sumByCurrency = new List<AccountSummaryTotal>();
                AccountSummaryTotal accountSummaryTotal;
                decimal diffAmount;
                foreach (var item in grouped)
                {
                    accountSummaryTotal = new AccountSummaryTotal();
                    accountSummaryTotal.CurrencySign = item.Currency;
                    diffAmount = item.DebitSum - item.CreditSum;
                    if (diffAmount > 0)
                        accountSummaryTotal.DebitAmout = diffAmount;
                    else
                        accountSummaryTotal.CreditAmount = diffAmount * -1;

                    sumByCurrency.Add(accountSummaryTotal);
                }

                accountSummary.TotalByCurrency = sumByCurrency;
            }

            return accountSummary;
        }

        #endregion

        #region Account By Account Number

        /// <summary>
        /// Get Account Summary contains totals grouped by currency for specific account number
        /// Used to fill summary table at page top
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static AccountSummary GetAccSummaryByAccId(string accId, string fromDate=null , string toDate=null)
        {
            ReportsAccountingEntities db = new ReportsAccountingEntities();
            AccountSummary accSummary = new AccountSummary();
            var spResult = db.GetAccountDetailsByAccId(accId, fromDate == "" ? null : fromDate, toDate == "" ? null : toDate).ToList();
            if (spResult.Count > 0)
            {
                accSummary.AccountNameEn = spResult.FirstOrDefault().AccountNameEn;
                accSummary.AccountNameAr = spResult.FirstOrDefault().AccountNameAr;
                accSummary.DiffAmount = accSummary.DebitAmout - accSummary.CreditAmount;

                var grouped = spResult.GroupBy(x => x.CurrencySign)
                        .Select(x => new
                        {
                            Currency = x.Key,
                            DebitSum = x.Sum(y => y.DebitAmount),
                            CreditSum = x.Sum(y => y.CreditAmount)
                        }).ToList();

                List<AccountSummaryTotal> sumByCurrency = new List<AccountSummaryTotal>();
                AccountSummaryTotal accountSummaryTotal;
                decimal diffAmount;
                foreach (var item in grouped)
                {
                    accountSummaryTotal = new AccountSummaryTotal();
                    accountSummaryTotal.CurrencySign = item.Currency;
                    diffAmount = item.DebitSum - item.CreditSum;
                    if (diffAmount > 0)
                        accountSummaryTotal.DebitAmout = diffAmount;
                    else
                        accountSummaryTotal.CreditAmount = diffAmount * -1;

                    sumByCurrency.Add(accountSummaryTotal);
                }

                accSummary.TotalByCurrency = sumByCurrency;
            }

            return accSummary;
        }

        public static JObject GetAccTransByAccId(string accId, string fromDate = null, string toDate = null)
        {
            List<AccountSummary> accSummary = new List<AccountSummary>();
            ReportsAccountingEntities db = new ReportsAccountingEntities();
            var spResult = db.GetAccountDetailsByAccId(accId, fromDate == "" ? null : fromDate, toDate == "" ? null : toDate).ToList();

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in spResult)
            {
                pJTokenWriter.WriteStartObject();
                pJTokenWriter.WritePropertyName("AccountId");
                pJTokenWriter.WriteValue(item.AccountId);

                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("CreateBy");
                pJTokenWriter.WriteValue(item.UserName);

                pJTokenWriter.WritePropertyName("DebitAmount");
                pJTokenWriter.WriteValue(item.DebitAmount);

                pJTokenWriter.WritePropertyName("CreditAmount");
                pJTokenWriter.WriteValue(item.CreditAmount);

                pJTokenWriter.WritePropertyName("CurrencySign");
                pJTokenWriter.WriteValue(item.CurrencySign);

                pJTokenWriter.WritePropertyName("TransactionName");
                pJTokenWriter.WriteValue(item.TransactionName);

                pJTokenWriter.WritePropertyName("TransactionNameAr");
                pJTokenWriter.WriteValue(item.TransactionNameAr);

                pJTokenWriter.WritePropertyName("ReceiptNotes");
                //pJTokenWriter.WriteValue(item.ReceiptNotes);
                pJTokenWriter.WriteValue("");

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;

        }

        #endregion

        public static List<AccRpt_TopViewBalance2_Result> GetTopViewBalanceRpt(DateTime? fromDate = null,
                                                                               DateTime? toDate = null)
        {
            ReportsAccountingEntities db = new ReportsAccountingEntities();
            var result = db.AccRpt_TopViewBalance2(fromDate, toDate).ToList();

            return result;
        }

        public static Dictionary<string, decimal> GetTopBalanceTotals(List<AccRpt_TopViewBalance2_Result> balanceList)
        {
            Dictionary<string, decimal> balanceTotals = new Dictionary<string, decimal>();
            //Debit Totals
            decimal dTotalEGP = balanceList
                .Where(x => x.ParentAccountId.StartsWith("1") || x.ParentAccountId == "287") //Add Deposit Revenues
               .Sum(x => (x.EGP == null ? 0 : x.EGP.Value));

            decimal dTotalUSD = balanceList
                .Where(x => x.ParentAccountId.StartsWith("1") || x.ParentAccountId == "287")
               .Sum(x => (x.USD == null ? 0 : x.USD.Value));

            decimal dTotalEUR = balanceList
               .Where(x => x.ParentAccountId.StartsWith("1") || x.ParentAccountId == "287")
              .Sum(x => (x.EUR == null ? 0 : x.EUR.Value));

            decimal dTotalGBP = balanceList
               .Where(x => x.ParentAccountId.StartsWith("1") || x.ParentAccountId == "287")
              .Sum(x => (x.GBP == null ? 0 : x.GBP.Value));

            //Credit Totals
            decimal cTotalEGP = balanceList
               .Where(x => x.ParentAccountId.StartsWith("2") && x.ParentAccountId != "287") //Ignor Deposit Revenues
              .Sum(x => (x.EGP == null ? 0 : x.EGP.Value));

            decimal cTotalUSD = balanceList
                .Where(x => x.ParentAccountId.StartsWith("2") && x.ParentAccountId != "287")
               .Sum(x => (x.USD == null ? 0 : x.USD.Value));

            decimal cTotalEUR = balanceList
               .Where(x => x.ParentAccountId.StartsWith("2") && x.ParentAccountId != "287")
              .Sum(x => (x.EUR == null ? 0 : x.EUR.Value));

            decimal cTotalGBP = balanceList
               .Where(x => x.ParentAccountId.StartsWith("2") && x.ParentAccountId != "287")
              .Sum(x => (x.GBP == null ? 0 : x.GBP.Value));

            balanceTotals.Add("EGP", dTotalEGP - cTotalEGP);
            balanceTotals.Add("USD", dTotalUSD - cTotalUSD);
            balanceTotals.Add("EUR", dTotalEUR - cTotalEUR);
            balanceTotals.Add("GBP", dTotalGBP - cTotalGBP);

            //var values = Enum.GetValues(typeof(AccountingChartEnum));

            //string sss = values.GetValue(0).ToString();



            return balanceTotals;

        }

        /// <summary>
        /// Use the GetAccSummaryByParentAccId SP to get data for all bank accounts
        /// Then join to banks table to get data grouped by bank name 
        /// </summary>
        /// <param name="parentAccId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        private static List<AccountSummary> GetBankListRpt(string parentAccId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<AccountSummary> accSummaryList = new List<AccountSummary>();
            var spResult = GetAccSummaryByParentAccId(parentAccId, false, fromDate, toDate);
            //Get Bank List 
            AccountingEntities db = new AccountingEntities();
            var bankList = db.BankAccounts.Include("Bank").ToList();

            var temp = from sp in spResult
                       join b in bankList
                       on sp.AccountId equals b.AccountId
                       group sp by new { sp.CurrencySign, b.Bank.BankNameEn, b.Bank.BankId } into g
                       select new
                       {
                           AccountId = g.Key.BankId,
                           AccountNameAr = g.Key.BankNameEn,
                           AccountNameEn = g.Key.BankNameEn,
                           DiffAmount = g.Sum(t3 => t3.DiffAmount),
                           DebitAmout = g.Sum(t3 => t3.DebitAmout),
                           CreditAmount = g.Sum(t3 => t3.CreditAmount),
                           CurrencySign = g.Key.CurrencySign
                       };

            AccountSummary accountSummary;
            foreach (var item in temp)
            {
                accountSummary = new AccountSummary();
                accountSummary.AccountId = item.AccountId.ToString();
                accountSummary.AccountNameAr = item.AccountNameAr;
                accountSummary.AccountNameEn = item.AccountNameEn;
                accountSummary.DebitAmout = item.DebitAmout;
                accountSummary.CreditAmount = item.CreditAmount;
                accountSummary.DiffAmount = item.DiffAmount;
                accountSummary.CurrencySign = item.CurrencySign;

                accSummaryList.Add(accountSummary);
            }

            return accSummaryList;
        }



        public static List<CashBankRptVm> GetBankCashRpt()
        {
            List<CashBankRptVm> cashBankBalance = CashBankRptModels.Instance.GetCashBankRpt();
            return cashBankBalance;
        }


    }
}