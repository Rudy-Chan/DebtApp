using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

namespace Wpf_File
{
    class Json_Msg
    {
        public int error { get; set; }
        public string msg { get; set; }
        public static Json_Msg JsonStrToList(string json)
        {
            //反序列化
            Json_Msg model = new Json_Msg();
            model = JsonConvert.DeserializeObject<Json_Msg>(json);
            return model;
        }

        public static void ShowMsg(Json_Msg jmsg)
        {
            MessageBox.Show("错误代码:" + jmsg.error + "  " + jmsg.msg, "异常信息", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    class Json_Upload
    {
        public int error { get; set; }
        public string path { get; set; }

        public static Json_Upload JsonStrToList(string json)
        {
            //反序列化
            Json_Upload model = new Json_Upload();
            model = JsonConvert.DeserializeObject<Json_Upload>(json);
            return model;
        }
    }

    class Json_File
    {
        public string id { get; set; }
        public string fileOldName { get; set; }
        public string fileSize { get; set; }
        public DateTime uploadTime { get; set; }
    }

    class Json_FilesView
    {
        public int error { get; set; }
        public List<Json_File> data { get; set; }

        public static Json_FilesView JsonStrToList(string json)
        {
            //反序列化
            Json_FilesView model = new Json_FilesView();
            model = JsonConvert.DeserializeObject<Json_FilesView>(json);
            return model;
        }
    }

    class Json_Download
    {
        public int error { get; set; }
        public string url { get; set; }

        public static Json_Download JsonStrToList(string json)
        {
            //反序列化
            Json_Download model = new Json_Download();
            model = JsonConvert.DeserializeObject<Json_Download>(json);
            return model;
        }
    }

    class Json_Delete
    {
        public int error { get; set; }
        public string url { get; set; }

        public static Json_Delete JsonStrToList(string json)
        {
            //反序列化
            Json_Delete model = new Json_Delete();
            model = JsonConvert.DeserializeObject<Json_Delete>(json);
            return model;
        }
    }
}
