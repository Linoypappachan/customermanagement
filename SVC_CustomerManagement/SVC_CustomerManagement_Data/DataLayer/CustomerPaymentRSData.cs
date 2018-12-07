namespace SVC_CustomerManagement_Data.DataLayer
{
    public class CustomerPaymentRSData
    {
        public string PayByCreditCard(string accountPKID, int customerPKID, int orderNumber, string txnReference, string cardNumber, string cardType, int expiryMonth, int expiryYear, double principalAmount, double ccChargeAmount, double totalAmount)
        {

            string successMessage = "APPROVED:0123456";
            string declinedMessage = "DECLINED";
            string invalidInputMessage = "INVALID_INPUT";

            if (accountPKID == null || customerPKID == 0 || orderNumber == 0 ||
                    txnReference == null || cardNumber == null || cardType == null ||
                    expiryMonth == 0 || expiryYear == 0 ||
                    principalAmount == 0 || ccChargeAmount == 0)
            {
                return invalidInputMessage;
            }
            else
            {
                if (!("1234567891234567" == cardNumber) ||
                        (expiryMonth != 4) ||
                        expiryYear != 2020)
                {
                    return declinedMessage;
                }
            }
            return successMessage;
        }

    }
}
