using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Debt
{

    public class HttpTask
    {
        private MultipartFormDataContent content= new MultipartFormDataContent();
        public delegate void OnSucceed(string result);
        public delegate void OnFailed();

        public HttpTask data(string key, string value)
        {
            content.Add(new StringContent(value), String.Format("\"{0}\"", key));
            return this;//链式调用
        }

        public HttpTask data(string filePath)
        {
            content.Add(new ByteArrayContent(File.ReadAllBytes(filePath)), "\"uploadFile\"", Path.GetFileName(filePath));
            return this;
        }

        public async Task postAsync(string url, OnSucceed a, OnFailed b)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {//判断请求是否成功
                    string responseBodyAsText = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine("请求成功！result=" + responseBodyAsText);//控制台打印服务器响应

                    // TODO:在这里统一处理全局error代码
                    if (Json.JsonStrToObject(responseBodyAsText) != 0)
                        Error_Msg.JsonStrToObject(responseBodyAsText);
                    else
                        a(responseBodyAsText);//调用接口通知请求成功
                }
                else
                    b();//调用接口通知请求失败
            }
        }

        //private void Login(string userName, string password)
        //{

        //    new HttpTask().data("userName", "0001")
        //                  .data("password", "123456")
        //                  .postAsync(Net.Url_UserLogin);
        //}*/
    }

}

