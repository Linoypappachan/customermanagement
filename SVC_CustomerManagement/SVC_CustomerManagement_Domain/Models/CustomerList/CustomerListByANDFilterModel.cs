using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Domain.Models.CustomerList
{
    public class CustomerListByANDFilterModel
    {

        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string eidano { get; set; }
        public int bfunctionPKID { get; set; }
        public string bfunction_customerid { get; set; }
    }
}
