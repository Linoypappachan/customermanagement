using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SVC_CustomerManagement_Data.DataLayer;
using SVC_CustomerManagement_Domain.Models.common;
using SVC_CustomerManagement_Domain.Models.CustomerProfile;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SVC_CustomerManagement_Utilities.Helper;
using System.Dynamic;

namespace SVC_CustomerManagement.Controllers
{
    //[RoutePrefix("svc_customermanagement/rs/customer_profile")]
    [RoutePrefix("rs/customer_profile")]
    public class CustomerProfileController : BaseApiController
    {

        private CustomerProfileRSData _custProfileService;
        private CustomerPaymentRSData _payPaymentService;
        public string INFRA_URI = "http://intranet/api_infra/";
        string html = string.Empty;

        public CustomerProfileController()
        {
            _custProfileService = new CustomerProfileRSData();
            _payPaymentService = new CustomerPaymentRSData();
        }

        [Route("delinksmartpass")]
        [HttpPost]
        public HttpResponseMessage DeLinkSmartPass([FromBody]DeLinkSmartPassModel models)
        {
            try
            {
                _custProfileService.DelinkSmartpass(models.eida);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult("DELINKED"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("login")]
        [HttpPost]
        public HttpResponseMessage Login([FromBody]LoginModel model)
        {

            JObject loginDetails = new JObject();
            IEnumerable<string> headers = new List<string>();
            Request.Headers.TryGetValues("token", out headers);
            string token = headers?.First();
            IEnumerable<string> headers1 = new List<string>();
            Request.Headers.TryGetValues("x-forwarded-for", out headers1);
            string sourceAddress = headers1?.First();
            string logMsg = "CustomerProfile request is " + model.uname + ":" + model.spass + ":" + model.spassdata;
            Logger.Debug(logMsg);
            try
            {
                JObject userAuthProfileData = _custProfileService.GetRegistrationByLoginID(model.uname);
                string Msg = "CustomerProfile: db auth is " + userAuthProfileData;
                Logger.Debug(Msg);
                if (model.spass == "LOCAL")
                {
                    if (_custProfileService.IsAuthProcedureValid(userAuthProfileData, JsonConvert.DeserializeObject<JObject>(model.authProcedureJSON)))
                    {
                        loginDetails = _custProfileService.Login(model.uname, model.pwd, model.spass, sourceAddress, token);
                    }
                    else
                    {
                        return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("INVALID_CREDS"));
                    }
                }
                else
                {
                    loginDetails = _custProfileService.Login(model.uname, model.pwd, model.spass, sourceAddress, token);
                    dynamic obj = JsonConvert.DeserializeObject<JObject>(loginDetails.ToString());
                    try
                    {
                        if (model.spass != null && obj.status == "OK")
                        {
                            _custProfileService.SetSpass(loginDetails, model.spassdata);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
            return Request.CreateResponse(HttpStatusCode.OK, loginDetails);
        }

        [Route("loginlinksmartpass")]
        [HttpPost]
        public HttpResponseMessage LoginLinkSmartPass([FromBody]LoginLinkSmartPass models)
        {
            try
            {
                JObject loginlinkDetails = new JObject();
                IEnumerable<string> headers = new List<string>();
                Request.Headers.TryGetValues("token", out headers);
                string token = headers?.First();

                IEnumerable<string> headers1 = new List<string>();
                Request.Headers.TryGetValues("x-forwarded-for", out headers1);
                string sourceAddress = headers1?.First();

                JObject userAuthProfileData = _custProfileService.GetRegistrationByLoginID(models.uname);
                if (_custProfileService.IsAuthProcedureValid(userAuthProfileData, JsonConvert.DeserializeObject<JObject>(models.authProcedureJSON)))
                {
                    loginlinkDetails = _custProfileService.Loginlinksmartpass(models.uname, models.pwd, sourceAddress, models.eida, token);
                }
                if (loginlinkDetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, loginlinkDetails);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("INVALID_CREDS"));
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("{accountPKID}/cc_pay")]
        [HttpPost]
        public HttpResponseMessage PayByCreditCard(string accountPKID, [FromBody]CCPay ccpay)
        {
            try
            {
                string payStatusMessage = _payPaymentService.PayByCreditCard(accountPKID, ccpay.customerPKID, ccpay.orderNumber, ccpay.txnReference, ccpay.cardNumber, ccpay.cardType, ccpay.expiryMonth, ccpay.expiryYear, ccpay.principalAmount, ccpay.ccChargeAmount, ccpay.totalAmount);

                if (payStatusMessage == "INVALID_INPUT")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, payStatusMessage);
                }
                else if (payStatusMessage == "DECLINED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, payStatusMessage);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "APPROVED:0123456");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("{customerPKID}/info")]
        [HttpGet]
        public HttpResponseMessage Info(int customerPKID)
        {
            Customer customerDetails = null;
            Dictionary<String, Object> result = new Dictionary<string, object>();
            if (customerPKID != 0)
            {
                try
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["isDebugMode"] == "false")
                    {
                        String cacheKey = "customer_profile:" + customerPKID;
                        string json = GlobalCacheHelper.CheckCache(cacheKey);
                        if (String.IsNullOrEmpty(json))
                        {
                            customerDetails = _custProfileService.GetCustomerInfo(customerPKID);
                            result = ModifyCustInfo(customerDetails);
                            GlobalCacheHelper.AddToCache(cacheKey, result);
                        }
                        else
                        {
                            customerDetails = JsonConvert.DeserializeObject<Customer>(JsonConvert.DeserializeObject<JArray>(json)[0].ToString());
                            result = ModifyCustInfo(customerDetails);
                        }
                    }
                    else
                    {
                        customerDetails = _custProfileService.GetCustomerInfo(customerPKID);
                        result = ModifyCustInfo(customerDetails);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_INPUT"));
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private static Dictionary<string, object> ModifyCustInfo(Customer customerDetails)
        {
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                { "pkid", customerDetails.pkid },
                { "customerName", customerDetails.customerName },
                { "personalProfile", customerDetails.personalProfile },
                { "addressProfile", customerDetails.addressProfile },
                { "businessFunctionProfile", customerDetails.businessFunctionProfile },
                { "customerNumber", customerDetails.customerNumber },
                { "customerType", customerDetails.customerType },
                { "accountProfile", customerDetails.accountProfile }
            };
            if (customerDetails.customerLinks != null)
            {
                result.Add("customerLinks", customerDetails.customerLinks);
            }
            return result;
        }

        [Route("{customerPKID}/logout")]
        [HttpPost]
        public HttpResponseMessage LogOut(int customerPKID, [FromBody]LogOut logOut)
        {
            try
            {
                string logMsg = "CustomerProfileRS:Logout: " + customerPKID + ":" + logOut.uname;
                Logger.Debug(logMsg);
                string sURL = INFRA_URI + "cachedb/clear?context_s=SPASS_LOGIN&key_s=_SPASS_" + customerPKID;
                WebRequest request = WebRequest.Create(sURL);
                request.Method = "DELETE";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            try
            {
                JObject logoutDetails = new JObject();
                IEnumerable<string> headers = new List<string>();
                Request.Headers.TryGetValues("token", out headers);
                string token = headers != null ? headers.First() : string.Empty;
                IEnumerable<string> headers1 = new List<string>();
                Request.Headers.TryGetValues("x-forwarded-for", out headers1);
                string sourceAddress = headers1?.First();
                logoutDetails = _custProfileService.Logout(customerPKID, logOut.uname, sourceAddress, token);
                return Request.CreateResponse(HttpStatusCode.OK, logoutDetails);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("{customerPKID}/spass")]
        [HttpGet]
        public HttpResponseMessage GetCustomerSmartpassByPKID(int customerPKID)
        {
            try
            {
                Dictionary<string, object> list = new Dictionary<string, object>();
                string jsonString = string.Empty;
                if (customerPKID != 0)
                {
                    var request = (HttpWebRequest)WebRequest.Create(INFRA_URI + "cachedb/browse?context_s=SPASS_LOGIN&key_s=_SPASS_" + customerPKID);
                    request.Method = "GET";
                    request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    request.Headers.Set("charset", "UTF-8");
                    var response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
                    if (html != null)
                    {
                        List<string> dList = JsonConvert.DeserializeObject<List<string>>(html);
                        if (dList != null && dList.Count > 0)
                        {
                            jsonString = dList[0];
                            list = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                            list.Remove("authorities");
                            list.Remove("password");
                            list.Remove("accountNonExpired");
                            list.Remove("accountNonLocked");
                            list.Remove("credentialsNonExpired");
                            list.Remove("enabled");
                            list.Remove("username");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorStatusResult("NO_DATA"));
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorStatusResult("NO_DATA"));
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, list);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_INPUT"));
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("{customerPKID}/update_password")]
        [HttpPost]
        public HttpResponseMessage UpdatePassword(int customerPKID, [FromBody]UpdatePassword UpdatePassword)
        {
            IEnumerable<string> headers = new List<string>();
            Request.Headers.TryGetValues("x-forwarded-for", out headers);
            string sourceAddress = headers?.First();
            try
            {
                JObject updatedpassword = _custProfileService.UpdatePassword(customerPKID, UpdatePassword.uname, UpdatePassword.oldpwd, UpdatePassword.newpwd, sourceAddress);
                return Request.CreateResponse(HttpStatusCode.OK, updatedpassword);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("{customerPKID}/update_profile")]
        [HttpPost]
        public HttpResponseMessage UpdateProfile(int customerPKID, [FromBody]UpdateProfile UpdateProfile)
        {
            String cacheKey = "customer_profile:" + customerPKID;
            GlobalCacheHelper.ClearCache(cacheKey);
            IEnumerable<string> headers = new List<string>();
            Request.Headers.TryGetValues("token", out headers);
            string profile = UpdateProfile.profileJSON;
            JObject result = new JObject();
            try
            {
                result = _custProfileService.UpdateProfile(customerPKID, profile);
                dynamic obj = JsonConvert.DeserializeObject<JObject>(result.ToString());
                if (obj.status == "ERROR")
                {
                    result.Add("status", "ERROR");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("pobox_details/{emirates_id}/emiratesid")]
        [HttpGet]
        public HttpResponseMessage GetCustomerEmirates(string emirates_id)
        {
            var result = new List<CustomerPoboxEDIADetailsModel>();
            try
            {
                result = _custProfileService.GetCustomerInfoByEIDA(emirates_id);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }
    }
}
