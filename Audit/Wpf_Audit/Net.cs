using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Wpf_Audit
{
    class Net
    {
        public static string Url_Logout;
        public static string Url_ResetPassword;
        public static string Url_ChangeUserInfo;

        public static string Url_FileUpload;
        public static string Url_FileView;
        public static string Url_FileDownload;
        public static string Url_FileDelete;

        public static string Url_DebtChecked;
        public static string Url_DebtNotChecked;
        public static string Url_DebtChanged;
        public static string Url_DebtApplyPass;
        public static string Url_DebtRejectApply;

        public static string Url_SingleApplyInfo;
        public static string Url_SingleChangeInfo;

        public static string Url_BondInstitutions;
        public static string Url_OperationLogInfo;

        /// <summary>
        /// 获取键值集合对应的ByteArrayContent集合
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static void GetKeyValueMultipartContent(List<KeyValuePair<string, string>> collection, ref MultipartFormDataContent content)
        {
            foreach (var keyValuePair in collection)
            {
                content.Add(new StringContent(keyValuePair.Value),
                String.Format("\"{0}\"", keyValuePair.Key));
            }
        }


        /// <summary>
        /// 获取userId键值对集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static void GetKeyValuePairList(string key, string value, ref List<KeyValuePair<string, string>> list)
        {
            try
            {
                if (value != null)
                {
                    list.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            catch { }//忽略异常，不检测是否存在重复的键值
        }
    }
}
