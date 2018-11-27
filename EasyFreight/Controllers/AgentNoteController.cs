using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using Rotativa;

namespace EasyFreight.Controllers
{
    [Authorize]
    public class AgentNoteController : Controller
    {
        //
        // GET: /AgentNote/
        [CheckRights(ScreenEnum.AgentNote, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form)
        {
            JObject quotationOrders = AgentNoteHelper.GetAgNoteListJson(form);
            return quotationOrders;
        }

        [CheckRights(ScreenEnum.AgentNote, ActionEnum.Add)]
        public ActionResult AddAgentNote(int operId, byte noteType, int agNoteId = 0)
        {
            if (noteType == 1)
                ViewBag.NoteType = "Debit";
            else
                ViewBag.NoteType = "Credit";

            ViewBag.BankList = ListCommonHelper.GetBankList();
            ViewBag.CurrencyList = ListCommonHelper.GetCurrencyList();

            var agNoteVm = AgentNoteHelper.GetAgentNoteInfo(operId, noteType, agNoteId);
            return View(agNoteVm);
        }

        public ActionResult AddEditAgentNote(AgentNoteVm agentNoteVm)
        {
            string isSaved = AgentNoteHelper.AddEditAgentNote(agentNoteVm);
            return Json(isSaved);
        }

        public ActionResult GetBankDetails(int bankId, int currencyId)
        {
            var bankDetails = BankHelper.GetBankDetByCurrency(bankId, currencyId);
            return Json(bankDetails);
        }

        public ActionResult ViewAgNotePartial(int id)
        {
            Session["AgNoteId"] = id;
            var invObj = AgentNoteHelper.GetAgentNoteInfo(0, 1, id, false);
            return PartialView("~/Views/AgentNote/_ViewAgNote.cshtml", invObj);
        }

        [AllowAnonymous]
        public ActionResult PrintNoteV(int id)
        {
            var invObj = AgentNoteHelper.GetAgentNoteInfo(0, 1, id, false);
            return View(invObj);
        }

        [CheckRights(ScreenEnum.AgentNote, ActionEnum.Print)]
        public ActionResult PrintNote(int id = 0)
        {
            if (id == 0) //Print from view modal .. get ids from session
            {
                id = (int)Session["AgNoteId"];
            }
            return new ActionAsPdf("PrintNoteV", new { id = id, name = "Giorgio" }) { FileName = "AgentNote_" + id.ToString() + ".pdf" };
        }

        public ActionResult AdvSearch()
        {
            ViewBag.AgentList = ListCommonHelper.GetAgentList();
            return PartialView("~/Views/AgentNote/_AdvSearch.cshtml");
        }

        public ActionResult DeleteAgentIn(int receiptId, int agnid, string deleteReason)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.AgentNote, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action", JsonRequestBehavior.AllowGet);

            #endregion

            string isDeleted = AgentNoteHelper.DeleteAgentInReceipt(receiptId, agnid, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteAgentOut(int receiptId, int agnid, string deleteReason)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.AgentNote, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action", JsonRequestBehavior.AllowGet);

            #endregion

            string isDeleted = AgentNoteHelper.DeleteAgentOutReceipt(receiptId, agnid, deleteReason);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }

    }
}