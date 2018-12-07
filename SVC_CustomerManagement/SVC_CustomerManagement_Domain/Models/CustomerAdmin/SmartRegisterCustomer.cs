using System.ComponentModel;

namespace SVC_CustomerManagement_Domain.Models.CustomerAdmin
{
    public class SmartRegisterCustomer
    {
        public string loginID { get; set; }
        public string fNameEN { get; set; } = "-";
        public string lNameEN { get; set; } = "-";
        public string fNameAR { get; set; } = "-";
        public string lNameAR { get; set; } = "-";
        public string fullNameEN { get; set; } = "-";
        public string fullNameAR { get; set; } = "-";
        public string emailID { get; set; }
        public string mobile { get; set; }
        public string eida { get; set; }
        public string lang { get; set; }
        public string smartpassID { get; set; }
        public string regForm { get; set; }
    }
}
