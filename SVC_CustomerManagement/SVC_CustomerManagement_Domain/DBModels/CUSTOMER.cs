using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SVC_CustomerManagement_Domain.DBModel
{
    public class CUSTOMER
    {
        public int PKID { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string NATIONALITY { get; set; }
        public string CREATION_SOURCE { get; set; }
        public string CUSTOMER_NUMBER { get; set; }
        public string REMARKS { get; set; }
        public string LANGUAGE { get; set; }
        public string STATUS { get; set; }
        public DateTime CREATED_ON { get; set; }
        public string EIDA_CARDNUMBER { get; set; }
        public DateTime EIDA_EXPIRYDATE { get; set; }
        public string FNAME_EN { get; set; }
        public string FNAME_AR { get; set; }
        public string LNAME_EN { get; set; }
        public string LNAME_AR { get; set; }
    }
}