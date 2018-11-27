using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class BankVm
    {
        public int BankId { get; set; }
        public string BankNameEn { get; set; }
        public string BankNameAr { get; set; }
        public string BankAddressEn { get; set; }
        public string BankAddressAr { get; set; }
        public string SwiftCode { get; set; }
        public string AccountId { get; set; }

        public List<BankAccountVm> BankAccounts { get; set; }

        public BankVm()
        {
            BankAccounts = new List<BankAccountVm>();
        }

    }

    public class BankAccountVm
    {
        public int BankAccId { get; set; }
        public int BankId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
    }

    public class BankDetailsVm : BankVm
    {
        public int BankAccId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }


}