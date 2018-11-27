using EasyFreight.DAL;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class ConsigneeController : Controller
    {
        //
        // GET: /MasterData/Consignee/
        [CheckRights(ScreenEnum.Consignee, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            List<ConsigneeVm> consigneeList = ConsigneeHelper.GetConsigneeList();
            return View(consigneeList);
        }

        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Consignee, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Consignee, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion

            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            ConsigneeVm consigneeObj = ConsigneeHelper.GetConsigneeInfo(id);
            return View(consigneeObj);
        }

        public ActionResult AddEditConsignee(ConsigneeVm consigneeVm)
        {
            string isSaved = ConsigneeHelper.AddEditConsignee(consigneeVm);
            return Json(isSaved);
        }

        public ActionResult AddConsigneeQuick(string code, string name)
        {
            int conigneeId = ConsigneeHelper.AddConsigneeQuick(code, name);
            return Json(conigneeId);
        }

        public ActionResult GetConsigneeContacts(int consigneeId)
        {
            var contactList = ConsigneeHelper.GetContactsList(consigneeId);
            return PartialView("~/Views/Shared/_ViewContacts.cshtml", contactList);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.Consignee, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = ConsigneeHelper.Delete(id);
            return Json(isDeleted);
        }

    }
}
