using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;

namespace EasyFreight.DAL
{
    public static class ListCommonHelper
    {
        /// <summary>
        /// Get Country List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <returns>Dictionary CountryId, CountryName</returns>
        public static Dictionary<int, string> GetCountryList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> countryList = new Dictionary<int, string>();
            var countryListDb = db.Countries.ToList();

            if (langCode == "en")
            {
                countryList = countryListDb
                    .OrderBy(x => x.CountryNameEn)
                    .ToDictionary(x => x.CountryId, x => x.CountryNameEn);
            }
            else
            {
                countryList = countryListDb
                       .OrderBy(x => x.CountryNameAr)
                       .ToDictionary(x => x.CountryId, x => x.CountryNameAr);
            }

            return countryList;
        }

        /// <summary>
        /// Get Country List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <returns>Dictionary CountryId, CountryName</returns>
        public static Dictionary<int, string> GetCityList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> cityList = new Dictionary<int, string>();
            var cityListDb = db.Cities.ToList();

            if (langCode == "en")
            {
                cityList = cityListDb
                    .OrderBy(x => x.CityNameEn)
                    .ToDictionary(x => x.CityId, x => x.CityNameEn);
            }
            else
            {
                cityList = cityListDb
                       .OrderBy(x => x.CityNameAr)
                       .ToDictionary(x => x.CityId, x => x.CityNameAr);
            }

            return cityList;
        }

        /// <summary>
        /// Get city list for specific country
        /// </summary>
        /// <param name="countryId">Country Id to get its cities</param>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetCityList(int countryId, string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> cityList = new Dictionary<int, string>();
            var cityListDb = db.Cities.Where(x => x.CountryId == countryId).ToList();

            if (langCode == "en")
            {
                cityList = cityListDb
                    .OrderBy(x => x.CityNameEn)
                    .ToDictionary(x => x.CityId, x => x.CityNameEn);
            }
            else
            {
                cityList = cityListDb
                       .OrderBy(x => x.CityNameAr)
                       .ToDictionary(x => x.CityId, x => x.CityNameAr);
            }

            return cityList;
        }

        /// <summary>
        /// Get ports grouped by countries
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data</param>
        /// <param name="portType">0 Get all,1 seaport, 2 airport</param>
        /// <returns>CountryPortList View model</returns>
        public static List<CountryPortList> GetPortsGrouped(string langCode = "en", byte portType = 0)
        {
            List<CountryPortList> cunPortList = new List<CountryPortList>();
            CountryPortList cunPort;
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> portList;

            var portListDb = db.Ports.ToList();

            List<int> countInPortTn = portListDb.Select(x => x.CountryId).Distinct().ToList();

            Dictionary<int, string> counList = GetCountryList(langCode)
                .Where(x => countInPortTn.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            if (portType != 0)
                portListDb = portListDb.Where(x => x.PortType == portType).ToList();

            foreach (var item in counList)
            {
                int countryId = item.Key;
                cunPort = new CountryPortList();
                cunPort.CountryId = countryId;
                cunPort.CountryName = item.Value;

                portList = new Dictionary<int, string>();
                if (langCode == "en")
                    cunPort.PortList = portListDb.Where(x => x.CountryId == countryId)
                        .ToDictionary(x => x.PortId, x => x.PortNameEn);
                else
                    cunPort.PortList = portListDb.Where(x => x.CountryId == countryId)
                       .ToDictionary(x => x.PortId, x => x.PortNameAr);

                cunPortList.Add(cunPort);
            }

            return cunPortList;

        }

        public static JArray GetPortsGroupedJson(string langCode = "en", byte portType = 0)
        {
            var portsGrouped = GetPortsGrouped(langCode, portType);
            JTokenWriter pJTokenWriter = new JTokenWriter();
            JArray ordersJson = new JArray();

            pJTokenWriter.WriteStartArray();
            foreach (var item in portsGrouped)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("text");
                pJTokenWriter.WriteValue(item.CountryName);
                pJTokenWriter.WritePropertyName("children");
                pJTokenWriter.WriteStartArray();
                foreach (var item1 in item.PortList)
                {
                    pJTokenWriter.WriteStartObject();
                    pJTokenWriter.WritePropertyName("id");
                    pJTokenWriter.WriteValue(item1.Key.ToString());
                    pJTokenWriter.WritePropertyName("text");
                    pJTokenWriter.WriteValue(item1.Value);
                    pJTokenWriter.WriteEndObject();
                }
                pJTokenWriter.WriteEndArray();

                pJTokenWriter.WriteEndObject();
            }
            pJTokenWriter.WriteEndArray();


            ordersJson = (JArray)pJTokenWriter.Token;
            return ordersJson;
        }

        /// <summary>
        /// Get cities grouped by countries
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data</param>
        /// <returns>CountryCityList View model</returns>
        public static List<CountryCityList> GetCityGrouped(string langCode = "en")
        {
            List<CountryCityList> cunPortList = new List<CountryCityList>();
            CountryCityList cunPort;
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> portList;
            var cityDb = db.Cities.ToList();

            List<int> countInPortTn = cityDb.Select(x => x.CountryId).Distinct().ToList();

            Dictionary<int, string> counList = GetCountryList(langCode)
                .Where(x => countInPortTn.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in counList)
            {
                int countryId = item.Key;
                cunPort = new CountryCityList();
                cunPort.CountryId = countryId;
                cunPort.CountryName = item.Value;


                portList = new Dictionary<int, string>();
                if (langCode == "en")
                    cunPort.CityList = cityDb.Where(x => x.CountryId == countryId)
                        .ToDictionary(x => x.CityId, x => x.CityNameEn);
                else
                    cunPort.CityList = cityDb.Where(x => x.CountryId == countryId)
                        .ToDictionary(x => x.CityId, x => x.CityNameAr);

                cunPortList.Add(cunPort);
            }

            return cunPortList;
        }

        /// <summary>
        /// Get cities grouped by countries
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data</param>
        /// <returns>CountryCityList View model</returns>
        public static List<CityAreaList> GetAreaGrouped(string langCode = "en")
        {
            List<CityAreaList> cunPortList = new List<CityAreaList>();
            CityAreaList cunPort;
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> areaList;
            var areaDb = db.Areas.ToList();

            List<int> countInAreaTn = areaDb.Select(x => x.CityId).Distinct().ToList();

            Dictionary<int, string> cityList = GetCityList(langCode)
                .Where(x => countInAreaTn.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in cityList)
            {
                int cityId = item.Key;
                cunPort = new CityAreaList();
                cunPort.CityId = cityId;
                cunPort.CityName = item.Value;


                areaList = new Dictionary<int, string>();
                if (langCode == "en")
                    cunPort.AreaList = areaDb.Where(x => x.CityId == cityId)
                        .ToDictionary(x => x.AreaId, x => x.AreaNameEn);
                else
                    cunPort.AreaList = areaDb.Where(x => x.CityId == cityId)
                        .ToDictionary(x => x.AreaId, x => x.AreaNameAr);

                cunPortList.Add(cunPort);
            }

            return cunPortList;
        }

        /// <summary>
        /// Get Consignee List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <returns>Dictionary Consignee Id, Consignee Name</returns>
        public static Dictionary<int, string> GetConsigneeList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> consigneeList = new Dictionary<int, string>();
            var consigneeListDb = db.Consignees
                .Select(x => new { x.ConsigneeId, x.ConsigneeNameEn, x.ConsigneeNameAr }).ToList();

            if (langCode == "en")
            {
                consigneeList = consigneeListDb
                    .OrderBy(x => x.ConsigneeNameEn)
                    .ToDictionary(x => x.ConsigneeId, x => x.ConsigneeNameEn);
            }
            else
            {
                consigneeList = consigneeListDb
                       .OrderBy(x => x.ConsigneeNameAr)
                       .ToDictionary(x => x.ConsigneeId, x => x.ConsigneeNameAr);
            }

            return consigneeList;
        }

        /// <summary>
        /// Get Carrier List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <param name="Carrier Type">0 Get all,1 seaport, 2 airport</param>
        /// <returns>Dictionary Carrier Id, Carrier Name</returns>
        public static Dictionary<int, string> GetCarrierList(string langCode = "en", byte carrType = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> CarrierList = new Dictionary<int, string>();
            List<CarrierDic> carrierListDb = new List<CarrierDic>();

            if (carrType == 0)
            {
                carrierListDb = db.Carriers
                 .Select(x => new CarrierDic
                 {
                     CarrierId = x.CarrierId,
                     CarrierNameEn = x.CarrierNameEn,
                     CarrierNameAr = x.CarrierNameAr,
                     CarrierType = x.CarrierType
                 })
                 .ToList();
            }

            else
            {
                carrierListDb = db.Carriers
                  .Select(x => new CarrierDic
                  {
                      CarrierId = x.CarrierId,
                      CarrierNameEn = x.CarrierNameEn,
                      CarrierNameAr = x.CarrierNameAr,
                      CarrierType = x.CarrierType
                  })
                 .Where(x => x.CarrierType == carrType).ToList();
            }



            if (langCode == "en")
            {
                CarrierList = carrierListDb
                    .OrderBy(x => x.CarrierNameEn)
                    .ToDictionary(x => x.CarrierId, x => x.CarrierNameEn);
            }
            else
            {
                CarrierList = carrierListDb
                       .OrderBy(x => x.CarrierNameAr)
                       .ToDictionary(x => x.CarrierId, x => x.CarrierNameAr);
            }

            return CarrierList;
        }



        /// <summary>
        /// Get Currency List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data --Not used </param>
        /// <returns>Dictionary Currency Id, Currency Sign</returns>
        public static Dictionary<int, string> GetCurrencyList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> CurrencyList = new Dictionary<int, string>();
            var currencyListDb = db.Currencies.ToList();

            CurrencyList = currencyListDb
                .OrderBy(x => x.CurrencyId)
                .ToDictionary(x => x.CurrencyId, x => x.CurrencySign);

            return CurrencyList;
        }

        /// <summary>
        /// Get Container List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data --Not used </param>
        /// <returns>Dictionary Container Id, Container Name</returns>
        public static Dictionary<int, string> GetContainerList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> ContainerList = new Dictionary<int, string>();
            var containerListDb = db.ContainerTypes.Where(x => x.ContainerTypeId != 0).ToList();

            ContainerList = containerListDb
                .OrderBy(x => x.ContainerTypeName)
                .ToDictionary(x => x.ContainerTypeId, x => x.ContainerTypeName);

            return ContainerList;
        }

        /// <summary>
        /// Get Contractor List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <returns>Dictionary Contractor Id, Contractor Name</returns>
        public static Dictionary<int, string> GetContractorList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> ContractorList = new Dictionary<int, string>();
            var contractorListDb = db.Contractors.ToList();

            if (langCode == "en")
            {
                ContractorList = contractorListDb
                    .OrderBy(x => x.ContractorNameEn)
                    .ToDictionary(x => x.ContractorId, x => x.ContractorNameEn);
            }
            else
            {
                ContractorList = contractorListDb
                       .OrderBy(x => x.ContractorNameAr)
                       .ToDictionary(x => x.ContractorId, x => x.ContractorNameAr);
            }

            return ContractorList;
        }

        /// <summary>
        /// Get Trucking CostName List
        /// </summary> 
        /// <returns>Dictionary Trucking Cost Id, Trucking Cost Name</returns>
        public static Dictionary<int, string> GetTruckingCostNameList()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> ContainerList = new Dictionary<int, string>();
            var truckingCostNameListDb = db.TruckingCostLibs.ToList();

            ContainerList = truckingCostNameListDb
                .OrderBy(x => x.TruckingCostName)
                .ToDictionary(x => x.TruckingCostId, x => x.TruckingCostName);
            return ContainerList;
        }

        /// <summary>
        /// Get Contractor List By Area
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <param name="fromAreaId">From AreaId </param>
        /// <param name="toAreaId">To AreaId</param>
        /// <returns>Dictionary Contractor Id, Contractor Name</returns>
        public static Dictionary<int, string> GetContractorListByAreaByDate(string langCode = "en", int fromAreaID = 0, int toAreaId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> ContractorList = new Dictionary<int, string>();

            var contractorListDb = db.ContractorRates.Where(x => x.FromAreaId == fromAreaID && x.ToAreaId == toAreaId && x.ValidToDate >= DateTime.Now).Select
              (x => new { x.ContractorId, x.Contractor.ContractorNameEn, x.Contractor.ContractorNameAr }).Distinct().ToList();

            //var contractorListDb = db.Contractors.Include("ContractorRates").Where(x=> x.ContractorRates.FirstOrDefault().FromAreaId == fromAreaID
            //    && x.ContractorRates.FirstOrDefault().ToAreaId == toAreaId)
            //    .ToList();

            if (langCode == "en")
            {
                ContractorList = contractorListDb
                    .OrderBy(x => x.ContractorNameEn)
                    .ToDictionary(x => x.ContractorId, x => x.ContractorNameEn);
            }
            else
            {
                ContractorList = contractorListDb
                       .OrderBy(x => x.ContractorNameAr)
                       .ToDictionary(x => x.ContractorId, x => x.ContractorNameAr);
            }

            return ContractorList;
        }

        /// <summary>
        /// Get Contractor List By City
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <param name="fromCityID">From CityId </param>
        /// <param name="toCityId">To CityId</param>
        /// <returns>Dictionary Contractor Id, Contractor Name</returns>
        public static Dictionary<int, string> GetContractorListByCity(string langCode = "en", int fromCityID = 0, int toCityId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> ContractorList = new Dictionary<int, string>();

            var contractorListDb = db.ContractorRates.Where(x => (x.FromCityId == fromCityID && x.ToCityId == toCityId) ||
                x.ToCityId == fromCityID && x.FromCityId == toCityId).Select
              (x => new { x.ContractorId, x.Contractor.ContractorNameEn, x.Contractor.ContractorNameAr }).Distinct().ToList();


            if (langCode == "en")
            {
                ContractorList = contractorListDb
                    .OrderBy(x => x.ContractorNameEn)
                    .ToDictionary(x => x.ContractorId, x => x.ContractorNameEn);
            }
            else
            {
                ContractorList = contractorListDb
                       .OrderBy(x => x.ContractorNameAr)
                       .ToDictionary(x => x.ContractorId, x => x.ContractorNameAr);
            }

            return ContractorList;
        }

        /// <summary>
        /// Get Contractor List By Area
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <param name="fromAreaId">From AreaId </param>
        /// <param name="toAreaId">To AreaId</param>
        /// <returns>Dictionary Contractor Id, Contractor Name</returns>
        public static Dictionary<int, string> GetContractorListByArea(string langCode = "en", int fromAreaID = 0, int toAreaId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> ContractorList = new Dictionary<int, string>();

            var contractorListDb = db.ContractorRates.Where(x => (x.FromAreaId == fromAreaID && x.ToAreaId == toAreaId) ||
                x.ToAreaId == fromAreaID && x.FromAreaId == toAreaId).Select
              (x => new { x.ContractorId, x.Contractor.ContractorNameEn, x.Contractor.ContractorNameAr }).Distinct().ToList();

            //var contractorListDb = db.Contractors.Include("ContractorRates").Where(x=> x.ContractorRates.FirstOrDefault().FromAreaId == fromAreaID
            //    && x.ContractorRates.FirstOrDefault().ToAreaId == toAreaId)
            //    .ToList();

            if (langCode == "en")
            {
                ContractorList = contractorListDb
                    .OrderBy(x => x.ContractorNameEn)
                    .ToDictionary(x => x.ContractorId, x => x.ContractorNameEn);
            }
            else
            {
                ContractorList = contractorListDb
                       .OrderBy(x => x.ContractorNameAr)
                       .ToDictionary(x => x.ContractorId, x => x.ContractorNameAr);
            }

            return ContractorList;
        }

        /// <summary>
        /// Get Shipper List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <returns>Dictionary Shipper Id, Shipper Name</returns>
        public static Dictionary<int, string> GetShipperList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> shipperList = new Dictionary<int, string>();
            var shipperListDb = db.Shippers
                .Select(x => new { x.ShipperId, x.ShipperNameEn, x.ShipperNameAr })
                .ToList();

            if (langCode == "en")
            {
                shipperList = shipperListDb
                    .OrderBy(x => x.ShipperNameEn)
                    .ToDictionary(x => x.ShipperId, x => x.ShipperNameEn);
            }
            else
            {
                shipperList = shipperListDb
                       .OrderBy(x => x.ShipperNameAr)
                       .ToDictionary(x => x.ShipperId, x => x.ShipperNameAr);
            }

            return shipperList;
        }

        /// <summary>
        /// Get Shipper List
        /// </summary>
        /// <param name="langCode">en is default.. send "ar" to get Arabic data </param>
        /// <param name="consigneeId">consignee Id</param>
        /// <returns>Dictionary Shipper Id, Shipper Name</returns>
        public static Dictionary<int, string> GetNotifierList(int consigneeId, string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> notifierList = new Dictionary<int, string>();
            var notifierListDb = db.Notifiers.Where(x => x.ConsigneeId == consigneeId).ToList();

            if (langCode == "en")
            {
                notifierList = notifierListDb
                    .OrderBy(x => x.NotifierAddressEn)
                    .ToDictionary(x => x.NotifierId, x => x.NotifierNameEn);
            }
            else
            {
                notifierList = notifierListDb
                       .OrderBy(x => x.NotifierNameAr)
                       .ToDictionary(x => x.NotifierId, x => x.NotifierNameAr);
            }

            return notifierList;
        }

        /// <summary>
        /// Get IncotermLib List
        /// </summary>
        /// <returns>Dictionary Incoterm Id, Incoterm Name</returns>
        public static Dictionary<int, string> GetIncotermLibList()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> IncotermLibList = new Dictionary<int, string>();
            var consigneeListDb = db.IncotermLibs.ToList();


            IncotermLibList = consigneeListDb
                .OrderBy(x => x.IncotermName)
                .ToDictionary(x => x.IncotermId, x => x.IncotermName + " (" + x.IncotermCode + ")");


            return IncotermLibList;
        }

        /// <summary>
        /// Get Vessel List by Carrier Id
        /// </summary>
        /// <param name="carrierId">carrier Id</param>
        /// <returns>Dictionary Shipper Id, Shipper Name</returns>
        public static Dictionary<int, string> GetVesselLis(int carrierId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> VesseDic = new Dictionary<int, string>();
            List<Vessel> vesselList = new List<Vessel>();
            if (carrierId != 0)
                vesselList = db.Vessels.Where(x => x.CarrierId == carrierId).ToList();
            else
                vesselList = db.Vessels.ToList();



            VesseDic = vesselList
                .OrderBy(x => x.VesselName)
                .ToDictionary(x => x.VesselId, x => x.VesselName);


            return VesseDic;
        }

        /// <summary>
        /// Get Package Type List
        /// </summary>
        /// <returns>Dictionary Incoterm Id, Incoterm Name</returns>
        public static Dictionary<int, string> GetPackageTypeList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> packageTypeList = new Dictionary<int, string>();
            var packageTypeListDb = db.PackageTypes.ToList();

            if (langCode == "en")
            {
                packageTypeList = packageTypeListDb
                    .OrderBy(x => x.PackageTypeNameEn)
                    .ToDictionary(x => x.PackageTypeId, x => x.PackageTypeNameEn);
            }
            else
            {
                packageTypeList = packageTypeListDb
                       .OrderBy(x => x.PackageTypeNameAr)
                       .ToDictionary(x => x.PackageTypeId, x => x.PackageTypeNameAr);
            }


            return packageTypeList;
        }

        /// <summary>
        /// Get Package Type List
        /// </summary>
        /// <returns>Dictionary Incoterm Id, Incoterm Name</returns>
        public static Dictionary<int, string> GetAgentList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> agentList = new Dictionary<int, string>();
            var agentListDb = db.Agents
                .Select(x => new { x.AgentId, x.AgentNameEn, x.AgentNameAr })
                .ToList();

            if (langCode == "en")
            {
                agentList = agentListDb
                    .OrderBy(x => x.AgentNameEn)
                    .ToDictionary(x => x.AgentId, x => x.AgentNameEn);
            }
            else
            {
                agentList = agentListDb
                       .OrderBy(x => x.AgentNameAr)
                       .ToDictionary(x => x.AgentId, x => x.AgentNameAr);
            }


            return agentList;
        }

        /// <summary>
        /// Get Custom Clearance cost List
        /// </summary>
        /// <returns>Dictionary Custom Clearance Id, Custom Clearance Name</returns>
        public static Dictionary<int, string> GetCustClearCostList(string langCode = "en")
        {
            OperationsEntities db = new OperationsEntities();
            Dictionary<int, string> custClearCostList = new Dictionary<int, string>();
            var custClearCostListDb = db.CustomClearanceCostLibs.ToList();

            if (langCode == "en")
            {
                custClearCostList = custClearCostListDb
                    .OrderBy(x => x.CostNameEn)
                    .ToDictionary(x => x.CCCostId, x => x.CostNameEn);
            }
            else
            {
                custClearCostList = custClearCostListDb
                       .OrderBy(x => x.CostNameAr)
                       .ToDictionary(x => x.CCCostId, x => x.CostNameAr);
            }


            return custClearCostList;
        }


        /// <summary>
        /// Get Department List
        /// </summary>
        /// <param name="langCode">en is default.. ar for arabic </param>
        /// <returns>Dictionary Department Id, Department Name</returns>
        public static Dictionary<int, string> GetDepartmentList(string langCode = "en")
        {
            EasyFreightEntities db = new EasyFreightEntities();
            Dictionary<int, string> DepartmentList = new Dictionary<int, string>();
            var departmentListDb = db.DepartmentEasyModels.ToList();

            if (langCode == "en")
            {
                DepartmentList = departmentListDb
                    .OrderBy(x => x.DepNameEn)
                    .ToDictionary(x => x.DepId, x => x.DepNameEn);
            }
            else
            {
                DepartmentList = departmentListDb
                       .OrderBy(x => x.DepNameAr)
                       .ToDictionary(x => x.DepId, x => x.DepNameAr);
            }

            return DepartmentList;
        }

        /// <summary>
        /// Get Operation cost List
        /// </summary>
        /// <returns>Dictionary OperCostLibId, Operation Cost Name</returns>
        public static Dictionary<int, string> GetOperationCostList(string langCode = "en")
        {
            OperationsEntities db = new OperationsEntities();
            Dictionary<int, string> operationCostList = new Dictionary<int, string>();
            var operationCostListDb = db.OperationCostLibs.ToList();

            if (langCode == "en")
            {
                operationCostList = operationCostListDb
                    .OrderBy(x => x.OperCostNameEn)
                    .ToDictionary(x => x.OperCostLibId, x => x.OperCostNameEn);
            }
            else
            {
                operationCostList = operationCostListDb
                       .OrderBy(x => x.OperCostNameAr)
                       .ToDictionary(x => x.OperCostLibId, x => x.OperCostNameAr);
            }


            return operationCostList;
        }


        private class CarrierDic
        {
            public int CarrierId { get; set; }
            public string CarrierNameEn { get; set; }
            public string CarrierNameAr { get; set; }
            public byte? CarrierType { get; set; }
        }


        internal static Dictionary<byte, string> GetPaymentTerm(string langCode = "en", bool includeDepositOption = false)
        {
            AccountingEntities db = new AccountingEntities();

            Dictionary<byte, string> paymentTermList = new Dictionary<byte, string>();
            var operationCostListDb = db.PaymentTerms.ToList();

            if (!includeDepositOption)
                operationCostListDb = operationCostListDb.Where(x => x.PaymentTermId != 5).ToList();

            if (langCode == "en")
            {
                paymentTermList = operationCostListDb
                    .OrderBy(x => x.PaymentTermId)
                    .ToDictionary(x => x.PaymentTermId, x => x.PaymentTermEn);
            }
            else
            {
                paymentTermList = operationCostListDb
                       .OrderBy(x => x.PaymentTermId)
                       .ToDictionary(x => x.PaymentTermId, x => x.PaymentTermAr);
            }


            return paymentTermList;
        }

        internal static Dictionary<int, string> GetBankList(string langCode = "en")
        {
            AccountingEntities db = new AccountingEntities();

            Dictionary<int, string> banksList = new Dictionary<int, string>();
            var operationCostListDb = db.Banks.ToList();

            if (langCode == "en")
            {
                banksList = operationCostListDb
                    .OrderBy(x => x.BankId)
                    .ToDictionary(x => x.BankId, x => x.BankNameEn);
            }
            else
            {
                banksList = operationCostListDb
                       .OrderBy(x => x.BankId)
                       .ToDictionary(x => x.BankId, x => x.BankNameAr);
            }


            return banksList;
        }

        internal static Dictionary<int, string> GetBankAccList()
        {
            AccountingEntities db = new AccountingEntities();

            Dictionary<int, string> banksList = new Dictionary<int, string>();
            var bankAccListDb = db.BankAccounts.ToList();

            banksList = bankAccListDb
                   .OrderBy(x => x.BankId)
                   .ToDictionary(x => x.BankAccId, x => x.AccountName + " (" + x.AccountNumber + ")");

            return banksList;

        }

        public static Dictionary<int, string> GetExpensesLibraryList(string langCode = "en")
        {
            AccountingEntities db = new AccountingEntities();

            Dictionary<int, string> banksList = new Dictionary<int, string>();
            var bankAccListDb = db.ExpenseLibs.ToList();

            if (langCode == "en")
            {
                banksList = bankAccListDb
                    .OrderBy(x => x.ExpenseNameEn)
                    .ToDictionary(x => x.ExpenseId, x => x.ExpenseNameEn);
            }
            else
            {
                banksList = bankAccListDb
                       .OrderBy(x => x.ExpenseNameAr)
                       .ToDictionary(x => x.ExpenseId, x => x.ExpenseNameAr);
            }

            return banksList;

        }

        public static Dictionary<string, string> GetPartnersList(string langCode = "en")
        {
            AccountingEntities db = new AccountingEntities();

            Dictionary<string, string> banksList = new Dictionary<string, string>();
            string parentAccId = ((int)AccountingChartEnum.PartnersDrawingAccounts).ToString();
            var bankAccListDb = db.AccountingCharts.Where(x => x.ParentAccountId == parentAccId).ToList();

            if (langCode == "en")
            {
                banksList = bankAccListDb
                    .OrderBy(x => x.AccountNameEn)
                    .ToDictionary(x => x.AccountId, x => x.AccountNameEn);
            }
            else
            {
                banksList = bankAccListDb
                       .OrderBy(x => x.AccountNameAr)
                       .ToDictionary(x => x.AccountId, x => x.AccountNameAr);
            }

            return banksList;
        }

        public static Dictionary<string, string> GetCCDepositList(string langCode = "en")
        {
            AccountingEntities db = new AccountingEntities();

            Dictionary<string, string> banksList = new Dictionary<string, string>();
            string parentAccId = "177115";// ((int)AccountingChartEnum.CashDepositTemp).ToString();
            var bankAccListDb = db.AccountingCharts.Where(x => x.AccountId == parentAccId).ToList();

            if (langCode == "en")
            {
                banksList = bankAccListDb
                    .OrderBy(x => x.AccountNameEn)
                    .ToDictionary(x => x.AccountId, x => x.AccountNameEn);
            }
            else
            {
                banksList = bankAccListDb
                       .OrderBy(x => x.AccountNameAr)
                       .ToDictionary(x => x.AccountId, x => x.AccountNameAr);
            }

            return banksList;
        }
    }


}