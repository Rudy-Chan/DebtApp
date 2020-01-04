using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace NewWPF
{
    class HttpTask
    {
        private List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
        public delegate void OnSucceed(string result);
        public delegate void OnFailed();
        public HttpTask data(string key, string value)
        {
            //var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>(key, value));
            return this;//链式调用
        }

        //public interface IListener
        //{
        //    void onSucceed(string result);//请求成功
        //    void onFailed();//请求失败
        //}

        public async Task postAsync(string url, OnSucceed a, OnFailed b)
        {
            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                Net.GetKeyValueMultipartContent(list, ref content);
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {//判断请求是否成功
                    string responseBodyAsText = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("请求成功！result=" + responseBodyAsText);//控制台打印服务器响应
                                                                           //================================
                                                                           // TODO:在这里统一处理全局error代码
                                                                           //================================
                    a(responseBodyAsText);//调用接口通知请求成功
                }
                else
                    b();//调用接口通知请求失败
            }
        }

    }
}
