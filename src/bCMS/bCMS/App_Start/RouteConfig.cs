using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
                );

            routes.MapRoute(
                name: "SimpleRoute",
                url: "{article}",
                defaults: new { controller = "Router", action = "SimpleRoute", article = "" }
                );

            
            routes.MapRoute(
                name: "ComplexRoute",
                url : "{*.}",    
                defaults: new { controller = "Router", action = "ComplexRoute" }
                );

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}