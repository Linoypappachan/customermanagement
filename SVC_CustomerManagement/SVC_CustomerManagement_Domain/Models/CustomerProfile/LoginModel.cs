
namespace SVC_CustomerManagement_Domain.Models.CustomerProfile
{
    public class LoginModel
    {
        public string uname { get; set; }

        public string pwd { get; set; } = "";

        public string spass { get; set; } = "LOCAL";

        public string spassdata { get; set; } = "{}";

        public string authProcedureJSON { get; set; }
    }
}
