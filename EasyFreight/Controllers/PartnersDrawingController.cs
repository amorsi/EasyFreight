using EasyFreight.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Controllers
{
    public class PartnersDrawingController : Controller
    {
        // GET: PartnersDrawing
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form)
        {
            JObject quotationOrders = PartnersDrawingHelper.GetPartnersDrawingJson(form);
            return quotationOrders;
        }

        public ActionResult AddCashReceipt(string type)
        {
            if (type.ToLower() == "in")
                ViewBag.CashRecTitle = "Cash In Receipt";
            else if (type.ToLower() == "out")
                ViewBag.CashRecTitle = "Cash out Receipt";
            else
                return RedirectToAction("Index");

            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en").Where(x => x.Key < 6).ToList(); 

            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();
            ViewBag.PartnersList = ListCommonHelper.GetPartnersList();
            var cashVm = PartnersDrawingHelper.GetCashReceiptForPartners(type);
            return View(cashVm);
        }

        public ActionResult AddEditCashReceipt(ViewModel.CashInVm cashObj)
        {
            string isSaved = "false";
            int receiptId;
            if (cashObj.CashType == "cashin")
                 isSaved = CashHelper.AddEditCashReceipt(cashObj,out receiptId);
            else
                isSaved = CashOutHelper.AddEditCashReceipt(cashObj, out receiptId);

            return Json(isSaved + "_" + receiptId);
        }
    }
}