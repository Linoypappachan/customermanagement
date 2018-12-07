using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Domain.Models.CustomerAdmin
{
    public class CustomerRegistrationResponse
    {
        public string login_id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email_id { get; set; }
        public string auth_procedure { get; set; }
        public string status { get; set; }
        public string internal_reg_id { get; set; }
        public string reg_form { get; set; }
        public int customer_pkid { get; set; }
    }
}
