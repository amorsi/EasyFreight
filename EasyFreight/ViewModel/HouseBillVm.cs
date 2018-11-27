using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class HouseBillVm : OperationBaseVm
    {
        public int HouseBillId { get; set; }
        public int OperationId { get; set; }
        public string OperationCode { get; set; }
        public string HouseBL { get; set; }
        public DateTime? OperationDate { get; set; }
        public int? AgentId { get; set; }
        public string Marks { get; set; }
        public byte DeliveryType { get; set; }
        public decimal? CollectedFreightCost { get; set; }
        public decimal? CollectedThcCost { get; set; }
        public DateTime OperationMinDate { get; set; }
        public int? PackageTypeId { get; set; }
        public bool IsHazardous { get; set; }

        public HouseBillVm()
        {

        }

        public HouseBillVm(byte orderFrom)
        {
            OrderFrom = orderFrom;
            CreateDate = DateTime.Now;
            OperationDate = DateTime.Now;
            StatusId = 1;
        }

    }

    public class HouseBillListVm
    {
        public int HouseBillId { get; set; }
        public string OperationCode { get; set; }
        public string HouseBL { get; set; }
        public DateTime? OperationDate { get; set; }
        public string ClientName { get; set; }
        public int? NumberOfPackages { get; set; }
        public int ShipperId { get; set; }
        public int ConsigneeId { get; set; }
        public int OperationId { get; set; }
        public byte CarrierType { get; set; }
        public byte StatusId { get; set; }
        public decimal? FreightCostAmount { get; set; }
        public string FreighCurrencySign { get; set; }
        public decimal? ThcCostAmount { get; set; }
        public string ThcCurrencySign { get; set; }
        public int? AgentId { get; set; }
        public List<InvoiceLightVm> InvoiceList { get; set; }

        public HouseBillListVm()
        {
            InvoiceList = new List<InvoiceLightVm>();
        }
    }


}