using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Threading;

namespace Wpf_Audit
{
    /// <summary>
    /// Page_OperationLog.xaml 的交互逻辑
    /// </summary>
    public partial class Page_OperationLog : Page
    {
        private string token;
        private List<Json_Operation> list_OperationLog = new List<Json_Operation>();//datagrid数据源

        public Page_OperationLog(string token)
        {
            this.token = token;
            InitializeComponent();
        }

        public void GetUserOperationLog(object Para)
        {
            string page = Para as string;
            string startTime = null;
            string endTime = null;

            Dp_TimeStart.Dispatcher.Invoke(new Action(() => {
                startTime = Convert.ToString(Dp_TimeStart.SelectedDate);
                endTime = Convert.ToString(Dp_TimeEnd.SelectedDate);
            }));

            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValuePairList("startTime", startTime, ref list);
                Net.GetKeyValuePairList("endTime", endTime, ref list);
                Net.GetKeyValuePairList("page", page, ref list);
                Net.GetKeyValuePairList("rows", "30", ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_OperationLog>();
                try
                {
                    var response = client.PostAsync(Net.Url_OperationLogInfo, mulContent).Result.Content.ReadAsStringAsync().Result;
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_OperationLog.JsonStrToList(response);
                        jsonList.Add(json);
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        proBar.Dispatcher.Invoke(new Action(() => {
                            proBar.Visibility = Visibility.Hidden;
                            Lab_Empty.Content = json.msg;
                            Lab_Empty.Visibility = Visibility.Visible;
                        }));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    proBar.Dispatcher.Invoke(new Action(() => {
                        proBar.Visibility = Visibility.Hidden;
                        Lab_Empty.Content = ex.Message;
                        Lab_Empty.Visibility = Visibility.Visible;
                    }));
                    return;
                }

                foreach (var j in jsonList)
                {
                    if (j.error == 0)
                    {
                        list_OperationLog.Clear();
                        foreach (var item in j.data)
                        {
                            list_OperationLog.Add(item);
                        }
                        proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                        if (list_OperationLog.Count <= 0)
                        {
                            Lab_Empty.Dispatcher.Invoke(new Action(() => {
                                Dg_OperationLog.ItemsSource = null;
                                Lab_Empty.Content = "没有记录";
                                Lab_Empty.Visibility = Visibility.Visible;
                            }));
                        }
                        else
                        {
                            Lab_Empty.Dispatcher.Invoke(new Action(() => {
                                Lab_Empty.Visibility = Visibility.Hidden;
                                MainViewModel model = new MainViewModel(list_OperationLog, Convert.ToInt32(j.page), Convert.ToInt32(j.totalPage), this);
                                DataContext = model;
                                Dg_OperationLog.ItemsSource = model.FakeSource_OperationLog;
                                Dg_OperationLog.Items.Refresh();
                            }));
                        }
                    }
                }
            }
        }

        private void Btn_Query_Click(object sender, RoutedEventArgs e)
        {
            Lab_Empty.Visibility = Visibility.Hidden;
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(GetUserOperationLog);
            thread.IsBackground = true;//设置为后台线程
            thread.Start("1");
        }

        private void Btn_Export_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel.Export(Dg_OperationLog, @"操作日志表(" + DateTime.Now.ToString("yyyyMMdd") + @")");
        }

    }
}
