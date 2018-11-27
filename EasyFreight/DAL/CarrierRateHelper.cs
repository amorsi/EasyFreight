using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.ViewModel;
using EasyFreight.Models;
using AutoMapper;
using System.Data.Entity.Validation;
using Newtonsoft.Json.Linq;

namespace EasyFreight.DAL
{
    public static class CarrierRateHelper
    {
        public static string AddEditCarrierRate(CarrierRateVm carrRateVm)
        {

            string isSaved = "true";
            bool isSameActive = CheckIfActiveRate(carrRateVm);
            if (isSameActive)
                return "false. Another rate is active with same data";

            EasyFreightEntities db = new EasyFreightEntities();
            CarrierRate carrRateDb;
            int carrRateVmId = carrRateVm.CarrierRateId;
            if (carrRateVmId == 0)
                carrRateDb = new CarrierRate();
            else
                carrRateDb = db.CarrierRates.Include("CarrierRateTransits").Where(x => x.CarrierRateId == carrRateVmId).FirstOrDefault();

            Mapper.CreateMap<CarrierRateVm, CarrierRate>().IgnoreAllNonExisting();
            Mapper.Map(carrRateVm, carrRateDb);

            var transitList = carrRateDb.CarrierRateTransits.ToList();

            //Delete all transit and insert it again
            if (carrRateDb.CarrierRateTransits.Count > 0)
            {
                foreach (var item in transitList)
                {
                    carrRateDb.CarrierRateTransits.Remove(item);
                }
            }

            if (carrRateVm.TransitPortId != null)
            {
                CarrierRateTransit carrTransit;
                foreach (var item in carrRateVm.TransitPortId)
                {
                    carrTransit = new CarrierRateTransit();
                    carrTransit.CarrierRateId = carrRateVm.CarrierRateId;
                    carrTransit.TransitPortId = item;
                    carrRateDb.CarrierRateTransits.Add(carrTransit);
                }

            }

            if (carrRateVmId == 0)
                db.CarrierRates.Add(carrRateDb);

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

        public static List<CarrierRateVm> GetCarrierRatesList(bool getValidOnly = false)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<CarrierRateVm> carrRateVmList = new List<CarrierRateVm>();
            List<GetCarrierRate_Result> spResult = db.GetCarrierRate(getValidOnly).ToList();

            Mapper.CreateMap<GetCarrierRate_Result, CarrierRateVm>().IgnoreAllNonExisting();
            Mapper.Map(spResult, carrRateVmList);

            return carrRateVmList;

        }

        public static CarrierRateVm GetCarrierRateInfo(int carrRateId)
        {
            CarrierRateVm carrRateVm = new CarrierRateVm();
            if (carrRateId == 0)
                return carrRateVm;

            EasyFreightEntities db = new EasyFreightEntities();
            CarrierRate carrRateDb = db.CarrierRates.Include("CarrierRateTransits").Where(x => x.CarrierRateId == carrRateId).FirstOrDefault();
            Mapper.CreateMap<CarrierRate, CarrierRateVm>().IgnoreAllNonExisting();
            Mapper.Map(carrRateDb, carrRateVm);
            carrRateVm.TransitPortId = carrRateDb.CarrierRateTransits.Select(x => x.TransitPortId).ToArray();
            return carrRateVm;
        }

        private static bool CheckIfActiveRate(CarrierRateVm carrRateVm)
        {
            bool isActive = false;
            int carrId, fromPortId, toPortId, containerTypeId, carrRateId;
            carrId = carrRateVm.CarrierId;
            fromPortId = carrRateVm.FromPortId;
            toPortId = carrRateVm.ToPortId;
            containerTypeId = carrRateVm.ContainerTypeId;
            carrRateId = carrRateVm.CarrierRateId;

            EasyFreightEntities db = new EasyFreightEntities();
            if (carrRateId == 0)
                isActive = db.CarrierRates.Any(x => x.CarrierId == carrId
                    && x.ContainerTypeId == containerTypeId && x.FromPortId == fromPortId
                    && x.ToPortId == toPortId
                    && x.IsValid == true);
            else
                isActive = db.CarrierRates.Any(x => x.CarrierId == carrId
                       && x.ContainerTypeId == containerTypeId && x.FromPortId == fromPortId
                       && x.ToPortId == toPortId && x.CarrierRateId != carrRateId
                       && x.IsValid == true);

            return isActive;
        }

        /// <summary>
        /// Will set isValid to false .. will not be deleted
        /// </summary>
        /// <param name="carrRateId"></param>
        /// <returns></returns>
        public static string Delete(int carrRateId)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            CarrierRate carrRateDb = db.CarrierRates.Where(x => x.CarrierRateId == carrRateId).FirstOrDefault();
            carrRateDb.IsValid = false;
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
        public static List<CarrierRateVm> GetCarrierRatesInquiry(System.Web.Mvc.FormCollection form)
        {
            EasyFreightEntities db = new EasyFreightEntities();

            List<CarrierRateVm> carrRateVmList = GetCarrierRatesList(true);

            if (form == null || form.AllKeys.Count() == 0)
                return carrRateVmList;

            if (!string.IsNullOrEmpty(form["CarrierId"]))
            { 
                int carrId = int.Parse(form["CarrierId"]);
                carrRateVmList = carrRateVmList.Where(x => x.CarrierId == carrId).ToList();
            }
            if (!string.IsNullOrEmpty(form["ContainerTypeId"]))
            {
                int contId = int.Parse(form["ContainerTypeId"]);
                carrRateVmList = carrRateVmList.Where(x => x.ContainerTypeId == contId).ToList();
            }
            if (!string.IsNullOrEmpty(form["FromPortId"]))
            {
                int fromPortId = int.Parse(form["FromPortId"]);
                carrRateVmList = carrRateVmList.Where(x => x.FromPortId == fromPortId).ToList();
            }

            if (!string.IsNullOrEmpty(form["ToPortId"]))
            {
                int toPortId = int.Parse(form["ToPortId"]);
                carrRateVmList = carrRateVmList.Where(x => x.ToPortId == toPortId).ToList();
            }

            if (!string.IsNullOrEmpty(form["FromDate"]))
            {
                DateTime validDate = DateTime.Parse(form["FromDate"]);
                carrRateVmList = carrRateVmList.Where(x => x.ValidToDate >= validDate).ToList();
            }

            if (!string.IsNullOrEmpty(form["ToDate"]))
            {
                DateTime validDate = DateTime.Parse(form["ToDate"]);
                carrRateVmList = carrRateVmList.Where(x => x.ValidToDate <= validDate).ToList();
            }
                       
            return carrRateVmList;
        }


        internal static JObject GetCarrierRates(System.Web.Mvc.FormCollection form)
        {
            List<CarrierRateVm> carrRateList = CarrierRateHelper.GetCarrierRatesInquiry(form);

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in carrRateList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("CarrierRateId");
                pJTokenWriter.WriteValue(item.CarrierRateId);

                pJTokenWriter.WritePropertyName("CarrierName");
                pJTokenWriter.WriteValue(item.CarrierName);

                pJTokenWriter.WritePropertyName("ContainerTypeName");
                pJTokenWriter.WriteValue(item.ContainerTypeName);

                pJTokenWriter.WritePropertyName("FromPort");
                pJTokenWriter.WriteValue(item.FromCountryName + " / " + item.FromPortName);

                pJTokenWriter.WritePropertyName("ToPort");
                pJTokenWriter.WriteValue(item.ToCountryName + " / " + item.ToPortName);

                pJTokenWriter.WritePropertyName("FreightCost");
                pJTokenWriter.WriteValue(item.FreightCostAmount + "  " + item.FreighCurrencySign);

                pJTokenWriter.WritePropertyName("ThcCost");
                pJTokenWriter.WriteValue(item.ThcCostAmount + "  " + item.ThcCurrencySign);

                pJTokenWriter.WritePropertyName("ValidToDate");
                pJTokenWriter.WriteValue(item.ValidToDate == null ? "" : item.ValidToDate.Value.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("CarrierId");
                pJTokenWriter.WriteValue(item.CarrierId);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }
    }
}