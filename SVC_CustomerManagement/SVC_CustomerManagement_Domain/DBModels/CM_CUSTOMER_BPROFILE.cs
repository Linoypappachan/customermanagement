using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SVC_CustomerManagement_Domain.DBModel
{
    public class CM_CUSTOMER_BPROFILE
    {
        public int PKID { get; set; }
        public string BPKID { get; set; }
        public string BVALUE { get; set; }
        public string LINK_CUSTOMER_ID { get; set; }
    }
}