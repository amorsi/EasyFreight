using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class ConsigneeVm
    {
        public int ConsigneeId { get; set; }
        public string ConsigneeCode { get; set; }
        public string ConsigneeNameEn { get; set; }
        public string ConsigneeNameAr { get; set; }
        public string ConsigneeAddressEn { get; set; }
        public string ConsigneeAddressAr { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string AccountId { get; set; }
        public string TaxDepositAccountId { get; set; }
        public List<ContactPersonVm> ContactPersons { get; set; }

        public ConsigneeVm()
        {
            ContactPersons = new List<ContactPersonVm>();
        }
    }
}