using Oracle.ManagedDataAccess.Client;
using SVC_CustomerManagement_Data.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Collections;
using Dapper;
using SVC_CustomerManagement_Utilities.CustomerProfile;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class CustomerProfileData
    {
        public Customer GetCustomerInfo(int customerPKID)
        {
            Customer customer = null;
            try
            {
                using (OracleConnection cnn = new OracleConnection(OracleProviders.GetConnectionString(DBConnections.esvccorpdb)))
                {
                    cnn.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("p_CUSTOMER_PKID", customerPKID, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.RefCursor, direction: ParameterDirection.ReturnValue);

                    //results = cnn.Query("FN_GET_CUSTOMER_PROFILE", param: param, commandType: CommandType.StoredProcedure) as IEnumerable<IDictionary<string, object>>;
                    //outData = results.Select(r => r.ToDictionary(d => d.Key, d => d.Value == null ? "" : d.Value));
                    var results = cnn.Query("FN_GET_CUSTOMER_PROFILE", param: param, commandType: CommandType.StoredProcedure);
                    customer = CustomerTransformer.updateCustomer(results);
                }
            }
            catch (System.Exception)
            {
            }

            return customer;
        }


        public bool IsAuthProcedureValid(JObject profileAuthProc, JObject userInputData)
        {
            bool result = true;
            if ((profileAuthProc != null))
            {
                if ((userInputData == null))
                {
                    return false;
                }

                String dbQuestion = ((String)(profileAuthProc["question"]));
                String dbAnswer = ((String)(profileAuthProc["answer"]));
                String dbAuthType = ((String)(profileAuthProc["authType"]));
                String authQuestion = ((String)(userInputData["question"]));
                String authAnswer = ((String)(userInputData["answer"]));
                String authAuthType = ((String)(userInputData["authType"]));

                if (((dbAuthType != null)
                            && !"PWD_ONLY".Equals(dbAuthType)))
                {
                    //  check if QA is valid
                    if ((!dbQuestion.Equals(authQuestion)
                                || (!dbAnswer.Equals(authAnswer)
                                || !dbAuthType.Equals(authAuthType))))
                    {
                        result = false;
                    }

                }

            }

            return result;
        }

        public JObject GetRegistrationByLoginID(string uname)
        {
            JObject results = null;
            using (OracleConnection cnn = new OracleConnection(OracleProviders.GetConnectionString(DBConnections.esvcuserdb)))
            {
                cnn.Open();
                var queryResult = cnn.Query("SELECT * FROM  WEB_CUSTOMER_REGISTER WHERE LOGIN_ID = " + uname + "", commandType: CommandType.Text);
            }
            return results;
        }

        public JObject login(string uname, string pwd, string spass, string sourceAddress, string token)
        {
            try
            {

                using (OracleConnection cnn = new OracleConnection(OracleProviders.GetConnectionString(DBConnections.esvcuserdb)))
                {
                    cnn.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("v_Return", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                    param.Add("uname", uname, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("pwd", pwd, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("sourceAddress", sourceAddress, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("spass", spass, OracleDbType.Int32, ParameterDirection.Input);

                    if (token == null)
                    {
                        param.Add("token", 6, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    else
                    {
                        param.Add("token", token, OracleDbType.Int32, ParameterDirection.Input);
                    }
                    var queryResult = cnn.Query("FN_UA_AUTH_LOGIN", param: param, commandType: CommandType.StoredProcedure);
                    int result = 111;//queryResult;
                    if (result > 0)
                    {
                        //queryResult.put("status", "OK");
                        // queryResult.put("customerPKID",  result);
                    }
                    else
                    {
                        // queryResult.put("status", "ERROR");
                        //queryResult.put("reason", "INVALID_CREDS");
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new JObject();
        }

        public void setSpass(JObject loginDetails, string p)
        {
            //    Gson gson = new GsonBuilder().create();
            //// Cache spass data in global. Invoke moi data and cache in global
            //// context_s=SPASS_LOGIN&key_s=_SPASS_" + customerPKID
            //LogSupport.debug("SpassCacheBean", "[SPASSDATA SET] as key = SPASS_LOGIN,"+"_SPASS_" +accMap.get("customerPKID"));
            //Map spassDataMap = gson.fromJson(spassData, Map.class);                    
            //String dataJSON = moiClient.doPersonInquiry((String)spassDataMap.get("idn"));
            //if (dataJSON == null) {
            //    // retry once more
            //    LogSupport.debug("SpassCacheBean","RETRY ONCEMORE");
            //    dataJSON = moiClient.doPersonInquiry((String)spassDataMap.get("idn"));
            //    if (dataJSON == null) dataJSON = "{}";
            //}
            //spassDataMap.put("moi", gson.fromJson(dataJSON, Map.class));
            //LogSupport.debug("CustomerProfile", "[SPASSDATA SET] as value = "+ gson.toJson(spassDataMap));
            //LogSupport.debug("CustomerProfile", "GlobalCacheBean " + globalCache);
            //globalCache.toCache("SPASS_LOGIN","_SPASS_" +accMap.get("customerPKID"), gson.toJson(spassDataMap));
        }
    }
}




