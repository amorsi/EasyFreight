using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;
using EasyFreight.ViewModel;
using Rotativa;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class CustomClearanceController : Controller
    {
        //
        // GET: /CustomClearance/
        [CheckRights(ScreenEnum.CustomClearanceOrders, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form = null)
        {
            var custClearOrders = CustomClearanceHelper.GetCCAllOrders(form);
            return custClearOrders;
        }


        [CheckRights(ScreenEnum.CustomClearanceOrders, ActionEnum.ViewAll)]
        public ActionResult Summary()
        {
            return View();
        }

        [CheckRights(ScreenEnum.CustomClearanceOrders, ActionEnum.ManageOrder)]
        public ActionResult ManageOrder(int id)
        {
            ViewBag.customClearObj = CustomClearanceHelper.GetOneCustClearView(id);
            CustomClearanceDetailMainVm custClearDetMain = CustomClearanceHelper.GetCustClearDetailList(id);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.CCCostNameList = ListCommonHelper.GetCustClearCostList();
            ViewBag.ccId = id;
            return View(custClearDetMain);
        }

        /// <summary>
        /// operation employee add custom clearance order from process operation order  
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <returns>add custom clearance order patial view</returns>
        public ActionResult CustomClearanceOrder(int id, int houseBillId, byte orderFrom)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportHB, ActionEnum.AddEditCCOrder);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportHB, ActionEnum.AddEditCCOrder);
            }

            if (!hasRights)
                return PartialView("~/Views/Shared/_UnAuthorized.cshtml");

            #endregion

            ViewBag.customClearObj = CustomClearanceHelper.GetCustomClearance(id, houseBillId);
            EasyFreight.Models.HouseBillView operationView = HouseBillHelper.GetHBView(houseBillId);
            ViewBag.ContainerSummary = OperationHelper.GetContainersSummary(id);
            return PartialView("~/Views/CustomClearance/_CustomClearance.cshtml", operationView);
        }

        /// <summary>
        /// Add and edit custom clearance order from operation process
        /// </summary>
        /// <param name="custClearVm">Custom Clearance Vm</param>
        /// <returns>true if succsed else false + error message</returns>
        [ValidateAntiForgeryToken]
        public ActionResult AddEditCustClear(CustomClearVm custClearVm)
        {
            string isSaved = CustomClearanceHelper.AddEditCustClear(custClearVm);
            return Json(isSaved);
        }

        public ActionResult AddEditCustClearDet(/*CustomClearanceDetailMainVm custClearDetailMain*/FormCollection form, string notes)
        {
            CustomClearanceDetailMainVm custClearDetailMain = new CustomClearanceDetailMainVm();
            TryUpdateModel(custClearDetailMain, form);
            string isSaved = CustomClearanceHelper.AddEditCustClearDet(custClearDetailMain, notes, form["Comment"]);
            return Json(isSaved);
        }

        public ActionResult GetCCOrderList(int operationId, byte orderFrom)
        {
            var ccOrderList = CustomClearanceHelper.GetCCOrdersList(operationId, orderFrom);
            return PartialView("~/Views/CustomClearance/_CCOrdersList.cshtml", ccOrderList);
        }

        public ActionResult GetOrderDetails(int ccId, int operationId)
        {
            Session["CCId"] = ccId;
            var ccViewObj = CustomClearanceHelper.GetOneCustClearView(ccId);
            ViewBag.OrderDetails = CustomClearanceHelper.GetCCDetailsList(ccId);
            ViewBag.ContainersSummary = OperationHelper.GetContainersSummary(operationId);
            return PartialView("~/Views/CustomClearance/_MoreDetails.cshtml", ccViewObj);
        }

        public ActionResult CloseCC(int ccId, byte statusId, byte orderFrom = 1)
        {
            #region Check Rights
            bool hasRights = false;
            if (statusId == 4) //Canceled .. will be from HB screen only
            {
                if (orderFrom == 1) //Check export rights
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportHB, ActionEnum.CancelCCOrder);
                else
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportHB, ActionEnum.CancelCCOrder);
            }
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CustomClearanceOrders, ActionEnum.CloseOrder);

            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isClosed = CustomClearanceHelper.CloseCC(ccId, statusId);
            return Json(isClosed);
        }

        /// <summary>
        /// Print CC order details
        /// </summary>
        /// <param name="id">CCId .. will be 0 if print from CC grid</param>
        /// <returns></returns>
        public ActionResult PrintDetails(int operationId,int ccId = 0)
        {
            if (ccId == 0)
                ccId = (int)Session["CCId"];

            return new ActionAsPdf("PrintDetailsV", new { id = ccId, operationId = operationId, name = "Giorgio" }) { FileName = "CustomClearanceDetails.pdf" };
        }

        [AllowAnonymous]
        public ActionResult PrintDetailsV(int id, int operationId)
        {
            var ccViewObj = CustomClearanceHelper.GetOneCustClearView(id);
            ViewBag.OrderDetails = CustomClearanceHelper.GetCCDetailsList(id);
            ViewBag.ContainersSummary = OperationHelper.GetContainersSummary(operationId);
            return View(ccViewObj);
        }




        public ActionResult PrintWorkOrder(int operationId, int ccId = 0)
        {
            if (ccId == 0)
                ccId = (int)Session["CCId"];

            return new ActionAsPdf("PrintWorkOrderV", new { id = ccId, operationId = operationId, name = "Giorgio" }) { FileName = "CustomClearanceDetails.pdf" };
          }

        [AllowAnonymous]
        public ActionResult PrintWorkOrderV(int id, int operationId)
        { 
            var ccViewObj = CustomClearanceHelper.GetOneCustClearView(id);
            ViewBag.OrderDetails = CustomClearanceHelper.GetCCDetailsList(id);
            ViewBag.ContainersSummary = OperationHelper.GetContainersSummary(operationId);
            return View(ccViewObj);
        }

        public ActionResult AdvSearch()
        {
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            return PartialView("~/Views/CustomClearance/_AdvSearch.cshtml");
        }

    }
}
