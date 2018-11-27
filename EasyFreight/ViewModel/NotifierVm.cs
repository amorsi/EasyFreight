using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class NotifierVm
    {
        public int NotifierId { get; set; }
        public string NotifierCode { get; set; }
        public string NotifierNameEn { get; set; }
        public string NotifierNameAr { get; set; }
        public string NotifierAddressEn { get; set; }
        public string NotifierAddressAr { get; set; }
        public int ConsigneeId { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public List<ContactPersonVm> ContactPersons { get; set; }

        public NotifierVm()
        {
            ContactPersons = new List<ContactPersonVm>();
        }
    }
}