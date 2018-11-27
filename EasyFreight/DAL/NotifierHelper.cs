using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EasyFreight.DAL
{
    public static class NotifierHelper
    {
        public static string AddEditNotifier(NotifierVm carrVm)
        {
            int carrId = carrVm.NotifierId;
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Notifier carrDb;
            List<NotifierContact> dbContactList = new List<NotifierContact>();
            if (carrId == 0) //Add new case
                carrDb = new Notifier();
            else
            {
                carrDb = db.Notifiers.Include("NotifierContacts").Where(x => x.NotifierId == carrId).FirstOrDefault();
                //delete any removed contact on the screen
                dbContactList = carrDb.NotifierContacts.ToList();
                try
                {
                    //Get contact Ids sent from the screen
                    List<int> contactVmIds = carrVm.ContactPersons.Select(x => x.ContactId).ToList();
                    var contactDel = dbContactList.Where(x => !contactVmIds.Contains(x.ContactId)).ToList();

                    foreach (var item in contactDel)
                    {
                        db.NotifierContacts.Remove(item);
                    }
                }
                catch { }
            }
             
            Mapper.CreateMap<NotifierVm, Notifier>().IgnoreAllNonExisting();
            Mapper.Map(carrVm, carrDb);



            if (carrId == 0)
            {
                Random rand = new Random();
                carrDb.NotifierCode = rand.Next(10000).ToString();
                db.Notifiers.Add(carrDb);
            }

            Mapper.CreateMap<ContactPersonVm, NotifierContact>().IgnoreAllNonExisting()
            .ForMember(x => x.NotifierId, opts => opts.MapFrom(scr => scr.FkValue));

            NotifierContact carrContactDb;
            if (carrVm.ContactPersons != null && carrVm.ContactPersons.Count > 0)
            {
                foreach (var item in carrVm.ContactPersons.Where(x => !string.IsNullOrEmpty(x.ContactName)))
                {
                    if (item.ContactId == 0)
                        carrContactDb = new NotifierContact();
                    else
                    {
                        int contVmId = item.ContactId;
                        carrContactDb = dbContactList.Where(x => x.ContactId == contVmId).FirstOrDefault();
                    }

                    Mapper.Map(item, carrContactDb);

                    if (item.ContactId == 0)
                        carrDb.NotifierContacts.Add(carrContactDb);
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

        public static NotifierVm GetNotifierInfo(int id)
        {
            NotifierVm carrVmObj = new NotifierVm();
            if (id == 0)
            {
                ContactPersonVm contactVm = new ContactPersonVm();
                carrVmObj.ContactPersons.Add(contactVm);
            }
            else
            {
                EasyFreightEntities db = new EasyFreightEntities();
                Notifier carrDbObj = db.Notifiers.Include("NotifierContacts")
                    .Where(x => x.NotifierId == id).FirstOrDefault();

                Mapper.CreateMap<Notifier, NotifierVm>().IgnoreAllNonExisting();
                Mapper.Map(carrDbObj, carrVmObj);

                Mapper.CreateMap<NotifierContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.NotifierId));

                ContactPersonVm contactVm;
                foreach (var item in carrDbObj.NotifierContacts)
                {
                    contactVm = new ContactPersonVm();
                    Mapper.Map(item, contactVm);
                    carrVmObj.ContactPersons.Add(contactVm);
                }
            }
            return carrVmObj;
        }

        public static List<NotifierVm> GetNotifierList()
        {
            List<NotifierVm> notifierList = new List<NotifierVm>();
            EasyFreightEntities db = new EasyFreightEntities();
            var carrDbList = db.Notifiers.ToList();
            Mapper.CreateMap<Notifier, NotifierVm>().IgnoreAllNonExisting();
            Mapper.Map(carrDbList, notifierList);
            return notifierList;
        }

        public static List<ContactPersonVm> GetContactsList(int notifierId)
        {
            List<ContactPersonVm> contactsList = new List<ContactPersonVm>();
            EasyFreightEntities db = new EasyFreightEntities();

            List<NotifierContact> carrContact = db.NotifierContacts.Where(x => x.NotifierId == notifierId).ToList();

            Mapper.CreateMap<NotifierContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.NotifierId));
            Mapper.Map(carrContact, contactsList);

            return contactsList;
        }

        public static string Delete(int id)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Notifier carrObj = db.Notifiers.Where(x => x.NotifierId == id).FirstOrDefault();
            db.Notifiers.Remove(carrObj);

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
    }
}