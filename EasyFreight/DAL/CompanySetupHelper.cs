using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.ViewModel;
using EasyFreight.Models;
using AutoMapper;

namespace EasyFreight.DAL
{
    public static class CompanySetupHelper
    {
        public static string AddEditCompSetup(CompanySetupVm compSetVm)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            if (compSetVm.TabIndex == 1) // First tab .. CompanySetup table
            {
                int rowsCount = db.CompanySetups.Count();
                CompanySetup compSetupDb;

                if (rowsCount == 0)
                    compSetupDb = new CompanySetup();
                else
                    compSetupDb = db.CompanySetups.FirstOrDefault();

                Mapper.CreateMap<CompanySetupVm, CompanySetup>();

                Mapper.Map(compSetVm, compSetupDb);

                compSetupDb.CompanyId = 1;
                //Add new
                if (rowsCount == 0)      
                    db.CompanySetups.Add(compSetupDb);


                db.SaveChanges();
            }
            else if (compSetVm.TabIndex == 2) // Second tab .. CompanySetupContact table
            {
                int rowsCount = db.CompanySetupContacts.Count();
                CompanySetupContact compSetupDb;

                if (rowsCount == 0)
                    compSetupDb = new CompanySetupContact();
                else
                    compSetupDb = db.CompanySetupContacts.FirstOrDefault();

                Mapper.CreateMap<CompanySetupVm, CompanySetupContact>();
                Mapper.Map(compSetVm, compSetupDb);

                compSetupDb.CompanyId = 1;

                //Add new
                if (rowsCount == 0)
                    db.CompanySetupContacts.Add(compSetupDb);


                db.SaveChanges();
            }

            else if (compSetVm.TabIndex == 3) // last tab .. SystemSetup table
            {
                int rowsCount = db.SystemSetups.Count();
                SystemSetup compSetupDb;

                if (rowsCount == 0)
                    compSetupDb = new SystemSetup();
                else
                    compSetupDb = db.SystemSetups.FirstOrDefault();

                Mapper.CreateMap<CompanySetupVm, SystemSetup>();
                Mapper.Map(compSetVm, compSetupDb);

                compSetupDb.CompanyId = 1;

                //Add new
                if (rowsCount == 0)
                    db.SystemSetups.Add(compSetupDb);


                db.SaveChanges();
            }

            return isSaved;
        }

        public static CompanySetupVm GetCompanySetup()
        {
            CompanySetupVm compSetVm = new CompanySetupVm();
            EasyFreightEntities db = new EasyFreightEntities();
            CompanySetup compSetupDb = db.CompanySetups.FirstOrDefault();
            CompanySetupContact compSetupContactDb = db.CompanySetupContacts.FirstOrDefault();
       //     SystemSetup sysSetupDb = db.SystemSetups.FirstOrDefault();

            if (compSetupDb != null)
            {
                Mapper.CreateMap<CompanySetup, CompanySetupVm>();
                Mapper.Map(compSetupDb, compSetVm);
            }

            if (compSetupContactDb != null)
            {
                Mapper.CreateMap<CompanySetupContact, CompanySetupVm>();
                Mapper.Map(compSetupContactDb, compSetVm);
            }

            //if (sysSetupDb != null)
            //{
            //    Mapper.CreateMap<SystemSetup, CompanySetupVm>();
            //    Mapper.Map(sysSetupDb, compSetVm);
            //}


            return compSetVm;
        }
    }
}