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
    
    public partial class CarrierRate
    {
        public CarrierRate()
        {
            this.CarrierRateTransits = new HashSet<CarrierRateTransit>();
        }
    
        public int CarrierRateId { get; set; }
        public int CarrierId { get; set; }
        public int FromPortId { get; set; }
        public int ToPortId { get; set; }
        public int FromCountryId { get; set; }
        public int ToCountryId { get; set; }
        public int ContainerTypeId { get; set; }
        public Nullable<decimal> FreightCostAmount { get; set; }
        public Nullable<int> FreighCurrencyId { get; set; }
        public Nullable<decimal> ThcCostAmount { get; set; }
        public Nullable<int> ThcCurrencyId { get; set; }
        public Nullable<bool> IsValid { get; set; }
        public Nullable<bool> ValidToChecked { get; set; }
        public Nullable<System.DateTime> ValidToDate { get; set; }
    
        public virtual Carrier Carrier { get; set; }
        public virtual ContainerType ContainerType { get; set; }
        public virtual Country Country { get; set; }
        public virtual Country Country1 { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Currency Currency1 { get; set; }
        public virtual Port Port { get; set; }
        public virtual Port Port1 { get; set; }
        public virtual ICollection<CarrierRateTransit> CarrierRateTransits { get; set; }
    }
}
