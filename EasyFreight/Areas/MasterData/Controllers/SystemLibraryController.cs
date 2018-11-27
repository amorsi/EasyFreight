using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class SystemLibraryController : Controller
    {
        //
        // GET: /MasterData/SystemLibrary/

        #region ContainerType Library -- Kamal 13/10/2015
        [CheckRights(ScreenEnum.ContainersTypes, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            var containersTypeList = LibraryCommonHelper.GetContainersTypeList();
            return View(containersTypeList);
        }

        public ActionResult AddEditContainerType(int containerTypeId, string containerTypeName, string maxCBM, string maxWeight)
        {
            #region Check Rights
            bool hasRights = false;
            if (containerTypeId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ContainersTypes, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ContainersTypes, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.AddEditContainerType(containerTypeId, containerTypeName, maxCBM, maxWeight));
        }

        public ActionResult DeleteContainerType(int containerTypeId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.ContainersTypes, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.DeleteContainerType(containerTypeId));
        }

        #endregion



        #region PackageType Library -- Kamal 13/10/2015
        [CheckRights(ScreenEnum.PackagesTypes, ActionEnum.ViewAll)]
        public ActionResult PackageType()
        {
            var packageTypeList = LibraryCommonHelper.GetPackageTypesList();
            return View(packageTypeList);
        }

        public ActionResult AddEditPackageType(int packageTypeId, string packageTypeNameEn, string packageTypeNameAr)
        {
            #region Check Rights
            bool hasRights = false;
            if (packageTypeId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.PackagesTypes, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.PackagesTypes, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            return Json(LibraryCommonHelper.AddEditPackageType(packageTypeId, packageTypeNameEn, packageTypeNameAr));
        }

        public ActionResult DeletePackageType(int packageTypeId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.PackagesTypes, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.DeletePackageType(packageTypeId));
        }


        #endregion

        #region Vessel Library

        [CheckRights(ScreenEnum.VesselsLibrary, ActionEnum.ViewAll)]
        public ActionResult VesselLibrary()
        {
            var vesselList = LibraryCommonHelper.GetVesselList();
            return View(vesselList);
        }

        public ActionResult AddEditVessel(int vesselId, string vesselName)
        {
            #region Check Rights
            bool hasRights = false;
            if (vesselId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.VesselsLibrary, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.VesselsLibrary, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");
            #endregion

            string isSaved = LibraryCommonHelper.AddEditVessel(vesselId, vesselName);
            return Json(isSaved);
        }

        public ActionResult DeleteVessel(int vesselId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.VesselsLibrary, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.DeleteVessel(vesselId));
        }

        #endregion


        #region Incoterm Library  
        [CheckRights(ScreenEnum.IncotermLibrary, ActionEnum.ViewAll)]
        public ActionResult Incoterm()
        {
            var incotermList = LibraryCommonHelper.GetIncotermList();
            return View(incotermList);
        }

        public ActionResult AddEditIncoterm(int incotermId, string incotermCode, string incotermName)
        {
            #region Check Rights
            bool hasRights = false;
            if (incotermId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.IncotermLibrary, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.IncotermLibrary, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.AddEditIncoterm(incotermId, incotermCode, incotermName));
        }

        public ActionResult DeleteIncoterm(int incotermId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.IncotermLibrary, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.DeleteIncoterm(incotermId));
        }

        #endregion

        #region Expenses Library Morsi 27-07-2016

        [CheckRights(ScreenEnum.ExpensesLibrary, ActionEnum.ViewAll)]
        public ActionResult ExpensesLibrary()
        {
            var expensesList = LibraryCommonHelper.GetExpenseLibList();
            return View(expensesList);
        }

        public ActionResult AddEditExpense(int expenseId, string expenseNameEn, string expenseNameAr)
        {
            #region Check Rights
            bool hasRights = false;
            if (expenseId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExpensesLibrary, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExpensesLibrary, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.AddEditExpense(expenseId, expenseNameEn, expenseNameAr));
        }

        public ActionResult DeleteExpense(int expenseId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExpensesLibrary, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            return Json(LibraryCommonHelper.DeleteExpense(expenseId));
        }

        #endregion

    }
}
