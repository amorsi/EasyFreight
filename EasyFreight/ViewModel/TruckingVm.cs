using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class TruckingOrderDetailVm
    {
        public int TruckingOrderId { get; set; }
        public string TruckingOrderCode { get; set; }
        public int OperationId { get; set; }
        public int OrderFrom { get; set; }
        public string OperationCode { get; set; }
        public string BookingNo { get; set; }
        public int FromAreaId { get; set; }
        public int ToAreaId { get; set; }
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }
        public int ShipperId { get; set; }
        public int CarrierId { get; set; }
        public int ConsigneeId { get; set; }
        public DateTime NeedArriveDate { get; set; }
        public TimeSpan NeedArriveTime { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public int StatusId { get; set; }
        public string AssignToUserId { get; set; }
        public string ContractorNameEn { get; set; }

        public string ClientName { get; set; }
        public string FromAreaName { get; set; }
        public string ToAreaName { get; set; } 

        public int ContractorId { get; set; }
        public string DriverName { get; set; }
        public string DriverMobile { get; set; }
        public DateTime? ArriveDate { get; set; }
        public TimeSpan? ArriveTime { get; set; }
        public DateTime? LeaveDate { get; set; }
        public TimeSpan? LeaveTime { get; set; }
        public DateTime? LoadedDate { get; set; }
        public TimeSpan? LoadedTime { get; set; }
        public string Notes { get; set; } 
        public DateTime? FollowUpDate { get; set; }
        public int? FollowUpBy { get; set; }
        public int? CostCurrencyId { get; set; }
        public string ShipFromAddress { get; set; }
        public string CarrierNameEn { get; set; }

        public decimal? TotalCostNet { get; set; }
        public decimal? TotalCostSelling { get; set;}

        public string ContainersSummary { get; set; }
        public DateTime HbDate { get; set; }


        public List<TruckingCostVm> TruckingOrderCosts { get; set; }
        public List<OperationContainerVm> OperationContainers { get; set; }

        public TruckingOrderDetailVm(int truckOrderId)
        {
            TruckingOrderCosts = new List<TruckingCostVm>();
            OperationContainers = new List<OperationContainerVm>();
            CreateDate = DateTime.Now;
            FollowUpDate = DateTime.Now;
            TruckingOrderId = truckOrderId;
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
            
        }

        public TruckingOrderDetailVm()
        {

        }

        public enum OrderForm
        {
            Export = 1,
            Import = 2
        }

        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
    }

    public class TruckingCostVm
    {
        public int TruckingOrderCostId { get; set; }
        public int TruckingOrderId { get; set; }
        public int ContractorId { get; set; }
        public int TruckingCostId { get; set; }
        public decimal? TruckingCostNet { get; set; }
        public decimal? TruckingCostSelling { get; set; }
        public int CurrencyId { get; set; }

        public string TruckingCostName { get; set; }
        public string CurrencySign { get; set; }

        public TruckingCostVm(int truckOrderId)
        {
            TruckingOrderId = truckOrderId;
        }

        public TruckingCostVm()
        {

        }
    }

    public class TruckingOrderVm
    {
        public int TruckingOrderId { get; set; }
        public int OperationId { get; set; }
        public int OrderFrom { get; set; }
        public string OperationCode { get; set; }
        public string BookingNo { get; set; }
        public int FromAreaId { get; set; }
        public int ToAreaId { get; set; }
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }
        public int ShipperId { get; set; }
        public int CarrierId { get; set; }
        public int ConsigneeId { get; set; }
        public DateTime? NeedArriveDate { get; set; }
        public TimeSpan? NeedArriveTime { get; set; }
        public string ShipFromAddress { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public int StatusId { get; set; }
        public string AssignToUserId { get; set; }
        public int HouseBillId { get; set; }
        public string HouseBL { get; set; }
        public DateTime OperationDate { get; set; }

        public string ContactName { get; set; }
        public string ContactPhone { get; set; }

        public TruckingOrderVm()
        {

        }

        public TruckingOrderVm(byte orderFrom)
        {
            OrderFrom = orderFrom;
            CreateDate = DateTime.Now;
            StatusId = 1;
            
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
        }

        public enum OrderForm
        {
            Export = 1,
            Import = 2
        }

        
    }

    public class TruckingOrderListVm
    {
        public int TruckingOrderId { get; set; }
        public string TruckingOrderCode { get; set; }
        public string OperationCode { get; set; }
        public string HouseBL { get; set; }
        public int ShipperId { get; set; }
        public int ConsigneeId { get; set; }
        public string ClientName { get; set; }
        public int? NumberOfPackages { get; set; }
        public DateTime NeedArriveDate { get; set; }
        public TimeSpan NeedArriveTime { get; set; }
        public int HouseBillId { get; set; }
        public int StatusId { get; set; }
    }

    public class TruckingNewSummary
    {
        public int TruckingOrderId { get; set; }
        public int OperationId { get; set; }
        public byte OrderFrom { get; set; }
        public string OrderFromImg { get; set; }
        public string NeedArrive { get; set; }
        public string ClientName { get; set; }
        public string FromArea { get; set; }
        public string ToArea { get; set; }
        public string LabelClass { get; set; } //  label-success OR  label-warning OR  label-danger

    }

    public class TurckingTopSummary
    {
        public int ContractorId { get; set; }
        public string ContractorNameEn { get; set; }
        public int OpCount { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
         public int ContainersCount { get; set; }
        public decimal TotalNetCost { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySign { get; set; }
        public int CurrencyId { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalSellingCost { get; set; }
    }

}