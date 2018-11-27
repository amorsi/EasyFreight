using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.ViewModel;
using Rotativa;
using Newtonsoft.Json.Linq;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class CashManagementController : Controller
    {
        //
        // GET: /CashManagement/
        [CheckRights(ScreenEnum.CashManagment, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form, string cashType = "cashin")
        {
            JObject quotationOrders = CashHelper.GetCashReceiptsJson(form, cashType);
            return quotationOrders;
        }

        [CheckRights(ScreenEnum.CashManagment, ActionEnum.Add)]
        public ActionResult CashInReceipt(int invId, int agNoteId = 0)
        {
            ViewBag.CashRecTitle = "Cash In Receipt";
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en", true)
                .Where(x => x.Key != 2 && x.Key < 6)
                .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();
            var cashReceiptObj = CashHelper.GetCashReceiptInfo(invId, agNoteId);
            return View(cashReceiptObj);
        }

        public ActionResult AddEditCashReceipt(CashInVm cashVmObj)
        {
            string isSaved = "false";
            int receiptId;
            if (cashVmObj.CashType == "cashin")
                isSaved = CashHelper.AddEditCashReceipt(cashVmObj, out receiptId);
            else //Cash Out Receipt or CC cash deposit
                isSaved = CashOutHelper.AddEditCashReceipt(cashVmObj, out receiptId);


            return Json(isSaved + "_" + receiptId);
        }

        public ActionResult GetCashReceiptsForInv(int invId, bool isOut = false)
        {
            List<CashInInvoiceVm> cashReceiptList;
            if (isOut)
                cashReceiptList = CashOutHelper.GetCashInvList(invId);
            else
                cashReceiptList = CashHelper.GetCashInvList(invId);

            return PartialView("~/Views/CashManagement/_CashReceiptsForInv.cshtml", cashReceiptList);
        }

        public ActionResult GetCashReceiptsForNote(int agNoteId, bool isOut = false)
        {
            List<CashInInvoiceVm> cashReceiptList;
            if (isOut)
                cashReceiptList = CashOutHelper.GetCashAgNoteList(agNoteId);
            else
                cashReceiptList = CashHelper.GetCashAgNoteList(agNoteId);

            return PartialView("~/Views/CashManagement/_CashReceiptsForInv.cshtml", cashReceiptList);
        }

        public ActionResult ViewCashInPartial(int id, bool isOut = false)
        {
            System.Globalization.CultureInfo clut = new System.Globalization.CultureInfo("ar-EG");

            Session["receiptId"] = id;
            Session["CashType"] = isOut;
            CashInVm invObj;
            if (isOut)
                invObj = CashOutHelper.GetCashReceiptInfo(0, 0, id);
            else
                invObj = CashHelper.GetCashReceiptInfo(0, 0, id);
            return PartialView("~/Views/CashManagement/_ViewCashInReceipt.cshtml", invObj);
        }

        [AllowAnonymous]
        public ActionResult PrintCashInV(int id, bool isOut)
        {
            CashInVm invObj;
            if (isOut)
                invObj = CashOutHelper.GetCashReceiptInfo(0, 0, id);
            else
                invObj = CashHelper.GetCashReceiptInfo(0, 0, id);
            return View( invObj);
        }

        [CheckRights(ScreenEnum.CashManagment, ActionEnum.Print)]
        public ActionResult PrintCashIn(int id = 0, bool isOut = false)
        {
            if (id == 0) //Print from view modal .. get ids from session
            {
                id = (int)Session["receiptId"];
                isOut = (bool)Session["CashType"];
            }
            return new ActionAsPdf("PrintCashInV", new { id = id, isOut = isOut, name = "Giorgio" }) { FileName = "CashReceipt_" + id.ToString() + ".pdf" };
        }

        [CheckRights(ScreenEnum.CashManagment, ActionEnum.Add)]
        public ActionResult CashOutReceipt(int invId, int agNoteId = 0)
        {
            ViewBag.CashRecTitle = "Cash Out Receipt";
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en")
                .Where(x => x.Key != 2 && x.Key < 6)
                .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();
            var cashReceiptObj = CashOutHelper.GetCashReceiptInfo(invId, agNoteId);
            return View("~/Views/CashManagement/CashInReceipt.cshtml", cashReceiptObj);
        }

        [CheckRights(ScreenEnum.CashManagment, ActionEnum.Add)]
        public ActionResult CCCashDeposit(int operationId, decimal receiptAmount = 0)
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en")
                .Where(x => x.Key == 1)
                .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();
            var cashReceiptObj = CashOutHelper.GetCashReceiptInfo(0, 0, 0, operationId, receiptAmount);
            ViewBag.CashRecTitle = "Custom Clearance Cash Deposit For Operation " + cashReceiptObj.OperationCode;

            return View("~/Views/CashManagement/CashInReceipt.cshtml", cashReceiptObj);

        }

        [CheckRights(ScreenEnum.CashManagment, ActionEnum.Add)]
        public ActionResult CCCashSettlement(int operationId, decimal receiptAmount = 0)
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en")
                .Where(x => x.Key == 1)
                .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();

            CashInVm cashReceiptObj = CashHelper.GetCashReceiptInfoForSettlement(operationId, receiptAmount);

            ViewBag.CashRecTitle = "Custom Clearance Cash Settlement (In) For Operation " + cashReceiptObj.OperationCode;

            return View("~/Views/CashManagement/CashInReceipt.cshtml", cashReceiptObj);
        }


        public ActionResult DeleteCashIn(int receiptId, int invId, string deleteReason)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.CashManagment, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action", JsonRequestBehavior.AllowGet);

            #endregion

            string isDeleted = CashHelper.DeleteCashInReceipt(receiptId, invId, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteCashOut(int receiptId, int invId, string deleteReason)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.CashManagment, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action", JsonRequestBehavior.AllowGet);

            #endregion

            string isDeleted = CashOutHelper.DeleteCashOutReceipt(receiptId, invId, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCashOutReceipt(int receiptId,  string deleteReason)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.CashManagment, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action", JsonRequestBehavior.AllowGet);

            #endregion

            string isDeleted = CashOutHelper.DeleteCashOutReceipt(receiptId, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult AdvSearch()
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm();
            ViewBag.Currency = ListCommonHelper.GetCurrencyList();
            return PartialView("~/Views/CashManagement/_AdvSearch.cshtml");
        }


        [CheckRights(ScreenEnum.CashManagment, ActionEnum.Add)]
        public ActionResult CashOpenBalance(string accid, int cid )
        {
            
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en", true)
                .Where(x => x.Key != 2 && x.Key < 6)
                .ToDictionary(x => x.Key, x => x.Value);

            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();

            var cashReceiptObj = CashHelper.GetCashReceiptOpenBalance(accid, cid);
             
            var openBalanceObject = AccountingHelper.Get_OpenBalanceObject(accid, cid); 
            ViewBag.OpenBalanceObject = openBalanceObject;
            
            ViewBag.CashRecTitle = openBalanceObject.Amount < 0 ? "Cash Out Receipt" : "Cash In Receipt";

            return View(cashReceiptObj);
        }


        public ActionResult AddEditOpenCashReceipt(CashInVm cashVmObj)
        {
            string isSaved = "false";
            int receiptId;
            if (cashVmObj.CashType == "cashin")
                isSaved = CashHelper.AddEditOpenCashReceipt(cashVmObj, out receiptId);
            else //Cash Out Receipt 
                isSaved = CashOutHelper.AddEditOpenCashReceipt(cashVmObj, out receiptId);


            return Json(isSaved + "_" + receiptId);
        }

    }
}