using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EasyFreight.DAL
{
    public static class ConsigneeHelper
    {
        public static string AddEditConsignee(ConsigneeVm carrVm)
        {
            int carrId = carrVm.ConsigneeId;
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Consignee carrDb;
            List<ConsigneeContact> dbContactList = new List<ConsigneeContact>();
            if (carrId == 0) //Add new case
                carrDb = new Consignee();
            else
            {
                carrDb = db.Consignees.Include("ConsigneeContacts").Where(x => x.ConsigneeId == carrId).FirstOrDefault();
                //delete any removed contact on the screen
                dbContactList = carrDb.ConsigneeContacts.ToList();
                try
                {
                    //Get contact Ids sent from the screen
                    List<int> contactVmIds = carrVm.ContactPersons.Select(x => x.ContactId).ToList();
                    var contactDel = dbContactList.Where(x => !contactVmIds.Contains(x.ContactId)).ToList();

                    foreach (var item in contactDel)
                    {
                        db.ConsigneeContacts.Remove(item);
                    }
                }
                catch { }
            }

            Mapper.CreateMap<ConsigneeVm, Consignee>().IgnoreAllNonExisting();
            Mapper.Map(carrVm, carrDb);



            if (carrId == 0)
            {
                Random rand = new Random();
                carrDb.ConsigneeCode = rand.Next(10000).ToString();
                db.Consignees.Add(carrDb);
            }

            Mapper.CreateMap<ContactPersonVm, ConsigneeContact>().IgnoreAllNonExisting()
            .ForMember(x => x.ConsigneeId, opts => opts.MapFrom(scr => scr.FkValue));

            ConsigneeContact carrContactDb;
            if (carrVm.ContactPersons != null && carrVm.ContactPersons.Count > 0)
            {
                foreach (var item in carrVm.ContactPersons.Where(x => !string.IsNullOrEmpty(x.ContactName)))
                {
                    if (item.ContactId == 0)
                        carrContactDb = new ConsigneeContact();
                    else
                    {
                        int contVmId = item.ContactId;
                        carrContactDb = dbContactList.Where(x => x.ContactId == contVmId).FirstOrDefault();
                    }

                    Mapper.Map(item, carrContactDb);

                    if (item.ContactId == 0)
                        carrDb.ConsigneeContacts.Add(carrContactDb);
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

        public static ConsigneeVm GetConsigneeInfo(int id)
        {
            ConsigneeVm carrVmObj = new ConsigneeVm();
            if (id == 0)
            {
                ContactPersonVm contactVm = new ContactPersonVm();
                carrVmObj.ContactPersons.Add(contactVm);
            }
            else
            {
                EasyFreightEntities db = new EasyFreightEntities();
                Consignee carrDbObj = db.Consignees.Include("ConsigneeContacts")
                    .Where(x => x.ConsigneeId == id).FirstOrDefault();

                Mapper.CreateMap<Consignee, ConsigneeVm>().IgnoreAllNonExisting();
                Mapper.Map(carrDbObj, carrVmObj);

                Mapper.CreateMap<ConsigneeContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.ConsigneeId));

                ContactPersonVm contactVm;
                foreach (var item in carrDbObj.ConsigneeContacts)
                {
                    contactVm = new ContactPersonVm();
                    Mapper.Map(item, contactVm);
                    carrVmObj.ContactPersons.Add(contactVm);
                }
            }
            return carrVmObj;
        }

        public static List<ConsigneeVm> GetConsigneeList()
        {
            List<ConsigneeVm> consigneeList = new List<ConsigneeVm>();
            EasyFreightEntities db = new EasyFreightEntities();
            var carrDbList = db.Consignees.ToList();
            Mapper.CreateMap<Consignee, ConsigneeVm>().IgnoreAllNonExisting();
            Mapper.Map(carrDbList, consigneeList);
            return consigneeList;
        }

        public static List<ContactPersonVm> GetContactsList(int consigneeId)
        {
            List<ContactPersonVm> contactsList = new List<ContactPersonVm>();
            EasyFreightEntities db = new EasyFreightEntities();

            List<ConsigneeContact> carrContact = db.ConsigneeContacts.Where(x => x.ConsigneeId == consigneeId).ToList();

            Mapper.CreateMap<ConsigneeContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.ConsigneeId));
            Mapper.Map(carrContact, contactsList);

            return contactsList;
        }

        public static string Delete(int id)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Consignee carrObj = db.Consignees.Where(x => x.ConsigneeId == id).FirstOrDefault();
            db.Consignees.Remove(carrObj);

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

        public static int AddConsigneeQuick(string code, string name)
        {
            Consignee consigneeObj = new Consignee();
            consigneeObj.ConsigneeCode = code;
            consigneeObj.ConsigneeNameEn = name;

            EasyFreightEntities db = new EasyFreightEntities();
            string isSaved = "true";
            db.Consignees.Add(consigneeObj);
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
                isSaved = "false " + e.InnerException.InnerException;
            }

            return consigneeObj.ConsigneeId;
        }
    }
}