using EasyFreight.DAL;
using EasyFreight.Areas.Admin.DAL;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using Rotativa;
using System.Linq;
using System.Web.Mvc;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class QuotationController : Controller
    {
        //
        // GET: /Quotation/

        public ActionResult Index(byte orderFrom = 1)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportQuotation, ActionEnum.ViewAll);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportQuotation, ActionEnum.ViewAll);

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home");
            #endregion

            ViewBag.OrderFrom = orderFrom;
            ViewBag.OrderFromText = orderFrom == 1 ? "Export" : "Import";
            return View();
        }

        public JObject GetTableJson(FormCollection form = null)
        {
            var quotationOrders = QuotationHelper.GetQuotationsOrders(form);
            return quotationOrders;
        }

        public ActionResult Add(int id = 0, byte orderFrom = 1)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
            {
                if(id == 0)
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportQuotation, ActionEnum.Add);
                else
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportQuotation, ActionEnum.Edit);
            }
            else
            {
                if (id == 0)
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportQuotation, ActionEnum.Add);
                else
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportQuotation, ActionEnum.Edit);
            }


            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home");
            #endregion

            QuotationVm quotationsVm = QuotationHelper.GetQuotationInfo(id, orderFrom);

            ViewBag.CarrierList = ListCommonHelper.GetCarrierList("en", 1);
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewBag.NotifierList = ListCommonHelper.GetNotifierList(0);
            ViewBag.IncotermLib = ListCommonHelper.GetIncotermLibList();
            ViewBag.Containers = ListCommonHelper.GetContainerList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped("en", 1);
            ViewData["CurrencyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.AgentList = ListCommonHelper.GetAgentList();

            return View(quotationsVm);
        }

        public ActionResult GetNotifierList(int consigneeId)
        {
            var notifierList = ListCommonHelper.GetNotifierList(consigneeId, "en");
            return Json(notifierList.Select(x => new { Id = x.Key.ToString(), Value = x.Value })
                .OrderBy(x => x.Value).ToList());
        }

        public ActionResult GetVesselList(int carrierId)
        {
            var vesselList = ListCommonHelper.GetVesselLis(carrierId);
            return Json(vesselList.Select(x => new { Id = x.Key.ToString(), Value = x.Value })
                .OrderBy(x => x.Value).ToList());
        }

        public ActionResult AddEditQuotation(QuotationVm quotationVm)
        {
            string isSaved = QuotationHelper.AddEditQuotation(quotationVm);
            return Json(isSaved);
        }

        public ActionResult ChangeStatus(int quotationId, byte statusId)
        {
            string isSaved = QuotationHelper.ChangeStatus(quotationId, statusId);
            return Json(isSaved);
        }

        public ActionResult GetQuotationDetails(int id)
        {
            EasyFreight.Models.QuotationView quoteView = QuotationHelper.GetOneQuote(id);
            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.QuoteContainers = QuotationHelper.GetQuotationContainers(id);
            return PartialView("~/Views/Quotation/_MoreDetails.cshtml", quoteView);
        }

        public ActionResult GetShippingDeclaration(int id)
        {
            EasyFreight.Models.QuotationView quoteView = QuotationHelper.GetOneQuote(id);
            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.QuoteContainers = QuotationHelper.GetQuotationContainers(id);
            ViewData["StaticText"] = CommonHelper.GetStaticTextById(1);
            Session["quoteId"] = id; 
            if (quoteView.IsCareOf)
            {
                CompanySetupVm company = CompanySetupHelper.GetCompanySetup();
                ViewBag.ShipperAddress = company.CompanyAddressEn;
                ViewBag.ShipperTel  = (!string.IsNullOrEmpty(company.PhoneNumber1) ? " Tel. " + company.PhoneNumber1 : "") + 
                    (!string.IsNullOrEmpty(company.FaxNumber) ? "  Fax. " + company.FaxNumber : "");
            }
            else
            {

                ViewBag.ShipperAddress = quoteView.ShipperAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(quoteView.ShipperPhoneNumber) ? " Tel. " + quoteView.ShipperPhoneNumber : "") +
                    (!string.IsNullOrEmpty(quoteView.ShipperFaxNumber) ? " Fax. " + quoteView.ShipperPhoneNumber : "");  
            } 
            return PartialView("~/Views/Quotation/_ShippingDecl.cshtml", quoteView);
        }

        [AllowAnonymous]
        public ActionResult PrintShippingDeclV(int id)
        {
            EasyFreight.Models.QuotationView quoteView = QuotationHelper.GetOneQuote(id);
            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.QuoteContainers = QuotationHelper.GetQuotationContainers(id);
            ViewData["StaticText"] = CommonHelper.GetStaticTextById(1);

            if (quoteView.IsCareOf)
            {
                CompanySetupVm company = CompanySetupHelper.GetCompanySetup();
                ViewBag.ShipperAddress = company.CompanyAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(company.PhoneNumber1) ? " Tel. " + company.PhoneNumber1 : "") +
                    (!string.IsNullOrEmpty(company.FaxNumber) ? "  Fax. " + company.FaxNumber : "");
            }
            else
            {
                ViewBag.ShipperAddress = quoteView.ShipperAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(quoteView.ShipperPhoneNumber) ? " Tel. " + quoteView.ShipperPhoneNumber : "") +
                    (!string.IsNullOrEmpty(quoteView.ShipperFaxNumber) ? " Fax. " + quoteView.ShipperPhoneNumber : "");
            } 

            return View(quoteView);
        }

        public ActionResult PrintShippingDecl()
        {
            int quoteId = (int)Session["quoteId"];
            return new ActionAsPdf("PrintShippingDeclV", new { id = quoteId, name = "Giorgio" }) { FileName = "ShippingDeclaration.pdf" };
        }

        public ActionResult Cancel(int quotationId, byte orderFrom = 1)
        {

            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportQuotation, ActionEnum.Delete);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportQuotation, ActionEnum.Delete);

            if (!hasRights)
                return Json("You are UnAuthorized to do this action");
            #endregion

            string isChanged = QuotationHelper.ChangeStatus(quotationId, 4);
            return Json(isChanged);//, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvSearch()
        {
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewBag.CarrierList = ListCommonHelper.GetCarrierList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            return PartialView("~/Views/Quotation/_AdvSearch.cshtml");
        }
    }
}
