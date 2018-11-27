using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class ContractorVm
    {
        public int ContractorId { get; set; }
        public string ContractorCode { get; set; }
        public string ContractorNameEn { get; set; }
        public string ContractorNameAr { get; set; }
        public string ContractorAddressEn { get; set; }
        public string ContractorAddressAr { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string AccountId { get; set; }
        public List<ContactPersonVm> ContactPersons { get; set; }

        public ContractorVm()
        {
            ContactPersons = new List<ContactPersonVm>();
        }
    }
}