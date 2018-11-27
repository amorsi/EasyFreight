using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CompanyInfoVm
    {
        public int CompanyId { get; set; }
        public string CompanyNameEn { get; set; }
        public string CompanyNameAr { get; set; }
        public string CompanyAddressEn { get; set; }
        public string CompanyAddressAr { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string EnglishLogoUrl { get; set; }
        public string ArabicLogoUrl { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string PhoneNumber3 { get; set; }
        public string FaxNumber { get; set; }

    }

    public class CompanySetupVm : CompanyInfoVm
    {

        public int CountryId { get; set; }
        public int CityId { get; set; }

        public int TabIndex { get; set; }



        public string CarrierPrefix { get; set; }
        public string ShipperPrefix { get; set; }
        public string ConsigneePrefix { get; set; }
        public string NotifierPrefix { get; set; }
        public string ContractorPrefix { get; set; }
        public int DefaultCurrencyId { get; set; }




    }

    
}