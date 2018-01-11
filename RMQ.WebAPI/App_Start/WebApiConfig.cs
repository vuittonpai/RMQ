using RMQ.WebAPI.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RMQ.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new AuthorizeAttribute());
            // Web API routes
            config.MapHttpAttributeRoutes();
            //config.SuppressHostPrincipal();//disable host-level authentication, 因使用 token-based authentication了
            //config.MessageHandlers.Add(new IPValidationHandler());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
