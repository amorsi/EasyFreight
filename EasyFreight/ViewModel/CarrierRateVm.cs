using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CarrierRateVm
    {
        public CarrierRateVm()
        {
            IsValid = true;
        }
        public int CarrierRateId { get; set; }
        public int CarrierId { get; set; }
        public string CarrierName { get; set; }

        public int FromPortId { get; set; }
        public int ToPortId { get; set; }
        public string FromPortName { get; set; }
        public string ToPortName { get; set; }

        public int FromCountryId { get; set; }
        public int ToCountryId { get; set; }
        public string FromCountryName { get; set; }
        public string ToCountryName { get; set; }

        public int ContainerTypeId { get; set; }
        public string ContainerTypeName { get; set; }


        public decimal? FreightCostAmount { get; set; }
        public int FreighCurrencyId { get; set; }
        public string FreighCurrencySign { get; set; }

        public decimal? ThcCostAmount { get; set; }
        public int ThcCurrencyId { get; set; }
        public string ThcCurrencySign { get; set; }

        public bool IsValid { get; set; } 
        public bool ValidToChecked { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ValidToDate { get; set; }

        public int [] TransitPortId { get; set; }

    }




}