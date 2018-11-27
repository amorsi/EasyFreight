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
    public class AgentReportController : Controller
    {
        //
        // GET: /AgentReport/

        public ActionResult Index(int countryId =0)
        {
           // List<AgentVm> agentList = AgentHelper.GetAgentListByCountry(countryId);
            ViewBag.CountryList = ListCommonHelper.GetCountryList();
            return View();//agentList
        }

        public JObject GetTableJson(int countryId = 0)
        {
            var agentList = AgentHelper.GetAllAgentsByCountry(countryId);
            return agentList;
        }

        public ActionResult GetAgentPrint(int id = 0)
        { 
            List<AgentVm> agentList = AgentHelper.GetAgentListByCountry( id  );
            return View(agentList);
        }

        public ActionResult PrintAgents(int countryId = 0)
        {
             return new ActionAsPdf("GetAgentPrint", new { id = countryId, name = "Giorgio" }) { FileName = "AgentReport.pdf" };
        }

    }
}
