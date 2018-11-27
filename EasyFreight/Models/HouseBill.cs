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
    
    public partial class HouseBill
    {
        public HouseBill()
        {
            this.CustomClearanceOrders = new HashSet<CustomClearanceOrder>();
            this.OperationCosts = new HashSet<OperationCost>();
        }
    
        public int HouseBillId { get; set; }
        public byte OrderFrom { get; set; }
        public int OperationId { get; set; }
        public string OperationCode { get; set; }
        public Nullable<System.DateTime> OperationDate { get; set; }
        public string HouseBL { get; set; }
        public int FromPortId { get; set; }
        public int ToPortId { get; set; }
        public int ShipperId { get; set; }
        public int ConsigneeId { get; set; }
        public Nullable<int> NotifierId { get; set; }
        public Nullable<bool> NotifierAsConsignee { get; set; }
        public byte DeliveryType { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public Nullable<decimal> CBM { get; set; }
        public Nullable<int> NumberOfPackages { get; set; }
        public Nullable<int> PackageTypeId { get; set; }
        public Nullable<bool> IsPrepaid { get; set; }
        public Nullable<decimal> CollectedFreightCost { get; set; }
        public Nullable<int> FreighCurrencyId { get; set; }
        public Nullable<decimal> CollectedThcCost { get; set; }
        public Nullable<int> ThcCurrencyId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string GoodsDescription { get; set; }
        public string Marks { get; set; }
        public byte StatusId { get; set; }
        public string BookingNumber { get; set; }
        public Nullable<bool> IsHazardous { get; set; }
    
        public virtual Operation Operation { get; set; }
        public virtual ICollection<CustomClearanceOrder> CustomClearanceOrders { get; set; }
        public virtual ICollection<OperationCost> OperationCosts { get; set; }
        public virtual CurrencyOper Currency { get; set; }
        public virtual CurrencyOper Currency1 { get; set; }
        public virtual StatusLibOper StatusLib { get; set; }
    }
}
