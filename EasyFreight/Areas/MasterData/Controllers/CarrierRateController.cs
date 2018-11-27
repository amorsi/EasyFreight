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
    public class CarrierRateController : Controller
    {
        //
        // GET: /MasterData/CarrierRate/
        [CheckRights(ScreenEnum.CarrierRate, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            //var carrRateList = CarrierRateHelper.GetCarrierRatesList();

            return View();
        }

        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CarrierRate, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CarrierRate, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion
            CarrierRateVm carrRateVm = CarrierRateHelper.GetCarrierRateInfo(id);
            ViewBag.CarrierList = ListCommonHelper.GetCarrierList();
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            ViewData["CurrencyList"] = ListCommonHelper.GetCurrencyList();
            return View(carrRateVm);
        }


        public ActionResult AddEditCarrierRate(CarrierRateVm carrRateVm)
        {
            string isSaved = CarrierRateHelper.AddEditCarrierRate(carrRateVm);
            return Json(isSaved);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.CarrierRate, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = CarrierRateHelper.Delete(id);
            return Json(isDeleted);
        }


    }
}
