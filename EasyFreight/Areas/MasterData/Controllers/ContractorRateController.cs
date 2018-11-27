using EasyFreight.DAL;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class ContractorRateController : Controller
    {
        //
        // GET: /MasterData/ContractorRate/
        [CheckRights(ScreenEnum.ContractorRate, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            var ContractRateList = ContractorRateHelper.GetContractorRatesList();

            return View(ContractRateList);
        }




        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ContractorRate, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ContractorRate, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion

            ContractorRateVm carrRateVm = ContractorRateHelper.GetContractorRateInfo(id);
            ViewBag.ContractorList = ListCommonHelper.GetContractorList();
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            ViewData["AreaList"] = ListCommonHelper.GetAreaGrouped();
            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            ViewData["CurrencyList"] = ListCommonHelper.GetCurrencyList();
            return View(carrRateVm);
        }

        public ActionResult AddEditContractorRate(ContractorRateVm carrRateVm )
        {
            string isSaved = ContractorRateHelper.AddEditContractorRate(carrRateVm);
            return Json(isSaved);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.ContractorRate, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = ContractorRateHelper.Delete(id);
            return Json(isDeleted);
        }
    }
}
