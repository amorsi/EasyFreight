using EasyFreight.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.ViewModel;

namespace EasyFreight.Controllers
{
    public class AccountingRptController : Controller
    {
        //
        // GET: /AccountingRpt/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AccTransByAccId(string accId)
        {
            ViewBag.AccountId = accId;
            var accSummary = AccountingRptHelper.GetAccSummaryByAccId(accId);
            return View(accSummary);
        }

        public JObject GetAccTransJson(string accId, string fromDate = null, string toDate = null)
        {
            var tbResult = AccountingRptHelper.GetAccTransByAccId(accId, fromDate, toDate);
            return tbResult;
        }

        public ActionResult GetAccTotalPartial(string accountId, string fromDate = null, string toDate = null)
        {
            //Fill Account Summary 
            var accSummary = AccountingRptHelper.GetAccSummaryByAccId(accountId, fromDate, toDate);
            return PartialView("~/Views/AccountingRpt/_TotalsSummaryTb.cshtml", accSummary);
        }


        public ActionResult AccSummaryByParentAccId(string parentAccId, bool isCreditAccount = false, int bId = 0)
        {
            ViewBag.ParentAccId = parentAccId;
            ViewBag.IsCreditAccount = isCreditAccount;
            ViewBag.bId = bId;

            ViewBag.AccNameHeader = "Account Receivables";
            ViewBag.AccountType = "Debit";

            if (isCreditAccount)
            {
                ViewBag.AccNameHeader = "Account Payable";
                ViewBag.AccountType = "Credit";

            }


            //Fill Account Summary 
            var accountSummary = AccountingRptHelper.GetAccountSummaryGrouped(parentAccId, isCreditAccount, bId:bId);

            return View(accountSummary);
        }

        public JObject GetAccSummaryJson(string parentAccId, bool isCreditAccount = false,
            DateTime? fromDate = null, DateTime? toDate = null, int bId = 0)
        {
            var tbResult = AccountingRptHelper.GetAccSummaryByParentAccIdJObj(parentAccId, isCreditAccount, fromDate, toDate, bId);
            return tbResult;
        }

        public ActionResult GetAccSummaryTotalPartial(string parentAccId, bool isCreditAccount = false, DateTime? fromDate = null, DateTime? toDate = null)
        {
            //Fill Account Summary 
            var accountSummary = AccountingRptHelper.GetAccountSummaryGrouped(parentAccId, isCreditAccount, fromDate, toDate);
            return PartialView("~/Views/AccountingRpt/_TotalsSummaryTb.cshtml", accountSummary);
        }





    }
}