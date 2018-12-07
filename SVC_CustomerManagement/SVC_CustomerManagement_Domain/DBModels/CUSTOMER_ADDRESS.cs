using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SVC_CustomerManagement_Domain.DBModel
{
    public class CUSTOMER_ADDRESS
    {
        public int PKID { get; set; }
        public string OFFICE { get; set; }
        public string BUILDING { get; set; }
        public string FAX { get; set; }
        public string LANDMARK { get; set; }
        public string WEBSITE { get; set; }
        public string STREET { get; set; }
        public string OTHER_BOX_NUMBER { get; set; }
        public string CITY { get; set; }
        public string EXTENSION { get; set; }
        public string AREA { get; set; }
        public string EMAIL { get; set; }
        public string DISTRICT { get; set; }
        public string TELEPHONE { get; set; }
        public string MOBILE { get; set; }
        //foreign key
        public int CUSTOMER_PKID { get; set; }
        public string ADDRESS_TYPE { get; set; }
    }
}