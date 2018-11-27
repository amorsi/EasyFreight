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
    public class OperationController : Controller
    {
        //
        // GET: /Operation/

        public ActionResult Index(string orderFrom)
        {

            #region Check Rights
            bool hasRights;
            if (orderFrom.ToLower() == "export") //Check export rights
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportMBL, ActionEnum.ViewAll);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportMBL, ActionEnum.ViewAll);

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home");
            #endregion

            if (orderFrom.ToLower() == "export")
            {
                ViewBag.OperationType = "Export";
                ViewBag.OrderFrom = 1;
            }
            else if (orderFrom.ToLower() == "import")
            {
                ViewBag.OperationType = "Import";
                ViewBag.OrderFrom = 2;
            }
            else
            {
                //Error Page
            }

            return View();
        }

        public JObject GetTableJson(FormCollection form)
        {
            var quotationOrders = OperationHelper.GetOperationOrders(form);
            return quotationOrders;
        }

        /// <summary>
        /// Add export or import MBL operation
        /// </summary>
        /// <param name="orderFrom"> export --- import</param>
        /// <param name="id">operation id to load edit mode view</param>
        /// <param name="quoteId">quotation id in case of processing an exsiting quotation</param>
        /// <returns>the add operation view</returns>
        public ActionResult Add(string orderFrom = "export", int id = 0, int quoteId = 0)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom.ToLower() == "export") //Check export rights
            {
                if (quoteId != 0) //process quotation check
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportQuotation, ActionEnum.ProcessToMBL);
                else
                {
                    if (id == 0)
                        hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportMBL, ActionEnum.Add);
                    else
                        hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportMBL, ActionEnum.Edit);
                }

            }
            else
            {
                if (quoteId != 0) //process quotation check
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportQuotation, ActionEnum.ProcessToMBL);
                else
                {
                    if (id == 0)
                        hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportMBL, ActionEnum.Add);
                    else
                        hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportMBL, ActionEnum.Edit);
                }

            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home");

            #endregion

            byte orderFromInt = 0;
            if (orderFrom.ToLower() == "export")
            {
                ViewBag.OperationType = "Export";
                orderFromInt = 1;
            }
            else if (orderFrom.ToLower() == "import")
            {
                ViewBag.OperationType = "Import";
                orderFromInt = 2;
            }
            else
            {
                //Error Page
            }


            OperationVm operationVm = OperationHelper.GetOperationInfo(id, orderFromInt, quoteId);

            ViewBag.CarrierList = ListCommonHelper.GetCarrierList();
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewBag.NotifierList = ListCommonHelper.GetNotifierList(0);
            ViewBag.IncotermLib = ListCommonHelper.GetIncotermLibList();
            ViewBag.Containers = ListCommonHelper.GetContainerList();
            ViewBag.PackageList = ListCommonHelper.GetPackageTypeList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            ViewData["CurrencyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.AgentList = ListCommonHelper.GetAgentList();

            return View(operationVm);
        }

        /// <summary>
        /// The post action from add view. Save the operation data
        /// </summary>
        /// <param name="operationVm">Operation view model object</param>
        /// <returns>string "true" or "false + error message"</returns>
        public ActionResult AddEditOperation(OperationVm operationVm)
        {
            int operId;
            string isSaved = OperationHelper.AddEditOperation(operationVm, out operId);
            return Json(new { isSaved = isSaved, operId = operId });
        }

        /// <summary>
        /// The main view to process MBL .. it contains the all tabs
        /// </summary>
        /// <param name="orderFrom">export --- import</param>
        /// <param name="id">Operation Id</param>
        /// <returns>HouseBill with tabs</returns>
        public ActionResult HouseBill(string orderFrom, int id)
        {

            #region Check Rights
            bool hasRights;
            if (orderFrom.ToLower() == "export") //Check export rights
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportMBL, ActionEnum.ProcessToHB);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportMBL, ActionEnum.ProcessToHB);

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home");
            #endregion

            if (orderFrom.ToLower() == "export")
            {
                ViewBag.OperationType = "Export";
            }
            else if (orderFrom.ToLower() == "import")
            {
                ViewBag.OperationType = "Import";
            }
            else
            {
                //Error Page
            }


            EasyFreight.Models.OperationView operationView = OperationHelper.GetOne(id);
            TempData["OperationObj"] = operationView;
            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.OperContainers = OperationHelper.GetOperationContainers(id);
            ViewBag.HBCount = HouseBillHelper.GetHBCount(id);
            //ViewBag.TruckingCount = TruckingHelper.GetTruckingOrdersCount(id);
            return View(operationView);
        }

        /// <summary>
        /// Load add container partial view based on operationtype
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="carrierType"></param>
        /// <returns></returns>
        public ActionResult GetOperationContainers(int operationId, byte carrierType)
        {
            List<OperationContainerVm> operContainerList = OperationHelper.GetOperationContainers(operationId);
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            ViewBag.PackageList = ListCommonHelper.GetPackageTypeList();
            if (carrierType == 1)//sea
            {
                return PartialView("~/Views/Operation/_AddContainer.cshtml", operContainerList);
            }
            else
            {
                return PartialView("~/Views/Operation/_AddContainerAir.cshtml", operContainerList);
            }
        }

        /// <summary>
        /// Get operation details for details pop-up
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <returns></returns>
        public ActionResult GetOperationDetails(int id)
        {
            Models.OperationView operationView = new Models.OperationView();
            if (TempData["OperationObj"] != null)
                operationView = (Models.OperationView)TempData["OperationObj"];
            else
                operationView = OperationHelper.GetOne(id);

            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.OperContainers = OperationHelper.GetOperationContainers(id);
            return PartialView("~/Views/Operation/_MoreDetails.cshtml", operationView);
        }

        public ActionResult AdvSearch()
        {
            ViewBag.ContainerList = ListCommonHelper.GetContainerList();
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewBag.CarrierList = ListCommonHelper.GetCarrierList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            return PartialView("~/Views/Operation/_AdvSearch.cshtml");
        }

        public ActionResult CloseOperation(int id, byte orderFrom)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportMBL, ActionEnum.CloseOrder);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportMBL, ActionEnum.CloseOrder);

            if (!hasRights)
                return Json("You are UnAuthorized to do this action");
            #endregion

            string isClosed = OperationHelper.CloseOperation(id);
            return Json(isClosed);
        }

        public ActionResult DeleteOperation(int id, byte orderFrom)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportMBL, ActionEnum.CloseOrder);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportMBL, ActionEnum.CloseOrder);

            if (!hasRights)
                return Json("You are UnAuthorized to do this action");
            #endregion

            string isClosed = OperationHelper.DeleteOperation(id);
            return Json(isClosed);
        }

        #region House Bill
        /// <summary>
        /// Get house bill .. in case of no house bills .. will fill its view model from operation data
        /// </summary>
        /// <param name="operationId"> int Operation Id </param>
        /// <param name="oprOrderFrom">byte Order From</param>
        /// <returns></returns>
        public ActionResult GetHbContent(int operationId, byte oprOrderFrom, int hbId = 0)
        {
            #region Check Rights
            bool hasRights;
            if (oprOrderFrom == 1) //Check export rights
            {
                if (hbId == 0)
                        hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportHB, ActionEnum.Add);
                    else
                    hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportHB, ActionEnum.Edit);
            }
            else
            {
                if (hbId == 0)
                        hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportHB, ActionEnum.Add);
                    else
                        hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportHB, ActionEnum.Edit);
            }

            if (!hasRights)
                return PartialView("~/Views/Shared/_UnAuthorized.cshtml");

            #endregion
            HouseBillVm houseBillVm = HouseBillHelper.GetHbContent(operationId, oprOrderFrom, hbId);
            ViewBag.ShipperList = ListCommonHelper.GetShipperList();
            ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewBag.IncotermLib = ListCommonHelper.GetIncotermLibList();
            ViewBag.Containers = ListCommonHelper.GetContainerList();
            ViewBag.PackageList = ListCommonHelper.GetPackageTypeList();
            ViewData["PortList"] = ListCommonHelper.GetPortsGrouped();
            ViewData["CurrencyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.AgentList = ListCommonHelper.GetAgentList();
            ViewBag.OperationVm = OperationHelper.GetOperationInfo(operationId);
            ViewBag.OperContainers = OperationHelper.GetOperationContainers(operationId);
            ViewBag.SelectedHbCon = HouseBillHelper.GetHousContainerByHouseID(hbId);
             ViewBag.NotifierList = ListCommonHelper.GetNotifierList(0);
            return PartialView("~/Views/Operation/_HouseBill.cshtml", houseBillVm);
        }

        /// <summary>
        /// The post action to save House Bill data
        /// </summary>
        /// <param name="houseBillVm">House Bill view model object</param>
        /// <returns>string "true" or "false + error message"</returns>
        public ActionResult AddEditHouseBill(HouseBillVm houseBillVm)
        {
            string isSaved = HouseBillHelper.AddEditHouseBill(houseBillVm);
            return Json(isSaved);
        }

        public ActionResult GetHbList(int operationId, byte oprOrderFrom)
        {
            var hbList = HouseBillHelper.GetHBList(operationId, oprOrderFrom);
            TempData["hbList"] = hbList;
            ViewBag.IsConsolidation = OperationHelper.CheckOperationIsConsolidation(operationId);
            ViewBag.OrderFrom = oprOrderFrom;
            return PartialView("~/Views/Operation/_HouseBillList.cshtml", hbList);
        }

        /// <summary>
        /// Get One record from HB view to view in the modal
        /// </summary>
        /// <param name="houseBillId">House bill Id</param>
        /// <returns>_ViewHouseBill partial view</returns>
        public ActionResult GetHBForView(int houseBillId)
        {
            var hbView = HouseBillHelper.GetHBView(houseBillId);
            return PartialView("~/Views/Operation/_ViewHouseBill.cshtml", hbView);
        }

        [AllowAnonymous]
        public ActionResult PrintHBDetailsV(int houseBillId=1033)
        {
            var hbView = HouseBillHelper.GetHBView(houseBillId); 
            int id = hbView.OperationId;
            EasyFreight.Models.OperationView quoteView = OperationHelper.GetOne(id);
            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.QuoteContainers = OperationHelper.GetOperationContainers(id);
            ViewData["StaticText"] = CommonHelper.GetStaticTextById(1);

            //if (quoteView.IsCareOf)
            //{
            //    CompanySetupVm company = CompanySetupHelper.GetCompanySetup();
            //    ViewBag.ShipperAddress = company.CompanyAddressEn;
            //    ViewBag.ShipperTel = (!string.IsNullOrEmpty(company.PhoneNumber1) ? " Tel. " + company.PhoneNumber1 : "") +
            //        (!string.IsNullOrEmpty(company.FaxNumber) ? "  Fax. " + company.FaxNumber : "");
            //}
            //else
            //{
                ViewBag.ShipperAddress = quoteView.ShipperAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(quoteView.ShipperPhoneNumber) ? " Tel. " + quoteView.ShipperPhoneNumber : "") +
                    (!string.IsNullOrEmpty(quoteView.ShipperFaxNumber) ? " Fax. " + quoteView.ShipperPhoneNumber : "");
            //}




            return View(hbView);
        }

        public ActionResult PrintHBDetails(int houseBillId)
        {
           return new ActionAsPdf("PrintHBDetailsV", new { houseBillId = houseBillId, name = "Giorgio" }) { FileName = "HouseBillDetails.pdf" };
            // return View("PrintHBDetailsV");
        }

        public ActionResult GetHBCost(int houseBillId, int operationId)
        {
            ViewBag.OperationCostNameList = ListCommonHelper.GetOperationCostList();
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            ViewBag.HbObj = ((List<HouseBillListVm>)TempData["hbList"]).Where(x => x.HouseBillId == houseBillId).FirstOrDefault();
            Session["hbOneObj"] = ((List<HouseBillListVm>)TempData["hbList"]).Where(x => x.HouseBillId == houseBillId).FirstOrDefault();
            OperationCostMainVm hbCostAdd = HouseBillHelper.GetHBCost(houseBillId, operationId);
            return PartialView("~/Views/Operation/_OperationCost.cshtml", hbCostAdd);
        }


        public ActionResult AddEditHbCost(OperationCostMainVm operCostMainVm)
        {
            string isSaved = HouseBillHelper.AddEditHbCost(operCostMainVm);
            return Json(isSaved);
        }

        public ActionResult AddEditAccountHbCost(OperationCostMainVm operCostMainVm)
        {
            string isSaved = HouseBillHelper.AddEditAccountHbCost(operCostMainVm);
            return Json(isSaved);
        }

        [AllowAnonymous]
        public ActionResult PrintHBCostV(HouseBillListVm hbObj)
        {
            ViewBag.HbObj = hbObj;
            OperationCostMainVm hbCostView = HouseBillHelper.GetHBCost(hbObj.HouseBillId, 0);
            return View(hbCostView);
        }
        /// <summary>
        /// Print House bill operation cost
        /// </summary>
        /// <param name="id">houseBillId</param>
        /// <returns></returns>
        public ActionResult PrintHbCost(int id)
        {
            var HbObj = (HouseBillListVm)Session["hbOneObj"];
            return new ActionAsPdf("PrintHBCostV", HbObj) { FileName = "HouseBillCostDetails.pdf" };
        }

        /// <summary>
        /// Close House bill order
        /// </summary>
        /// <param name="hbId">HouseBillId</param>
        /// <returns>true is succeed</returns>
        public ActionResult CloseHB(int hbId, byte orderFrom = 1)
        {
            #region Check Rights
            bool hasRights;
            if (orderFrom == 1) //Check export rights
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ExportHB, ActionEnum.CloseOrder);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.ImportHB, ActionEnum.CloseOrder);

            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isClosed = HouseBillHelper.ChangeHBStatus(hbId, (int)StatusEnum.Closed);
            return Json(isClosed);
        }

        #endregion


        public ActionResult Summary(string orderFrom)
        {
            if (orderFrom.ToLower() == "export")
            {
                ViewBag.OperationType = "Export";
                ViewBag.OrderFrom = 1;
            }
            else if (orderFrom.ToLower() == "import")
            {
                ViewBag.OperationType = "Import";
                ViewBag.OrderFrom = 2;
            }
            else
            {
                //Error Page
            }

            //var operationStatistic = OperationHelper.GetOperationStatistics(DateTime.Now.AddDays(-7), DateTime.Now,(int)ViewBag.OrderFrom);
            //return PartialView("~/Views/Operation/_Widget.cshtml", operationStatistic);
           return View( );
        }

        public ActionResult OperationStatistics(int? orderfrom, DateTime fromDate, DateTime toDate)
        { 
            var operationStatistic = OperationHelper.GetOperationStatistics(fromDate, toDate, orderfrom);
            return PartialView("~/Views/Operation/_Widget.cshtml", operationStatistic);
            //return View(operationStatistic);
        }

        //public ActionResult Dashoard( byte orderFrom  =1,string fromDate="", string toDate="")
        //{
        //    if (fromDate == "")
        //        fromDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
        //    if (toDate == "")
        //        toDate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");


        //    string isClosed = OperationHelper.CloseOperation(id);
        //    return Json(isClosed);
        //}


        public ActionResult GetShippingDeclaration(int id)
        {
            EasyFreight.Models.OperationView quoteView = OperationHelper.GetOne(id);
            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.QuoteContainers = OperationHelper.GetOperationContainers(id);
            ViewData["StaticText"] = CommonHelper.GetStaticTextById(1);
            Session["quoteId"] = id;
            if (quoteView.IsCareOf)
            {
                CompanySetupVm company = CompanySetupHelper.GetCompanySetup();
                ViewBag.ShipperAddress = company.CompanyAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(company.PhoneNumber1) ? " Tel. " + company.PhoneNumber1 : "") +
                    (!string.IsNullOrEmpty(company.FaxNumber) ? "  Fax. " + company.FaxNumber : "");
            }
            else
            {
                ViewBag.ShipperAddress = quoteView.ShipperAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(quoteView.ShipperPhoneNumber) ? " Tel. " + quoteView.ShipperPhoneNumber : "") +
                    (!string.IsNullOrEmpty(quoteView.ShipperFaxNumber) ? " Fax. " + quoteView.ShipperPhoneNumber : "");
            }
            return PartialView("~/Views/Operation/_ShippingDecl.cshtml", quoteView);
        }

        [AllowAnonymous]
        public ActionResult PrintShippingDeclV(int id)
        {
            EasyFreight.Models.OperationView quoteView = OperationHelper.GetOne(id);
            ViewData["CurrecyList"] = ListCommonHelper.GetCurrencyList();
            ViewBag.QuoteContainers = OperationHelper.GetOperationContainers(id);
            ViewData["StaticText"] = CommonHelper.GetStaticTextById(1);

            if (quoteView.IsCareOf)
            {
                CompanySetupVm company = CompanySetupHelper.GetCompanySetup();
                ViewBag.ShipperAddress = company.CompanyAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(company.PhoneNumber1) ? " Tel. " + company.PhoneNumber1 : "") +
                    (!string.IsNullOrEmpty(company.FaxNumber) ? "  Fax. " + company.FaxNumber : "");
            }
            else
            {
                ViewBag.ShipperAddress = quoteView.ShipperAddressEn;
                ViewBag.ShipperTel = (!string.IsNullOrEmpty(quoteView.ShipperPhoneNumber) ? " Tel. " + quoteView.ShipperPhoneNumber : "") +
                    (!string.IsNullOrEmpty(quoteView.ShipperFaxNumber) ? " Fax. " + quoteView.ShipperPhoneNumber : "");
            }

            return View(quoteView);

        }

        public ActionResult PrintShippingDecl()
        {
            int quoteId = (int)Session["quoteId"];
            return new ActionAsPdf("PrintShippingDeclV", new { id = quoteId, name = "Giorgio" }) { FileName = "ShippingDeclaration.pdf" };
        }

        public ActionResult AddHbContainer(List<int> data, int hbID,int operationId)
        {
            string isSaved = "";
            HouseContainerVm hcv = new HouseContainerVm();
            hcv.HouseBillId = hbID ;
            hcv.OperationId = operationId;
            hcv.OperConId = 0 ;

            HouseBillHelper.DeleteByHouseContainer(hcv);
            foreach (var item in data)
            {
                hcv.OperConId = item;
                HouseBillHelper.CreateHouseContainer(hcv);

            }
             return Json(isSaved);
        
        }
    }
}
