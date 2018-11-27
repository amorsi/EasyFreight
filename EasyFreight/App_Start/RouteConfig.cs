using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EasyFreight
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Operation",                                           // Route name
                "Operation/{orderFrom}",                            // URL with parameters
                new { controller = "Operation", action = "Index" }  // Parameter defaults
            );

            routes.MapRoute(
                "OperationAdd",                                           // Route name
                "Operation/{orderFrom}/{action}/{id}",                            // URL with parameters
                new { controller = "Operation", action = "Add", id = UrlParameter.Optional }  // Parameter defaults
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "EasyFreight.Controllers" }
            );

        }
    }
}
