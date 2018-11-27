using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class OperationBaseVm
    {

        public byte OrderFrom { get; set; }
        public byte CarrierType { get; set; }
        public int FromPortId { get; set; }
        public int ToPortId { get; set; }
        public int ShipperId { get; set; }
        public int ConsigneeId { get; set; }
        public int? NotifierId { get; set; }
        public bool NotifierAsConsignee { get; set; }
        public int CarrierId { get; set; }
        public int? IncotermId { get; set; }
        public int? VesselId { get; set; }
        public DateTime? DateOfDeparture { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? CBM { get; set; }
        public int? NumberOfPackages { get; set; }
        //public bool IsHazardous { get; set; }
        public bool IsPrepaid { get; set; }
        public bool PickupNeeded { get; set; }
        public bool DeliveryNeeded { get; set; }
        public bool CustomClearanceNeeded { get; set; }
        public string GoodsDescription { get; set; }
        public byte StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public decimal? FreightCostAmount { get; set; }
        public int? FreighCurrencyId { get; set; }
        public decimal? ThcCostAmount { get; set; }
        public int? ThcCurrencyId { get; set; }
        public string FlightNumber { get; set; }
        public bool IsCareOf { get; set; }
        public DateTime? ArriveDate { get; set; }
        public byte? TransitTime { get; set; }
        public byte? FreeDays { get; set; }

        public OperationBaseVm()
        {
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
        }
    }
}