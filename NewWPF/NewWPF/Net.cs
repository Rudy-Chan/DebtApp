using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewWPF
{
    class Net
    {
        public static string Url_Login;
        public static string Url_Logout;
        public static string Url_getInformation;
        public static string Url_queryDebtList;

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
    }
}
