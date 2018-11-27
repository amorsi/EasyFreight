using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Areas.Admin.Models;
using System.Web.Mvc;
using System.Text;

namespace EasyFreight.Areas.Admin.DAL
{
    public static class PrefixSetupHelper
    {
        public static List<PrefixSetup> GetPrefixSetupList()
        { 
            SetupEntities db = new SetupEntities();
            var prefixSetupList = db.PrefixSetups.ToList();
            return prefixSetupList;
        }

        public static string AddEditPrefix(FormCollection form)
        {
            string isSaved = "true";
            SetupEntities db = new SetupEntities();
            var count = db.PrefixSetups.Count();
            PrefixSetup prefixSetupDb;
            for (int i = 0; i < count-1; i++)
            {
                //prefix for Id
                int prefixForId = int.Parse(form["PrefixSetup[" + i + "].PrefixForId"]);
                prefixSetupDb = db.PrefixSetups.Where(x => x.PrefixForId == prefixForId).FirstOrDefault();
                prefixSetupDb.Delimiter = form["PrefixSetup[" + i + "].Delimiter"];
                prefixSetupDb.IncludeMonth = bool.Parse(form["PrefixSetup[" + i + "].IncludeMonth"]);
                prefixSetupDb.IncludeYear = bool.Parse(form["PrefixSetup[" + i + "].IncludeYear"]);
                prefixSetupDb.NumberAfterChar = bool.Parse(form["PrefixSetup[" + i + "].NumberAfterChar"]);
                prefixSetupDb.PrefixChar = form["PrefixSetup[" + i + "].PrefixChar"];
                prefixSetupDb.ResetNumberEvery = form["PrefixSetup[" + i + "].ResetNumberEvery"];

            }

            db.SaveChanges();

            return isSaved;
        }




    }
}

