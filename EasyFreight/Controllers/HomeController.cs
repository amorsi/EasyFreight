using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.ViewModel;
using EasyFreight.DAL;
using Newtonsoft.Json.Linq;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult UnAuthorized()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult GetPrintFooter()
        { 
            CompanySetupVm compSetVm = new CompanySetupVm();

            if (Session["CompVm"] == null)
            {
                compSetVm = CompanySetupHelper.GetCompanySetup();
                Session["CompVm"] = compSetVm;
            }
            else
                compSetVm = (CompanySetupVm)Session["CompVm"];

            return PartialView("~/Views/Shared/_PrintFooter.cshtml", compSetVm);           
        }

        public JObject GetByOrderType(int year = 0)
        {
            var serviceUsageData = DashBoardHelper.GetCountByOrderTypePie(year);
            return serviceUsageData;
        }

        public JObject GetByCarrierType(int year = 0)
        {
            var serviceUsageData = DashBoardHelper.GetCountByCarrierTypePie(year);
            return serviceUsageData;
        }

        public JObject GetByStatus(int year = 0)
        {
            var serviceUsageData = DashBoardHelper.GetCountByStatusPie(year);
            return serviceUsageData;
        }

        public JObject GetOperationsCountLine(int year = 0)
        {
            var serviceUsageData = DashBoardHelper.GetOperationsCountLineChart(year);
            return serviceUsageData;
        }
    }
}