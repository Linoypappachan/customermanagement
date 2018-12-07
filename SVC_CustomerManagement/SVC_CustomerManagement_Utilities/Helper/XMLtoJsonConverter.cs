using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;

namespace SVC_CustomerManagement_Utilities.Helper
{
    public class XMLtoJsonConverter
    {
        public static T GetJson<T>(string xmlString) where T : class
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(xml);
            var result = JsonConvert.DeserializeObject<T>(jsonString);
            return (T)result;
        }


        public static JObject GetJson(string xmlString)
        {
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xmlString);
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(xml);
                return JsonConvert.DeserializeObject<JObject>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
