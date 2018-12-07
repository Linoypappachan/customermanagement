using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SVC_CustomerManagement_Data.DataLayer;
using SVC_CustomerManagement_Domain.Models.common;
using SVC_CustomerManagement_Domain.Models.CustomerAdmin;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System.Transactions;
using SVC_CustomerManagement_Domain.Models;

namespace SVC_CustomerManagement.Controllers
{
    //[RoutePrefix("svc_customermanagement/rs/admin")]
    [RoutePrefix("rs/admin")]
    public class CustomerAdminRSController : ApiController
    {
        private CustomerAdminRSData _custAdminService;

        public CustomerAdminRSController()
        {
            _custAdminService = new CustomerAdminRSData();
        }

        [Route("create_login")]
        [HttpPost]
        public HttpResponseMessage CreateLoginAccount([FromBody]CreateLoginAccountModel models)
        {
            try
            {
                JObject createLoginAccountValue;
                createLoginAccountValue = _custAdminService.CreateLoginAccount(models.customerPKID, models.uname, models.pwd);
                if (createLoginAccountValue.Count != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, createLoginAccountValue);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("Cannot Activate Account"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }


        [Route("customer_registration/{regReference}")]
        [HttpGet]
        //[LowerPropertyName]
        public HttpResponseMessage GetCustomerRegistration(string regReference)
        {
            string msg = "CustomerAdminRS:Registration Inquiry for " + regReference;
            Logger.Debug(msg);
            try
            {
                CustomerRegistrationResponse regRefDetails = _custAdminService.GetRegistration(regReference);
                if (regRefDetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, regRefDetails);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }


        [Route("customer_registration/loginID/{loginID}")]
        [HttpGet]
        [LowerPropertyName]
        public HttpResponseMessage GetCustomerRegistrationByLoginID(string loginID)
        {
            string msg = "CustomerAdminRS:Registration Inquiry for " + loginID;
            Logger.Debug(msg);
            try
            {
                CustomerRegistrationResponse reglogDetails = _custAdminService.GetRegistrationByLoginID(loginID);
                if (reglogDetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, reglogDetails);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("is_uname_available")]
        [HttpGet]
        public HttpResponseMessage IsLoginIDAvailable()
        {
            string uname = Request.Headers.GetValues("uname").FirstOrDefault();
            try
            {
                JObject islogidavail;
                islogidavail = _custAdminService.IsLoginIdAvailable(uname);
                if (islogidavail != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, islogidavail);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("Login ID Not Available"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("is_pbox_customervalid")]
        [HttpGet]
        public HttpResponseMessage IsPostBoxCustomerValid()
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("city");
            string city = headerValues.FirstOrDefault();
            IEnumerable<string> headerValues1 = Request.Headers.GetValues("boxNumber");
            string boxNumber = headerValues1.FirstOrDefault();
            IEnumerable<string> headerValues2 = Request.Headers.GetValues("verification");
            string verification = headerValues2.FirstOrDefault();
            string logMsg = "isPostBoxCustomerValid :" + city + ":" + boxNumber + ":" + verification;
            Logger.Debug(logMsg);
            try
            {
                bool ispostboxcustvalid = false;
                ispostboxcustvalid = _custAdminService.IsPostBoxCustomerValid(city, boxNumber, verification);
                string logMsg1 = "isPostBoxCustomerValid json:" + ispostboxcustvalid;
                Logger.Debug(logMsg1);
                return Request.CreateResponse(HttpStatusCode.OK, ispostboxcustvalid);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("is_eida_valid")]
        [HttpGet]
        public HttpResponseMessage IsEIDAValid()
        {

            IEnumerable<string> headerValues = Request.Headers.GetValues("eida");
            string eida = headerValues.FirstOrDefault();

            string logMsg = "isEIDAValid:" + eida;
            Logger.Debug(logMsg);
            try
            {
                bool iseidavalid = false;
                iseidavalid = _custAdminService.IsEIDAValid(eida);

                string logMsg1 = "isEIDAValid json:" + iseidavalid;
                Logger.Debug(logMsg1);
                return Request.CreateResponse(HttpStatusCode.OK, iseidavalid);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("is_accountvalid")]
        [HttpGet]
        public HttpResponseMessage IsUserAccountValid(string floginName, string femail)
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("loginname");
            string loginname = headerValues.FirstOrDefault();
            IEnumerable<string> headerValues1 = Request.Headers.GetValues("email");
            string email = headerValues1.FirstOrDefault();
            try
            {
                bool isUserAcctValid = false;
                if (floginName != null)
                {
                    loginname = floginName;
                    email = femail;
                }

                isUserAcctValid = _custAdminService.IsUserAccountValid(loginname, email);
                return Request.CreateResponse(HttpStatusCode.OK, isUserAcctValid);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }

        [Route("smartpass/account/{smartpassid}")]
        [HttpGet]
        [LowerPropertyName]
        public HttpResponseMessage GetAccountBySmartpassID(string smartpassid)
        {
            try
            {
                Object accountData = _custAdminService.GetAccountBySmartpassID(smartpassid);
                if (accountData != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, accountData);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("is_loggedin")]
        [HttpGet]
        public HttpResponseMessage IsLoggedIn(int customerPKID)
        {

            try
            {
                JObject isLoggedIn;
                IEnumerable<string> headers = new List<string>();
                Request.Headers.TryGetValues("token", out headers);
                string token = headers?.First();
                isLoggedIn = _custAdminService.IsLoggedInCurrently(token, customerPKID);
                if (isLoggedIn.Count != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, isLoggedIn);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("Has not Logged In"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("register_customer")]
        [HttpPost]
        public HttpResponseMessage RegisterCustomer([FromBody]RegisterCustomerModel model)
        {
            JObject islogidavail;
            JObject result = new JObject();
            try
            {
                JObject validateQAMap = new JObject
                {
                    { "loginID", model.loginID },
                    { "fName", model.fName },
                    { "lName", model.lName },
                    { "emailID", model.emailID },
                    { "regForm", model.regForm }
                };
                islogidavail = _custAdminService.IsLoginIdAvailable(model.loginID);
                dynamic objlogidavail = JsonConvert.DeserializeObject<JObject>(islogidavail.ToString());
                using (var transactionScope = new TransactionScope())
                {
                    if (objlogidavail.status != "AVAILABLE")
                    {
                        result.Add("status", "ERROR");
                        result.Add("error", "REG_LOGINNAME_UNAVAILABLE");
                    }
                    else
                    {
                        Dictionary<string, string> response = _custAdminService.RegisterFunction(validateQAMap, model);
                        int customerAccountPKID = Convert.ToInt32(response.Where(c => c.Key == "customerAccountPKID").Select(c => c.Value).FirstOrDefault());
                        string registerCustomer = response.Where(c => c.Key == "registerCustomer").Select(c => c.Value).FirstOrDefault();
                        if (customerAccountPKID == -1)
                        {
                            result.Add("status", "ERROR");
                            result.Add("error", "REG_LOGINNAME_UNAVAILABLE");
                        }
                        else
                        {
                            if (System.Configuration.ConfigurationManager.AppSettings["isDebugMode"] == "false")
                            {
                                _custAdminService.EmailCredentials(customerAccountPKID, model.emailID);
                            }
                            result.Add("reference", registerCustomer);
                            result.Add("status", "OK");
                        }
                    }
                    transactionScope.Complete();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("complete_registration")]
        [HttpPost]
        public HttpResponseMessage CompleteRegistration([FromBody]CompleteRegistrationModel model)
        {
            string accountPKID = null;
            string bfunctionPKID = null;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    var registration = _custAdminService.GetRegistration(model.regReference);
                    string registrationForm = registration.reg_form;
                    int customerAccountPKID = Int32.Parse(registration.customer_pkid.ToString());
                    dynamic obj = JsonConvert.DeserializeObject<JObject>(registrationForm);
                    accountPKID = obj.accountPKID;
                    bfunctionPKID = obj.bfunctionPKID;

                    if (customerAccountPKID == 0 && accountPKID != null)
                    {
                        customerAccountPKID = Convert.ToInt32(accountPKID);
                    }
                    string logMsg = "CustomerAdminRS:" + bfunctionPKID + ":" + customerAccountPKID + ":" + registration.fname;
                    Logger.Debug(logMsg);

                    if (customerAccountPKID == 0 && bfunctionPKID == "0")
                    {
                        customerAccountPKID = _custAdminService.CreatePostBoxAccount(obj.pboxcity, obj.pboxnumber, registration.login_id, registration.fname, registration.lname);

                    }
                    string loginid = registration.login_id;
                    string emailid = registration.email_id;

                    string logMsg1 = "CustomerAdminRS:Completing registration with login id" + loginid;
                    Logger.Debug(logMsg1);

                    _custAdminService.CompleteRegistration(model.regReference, loginid, customerAccountPKID, model.authProcedureJSON, emailid);

                    string logMsg2 = "CustomerAdminRS :customerAccountPKID:" + customerAccountPKID;
                    Logger.Debug(logMsg2);

                    _custAdminService.EmailCredentials(customerAccountPKID, model.regReference);
                    transactionScope.Complete();
                }
                return Request.CreateResponse(HttpStatusCode.OK, "{\"status\" : \"OK\"}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }

        [Route("create_pbox_account")]
        [HttpPost]
        public HttpResponseMessage CreatePostBoxAccount([FromBody]CreatePostBoxAccountModel model)
        {
            try
            {
                int createPostBoxAccountDet;
                using (var transactionScope = new TransactionScope())
                {
                    createPostBoxAccountDet = _custAdminService.CreatePostBoxAccount(model.city, model.number, model.loginid, model.fname, model.lname);
                    transactionScope.Complete();
                }
                if (createPostBoxAccountDet != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, createPostBoxAccountDet);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("ERROR"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("import_servicecustomer")]
        [HttpPost]
        public HttpResponseMessage ImportServiceCustomer([FromBody]ImportServiceCustomerModel model)
        {
            try
            {
                int importServiceCustomerDet = _custAdminService.CreateCustomer(model.bfunctionPKID, model.serviceCustomerID);
                if (importServiceCustomerDet != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, importServiceCustomerDet);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }

        [Route("attach_to_loginaccount")]
        [HttpPost]
        public HttpResponseMessage AttachToLoginAccount([FromBody]AttachtoLoginAccountModel model)
        {
            try
            {
                JObject attachToLoginAccountDet = _custAdminService.AttachToLoginAccount(model.accountCustomerPKID, model.serviceCustomerPKID, model.bfunctionPKID);
                return Request.CreateResponse(HttpStatusCode.OK, attachToLoginAccountDet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("attach_pobox_to_loginaccount")]
        [HttpPost]
        public HttpResponseMessage AttachPOBoxToLoginAccount([FromBody]AttachpoboxtoLoginAccountModel model)
        {
            try
            {
                bool attachPOBoxToLoginAccountDet;
                using (var transactionscope = new TransactionScope())
                {
                    attachPOBoxToLoginAccountDet = _custAdminService.AttachPoboxToLoginAccount(model.accountCustomerPKID, model.city, model.boxNumber);
                    transactionscope.Complete();
                }
                return Request.CreateResponse(HttpStatusCode.OK, attachPOBoxToLoginAccountDet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("dettach_pobox_from_loginaccount")]
        [HttpPost]
        public HttpResponseMessage DettachPOBoxFromLoginAccount([FromBody]DettachPOBoxFromLoginAccountModel model)
        {
            try
            {
                bool dettachPOBoxFromLoginAccountDet;
                using (var transactionscope = new TransactionScope())
                {
                    dettachPOBoxFromLoginAccountDet = _custAdminService.DettachPoboxFromLoginAccount(model.accountCustomerPKID, model.city, model.boxNumber);
                    transactionscope.Complete();
                }
                return Request.CreateResponse(HttpStatusCode.OK, dettachPOBoxFromLoginAccountDet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("dettach_from_loginaccount")]
        [HttpPost]
        public HttpResponseMessage DettachFromLoginAccount([FromBody]DettachFromLoginAccountModel model)
        {
            String json = "OK";
            try
            {
                _custAdminService.DettachFromLoginAccount(model.accountCustomerPKID, model.serviceCustomerPKID, model.bfunctionPKID);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("ERROR"));
            }
        }

        [Route("{customerPKID}/email_creds")]
        [HttpPost]
        public HttpResponseMessage EmailCredentials(int customerPKID, [FromBody]EmailCredentialsModel model)
        {
            try
            {
                JObject emailCredentialsResult;
                using (var transactionscope = new TransactionScope())
                {
                    emailCredentialsResult = _custAdminService.EmailCredentials(customerPKID, model.email);
                    transactionscope.Complete();
                }

                if (emailCredentialsResult != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, emailCredentialsResult);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("ERROR"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }
        [Route("forgot_creds")]
        [HttpPost]
        public HttpResponseMessage ForgotCredentials([FromBody]ForgotCredentialsModel model)
        {
            try
            {
                JObject resultMap = new JObject();
                using (var transactionscope = new TransactionScope())
                {
                    int customerPKID = _custAdminService.GetCustomerPKIDByUname(model.uname);
                    if (customerPKID == 0)
                    {
                        resultMap.Add("status", "ERROR");
                        resultMap.Add("reason", "INVALID_USER");
                    }
                    else
                    {
                        Customer customer = _custAdminService.GetCustomerByPKID(customerPKID);
                        if (customer.addressProfile != null && customer.addressProfile["contact"] != null
                                && customer.addressProfile["contact"]["email"].ToString() == model.email)
                        {
                            resultMap = _custAdminService.ForgotCredentials(model.uname, model.email);
                        }
                        else
                        {
                            resultMap.Add("status", "ERROR");
                            resultMap.Add("reason", "INCORRECT_EMAIL");
                        }
                    }
                    transactionscope.Complete();
                }
                return Request.CreateResponse(HttpStatusCode.OK, resultMap);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }

        [Route("{customerPKID}/sms_creds")]
        [HttpPost]
        public HttpResponseMessage SmsCredentials(int customerPKID, [FromBody]SMSCredentialsModel model)
        {
            try
            {
                JObject smsCredentialsResult;
                using (var transactionscope = new TransactionScope())
                {
                    smsCredentialsResult = _custAdminService.SmsCredentials(customerPKID, model.mobile);
                    transactionscope.Complete();
                }
                    if (smsCredentialsResult != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, smsCredentialsResult);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("ERROR"));
                    }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("smartregister_customer")]
        [HttpPost]
        public HttpResponseMessage SmartRegisterCustomer([FromBody]SmartRegisterCustomer models)
        {
            JObject islogidavail;
            string registerCustomer = null;
            JObject result = new JObject();
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    JObject validateQAMap = new JObject
                    {
                        { "loginID", models.loginID },
                        { "fName", models.fNameEN },
                        { "lName", models.lNameEN },
                        { "emailID", models.emailID },
                        { "regForm", models.regForm }
                    };
                    islogidavail = _custAdminService.IsLoginIdAvailable(models.loginID);
                    dynamic objlogidavail = JsonConvert.DeserializeObject<JObject>(islogidavail.ToString());
                    if (objlogidavail.status != "AVAILABLE")
                    {
                        result.Add("status", "ERROR");
                        result.Add("error", "REG_LOGINNAME_UNAVAILABLE");
                    }
                    else
                    {
                        registerCustomer = _custAdminService.RegisterCustomer(validateQAMap);
                        string logMsg = "smartRegisterCustomer REFERENCE -> " + registerCustomer;
                        Logger.Info(logMsg);

                        int customerAccountPKID = _custAdminService.GenerateAccount(
                               models.loginID, models.fNameEN, models.lNameEN, models.fNameAR, models.lNameAR, models.fullNameEN, models.fullNameAR,
                               models.emailID, models.mobile, models.eida, models.lang, models.smartpassID);

                        string logMsg1 = "smartRegisterCustomer customerPKID -> " + customerAccountPKID;
                        Logger.Info(logMsg1);

                        if (customerAccountPKID == -1)
                        {
                            result.Add("status", "ERROR");
                            result.Add("error", "REG_LOGINNAME_UNAVAILABLE");
                        }
                        else
                        {
                            result.Add("customerPKID", customerAccountPKID);
                            _custAdminService.CompleteRegistration(registerCustomer, models.loginID,
                                    customerAccountPKID,
                                    "{\"authType\":\"PWD_ONLY\"}",
                                    models.emailID);

                            if (models.smartpassID == null || models.smartpassID.Length == 0)
                            {
                                if (System.Configuration.ConfigurationManager.AppSettings["isDebugMode"] == "false")
                                {
                                    _custAdminService.EmailCredentials(customerAccountPKID, models.emailID);
                                }
                            }
                            result.Add("status", "OK");
                            string logMsg2 = "smartRegisterCustomer RETURNING -> " + result + " for smartpass :" + models.smartpassID + ":";
                            Logger.Info(logMsg2);
                        }
                    }
                    transactionScope.Complete();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }


        [Route("register_corporate_customer")]
        [HttpPost]
        public HttpResponseMessage RegisterCorporateCustomer([FromBody]CoporateCustomerModel model)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    var result = _custAdminService.RegisterCorporateCustomer(model);
                    transactionScope.Complete();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }
    }

}

