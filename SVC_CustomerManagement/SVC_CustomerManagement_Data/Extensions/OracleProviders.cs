using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Data.Extensions
{
    public class OracleProviders
    {
        public static string GetConnectionString(DBConnections name)
        {
            string connectionString = string.Empty;
            switch (name)
            {
                case DBConnections.esvcuserdb:
                    connectionString = "Persist Security Info=False;User ID=epweb_portal;Password=E9eWb_90rtA$;Data Source=10.1.9.211:1521/esvdb;";
                    break;
                case DBConnections.esvcdb:
                    connectionString = "Persist Security Info=False;User ID=esvc_pbox_data;Password=E$vc_P0B0xDo1A;Data Source=10.1.9.211:1521/opsdev;";
                    break;
                case DBConnections.messagedb:
                    connectionString = string.Empty;
                    break;
                case DBConnections.esvccorpdb:
                   // connectionString = "Persist Security Info=False;User ID=esvc_pbox_data;Password=E$vc_C0r9_Do1A;Data Source=10.1.9.211:1521/opsdev;";
                    connectionString = "Persist Security Info=False;User ID=esvc_corp_data;Password=E$vc_C0r9_Do1A;Data Source=10.1.4.107:1521/opsdb;";
                    break;
                default:
                    break;
            }
            return connectionString;
        }
    }

    public enum DBConnections
    {
        esvcuserdb = 0,
        esvcdb = 1,
        messagedb = 2,
        esvccorpdb = 3
    }
}
