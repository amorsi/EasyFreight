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
    public class ContractorRateInquiryController : Controller
    {
        //
        // GET: /ContractorRateInquiry/
        [CheckRights(ScreenEnum.ContractorRateList, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            ViewBag.ContractorList = ListCommonHelper.GetContractorList();
            ViewData["AreaList"] = ListCommonHelper.GetAreaGrouped();
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            
            return View();
        }

        public JObject GetTableJson(FormCollection form = null)
        {
            var contractorRateList = ContractorRateHelper.GetContractorRates(form);
            return contractorRateList;
        }

        public ActionResult GetSearchResult(FormCollection form)
        {
            var contractorRateList = ContractorRateHelper.GetContractorRatesInquiry(form);
            return PartialView("~/Views/ContractorRateInquiry/_ResultTable.cshtml", contractorRateList);
        }

        public ActionResult GetRatesForArea(int fromCityId, int toCityId)
        {
            var contractorRateList = ContractorRateHelper.GetContractorRatesForArea(fromCityId,toCityId);
            return PartialView("~/Views/ContractorRateInquiry/_ContractorRateSummary.cshtml", contractorRateList);
        }

    }
}
