namespace SVC_CustomerManagement_Domain.Models.CustomerProfile
{
    public class UpdatePassword
    {
        public string uname { get; set; }
        public string oldpwd { get; set; }
        public string newpwd { get; set; }
    }
}
