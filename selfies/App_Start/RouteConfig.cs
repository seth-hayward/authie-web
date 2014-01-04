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
                name: "WhatRoute",
                url: "what",
                defaults: new { controller = "Home", action = "What" }
            );

            routes.MapRoute(
                name: "ContactsRoute",
                url: "contacts",
                defaults: new { controller = "Home", action = "Contacts" }
            );

            routes.MapRoute(
                name: "RegisterRoute",
                url: "register",
                defaults: new { controller = "Home", action = "Register" }
            );

            routes.MapRoute(
                name: "UploadRoute",
                url: "upload/{guid}",
                defaults: new { controller = "Home", action = "UploadSnap" }
            );

            routes.MapRoute(
                name: "ProfileRoute",
                url: "{handle}",
                defaults: new { controller = "Home", action = "Details" }
            );

            routes.MapRoute(
                name: "ThreadDetailRoute",
                url: "{handle}/{key}",
                defaults: new { controller = "Home", action = "Thread" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}