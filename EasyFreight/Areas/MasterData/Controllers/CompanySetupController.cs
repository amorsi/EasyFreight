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
    public class CompanySetupController : Controller
    {
        //
        // GET: /MasterData/CompanySetup/
        [CheckRights(ScreenEnum.CompanySetup, ActionEnum.Edit)]
        public ActionResult Index()
        {

            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            CompanySetupVm compSetVm = CompanySetupHelper.GetCompanySetup();

            return View(compSetVm);
        }

        public ActionResult SaveTab(CompanySetupVm compSetVm)
        {
           string isSaved = CompanySetupHelper.AddEditCompSetup(compSetVm);
           return Json(isSaved);
        }


    }
}
