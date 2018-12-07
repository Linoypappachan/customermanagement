using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace SVC_CustomerManagement.Controllers
{
    //[Log]
    public class BaseApiController : ApiController
    {
        public BaseApiController()
        {
            CacheExpiry = 0.3;
        }
        public double CacheExpiry { get; set; }
    }

    public class LogAttribute : ActionFilterAttribute
    {
        public LogAttribute()
        {
        }


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new HttpException(200, "Request: /" + actionContext.Request.RequestUri));
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            int statusCode = 200;
            if (actionExecutedContext.Response != null)
            {
                statusCode = actionExecutedContext.Response.StatusCode.GetHashCode();
            }
            Elmah.ErrorSignal.FromCurrentContext().Raise(new HttpException(statusCode, "Response: /" + actionExecutedContext.Request.RequestUri));
        }
    }

    public class LowerPropertyNameAttribute : ActionFilterAttribute
    {
        public LowerPropertyNameAttribute()
        {
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
            {
                var oldObjectContent = (actionExecutedContext.ActionContext.Response.Content as ObjectContent);
                var newContent = oldObjectContent.Value;
                var result = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(newContent, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new LowercaseContractResolver() }));
                oldObjectContent.Value = result;
                actionExecutedContext.ActionContext.Response.Content = oldObjectContent;
            }
        }
    }

    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.ToString().Contains("swagger"))
            {
                return await base.SendAsync(request, cancellationToken);
            }
            else
            {
                // log request body
                string requestBody = await request.Content.ReadAsStringAsync();
                Logger.Info("REQUEST URL: " + request.RequestUri.OriginalString + Environment.NewLine + "\t" + "Request Body: " + requestBody);

                // let other handlers process the request
                var result = await base.SendAsync(request, cancellationToken);

                if (result.Content != null)
                {
                    // once response body is ready, log it
                    var responseBody = await result.Content.ReadAsStringAsync();
                    Logger.Info("RESPONSE URL: " + result.RequestMessage.RequestUri.OriginalString + Environment.NewLine + "\t" + "Response Body: " + responseBody);
                }

                return result;
            }
        }
    }
}
