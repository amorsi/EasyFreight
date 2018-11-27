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
    
    public partial class Quotation
    {
        public Quotation()
        {
            this.QuotationContainers = new HashSet<QuotationContainer>();
            this.Operations = new HashSet<Operation>();
        }
    
        public int QuoteId { get; set; }
        public byte OrderFrom { get; set; }
        public string QuoteCode { get; set; }
        public Nullable<System.DateTime> QuoteDate { get; set; }
        public byte CarrierType { get; set; }
        public Nullable<int> FromPortId { get; set; }
        public Nullable<int> ToPortId { get; set; }
        public Nullable<int> ShipperId { get; set; }
        public Nullable<int> ConsigneeId { get; set; }
        public Nullable<int> NotifierId { get; set; }
        public Nullable<bool> NotifierAsConsignee { get; set; }
        public Nullable<int> CarrierId { get; set; }
        public Nullable<int> IncotermId { get; set; }
        public Nullable<int> VesselId { get; set; }
        public Nullable<System.DateTime> DateOfDeparture { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public Nullable<decimal> CBM { get; set; }
        public Nullable<int> NumberOfPackages { get; set; }
        public bool IsHazardous { get; set; }
        public bool IsPrepaid { get; set; }
        public Nullable<bool> PickupNeeded { get; set; }
        public Nullable<bool> DeliveryNeeded { get; set; }
        public Nullable<bool> CustomClearanceNeeded { get; set; }
        public string GoodsDescription { get; set; }
        public byte StatusId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<bool> HasExpireDate { get; set; }
        public Nullable<byte> ExpirationInDays { get; set; }
        public Nullable<decimal> FreightCostAmount { get; set; }
        public Nullable<int> FreighCurrencyId { get; set; }
        public Nullable<decimal> ThcCostAmount { get; set; }
        public Nullable<int> ThcCurrencyId { get; set; }
        public string FlightNumber { get; set; }
        public bool IsCareOf { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<decimal> TruckingCost { get; set; }
        public Nullable<int> TruckingCurrencyId { get; set; }
        public Nullable<decimal> CusClearanceCost { get; set; }
        public Nullable<int> CusClearanceCurrencyId { get; set; }
        public Nullable<byte> TransitTime { get; set; }
        public Nullable<byte> FreeDays { get; set; }
        public Nullable<System.DateTime> ArriveDate { get; set; }
    
        public virtual ICollection<QuotationContainer> QuotationContainers { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual CurrencyOper Currency { get; set; }
        public virtual CurrencyOper Currency1 { get; set; }
        public virtual CurrencyOper Currency2 { get; set; }
        public virtual CurrencyOper Currency3 { get; set; }
        public virtual StatusLibOper StatusLib { get; set; }
    }
}
