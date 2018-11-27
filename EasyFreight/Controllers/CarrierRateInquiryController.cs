using EasyFreight.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class CarrierRateInquiryController : Controller
    {
        //
        // GET: /CarrierRateInquiry/
        [CheckRights(ScreenEnum.CarrierRatePriceList,ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            ViewBag.CarrierList = ListCommonHelper.GetCarrierList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
           // var carrRateList = CarrierRateHelper.GetCarrierRatesList(true);
            return View();
        }

        public JObject GetTableJson(FormCollection form = null)
        {
            var carrRateList = CarrierRateHelper.GetCarrierRates(form);
            return carrRateList;
        }

        public ActionResult GetSearchResult(FormCollection form)
        {
            var carrRateList = CarrierRateHelper.GetCarrierRatesInquiry(form);
            return PartialView("~/Views/CarrierRateInquiry/_ResultTable.cshtml", carrRateList);
        }

    }
}
