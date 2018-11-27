using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using Rotativa;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class CashOutExpenseController : Controller
    {
        // GET: CashOutExpense
        [CheckRights(ScreenEnum.ManageExpenses, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form)
        {
            JObject quotationOrders = CashOutExpenseHelper.GetExpensesCashOutJson(form);
            return quotationOrders;
        }

        public ActionResult GetExpensesForReceipt(int receiptId)
        {
            var expensesList = CashOutExpenseHelper.GetExpensesListForReceipt(receiptId);
            return PartialView("~/Views/CashOutExpense/_ExpenseListTb.cshtml", expensesList);
        }

        [CheckRights(ScreenEnum.ManageExpenses, ActionEnum.Add)]
        public ActionResult Add(int receiptId = 0)
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en")
             .Where(x => x.Key == 1 )
             .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.BankList = ListCommonHelper.GetBankList();
            var cashOutObj = CashOutExpenseHelper.GetCashReceiptForExpense(receiptId);

            ViewBag.ExpenseNameList = ListCommonHelper.GetExpensesLibraryList();
            ViewBag.ReceiptId = receiptId;

            return View(cashOutObj);
        }

        public ActionResult AddEditCashReceipt(CashInVm cashObj)
        {
            string isSaved = CashOutExpenseHelper.AddEditExpenseCashReceipt(cashObj);
            return Json(isSaved);
        }

        /// <summary>
        /// For view receipt with expenses in pop-up
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns>HTML partial view</returns>
        public ActionResult ViewReceipt(int receiptId)
        {
            Session["receiptId"] = receiptId;
            var expensesList = CashOutExpenseHelper.GetCashReceiptForExpense(receiptId);
            return PartialView("~/Views/CashOutExpense/_ViewReceipt.cshtml", expensesList);
        }

        [AllowAnonymous]
        public ActionResult PrintReceiptV(int receiptId)
        {
            var expensesList = CashOutExpenseHelper.GetCashReceiptForExpense(receiptId);
            return View(expensesList);
        }

        [CheckRights(ScreenEnum.ManageExpenses, ActionEnum.Print)]
        public ActionResult PrintReceipt(int receiptId = 0)
        {
            if (receiptId == 0)
                receiptId = (int)Session["receiptId"];
            var expensesList = CashOutExpenseHelper.GetCashReceiptForExpense(receiptId);
            return new ActionAsPdf("PrintReceiptV", new { receiptId = receiptId, name = "Giorgio" }) { FileName = "CashReceipt_" + receiptId.ToString() + ".pdf" };
        }

    }
}