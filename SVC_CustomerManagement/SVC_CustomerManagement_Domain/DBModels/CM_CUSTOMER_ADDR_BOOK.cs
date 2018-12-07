using Dapper;
using System;

namespace SVC_CustomerManagement_Domain.DBModels
{
    [Table("CM_CUSTOMER_ADDR_BOOK")]
    public class CM_CUSTOMER_ADDR_BOOK
    {
        private string _IS_DEFAULT_ADDR = "N";
        private string _ISACTIVE = "Y";
        [Key]
        public int ID { get; set; }
        public int CUSTOMER_PKID { get; set; }
        public string ADDRESS_TYPE { get; set; }
        public string IS_DEFAULT_ADDR
        {
            get
            {
                return _IS_DEFAULT_ADDR ?? "N";
            }
            set
            {
                _IS_DEFAULT_ADDR = value;
            }
        }
        public string CONTACT_NAME { get; set; }
        public string COMPANY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string ORGIN { get; set; }
        public string ORGIN_CITY { get; set; }
        public string POBOX { get; set; }
        public string CONTACT_PHONE { get; set; }
        public string CONTACT_MOBILE { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string EMAIL { get; set; }
        public DateTime CREATED_ON { get; set; }
        public string ISACTIVE
        {
            get
            {
                return _ISACTIVE ?? "Y";
            }
            set
            {
                _ISACTIVE = value;
            }
        }
        public DateTime UPDATED_ON { get; set; }
        public string NICK_NAME { get; set; }
        public string COUNTRY { get; set; }
    }

}

