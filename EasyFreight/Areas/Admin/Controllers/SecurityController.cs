using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.Areas.Admin.DAL;
using EasyFreight.Areas.HR.DAL;

namespace EasyFreight.Areas.Admin.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        //
        // GET: /Admin/Security/
        [CheckRights(ScreenEnum.Security, ActionEnum.SetRights)]
        public ActionResult Index()
        {
            ViewBag.EmpList = EmployeeHelper.GetEmpDic();
            return View();
        }

        [CheckRights(ScreenEnum.Security, ActionEnum.SetRights)]
        public ActionResult UsersList()
        {
            var userList = UserManagementHelper.GetUsersList();
            return View(userList);
        }

        public ActionResult GetSecRights(int empId)
        {
            string userId = UserManagementHelper.GetUserIdByEmpId(empId);
            if (string.IsNullOrEmpty(userId))
                return Content( "<p>No user is found for this employee. Must Add user first.</p>");
            var moduleList = UserManagementHelper.GetModulesList(empId);
            return PartialView("~/Areas/Admin/Views/Security/_SecRights.cshtml", moduleList);
        }

        public ActionResult AddEditSecRights(FormCollection form)
        {
            string isSaved = UserManagementHelper.AddEditSecRights(form);
            return Json(isSaved);
        }
    }
}