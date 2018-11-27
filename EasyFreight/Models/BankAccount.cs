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
    
    public partial class BankAccount
    {
        public BankAccount()
        {
            this.AgentNotes = new HashSet<AgentNote>();
            this.CashInReceipts = new HashSet<CashInReceipt>();
            this.CashOutReceipts = new HashSet<CashOutReceipt>();
        }
    
        public int BankAccId { get; set; }
        public int BankId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountId { get; set; }
        public int CurrencyId { get; set; }
    
        public virtual Bank Bank { get; set; }
        public virtual CurrencyAcc Currency { get; set; }
        public virtual ICollection<AgentNote> AgentNotes { get; set; }
        public virtual ICollection<CashInReceipt> CashInReceipts { get; set; }
        public virtual ICollection<CashOutReceipt> CashOutReceipts { get; set; }
    }
}