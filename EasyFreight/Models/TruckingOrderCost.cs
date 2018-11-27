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
    
    public partial class TruckingOrderCost
    {
        public int TruckingOrderCostId { get; set; }
        public int TruckingOrderId { get; set; }
        public int TruckingCostId { get; set; }
        public int ContractorId { get; set; }
        public decimal TruckingCostNet { get; set; }
        public decimal TruckingCostSelling { get; set; }
        public int CurrencyId { get; set; }
    
        public virtual Currency Currency { get; set; }
        public virtual TruckingCostLib TruckingCostLib { get; set; }
        public virtual TruckingOrderDetail TruckingOrderDetail { get; set; }
    }
}