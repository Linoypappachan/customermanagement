using System;
using System.IO;
using System.Net;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class MOIWSClient
    {
        public string MOI_API_URI = "http://intranet/api_moi/inquire_profile";
        public String doPersonInquiry(string eidaNumber)
        {
            String dataJSON = null;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(MOI_API_URI + "?emirates_id_number=" + eidaNumber);
                request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                request.Headers.Set("charset", "UTF-8");
                var response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataJSON = reader.ReadToEnd();
                    return dataJSON;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
