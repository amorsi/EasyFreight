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
    
    public partial class CashInReceiptCheck
    {
        public int CashInCheckId { get; set; }
        public int ReceiptId { get; set; }
        public string CheckNumber { get; set; }
        public System.DateTime CheckDueDate { get; set; }
        public bool IsCollected { get; set; }
        public Nullable<int> TransId { get; set; }
        public Nullable<System.DateTime> CollectDate { get; set; }
        public string CollectBy { get; set; }
        public Nullable<int> BankId { get; set; }
    
        public virtual CashInReceipt CashInReceipt { get; set; }
        public virtual AccTransaction AccTransaction { get; set; }
    }
}
