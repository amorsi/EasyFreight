using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace EasyFreight.DAL
{
    public static class AgentHelper
    {

        public static string AddEditAgent(AgentVm carrVm)
        {
            int carrId = carrVm.AgentId;
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Agent carrDb;
            List<AgentContact> dbContactList = new List<AgentContact>();
            if (carrId == 0) //Add new case
                carrDb = new Agent();
            else
            {
                carrDb = db.Agents.Include("AgentContacts").Where(x => x.AgentId == carrId).FirstOrDefault();
                //delete any removed contact on the screen
                dbContactList = carrDb.AgentContacts.ToList();
                try
                {
                    //Get contact Ids sent from the screen
                    List<int> contactVmIds = carrVm.ContactPersons.Select(x => x.ContactId).ToList();
                    var contactDel = dbContactList.Where(x => !contactVmIds.Contains(x.ContactId)).ToList();

                    foreach (var item in contactDel)
                    {
                        db.AgentContacts.Remove(item);
                    }
                }
                catch { }
            }

            Mapper.CreateMap<AgentVm, Agent>().IgnoreAllNonExisting();
            Mapper.Map(carrVm, carrDb);



            if (carrId == 0)
            {
                Random rand = new Random();
                carrDb.AgentCode = rand.Next(10000).ToString();
                db.Agents.Add(carrDb);
            }

            Mapper.CreateMap<ContactPersonVm, AgentContact>().IgnoreAllNonExisting()
            .ForMember(x => x.AgentId, opts => opts.MapFrom(scr => scr.FkValue));

            AgentContact carrContactDb;
            if (carrVm.ContactPersons != null && carrVm.ContactPersons.Count > 0)
            {
                foreach (var item in carrVm.ContactPersons.Where(x => !string.IsNullOrEmpty(x.ContactName)))
                {
                    if (item.ContactId == 0)
                        carrContactDb = new AgentContact();
                    else
                    {
                        int contVmId = item.ContactId;
                        carrContactDb = dbContactList.Where(x => x.ContactId == contVmId).FirstOrDefault();
                    }

                    Mapper.Map(item, carrContactDb);

                    if (item.ContactId == 0)
                        carrDb.AgentContacts.Add(carrContactDb);
                }
            }
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

        public static AgentVm GetAgentInfo(int id)
        {
            AgentVm carrVmObj = new AgentVm();
            if (id == 0)
            {
                ContactPersonVm contactVm = new ContactPersonVm();
                carrVmObj.ContactPersons.Add(contactVm);
            }
            else
            {
                EasyFreightEntities db = new EasyFreightEntities();
                Agent carrDbObj = db.Agents.Include("AgentContacts")
                    .Where(x => x.AgentId == id).FirstOrDefault();

                Mapper.CreateMap<Agent, AgentVm>().IgnoreAllNonExisting();
                Mapper.Map(carrDbObj, carrVmObj);

                Mapper.CreateMap<AgentContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.AgentId));

                ContactPersonVm contactVm;
                foreach (var item in carrDbObj.AgentContacts)
                {
                    contactVm = new ContactPersonVm();
                    Mapper.Map(item, contactVm);
                    carrVmObj.ContactPersons.Add(contactVm);
                }
            }
            return carrVmObj;
        }

        public static List<AgentVm> GetAgentList()
        {
            List<AgentVm> agentList = new List<AgentVm>();
            EasyFreightEntities db = new EasyFreightEntities();
            var carrDbList = db.Agents.ToList();
            Mapper.CreateMap<Agent, AgentVm>().IgnoreAllNonExisting();
            Mapper.Map(carrDbList, agentList);
            return agentList;
        }

        public static List<ContactPersonVm> GetContactsList(int agentId)
        {
            List<ContactPersonVm> contactsList = new List<ContactPersonVm>();
            EasyFreightEntities db = new EasyFreightEntities();

            List<AgentContact> carrContact = db.AgentContacts.Where(x => x.AgentId == agentId).ToList();

            Mapper.CreateMap<AgentContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.AgentId));
            Mapper.Map(carrContact, contactsList);

            return contactsList;
        }

        public static string Delete(int id)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Agent carrObj = db.Agents.Where(x => x.AgentId == id).FirstOrDefault();
            db.Agents.Remove(carrObj);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isDeleted = "false " + ex.Message;
            }

            return isDeleted;
        }

        public static List<AgentVm> GetAgentListByCountry(int countryId=0)
        {
            List<AgentVm> agentList = new List<AgentVm>();
            EasyFreightEntities db = new EasyFreightEntities();
             var carrDbList = db.Agents.Where(c => c.CountryId == (countryId > 0 ? countryId : c.CountryId)).ToList();
            Mapper.CreateMap<Agent, AgentVm>().IgnoreAllNonExisting();
            Mapper.Map(carrDbList, agentList); 
            return agentList;
        }
         
        public static JObject GetAllAgentsByCountry(int countryId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();


           
                var selectedAgents = db.Agents.Include("Countries").Where(c => c.CountryId == (countryId > 0 ? countryId : c.CountryId))
                      .Select(x => new
                      {
                          x.AgentAddressEn,
                          x.AgentCode,
                          x.AgentId,
                          x.AgentNameEn,
                          x.Country.CountryNameEn,
                          x.CountryId,
                          x.Email,
                          x.FaxNumber,
                          x.PhoneNumber,
                          x.WebSite
                      }).ToList();
          
             

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in selectedAgents)
            {
                pJTokenWriter.WriteStartObject();
                pJTokenWriter.WritePropertyName("AgentCode");
                pJTokenWriter.WriteValue(item.AgentCode);

                pJTokenWriter.WritePropertyName("AgentAddressEn");
                pJTokenWriter.WriteValue(item.AgentAddressEn);

                pJTokenWriter.WritePropertyName("AgentNameEn");
                pJTokenWriter.WriteValue(item.AgentNameEn);

               

                pJTokenWriter.WritePropertyName("Country");
                pJTokenWriter.WriteValue(item.CountryNameEn);

                pJTokenWriter.WritePropertyName("CountryId");
                pJTokenWriter.WriteValue(item.CountryId);

                pJTokenWriter.WritePropertyName("Email");
                pJTokenWriter.WriteValue(item.Email);

                pJTokenWriter.WritePropertyName("FaxNumber");
                pJTokenWriter.WriteValue(item.FaxNumber);

                pJTokenWriter.WritePropertyName("PhoneNumber");
                pJTokenWriter.WriteValue(item.PhoneNumber);

                pJTokenWriter.WritePropertyName("WebSite");
                pJTokenWriter.WriteValue(item.WebSite);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }
 
    }
}