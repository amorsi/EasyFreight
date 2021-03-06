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
    
    public partial class ArCashDepositBalance
    {
        public int BalanceId { get; set; }
        public Nullable<int> ShipperId { get; set; }
        public Nullable<int> ConsigneeId { get; set; }
        public int CurrencyId { get; set; }
        public decimal BalanceAmount { get; set; }
    
        public virtual ConsigneeAcc Consignee { get; set; }
        public virtual CurrencyAcc Currency { get; set; }
        public virtual ShipperAcc Shipper { get; set; }
    }
}
