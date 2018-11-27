using EasyFreight.DAL;
using EasyFreight.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;


namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class NotifierController : Controller
    {
        //
        // GET: /MasterData/Notifier/
        [CheckRights(ScreenEnum.Notifier, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            List<NotifierVm> notifierList = NotifierHelper.GetNotifierList();
            return View(notifierList);
        }

        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Notifier, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Notifier, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion
           // ViewBag.ConsigneeList = ListCommonHelper.GetConsigneeList();
            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            NotifierVm notifierObj = NotifierHelper.GetNotifierInfo(id);
            return View(notifierObj);
        }

        public ActionResult AddEditNotifier(NotifierVm notifierVm)
        {
            string isSaved = NotifierHelper.AddEditNotifier(notifierVm);
            return Json(isSaved);
        }

        public ActionResult GetNotifierContacts(int notifierId)
        {
            var contactList = NotifierHelper.GetContactsList(notifierId);
            return PartialView("~/Views/Shared/_ViewContacts.cshtml", contactList);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.Notifier, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = NotifierHelper.Delete(id);
            return Json(isDeleted);
        }
    }
}
