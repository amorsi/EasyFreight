using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;

namespace EasyFreight.Controllers
{
    public class CheckManagementController : Controller
    {
        // GET: CheckManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARChecks()
        {
            ViewBag.ColumnHeader = "Customer Name";
            return View();
        }

        public JObject GetARChecksJson(FormCollection form)
        {
            JObject aRChecks = CheckManageHelper.GetARChecksListJson(form);
            return aRChecks;
        }

        public ActionResult CollectARCheck(int receiptId)
        {
            string isCollected = CheckManageHelper.CollectCheck(receiptId);
            return Json(isCollected);
        }

        public ActionResult APChecks()
        {
            ViewBag.ColumnHeader = "Supplier Name";
            return View();
        }

        public JObject GetAPChecksJson(FormCollection form)
        {
            JObject aRChecks = CheckManageHelper.GetAPChecksListJson(form);
            return aRChecks;
        }

        public ActionResult PayAPCheck(int receiptId)
        {
            string isPaid = CheckManageHelper.PayCheck(receiptId);
            return Json(isPaid);
        }



    }
}