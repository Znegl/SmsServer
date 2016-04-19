using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SmsServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "IncomingSms",
            //    routeTemplate: "api/sms",
            //    defaults: new { controller = "Sms" }
            //    );

            config.Routes.MapHttpRoute(
                name: "UpdatedLocation",
                routeTemplate: "api/updateLocationForPost",
                defaults: new { controller = "PostsApi", action = "UpdateLocationForPost" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
