using EasyFreight.Areas.HR.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;
using EasyFreight.ViewModel;

namespace EasyFreight.Areas.HR.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        //
        // GET: /HR/Employee/
        [CheckRights(ScreenEnum.Employee, ActionEnum.ViewAll)]
        public ActionResult Index()
        {
            return View();
        }

        public JObject GetTableJson(FormCollection form = null)
        {
            JObject empList = EmployeeHelper.GetEmpList(form);
            return empList;
        }

        public ActionResult Add(int id = 0)
        {
            #region Check Rights
            bool hasRights;
            if (id == 0)
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Employee, ActionEnum.Add);
            }
            else
            {
                hasRights = AdminHelper.CheckUserAction(ScreenEnum.Employee, ActionEnum.Edit);
            }

            if (!hasRights)
                return RedirectToAction("UnAuthorized", "Home", new { area = "" });

            #endregion
            ViewBag.DepartmentList = ListCommonHelper.GetDepartmentList();
            var empObj = EmployeeHelper.GetEmployeeVm(id);
            return View(empObj);
        }

        public ActionResult AddEditEmployee(EmployeeVm empVm)
        {
            string isSaved = EmployeeHelper.AddEditEmployee(empVm);
            return Json(isSaved);
        }

        public JsonResult DeleteEmployee(int id)
        {
            #region Check Rights
            bool hasRights = false;
            hasRights = AdminHelper.CheckUserAction(ScreenEnum.Employee, ActionEnum.Delete);
            if (!hasRights)
                return Json("You are UnAuthorized to do this action");

            #endregion

            string isDeleted = EmployeeHelper.DeleteEmp(id);

            return Json(isDeleted);
        }
    }
}