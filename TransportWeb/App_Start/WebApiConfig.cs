using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TransportWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "ConfigurationLoad",
                routeTemplate: "api/config/load",
                defaults: new { controller = "Configuration", action = "Load" }
            );

            config.Routes.MapHttpRoute(
                name: "CreateUser",
                routeTemplate: "api/users/create",
                defaults: new { controller = "Configuration", action = "CreateUser" }
            );

            config.Routes.MapHttpRoute(
                name: "DeleteUser",
                routeTemplate: "api/users/delete",
                defaults: new { controller = "Configuration", action = "DeleteUser" }
            );
        }
    }
}
