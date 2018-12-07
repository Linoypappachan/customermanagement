using Newtonsoft.Json.Linq;
using SVC_CustomerManagement_Domain.Models.CustomerProfile.Mapping;
using SVC_CustomerManagement_Utilities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SVC_CustomerManagement_Utilities.CustomerProfile
{
    public class CustomerTransformer
    {
        public static Customer updateCustomer(IEnumerable<dynamic> rs)
        {
            if (rs.Any())
            {
                dynamic resultSet = rs.First();
                Customer customer = new Customer();
                customer.pkid = (int)resultSet.PKID;
                customer.customerNumber = resultSet.CUSTOMER_NUMBER;
                customer.customerType = resultSet.CUSTOMER_TYPE;

                var customerName = XMLtoJsonConverter.GetJson(resultSet.CUSTOMER_NAME);
                if (customerName != null)
                {
                    customer.customerName = customerName.CUSTOMER_NAME;
                }

                var businessFunctionProfile = XMLtoJsonConverter.GetJson(resultSet.BUSINESSFUNCTION_PROFILE);
                if (businessFunctionProfile != null)
                {
                    if (businessFunctionProfile.BPROFILE.KEY.HasValues)
                    {
                        customer.businessFunctionProfile = businessFunctionProfile.BPROFILE.KEY;
                    }
                    else
                    {
                        customer.businessFunctionProfile = new JArray() { businessFunctionProfile.BPROFILE.KEY.Value };
                    }
                }

                var addressProfile = XMLtoJsonConverter.GetJson(resultSet.ADDRESS_PROFILE);
                if (addressProfile != null)
                {
                    customer.addressProfile = addressProfile.ADDRESS_PROFILE;
                }

                var personalProfile = XMLtoJsonConverter.GetJson(resultSet.PERSONAL_PROFILE);
                if (personalProfile != null)
                {
                    customer.personalProfile = personalProfile.PERSONAL_PROFILE;
                }

                var customerLinks = XMLtoJsonConverter.GetJson(resultSet.CUSTOMER_LINKS);
                if (customerLinks != null)
                {
                    try
                    {
                        if (customerLinks.CUSTOMER_LINKS.CUSTOMER.Type != null && customerLinks.CUSTOMER_LINKS.CUSTOMER.Type.ToString() == "Array")
                        {
                            customer.customerLinks = customerLinks.CUSTOMER_LINKS.CUSTOMER;
                        }
                        else
                        {
                            customer.customerLinks = new JArray() { customerLinks.CUSTOMER_LINKS.CUSTOMER };
                        }
                    }
                    catch (Exception)
                    {
                        customer.customerLinks = null;
                    }
                }
            
                var accountProfile = XMLtoJsonConverter.GetJson(resultSet.ACCOUNT_PROFILE);
                if (accountProfile != null)
                {
                    customer.accountProfile = accountProfile.ACCOUNT_PROFILE;
                }

                return customer;
            }
            return null;
        }
        public static List<Customer> updateCustomerBusinessFunction(IEnumerable<dynamic> rs)
        {
            List<Customer> customerList = new List<Customer>();
            for (int i = 0; i < rs.Count(); i++)
            {
                Customer customer = new Customer();
                dynamic resultSet = rs.ElementAt(i);
                customer.pkid = (int)resultSet.PKID;
                customer.customerNumber = resultSet.CUSTOMER_NUMBER;
                customer.customerType = resultSet.CUSTOMER_TYPE;

                var customerName = XMLtoJsonConverter.GetJson(resultSet.CUSTOMER_NAME);
                if (customerName != null)
                {
                    customer.customerName = customerName.CUSTOMER_NAME;
                }

                var businessFunctionProfile = resultSet.BUSINESSFUNCTION_PROFILE;
                if (businessFunctionProfile != null)
                {
                    customer.businessFunctionProfile = new JArray() { businessFunctionProfile };
                }

                var personalProfile = XMLtoJsonConverter.GetJson(resultSet.PERSONAL_PROFILE);
                if (personalProfile != null)
                {
                    customer.personalProfile = personalProfile.PERSONAL_PROFILE;
                }
                customerList.Add(customer);
            }
            return customerList;
        }
    }
}
