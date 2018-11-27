using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Areas.HR.Models;
using EasyFreight.ViewModel;
using AutoMapper;
using System.Data.Entity.Validation;

namespace EasyFreight.Areas.HR.DAL
{
    public static class EmployeeHelper
    {
        
        public static JObject GetEmpList(System.Web.Mvc.FormCollection form)
        {
            HREntities db = new HREntities();
            var empListDb = db.Employees.Include("Department").OrderBy(x => x.EmpNameEn).ToList();
            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in empListDb)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("EmpId");
                pJTokenWriter.WriteValue(item.EmpId);

                pJTokenWriter.WritePropertyName("EmpCode");
                pJTokenWriter.WriteValue(item.EmpCode);

                pJTokenWriter.WritePropertyName("EmpNameEn");
                pJTokenWriter.WriteValue(item.EmpNameEn);

                pJTokenWriter.WritePropertyName("EmpNameAr");
                pJTokenWriter.WriteValue(item.EmpNameAr);

                pJTokenWriter.WritePropertyName("DepNameEn");
                pJTokenWriter.WriteValue(item.Department.DepNameEn);

                pJTokenWriter.WriteEndObject();
            }
            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static EmployeeVm GetEmployeeVm(int empId = 0)
        {
            EmployeeVm empVm = new EmployeeVm();
            if (empId != 0)
            {
                HREntities db = new HREntities();
                Employee empDb = db.Employees.Where(x => x.EmpId == empId).FirstOrDefault();
                Mapper.CreateMap<Employee, EmployeeVm>().IgnoreAllNonExisting();
                Mapper.Map(empDb, empVm);
            }

            return empVm;
        }

        public static string AddEditEmployee(EmployeeVm empVm)
        {
            string isSaved = "true";
            HREntities db = new HREntities();
            int empId = empVm.EmpId;
            Employee empDb;
            if (empId == 0)
                empDb = new Employee();
            else
                empDb = db.Employees.Where(x => x.EmpId == empId).FirstOrDefault();

            Mapper.CreateMap<EmployeeVm, Employee>().IgnoreAllNonExisting();
            Mapper.Map(empVm, empDb);

            if (empId == 0)
                db.Employees.Add(empDb);

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.InnerException;
            }


            return isSaved;
        }

        public static Dictionary<int, string> GetEmpDic()
        {
            HREntities db = new HREntities();
            var empList = db.Employees
                .Select(x => new { x.EmpId, x.EmpNameEn })
                .OrderBy(x => x.EmpNameEn).ToList();
            Dictionary<int, string> empDic = empList.ToDictionary(x => x.EmpId, x => x.EmpNameEn);
            return empDic;
        }

        internal static string DeleteEmp(int id)
        {
            HREntities db = new HREntities();
            Employee empDb = db.Employees.Where(x=>x.EmpId == id).FirstOrDefault();
            string isDeleted = "true";

            db.Employees.Remove(empDb);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                if ((ex.InnerException).InnerException.Message.Contains("DELETE statement conflicted"))
                    isDeleted = "false. Can't delete this row as it is linked with some operation orders";
                else
                    isDeleted = "false" + ex.Message;
            }

            return isDeleted;
        }
    }
}