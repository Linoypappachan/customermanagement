namespace SVC_CustomerManagement_Domain.Models.Messaging
{
    public  class SendSMSModel
    {
       public int bfunctionPKID { get; set; }
       public string recepientList { get; set; }
       public string message { get; set; }
    }
}
