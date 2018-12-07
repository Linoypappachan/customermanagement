using SVC_CustomerManagement_Data.DataLayer;
using SVC_CustomerManagement_Domain.Models.common;
using SVC_CustomerManagement_Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Transactions;

namespace SVC_CustomerManagement.Controllers
{
    [RoutePrefix("rs/address_book")]
    public class CustomerAddressBookController : BaseApiController
    {
        private CustomerAddressBookRSData _custAddressBookingService;

        public CustomerAddressBookController()
        {
            _custAddressBookingService = new CustomerAddressBookRSData();
        }

        [Route("get/{id}")]
        [HttpGet]
        [LowerPropertyName]
        public HttpResponseMessage Get(int id)
        {
            var result = new CM_CUSTOMER_ADDR_BOOK();
            try
            {
                result = _custAddressBookingService.Get(id);
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("list/{customerPkid}/customer_pkid")]
        [HttpGet]
        [LowerPropertyName]
        public HttpResponseMessage GetByCustomerPkid(int customerPkid)
        {
            var result = Enumerable.Empty<CM_CUSTOMER_ADDR_BOOK>();
            try
            {
                result = _custAddressBookingService.GetByCustomerPkid(customerPkid);
                if (result.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("No data found"));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("defaultaddressdetails/{customer_pkid}/customer_pkid")]
        [HttpGet]
        [LowerPropertyName]
        public HttpResponseMessage GetDefaultAddressDetails(int customer_pkid)
        {
            var result = new CM_CUSTOMER_ADDR_BOOK();
            try
            {
                result = _custAddressBookingService.GetDefaultAddressDetails(customer_pkid);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("No data found"));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("add")]
        [HttpPost]
        public HttpResponseMessage Add([FromBody]CM_CUSTOMER_ADDR_BOOK model)
        {
            int result;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    result = _custAddressBookingService.Add(model);
                    transactionScope.Complete();
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("update/{id}")]
        [HttpPost]
        public HttpResponseMessage Update(int id, [FromBody]CM_CUSTOMER_ADDR_BOOK model)
        {
            bool result = false;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    result = _custAddressBookingService.Update(id, model);
                    transactionScope.Complete();
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("delete/{id}")]
        [HttpPost]
        public HttpResponseMessage Delete(int id)
        {
            bool result;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    result = _custAddressBookingService.Delete(id);
                    transactionScope.Complete();
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

        [Route("bulk_upload")]
        [HttpPost]
        public HttpResponseMessage BulkUpload([FromBody]List<CM_CUSTOMER_ADDR_BOOK> customerAddressList)
        {
            bool result = false;
            try
            {
                if (customerAddressList.Any())
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        result = _custAddressBookingService.BulkInsert(customerAddressList);
                        transactionScope.Complete();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetSucessResult(result));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult("No Items to insert"));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonResult.GetErrorResult(ex.Message));
            }
        }

    }
}
