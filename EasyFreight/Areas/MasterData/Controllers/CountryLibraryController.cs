using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class CountryLibraryController : Controller
    {
        //
        // GET: /MasterData/CountryLibrary/

        #region Country Library -- Kamal 05/10/2015
        [CheckRights(ScreenEnum.CountriesLibrary, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            var countyList = LibraryCommonHelper.GetCountriesList();
            return View(countyList);
        }

        public ActionResult AddEditCountry(int countryId, string countyNameEn, string countryNameAr)
        {
            #region Check Rights
            bool hasRights = false;
            if (countryId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CountriesLibrary, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CountriesLibrary, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            return Json(LibraryCommonHelper.AddEditCountry(countryId, countyNameEn, countryNameAr));
        }


        public ActionResult DeleteCountry(int countryId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.CountriesLibrary, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.DeleteCountry(countryId));
        }

        #endregion

        #region CityLibrary -- Kamal 10/10/2015
        [CheckRights(ScreenEnum.CitiesLibrary, ActionEnum.ViewAll)]
        public ActionResult CityLibrary()
        {
            var citiesList = LibraryCommonHelper.GetCitiesListByCountryId(0);
            ViewData["CountryList"] = ListCommonHelper.GetCountryList();
            return View(citiesList);
        }

        public ActionResult AddEditCity(EasyFreight.Models.City cityDb)
        {
            #region Check Rights
            bool hasRights;
            if (cityDb.CityId == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CitiesLibrary, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CitiesLibrary, ActionEnum.Edit);
            }

            if (!hasRights)
                return Json("false. You are UnAuthorized to do this action");

            #endregion
            string isSaved = LibraryCommonHelper.AddEditCities(cityDb);
            if(isSaved != "true")
                return Json(isSaved);
            var citiesList = LibraryCommonHelper.GetCitiesListByCountryId(0);
            return PartialView("~/Areas/MasterData/Views/CountryLibrary/_CityTable.cshtml", citiesList);
        }

        public ActionResult DeleteCity(int cityId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.CitiesLibrary, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            string isDeleted = LibraryCommonHelper.DeleteCity(cityId);
            return Json(isDeleted);
        }

        #endregion

        #region Ports Library -- Morsi 01/10/2015
        [CheckRights(ScreenEnum.PortsLibrary, ActionEnum.ViewAll)]
        public ActionResult PortLibrary()
        {
            var portsList = LibraryCommonHelper.GetPortsListByCityId(0);
            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            return View(portsList);
        }

        public ActionResult AddEditPort(EasyFreight.Models.Port portDb)
        {
            #region Check Rights
            bool hasRights;
            if (portDb.PortId == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.PortsLibrary, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.PortsLibrary, ActionEnum.Edit);
            }

            if (!hasRights)
                return Json("false. You are UnAuthorized to do this action");

            #endregion

            string isSaved = LibraryCommonHelper.AddEditPort(portDb);
            if (isSaved != "true")
                return Json(isSaved);
            var portsList = LibraryCommonHelper.GetPortsListByCityId(0);
            return PartialView("~/Areas/MasterData/Views/CountryLibrary/_PortsTable.cshtml", portsList);
        }

        public ActionResult DeletePort(int portId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.PortsLibrary, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = LibraryCommonHelper.DeletePort(portId);
            return Json(isDeleted);
        }

        #endregion

        #region Areas Library -- Morsi 02/10/2015
        [CheckRights(ScreenEnum.AreasLibrary, ActionEnum.ViewAll)]
        public ActionResult AreaLibrary()
        {

            var areasList = LibraryCommonHelper.GetAreasListByCityId(0);
            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            return View(areasList);
        }

        public ActionResult AddEditArea(EasyFreight.Models.Area areaDb)
        {
            #region Check Rights
            bool hasRights;
            if (areaDb.AreaId == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.AreasLibrary, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.AreasLibrary, ActionEnum.Edit);
            }

            if (!hasRights)
                return Json("false. You are UnAuthorized to do this action");

            #endregion
           string isSaved = LibraryCommonHelper.AddEditArea(areaDb);
            if (isSaved != "true")
                return Json(isSaved);

            var areasList = LibraryCommonHelper.GetAreasListByCityId(0);
            return PartialView("~/Areas/MasterData/Views/CountryLibrary/_AreasTable.cshtml", areasList);
        }

        public ActionResult DeleteArea(int areaId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.AreasLibrary, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = LibraryCommonHelper.DeleteArea(areaId);
            return Json(isDeleted);
        }

        #endregion



    }
}
