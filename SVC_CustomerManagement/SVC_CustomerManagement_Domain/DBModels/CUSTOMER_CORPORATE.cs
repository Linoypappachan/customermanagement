using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SVC_CustomerManagement_Domain.DBModel
{
    public class CUSTOMER_CORPORATE
    {
        public int PKID { get; set; }
        public string MANAGER { get; set; }
        public string LICENSE_ISSUE_PLACE { get; set; }
        public string LICENSE_ISSUE_CITY { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_ACTIVITY { get; set; }
        public int LICENSE_ISSUE_YEAR { get; set; }
        public int LICENSE_ISSUE_DAY { get; set; }
        public string MANAGER_MOBILE { get; set; }
        public string SPONSOR_NAME { get; set; }
        public string LICENSE_NUMBER { get; set; }
        public string SECTOR_TYPE { get; set; }
        public int NUM_EMPLOYEES { get; set; }
        public int LICENSE_ISSUE_MONTH { get; set; }

        public void SetLicenseIssueDate(string licenseIssueDate)
        {
            DateTime licenseDate = Convert.ToDateTime(licenseIssueDate);
            this.LICENSE_ISSUE_DAY = licenseDate.Day;
            this.LICENSE_ISSUE_MONTH = licenseDate.Month;
            this.LICENSE_ISSUE_YEAR = licenseDate.Year;
        }
    }
}