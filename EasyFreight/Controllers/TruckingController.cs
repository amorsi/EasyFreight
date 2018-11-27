using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.ViewModel;
using EasyFreight.Models;
using EasyFreight.DAL;
using Newtonsoft.Json.Linq;
using Rotativa;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class TruckingController : Controller
    {
        //
        // GET: /Trucking/

        [CheckRights(ScreenEnum.TruckingOrders, ActionEnum.ViewAll)]
        public ActionResult Index(int id = 0)
        {
            return View();
        }

        public ActionResult AdvSearch()
        {
            ViewBag.ContractorList = ListCommonHelper.GetContractorList();
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewData["AreaList"] = ListCommonHelper.GetAreaGrouped();
            return PartialView("~/Views/Trucking/_AdvSearch.cshtml");
        }

        public JObject GetTableJson(FormCollection form = null)
        {
            var truckingOrders = TruckingHelper.GetAllOrders(form);
            return truckingOrders;
        }

        [CheckRights(ScreenEnum.TruckingOrders, ActionEnum.ManageOrder)]
        public ActionResult ManageOrder(int id = 0)
        {
            var truckingOrderDetailVm = TruckingHelper.GetTruckingOrderDetailInfo(id);

            ViewBag.TruckingCostNameList = ListCommonHelper.GetTruckingCostNameList();
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.ContractorList = ListCommonHelper.GetContractorListByCity("en", truckingOrderDetailVm.FromCityId,
                truckingOrderDetailVm.ToCityId);

            ViewBag.PackageList = ListCommonHelper.GetPackageTypeList();

            if (truckingOrderDetailVm.ContractorId != 0)
                ViewBag.Contractorobj = ContractorHelper.GetContractorInfo(truckingOrderDetailVm.ContractorId).ContractorNameEn;



            return View(truckingOrderDetailVm);
        }

        public ActionResult AddEditTruckingOrderDetails(TruckingOrderDetailVm truckingOrderVm)
        {
            string isSaved = TruckingHelper.AddEditTruckingOrderDetails(truckingOrderVm);
            return Json(isSaved);
        }

        public ActionResult GetOrderDetails(int truckingOrder)
        {
            var truckingOrderDetailVm = TruckingHelper.GetTruckingOrderDetailInfo(truckingOrder);
            Session["truckingOrderId"] = truckingOrder;
            return PartialView("~/Views/Trucking/_MoreDetails.cshtml", truckingOrderDetailVm);

        }

        [AllowAnonymous]
        public ActionResult PrintDetailsV(int id)
        {
            var truckingOrderDetailVm = TruckingHelper.GetTruckingOrderDetailInfo(id);
            return View(truckingOrderDetailVm);
        }

        public ActionResult PrintDetails(int truckingOrderId = 0)
        {
            if (truckingOrderId == 0)
                truckingOrderId = (int)Session["truckingOrderId"];

            return new ActionAsPdf("PrintDetailsV", new { id = truckingOrderId, name = "Giorgio" }) { FileName = "TruckingDetails.pdf" };
        }

        [CheckRights(ScreenEnum.TruckingSummary, ActionEnum.ViewAll)]
        public ActionResult Summary()
        {
            DateTime fromDate = DateTime.Now.AddDays(-7) ;
            DateTime toDate = DateTime.Now.AddDays(1) ; 
            
            //ViewBag.LatestOrders = TruckingHelper.GetTopNewFive();
            //return View();
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewData["truckingtopSaummary"] = TruckingHelper.GetTruckingTopByDate(fromDate, toDate);
            
            return View ( );
        }

        public ActionResult AddTruckingOrder(int operationId, byte orderFrom = 1, int houseBillId = 0)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportHB, ActionEnum.AddEditTruckingOrder);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportHB, ActionEnum.AddEditTruckingOrder);
            }

            if (!hasRights)
                return PartialView("~/Views/Shared/_UnAuthorized.cshtml");

            #endregion

            // remember that cannot create more than one tracking order unless status canceled 

            TruckingOrderVm trkoVmObj = TruckingHelper.NewTruckingOrderByOperation(operationId, orderFrom, houseBillId);
            if (trkoVmObj.OrderFrom == 1)
            {
                var shipperObj = ShipperHelper.GetShipperInfo(trkoVmObj.ShipperId);
                ViewBag.ClientName = shipperObj.ShipperNameEn;
                trkoVmObj.ShipFromAddress = shipperObj.ShipperAddressEn;
            }
            else
            {
                var consigneeObj = ConsigneeHelper.GetConsigneeInfo(trkoVmObj.ConsigneeId);
                ViewBag.ClientName = consigneeObj.ConsigneeNameEn;
                trkoVmObj.ShipFromAddress = consigneeObj.ConsigneeAddressEn;
            }

            ViewData["AreaList"] = ListCommonHelper.GetAreaGrouped();
            ViewBag.ContainerSummary = OperationHelper.GetContainersSummary(trkoVmObj.OperationId);

            return PartialView("~/Views/Trucking/AddTruckingOrder.cshtml", trkoVmObj);
        }

        /// <summary>
        /// Post action to create truckign order from operation
        /// </summary>
        /// <param name="truckingOrderVm">TruckingOrderVm</param>
        /// <returns>strign "true" if saved successfully</returns>
        public ActionResult NewTruckingOrder(TruckingOrderVm truckingOrderVm)
        {
            string isSaved = TruckingHelper.AddTruckingOrder(truckingOrderVm);
            return Json(isSaved);
        }

        public ActionResult CloseOrder(int truckingOrderId, byte statusId, byte orderFrom = 1)
        {
            #region Check Rights
            bool hasRights = false;
            if (statusId == 4) //Canceled .. will be from HB screen only
            {
                if (orderFrom == 1) //Check export rights
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportHB, ActionEnum.CancelTruckingOrder);
                else
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportHB, ActionEnum.CancelTruckingOrder);
            }
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.TruckingOrders, ActionEnum.CloseOrder);

            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isChanged = TruckingHelper.ChangeStatus(truckingOrderId, statusId);
            return Json(isChanged);//, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Roll(int truckingOrderId)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.TruckingOrders, ActionEnum.RollOrder);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

             //string isChanged = TruckingHelper.ChangeStatus(truckingOrderId, 5);
            string isChanged = TruckingHelper.DeleteTruckingOrder(truckingOrderId );
            return Json(isChanged);//, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTruckingOrderList(int operationId, byte orderFrom)
        {
            var truckingOrderList = TruckingHelper.GetTruckingOrderList(operationId, orderFrom);
            return PartialView("~/Views/Trucking/_TruckingOrderList.cshtml", truckingOrderList);
        }


        public ActionResult TruckingStatistics(DateTime? fromDate , DateTime? toDate )
        {
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var truckingtopSaummary = TruckingHelper.GetTruckingTopByDate(fromDate, toDate);
            return PartialView("~/Views/Trucking/_Widget.cshtml", truckingtopSaummary);
            //return View(operationStatistic);
        }


        public ActionResult CheckIfHasInv(int truckingOrderId)
        {
            bool isHas = TruckingHelper.HasInv(truckingOrderId);
            return Json(isHas);
        }
    }
}
