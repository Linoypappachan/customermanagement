using System.ComponentModel;

namespace SVC_CustomerManagement_Domain.Models.CustomerAdmin
{
    public class RegisterCustomerModel
    {
        public string loginID { get; set; }
        public string fName { get; set; } = "-";
        public string lName { get; set; } = "-";
        public string emailID { get; set; }
        public string regForm { get; set; }
    }
}
