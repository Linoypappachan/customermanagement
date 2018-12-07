using SVC_CustomerManagement_Data.DataLayer;
using SVC_CustomerManagement_Domain.Models.common;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SVC_CustomerManagement.Controllers
{
    //[RoutePrefix("svc_customermanagement/rs/lookup")]
    [RoutePrefix("rs/lookup")]
    public class LookupProviderRSController : ApiController
    {
        private LookupProviderRSData _lookupListService;
        public LookupProviderRSController()
        {
            _lookupListService = new LookupProviderRSData();
        }

        [Route("oraf_customer_list")]
        [HttpGet]
        [LowerPropertyName]
        public HttpResponseMessage GetCustomerListByFilter(string name, string email, string mobile, string bfunctionCustomerID)
        {
            try

            {
                Object customerlistbyfilterdetails = _lookupListService.GetCustomerListByFilter(name, email, mobile, bfunctionCustomerID);
                if (customerlistbyfilterdetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, customerlistbyfilterdetails);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }


        [Route("oraf_supplier_list")]
        [HttpGet]
        //[LowerPropertyName]
        public HttpResponseMessage GetSupplierListByFilter(string name, string email, string mobile, string bfunctionCustomerID)
        {
            try
            {
                Object supplierlistbyfilterdetails = _lookupListService.GetSupplierListByFilter(name, email, mobile, bfunctionCustomerID);
                if (supplierlistbyfilterdetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, supplierlistbyfilterdetails);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }


        [Route("service_list/{bfunctionPKID}/{customerPKID}")]
        [HttpGet]
        public HttpResponseMessage GetServiceList(int bfunctionPKID, int customerPKID)
        {
            try
            {
                Object supplierlistdetails = _lookupListService.GetServiceList(customerPKID, bfunctionPKID);
                if (supplierlistdetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, supplierlistdetails);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }
            }

            catch (Exception ex)
            {
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("businessfunction_list")]
        [HttpGet]
        public HttpResponseMessage GetBusinessFunctionList()
        {
            try

            {
                Object businessfunctiondetails = _lookupListService.GetBusinessFunctionList();
                if (businessfunctiondetails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, businessfunctiondetails);
                }
                else
                {
                    return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult("NO_DATA"));
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse<CommonErrorMap>(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }
    }
}
