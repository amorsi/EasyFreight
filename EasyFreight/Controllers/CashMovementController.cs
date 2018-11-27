using EasyFreight.DAL;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyFreight.Controllers
{
    public class CashMovementController : Controller
    {
        // GET: CashMovement
        public ActionResult Index()
        {
            List<CashBankRptVm> currVm = AccountingRptHelper.GetBankCashRpt();
            return View(currVm);
        }

        [HttpGet]
        public ActionResult CurrencyExchange()
        {
            ViewBag.CashBalance = AccountingRptHelper.GetBankCashRpt().Where(a => a.AccountId == "194").ToList();
            CurrencyExchangeVm currVm = new CurrencyExchangeVm();
            return View(currVm);
        }

        [HttpPost]
        public ActionResult CurrencyExchange(CurrencyExchangeVm currVm)
        {
            var currentBalance = AccountingRptHelper.GetBankCashRpt().Where(a => a.AccountId == "194").FirstOrDefault();
            
            bool isvalid = true;
            if (currVm.CurrentCurrencyId == 1)
                isvalid = currVm.CurrentAmount <= currentBalance.EGP ? true : false;
            if (currVm.CurrentCurrencyId == 2)
                isvalid = currVm.CurrentAmount <= currentBalance.USD ? true : false;
            if (currVm.CurrentCurrencyId == 3)
                isvalid = currVm.CurrentAmount <= currentBalance.EUR ? true : false;
            if (currVm.CurrentCurrencyId == 4)
                isvalid = currVm.CurrentAmount <= currentBalance.GBP ? true : false;
            if (!isvalid)
                return Json("Sorry the Exchange amount must be equal or less to current balance");
            else
            {
                string result = CashMovementHelper.SaveCurrencyExchange(currVm);
                return Json(result);
            }
        }



        [HttpGet]
        public ActionResult CashToBank()
        {
            ViewBag.CashBalance = AccountingRptHelper.GetBankCashRpt().Where(a => a.AccountId == "194").ToList();
            ViewBag.BankList = BankHelper.GetAllBankAndAccountList();

            CashBankTransferVm currVm = new CashBankTransferVm();
            currVm.IsCashToBank = true;
            currVm.Notes = "Cash To Bank ";
            return View(currVm);
        }

        [HttpPost]
        public ActionResult CashToBank(CashBankTransferVm currVm)
        {
            var currentBalance = AccountingRptHelper.GetBankCashRpt().Where(a => a.AccountId == "194").FirstOrDefault();

            var bankAcc = BankHelper.GetBankAccountInfo(currVm.BankAccId);

            bool isvalid = true;

            if (bankAcc.CurrencyId == currVm.CurrentCurrencyId)
            {
                if (currVm.CurrentCurrencyId == 1)
                    isvalid = currVm.CurrentAmount <= currentBalance.EGP ? true : false;
                if (currVm.CurrentCurrencyId == 2)
                    isvalid = currVm.CurrentAmount <= currentBalance.USD ? true : false;
                if (currVm.CurrentCurrencyId == 3)
                    isvalid = currVm.CurrentAmount <= currentBalance.EUR ? true : false;
                if (currVm.CurrentCurrencyId == 4)
                    isvalid = currVm.CurrentAmount <= currentBalance.GBP ? true : false;
                if (!isvalid)
                    return Json("Sorry the Transfear amount must be equal or less to current balance");
                else
                {
                    string result = CashMovementHelper.SaveTransfer(currVm);
                    return Json(result);
                }
            }
            else
            {
                return Json("Sorry the transfear currency must be match the bank account");
            }
        }


        [HttpGet]
        public ActionResult BankToCash()
        {
            var accountRpt = AccountingRptHelper.GetBankCashRpt().Where(a => a.AccountId != "194").ToList();
            ViewBag.CashBalance = accountRpt ;
            ViewBag.BankList = BankHelper.GetAllBankAndAccountList();//.Where(a => accountRpt.Any(s=>s.AccountId == a.AccountId)).ToList();

            CashBankTransferVm currVm = new CashBankTransferVm();
            currVm.IsCashToBank = false;
            currVm.Notes = "Bank To Cash ";
            return View(currVm);
        }

        [HttpPost]
        public ActionResult BankToCash(CashBankTransferVm currVm)
        {
            var currentBalance = AccountingRptHelper.GetBankCashRpt().Where(a => a.AccountId == currVm.AccountId).FirstOrDefault();
 
            if(currentBalance == null )
                        return Json("Sorry there is no balance in the selected bank ");

            var bankAcc = BankHelper.GetBankAccountInfo(currVm.BankAccId);

            bool isvalid = true;

            if (bankAcc.CurrencyId == currVm.CurrentCurrencyId)
            {
                if (currVm.CurrentCurrencyId == 1)
                    isvalid = currVm.CurrentAmount <= currentBalance.EGP ? true : false;
                if (currVm.CurrentCurrencyId == 2)
                    isvalid = currVm.CurrentAmount <= currentBalance.USD ? true : false;
                if (currVm.CurrentCurrencyId == 3)
                    isvalid = currVm.CurrentAmount <= currentBalance.EUR ? true : false;
                if (currVm.CurrentCurrencyId == 4)
                    isvalid = currVm.CurrentAmount <= currentBalance.GBP ? true : false;
                if (!isvalid)
                    return Json("Sorry the Transfear amount must be equal or less to current balance");
                else
                {
                    string result = CashMovementHelper.SaveTransfer(currVm);
                    return Json(result);
                }
            }
            else
            {
                return Json("Sorry the transfear currency must be match the bank account");
            }
        }
    
    }
}