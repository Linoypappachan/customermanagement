namespace SVC_CustomerManagement_Domain.Models.Messaging
{
    public  class SendEmailModel
    {
     public int bfunctionPKID { get; set; }
     public string recepientList { get; set; }
     public string subject { get; set; }
     public string body { get; set; }
    }
}
