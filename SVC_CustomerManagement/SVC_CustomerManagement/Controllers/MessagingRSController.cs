using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SVC_CustomerManagement_Data.DataLayer;
using SVC_CustomerManagement_Domain.Models.common;
using SVC_CustomerManagement_Domain.Models.Messaging;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SVC_CustomerManagement.Controllers
{
    //[RoutePrefix("svc_customermanagement/rs/messaging")]
    [RoutePrefix("rs/messaging")]
    public class MessagingRSController : ApiController
    {
        private MessagingRSData _messService;
        public MessagingRSController()
        {
            _messService = new MessagingRSData();
        }

        [Route("{customerPKID}/send_email")]
        [HttpPost]
        public HttpResponseMessage SendEmail(int customerPKID, [FromBody]SendEmailModel model)
        {
            try
            {
                JObject sendEmailData = _messService.SendEmail(customerPKID, model.bfunctionPKID, model.recepientList, model.subject, model.body);
                if (sendEmailData != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, sendEmailData);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("Failed"));
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("{customerPKID}/send_email_with_attachments")]
        [HttpPost]
        public HttpResponseMessage SendEmailWithAttachments(int customerPKID, [FromBody]SendEmailWithAttachmentsModel model)
        {
            try
            {

                JObject sendEmailData = _messService.SendEmail(customerPKID, model.bfunctionPKID, model.recepientList, model.subject, model.body);
                if (sendEmailData != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, sendEmailData);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("Failed"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("{customerPKID}/send_sms")]
        [HttpPost]
        public HttpResponseMessage SendSms(int customerPKID, [FromBody]SendSMSModel model)
        {
            try
            {
                JObject json = new JObject();
                if (model.recepientList.Contains("552369193"))
                {
                    json = _messService.SendSms(customerPKID, model.bfunctionPKID, model.recepientList, model.message);
                }
                else
                {
                    json.Add("error", "INVALID_RECEPIENT");
                }
                return Request.CreateResponse(HttpStatusCode.OK, json);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }
    }

}

