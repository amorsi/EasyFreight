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
    public class InvoiceController : Controller
    {
        //
        // GET: /Invoice/
        [CheckRights(ScreenEnum.InvoiceList, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form)
        {
            JObject quotationOrders = InvoiceHelper.GetInvListJson(form);
            return quotationOrders;
        }

        public ActionResult ViewInvoice(int id, int hbId)
        {
            var invObj = InvoiceHelper.GetInvoiceInfoNew(hbId, id, false);
            return View(invObj);
        }

        public ActionResult ViewInvoicePartial(int id, int hbId)
        {
            Session["invId"] = id;
            Session["hbId"] = hbId;
            var invObj = InvoiceHelper.GetInvoiceInfoNew(hbId, id, false);
            return PartialView("~/Views/Invoice/_ViewInvoice.cshtml", invObj);
        }

        [CheckRights(ScreenEnum.InvoiceList, ActionEnum.Add)]
        public ActionResult AddInvoice(int hbId, int invId = 0)
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en").Where(x => x.Key < 6).ToList();
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            var invObj = InvoiceHelper.GetInvoiceInfoNew(hbId, invId);
            return View(invObj);
        }

        public ActionResult AddEditInvoice(InvoiceVm invoiceVm)
        {
            string isSaved = InvoiceHelper.AddEditInvoice(invoiceVm);
            return Json(isSaved);
        }

        [AllowAnonymous]
        public ActionResult PrintInvoiceV(int id,int hbId)
        {
            var invObj = InvoiceHelper.GetInvoiceInfoNew(hbId, id, false);
            ///return new Rotativa.ViewAsPdf("PrintInvoiceV", invObj) { FileName = "TestViewAsPdf.pdf" };
            return View(invObj);
        }

        [CheckRights(ScreenEnum.InvoiceList, ActionEnum.Print)]
        public ActionResult PrintInvoice(int id = 0, int hbId =0)
        {
            if (id == 0) //Print from view modal .. get ids from session
            {
                id = (int)Session["invId"];
                hbId = (int)Session["hbId"];
            }
            return new ActionAsPdf("PrintInvoiceV", new { id = id, hbId = hbId, name = "Giorgio" }) { FileName = "Invoice.pdf" };
        }


        public ActionResult Delete(int invId, string deleteReason)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.InvoiceList, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action", JsonRequestBehavior.AllowGet);

            #endregion

            string isDeleted = InvoiceHelper.Delete(invId, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvSearchInv()
        {
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            return PartialView("~/Views/Invoice/_AdvSearchInv.cshtml");
        }

    }
}