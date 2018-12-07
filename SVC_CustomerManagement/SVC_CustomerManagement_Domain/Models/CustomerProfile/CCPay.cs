namespace SVC_CustomerManagement_Domain.Models.CustomerProfile
{
    public class CCPay
    {
        public int customerPKID { get; set; }
        public int orderNumber { get; set; }
        public string txnReference { get; set; }
        public string cardNumber { get; set; }
        public string cardType { get; set; }
        public int expiryMonth { get; set; }
        public int expiryYear { get; set; }
        public double principalAmount { get; set; }
        public double ccChargeAmount { get; set; }
        public double totalAmount { get; set; }
    }
}
