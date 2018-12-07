using SVC_CustomerManagement_Data.DataLayer;
using SVC_CustomerManagement_Domain.Models.common;
using SVC_CustomerManagement_Domain.Models.CustomerList;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SVC_CustomerManagement.Controllers
{
    //[RoutePrefix("svc_customermanagement/rs/customers")]
    [RoutePrefix("rs/customers")]
    public class CustomerListRSController : ApiController
    {
        private CustomerListRSData _custListService;
        public CustomerListRSController()
        {
            _custListService = new CustomerListRSData();
        }

        [Route("customer_by_box/{boxPKID}")]
        [HttpGet]
        public HttpResponseMessage GetCustomerByBox(int boxPKID)
        {
            try
            {
                Customer customerDetails = null;
                customerDetails = _custListService.GetCustomerInfo(_custListService.GetCustomerPKIDByBox(boxPKID));
                if (customerDetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, customerDetails);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }

        }

        [Route("list")]
        [HttpGet]
        public HttpResponseMessage GetCustomerListByBusinessFunction(int businessFunctionPKID)
        {
            try
            {
                List<Customer> customerDetails = null;
                customerDetails = _custListService.GetCustomerListByBusinessFunction(businessFunctionPKID);
                if (customerDetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, customerDetails);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }


        [HttpPost]
        [Route("list_by_andfilter")]
        //[LowerPropertyName]
        public HttpResponseMessage GetCustomerListByANDFilter([FromUri]CustomerListByANDFilterModel model)
        {
            try
            {
                Object customermdetailsandfilter = _custListService.GetCustomerListByANDFilter(model.name, model.email, model.mobile, model.eidano, model.bfunctionPKID.ToString(), model.bfunction_customerid);
                if (customermdetailsandfilter != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, customermdetailsandfilter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [HttpPost]
        [Route("list_by_orfilter")]
        //[LowerPropertyName]
        public HttpResponseMessage GetCustomerListByORFilter([FromUri]CustomerListByORFilterModel model)
        {
            try
            {
                int bfnPkid = 0;
                if (model.bfunctionPKID != 0)
                {
                    bfnPkid = model.bfunctionPKID;
                }
                Object customermdetailsorfilter = _custListService.GetCustomerListByORFilter(model.name, model.email, model.mobile, model.eidano, bfnPkid.ToString(), model.bfunction_customerid);
                if (customermdetailsorfilter != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, customermdetailsorfilter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

    }
}
