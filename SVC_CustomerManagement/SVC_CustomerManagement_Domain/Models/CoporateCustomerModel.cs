using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Domain.Models
{
    public class CoporateCustomerModel
    {
        public int pkid { get; set; }
        public string fname_en { get; set; }
        public string fname_ar { get; set; }
        public string lname_en { get; set; }
        public string lname_ar { get; set; }
        public string full_name_en { get; set; }
        public string full_name_ar { get; set; }
        public string company_name { get; set; }
        public string email_id { get; set; }
        public string mobile { get; set; }
        public string eida { get; set; }
        public string lang { get; set; }
        public int account_cust_pkid { get; set; }
        public string bpkid { get; set; }
        public string customer_number { get; set; }
        public string customer_number_prefix { get; set; }
    }
}
