using Dapper;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using SVC_CustomerManagement_Data.Extensions;
using System;
using System.Configuration;
using System.Data;
using System.Transactions;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class MessagingRSData
    {
        public JObject SendEmail(int customerPKID, int servicePKID, string recepientList, string subject, string body)
        {
            JObject result = new JObject();
            string[] recpts = recepientList.Split(';');
            try
            {
                using (var transactionscope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["messagedb"].ConnectionString))
                    {
                        connection.Open();
                        foreach (var rcpt in recpts)
                        {
                            var param = new OracleDynamicParameters();
                            param.Add("P_SENDER", "SVC_CUSTMGMT", OracleDbType.Varchar2, ParameterDirection.Input);
                            param.Add("P_RECEPIENTS", rcpt, OracleDbType.Varchar2, ParameterDirection.Input);
                            param.Add("P_SUBJECT", subject, OracleDbType.Varchar2, ParameterDirection.Input);
                            param.Add("P_BODY", body, OracleDbType.Varchar2, ParameterDirection.Input);
                            param.Add("v_Return", dbType: OracleDbType.NVarchar2, direction: ParameterDirection.ReturnValue, size: 500);
                            connection.Execute("UTL_SEND_EMAIL",
                                                  param,
                                                  commandType: CommandType.StoredProcedure);
                            var rowCount = param.Get<dynamic>("@v_Return");
                            string message = rowCount.Value;
                            result.Add("recepient", rcpt);
                            if (message.Contains("OK") || message.Contains("SENT"))
                            {
                                result.Add("status", "OK");
                            }
                            else
                            {
                                result.Add("status", "FAILED");
                            }
                        }
                    }
                    transactionscope.Complete();
                }
            }
            catch (Exception ex)
            {
                result.Add("status", "ERROR");
                result.Add("error", "SYSTEM_ERROR: " + ex.Message);
            }
            return result;
        }

        public JObject SendSms(int customerPKID, int servicePKID, string recepientList, string body)
        {
            JObject result = new JObject();
            string[] recpts = recepientList.Split(';');
            try
            {
                using (var transactionscope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["messagedb"].ConnectionString))
                    {
                        connection.Open();
                        foreach (var rcpt in recpts)
                        {

                            var param = new OracleDynamicParameters();
                            param.Add("P_SENDER", "SVC_CMTEST", OracleDbType.Varchar2, ParameterDirection.Input);
                            param.Add("P_RECEPIENTS", rcpt, OracleDbType.Varchar2, ParameterDirection.Input);
                            param.Add("P_BODY", body, OracleDbType.Varchar2, ParameterDirection.Input);
                            param.Add("v_Return", dbType: OracleDbType.NVarchar2, direction: ParameterDirection.ReturnValue, size: 500);
                            connection.Execute("UTL_SEND_SMS",
                                                  param,
                                                  commandType: CommandType.StoredProcedure);
                            var rowCount = param.Get<dynamic>("@v_Return");
                            string message = rowCount.Value;
                            result.Add("recepient", rcpt);

                            if (message.Contains("OK") || message.Contains("SENT"))
                            {
                                result.Add("status", "OK");
                            }
                            else
                            {
                                result.Add("status", "FAILED");
                            }

                        }
                        transactionscope.Complete();
                    }

                }
            }
            catch (Exception ex)
            {
                result.Add("error", "SYSTEM_ERROR" + ex.Message);
            }
            return result;

        }

    }
}
