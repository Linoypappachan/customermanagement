using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System;
using Dapper;
using SVC_CustomerManagement_Utilities.LoggerUtil;
using SVC_CustomerManagement_Domain.DBModels;

namespace SVC_CustomerManagement_Data.DataLayer
{
    public class CustomerAddressBookRSData
    {

        public CM_CUSTOMER_ADDR_BOOK Get(int id)
        {
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    cn.Open();
                    CM_CUSTOMER_ADDR_BOOK data = cn.Get<CM_CUSTOMER_ADDR_BOOK>(id);
                    cn.Close();
                    return data;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Address book get: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CM_CUSTOMER_ADDR_BOOK> GetByCustomerPkid(int customerPkid)
        {
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    cn.Open();
                    var data = cn.GetList<CM_CUSTOMER_ADDR_BOOK>(new { CUSTOMER_PKID = customerPkid, ISACTIVE = "Y" }).ToList();
                    var sortedList = from item in data
                                     orderby item.CREATED_ON descending
                                     select item;
                    cn.Close();
                    return sortedList;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Address book GetByCustomerPkid: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }


        public CM_CUSTOMER_ADDR_BOOK GetDefaultAddressDetails(int customerPkid)
        {
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    cn.Open();
                    CM_CUSTOMER_ADDR_BOOK data = cn.GetList<CM_CUSTOMER_ADDR_BOOK>(new { CUSTOMER_PKID = customerPkid,  IS_DEFAULT_ADDR = "Y", ISACTIVE = "Y" }).FirstOrDefault();
                    cn.Close();
                    return data;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Address book GetDefaultAddressDetails: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public int Add(CM_CUSTOMER_ADDR_BOOK model)
        {
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    cn.Open();
                    model.IS_DEFAULT_ADDR = model.IS_DEFAULT_ADDR.ToUpper();
                    model.CREATED_ON = DateTime.Now;
                    if (model.IS_DEFAULT_ADDR.ToUpper() == "Y")
                    {
                        object condition = new { CUSTOMER_PKID = model.CUSTOMER_PKID };
                        IEnumerable<CM_CUSTOMER_ADDR_BOOK> list = cn.GetList<CM_CUSTOMER_ADDR_BOOK>(condition);
                        list = list.Select(c => { c.IS_DEFAULT_ADDR = "N"; return c; });
                        foreach (var item in list)
                        {
                            cn.Update(item);
                        }
                    }
                    int? id = cn.Insert(model);
                    cn.Close();
                    return id.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Address book Add: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public bool Update(int id, CM_CUSTOMER_ADDR_BOOK model)
        {
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    cn.Open();
                    if (model.IS_DEFAULT_ADDR.ToUpper() == "Y")
                    {
                        object condition = new { CUSTOMER_PKID = model.CUSTOMER_PKID };
                        IEnumerable<CM_CUSTOMER_ADDR_BOOK> list = cn.GetList<CM_CUSTOMER_ADDR_BOOK>(condition);
                        list = list.Select(c => { c.IS_DEFAULT_ADDR = "N"; return c; });
                        foreach (var item in list)
                        {
                            cn.Update(item);
                        }
                    }

                    CM_CUSTOMER_ADDR_BOOK customerAddress = cn.Get<CM_CUSTOMER_ADDR_BOOK>(id);
                    if (customerAddress != null)
                    {
                        customerAddress.CUSTOMER_PKID = model.CUSTOMER_PKID;
                        customerAddress.ADDRESS_TYPE = model.ADDRESS_TYPE;
                        customerAddress.IS_DEFAULT_ADDR = model.IS_DEFAULT_ADDR;
                        customerAddress.CONTACT_NAME = model.CONTACT_NAME;
                        customerAddress.COMPANY_NAME = model.COMPANY_NAME;
                        customerAddress.ADDRESS = model.ADDRESS;
                        customerAddress.ORGIN = model.ORGIN;
                        customerAddress.ORGIN_CITY = model.ORGIN_CITY;
                        customerAddress.POBOX = model.POBOX;
                        customerAddress.CONTACT_PHONE = model.CONTACT_PHONE;
                        customerAddress.CONTACT_MOBILE = model.CONTACT_MOBILE;
                        customerAddress.LATITUDE = model.LATITUDE;
                        customerAddress.LONGITUDE = model.LONGITUDE;
                        customerAddress.EMAIL = model.EMAIL;
                        customerAddress.UPDATED_ON = DateTime.Now;
                        customerAddress.NICK_NAME = model.NICK_NAME;
                        customerAddress.ISACTIVE = model.ISACTIVE;
                        customerAddress.COUNTRY = model.COUNTRY;
                        cn.Update(customerAddress);
                        cn.Close();
                        return true;
                    }
                    else
                    {
                        Logger.Error("Address book update: No Item exsist");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Address book update: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                {
                    cn.Open();
                    CM_CUSTOMER_ADDR_BOOK customerAddress = cn.Get<CM_CUSTOMER_ADDR_BOOK>(id);
                    if (customerAddress != null)
                    {
                        customerAddress.ISACTIVE = "N";
                        cn.Update(customerAddress);
                        // cn.Delete(customerAddress);
                        cn.Close();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Address book delete: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public bool BulkInsert(List<CM_CUSTOMER_ADDR_BOOK> customerAddressList)
        {
            try
            {
                if (customerAddressList != null)
                {
                    customerAddressList.Select(c => { c.CREATED_ON = DateTime.Now; return c; }).ToList();
                    using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["esvccorpdb"].ConnectionString))
                    {
                        cn.Open();
                        //DapperPlusManager.Entity<CM_CUSTOMER_ADDR_BOOK>().Identity(x => x.ID);
                        //var result = cn.BulkInsert<CM_CUSTOMER_ADDR_BOOK>(customerAddressList);
                        customerAddressList.ForEach(row => {
                            cn.Insert(row);
                        });
                        cn.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Address book bulk insert: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}

