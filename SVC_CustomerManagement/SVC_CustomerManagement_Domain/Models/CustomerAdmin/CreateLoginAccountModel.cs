namespace SVC_CustomerManagement_Domain.Models.CustomerAdmin
{
    public class CreateLoginAccountModel
    {
        public int customerPKID { get; set; }
        public string uname { get; set; }
        public string pwd { get; set; }

    }
}
