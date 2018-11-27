using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CountryPortList
    {
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string CountryName { get; set; }
        public Dictionary<int, string> PortList { get; set; }

        public CountryPortList()
        {
            PortList = new Dictionary<int, string>();
        }
    }

    public class CountryCityList
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public Dictionary<int, string> CityList { get; set; }

        public CountryCityList()
        {
            CityList = new Dictionary<int, string>();
        }
    }


    public class CityAreaList
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public Dictionary<int, string> AreaList { get; set; }

        public CityAreaList()
        {
            AreaList = new Dictionary<int, string>();
        }
    }

    public class IncotermList
    {
        public int IncotermId { get; set; }
        public string IncotermCode { get; set; }
        public string IncotermName { get; set; }
    }

    

}