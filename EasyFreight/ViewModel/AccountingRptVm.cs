using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class AccountSummaryMain
    {
        public List<AccountSummary> ArSummary { get; set; }
        public List<AccountSummary> ApSummary { get; set; }
        public List<AccountSummary> AgentSummary { get; set; }
        public List<AccountSummary> CashInBankSummary { get; set; }
        public List<AccountSummary> ApCarrierSummary { get; set; }
        public List<AccountSummary> ApContractorSummary { get; set; }
        public List<AccountSummary> TreasurySummary { get; set; }
        public List<Models.AccRpt_TopViewBalance2_Result> TopViewBalance { get; set; }

        public Dictionary<string, decimal> TotalCollectedByCurrency { get; set; }
        public Dictionary<string, decimal> TotalPaidByCurrency { get; set; }

    }

    public class AccountSummary
    {
        public string AccountId { get; set; }
        public string AccountNameAr { get; set; }
        public string AccountNameEn { get; set; }
        public decimal DebitAmout { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal DiffAmount { get; set; }
        public string CurrencySign { get; set; }
        public string TransactionName { get; set; }
        public string TransactionNameAr { get; set; }
        public string ReceiptNotes { get; set; }

        public List<AccountSummaryTotal> TotalByCurrency { get; set; }

    }

    public class AccountSummaryTotal
    {
        public string CurrencySign { get; set; }
        public decimal DebitAmout { get; set; }
        public decimal CreditAmount { get; set; }
    }


}