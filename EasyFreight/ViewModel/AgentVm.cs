using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class AgentVm
    {
        public int AgentId { get; set; }
        public string AgentCode { get; set; }
        public string AgentNameEn { get; set; }
        public string AgentNameAr { get; set; }
        public string AgentAddressEn { get; set; }
        public string AgentAddressAr { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string AccountId { get; set; }
        public List<ContactPersonVm> ContactPersons { get; set; }

        public AgentVm()
        {
            ContactPersons = new List<ContactPersonVm>();
        }

    }
}