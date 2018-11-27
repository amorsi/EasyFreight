using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;
using EasyFreight.ViewModel;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class CarrierController : Controller
    {
        //
        // GET: /MasterData/Carrier/
        [CheckRights(ScreenEnum.Carrier, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            List<CarrierVm> carrList = CarrierHelper.GetCarrierList();
            return View(carrList);
        }

        public ActionResult Add(int carrId =0)
        {
            #region Check Rights
            bool hasRights;
            if (carrId == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Carrier, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Carrier, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion
            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            CarrierVm carrierObj = CarrierHelper.GetCarrierInfo(carrId);
            return View(carrierObj);
        }

        public ActionResult AddEditCarrier(CarrierVm carrVm)
        {
           string isSaved = CarrierHelper.AddEditCarrier(carrVm);
           return Json(isSaved);
        }

        public ActionResult GetCarrierContacts(int carrierId)
        {
           var contactList = CarrierHelper.GetContactsList(carrierId);
           return PartialView("~/Views/Shared/_ViewContacts.cshtml", contactList);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.Carrier, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = CarrierHelper.Delete(id);
            return Json(isDeleted);
        }
    }
}
