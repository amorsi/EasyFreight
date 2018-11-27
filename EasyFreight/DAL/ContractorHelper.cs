using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EasyFreight.DAL
{
    public static class ContractorHelper
    {
        public static string AddEditContractor(ContractorVm carrVm)
        {
            int carrId = carrVm.ContractorId;
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Contractor carrDb;
            List<ContractorContact> dbContactList = new List<ContractorContact>();
            if (carrId == 0) //Add new case
                carrDb = new Contractor();
            else
            {
                carrDb = db.Contractors.Include("ContractorContacts").Where(x => x.ContractorId == carrId).FirstOrDefault();
                //delete any removed contact on the screen
                dbContactList = carrDb.ContractorContacts.ToList();
                try
                {
                    //Get contact Ids sent from the screen
                    List<int> contactVmIds = carrVm.ContactPersons.Select(x => x.ContactId).ToList();
                    var contactDel = dbContactList.Where(x => !contactVmIds.Contains(x.ContactId)).ToList();

                    foreach (var item in contactDel)
                    {
                        db.ContractorContacts.Remove(item);
                    }
                }
                catch { }
            }

            Mapper.CreateMap<ContractorVm, Contractor>().IgnoreAllNonExisting();
            Mapper.Map(carrVm, carrDb);



            if (carrId == 0)
            {
                Random rand = new Random();
                carrDb.ContractorCode = rand.Next(10000).ToString();
                db.Contractors.Add(carrDb);
            }

            Mapper.CreateMap<ContactPersonVm, ContractorContact>().IgnoreAllNonExisting()
            .ForMember(x => x.ContractorId, opts => opts.MapFrom(scr => scr.FkValue));

            ContractorContact carrContactDb;
            if (carrVm.ContactPersons != null && carrVm.ContactPersons.Count > 0)
            {
                foreach (var item in carrVm.ContactPersons.Where(x => !string.IsNullOrEmpty(x.ContactName)))
                {
                    if (item.ContactId == 0)
                        carrContactDb = new ContractorContact();
                    else
                    {
                        int contVmId = item.ContactId;
                        carrContactDb = dbContactList.Where(x => x.ContactId == contVmId).FirstOrDefault();
                    }

                    Mapper.Map(item, carrContactDb);

                    if (item.ContactId == 0)
                        carrDb.ContractorContacts.Add(carrContactDb);
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

        public static ContractorVm GetContractorInfo(int id)
        {
            ContractorVm carrVmObj = new ContractorVm();
            if (id == 0)
            {
                ContactPersonVm contactVm = new ContactPersonVm();
                carrVmObj.ContactPersons.Add(contactVm);
            }
            else
            {
                EasyFreightEntities db = new EasyFreightEntities();
                Contractor carrDbObj = db.Contractors.Include("ContractorContacts")
                    .Where(x => x.ContractorId == id).FirstOrDefault();

                Mapper.CreateMap<Contractor, ContractorVm>().IgnoreAllNonExisting();
                Mapper.Map(carrDbObj, carrVmObj);

                Mapper.CreateMap<ContractorContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.ContractorId));

                ContactPersonVm contactVm;
                foreach (var item in carrDbObj.ContractorContacts)
                {
                    contactVm = new ContactPersonVm();
                    Mapper.Map(item, contactVm);
                    carrVmObj.ContactPersons.Add(contactVm);
                }
            }
            return carrVmObj;
        }

        public static List<ContractorVm> GetContractorList()
        {
            List<ContractorVm> contractorList = new List<ContractorVm>();
            EasyFreightEntities db = new EasyFreightEntities();
            var carrDbList = db.Contractors.ToList();
            Mapper.CreateMap<Contractor, ContractorVm>().IgnoreAllNonExisting();
            Mapper.Map(carrDbList, contractorList);
            return contractorList;
        }

        public static List<ContactPersonVm> GetContactsList(int contractorId)
        {
            List<ContactPersonVm> contactsList = new List<ContactPersonVm>();
            EasyFreightEntities db = new EasyFreightEntities();

            List<ContractorContact> carrContact = db.ContractorContacts.Where(x => x.ContractorId == contractorId).ToList();

            Mapper.CreateMap<ContractorContact, ContactPersonVm>().IgnoreAllNonExisting()
                .ForMember(x => x.FkValue, opts => opts.MapFrom(scr => scr.ContractorId));
            Mapper.Map(carrContact, contactsList);

            return contactsList;
        }

        public static string Delete(int id)
        {
            string isDeleted = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Contractor carrObj = db.Contractors.Where(x => x.ContractorId == id).FirstOrDefault();
            db.Contractors.Remove(carrObj);

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