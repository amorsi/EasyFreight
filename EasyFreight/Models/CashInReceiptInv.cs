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
    
    public partial class CashInReceiptInv
    {
        public int ReceiptId { get; set; }
        public int InvoiceId { get; set; }
        public decimal PaidAmount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
    
        public virtual CashInReceipt CashInReceipt { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
