using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class OperationVm : OperationBaseVm
    {

        public int OperationId { get; set; }
        public int? QuoteId { get; set; }
        public DateTime? QuoteDate { get; set; }
        public string OperationCode { get; set; }
        public string BookingNumber { get; set; }
        public string MBL { get; set; }
        public DateTime? OperationDate { get; set; }
        public bool IsConsolidation { get; set; }
        public int? AgentId { get; set; }
        public bool IsFixedCost { get; set; }
        public decimal? FixedCost { get; set; }
        public int FixedCostCurrencyId { get; set; }
        public decimal? PercentageOfAmount { get; set; }


        public List<OperationContainerVm> OperationContainers { get; set; }

        public OperationVm()
        {

        }

        public OperationVm(byte orderFrom, int quoteId)
        {
            OrderFrom = orderFrom;
            CreateDate = DateTime.Now;
            OperationDate = DateTime.Now;
            StatusId = 1;
            IsConsolidation = false;
            IsFixedCost = true;
            CarrierType = 1;
            if (quoteId != 0)
                QuoteId = quoteId;
            if (orderFrom == 2)
            {
                NotifierAsConsignee = true;
                OperationCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.Import, false);
            }
            else
                OperationCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.Export, false);



            OperationContainers = new List<OperationContainerVm>();
        }

    }

    public class OperationContainerVm
    {
        public long OperConId { get; set; }
        public int OperationId { get; set; }
        public int ContainerTypeId { get; set; }
        public string ContainerTypeName { get; set; }
        public string ContainerNumber { get; set; }
        public string SealNumber { get; set; }
        public int? PackageTypeId { get; set; }
        public string PackageTypeName { get; set; }
        public int? NumberOfPackages { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? CBM { get; set; }

    }

    public class ConcessionLetterVm
    {
        public string CarrierName { get; set; }
        public string FromPort { get; set; }
        public string ToPort { get; set; }
        public string MBL { get; set; }
        public string Containers { get; set; }
        public string HouseBL { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal CBM { get; set; }
        public string ConsigneeName { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfPackages { get; set; }
        public string VesselName { get; set; }
        public string ContainerNumber { get; set; }
        public DateTime? ArriveDate { get; set; }
        public string GoodsDescription { get; set; }
        public Dictionary<string, string> StaticLabels { get; set; }


        public ConcessionLetterVm()
        {
            StaticLabels = new Dictionary<string, string>();
        }

    }

    public class OperationCostMainVm
    {
        public int OperationId { get; set; }
        public List<OperationCostVm> OperationCosts { get; set; }
        public List<OperationCostTotalsVm> Totals { get; set; }

        public OperationCostMainVm()
        {
            OperationCosts = new List<OperationCostVm>();
            Totals = new List<OperationCostTotalsVm>();
        }

    }

    public class OperationCostVm
    {
        public int OperCostId { get; set; }
        public int HouseBillId { get; set; }
        public int OperCostLibId { get; set; }
        public int OperationId { get; set; }
        public decimal? OperationCostNet { get; set; }
        public decimal? OperationCostSelling { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
        public string OperCostNameEn { get; set; }
        public string OperCostNameAr { get; set; }
        public bool IsAgentCost { get; set; }
        public int CurrencyIdSelling { get; set; }
        public string CurrencySignSelling { get; set; }
        public bool IsAccounting { get; set; }
    }

    public class OperationCostTotalsVm
    {
        public string CurrencySign { get; set; }
        public decimal? TotalCostNet { get; set; }
        public decimal? TotalCostSelling { get; set; }
    }

    public class OperationStatisticVm
    {
        public int NewAir { get; set; }
        public decimal NewAirNet { get; set; }
        public decimal NewAirCBM { get; set; }

        public int NewSea { get; set; }
        public decimal NewSeaNet { get; set; }
        public decimal NewSeaCBM { get; set; }

        public int OpenAir { get; set; }
        public decimal OpenAirNet { get; set; }
        public decimal OpenAirCBM { get; set; }


        public int OpenSea { get; set; }
        public decimal OpenSeaNet { get; set; }
        public decimal OpenSeaCBM { get; set; }


        public int ClosedAir { get; set; }
        public decimal ClosedAirNet { get; set; }
        public decimal ClosedAirCBM { get; set; }

        public int ClosedSea { get; set; }
        public decimal ClosedSeaNet { get; set; }
        public decimal ClosedSeaCBM { get; set; }

        public int InvoiceAir { get; set; }
        public decimal InvoiceAirNet { get; set; }
        public decimal InvoiceAirCBM { get; set; }

        public int InvoiceSea { get; set; }
        public decimal InvoiceSeaNet { get; set; }
        public decimal InvoiceSeaCBM { get; set; }

        public int QuotNewAir { get; set; }
        public int QuotNewSea { get; set; } 
    }
}