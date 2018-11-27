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
    
    public partial class AccTransaction
    {
        public AccTransaction()
        {
            this.AccTransactionDetails = new HashSet<AccTransactionDetail>();
            this.Invoices = new HashSet<Invoice>();
            this.CashInReceipts = new HashSet<CashInReceipt>();
            this.CashOutReceipts = new HashSet<CashOutReceipt>();
            this.CashInReceiptChecks = new HashSet<CashInReceiptCheck>();
            this.CashOutReceiptChecks = new HashSet<CashOutReceiptCheck>();
        }
    
        public int TransId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string TransactionName { get; set; }
        public string TransactionNameAr { get; set; }
    
        public virtual ICollection<AccTransactionDetail> AccTransactionDetails { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<CashInReceipt> CashInReceipts { get; set; }
        public virtual ICollection<CashOutReceipt> CashOutReceipts { get; set; }
        public virtual ICollection<CashInReceiptCheck> CashInReceiptChecks { get; set; }
        public virtual ICollection<CashOutReceiptCheck> CashOutReceiptChecks { get; set; }
    }
}
