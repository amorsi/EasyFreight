using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class OpenBalancePaymentVm
    {

        public string AccountId { get; set; }
        public int CurrencyId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Amount { get; set; }
        public int IsCredit { get; set; }
        public string AccountNameEn { get; set; }
        public string AccountNameAr { get; set; }
        public string CurrencySign { get; set; }
        public string CurrencyName { get; set; }
    }
}