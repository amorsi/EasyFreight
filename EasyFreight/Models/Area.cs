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
    
    public partial class Area
    {
        public Area()
        {
            this.ContractorRates = new HashSet<ContractorRate>();
            this.ContractorRates1 = new HashSet<ContractorRate>();
        }
    
        public int AreaId { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string AreaNameEn { get; set; }
        public string AreaNameAr { get; set; }
    
        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<ContractorRate> ContractorRates { get; set; }
        public virtual ICollection<ContractorRate> ContractorRates1 { get; set; }
    }
}