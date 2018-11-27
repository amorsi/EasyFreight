using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class ContractorRateVm
    {
        public ContractorRateVm()
        {
            IsValid = true;
            ValidToChecked = true;
            Contractor = new ContractorVm();
        }

        public int ContractorRateId { get; set; }
        public int ContractorId { get; set; }
        public string ContractorName { get; set; }

        public int? FromAreaId { get; set; }
        public int? ToAreaId { get; set; }
        public string FromAreaName { get; set; }
        public string ToAreaName { get; set; }

        public int FromCityId { get; set; }
        public int ToCityId { get; set; }
        public string FromCityName { get; set; }
        public string ToCityName { get; set; }

        public int ContainerTypeId { get; set; }
        public string ContainerTypeName { get; set; }

        public decimal? CostAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }

        public bool IsValid { get; set; }
        public bool ValidToChecked { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ValidToDate { get; set; }

        public ContractorVm Contractor { get; set; }


    }

}