using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Domain.Models.common
{
    public class CommonErrorMap
    {
        private string _status;
        public string status
        {
            get { return _status ?? "ERROR"; }
            set { _status = value; }
        }
        public string reason { get; set; }
    }

    public class CommonSuccessMap
    {

        private string _status;
        public string status
        {
            get { return _status ?? "OK"; }
            set { _status = value; }
        }
        public object detail { get; set; }
    }

    public class ErrorStatusMap
    {
        private string _status;
        public string status
        {
            get { return _status ?? "ERROR"; }
            set { _status = value; }
        }

        private string _error;
        public string error
        {
            get { return _error ?? "NO_DATA"; }
            set { _error = value; }
        }
    }

    public class CommonResult
    {
        public static CommonSuccessMap GetSucessResult(object result)
        {
            var data = new CommonSuccessMap()
            {
                detail = result
            };
            return data;
        }

        public static CommonErrorMap GetErrorResult(string result)
        {
            var data = new CommonErrorMap()
            {
                reason = result
            };
            return data;
        }

        public static ErrorStatusMap GetErrorStatusResult(string error)
        {
            var data = new ErrorStatusMap()
            {
                error = error
            };
            return data;
        }
    }
}
