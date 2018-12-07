using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SVC_CustomerManagement_Data.Extensions
{
    public class Converters
    {
        public static string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow[0]);
        }

        public static string GetEmirateNameById(string emiratesId, string lang)
        {
            string emirateNameEn = "";
            string emirateNameAr = "";
            switch (emiratesId)
            {
                case "1":
                    emirateNameEn = "Abu Dhabi";
                    emirateNameAr = "أبوظبي";
                    break;
                case "2":
                    emirateNameEn = "Dubai";
                    emirateNameAr = "دبي";
                    break;
                case "3":
                    emirateNameEn = "Sharjah";
                    emirateNameAr = "الشارقة";
                    break;
                case "4":
                    emirateNameEn = "Ajman";
                    emirateNameAr = "عجمان";
                    break;
                case "5":
                    emirateNameEn = "Umm al-Quwain";
                    emirateNameAr = "أم القيوين";
                    break;
                case "6":
                    emirateNameEn = "Ras al-Khaimah";
                    emirateNameAr = "رأس الخيمة";
                    break;
                case "7":
                    emirateNameEn = "Al Fujairah";
                    emirateNameAr = "الفجيرة";
                    break;
            }
            return (lang == "en") ? emirateNameEn : emirateNameAr;
        }
    }
}
