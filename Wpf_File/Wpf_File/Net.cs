using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Web;

namespace Wpf_File
{
    class Net
    {
        public static string IP
        {
            set
            {
                Url_FileUpload = @"http://" + value.Trim() + @"/api/file.php?act=upload";
                Url_FileView= @"http://" + value.Trim() + @"/api/file.php?act=getFileList";
                Url_FileDownload= @"http://" + value.Trim() + @"/api/file.php?act=download";
                Url_FileDelete = @"http://" + value.Trim() + @"/api/file.php?act=delete";
            }
        }

        public static string Url_FileUpload;
        public static string Url_FileView;
        public static string Url_FileDownload;
        public static string Url_FileDelete;
        

        /// <summary>
        /// 获取文件集合对应的ByteArrayContent集合
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static void GetFileMultipartContent(string file,string param, ref MultipartFormDataContent content)
        {
            content.Add(new ByteArrayContent(File.ReadAllBytes(file)), param.Trim(), Path.GetFileName(file));
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


        /// <summary>
        /// 从ListView中获取选择的文件集合
        /// </summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        public static HashSet<string> GetHashSet(ListView lv)
        {
            HashSet<string> hash = new HashSet<string>();
            var rows = lv.Items;
            foreach (var row in rows)
            {
                Lsv_SingleItem item = row as Lsv_SingleItem;
                if (item != null && item is Lsv_SingleItem)
                {
                    hash.Add(item.singleFile.fileOldName);
                }
            }
            return hash;
        }
    }
}
