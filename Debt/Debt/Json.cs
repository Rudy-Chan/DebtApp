using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Debt
{
    class Json
    {
        public int error { get; set; }    //错误码
        public static int JsonStrToObject(string json)
        {//反序列化
            return JsonConvert.DeserializeObject<Json>(json).error;
        }
    }

    class Error_Msg
    {
        public string msg { get; set; }  //错误信息
        public static void JsonStrToObject(string json)
        {
            var model = JsonConvert.DeserializeObject<Error_Msg>(json);
            MessageBox.Show(model.msg, "异常提醒", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    class File_Single
    {
        public string id { get; set; }
        public string fileName { get; set; }
        public string fileSize { get; set; }
        public string uploadTime { get; set; }
        public File_Single(string id, string fileName, string fileSize, string uploadTime)
        {
            this.id = id;
            this.fileName = fileName;
            this.fileSize = fileSize;
            this.uploadTime = uploadTime;
        }
        public static File_Single JsonStrToObject(string json)
        {
            //反序列化
            return JsonConvert.DeserializeObject<File_Single>(json);
        }
    }

}
