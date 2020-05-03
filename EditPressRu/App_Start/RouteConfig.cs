using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EditPressRu
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapMvcAttributeRoutes(); //Enables Attribute Routing

            routes.MapRoute(
               "Default3",                                           // Route name
               "products/product/{id}",                            // URL with parameters
               new { controller = "Products", action = "product", id = 0 }  // Parameter defaults

           );

            routes.MapRoute(
                "Default1",                                           // Route name
                "products/{cpu}",                            // URL with parameters
                new { controller = "Products", action = "Index",cpu=String.Empty }  // Parameter defaults
                
            );

            routes.MapRoute(
               "Default2",                                           // Route name
               "products/{cpu}/{page}",                            // URL with parameters
               new { controller = "Products", action = "Index", cpu = String.Empty,page=1 },  // Parameter defaults
               constraints: new
               {
                   page = "^[0-9]+$"
               }
           );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

           
        }
    }
}
