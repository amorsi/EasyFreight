//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasyFreight.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CurrencyAcc
    {
        public CurrencyAcc()
        {
            this.HouseBills = new HashSet<HouseBillAcc>();
            this.HouseBills1 = new HashSet<HouseBillAcc>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.AccTransactionDetails = new HashSet<AccTransactionDetail>();
            this.InvoiceTotals = new HashSet<InvoiceTotal>();
            this.Invoices = new HashSet<Invoice>();
            this.InvoiceDetails = new HashSet<InvoiceDetail>();
            this.InvoiceDetails1 = new HashSet<InvoiceDetail>();
            this.CashInReceipts = new HashSet<CashInReceipt>();
            this.InvoiceTotalAPs = new HashSet<InvoiceTotalAP>();
            this.CashOutReceipts = new HashSet<CashOutReceipt>();
            this.InvoiceAPs = new HashSet<InvoiceAP>();
            this.InvoiceDetailAPs = new HashSet<InvoiceDetailAP>();
            this.InvoiceDetailAPs1 = new HashSet<InvoiceDetailAP>();
            this.ArCashDeposits = new HashSet<ArCashDeposit>();
            this.ArCashDepositBalances = new HashSet<ArCashDepositBalance>();
        }
    
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
        public string CurrencyName { get; set; }
        public string AccountId { get; set; }
    
        public virtual ICollection<HouseBillAcc> HouseBills { get; set; }
        public virtual ICollection<HouseBillAcc> HouseBills1 { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<AccTransactionDetail> AccTransactionDetails { get; set; }
        public virtual ICollection<InvoiceTotal> InvoiceTotals { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails1 { get; set; }
        public virtual ICollection<CashInReceipt> CashInReceipts { get; set; }
        public virtual ICollection<InvoiceTotalAP> InvoiceTotalAPs { get; set; }
        public virtual ICollection<CashOutReceipt> CashOutReceipts { get; set; }
        public virtual ICollection<InvoiceAP> InvoiceAPs { get; set; }
        public virtual ICollection<InvoiceDetailAP> InvoiceDetailAPs { get; set; }
        public virtual ICollection<InvoiceDetailAP> InvoiceDetailAPs1 { get; set; }
        public virtual ICollection<ArCashDeposit> ArCashDeposits { get; set; }
        public virtual ICollection<ArCashDepositBalance> ArCashDepositBalances { get; set; }
    }
}
