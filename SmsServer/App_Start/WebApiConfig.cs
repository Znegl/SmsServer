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
                routeTemplate: "api/updateLocationForPost/{postid}", // /{longitude}/{lattitude}
                defaults: new { controller = "PostsApi", action = "UpdateLocationForPost" }
            );

            config.Routes.MapHttpRoute(
                name: "GetPostsOnly",
                routeTemplate: "api/postsonly/{id}",
                defaults: new { controller = "PostsApi", action = "GetOnlyPosts" }
            );

            config.Routes.MapHttpRoute(
                name: "GetAllPosts",
                routeTemplate: "api/getAllPosts/{id}",
                defaults: new { controller = "PostsApi", action = "GetAll" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
        }
    }
}
