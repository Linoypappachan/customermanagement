using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Domain.Models.CustomerList
{
    public class ListFilterResponse
    {
        public int pkid { get; set; }
        public string customer_type { get; set; }
        public string customer_name { get; set; }
        public string eida_no { get; set; }
        public string login_name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string language { get; set; }
    }
}
