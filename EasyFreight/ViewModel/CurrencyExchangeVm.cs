using System;
using System.Collections.Generic;

namespace EasyFreight.ViewModel
{
    public class CurrencyExchangeVm
    {
        public int CurrentCurrencyId { get; set; }
        public decimal CurrentAmount { get; set; }
        public int NewCurrencyId { get; set; }
        public decimal NewAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Notes { get; set; }
        public Dictionary<int, string> CurrencyList { get; set; }

        public CurrencyExchangeVm()
        {
            CreateDate = DateTime.Now;
            CreateBy = DAL.AdminHelper.GetCurrentUserId();
            Notes = "Currency Exchange";
            CurrencyList = DAL.ListCommonHelper.GetCurrencyList();
        }
    }


    public class CashBankTransferVm
    {
        public int CurrentCurrencyId { get; set; }
        public decimal CurrentAmount { get; set; }
        public int BankAccId { get; set; }
        public string AccountId { get; set; }
        public bool IsCashToBank { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Notes { get; set; }
        public Dictionary<int, string> CurrencyList { get; set; }

        public CashBankTransferVm()
        {
            CreateDate = DateTime.Now;
            CreateBy = DAL.AdminHelper.GetCurrentUserId();
            Notes = IsCashToBank ? "Cash To Bank" : "Bank To Cash";
            CurrencyList = DAL.ListCommonHelper.GetCurrencyList();
        }
    }

}