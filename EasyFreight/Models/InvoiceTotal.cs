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
    
    public partial class InvoiceTotal
    {
        public int InvoiceId { get; set; }
        public int CurrencyId { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencySign { get; set; }
        public decimal TaxDepositAmount { get; set; }
        public decimal TotalBeforeTax { get; set; }
        public decimal VatTaxAmount { get; set; }
    
        public virtual CurrencyAcc Currency { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}