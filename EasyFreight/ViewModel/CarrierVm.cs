using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CarrierVm
    {
        public int CarrierId { get; set; }
        public string CarrierCode { get; set; }
        public string CarrierNameEn { get; set; }
        public string CarrierNameAr { get; set; }
        public string CarrierAddressEn { get; set; }
        public string CarrierAddressAr { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int CarrierType { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string AccountId { get; set; }
        public List<ContactPersonVm> ContactPersons { get; set; }

        public CarrierVm()
        {
            ContactPersons = new List<ContactPersonVm>();
        }


    }
}