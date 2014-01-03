using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace selfies
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "StartThreadRoute",
                url: "threads/start",
                defaults: new { controller = "Home", action = "StartThread", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "LogoutRoute",
                url: "logout",
                defaults: new { controller = "Account", action = "Logoff", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}