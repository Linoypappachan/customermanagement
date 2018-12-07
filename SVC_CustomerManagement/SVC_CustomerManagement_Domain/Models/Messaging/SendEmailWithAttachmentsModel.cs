namespace SVC_CustomerManagement_Domain.Models.Messaging
{
    public class SendEmailWithAttachmentsModel
    {

        public int bfunctionPKID { get; set; }
        public string recepientList { get; set; }
        public string subject { get; set; }
        public string body { get; set; }

        public byte[] attachment1 { get; set; }
        public byte[] attachment2 { get; set; }
        public byte[] attachment3 { get; set; }

    }
}
