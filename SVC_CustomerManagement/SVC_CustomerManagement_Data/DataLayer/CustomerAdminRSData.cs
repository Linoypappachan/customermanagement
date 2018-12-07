using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using SVC_CustomerManagement_Data.Extensions;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using SVC_CustomerManagement_Utilities.CustomerProfile;
using SVC_CustomerManagement_Domain.Models.CustomerAdmin;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using SVC_CustomerManagement_Domain.DBModel;
using SVC_CustomerManagement_Domain.Models;
using System.Xml;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class CustomerAdminRSData
    {
        public JObject CreateLoginAccount(int serviceCustomerPKID, string loginName, string password)
        {
            JObject obj = new JObject();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var param = new OracleDynamicParameters();
                        param.Add("P_CUSTOMER_PKID", serviceCustomerPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_UNAME", loginName, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_PWD", password, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                        connection.Execute("FN_AD_ACTIVATE_LOGIN",
                                             param,
                                             commandType: CommandType.StoredProcedure);
                        var result = param.Get<dynamic>("@v_Return");
                        obj.Add("accountPKID", result.ToString());
                        obj.Add("customerPKID", serviceCustomerPKID);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error(ex);
                        obj.Add("error", "Cannot Activate Account");
                        throw new Exception(ex.Message);
                    }
                }
            }
            return obj;
        }

        public CustomerRegistrationResponse GetRegistration(string regReference)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcuserdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query<CustomerRegistrationResponse>(@"SELECT * FROM WEB_CUSTOMER_REGISTER WHERE INTERNAL_REG_ID = :regReference", new { regReference = regReference }).ToList();
                    return contacts.Count > 0 ? contacts[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CustomerRegistrationResponse GetRegistrationByLoginID(string loginID)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcuserdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query<CustomerRegistrationResponse>(@"SELECT * FROM WEB_CUSTOMER_REGISTER WHERE LOGIN_ID = :loginID", new { loginID = loginID }).ToList();
                    return contacts.Count > 0 ? contacts[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public JObject IsLoginIdAvailable(string uname)
        {
            JObject resultMap = new JObject();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("@p_LOGINID", uname, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("@v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                    connection.Execute("FN_UA_IS_LOGINID_FREE",
                            param,
                            commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    if (rowCount == 0)
                    {
                        resultMap.Add("status", "AVAILABLE");
                    }
                    else
                    {
                        resultMap.Add("status", "NOT AVAILABLE");
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resultMap;
        }

        public bool IsPostBoxCustomerValid(string city, string boxNumber, string verification)
        {
            bool resultmap = false;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_BOXNUMBER", boxNumber, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_BOXADMINCITY", city, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_ID", verification, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                    connection.Execute("FN_UA_VERIFY_USER",
                             param,
                             commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    if (rowCount.ToString() == "0")
                    {
                        resultmap = true;
                    }
                    else
                    {
                        resultmap = false;
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return resultmap;
        }

        public bool IsEIDAValid(string eida)
        {
            bool resultmap = false;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_EIDA", eida, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);

                    connection.Execute("FN_UA_VERIFY_EIDA",
                               param,
                               commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    if (rowCount == 0)
                    {
                        resultmap = true;
                    }
                    else
                    {
                        resultmap = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resultmap;
        }

        public bool IsUserAccountValid(string loginname, string email)
        {
            bool resultmap = false;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_LOGINNAME", loginname, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_EMAIL", email, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                    connection.Execute("FN_UA_VERIFY_ACCOUNT",
                                             param,
                                             commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    if (rowCount == 0)
                    {
                        resultmap = true;
                    }
                    else
                    {
                        resultmap = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resultmap;
        }

        public dynamic GetAccountBySmartpassID(string smartpassID)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query(@"SELECT PKID, NVL(STATUS,'VALID') STATUS, LOGIN_NAME FROM CUSTOMER_LOGIN WHERE SMARTPASS_ID = :smartpassID", new { smartpassID = smartpassID }).ToList();
                    return contacts.Count > 0 ? contacts[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JObject IsLoggedInCurrently(string token, int customerPKID)
        {
            DateTime dt = DateTime.Now;
            JObject result = new JObject();
            string resultmap = null;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_CUST_ACCOUNT_PKID", customerPKID, OracleDbType.Int32, ParameterDirection.Input);
                    if (token == null)
                    {
                        param.Add("P_TOKEN", null, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    else
                    {
                        param.Add("P_TOKEN", token, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    param.Add("v_Return", dbType: OracleDbType.TimeStamp, direction: ParameterDirection.ReturnValue);

                    connection.Query("FN_UA_IS_LOGGED_IN", param: param, commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");

                    if (!rowCount.IsNull)
                    {
                        resultmap = rowCount.Value.ToString();
                        dt = DateTime.Parse(resultmap);
                        resultmap = dt.ToString("MMM d, yyyy h:mm:ss tt");
                        result.Add("last_accessed_time", resultmap);
                        result.Add("status", "YES");
                    }
                    else
                    {
                        result.Add("status", "NO");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public string RegisterCustomer(JObject validateQAMap)
        {
            String reference = null;
            dynamic value = JsonConvert.DeserializeObject<JObject>(validateQAMap.ToString());
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcuserdb"].ConnectionString))
            {
                connection.Open();
                try
                {
                    var contacts = connection.Query(@"SELECT SQ_CUSTOMER_REG.NEXTVAL FROM DUAL").ToList();
                    reference = "REGRQ" + contacts[0].NEXTVAL.ToString();

                    if (value.regForm != "" || value.regForm != null)
                    {
                        string loginid = value.loginID;
                        string fname = value.fName;
                        string lname = value.lName;
                        string emailid = value.emailID;
                        string regForm = value.regForm;
                        string status = "PENDING_AUTHPROC";
                        string Reference = reference;

                        string processQuery = @"INSERT INTO WEB_CUSTOMER_REGISTER
                            (LOGIN_ID, FNAME, LNAME, EMAIL_ID, REG_FORM, STATUS, INTERNAL_REG_ID)
                            VALUES(:loginID,:fName,:lName,:emailID,:regForm,:status,:reference)";

                        connection.Execute(processQuery,
                            new
                            {
                                loginID = loginid,
                                fName = fname,
                                lName = lname,
                                emailID = emailid,
                                regForm = regForm,
                                status = status,
                                reference = Reference
                            });
                    }
                    else
                    {
                        string processQuery = @"INSERT INTO WEB_CUSTOMER_REGISTER
                            (LOGIN_ID, FNAME, LNAME, EMAIL_ID, REG_FORM, STATUS, INTERNAL_REG_ID)
                            VALUES(:loginID,:fName,:lName,:emailID,:regForm,:status,:reference)";

                        connection.Execute(processQuery,
                            new
                            {
                                loginID = value.loginID,
                                fName = value.fName,
                                lName = value.lName,
                                emailID = value.emailID,
                                regForm = "null",
                                status = "PENDING_AUTHPROC",
                                reference = reference
                            });
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return reference;
        }

        public int CreatePostBoxAccount(string city, string boxNumber, string loginid, string fname, string lname)
        {
            int boxCustomerPkid = 0;
            int accountPkid = 0;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_CITY_ID", city, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_BOX_NUMBER", boxNumber, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                    connection.Execute("FN_BR_IMPORT_CUSTOMER",
                                            param,
                                            commandType: CommandType.StoredProcedure);
                    var res = param.Get<dynamic>("@v_Return");
                    boxCustomerPkid = Convert.ToInt32(res.Value);
                    param = new OracleDynamicParameters();
                    param.Add("P_BOXCUSTOMER_PKID", boxCustomerPkid, OracleDbType.Int32, ParameterDirection.Input);
                    connection.Execute("PR_BR_IMPORT_RENTDATA",
                                          param,
                                          commandType: CommandType.StoredProcedure);

                    param.Add("P_BOXCUSTOMER_PKID", boxCustomerPkid, OracleDbType.Int32, ParameterDirection.Input);
                    if (loginid == null)
                    {
                        param.Add("P_LOGIN_NAME", null, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    else
                    {
                        param.Add("P_LOGIN_NAME", loginid, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    if (fname == null)
                    {
                        param.Add("P_FNAME", null, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    else
                    {
                        param.Add("P_FNAME", fname, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    if (lname == null)
                    {
                        param.Add("P_LNAME", null, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    else
                    {
                        param.Add("P_LNAME", lname, OracleDbType.Varchar2, ParameterDirection.Input);
                    }
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                    connection.Execute("FN_UA_GENERATE_ACCOUNT",
                                            param,
                                            commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    accountPkid = Convert.ToInt32(rowCount.ToString());
                    return accountPkid;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CompleteRegistration(string regReference, string loginID, int customerPKID, string authProcedureJSON, string email)
        {
            string logMsg = "completeRegistration -> " + regReference + "/" + loginID + "/" + customerPKID + "/" + authProcedureJSON + "/" + email;
            Logger.Debug(logMsg);
            int numRowsAffected = 0;
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcuserdb"].ConnectionString))
            {
                connection.Open();
                try
                {
                    var param = new OracleDynamicParameters();

                    string query = " DELETE FROM WEB_CUSTOMER_REGISTER "
                         + " WHERE CUSTOMER_PKID = :customerPKID";
                    CommandDefinition cmd4 = new CommandDefinition(query, new { customerPKID = customerPKID });
                    connection.Execute(cmd4);

                    string query1 = " UPDATE WEB_CUSTOMER_REGISTER "
                       + " SET STATUS = 'COMPLETE', CUSTOMER_PKID = :customerPKID, AUTH_PROCEDURE = :jsonauthProcedureJSON"
                         + " WHERE INTERNAL_REG_ID = :regReference";
                    CommandDefinition cmd1 = new CommandDefinition(query1, new { customerPKID = customerPKID, jsonauthProcedureJSON = authProcedureJSON, regReference = regReference });
                    connection.Execute(cmd1);

                    string logMsg1 = "completeRegistration loginID:" + loginID;
                    Logger.Debug(logMsg1);

                    if (loginID != null)
                        try
                        {
                            {
                                using (OracleConnection connection1 = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                                {
                                    connection1.Open();
                                    String query2 = "UPDATE CUSTOMER_LOGIN "
                       + " SET LOGIN_NAME = :LoginName "
                         + " WHERE PKID = :pkid ";
                                    CommandDefinition cmd2 = new CommandDefinition(query2, new { pkid = customerPKID, LoginName = loginID });
                                    connection1.Execute(cmd2);

                                    String query3 = "UPDATE CUSTOMER_ADDRESS "
                      + " SET EMAIL = :email"
                        + " WHERE CUSTOMER_PKID = :customerPKID";
                                    CommandDefinition cmd3 = new CommandDefinition(query3, new { customerPKID = customerPKID, email = email });
                                    numRowsAffected = connection1.Execute(cmd3);
                                }
                            }
                            string logMsg2 = "completeRegistration customer login update" + loginID + ". Affected numRowsAffected = " + numRowsAffected;
                            Logger.Debug(logMsg2);
                        }

                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            throw new Exception(ex.Message);
                        }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            }
        }

        public JObject EmailCredentials(int customerAccountPKID, string email)
        {
            JObject resultmap = new JObject();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_CUSTOMER_PKID", customerAccountPKID, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("P_EMAIL_ADDRESS", email, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.NVarchar2, direction: ParameterDirection.ReturnValue, size: 500);
                    connection.Execute("FN_UA_EMAIL_CREDS",
                                          param,
                    commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    resultmap.Add("status", rowCount.Value);
                    string msg = "emailCredentials for " + customerAccountPKID + "to" + email + "is" + rowCount.Value;
                    Logger.Info(msg);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                resultmap.Add("status", "ERROR");
            }
            return resultmap;
        }

        public int CreateCustomer(int bfunctionPKID, string serviceCustomerID)
        {
            int serviceCustomerPKID = 0;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                    {
                        connection.Open();
                        var param = new OracleDynamicParameters();
                        param.Add("P_BFUNC_PKID", bfunctionPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_SERVICE_NUMBER", serviceCustomerID, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);

                        connection.Execute("FN_AD_UPS_CUSTOMER",
                                              param,
                                              commandType: CommandType.StoredProcedure);
                        var rowCount = param.Get<dynamic>("@v_Return");
                        serviceCustomerPKID = Convert.ToInt32(rowCount.Value);
                    }
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return serviceCustomerPKID;
        }

        public JObject AttachToLoginAccount(int accountCustomerPKID, int serviceCustomerPKID, int bfunctionPKID)
        {
            JObject creds = new JObject();
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                    {
                        connection.Open();
                        var param = new OracleDynamicParameters();
                        param.Add("P_ACCOUNT_PKID", accountCustomerPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_CUSTOMER_PKID", serviceCustomerPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_BFUNC_PKID", bfunctionPKID, OracleDbType.Int32, ParameterDirection.Input);

                        connection.Execute("PR_AD_LINK_TO_ACCOUNT", param, commandType: CommandType.StoredProcedure);
                        creds.Add("accountPKID", accountCustomerPKID);
                        creds.Add("customerPKID", serviceCustomerPKID);
                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                creds.Add("error", "Cannot Attach Account");
                throw new Exception(ex.Message);
            }
            return creds;
        }

        public bool AttachPoboxToLoginAccount(int accountCustomerPKID, string cityID, string boxNumber)
        {
            string logMsg = "linkPostBoxToAccount :" + accountCustomerPKID + ":" + cityID + ":" + boxNumber;
            Logger.Info(logMsg);
            int boxCustomerPkid = 0;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_CITY_ID", cityID, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_BOX_NUMBER", boxNumber, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                    connection.Execute("FN_BR_IMPORT_CUSTOMER",
                                            param,
                                            commandType: CommandType.StoredProcedure);
                    var res = param.Get<dynamic>("@v_Return");
                    boxCustomerPkid = Convert.ToInt32(res.Value);

                    param = new OracleDynamicParameters();
                    param.Add("P_BOXCUSTOMER_PKID", boxCustomerPkid, OracleDbType.Int32, ParameterDirection.Input);
                    connection.Execute("PR_BR_IMPORT_RENTDATA",
                                          param,
                                          commandType: CommandType.StoredProcedure);
                    param = new OracleDynamicParameters();
                    param.Add("P_BOXCUSTOMERPKID", boxCustomerPkid, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("P_ACCOUNTPKID", accountCustomerPKID, OracleDbType.Int32, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                    connection.Execute("FN_UA_LINK_TO_ACCOUNT",
                                            param,
                                            commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    Object msg = "linkPostBoxToAccount:linkPostBoxToAccount Returned:" + accountCustomerPKID + ":" + boxCustomerPkid + ":" + rowCount.Value;
                    Logger.Info(msg);
                    return rowCount.Value != 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }

        public bool DettachPoboxFromLoginAccount(int accountCustomerPKID, string cityID, string boxNumber)
        {
            string logMsg = "delinkPostBoxFromAccount :" + accountCustomerPKID + ":" + cityID + ":" + boxNumber;
            Logger.Debug(logMsg);
            bool result = false;
            int boxCustomerPkid = 0;
            string box_Pkid = null;
            int returnvalue = 0;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        var param = new OracleDynamicParameters();
                        param.Add("P_CITY_ID", cityID, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("P_BOX_NUMBER", boxNumber, OracleDbType.Varchar2, ParameterDirection.Input);
                        param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                        connection.Execute("FN_BR_IMPORT_CUSTOMER",
                                                param,
                                                commandType: CommandType.StoredProcedure);
                        var res = param.Get<dynamic>("@v_Return");
                        boxCustomerPkid = Convert.ToInt32(res.Value);
                    }
                    catch (Exception)
                    {
                        var param2 = new OracleDynamicParameters();
                        param2.Add("P_CITY_ID", cityID, OracleDbType.Varchar2, ParameterDirection.Input);
                        param2.Add("P_BOX_NUMBER", boxNumber, OracleDbType.Varchar2, ParameterDirection.Input);
                        param2.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                        connection.Execute("FN_BD_GET_BOXPKID",
                                                param2,
                                                commandType: CommandType.StoredProcedure);
                        var res2 = param2.Get<dynamic>("@v_Return");
                        box_Pkid = res2.Value.ToString();
                        var contacts = connection.Query(@"select customer_pkid from box_rent where box_pkid = :box_pkid and status = 1", new { box_Pkid = box_Pkid }).ToList();
                        boxCustomerPkid = Int32.Parse(contacts[0].CUSTOMER_PKID.ToString());
                    }
                    try
                    {
                        var param1 = new OracleDynamicParameters();
                        param1.Add("P_BOXCUSTOMERPKID", boxCustomerPkid, OracleDbType.Int32, ParameterDirection.Input);
                        param1.Add("P_ACCOUNTPKID", accountCustomerPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param1.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);
                        connection.Execute("FN_UA_UNLINK_FROM_ACCOUNT",
                                                param1,
                                                commandType: CommandType.StoredProcedure);
                        var rowCount1 = param1.Get<dynamic>("@v_Return");
                        returnvalue = Convert.ToInt32(rowCount1.Value);
                        if (returnvalue == 0)
                            result = false;
                        else
                            result = true;
                        Object msg = "delinkPostBoxFromAccount Returned" + accountCustomerPKID + cityID + boxNumber + returnvalue;
                        Logger.Debug(msg);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return result;
        }

        public JObject DettachFromLoginAccount(int accountCustomerPKID, int serviceCustomerPKID, int bfunctionPKID)
        {
            JObject creds = new JObject();
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                    {
                        connection.Open();
                        var param = new OracleDynamicParameters();
                        param.Add("P_ACCOUNT_PKID", accountCustomerPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_CUSTOMER_PKID", serviceCustomerPKID, OracleDbType.Int32, ParameterDirection.Input);
                        param.Add("P_BFUNC_PKID", bfunctionPKID, OracleDbType.Int32, ParameterDirection.Input);
                        connection.Execute("PR_AD_UNLINK_FROM_ACCOUNT",
                                              param,
                                              commandType: CommandType.StoredProcedure);
                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                creds.Add("error", "Cannot Attach Account");
                throw new Exception(ex.Message);
            }
            return creds;
        }

        public int GetCustomerPKIDByUname(string uname)
        {
            int customerPKID = 0;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query(@"SELECT pkid FROM CUSTOMER_LOGIN WHERE login_name = :uname", new { uname = uname }).ToList();
                    customerPKID = Int32.Parse(contacts[0].PKID.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customerPKID;
        }

        public Customer GetCustomerByPKID(int customerPKID)
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
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customer;
        }

        public JObject ForgotCredentials(string loginName, string email)
        {
            JObject resultmap = new JObject();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_LOGIN_NAME", loginName, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_EMAIL_ADDRESS", email, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.NVarchar2, direction: ParameterDirection.ReturnValue, size: 500);
                    connection.Execute("FN_UA_EMAIL_CREDS_FORGOT",
                                          param,
                                          commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    resultmap.Add("status", rowCount.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return resultmap;
        }

        public JObject SmsCredentials(int customerPKID, string mobile)
        {
            JObject resultmap = new JObject();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_CUSTOMER_PKID", customerPKID, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_SMS_ADDRESS", mobile, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.NVarchar2, direction: ParameterDirection.ReturnValue, size: 500);
                    connection.Execute("FN_UA_SMS_CREDS",
                                          param,
                                          commandType: CommandType.StoredProcedure);
                    var rowCount = param.Get<dynamic>("@v_Return");
                    resultmap.Add("status", rowCount.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resultmap;
        }

        public int GenerateAccount(string loginID, string userFNameEN, string userLNameEN,
                                    string userFNameAR, string userLNameAR,
                                    string userFullNameEN, string userFullNameAR, string emailID,
                                    string mobile, string eida, string lang,
                                    string smartpassID)
        {
            int customerPKID = 0;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    connection.Open();
                    var param = new OracleDynamicParameters();
                    param.Add("P_LOGIN_NAME", loginID, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_NAME_EN", userFullNameEN, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_NAME_AR", userFullNameAR, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_FNAME_EN", userFNameEN, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_LNAME_EN", userLNameEN, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_FNAME_AR", userFNameAR, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_LNAME_AR", userLNameAR, OracleDbType.Varchar2, ParameterDirection.Input);

                    param.Add("P_EMAIL", emailID, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_MOBILE", mobile, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_EIDA", eida, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_LANG", lang, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("P_SMARTPASS_ID", smartpassID, OracleDbType.Varchar2, ParameterDirection.Input);
                    param.Add("v_Return", dbType: OracleDbType.Int32, direction: ParameterDirection.ReturnValue);

                    connection.Execute("FN_UA_CREATE_ACCOUNT",
                                          param,
                                          commandType: CommandType.StoredProcedure);
                    var result = param.Get<dynamic>("@v_Return");
                    customerPKID = Convert.ToInt32(result.ToString());
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customerPKID;
        }

        public Dictionary<string, string> RegisterFunction(JObject validateQAMap, RegisterCustomerModel model)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                string registerCustomer = RegisterCustomer(validateQAMap);
                response.Add("registerCustomer", registerCustomer);
                dynamic obj = JsonConvert.DeserializeObject<JObject>(model.regForm);
                string pboxcity = obj.pboxcity;
                string pboxnumber = obj.pboxnumber;
                int customerAccountPKID = CreatePostBoxAccount(pboxcity, pboxnumber, model.loginID, model.fName, model.lName);
                response.Add("customerAccountPKID", customerAccountPKID.ToString());
                if (customerAccountPKID != -1)
                {
                    CompleteRegistration(registerCustomer, model.loginID, customerAccountPKID, "{\"authType\":\"PWD_ONLY\"}", model.emailID);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int RegisterCorporateCustomer(CoporateCustomerModel model)
        {
            int? corporateCustomerPkid = 0;
            try
            {
                string customer_number = model.customer_number_prefix + "_" + model.account_cust_pkid.ToString();
                CUSTOMER customer = new CUSTOMER()
                {
                    FNAME_EN = model.fname_en,
                    FNAME_AR = model.full_name_ar,
                    LNAME_EN = model.lname_en,
                    LNAME_AR = model.lname_ar,
                    CUSTOMER_NAME = model.company_name,
                    LANGUAGE = model.lang,
                    EIDA_CARDNUMBER = model.eida,
                    CUSTOMER_TYPE = "CORPORATE",
                    CUSTOMER_NUMBER = customer_number
                };

                CUSTOMER_ADDRESS custAddress = new CUSTOMER_ADDRESS()
                {
                    EMAIL = model.email_id,
                    MOBILE = model.mobile,
                    CUSTOMER_PKID = model.account_cust_pkid
                };

                CUSTOMER_CORPORATE corporateCustomer = new CUSTOMER_CORPORATE()
                {
                    COMPANY_NAME = model.company_name,
                    MANAGER = model.fname_en,
                    MANAGER_MOBILE = model.mobile
                };

                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvcpldb"].ConnectionString))
                {
                    cn.Open();

                    //check customer is already imported wit same customer number
                    var customerDataList = cn.GetList<CUSTOMER>(new { CUSTOMER_NUMBER = customer_number }).ToList();
                    if (customerDataList.Any())
                    {
                        return customerDataList.First().PKID;
                    }

                    string corporateCustPkidQuery = @"select seq_customerpk.nextval from dual";
                    var corporateCustPkid = cn.Query(corporateCustPkidQuery);
                    customer.PKID = (int)corporateCustPkid.First().NEXTVAL;
                    cn.Insert(customer);

                    string corporateCustAddrPkidQueryForContact = @"select seq_addresspk.nextval from dual";
                    var corporateCustAddrPkidForContact = cn.Query(corporateCustAddrPkidQueryForContact);
                    custAddress.PKID = (int)corporateCustAddrPkidForContact.First().NEXTVAL;
                    custAddress.ADDRESS_TYPE = "CONTACT";
                    cn.Insert(custAddress);

                    string corporateCustAddrPkidQueryForBusiness = @"select seq_addresspk.nextval from dual";
                    var corporateCustAddrPkidForBusiness = cn.Query(corporateCustAddrPkidQueryForBusiness);
                    custAddress.PKID = (int)corporateCustAddrPkidForBusiness.First().NEXTVAL;
                    custAddress.ADDRESS_TYPE = "BUSINESS";
                    cn.Insert(custAddress);

                    corporateCustomerPkid = corporateCustomer.PKID = (int)corporateCustPkid.First().NEXTVAL;
                    cn.Insert(corporateCustomer);

                    var accountCustomer = cn.GetList<CUSTOMER>(new { PKID = model.account_cust_pkid }).First();
                    accountCustomer.CUSTOMER_NUMBER = customer_number;
                    cn.Update(accountCustomer);

                    cn.Close();
                }

                string xmlString = @"<bfunction_list>
                                        <bfunction>
                                            <link id='k2_customer'>
                                                <attr entity='customer' key='customer_id' value='" + corporateCustomerPkid + @"'/>
                                            </link>
                                            <privilege pkid='1'/>
                                        </bfunction>
                                    </bfunction_list>";

                CM_CUSTOMER_BPROFILE bprofile = new CM_CUSTOMER_BPROFILE()
                {
                    PKID = model.account_cust_pkid,
                    BPKID = model.bpkid,
                    BVALUE = xmlString,
                    LINK_CUSTOMER_ID = corporateCustomerPkid.ToString()
                };

                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    cn.Open();
                    cn.Insert(bprofile);
                    cn.Close();
                }
                return corporateCustomerPkid.Value;
            }
            catch (Exception ex)
            {
                Logger.Error("Address book Add: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
