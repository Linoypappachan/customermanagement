using Dapper;
using Oracle.ManagedDataAccess.Client;
using SVC_CustomerManagement_Data.Extensions;
using SVC_CustomerManagement_Domain.Models.CustomerList;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using SVC_CustomerManagement_Utilities.CustomerProfile;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class CustomerListRSData
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
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            return customer;
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
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return customerPKID;
        }

        public List<Customer> GetCustomerListByBusinessFunction(int businessFunctionPKID)
        {
            List<Customer> customer = null;
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query(@"SELECT "
                        + "ccb.BPKID BUSINESSFUNCTION_PROFILE, "
                        + "c.PKID, c.CUSTOMER_NUMBER, c.CUSTOMER_TYPE, "
                        + "replace(c.CUSTOMER_NAME.getStringVal(), chr(38),' and ') CUSTOMER_NAME, "
                        + "c.PERSONAL_PROFILE "
                        + " FROM CM_CUSTOMER c, CM_CUSTOMER_BPROFILE ccb "
                        + " WHERE c.pkid = ccb.pkid (+)"
                        + "   AND ccb.bpkid = :businessFunctionPKID", new { businessFunctionPKID = businessFunctionPKID }).ToList();
                    customer = CustomerTransformer.updateCustomerBusinessFunction(contacts);
                    return contacts.Count > 0 ? customer : null;

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

        }

        public dynamic GetCustomerListByANDFilter(string name, string email, string mobile, string eidano, string bfunctionPKID, string bfunctionCustomerID)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query<ListFilterResponse>(@"select * from TABLE(fn_search_customers_and_cond(:name,:email,:mobile,:eidano,:bfunctionPKID,:bfunctionCustomerID))", new { name = name, email = email, mobile = mobile, eidano = eidano, bfunctionPKID = bfunctionPKID, bfunctionCustomerID = bfunctionCustomerID }).ToList();
                    return contacts.Count > 0 ? contacts : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public dynamic GetCustomerListByORFilter(string name, string email, string mobile, string eidano, string bfunctionPKID, string bfunctionCustomerID)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query<ListFilterResponse>(@"select * from TABLE(fn_search_customers_or_cond(:name,:email,:mobile,:eidano,:bfunctionPKID,:bfunctionCustomerID))", new { name = name, email = email, mobile = mobile, eidano = eidano, bfunctionPKID = bfunctionPKID, bfunctionCustomerID = bfunctionCustomerID }).ToList();
                    return contacts.Count > 0 ? contacts : null;

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

        }

    }

}

