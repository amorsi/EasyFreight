using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class QuotationVm : OperationBaseVm
    {
        public int QuoteId { get; set; }
       
        public string QuoteCode { get; set; }
        public DateTime? QuoteDate { get; set; }

        public bool HasExpireDate { get; set; }
        public byte? ExpirationInDays { get; set; }
        public int? AgentId { get; set; }

        public decimal? TruckingCost { get; set; }
        public int? TruckingCurrencyId { get; set; }

        public decimal? CusClearanceCost { get; set; }
        public int? CusClearanceCurrencyId { get; set; }
        public string PackagesType { get; set; }

        public List<QuotationContainerVm> QuotationContainers { get; set; }
        public QuotationVm(byte orderFrom)
        {
            QuotationContainers = new List<QuotationContainerVm>();
            StatusId = 1;
            OrderFrom = orderFrom;
            CreateDate = DateTime.Now;
            QuoteDate = DateTime.Now;
            CarrierType = 1;
            if (orderFrom == 2)
            {
                NotifierAsConsignee = true;
                QuoteCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.QuoteImport, false);
            }
            else
                QuoteCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.QuoteExport, false);
        }
        public QuotationVm()
        {
            QuotationContainers = new List<QuotationContainerVm>();
        }
    }

    public class QuotationContainerVm
    {
        public int QuoteId { get; set; }
        public int? ContainerTypeId { get; set; }
        public byte? NumberOfContainers { get; set; }

        public string ContainerTypeName { get; set; }
    }
}