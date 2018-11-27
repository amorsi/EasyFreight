using EasyFreight.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNet.Identity;

namespace EasyFreight.DAL
{
    public static class AdminHelper
    {
        public static string GeneratePrefixCode(PrefixForEnum prefixFor , bool forSubmit)
        {
            StringBuilder prefixCode = new StringBuilder();
            SetupEntities db = new SetupEntities();

            int prefixForId = (int)prefixFor;

            PrefixSetup prefixSetup = db.PrefixSetups.Include("PrefixLastId")
                .Where(x => x.PrefixForId == prefixForId).FirstOrDefault();
            //Take the Id
            PrefixLastId prefixLastIdObj = prefixSetup.PrefixLastId;

            string currentId = prefixLastIdObj.LastId.ToString();
            //Reset Id every 
            DateTime lastUpdateDate = prefixLastIdObj.LastUpdateDate.Value;

            if (prefixSetup.ResetNumberEvery == "month")
            {
                //If reset is active .. must include month and year .. even if it was wrong inseted in the setup 
                // to avoid duplicate codes 
                prefixSetup.IncludeMonth = true;
                prefixSetup.IncludeYear = true;

                if (lastUpdateDate.Month != DateTime.Now.Month) // New record in new month .. reset the ID
                    currentId = "1";
            }
            if (prefixSetup.ResetNumberEvery == "year")
            {
                //If reset is active .. must include year .. even if it was wrong inseted in the setup 
                // to avoid duplicate codes 
             //   prefixSetup.IncludeMonth = true;
                prefixSetup.IncludeYear = true;

                if (lastUpdateDate.Year != DateTime.Now.Year) // New record in new year .. reset the ID
                    currentId = "1";
            }

            if (!forSubmit)
                currentId = "??";
            //build the prefix
            if (prefixSetup.NumberAfterChar.Value)
            {
                prefixCode.Append(prefixSetup.PrefixChar);
                prefixCode.Append(currentId);
            }
            else
            {
                prefixCode.Append(currentId);
                prefixCode.Append(prefixSetup.PrefixChar);
            }

            if(prefixSetup.IncludeMonth.Value)
            {
                prefixCode.Append(prefixSetup.Delimiter);
                prefixCode.Append(DateTime.Now.Month);
            }

            if (prefixSetup.IncludeYear.Value)
            {
                prefixCode.Append(prefixSetup.Delimiter);
                prefixCode.Append(DateTime.Now.Year);
            }

            if (forSubmit)
            {
                
                if (prefixSetup.ResetNumberEvery == "never")
                {
                    //update lastId = lastId +1
                    prefixLastIdObj.LastId = prefixLastIdObj.LastId + 1;
                }
                else if (prefixSetup.ResetNumberEvery == "month")
                {
                    if (lastUpdateDate.Month != DateTime.Now.Month) // New record in new month .. reset the ID
                        prefixLastIdObj.LastId = 2; // one is alrady taken ..static codeing above
                    else // reset monthly but already Id rested by anoth record
                        prefixLastIdObj.LastId = prefixLastIdObj.LastId + 1;
                }
                if (prefixSetup.ResetNumberEvery == "year")
                {
                    if (lastUpdateDate.Year != DateTime.Now.Year) // New record in new year .. reset the ID
                        prefixLastIdObj.LastId = 2; // one is alrady taken ..static codeing above
                    else // reset yearly but already Id rested by anoth record
                        prefixLastIdObj.LastId = prefixLastIdObj.LastId + 1;
                }

                prefixLastIdObj.LastUpdateDate = DateTime.Now;

                try
                {
                    db.SaveChanges();
                    
                }
                catch {
                    throw;
                }
            }

            

            return prefixCode.ToString();

        }

        /// <summary>
        /// Will call this in case of insert new record is falied
        /// </summary>
        /// <param name="prefixFor">Enum PrefixFor</param>
        public static void LastIdRemoveOne(PrefixForEnum prefixFor)
        {
            SetupEntities db = new SetupEntities();
            int prefixForId = (int)prefixFor;
            var lastIndexObj = db.PrefixLastIds.Where(x => x.PrefixForId == prefixForId).FirstOrDefault();
            lastIndexObj.LastId = lastIndexObj.LastId - 1;
            db.SaveChanges();
        }

        public static bool CheckUserAction(ScreenEnum screen, ActionEnum action)
        {
            bool hasRights = false;
            SetupEntities db = new SetupEntities();
            string userId = HttpContext.Current.User.Identity.GetUserId();
            int screenId = (int)screen;
            int actionId = (int)action;

            hasRights = db.ScreenActionUsers
                .Any(x => x.ScreenId == screenId && x.ActionId == actionId && x.UserId == userId);

            return hasRights;
        }

        public static string GetCurrentUserId()
        {
            return HttpContext.Current.User.Identity.GetUserId();
        }
     
    }
}