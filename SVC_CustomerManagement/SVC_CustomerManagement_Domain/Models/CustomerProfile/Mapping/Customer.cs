using Newtonsoft.Json.Linq;

namespace SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping
{
    public class Customer
    {
        public int pkid { get; set; }
        public JObject customerName { get; set; }
        public JObject personalProfile { get; set; }
        public JObject addressProfile { get; set; }
        public JArray businessFunctionProfile { get; set; }
        public string customerNumber { get; set; }
        public string customerType { get; set; }
       // public JToken customerLinks { get; set; }
        public JArray customerLinks { get; set; }
        public JObject accountProfile { get; set; }
    }
}
