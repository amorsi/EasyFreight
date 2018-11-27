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
    
    public partial class ContainerType
    {
        public ContainerType()
        {
            this.CarrierRates = new HashSet<CarrierRate>();
            this.ContractorRates = new HashSet<ContractorRate>();
        }
    
        public int ContainerTypeId { get; set; }
        public string ContainerTypeName { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal MaxCBM { get; set; }
    
        public virtual ICollection<CarrierRate> CarrierRates { get; set; }
        public virtual ICollection<ContractorRate> ContractorRates { get; set; }
    }
}
