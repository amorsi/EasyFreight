using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.ViewModel;
using EasyFreight.Models;
using AutoMapper;
using System.Data.Entity.Validation;


namespace EasyFreight.DAL
{
    public static class CarrierHelper
    {
        public static string AddEditCarrier(CarrierVm carrVm)
        {
            int carrId = carrVm.CarrierId;
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Carrier carrDb;
            List<CarrierContact> dbContactList = new List<CarrierContact>();
            if (carrId == 0) //Add new case
                carrDb = new Carrier();
            else
            {
                carrDb = db.Carriers.Include("CarrierContacts").Where(x => x.CarrierId == carrId).FirstOrDefault();
                //delete any removed contact on the screen
                dbContactList = carrDb.CarrierContacts.ToList();

                //Get contact Ids sent from the screen
                List<int> contactVmIds = carrVm.ContactPersons.Select(x => x.ContactId).ToList();
                var contactDel = dbContactList.Where(x => !contactVmIds.Contains(x.ContactId)).ToList();

                foreach (var item in contactDel)
                {
                    db.CarrierContacts.Remove(item);
                }

            }

            Mapper.CreateMap<CarrierVm, Carrier>().IgnoreAllNonExisting();
            Mapper.Map(carrVm, carrDb);



            if (carrId == 0)
            {
                Random rand = new Random();
                carrDb.CarrierCode = rand.Next(10000).ToString();
                db.Carriers.Add(carrDb);
            }

            Mapper.CreateMap<ContactPersonVm, CarrierContact>().IgnoreAllNonExisting()
            .ForMember(x => x.CarrierId, opts => opts.MapFrom(scr => scr.FkValue));

            CarrierContact carrContactDb;
            foreach (var item in carrVm.ContactPersons.Where(x=> !string.IsNullOrEmpty(x.ContactName)))
            {
                if (item.ContactId == 0)
                    carrContactDb = new CarrierContact();
                else
                {
                    int contVmId = item.ContactId;
                    carrContactDb = dbContactList.Where(x => x.ContactId == contVmId).FirstOrDefault();
                }

                Mapper.Map(item, carrContactDb);

                if (item.ContactId == 0)
                    carrDb.CarrierContacts.Add(carrContactDb);
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

        public static CarrierVm GetCarrierInfo(int carrId)
        {
            CarrierVm carrVmObj = new CarrierVm();
            if (carrId == 0)
            {
                ContactPersonVm contactVm = new ContactPersonVm();
                carrVmObj.ContactPersons.Add(contactVm);
            }
            else
            {
                EasyFreightEntities db = new EasyFreightEntities();
                Carrier carrDbObj = db.Carriers.Include("CarrierContacts")
                    .Where(x => x.CarrierId == carrId).FirstOrDefault();

                Mapper.CreateMap<Carrier, CarrierVm>().IgnoreAllNonExisting();
                Mapper.Map(carrDbObj, carrVmObj);

                Mapper.CreateMap<CarrierContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.CarrierId));

                ContactPersonVm contactVm;
                foreach (var item in carrDbObj.CarrierContacts)
                {
                    contactVm = new ContactPersonVm();
                    Mapper.Map(item, contactVm);
                    carrVmObj.ContactPersons.Add(contactVm);
                }
            }
            return carrVmObj;
        }

        public static List<CarrierVm> GetCarrierList()
        {
            List<CarrierVm> carrList = new List<CarrierVm>();
            EasyFreightEntities db = new EasyFreightEntities();
            var carrDbList = db.Carriers.ToList();
            Mapper.CreateMap<Carrier, CarrierVm>().IgnoreAllNonExisting();
            Mapper.Map(carrDbList, carrList);
            return carrList;
        }

        public static List<ContactPersonVm> GetContactsList(int carrierId)
        {
            List<ContactPersonVm> contactsList = new List<ContactPersonVm>();
            EasyFreightEntities db = new EasyFreightEntities();

            List<CarrierContact> carrContact = db.CarrierContacts.Where(x => x.CarrierId == carrierId).ToList();

            Mapper.CreateMap<CarrierContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.CarrierId));
            Mapper.Map(carrContact, contactsList);

            return contactsList;
        }

        public static string Delete(int id)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Carrier carrObj = db.Carriers.Where(x => x.CarrierId == id).FirstOrDefault();
            db.Carriers.Remove(carrObj);

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