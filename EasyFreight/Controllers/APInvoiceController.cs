using EasyFreight.DAL;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class APInvoiceController : Controller
    {
        //
        // GET: /APInvoice/
        [CheckRights(ScreenEnum.APInvoice, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form)
        {
            JObject quotationOrders = APInvoiceHelper.GetInvListJson(form);
            return quotationOrders;
        }

        [CheckRights(ScreenEnum.APInvoice, ActionEnum.Add)]
        public ActionResult AddInvoice(int hbId, int operId = 0, int invId = 0, byte invFor = 1)
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en").Where(x => x.Key < 6).ToList(); 
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            InvoiceVm invObj;
            if (operId == 0)
                invObj = APInvoiceHelper.GetInvoiceInfo(hbId, invId, invFor); // old code .. will not happen
            else
                invObj = APInvoiceHelper.GetInvoiceInfoFullOperation(operId, invId, invFor);
            return View(invObj);
        }

        public ActionResult AddEditInvoice(InvoiceVm invoiceVm)
        {
            string isSaved = APInvoiceHelper.AddEditInvoice(invoiceVm);
            return Json(isSaved);
        }

        public ActionResult ViewInvoicePartial(int id, int hbId, byte invFor, int operId)
        {
            Session["invId"] = id;
            Session["hbId"] = hbId;
            Session["invFor"] = invFor;
            Session["operId"] = operId;

            InvoiceVm invObj;
            if (operId == 0)
                invObj = APInvoiceHelper.GetInvoiceInfo(hbId, id, invFor, false); // old code .. will not happen
            else
                invObj = APInvoiceHelper.GetInvoiceInfoFullOperation(operId, id, invFor, false);

            return PartialView("~/Views/Invoice/_ViewInvoice.cshtml", invObj);
        }

        [AllowAnonymous]
        public ActionResult PrintInvoiceV(int id, int hbId, byte invFor, int operId)
        {
            InvoiceVm invObj;
            if (operId == 0)
                invObj = APInvoiceHelper.GetInvoiceInfo(hbId, id, invFor, false); // old code .. will not happen
            else
                invObj = APInvoiceHelper.GetInvoiceInfoFullOperation(operId, id, invFor, false);

            return View("~/Views/Invoice/PrintInvoiceV.cshtml", invObj);
        }

        [CheckRights(ScreenEnum.APInvoice, ActionEnum.Print)]
        public ActionResult PrintInvoice(int id = 0, int hbId = 0, byte invFor = 0, int operId = 0)
        {
            if (id == 0) //Print from view modal .. get ids from session
            {
                id = (int)Session["invId"];
                hbId = (int)Session["hbId"];
                invFor = (byte)Session["invFor"];
                operId = (int)Session["operId"];
            }
            return new ActionAsPdf("PrintInvoiceV", new { id = id, hbId = hbId, invFor = invFor, operId = operId, name = "Giorgio" }) { FileName = "Invoice" + id.ToString() + ".pdf" };
        }

        public ActionResult ViewInvoice(int id, int hbId, byte invFor, int operId = 0)
        {
            InvoiceVm invObj = APInvoiceHelper.GetInvoiceInfoFullOperation(operId, id, invFor, false);
            return View("~/Views/Invoice/ViewInvoice.cshtml", invObj);
        }

        public ActionResult Delete(int invId, string deleteReason)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.APInvoice, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action", JsonRequestBehavior.AllowGet);

            #endregion

            string isDeleted = APInvoiceHelper.Delete(invId, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvSearch()
        {
            ViewBag.CarrierList = ListCommonHelper.GetCarrierList();
            ViewBag.ContractorList = ListCommonHelper.GetContractorList();
            return PartialView("~/Views/APInvoice/_AdvSearch.cshtml");
        }

    }
}