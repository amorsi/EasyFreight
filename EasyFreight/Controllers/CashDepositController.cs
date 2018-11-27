using EasyFreight.DAL;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.Models;

namespace EasyFreight.Controllers
{
    public class CashDepositController : Controller
    {
        //
        // GET: /CashDeposit/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JObject GetTableJson()
        {
            JObject quotationOrders = CashDepositHelper.GetCashReceiptsJson();
            return quotationOrders;
        }

        public PartialViewResult GetCCCashDepositList(int operId)
        {
            var ccCashDepList = CashOutHelper.GetCCCashDepositList(operId);

            if (ccCashDepList.Count > 0)
            {
                Dictionary<string, decimal> DepositTotals = new Dictionary<string, decimal>();

                var DepositTotalsObj = ccCashDepList.GroupBy(x => x.CurrencySign)
                  .Select(x => new { curr = x.Key, total = x.Sum(y => y.ReceiptAmount.Value) }).ToList();

                //Get any collcted back deposits
                var depositSattl = CashHelper.GetCashSettlementForOper(operId);
                decimal depositAmout = 0;
                if (depositSattl != null)
                {
                    foreach (var item in DepositTotalsObj)
                    {
                        if (depositSattl.Keys.Contains(item.curr))
                            depositAmout = item.total - depositSattl[item.curr];
                        else
                            depositAmout = item.total;

                        DepositTotals.Add(item.curr, depositAmout);
                    }
                }

                ViewBag.DepositTotal = DepositTotals;
                //ViewBag.Currency = ccCashDepList.FirstOrDefault().CurrencySign;

            }
            else
            {
                ViewBag.DepositTotal = 0;
                ViewBag.Currency = "EGP";
            }
            //Get AP invoices
            var apInvList = APInvoiceHelper.GetInvoiceListForOper(operId);

            if (apInvList.Count > 0)
            {
                var currList = ListCommonHelper.GetCurrencyList();
                var apInvTotals = apInvList.Where(x => x.InvoiceType == 3).GroupBy(x => x.InvCurrencyId)
                .Select(x => new { curr = x.Key, total = x.Sum(y => y.InvoiceDetails.Sum(c => c.InvoiceAmount)) });

                Dictionary<string, decimal> apInvTotalsDic = new Dictionary<string, decimal>();
                string currSign = "";

                foreach (var item in apInvTotals)
                {
                    currSign = currList.Where(x => x.Key == item.curr).FirstOrDefault().Value;
                    apInvTotalsDic.Add(currSign, item.total);
                }

                ViewBag.CCInvTotal = apInvTotalsDic;
            }
            else
            {
                ViewBag.CCInvTotal = 0;
            }

            ViewBag.OperId = operId;

            var cashSettlement = CashHelper.GetCashSettlementForOper(operId);

            // if(cashSettlement.Count > 0)
            ViewBag.CashSettlement = cashSettlement;


            return PartialView("~/Views/CashDeposit/_CCCashDepositList.cshtml", ccCashDepList);
        }

        public ActionResult ArCashDeposit()
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en")
               .Where(x => x.Key == 1 || x.Key == 3) //Cash Or bank only
               .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();

            var cashReceiptObj = CashHelper.GetCashReceiptInfo(0, 0);
            cashReceiptObj.Notes = "Cash Deposit From Client";
            return View(cashReceiptObj);
        }

        [HttpPost]
        public ActionResult AddArCashDeposit(CashInVm cashInVm)
        {
            string isSaved = CashDepositHelper.AddArCashDeposit(cashInVm);
            return Json(isSaved);
        }


        public ActionResult ViewCashInPartial(int id)
        {
            Session["receiptId"] = id;
            CashInVm invObj = CashDepositHelper.GetCashReceiptInfo(id);
            invObj.CashType = "cashin";
            return PartialView("~/Views/CashManagement/_ViewCashInReceipt.cshtml", invObj);
        }

        [AllowAnonymous]
        public ActionResult PrintCashInV(int id)
        {
            CashInVm invObj = CashDepositHelper.GetCashReceiptInfo(id);
            invObj.CashType = "cashin";
            return View(invObj);
           // return View("~/Views/CashManagement/PrintCashInV.cshtml", invObj);
        }


        //[CheckRights(ScreenEnum.CashManagment, ActionEnum.Print)]
        public ActionResult PrintCashIn(int id = 0)
        {
            if (id == 0) //Print from view modal .. get ids from session
                id = (int)Session["receiptId"];

            return new ActionAsPdf("PrintCashInV", new { id = id, name = "Giorgio" }) { FileName = "CashDepositReceipt_" + id.ToString() + ".pdf" };
        }

        public ActionResult DeleteCashDeposit(int receiptId, string deleteReason)
        {

            string isDeleted = CashDepositHelper.DeleteCashDeposit(receiptId, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }


    }
}