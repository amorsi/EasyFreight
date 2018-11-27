using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class CostLibraryController : Controller
    {
        //
        // GET: /MasterData/CostLibrary/
        #region Trucking Cost
        [CheckRights(ScreenEnum.TruckingCostLib, ActionEnum.ViewAll)]
        public ActionResult Trucking()
        {
            var costLib = CostLibHelper.GetTruckingCostLib();
            return View(costLib);
        }

        public JsonResult AddEditTruckingCost(int costId, string costNameEn, string costNameAr)
        {
            #region Check Rights
            bool hasRights = false;
            if (costId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.TruckingCostLib, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.TruckingCostLib, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            string isSaved = CostLibHelper.AddEditTruckCost(costId, costNameEn, costNameAr);
            return Json(isSaved);
        }

        public ActionResult DeleteTruckingCost(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.TruckingCostLib, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = CostLibHelper.DeleteTruckingCost(id);
            return Json(isDeleted);
        }

        #endregion

        #region Operation Cost
        [CheckRights(ScreenEnum.OperationCostLib, ActionEnum.ViewAll)]
        public ActionResult Operation()
        {
            var costLib = CostLibHelper.GetOperationCostLib();
            return View(costLib);
        }

        public JsonResult AddEditOperationCost(int costId, string costNameEn, string costNameAr)
        {
            #region Check Rights
            bool hasRights = false;
            if (costId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.OperationCostLib, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.OperationCostLib, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            string isSaved = CostLibHelper.AddEditOperationCost(costId, costNameEn, costNameAr);
            return Json(isSaved);
        }

        public JsonResult DeleteOperationCost(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.OperationCostLib, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = CostLibHelper.DeleteOperationCost(id);
            return Json(isDeleted);
        }
        #endregion

        #region CustomClearance Cost
        [CheckRights(ScreenEnum.CustomClearanceCostLib, ActionEnum.ViewAll)]
        public ActionResult CustomClearance()
        {
            var costLib = CostLibHelper.GetCCCostLib();
            return View(costLib);
        }

        public JsonResult AddEditCCCost(int costId, string costNameEn, string costNameAr)
        {
            #region Check Rights
            bool hasRights = false;
            if (costId == 0)
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CustomClearanceCostLib, ActionEnum.Add);
            else
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.CustomClearanceCostLib, ActionEnum.Edit);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            string isSaved = CostLibHelper.AddEditCCCost(costId, costNameEn, costNameAr);
            return Json(isSaved);
        }

        public JsonResult DeleteCCCost(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.CustomClearanceCostLib, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = CostLibHelper.DeleteCCCost(id);
            return Json(isDeleted);
        }
        #endregion

    }
}