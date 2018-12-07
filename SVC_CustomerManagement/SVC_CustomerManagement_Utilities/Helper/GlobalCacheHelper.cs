using Newtonsoft.Json;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SVC_CustomerManagement_Utilities.Helper
{
    public class GlobalCacheHelper
    {
        public static string GLOBAL_CACHE_BASE_URI = "http://intranet/api_infra/cachedb/";
        private static int CACHE_EXPIRY_SECONDS = 60 * 60;

        public static string CheckCache(string key)
        {
            string responseData = string.Empty;
            Dictionary<String, Object> list = new Dictionary<string, object>();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(GLOBAL_CACHE_BASE_URI + "browse?context_s=GENERAL&key_s=" + key);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                request.Headers.Set("charset", "UTF-8");
                var response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseData = reader.ReadToEnd();
                }
                if (responseData == null)
                {
                    string logmsg1 = "GlobalCacheHelper [CUSTOMER] :CACHE BROWSE NO DATA for " + key;
                    Logger.Debug(logmsg1);
                }
                else
                {
                    string logMsg = "GlobalCacheHelper [CUSTOMER]:CACHE BROWSE BROWSE FOUND DATA for " + key;
                    Logger.Debug(logMsg);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return responseData;
        }

        public static void AddToCache(string key, Object value)
        {
            try
            {
                string sURL = GLOBAL_CACHE_BASE_URI + "set";
                WebRequest request = WebRequest.Create(sURL);
                string postData = "context_s=GENERAL&key_s=" + key + "&text_s=" + JsonConvert.SerializeObject(value) + "&expiryInSeconds_i="+CACHE_EXPIRY_SECONDS+"";
                var data = System.Text.Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                request.Headers.Set("charset", "UTF-8");
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (response.StatusCode.ToString() != "OK")
                {

                    string logMsg = "GlobalCacheHelper CACHE SET FAILED for " + key + " STATUS_CODE= " + response.StatusCode;
                    Logger.Debug(logMsg);
                }
                else
                {
                    string logMsg1 = "GlobalCacheHelper CACHE SET DONE for " + key + " STATUS_CODE= " + response.StatusCode;
                    Logger.Debug(logMsg1);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);

            }
        }

        public static void ClearCache(string key)
        {
            try
            {
                string sURL = GLOBAL_CACHE_BASE_URI + "clear?context_s=GENERAL&key_s=" + key;
                WebRequest request = WebRequest.Create(sURL);
                request.Method = "DELETE";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode.ToString() != "OK")
                {
                    string logMsg = "GlobalCacheHelper CACHE SET FAILED for " + key + " STATUS_CODE= " + response.StatusCode;
                    Logger.Debug(logMsg);
                }
                else
                {
                    string logMsg1 = "GlobalCacheHelper CACHE SET DONE for " + key + " STATUS_CODE= " + response.StatusCode;
                    Logger.Debug(logMsg1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static void toCache(String category, String key, String value)
        {
            try
            {
                string sURL = GLOBAL_CACHE_BASE_URI + "set";
                WebRequest request = WebRequest.Create(sURL);
                string postData = "context_s="+category+"&key_s=" + key + "&text_s=" + JsonConvert.SerializeObject(value) + "&expiryInSeconds_i="+CACHE_EXPIRY_SECONDS+"";
                var data = System.Text.Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                request.Headers.Set("charset", "UTF-8");
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (response.StatusCode.ToString() != "OK")
                {
                    Logger.Debug("CACHE SET FAILED for " + key + " STATUS_CODE= " + response.StatusCode);
                }
                else
                {
                    Logger.Debug("CACHE SET DONE for " + key + " STATUS_CODE= " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
