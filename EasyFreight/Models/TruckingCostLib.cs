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
    
    public partial class TruckingCostLib
    {
        public TruckingCostLib()
        {
            this.TruckingOrderCosts = new HashSet<TruckingOrderCost>();
        }
    
        public int TruckingCostId { get; set; }
        public string TruckingCostName { get; set; }
        public string TruckingCostNameAr { get; set; }
    
        public virtual ICollection<TruckingOrderCost> TruckingOrderCosts { get; set; }
    }
}
