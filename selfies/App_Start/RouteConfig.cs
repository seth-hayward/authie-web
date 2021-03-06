﻿using System;
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
                name: "SendRoute",
                url: "send",
                defaults: new { controller = "Home", action = "Send", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LogoutRoute",
                url: "logout",
                defaults: new { controller = "Account", action = "Logoff", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AppAboutRoute",
                url: "appabout",
                defaults: new { controller = "Home", action = "AppAbout" }
            );

            routes.MapRoute(
                name: "WhatRoute",
                url: "what",
                defaults: new { controller = "Home", action = "What" }
            );

            routes.MapRoute(
                name: "DailyRoute",
                url: "daily",
                defaults: new { controller = "Home", action = "Daily" }
            );

            routes.MapRoute(
                name: "InviteRoute",
                url: "invite",
                defaults: new { controller = "Home", action = "Invite" }
            );

            routes.MapRoute(
                name: "PrivateKeyRoute",
                url: "key",
                defaults: new { controller = "Home", action = "PrivateKey" }
            );

            routes.MapRoute(
                name: "AdminRoute",
                url: "admin",
                defaults: new { controller = "Home", action = "Admin" }
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
                name: "LoginRoute",
                url: "login",
                defaults: new { controller = "Home", action = "Login" }
            );

            routes.MapRoute(
                name: "UploadRoute",
                url: "upload/{guid}",
                defaults: new { controller = "Home", action = "UploadSnap" }
            );

            //routes.MapRoute(
            //    name: "ProfileRoute",
            //    url: "{handle}",
            //    defaults: new { controller = "Home", action = "Details" }
            //);

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