using EasyFreight.DAL;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Areas.MasterData.Controllers
{
    public class ShipperController : Controller
    {
        //
        // GET: /MasterData/Shipper/
        [CheckRights(ScreenEnum.Shipper, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            List<ShipperVm> shipperList = ShipperHelper.GetShipperList();
            return View(shipperList);
        }

        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Shipper, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Shipper, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion
            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            ShipperVm shipperObj = ShipperHelper.GetShipperInfo(id);
            return View(shipperObj);
        }

        public ActionResult AddEditShipper(ShipperVm shipperVm)
        {
            string isSaved = ShipperHelper.AddEditShipper(shipperVm);
            return Json(isSaved);
        }

        public ActionResult AddShipperQuick(string code, string name)
        {
            int shipperId = ShipperHelper.AddShipperQuick(code, name);
            return Json(shipperId);
        }

        public ActionResult GetShipperContacts(int shipperId)
        {
            var contactList = ShipperHelper.GetContactsList(shipperId);
            return PartialView("~/Views/Shared/_ViewContacts.cshtml", contactList);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.Shipper, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = ShipperHelper.Delete(id);
            return Json(isDeleted);
        }


    }
}
