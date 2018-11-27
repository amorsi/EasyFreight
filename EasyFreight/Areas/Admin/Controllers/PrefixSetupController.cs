using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.Areas.Admin.DAL;

namespace EasyFreight.Areas.Admin.Controllers
{
    [Authorize]
    public class PrefixSetupController : Controller
    {
        //
        // GET: /Admin/PrefixSetup/
        [CheckRights(ScreenEnum.PrefixSetup,ActionEnum.Edit)]
        public ActionResult Index()
        {
            var prefixList = PrefixSetupHelper.GetPrefixSetupList();
            return View(prefixList);
        }

        public ActionResult AddEditSetup(FormCollection form)
        {
            string isSaved = PrefixSetupHelper.AddEditPrefix(form);
            return Json(isSaved);
        }
	}
}