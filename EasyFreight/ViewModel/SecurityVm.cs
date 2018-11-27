using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class ModuleVm
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool IsSuperUser { get; set; }

        public List<ModuleScreenVm> ModuleScreens { get; set; }

        public ModuleVm()
        {
            ModuleScreens = new List<ModuleScreenVm>();
        }
    }

    public class ModuleScreenVm
    {
        public int ScreenId { get; set; }
        public int ModuleId { get; set; }
        public string ScreenName { get; set; }
        public List<ScreenActionVM> ScreenActions { get; set; }
        public List<ScreenActionDepVm> ScreenActionDeps { get; set; }
        public List<ScreenActionUserVm> ScreenActionUsers { get; set; }


        public ModuleScreenVm()
        {
            ScreenActions = new List<ScreenActionVM>();
            ScreenActionDeps = new List<ScreenActionDepVm>();
            ScreenActionUsers = new List<ScreenActionUserVm>();
        }

    }

    public class ScreenActionVM
    {
        public int ActionId { get; set; }
        public int ScreenId { get; set; }
        public string ActionName { get; set; }
        public string Selected { get; set; }
    }

    public class ScreenActionDepVm
    {
        public int ActionId { get; set; }
        public int ScreenId { get; set; }
        public int DepId { get; set; }

    }

    public class ScreenActionUserVm
    {
        public int ActionId { get; set; }
        public int ScreenId { get; set; }
        public string UserId { get; set; }

    }


    public class UserVm
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string EmpNameEn { get; set; }
        public int EmpId { get; set; }
        public string DepNameEn { get; set; } 

    }




}