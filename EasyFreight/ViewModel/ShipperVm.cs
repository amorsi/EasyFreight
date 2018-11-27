using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class ShipperVm
    {
        public int ShipperId { get; set; }
        public string ShipperCode { get; set; }
        public string ShipperNameEn { get; set; }
        public string ShipperNameAr { get; set; }
        public string ShipperAddressEn { get; set; }
        public string ShipperAddressAr { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string AccountId { get; set; }
        public string TaxDepositAccountId { get; set; }
        public List<ContactPersonVm> ContactPersons { get; set; }

        public ShipperVm()
        {
            ContactPersons = new List<ContactPersonVm>();
        }
    }
}