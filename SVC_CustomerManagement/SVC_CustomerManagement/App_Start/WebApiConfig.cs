using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Web.Http.ExceptionHandling;
using Elmah.Contrib.WebApi;
using SVC_CustomerManagement.Utilities;
using SVC_CustomerManagement.Controllers;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;

namespace SVC_CustomerManagement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                   name: "DefaultApi",
                   routeTemplate: "{controller}/{id}",
                   defaults: new { id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute(
            //    name: "NotFound",
            //    routeTemplate: "{*path}",
            //    defaults: new { controller = "BaseApi", action = "NotFound" }
            //);

            config.Formatters.JsonFormatter.SupportedMediaTypes
                    .Add(new MediaTypeHeaderValue("application/json"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            GlobalConfiguration.Configuration.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(new MultipartFormatterSettings()));
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new LowercaseContractResolver();

            //config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            //config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
            config.MessageHandlers.Add(new LogRequestAndResponseHandler());

        }
    }


    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLowerInvariant();
        }
    }
}
