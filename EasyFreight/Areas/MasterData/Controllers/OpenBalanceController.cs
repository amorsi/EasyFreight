using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;
using EasyFreight.ViewModel;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class OpenBalanceController : Controller
    {

      //  GET: /MasterData/OpenBalance/
          [CheckRights(ScreenEnum.OpenBalance, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            ViewBag.LibItems = ListCommonHelper.GetAgentList();
            //var openBalVm = AccountingHelper.GetOpenBalanceInfo("Agent", 0, "AgentId");
            ViewBag.TbName = "Agent";
            ViewBag.PkName = "AgentId";

            return View();
        }

        public ActionResult GetBalancePartial(int itemId, string tbName, string pkName)
        {
            var balanceVm = AccountingHelper.GetOpenBalanceInfo(tbName, itemId, pkName);
            return PartialView("~/Areas/MasterData/Views/OpenBalance/_AddForm.cshtml", balanceVm);
        }

        public ActionResult AddEditBalance(OpenBalanceVm openBalanceVm)
        {
            #region Check Rights
            bool hasRights;

            string accountId = "";
            string tbName, pkName; int libId;

            libId = openBalanceVm.LibItemId;
            tbName = openBalanceVm.TbName;
            pkName = openBalanceVm.PkName;
            //Get AccountId
            accountId = AccountingChartHelper.GetAccountIdByPkAndTbName(libId, tbName, pkName);
            if (string.IsNullOrEmpty(accountId))
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.OpenBalance, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.OpenBalance, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion

            string isSaved = AccountingHelper.AddEditOpenBalance(openBalanceVm);

            return Json(new { IsSaved = isSaved });
        }

        public ActionResult GetOtherTabForm(string tbName)
        {
            switch (tbName)
            {
                case "Shipper":
                    ViewBag.TbName = "Shipper";
                    ViewBag.PkName = "ShipperId";
                    ViewBag.LibItems = ListCommonHelper.GetShipperList();
                    break;
                case "Consignee":
                    ViewBag.TbName = "Consignee";
                    ViewBag.PkName = "ConsigneeId";
                    ViewBag.LibItems = ListCommonHelper.GetConsigneeList();
                    break;
                case "Carrier":
                    ViewBag.TbName = "Carrier";
                    ViewBag.PkName = "CarrierId";
                    ViewBag.LibItems = ListCommonHelper.GetCarrierList();
                    break;
                case "Contractor":
                    ViewBag.TbName = "Contractor";
                    ViewBag.PkName = "ContractorId";
                    ViewBag.LibItems = ListCommonHelper.GetContractorList();
                    break;
                case "Currency":
                    ViewBag.TbName = "Currency";
                    ViewBag.PkName = "CurrencyId";
                    ViewBag.LibItems = ListCommonHelper.GetCurrencyList();
                    break;
                case "BankAccount":
                    ViewBag.TbName = "BankAccount";
                    ViewBag.PkName = "BankAccId";
                    ViewBag.LibItems = ListCommonHelper.GetBankAccList();
                    break;
                case "PartnerAccount":
                    ViewBag.TbName = "AccountingChart";
                    ViewBag.PkName = "AccountId";
                    ViewBag.LibItems = ListCommonHelper.GetPartnersList();  
                    break;                                                  
                case "CashDepositTempAccount":                              
                    ViewBag.TbName = "AccountingChart";                     
                    ViewBag.PkName = "AccountId";                           
                    ViewBag.LibItems = ListCommonHelper.GetCCDepositList(); 
                    break;

            }


            return PartialView("~/Areas/MasterData/Views/OpenBalance/_AddBalanceMain.cshtml");
        }

    }
}