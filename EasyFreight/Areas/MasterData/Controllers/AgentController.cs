using EasyFreight.DAL;
using EasyFreight.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EasyFreight.Areas.MasterData.Controllers
{
    [Authorize]
    public class AgentController : Controller
    {
        //
        // GET: /MasterData/Agent/
        [CheckRights(ScreenEnum.Agent, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            List<AgentVm> agentList = AgentHelper.GetAgentList();
            return View(agentList);
        }


        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0) 
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Agent, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Agent, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion

            ViewData["CityList"] = ListCommonHelper.GetCityGrouped();
            AgentVm agentObj = AgentHelper.GetAgentInfo(id);
            return View(agentObj);
        }

        public ActionResult AddEditAgent(AgentVm agentVm)
        {
            string isSaved = AgentHelper.AddEditAgent(agentVm);
            return Json(isSaved);
        }

        public ActionResult GetAgentContacts(int agentId)
        {
            var contactList = AgentHelper.GetContactsList(agentId);
            return PartialView("~/Views/Shared/_ViewContacts.cshtml", contactList);
        }

        public ActionResult Delete(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.Agent, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            string isDeleted = AgentHelper.Delete(id);
            return Json(isDeleted);
        }
    }
}
