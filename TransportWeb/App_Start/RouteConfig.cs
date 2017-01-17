using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TransportWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Stops",
                url: "Stops/{query}",
                defaults: new { controller = "Visitor", action = "Stops", query = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Stop",
                url: "Stop/{stop_id}/{route_id}",
                defaults: new { controller = "Visitor", action = "Stop", route_id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Transports",
                url: "Transports",
                defaults: new { controller = "Visitor", action = "Transports" }
            );

            routes.MapRoute(
                name: "Transport",
                url: "Transport/{transport_id}/{route_id}",
                defaults: new { controller = "Visitor", action = "Transport", route_id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Visitor", action = "Index" }
            );
        }
    }
}
