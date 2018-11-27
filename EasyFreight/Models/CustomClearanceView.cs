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
    
    public partial class CustomClearanceView
    {
        public byte OrderFrom { get; set; }
        public string OperationCode { get; set; }
        public Nullable<System.DateTime> OperationDate { get; set; }
        public string FromPort { get; set; }
        public string ToPort { get; set; }
        public string ShipperNameEn { get; set; }
        public string ConsigneeNameEn { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public string GoodsDescription { get; set; }
        public int OperationId { get; set; }
        public System.DateTime NeedArriveDate { get; set; }
        public System.TimeSpan NeedArriveTime { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public byte StatusId { get; set; }
        public string StatusName { get; set; }
        public int CCId { get; set; }
        public string AssignToUserId { get; set; }
        public byte CarrierType { get; set; }
        public string MBL { get; set; }
        public int CarrierId { get; set; }
        public string CarrierNameEn { get; set; }
        public Nullable<System.DateTime> DateOfDeparture { get; set; }
        public Nullable<System.DateTime> ArriveDate { get; set; }
        public bool IsHazardous { get; set; }
        public Nullable<byte> FreeDays { get; set; }
        public string FlightNumber { get; set; }
        public Nullable<int> VesselId { get; set; }
        public string VesselName { get; set; }
        public string BookingNumber { get; set; }
        public Nullable<decimal> CBM { get; set; }
        public Nullable<int> NumberOfPackages { get; set; }
        public string HouseBL { get; set; }
        public string Notes { get; set; }
        public int FromPortId { get; set; }
        public int ToPortId { get; set; }
        public int ShipperId { get; set; }
        public int ConsigneeId { get; set; }
        public Nullable<int> NotifierId { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<System.DateTime> Expr1 { get; set; }
        public Nullable<System.DateTime> Expr2 { get; set; }
        public string Comment { get; set; }
    }
}