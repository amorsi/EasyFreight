using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace EasyFreight.DAL
{
    public static class ContractorRateHelper
    {
        public static string AddEditContractorRate(ContractorRateVm contractRateVm)
        {

            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            ContractorRate contractRateDb; 
            
                bool isSameActive = CheckIfActiveRate(contractRateVm);
                if (isSameActive)
                    return "false. Another rate is active with same data"; 
                
                
                int contractRateVmId = contractRateVm.ContractorRateId;
                if (contractRateVmId == 0)
                    contractRateDb = new ContractorRate();
                else
                    contractRateDb = db.ContractorRates.Where(x => x.ContractorRateId == contractRateVmId).FirstOrDefault();
                Mapper.CreateMap<ContractorRateVm, ContractorRate>()
                   .ForMember(x => x.Contractor, y => y.Ignore());
                Mapper.Map(contractRateVm, contractRateDb);

                if (contractRateVmId == 0)
                    db.ContractorRates.Add(contractRateDb);
           
            try
            {
               db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.Message;
            }

            return isSaved;
        }

        public static List<ContractorRateVm> GetContractorRatesList(bool getValidOnly = false)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<ContractorRateVm> contractRateVmList = new List<ContractorRateVm>();
            var spResult = db.GetContractorRate(getValidOnly).ToList();

            Mapper.CreateMap<GetContractorRate_Result, ContractorRateVm>().IgnoreAllNonExisting();
                //.ForMember(c => c.FromAreaId, option => option.Ignore())
                //.ForMember(c => c.ToAreaId, option => option.Ignore()).IgnoreAllNonExisting();
            Mapper.Map(spResult, contractRateVmList );

            return contractRateVmList;

        }

        public static ContractorRateVm GetContractorRateInfo(int contractRateId)
        {
            ContractorRateVm contractRateVm = new ContractorRateVm();
            if (contractRateId == 0)
                return contractRateVm;

            EasyFreightEntities db = new EasyFreightEntities();
            ContractorRate contractRateDb = db.ContractorRates.Where(x => x.ContractorRateId == contractRateId).FirstOrDefault();
            Mapper.CreateMap<ContractorRate, ContractorRateVm>().IgnoreAllNonExisting();
            Mapper.CreateMap<Contractor, ContractorVm>().IgnoreAllNonExisting();
            Mapper.Map(contractRateDb, contractRateVm);
            return contractRateVm;
        }

        /// <summary>
        /// Will set isValid to false .. will not be deleted
        /// </summary>
        /// <param name="contractRateId"></param>
        /// <returns></returns>
        public static string Delete(int contractRateId)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            ContractorRate contractRateDb = db.ContractorRates.Where(x => x.ContractorRateId == contractRateId).FirstOrDefault();
            contractRateDb.IsValid = false;
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isDeleted = "false" + ex.Message;
            }

            return isDeleted;
        }

        //Inquiry
        public static List<ContractorRateVm> GetContractorRatesInquiry(System.Web.Mvc.FormCollection form)
        {
            EasyFreightEntities db = new EasyFreightEntities();

            List<ContractorRateVm> contractRateVmList = GetContractorRatesList(true);
            if (form == null || form.AllKeys.Count() == 0)
                return contractRateVmList;

            if (!string.IsNullOrEmpty(form["ContractorId"]))
            {
                int contractId = int.Parse(form["ContractorId"]);
                contractRateVmList = contractRateVmList.Where(x => x.ContractorId == contractId).ToList();
            }
            if (!string.IsNullOrEmpty(form["ContainerTypeId"]))
            {
                int contId = int.Parse(form["ContainerTypeId"]);
                contractRateVmList = contractRateVmList.Where(x => x.ContainerTypeId == contId).ToList();
            }
            if (!string.IsNullOrEmpty(form["FromAreaId"]))
            {
                int fromAreaId = int.Parse(form["FromAreaId"]);
                contractRateVmList = contractRateVmList.Where(x => x.FromAreaId == fromAreaId).ToList();
            }

            if (!string.IsNullOrEmpty(form["ToAreaId"]))
            {
                int toAreaId = int.Parse(form["ToAreaId"]);
                contractRateVmList = contractRateVmList.Where(x => x.ToAreaId == toAreaId).ToList();
            }

            if (!string.IsNullOrEmpty(form["FromDate"]))
            {
                DateTime validDate = DateTime.Parse(form["FromDate"]);
                contractRateVmList = contractRateVmList.Where(x => x.ValidToDate >= validDate).ToList();
            }

            if (!string.IsNullOrEmpty(form["ToDate"]))
            {
                DateTime validDate = DateTime.Parse(form["ToDate"]);
                contractRateVmList = contractRateVmList.Where(x => x.ValidToDate <= validDate).ToList();
            }

            return contractRateVmList;
        }

        private static bool CheckIfActiveRate(ContractorRateVm contractRateVm)
        {
            bool isActive = false;
            int contractId, containerTypeId, contractRateId;
            int? fromAreaId, toAreaId;
            contractId = contractRateVm.ContractorId;
            fromAreaId = contractRateVm.FromAreaId;
            toAreaId = contractRateVm.ToAreaId;
            containerTypeId = contractRateVm.ContainerTypeId;
            contractRateId = contractRateVm.ContractorRateId;

            EasyFreightEntities db = new EasyFreightEntities();
            if (contractRateId == 0)
                isActive = db.ContractorRates.Any(x => x.ContractorId == contractId
                    && x.ContainerTypeId == containerTypeId && x.FromAreaId == fromAreaId
                    && x.ToAreaId == toAreaId
                    && x.IsValid == true);
            else
                isActive = db.ContractorRates.Any(x => x.ContractorId == contractId
                       && x.ContainerTypeId == containerTypeId && x.FromAreaId == fromAreaId
                       && x.ToAreaId == toAreaId && x.ContractorRateId != contractRateId
                       && x.IsValid == true);

            return isActive;
        }

        /// <summary>
        /// Get valid contractors rates between two areas
        /// </summary>
        /// <param name="fromAreaId">From Area Id</param>
        /// <param name="toAreaId">To Area Id</param>
        /// <returns>ContractorRateVm List</returns>
        public static List<ContractorRateVm> GetContractorRatesForArea(int fromCityId, int toCityId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<ContractorRateVm> contractRateVmList = new List<ContractorRateVm>();
            var dbList = db.ContractorRates.Include("ContainerType").Include("Contractor").Include("Currency")
                .Where(x => (x.FromCityId == fromCityId && x.ToCityId == toCityId) ||
            (x.ToCityId == fromCityId && x.FromCityId == toCityId) && x.IsValid == true)
                .OrderBy(x => x.ContainerTypeId).ThenByDescending(x => x.CostAmount)
                .ToList();

            Mapper.CreateMap<ContractorRate, ContractorRateVm>().IgnoreAllNonExisting()
                .ForMember(x => x.ContainerTypeName, c => c.MapFrom(d => d.ContainerType.ContainerTypeName))
                .ForMember(x => x.ContractorName, c => c.MapFrom(d => d.Contractor.ContractorNameEn))
                .ForMember(x => x.CurrencySign, c => c.MapFrom(d => d.Currency.CurrencySign))
             .ForMember(x => x.FromAreaName, c => c.MapFrom(d => d.Area.AreaNameEn))
             .ForMember(x => x.ToAreaName, c => c.MapFrom(d => d.Area1.AreaNameEn))

              .ForMember(x => x.FromCityName, c => c.MapFrom(d => d.City.CityNameEn))
             .ForMember(x => x.ToCityName, c => c.MapFrom(d => d.City1.CityNameEn))
             
             ;
            Mapper.CreateMap<Contractor, ContractorVm>().IgnoreAllNonExisting();
            Mapper.Map(dbList, contractRateVmList);
            return contractRateVmList;

        }

        internal static JObject GetContractorRates(System.Web.Mvc.FormCollection form)
        {
            List<ContractorRateVm> contractorRateList = ContractorRateHelper.GetContractorRatesInquiry(form);

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in contractorRateList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("ContractorRateId");
                pJTokenWriter.WriteValue(item.ContractorRateId);

                pJTokenWriter.WritePropertyName("ContractorName");
                pJTokenWriter.WriteValue(item.ContractorName);

                pJTokenWriter.WritePropertyName("ContainerTypeName");
                pJTokenWriter.WriteValue(item.ContainerTypeName);

                pJTokenWriter.WritePropertyName("FromArea");
                pJTokenWriter.WriteValue(item.FromCityName + " / " + item.FromAreaName);

                pJTokenWriter.WritePropertyName("ToArea");
                pJTokenWriter.WriteValue(item.ToCityName + " / " + item.ToAreaName);

                pJTokenWriter.WritePropertyName("Cost");
                pJTokenWriter.WriteValue(item.CostAmount + "  " + item.CurrencySign);

                pJTokenWriter.WritePropertyName("ValidToDate");
                pJTokenWriter.WriteValue(item.ValidToDate == null ? "" : item.ValidToDate.Value.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("ContractorId");
                pJTokenWriter.WriteValue(item.ContractorId); 
                
                pJTokenWriter.WriteEndObject();

            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }
    }
}