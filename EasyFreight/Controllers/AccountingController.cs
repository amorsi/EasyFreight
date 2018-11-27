using EasyFreight.DAL;
using Newtonsoft.Json.Linq;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.ViewModel;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        //
        // GET: /Accounting/
        [CheckRights(ScreenEnum.AccOperationsList, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form)
        {
            var quotationOrders = OperationHelper.GetOperationOrders(form);
            return quotationOrders;
        }

        public ActionResult Summary()
        {
            Dictionary<string, decimal> TopViewTotals = new Dictionary<string, decimal>();
            var accountSummary = AccountingRptHelper.FillAccountSummaryMain(out TopViewTotals);
            ViewBag.TopViewTotals = TopViewTotals;
            return View(accountSummary);
        }

        /// <summary>
        /// Expand table row.. Get HB for operation
        /// </summary>
        /// <param name="operationId">operation id</param>
        /// <param name="orderFrom">1 export or 2 import</param>
        /// <returns></returns>
        public ActionResult GetHbList(int operationId, byte orderFrom)
        {
            var hbList = HouseBillHelper.GetHBList(operationId, orderFrom, true);
            return PartialView("~/Views/Accounting/_HouseBillList.cshtml", hbList);
        }

        public ActionResult GetHbInvList(int hbId)
        {
            var invList = InvoiceHelper.GetInvListForHb(hbId);
            return PartialView("~/Views/Accounting/_InvoiceList.cshtml", invList);
        }

        public ActionResult GetOperationFullCost(int operationId, int hbId)
        {
            var operCostMain = AccountingHelper.GetOperationCost(operationId, hbId);
            return PartialView("~/Views/Accounting/_OperationCostMain.cshtml", operCostMain);
        }

        [AllowAnonymous]
        public ActionResult PrintOperationFullCostV(int operationId, int hbId)
        {
            var operCostMain = AccountingHelper.GetOperationCost(operationId,hbId);
            return View(operCostMain);
        }

        public ActionResult PrintOperCostDetails(int operationId = 0, int hbId = 0)
        {
            return new ActionAsPdf("PrintOperationFullCostV", new { operationId = operationId, hbId = hbId, name = "Giorgio" }) { FileName = "OperationCostAnalysis.pdf" };
        }

        /// <summary>
        /// Get Invoice for add new 
        /// </summary>
        /// <param name="id">House Bill Id</param>
        /// <param name="oId">Operation Id</param>
        /// <param name="iId">Invoice Id</param>
        /// <param name="iId">Invoice Type 0 all currencies .. 1 EGP .. 2 Other currencies</param>
        /// <returns></returns>
        public ActionResult Invoice(int id = 0, int oId = 0, int iId = 0, byte invType = 0)
        {
            ViewBag.PaymentTerm = ListCommonHelper.GetPaymentTerm("en").Where(x => x.Key < 6).ToList(); 
            var invObj = InvoiceHelper.GetInvoiceInfo(id, oId, iId, invType);
            return View(invObj);
        }

        public ActionResult AddEditInvoice(InvoiceVm invoiceVm)
        {
            string isSaved = InvoiceHelper.AddEditInvoice(invoiceVm);

            return Json(isSaved);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <param name="orderFrom"></param>
        /// <returns></returns>
        [CheckRights(ScreenEnum.AccOperationsList, ActionEnum.ManageOrder)]
        public ActionResult ManageOrder(int id, byte orderFrom)
        {
            if (orderFrom == 1)
                ViewBag.OperationType = "Export";
            else
                ViewBag.OperationType = "Import";

            ViewBag.OperationId = id;

            var invList = InvoiceHelper.GetInvoiceListForOper(id);
            
            var hbList = HouseBillHelper.GetHBList(id, orderFrom, true);
            ViewBag.OperationCode = hbList.FirstOrDefault().OperationCode;
            ViewBag.HbList = hbList;
            

            return View(invList);
        }

        public ActionResult GetAgNoteTab(int operId)
        {
            var agNoteList = AgentNoteHelper.GetAgentNoteList(operId);
            return PartialView("~/Views/Accounting/_AgentNoteList.cshtml", agNoteList);
        }

        public ActionResult GetAPInvoiceTab(int operId)
        {
            var agNoteList = APInvoiceHelper.GetInvoiceListForOper(operId);
            return PartialView("~/Views/Accounting/_APInvListForOper.cshtml", agNoteList);
        }

        public ActionResult AdvSearchOperations()
        {

            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewBag.CarrierList = ListCommonHelper.GetCarrierList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            return PartialView("~/Views/Accounting/_AdvSearchOperations.cshtml");
        }


        public ActionResult AddOperationCost(int hbId )
        {
            ViewBag.OperationCostNameList = ListCommonHelper.GetOperationCostList();
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();

            ViewBag.HbObj = HouseBillHelper.GetHbContent(hbId);
           // Session["hbOneObj"] = ((List<HouseBillListVm>)TempData["hbList"]).Where(x => x.HouseBillId == hbId).FirstOrDefault();

            OperationCostMainVm hbCostAdd = HouseBillHelper.GetHBCost(hbId);
            return View(hbCostAdd);
         }


        [CheckRights(ScreenEnum.OpenBalance, ActionEnum.ViewAll)]
        public ActionResult OpenBalance()
        {
            return View();
        }

        public JObject GetOpenBalanceTableJson( )
        {
            JObject quotationOrders = AccountingHelper.GetOpenBalanceListJson();
            return quotationOrders;
        }
    
    
    
    
    }
}