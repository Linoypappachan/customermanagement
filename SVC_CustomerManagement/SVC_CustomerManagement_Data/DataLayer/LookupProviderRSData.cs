using Dapper;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using SVC_CustomerManagement_Domain.Models.LookupProvider;
using SVC_CustomerManagement_Utilities.Helper;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using System;
using System.Configuration;
using System.Linq;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class LookupProviderRSData
    {
        public dynamic GetCustomerListByFilter(string name, string email, string mobile, string bfunctionCustomerID)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query(@"SELECT * FROM ORAF_CUSTOMER WHERE CUSTOMER_NAME like :name AND CUSTOMER_NUMBER like :bfunctionCustomerID", new { name = "%" + name + "%", bfunctionCustomerID = "%" + bfunctionCustomerID + "%" }).ToList();
                    return contacts.Count > 0 ? contacts[0] : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public dynamic GetSupplierListByFilter(string name, string email, string mobile, string bfunctionCustomerID)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query<ServiceListResponse>(@"SELECT * FROM ORAF_SUPPLIER WHERE VENDOR_NAME like :name AND VENDOR_NUMBER like :bfunctionCustomerID", new { name = "%" + name + "%", bfunctionCustomerID = "%" + bfunctionCustomerID + "%" }).ToList();
                    return contacts.Count > 0 ? contacts[0] : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public dynamic GetServiceList(int customerPKID, int businessFunctionPKID)
        {
            JArray ServiceList = new JArray();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query(@"SELECT PKID,
                                                      c.DESCRIPTION.getStringVal() DESCRIPTION,
                                                      c.FEATURES.getStringVal() FEATURES,
                                                      IS_LOCAL 
                                                      FROM BM_SERVICE c 
                                                      WHERE c.pkid IN (
                                                        SELECT x.service FROM CM_CUSTOMER_BPROFILE ccb, 
                                                              XMLTABLE ( 
                                                                 '/bfunction_list/bfunction/service'  
                                                                   passing ccb.BVALUE 
                                                                   columns service VARCHAR2(20) PATH '@pkid'
                                                              ) x 
                                                         WHERE ccb.bpkid = :businessFunctionPKID
                                                         AND ccb.pkid = :customerPKID
                                                      ) 
                                                      ORDER BY PKID ",
                  new { businessFunctionPKID = businessFunctionPKID, customerPKID = customerPKID }).ToList();

                    contacts.ForEach(x =>
                    {
                        ServiceList.Add(new JObject() {
                            { "pkid", x.PKID.ToString() },
                            { "description", XMLtoJsonConverter.GetJson(x.DESCRIPTION)["DESCRIPTION"] },
                            { "features", XMLtoJsonConverter.GetJson(x.FEATURES)["FEATURES"] },
                            { "is_local",x.IS_LOCAL }

                        });
                    });

                    return ServiceList.Count > 0 ? ServiceList : null;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
        public dynamic GetBusinessFunctionList()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    connection.Open();
                    var contacts = connection.Query(@"SELECT c.PKID, c.DESCRIPTION.getStringVal() DESCRIPTION,
                                                     c.PRIVILEGE_LIST.getStringVal() PRIVILEGE_LIST 
                                                     FROM CM_BUSINESSFUNCTION c").ToList();

                    JArray businessFnList = new JArray();
                    contacts.ForEach(x =>
                    {
                        businessFnList.Add(new JObject() {
                            { "pkid",x.PKID.ToString() },
                            { "description", XMLtoJsonConverter.GetJson(x.DESCRIPTION)["DESCRIPTION"] },
                            { "privilege_list", XMLtoJsonConverter.GetJson(x.PRIVILEGE_LIST)["PRIVILEGE_LIST"]["privilege"] }

                        });
                    });

                    return businessFnList.Count > 0 ? businessFnList : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
