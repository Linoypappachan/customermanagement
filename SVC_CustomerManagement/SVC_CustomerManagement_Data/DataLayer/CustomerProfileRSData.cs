using System;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Transactions;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using SVC_CustomerManagement_Data.Extensions;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using SVC_CustomerManagement_Utilities.CustomerProfile;
using SVC_CustomerManagement_Utilities.Helper;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System.Collections.Generic;
using System.Web.Helpers;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class CustomerProfileRSData
    {
        public Customer GetCustomerInfo(int customerPKID)
        {
            Customer customer = null;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("p_CUSTOMER_PKID", customerPKID, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.RefCursor, direction: ParameterDirection.ReturnValue);
                    var results = connection.Query("FN_GET_CUSTOMER_PROFILE", param: param, commandType: CommandType.StoredProcedure);
                    customer = CustomerTransformer.updateCustomer(results);
                    return customer;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public int GetCustomerPKIDByBox(int boxPKID)
        {
            int customerPKID = 0;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query(@"SELECT customer_pkid FROM BOX_RENT WHERE status = '1' and box_pkid = :boxPKID", new { boxPKID = boxPKID }).ToList();
                    customerPKID = Int32.Parse(contacts[0].CUSTOMER_PKID.ToString());

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customerPKID;
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
                string dbQuestion = (string)profileAuthProc["question"];
                string dbAnswer = (string)profileAuthProc["answer"];
                string dbAuthType = (string)profileAuthProc["authType"];

                string authQuestion = (string)userInputData["question"];
                string authAnswer = (string)userInputData["answer"];
                string authAuthType = (string)userInputData["authType"];

                if (dbAuthType != null && !"PWD_ONLY".Equals(dbAuthType.ToUpper()))
                {
                    //  check if QA is valid
                    if (!dbQuestion.ToUpper().Equals(authQuestion.ToUpper()) ||
                        !dbAnswer.ToUpper().Equals(authAnswer.ToUpper()) ||
                        !dbAuthType.ToUpper().Equals(authAuthType.ToUpper()))
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        public JObject GetRegistrationByLoginID(string uname)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcuserdb"].ConnectionString))
                {
                    connection.Open();
                    int sec = connection.ConnectionTimeout;
                    var contacts = connection.Query(@"SELECT * FROM WEB_CUSTOMER_REGISTER WHERE LOGIN_ID = :loginID", new { loginID = uname }).ToList();
                    return contacts.Count > 0 ? JObject.Parse(contacts[0].AUTH_PROCEDURE.ToString()) : null;

                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public JObject Login(string uname, string pwd, string spass, string sourceAddress, string token)
        {
            JObject result = new JObject();
            try
            {
                using (var transactionscope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                    {
                        connection.Open();
                        var param = new OracleDynamicParameters();
                        param.Add("ReturnValue", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                        param.Add("P_LOGIN_NAME", uname, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_PASSWORD", pwd, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_SOURCE_ADDRESS", sourceAddress, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_SMARTPASS_ID", spass, OracleDbType.Varchar2, ParameterDirection.Input);

                        if (token == null)
                        {
                            param.Add("P_TOKEN", null, OracleDbType.Varchar2, ParameterDirection.Input);
                        }
                        else
                        {
                            param.Add("P_TOKEN", token, OracleDbType.Varchar2, ParameterDirection.Input);
                        }
                        connection.Query("FN_UA_AUTH_LOGIN", param,
                                              commandType: CommandType.StoredProcedure);
                        var rowCount = param.Get<dynamic>("@ReturnValue");
                        if (rowCount > 0)
                        {
                            result.Add("status", "OK");
                            result.Add("customerPKID", (int)rowCount);
                        }
                        else
                        {
                            result.Add("status", "ERROR");
                            result.Add("reason", "INVALID_CREDS");
                        }
                    }
                    transactionscope.Complete();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Add("status", "ERROR");
                result.Add("reason", "SYSTEM_ERROR");
            }
            return result;
        }

        public JObject Loginlinksmartpass(string uname, string pwd, string sourceAddress, string eida, string token)
        {
            JObject result = new JObject();
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                    {
                        connection.Open();
                        var param = new OracleDynamicParameters();
                        param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                        param.Add("P_LOGIN_NAME", uname, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_PASSWORD", pwd, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_SOURCE_ADDRESS", sourceAddress, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_EIDA", eida, OracleDbType.Varchar2, ParameterDirection.Input);

                        if (token == null)
                        {
                            param.Add("P_TOKEN", null, OracleDbType.Varchar2, ParameterDirection.Input);
                        }
                        else
                        {
                            param.Add("P_TOKEN", token, OracleDbType.Varchar2, ParameterDirection.Input);
                        }
                        connection.Query("FN_UA_AUTH_LINK_SPASS", param: param, commandType: CommandType.StoredProcedure);
                        var rowCount = param.Get<dynamic>("@v_Return");
                        if (rowCount > 0)
                        {
                            result.Add("status", "OK");
                            result.Add("customerPKID", (int)rowCount);
                        }
                        else
                        {
                            result.Add("status", "ERROR");
                            result.Add("error", "INVALID_CREDS");
                        }
                    }
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Add("status", "ERROR");
                result.Add("reason", "SYSTEM_ERROR");

            }
            return result;
        }

        public JObject Logout(int customerPKID, string uname, string sourceAddress, string token)
        {
            JObject logoutresult = new JObject();
            int result = 0;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                    {
                        connection.Open();
                        var param = new OracleDynamicParameters();
                        param.Add("ReturnValue", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                        param.Add("P_CUST_ACCOUNT_PKID", customerPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_SOURCE_ADDRESS", sourceAddress, OracleDbType.Varchar2, ParameterDirection.Input);

                        if (token == null)
                        {
                            param.Add("P_TOKEN", null, OracleDbType.Varchar2, ParameterDirection.Input);
                        }
                        else
                        {
                            param.Add("P_TOKEN", token, OracleDbType.Varchar2, ParameterDirection.Input);
                        }
                        connection.Query("FN_UA_LOGOUT", param,
                                                commandType: CommandType.StoredProcedure);
                        var rowCount = param.Get<dynamic>("@ReturnValue");
                        result = Convert.ToInt32(rowCount.ToString());
                        logoutresult.Add("status", "OK");
                        if (result == -1)
                        {
                            logoutresult.Add("status", "ERROR");
                        }
                    }
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                logoutresult.Add("error", "SYSTEM_ERROR");
            }
            return logoutresult;
        }

        public void SetSpass(JObject loginDetails, string spassData)
        {
            dynamic loginMap = JsonConvert.DeserializeObject(loginDetails.ToString());
            string logMsg = "SpassCacheBean [SPASSDATA SET] as key = SPASS_LOGIN," + "_SPASS_" + loginMap.customerPKID;
            Logger.Debug(logMsg);
            dynamic spassDataMap = JsonConvert.DeserializeObject<JObject>(spassData);
            MOIWSClient moiClient = new MOIWSClient();
            String dataJSON = moiClient.doPersonInquiry((String)spassDataMap.idn);
            if (dataJSON == null)
            {
                string logMsg1 = "SpassCacheBean RETRY ONCEMORE";
                Logger.Debug(logMsg1);

                dataJSON = moiClient.doPersonInquiry((String)spassDataMap.idn);
                if (dataJSON == null) dataJSON = "{}";
            }
            spassDataMap.Add("moi", dataJSON);
            string logMsg2 = "CustomerProfile [SPASSDATA SET] as value = " + spassDataMap;
            Logger.Debug(logMsg2);
            GlobalCacheHelper.toCache("SPASS_LOGIN", "_SPASS_" + loginMap.customerPKID, spassData);
        }

        public JObject UpdateProfile(int customerPKID, string profileMap)
        {
            JObject result = new JObject();
            string concat = null;
            int vresult = 0;
            dynamic obj = JsonConvert.DeserializeObject<JObject>(profileMap);
            string email = obj.contactAddress.email;
            string mobile = obj.contactAddress.mobile;
            using (var transactionScope = new TransactionScope())
            {
                OracleConnection connection1 = null;
                OracleConnection connection2 = null;
                try
                {
                    connection1 = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString);
                    connection1.Open();
                    CommandDefinition cmd1 = new CommandDefinition("Update CUSTOMER_ADDRESS Set EMAIL = :email,MOBILE = :mobile Where ADDRESS_TYPE = 'CONTACT' and CUSTOMER_PKID = :customerPKID", new { email = email, mobile = mobile, customerPKID = customerPKID });
                    vresult = connection1.Execute(cmd1);
                    if (vresult < 0)
                    {
                        concat = "CONTACT_ADDRESS";
                    }
                    string firstnameen = obj.customerName.english.first;
                    string firstnamear = obj.customerName.arabic.first;
                    string lastnameen = obj.customerName.english.last;
                    string lastnamear = obj.customerName.arabic.last;
                    string eidano = obj.eida;
                    CommandDefinition cmd2 = new CommandDefinition("Update CUSTOMER Set FNAME_EN = :fnameen,FNAME_AR=:fnamear,LNAME_EN=:lnameen,LNAME_AR=:lnamear,EIDA_CARDNUMBER=:eida Where PKID = :customerPKID", new { fnameen = firstnameen, fnamear = firstnamear, lnameen = lastnameen, lnamear = lastnamear, eida = eidano, customerPKID = customerPKID });
                    vresult = connection1.Execute(cmd2);
                    if (vresult > 0)
                    {
                        result.Add("status", "OK");
                    }
                    else
                    {
                        concat = concat + "CUSTOMER_NAME";
                        result.Add("status", "ERROR");
                        result.Add("what", concat);
                        result.Add("reason", "NO_INFORMATION");
                    }
                    string loginid = obj.loginID;
                    connection2 = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcuserdb"].ConnectionString);
                    connection2.Open();
                    CommandDefinition cmd3 = new CommandDefinition("DELETE FROM WEB_CUSTOMER_REGISTER Where LOGIN_ID = :loginID or CUSTOMER_PKID =  :customerPKID", new { loginID = loginid, customerPKID = customerPKID });
                    connection2.Execute(cmd3);

                    string fname = obj.customerName.english.first;
                    string lname = obj.customerName.english.last;

                    if (fname == null)
                    {
                        fname = obj.customerName.arabic.first;
                        lname = obj.customerName.arabic.first;
                    }
                    string authproc = obj.authProc.ToString();
                    CommandDefinition cmd4 = new CommandDefinition("INSERT INTO WEB_CUSTOMER_REGISTER(LOGIN_ID, STATUS, AUTH_PROCEDURE, CUSTOMER_PKID, FNAME, LNAME, EMAIL_ID) VALUES(:loginID,:auto,:authproc,:customerPKID,:fname,:lname,:email)", new { loginID = loginid, auto = "AUTO", authproc = authproc, customerPKID = customerPKID, fname = fname, lname = lname, email = email });
                    connection2.Execute(cmd4);
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    connection1.Close();
                    connection2.Close();
                }

                return result;
            }
        }

        public JObject UpdatePassword(int accountPKID, string uname, string oldpwd, string newpwd, string sourceIdentifier)
        {
            JObject resultmap = new JObject();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
            {
                connection.Open();
                try
                {
                    using (var transactionscope = new TransactionScope())
                    {
                        var param = new OracleDynamicParameters();
                        param.Add("P_ACCOUNT_PKID", accountPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_PASSWORD_OLD", oldpwd, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_PASSWORD_NEW", newpwd, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_EVENT_SOURCE", sourceIdentifier, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("ReturnValue", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);

                        connection.Execute("FN_UA_UPDATE_CREDS",
                                              param,
                                              commandType: CommandType.StoredProcedure);
                        var rowCount = param.Get<dynamic>("@ReturnValue");
                        if (rowCount == 0)
                        {
                            resultmap.Add("status", "OK");
                        }
                        else
                        {
                            resultmap.Add("status", "ERROR");
                            resultmap.Add("reason", "INVALID_CREDS");
                        }
                        transactionscope.Complete();
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    resultmap.Add("status", "ERROR");
                    resultmap.Add("reason", "SYSTEM_ERROR");
                    throw new Exception(ex.Message);
                }
            }
            return resultmap;
        }

        public void DelinkSmartpass(string uaeID)
        {
            using (var transactionScope = new TransactionScope())
            {
                OracleConnection connection = null;
                OracleConnection connection1 = null;
                try
                {
                    using (connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))

                    using (connection1 = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcuserdb"].ConnectionString))
                    {
                        connection.Open();
                        connection1.Open();
                        connection.Query(@"UPDATE CUSTOMER_LOGIN SET smartpass_id = 'X-' || smartpass_id "
                           + " WHERE smartpass_id = :smartpass_id ", new { smartpass_id = uaeID }).ToList();
                        connection1.Query(@"UPDATE WEB_CUSTOMER_REGISTER SET login_id = 'X-' || login_id "
                  + " WHERE LOGIN_ID = :login_id ", new { login_id = uaeID }).ToList();

                        transactionScope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                    connection1.Close();
                }
            }
        }

        public List<CustomerPoboxEDIADetailsModel> GetCustomerInfoByEIDA(string uaeID)
        {
            var custDetailsCollection = new List<CustomerPoboxEDIADetailsModel>();
            OracleConnection connectionCoprdata = null;
            OracleConnection connectionPoboxPl = null;
            try
            {
                using (connectionCoprdata = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                using (connectionPoboxPl = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connectionPoboxPl.Open();
                    string queryCustDetailsByUaeId = @"select * from customer_login where smartpass_id = :smartpass_id";
                    var custData = connectionPoboxPl.Query(queryCustDetailsByUaeId, new { smartpass_id = uaeID }).FirstOrDefault();

                    if (custData != null)
                    {
                        string queryCustPoboxDetails = @"select lo.pkid,lo.smartpass_id,br.box_number,br.admin_office_city 
                                                            from customer_login lo 
                                                            inner join customer_login_link ll
                                                            on lo.pkid = ll.customer_login_pkid
                                                            inner join box_rent br
                                                            on br.customer_pkid = ll.customer_pkid
                                                            where lo.smartpass_id = :smartpass_id and br.status = 1";
                        dynamic CustPoboxDetails = connectionPoboxPl.Query(queryCustPoboxDetails, new { smartpass_id = uaeID }).ToList();

                        connectionCoprdata.Open();
                        foreach (var item in CustPoboxDetails)
                        {
                            string queryCustProfileData = @"select Replace(C.Customer_Name.Getstringval(), Chr(38)||'amp;', Chr(38)) Customer_Name
                                                           from cm_customer c where c.pkid = :pkid";
                            dynamic CustProfileDetails = connectionCoprdata.Query(queryCustProfileData, new { pkid = item.PKID }).FirstOrDefault();

                            dynamic customerName = CustProfileDetails.CUSTOMER_NAME;
                            dynamic customerNameParsed = XMLtoJsonConverter.GetJson(customerName);
                            string customerFullName = string.Empty;
                            if (customerName != null)
                            {
                                customerName = Json.Decode(customerNameParsed.CUSTOMER_NAME.ToString());
                                customerFullName = customerName.english.full;
                            }
                            custDetailsCollection.Add(new CustomerPoboxEDIADetailsModel()
                            {
                                CustomerName = customerFullName,
                                EidaNumber = uaeID,
                                PoboxEmirateNameEn = item.ADMIN_OFFICE_CITY != null ? Converters.GetEmirateNameById(item.ADMIN_OFFICE_CITY,"en") : "",
                                PoboxEmirateNameAr = item.ADMIN_OFFICE_CITY != null ? Converters.GetEmirateNameById(item.ADMIN_OFFICE_CITY, "ar") : "",
                                PoboxEmirateCode = item.ADMIN_OFFICE_CITY,
                                PoboxNumber = item.BOX_NUMBER
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Method: GetCustomerInfoByEIDA: " + ex.Message);
            }
            finally
            {
                connectionCoprdata.Close();
                connectionPoboxPl.Close();
            }
            return custDetailsCollection;
        }

    }
}






