using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Domain.Models.LookupProvider
{
    public class ServiceListResponse
    {
        public int vendor_id { get; set; }
        public int vendor_number { get; set; }
        public string vendor_name { get; set; }
        public string enabled_flag { get; set; }
        public string vendor_type_lookup_code { get; set; }
        public string vendor_site_code { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string address_line4 { get; set; }
        public string country { get; set; }
        public string area_code { get; set; }
        public string phone { get; set; }
        public string fax_area_code { get; set; }
        public string fax { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public DateTime inactive_date { get; set; }
        public DateTime load_date { get; set; }
        public int vendor_site_id { get; set; }
    }
}
