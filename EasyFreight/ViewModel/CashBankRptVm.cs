using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CashBankRptVm
    {
        public string AccountId { get; set; }
        public string AccountNameEn { get; set; }
        public string AccountNameAr { get; set; }
        public decimal EGP { get; set; }
        public decimal USD { get; set; }
        public decimal EUR { get; set; }
        public decimal GBP { get; set; }
    }
}