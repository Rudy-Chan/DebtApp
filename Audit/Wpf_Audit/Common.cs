using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Audit
{
    class Common
    {
        public static string ConvertSize(int length)
        {
            string newSize = "";
            double size = (int)(length / 1024.0 / 1024.0 * 100) / 100.0;
            if (size < 1)
            {
                size = (int)(length / 1024.0 * 100) / 100.0;
                newSize = size + "KB";
            }
            else if (size > 1000)
            {
                size = (int)(length / 1024.0 / 1024.0 / 1024.0 * 10) / 10.0;
                newSize = size + "GB";
            }
            else
            {
                newSize = size + "MB";
            }
            return newSize;
        }
    }
    class Is_CheckedAll
    {
        public bool isCheckedAll { get; set; }
    }

    class Dg_AuditItem
    {
        public int rowIndex { get; set; }
        public Json_AuditPassItem singleRow { get; set; }
        public string amount{ get; set; }
        public string auditUser { get; set; }
    }

    class Dg_NotAuditItem
    {
        public int rowIndex { get; set; }
        public bool checkBox_IsChecked { get; set; }
        public Json_AuditApplyItem singleRow { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
    }

    class Lsv_SingleItem
    {
        public int rowIndex { get; set; }

        public bool checkBox_IsChecked { get; set; }

        public Json_File singleFile { get; set; }
    }


    class Dg_ChangedItem
    {
        public int rowIndex { get; set; }
        public bool checkBox_IsChecked { get; set; }
        public Json_AuditChangeItem changeItem { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
    }
}
