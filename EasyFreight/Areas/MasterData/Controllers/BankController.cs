using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.ViewModel;
using EasyFreight.DAL;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class BankController : Controller
    {
        //
        // GET: /MasterData/Bank/
        [CheckRights(ScreenEnum.Bank, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            List<BankVm> bankList = BankHelper.GetBankList();
            return View(bankList);
        }

        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) //Check export rights
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Bank, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Bank, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion
            BankVm bankVm = BankHelper.GetBankInfo(id);
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();
            return View(bankVm);
        }

        public ActionResult AddEditBank(BankVm bankVm)
        {
            string isSaved = BankHelper.AddEditBank(bankVm);
            return Json(isSaved);
        }

        public ActionResult GetBankAccount(int bankId)
        {
            List<BankAccountVm> bankAccountList = BankHelper.GetBankAccount(bankId);
            return PartialView("~/Areas/MasterData/Views/Bank/_ViewBankAccounts.cshtml", bankAccountList);
        }

        public ActionResult DeleteBank(int id)
        {
             #region Check Rights
             bool hasRights = false;
             hasRights = AdminHelper.CheckUserAction(ScreenEnum.Bank, ActionEnum.Delete);
             if (!hasRights)
                 return Json("You are UnAuthorized to do this action");
             
             #endregion

            string isDeleted = BankHelper.DeleteBank(id);
            return Json(isDeleted);
        }
	}
}