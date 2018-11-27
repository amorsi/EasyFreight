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
    public class ContractorController : Controller
    {
        //
        // GET: /MasterData/Contractor/
        [CheckRights(ScreenEnum.Contractor, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            List<ContractorVm> contractorList = ContractorHelper.GetContractorList();
            return View(contractorList);
        }

        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Contractor, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Contractor, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion

            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            ContractorVm contractorObj = ContractorHelper.GetContractorInfo(id);
            return View(contractorObj);
        }

        public ActionResult AddEditContractor(ContractorVm contractorVm)
        {
            string isSaved = ContractorHelper.AddEditContractor(contractorVm);
            return Json(isSaved);
        }

        public ActionResult GetContractorContacts(int contractorId)
        {
            var contactList = ContractorHelper.GetContactsList(contractorId);
            return PartialView("~/Views/Shared/_ViewContacts.cshtml", contactList);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.Contractor, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion
            string isDeleted = ContractorHelper.Delete(id);
            return Json(isDeleted);
        }

    }
}
