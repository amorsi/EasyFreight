using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EasyFreight.DAL
{
    public static class ShipperHelper
    {
        public static string AddEditShipper(ShipperVm shipperVm)
        {
            int carrId = shipperVm.ShipperId;
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Shipper carrDb;
            List<ShipperContact> dbContactList = new List<ShipperContact>();
            if (carrId == 0) //Add new case
                carrDb = new Shipper();
            else
            {
                carrDb = db.Shippers.Include("ShipperContacts").Where(x => x.ShipperId == carrId).FirstOrDefault();
                //delete any removed contact on the screen
                dbContactList = carrDb.ShipperContacts.ToList();

                try
                {
                    //Get contact Ids sent from the screen
                    List<int> contactVmIds = shipperVm.ContactPersons.Select(x => x.ContactId).ToList();
                    var contactDel = dbContactList.Where(x => !contactVmIds.Contains(x.ContactId)).ToList();

                    foreach (var item in contactDel)
                    {
                        db.ShipperContacts.Remove(item);
                    }
                }
                catch { }

            }

            Mapper.CreateMap<ShipperVm, Shipper>().IgnoreAllNonExisting();
            Mapper.Map(shipperVm, carrDb);



            if (carrId == 0)
            {
                Random rand = new Random();
                carrDb.ShipperCode = rand.Next(10000).ToString();
                db.Shippers.Add(carrDb);
            }

            Mapper.CreateMap<ContactPersonVm, ShipperContact>().IgnoreAllNonExisting()
            .ForMember(x => x.ShipperId, opts => opts.MapFrom(scr => scr.FkValue));

            ShipperContact carrContactDb;
            if (shipperVm.ContactPersons !=null && shipperVm.ContactPersons.Count > 0)
            {
                foreach (var item in shipperVm.ContactPersons.Where(x => !string.IsNullOrEmpty(x.ContactName)))
                {
                    if (item.ContactId == 0)
                        carrContactDb = new ShipperContact();
                    else
                    {
                        int contVmId = item.ContactId;
                        carrContactDb = dbContactList.Where(x => x.ContactId == contVmId).FirstOrDefault();
                    }

                    Mapper.Map(item, carrContactDb);

                    if (item.ContactId == 0)
                        carrDb.ShipperContacts.Add(carrContactDb);
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
                isSaved = "false " + e.InnerException.InnerException;
            }

            return isSaved;
        }

        public static ShipperVm GetShipperInfo(int id)
        {
            ShipperVm carrVmObj = new ShipperVm();
            if (id == 0)
            {
                ContactPersonVm contactVm = new ContactPersonVm();
                carrVmObj.ContactPersons.Add(contactVm);
            }
            else
            {
                EasyFreightEntities db = new EasyFreightEntities();
                Shipper carrDbObj = db.Shippers.Include("ShipperContacts")
                    .Where(x => x.ShipperId == id).FirstOrDefault();

                Mapper.CreateMap<Shipper, ShipperVm>().IgnoreAllNonExisting();
                Mapper.Map(carrDbObj, carrVmObj);

                Mapper.CreateMap<ShipperContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.ShipperId));

                ContactPersonVm contactVm;
                foreach (var item in carrDbObj.ShipperContacts)
                {
                    contactVm = new ContactPersonVm();
                    Mapper.Map(item, contactVm);
                    carrVmObj.ContactPersons.Add(contactVm);
                }
            }
            return carrVmObj;
        }

        public static List<ShipperVm> GetShipperList()
        {
            List<ShipperVm> shipperList = new List<ShipperVm>();
            EasyFreightEntities db = new EasyFreightEntities();
            var carrDbList = db.Shippers.ToList();
            Mapper.CreateMap<Shipper, ShipperVm>().IgnoreAllNonExisting();
            Mapper.Map(carrDbList, shipperList);
            return shipperList;
        }

        public static List<ContactPersonVm> GetContactsList(int shipperId)
        {
            List<ContactPersonVm> contactsList = new List<ContactPersonVm>();
            EasyFreightEntities db = new EasyFreightEntities();

            List<ShipperContact> carrContact = db.ShipperContacts.Where(x => x.ShipperId == shipperId).ToList();

            Mapper.CreateMap<ShipperContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.ShipperId));
            Mapper.Map(carrContact, contactsList);

            return contactsList;
        }

        public static string Delete(int id)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Shipper carrObj = db.Shippers.Where(x => x.ShipperId == id).FirstOrDefault();
            db.Shippers.Remove(carrObj);

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


        public static int AddShipperQuick(string code,string name)
        {
            Shipper shipperObj = new Shipper();
            shipperObj.ShipperCode = code;
            shipperObj.ShipperNameEn = name;

            EasyFreightEntities db = new EasyFreightEntities();
            string isSaved = "true";
            db.Shippers.Add(shipperObj);
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

            return shipperObj.ShipperId;
        }

    }
}