using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Areas.Admin.Models;
using EasyFreight.Areas.HR.Models;
using EasyFreight.ViewModel;
using AutoMapper;

namespace EasyFreight.Areas.Admin.DAL
{
    public static class UserManagementHelper
    {
        public static string AssignUserIdToEmpId(int empId, string userId)
        {
            string isSaved = "true";
            SetupEntities db = new SetupEntities();
            UserEmployee userEmp = new UserEmployee()
            {
                EmpId = empId,
                UserId = userId
            };

            db.UserEmployees.Add(userEmp);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
                throw;
            }

            return isSaved;
        }

        public static List<ModuleVm> GetModulesList(int empId)
        {
            List<ModuleVm> moduleVmList = new List<ModuleVm>();
            SetupEntities db = new SetupEntities();
            var actionsList = db.SecActionLibs.ToList();
            var moduleDbList = db.Modules.Include("ModuleScreens").ToList();
            Mapper.CreateMap<Module, ModuleVm>().IgnoreAllNonExisting();
            Mapper.CreateMap<ModuleScreen, ModuleScreenVm>().IgnoreAllNonExisting();
            Mapper.CreateMap<ScreenAction, ScreenActionVM>().IgnoreAllNonExisting();
            Mapper.CreateMap<ScreenActionUser, ScreenActionUserVm>().IgnoreAllNonExisting();

            Mapper.Map(moduleDbList, moduleVmList);

            string userId = GetUserIdByEmpId(empId);

            int actionId, screenId;
            foreach (var moduleVmObj in moduleVmList)
            {
                foreach (var screenVmObj in moduleVmObj.ModuleScreens)
                {
                    screenId = screenVmObj.ScreenId;
                    foreach (var action in screenVmObj.ScreenActions)
                    {
                        actionId = action.ActionId;
                        action.ActionName = actionsList.Where(x => x.ActionId == actionId).FirstOrDefault().ActionName;
                        action.Selected = screenVmObj.ScreenActionUsers
                            .Any(x => x.ActionId == actionId && x.UserId == userId && x.ScreenId == screenId) ? "selected" : "";
                    }
                }
            }

            return moduleVmList;
        }

        public static string GetUserIdByEmpId(int empId)
        {
            SetupEntities db = new SetupEntities();
            string userId = "";
            var userEmpObj = db.UserEmployees.Where(x => x.EmpId == empId).FirstOrDefault();
            if (userEmpObj != null)
                userId = userEmpObj.UserId;

            return userId;
        }

        internal static string AddEditSecRights(System.Web.Mvc.FormCollection form)
        {
            string isSaved = "true";
            int empId = int.Parse(form["EmpId"]);
            string userId = GetUserIdByEmpId(empId);
            //Will be added into AspNetUserRoles to allow edit for closde operations
            string isSuperUser = form["IsSuperUser"];

            SetupEntities db = new SetupEntities();
            ScreenActionUser actionUser;
            foreach (var key in form.AllKeys)
            {
                if (key.StartsWith("ActionId"))
                {
                    string index = key.Split('[').ToArray()[1].Replace("]", "");
                    int screenId = int.Parse(form["ScreenId[" + index + "]"]);
                    var userActionsDb = db.ScreenActionUsers.Where(x => x.UserId == userId && x.ScreenId == screenId).ToList();
                    foreach (var item in userActionsDb)
                    {
                        db.ScreenActionUsers.Remove(item);
                    }
                    string actionId = form[key];
                    if (actionId.Contains(","))
                    {
                        string[] actionIds = actionId.Split(',').ToArray();
                        for (int i = 0; i < actionIds.Length; i++)
                        {
                            actionUser = new ScreenActionUser()
                            {
                                ActionId = int.Parse(actionIds[i]),
                                ScreenId = screenId,
                                UserId = userId
                            };
                            db.ScreenActionUsers.Add(actionUser);
                        }
                    }
                    else
                    {
                        actionUser = new ScreenActionUser()
                        {
                            ActionId = int.Parse(actionId),
                            ScreenId = screenId,
                            UserId = userId
                        };
                        db.ScreenActionUsers.Add(actionUser);
                    }


                }
            }

            //Check if added as super user before
            bool isExists = db.AspNetUserRoles.Any(x => x.UserId == userId && x.RoleId == "1");

            if (!isExists && isSuperUser.ToLower() == "true")
            {
                AspNetUserRole aspNetUserRole = new AspNetUserRole();
                aspNetUserRole.RoleId = "1";
                aspNetUserRole.UserId = userId;
                db.AspNetUserRoles.Add(aspNetUserRole);
            }
            else if (isExists && isSuperUser.ToLower() == "false") // removed from super user role
            {
                var aspNetUserRole = db.AspNetUserRoles.Where(x => x.UserId == userId && x.RoleId == "1").FirstOrDefault();
                db.AspNetUserRoles.Remove(aspNetUserRole);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false " + ex.Message;
            }

            return isSaved;
        }

        internal static List<UserVm> GetUsersList()
        {
            SetupEntities db = new SetupEntities();
            List<UserVm> userListVm = new List<UserVm>();
            var userListDb = db.AspNetUsers.ToList();
            var userEmps = db.UserEmployees.ToList();

            Mapper.CreateMap<AspNetUser, UserVm>().IgnoreAllNonExisting();
            Mapper.Map(userListDb, userListVm);

            HREntities db1 = new HREntities();
            List<Employee> empList = db1.Employees.Include("Department").ToList();

            int empId;
            Employee emp;
            foreach (var item in userListVm)
            {
                empId = userEmps.Where(x => x.UserId == item.Id).FirstOrDefault().EmpId;
                emp = empList.Where(x => x.EmpId == empId).FirstOrDefault();
                item.EmpId = empId;
                item.EmpNameEn = emp.EmpNameEn;
                item.DepNameEn = emp.Department.DepNameEn;
            }

            return userListVm;
        }

        public static bool IsSuperUser(string userId)
        {
            SetupEntities db = new SetupEntities();
            bool isExists = db.AspNetUserRoles.Any(x => x.UserId == userId && x.RoleId == "1");
            return isExists;
        }
    }
}