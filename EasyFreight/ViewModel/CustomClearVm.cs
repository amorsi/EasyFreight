using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CustomClearVm
    {
        public int CCId { get; set; }
        public int OperationId { get; set; }
        public DateTime? NeedArriveDate { get; set; }
        public TimeSpan? NeedArriveTime { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public int StatusId { get; set; }
        public string AssignToUserId { get; set; }
        public int HouseBillId { get; set; }
        public string Notes { get; set; } 
        public string Comment { get; set; }

        public CustomClearVm(int operationId, int houseBillId)
        {
            CreateDate = DateTime.Now;
            OperationId = operationId;
            StatusId = 1;
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
        }

        public CustomClearVm()
        {

        }
    }

    public class CustomClearanceDetailMainVm
    {
        public List<CustomClearanceDetailVm> CustomClearanceDetailVms { get; set; }
        public CustomClearanceDetailMainVm()
        {
            CustomClearanceDetailVms = new List<CustomClearanceDetailVm>();
        }

    }

    public class CustomClearanceDetailVm
    {
        public int CCDetailsId { get; set; }
        public int CCId { get; set; }
        public int CCCostId { get; set; }
        public string CCCostName { get; set; } 
        public decimal CCCostNet { get; set; }
        public decimal CCCostSelling { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
        public string Notes { get; set; }
        public string Comment { get; set; }
    }

    public class CustomClearanceListVm
    {
        public int CCId { get; set; }
        public string OperationCode { get; set; }
        public string HouseBL { get; set; }
        public int ShipperId { get; set; }
        public int ConsigneeId { get; set; }
        public string ClientName { get; set; }
        public int? NumberOfPackages { get; set; }
        public DateTime NeedArriveDate { get; set; }
        public TimeSpan NeedArriveTime { get; set; }
        public int HouseBillId { get; set; }
        public int OperationId { get; set; }
        public byte StatusId { get; set; }
     }


}