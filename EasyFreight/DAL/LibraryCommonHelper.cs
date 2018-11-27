using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using System.Data;
using System.Data.Entity;
using System.Transactions;
using System.Data.Entity.Validation;

namespace EasyFreight.DAL
{
    public static class LibraryCommonHelper
    {


        #region PortsLibrary --Morsi 01/10/2015 --
        public static List<Port> GetPortsListByCityId(int cityId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<Port> portsList = db.Ports.Include("Country").Include("City").ToList();

            if (cityId != 0)
                portsList.Where(x => x.CityId == cityId).ToList();

            return portsList;
        }

        public static string AddEditPort(Port portDb)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();

            if (portDb.PortId == 0)
                db.Ports.Add(portDb);
            else
            {
                db.Ports.Attach(portDb);
                db.Entry(portDb).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        public static string DeletePort(int portId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            Port portToDel = db.Ports.Where(x => x.PortId == portId).FirstOrDefault();
            db.Ports.Remove(portToDel);

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

        #endregion

        #region AreaLibrary --Morsi 02/10/2015 --
        public static List<Area> GetAreasListByCityId(int cityId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<Area> areasList = db.Areas.Include("Country").Include("City").ToList();

            if (cityId != 0)
                areasList.Where(x => x.CityId == cityId).ToList();

            return areasList;
        }

        public static string AddEditArea(Area areaDb)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();

            if (areaDb.AreaId == 0)
                db.Areas.Add(areaDb);
            else
            {
                db.Areas.Attach(areaDb);
                db.Entry(areaDb).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        public static string DeleteArea(int areaId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            Area areaToDel = db.Areas.Where(x => x.AreaId == areaId).FirstOrDefault();
            db.Areas.Remove(areaToDel);

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

        #endregion

        #region CountriesLibrary --kamal 05/10/2015 --

        public static List<Country> GetCountriesList()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<Country> countriesList = db.Countries.ToList();
            return countriesList;
        }

        public static string AddEditCountry(int countryId, string countyNameEn, string countryNameAr)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Country countryDb = new Country();
            countryDb.CountryId = countryId;
            countryDb.CountryNameAr = countryNameAr;
            countryDb.CountryNameEn = countyNameEn;

            if (string.IsNullOrEmpty(countyNameEn))
                return "false";

            if (countryId == 0)
            {
                db.Countries.Add(countryDb);
            }
            else
            {
                db.Countries.Attach(countryDb);
                db.Entry(countryDb).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        public static string DeleteCountry(int countryId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            Country countryToDel = db.Countries.Where(x => x.CountryId == countryId).FirstOrDefault();
            db.Countries.Remove(countryToDel);

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

        #endregion

        #region CitiesLibrary --Kamal 10/10/2015 --
        public static List<City> GetCitiesListByCountryId(int countryId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<City> citiesList = db.Cities.Include("Country").ToList();

            if (countryId != 0)
                citiesList.Where(x => x.CountryId == countryId).ToList();

            return citiesList;
        }

        public static string AddEditCities(City cityDb)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();

            if (cityDb.CityId == 0)
                db.Cities.Add(cityDb);
            else
            {
                db.Cities.Attach(cityDb);
                db.Entry(cityDb).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        public static string DeleteCity(int cityId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            City cityToDel = db.Cities.Where(x => x.CityId == cityId).FirstOrDefault();
            db.Cities.Remove(cityToDel);

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

        #endregion


        #region ContainerType Library --kamal 13/10/2015 --

        public static List<ContainerType> GetContainersTypeList()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<ContainerType> containersTypeList = db.ContainerTypes.ToList();
            return containersTypeList;
        }

        public static string AddEditContainerType(int containerTypeId, string containerTypeName, string maxCBM, string maxWeight)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            ContainerType containerTypeDb = new ContainerType();
            containerTypeDb.ContainerTypeId = containerTypeId;
            containerTypeDb.ContainerTypeName = containerTypeName;
            containerTypeDb.MaxCBM = decimal.Parse(maxCBM);
            containerTypeDb.MaxWeight = decimal.Parse(maxWeight);

            if (string.IsNullOrEmpty(containerTypeName))
                return "false";

            if (containerTypeId == 0)
            {
                db.ContainerTypes.Add(containerTypeDb);
            }
            else
            {
                db.ContainerTypes.Attach(containerTypeDb);
                db.Entry(containerTypeDb).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        public static string DeleteContainerType(int containerTypeId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            ContainerType countryToDel = db.ContainerTypes.Where(x => x.ContainerTypeId == containerTypeId).FirstOrDefault();
            db.ContainerTypes.Remove(countryToDel);

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

        #endregion

        #region Incoterm Library --kamal 15/04/2015 --

        public static List<IncotermLib> GetIncotermList()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<IncotermLib> incotermLibList = db.IncotermLibs.ToList();
            return incotermLibList;
        }

        public static string AddEditIncoterm(int incotermId, string incotermCode, string incotermName)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            IncotermLib incotermLibDb = new IncotermLib();
            incotermLibDb.IncotermId = incotermId;
            incotermLibDb.IncotermCode = incotermCode;
            incotermLibDb.IncotermName = incotermName;


            if (string.IsNullOrEmpty(incotermName))
                return "false";

            if (incotermId == 0)
            {
                db.IncotermLibs.Add(incotermLibDb);
            }
            else
            {
                db.IncotermLibs.Attach(incotermLibDb);
                db.Entry(incotermLibDb).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        public static string DeleteIncoterm(int incotermId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            IncotermLib countryToDel = db.IncotermLibs.Where(x => x.IncotermId == incotermId).FirstOrDefault();
            db.IncotermLibs.Remove(countryToDel);

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

        #endregion

        #region PackageTypes Library --kamal 13/10/2015 --

        public static List<PackageType> GetPackageTypesList()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<PackageType> packageTypeList = db.PackageTypes.ToList();
            return packageTypeList;
        }

        public static string AddEditPackageType(int packageTypeId, string packageTypeNameEn, string packageTypeNameAr)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            PackageType packageTypeDb = new PackageType();
            packageTypeDb.PackageTypeId = packageTypeId;
            packageTypeDb.PackageTypeNameEn = packageTypeNameEn;
            packageTypeDb.PackageTypeNameAr = packageTypeNameAr;

            if (string.IsNullOrEmpty(packageTypeNameEn))
                return "false";

            if (packageTypeId == 0)
            {
                db.PackageTypes.Add(packageTypeDb);
            }
            else
            {
                db.PackageTypes.Attach(packageTypeDb);
                db.Entry(packageTypeDb).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        public static string DeletePackageType(int packageTypeId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            PackageType packageToDel = db.PackageTypes.Where(x => x.PackageTypeId == packageTypeId).FirstOrDefault();
            db.PackageTypes.Remove(packageToDel);

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

        #endregion


        #region Vessel Library

        internal static List<Vessel> GetVesselList()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<Vessel> vesselList = db.Vessels.ToList();
            return vesselList;
        }

        internal static string AddEditVessel(int vesselId, string vesselName)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            Vessel vesselDb;
            if (vesselId == 0)
                vesselDb = new Vessel();
            else
                vesselDb = db.Vessels.Where(x => x.VesselId == vesselId).FirstOrDefault();

            vesselDb.VesselName = vesselName;

            if (vesselId == 0)
                db.Vessels.Add(vesselDb);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        internal static object DeleteVessel(int vesselId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";
            Vessel vesselDb = db.Vessels.Where(x => x.VesselId == vesselId).FirstOrDefault();

            db.Vessels.Remove(vesselDb);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                if ((ex.InnerException).InnerException.Message.Contains("DELETE statement conflicted"))
                    isDeleted = "false. Can't delete this row as it is linked with some operation orders";
                else
                    isDeleted = "false" + ex.Message;
            }

            return isDeleted;

        }

        #endregion

        #region Expenses Library --- Morsi 27-07-2016
        public static List<ExpenseLib> GetExpenseLibList()
        {
            AccountingEntities db = new AccountingEntities();
            List<ExpenseLib> expensesList = db.ExpenseLibs.ToList();
            return expensesList;
        }

        public static string AddEditExpense(int expenseId, string expenseNameEn, string expenseNameAr)
        {
            string isSaved = "";
            AccountingEntities db = new AccountingEntities();
            ExpenseLib expenseLibDb;
            if (expenseId != 0)
                expenseLibDb = db.ExpenseLibs.Where(x => x.ExpenseId == expenseId).FirstOrDefault();
            else
                expenseLibDb = new ExpenseLib();

            expenseLibDb.ExpenseNameEn = expenseNameEn;
            expenseLibDb.ExpenseNameAr = expenseNameAr;

            if (expenseId == 0)
                db.ExpenseLibs.Add(expenseLibDb);

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    db.SaveChanges();
                    if (expenseId == 0)
                    {
                        expenseId = expenseLibDb.ExpenseId;
                        string parentAccountId = ((int)AccountingChartEnum.GeneralAndAdministrativeExpenses).ToString();
                        string accountId = AccountingChartHelper.AddAccountToChart(expenseNameEn, expenseNameAr, parentAccountId);
                        AccountingChartHelper.AddAccountIdToObj(accountId, "ExpenseLib", expenseId, "ExpenseId");

                        isSaved = expenseId.ToString();
                    }

                    transaction.Complete();
                }
                catch (DbEntityValidationException e)
                {
                    isSaved = "false " + e.Message;
                }
                catch (Exception e)
                {
                    isSaved = "false " + e.Message;
                }
            }

            return isSaved;
        }

        public static string DeleteExpense(int expenseId)
        {
            string isDeleted = "true";
            AccountingEntities db = new AccountingEntities();
            string accountId = db.ExpenseLibs.Where(x => x.ExpenseId == expenseId).FirstOrDefault().AccountId;
            bool expIsUsed = db.AccTransactionDetails.Any(x => x.AccountId == accountId);
            if (expIsUsed)
                isDeleted = "Can't delete this expense as it has accounting transactions";
            else
            {
                var expenseObj = db.ExpenseLibs.Where(x => x.ExpenseId == expenseId).FirstOrDefault();
                var accountChartObj = db.AccountingCharts.Where(x => x.AccountId == accountId).FirstOrDefault();

                db.AccountingCharts.Remove(accountChartObj);
                db.ExpenseLibs.Remove(expenseObj);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    isDeleted = "false " + e.Message;
                }
                catch (Exception e)
                {
                    isDeleted = "false " + e.Message;
                }
            }


            return isDeleted;
        }


        #endregion
    }
}