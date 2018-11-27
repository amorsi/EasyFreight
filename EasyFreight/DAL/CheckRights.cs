using EasyFreight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


    public class CheckRights : ActionFilterAttribute
    {
        ActionEnum ActionID { get; set; }
        ScreenEnum ScreenID { get; set; }

        /// <summary>
        /// Checks if the current user has a certain right on certain screen
        /// </summary>
        /// <param name="screenID">The ScreenEnum that the user is trying to access</param>
        /// <param name="actionID">The ActionEnum that the user is trying to do</param>
        public CheckRights(ScreenEnum screenID, ActionEnum actionID)
        {
            ScreenID = screenID;
            ActionID = actionID;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            bool check = true;

            check = AdminHelper.CheckUserAction(ScreenID, ActionID);

            if (!check)
            {
                var UnAuthorized = new PartialViewResult();
                UnAuthorized.ViewName = "UnAuthorized";
                filterContext.Result = UnAuthorized;
            }
        }

    }
