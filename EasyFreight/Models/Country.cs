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
    
    public partial class Country
    {
        public Country()
        {
            this.Agents = new HashSet<Agent>();
            this.Carriers = new HashSet<Carrier>();
            this.CarrierRates = new HashSet<CarrierRate>();
            this.CarrierRates1 = new HashSet<CarrierRate>();
            this.Cities = new HashSet<City>();
            this.Consignees = new HashSet<Consignee>();
            this.Contractors = new HashSet<Contractor>();
            this.Notifiers = new HashSet<Notifier>();
            this.Ports = new HashSet<Port>();
            this.Shippers = new HashSet<Shipper>();
            this.Areas = new HashSet<Area>();
        }
    
        public int CountryId { get; set; }
        public string CountryNameEn { get; set; }
        public string CountryNameAr { get; set; }
    
        public virtual ICollection<Agent> Agents { get; set; }
        public virtual ICollection<Carrier> Carriers { get; set; }
        public virtual ICollection<CarrierRate> CarrierRates { get; set; }
        public virtual ICollection<CarrierRate> CarrierRates1 { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Consignee> Consignees { get; set; }
        public virtual ICollection<Contractor> Contractors { get; set; }
        public virtual ICollection<Notifier> Notifiers { get; set; }
        public virtual ICollection<Port> Ports { get; set; }
        public virtual ICollection<Shipper> Shippers { get; set; }
        public virtual ICollection<Area> Areas { get; set; }
    }
}
