using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
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

namespace Wpf_Audit
{
    /// <summary>
    /// Page_NotChecked.xaml 的交互逻辑
    /// </summary>
    public partial class Page_NotChecked : Page
    {
        private List<Dg_NotAuditItem> list_NotChecked = new List<Dg_NotAuditItem>();//datagrid数据源
        private List<int> list_SelectRow = new List<int>();//保存所选记录的索引
        private List<Lsv_SingleItem> list_allItems = new List<Lsv_SingleItem>();//listview数据源
        private List<string> selectFid = new List<string>();//保存所选文件的ID
        private string selectSupportId = string.Empty; //所选记录的借款编号

        private string token;
        public string userName;

        private string defaultDirectory = @"D:\DebtPlatform\FileRecv";
        public string DefaultDirectory
        {
            get { return defaultDirectory; }
            set { defaultDirectory = value; }
        }

        private string tempDirectory = @"D:\DebtPlatform\AppData\Roaming";
        public string TempDirectory
        {
            get { return tempDirectory; }
            set { tempDirectory = value; }
        }

        public Page_NotChecked(string token, string userName)
        {
            InitializeComponent();
            this.token = token;
            this.userName = userName;
            Lab_SaveDirectory.Content = defaultDirectory;
        }

        private void Cb_Single_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int index = Convert.ToInt32(cb.Tag.ToString());   //获取该行id 

            if (cb.IsChecked == true)
            {
                list_NotChecked[index].checkBox_IsChecked = true;
                list_SelectRow.Add(index);  //如果选中就保存id          
            }
            else
            {
                list_NotChecked[index].checkBox_IsChecked = false;
                list_SelectRow.Remove(index);   //如果选中取消就删除里面的id            
            }
        }

        public void GetUnCheckedDebtApplication()
        {
            Tbx_Comment.Dispatcher.Invoke(new Action(() => {
                Tbx_Comment.Text = string.Empty;
            }));
            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_AuditApplyInfo>();
                try
                {
                    var response = client.PostAsync(Net.Url_DebtNotChecked, mulContent).Result.Content.ReadAsStringAsync().Result;
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_AuditApplyInfo.JsonStrToList(response);
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
                        list_NotChecked.Clear();
                        foreach (var item in j.data)
                        {
                            list_NotChecked.Add(new Dg_NotAuditItem()
                            {
                                rowIndex = list_NotChecked.Count,
                                checkBox_IsChecked = false,
                                singleRow = item,
                                amount = string.Format("{0:N}", item.amount),
                                status = Property.GetStatus(item.status)
                            });
                        }
                        proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                        if (list_NotChecked.Count <= 0)
                        {
                            Lab_Empty.Dispatcher.Invoke(new Action(() => {
                                Dg_DebtNotChecked.ItemsSource = null;
                                Lab_Empty.Content = "没有记录";
                                Lab_Empty.Visibility = Visibility.Visible;
                            }));
                        }
                        else
                        {
                            Lab_Empty.Dispatcher.Invoke(new Action(() => {
                                Lab_Empty.Visibility = Visibility.Hidden;
                                MainViewModel model = new MainViewModel(list_NotChecked);
                                DataContext = model;
                                Dg_DebtNotChecked.ItemsSource = model.FakeSource_NotAudit;
                                Dg_DebtNotChecked.Items.Refresh();
                            }));
                        }
                    }
                }
            }
        }

        private void Btn_Pass_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int index = Convert.ToInt32(btn.Tag.ToString());
            Thread thread = new Thread(PassSingleApply);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(index);//开始线程
        }

        private void PassSingleApply(object btnPara)
        {
            int index = Convert.ToInt32(btnPara.ToString());

            using (var client = new HttpClient())
            {
                string remark = string.Empty;
                Tbx_Comment.Dispatcher.Invoke(new Action(() => { remark = Tbx_Comment.Text; }));

                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("supportId", list_NotChecked[index].singleRow.debtId, ref list);
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValuePairList("remark", remark, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                try
                {
                    var response = client.PostAsync(Net.Url_DebtApplyPass, mulContent).Result.Content.ReadAsStringAsync().Result;
                    proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                    if (response.StartsWith("{\"error\":0"))
                    {
                        Dg_DebtNotChecked.Dispatcher.Invoke(new Action(() => {
                            Lab_Empty.Visibility = Visibility.Hidden;
                            Dg_DebtNotChecked.ItemsSource = null;
                        }));
                        list_NotChecked.RemoveAt(index);
                        for(int i = 0; i < list_NotChecked.Count; i++)
                        {
                            list_NotChecked[i].rowIndex = i;
                        }
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Empty.Dispatcher.Invoke(new Action(() =>
                        {
                            Lab_Empty.Content = json.msg;
                            Lab_Empty.Visibility = Visibility.Visible;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    proBar.Dispatcher.Invoke(new Action(() => {
                        proBar.Visibility = Visibility.Hidden;
                        Lab_Empty.Content = ex.Message;
                        Lab_Empty.Visibility = Visibility.Visible;
                    }));
                }
                Dg_DebtNotChecked.Dispatcher.Invoke(new Action(() => {
                    if (list_NotChecked.Count <= 0)
                    {
                        Lab_Empty.Content = "没有记录";
                        Lab_Empty.Visibility = Visibility.Visible;
                    }
                    Dg_DebtNotChecked.ItemsSource = list_NotChecked;
                    Dg_DebtNotChecked.Items.Refresh();
                }));
            }
        }


        private void Btn_Reject_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int index = Convert.ToInt32(btn.Tag.ToString());
            Thread thread = new Thread(RejectSingleApply);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(index);//开始线程
        }

        private void RejectSingleApply(object btnPara)
        {
            int index = Convert.ToInt32(btnPara.ToString());

            using (var client = new HttpClient())
            {
                //Dg_DebtNotChecked.ItemsSource = null;

                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("supportId", list_NotChecked[index].singleRow.debtId, ref list);
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValuePairList("remark", list_NotChecked[index].singleRow.remark, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                try
                {
                    var response = client.PostAsync(Net.Url_DebtRejectApply, mulContent).Result.Content.ReadAsStringAsync().Result;
                    if (response.StartsWith("{\"error\":0"))
                    {
                        Dg_DebtNotChecked.Dispatcher.Invoke(new Action(() => {
                            Lab_Empty.Visibility = Visibility.Hidden;
                            Dg_DebtNotChecked.ItemsSource = null;
                        }));
                        list_NotChecked.RemoveAt(index);
                        for (int i = 0; i < list_NotChecked.Count; i++)
                        {
                            list_NotChecked[i].rowIndex = i;
                        }
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Empty.Dispatcher.Invoke(new Action(() =>
                        {
                            Lab_Empty.Content = json.msg;
                            Lab_Empty.Visibility = Visibility.Visible;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    proBar.Dispatcher.Invoke(new Action(() => {
                        proBar.Visibility = Visibility.Hidden;
                        Lab_Empty.Content = ex.Message;
                        Lab_Empty.Visibility = Visibility.Visible;
                    }));
                }
                Dg_DebtNotChecked.Dispatcher.Invoke(new Action(() => {
                    if (list_NotChecked.Count <= 0)
                    {
                        Lab_Empty.Content = "没有记录";
                        Lab_Empty.Visibility = Visibility.Visible;
                    }
                    Dg_DebtNotChecked.ItemsSource = list_NotChecked;
                    Dg_DebtNotChecked.Items.Refresh();
                }));
            }
        }

        private void Check_All_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Lsv_File.ItemsSource = null;

            if (cb.IsChecked == true)
            {
                selectFid.Clear();
                foreach (var item in list_allItems)
                {
                    item.checkBox_IsChecked = true;
                    selectFid.Add(item.singleFile.id);  //如果选中就保存id  
                }
            }
            else
            {
                foreach (var item in list_allItems)
                {
                    item.checkBox_IsChecked = false;
                    selectFid.Remove(item.singleFile.id);  
                }
            }
            Lsv_File.ItemsSource = list_allItems;
            Lsv_File.Items.Refresh();
        }

        private void Check_Single_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int index = Convert.ToInt32(cb.Tag.ToString());
            string fid = list_allItems[index].singleFile.id;   //获取该行id 

            if (cb.IsChecked == true)
            {
                list_allItems[index].checkBox_IsChecked = true;
                selectFid.Add(fid);  //如果选中就保存id          
            }
            else
            {
                list_allItems[index].checkBox_IsChecked = false;
                selectFid.Remove(fid);   //如果选中取消就删除里面的id            
            }

            
            int checknum = 0;
            Is_CheckedAll ca = new Is_CheckedAll();

            foreach (var item in list_allItems)
            {
                if (item.checkBox_IsChecked == true)
                {
                    checknum++;
                }
            }
            if (list_allItems.Count == checknum)
            {
                ca.isCheckedAll = true;
            }
            else
            {
                ca.isCheckedAll = false;
            }
            Check_All.DataContext = ca;
        }

        private void Dg_DebtNotChecked_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBarFile.Visibility = Visibility.Hidden;

            if (Dg_DebtNotChecked.SelectedItem != null)
            {
                selectSupportId = ((Dg_NotAuditItem)Dg_DebtNotChecked.SelectedItem).singleRow.debtId;
                string remark = ConfigHelper.GetAppConfig(selectSupportId);
                Tbx_Comment.Text = (remark == null ? string.Empty : remark);

                UpdateDetail(((Dg_NotAuditItem)Dg_DebtNotChecked.SelectedItem).rowIndex);
                proBarFile.Visibility = Visibility.Visible;
                Thread thread = new Thread(AddRemoteFileToListView);
                thread.IsBackground = true;//设置为后台线程
                thread.Start(((Dg_NotAuditItem)Dg_DebtNotChecked.SelectedItem).rowIndex);//开始线程
            }
        }

        private void AddRemoteFileToListView(object dgPara)
        {
            int index = int.Parse(dgPara.ToString());
            string supportId = list_NotChecked[index].singleRow.debtId;
            Lsv_File.Dispatcher.Invoke(new Action(() => {
                Lsv_File.ItemsSource = null;
                Lab_Empty.Visibility = Visibility.Hidden;
            }));

            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("supportId", supportId, ref list);
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_FilesView>();
                try
                {
                    var response = client.PostAsync(Net.Url_FileView, mulContent).Result.Content.ReadAsStringAsync().Result;
                    proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_FilesView.JsonStrToList(response);
                        jsonList.Add(json);
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Exception.Dispatcher.Invoke(new Action(() => {
                            Lab_Exception.Content = json.msg;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Lab_Exception.Dispatcher.Invoke(new Action(() => {
                        Lab_Exception.Content = ex.Message;
                        Lab_Exception.Visibility = Visibility.Visible;
                        proBarFile.Visibility = Visibility.Hidden;
                    }));
                    return;
                }
                //对json对象进行处理
                foreach (var j in jsonList)
                {
                    if (j.error == 0)
                    {
                        list_allItems.Clear();
                        selectFid.Clear();
                        foreach (var item in j.data)
                        {
                            item.fileSize = Common.ConvertSize(Convert.ToInt32(item.fileSize));
                            int rowNum = list_allItems.Count;
                            list_allItems.Add(new Lsv_SingleItem() { rowIndex = rowNum, checkBox_IsChecked = false, singleFile = item });
                        }
                        Lsv_File.Dispatcher.Invoke(new Action(() => {
                            Lsv_File.ItemsSource = list_allItems;
                            Lsv_File.Items.Refresh();
                        }));
                    }
                }
                if (list_allItems.Count <= 0)
                {
                    Lab_Exception.Dispatcher.Invoke(new Action(() => {
                        Lab_Exception.Content = "没有文件";
                        Lab_Exception.Visibility = Visibility.Visible;
                    }));
                }
            }
        }

        private void UpdateDetail(int index)
        {
            Tbk_Amount.Dispatcher.Invoke(new Action(() => {
                try
                {
                    Tbk_Amount.Text = list_NotChecked[index].amount;
                    Tbk_ApplyTime.Text = list_NotChecked[index].singleRow.applyTime.ToString();
                    Tbk_BankName.Text = list_NotChecked[index].singleRow.bankName;
                    Tbk_BaseInterest.Text = list_NotChecked[index].singleRow.baseInterest.ToString() + "%";
                    Tbk_BondInstituationName.Text = list_NotChecked[index].singleRow.bondInstitutionName;
                    Tbk_DebtEndTime.Text = list_NotChecked[index].singleRow.debtEndTime.ToShortDateString();
                    Tbk_DebtId.Text = list_NotChecked[index].singleRow.debtId;
                    Tbk_DebtStartTime.Text = list_NotChecked[index].singleRow.debtStartTime.ToShortDateString();
                    Tbk_DebtTypeName.Text = list_NotChecked[index].singleRow.debtTypeName;
                    Tbk_CreditUpdateName.Text = list_NotChecked[index].singleRow.creditUpdateName;
                    Tbk_DebtUnitName.Text = list_NotChecked[index].singleRow.debtUnitName;
                    Tbk_IsInGov.Text = Property.GetIsInGov(Convert.ToInt32(list_NotChecked[index].singleRow.isInGov));
                    Tbk_OperatorId.Text = list_NotChecked[index].singleRow.realName.ToString();
                    Tbk_PayTypeName.Text = list_NotChecked[index].singleRow.payTypeName.ToString();
                    Tbk_RateRatio.Text = list_NotChecked[index].singleRow.rateRatio.ToString();
                    Tbk_AdjustTypeName.Text = list_NotChecked[index].singleRow.interestRateAdjustTypeName.ToString();
                    Tbk_Status.Text = Property.GetStatus(list_NotChecked[index].singleRow.status);
                    Tbk_TermClassificationName.Text = list_NotChecked[index].singleRow.termClassificationName.ToString();
                    Tbk_Remark.Text = list_NotChecked[index].singleRow.remark;
                }
                catch { }  
            }));
        }

        private void Btn_ViewFile_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBarFile.Visibility = Visibility.Visible;
            Thread thread = new Thread(ViewSingleItem);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void Lsv_File_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //检查是否双击datagrid行
            if (Lsv_File.SelectedItems.Count == 1)
            {
                Lab_Exception.Visibility = Visibility.Hidden;
                proBarFile.Visibility = Visibility.Visible;
                Thread thread = new Thread(ViewSingleItem);
                thread.IsBackground = true;//设置为后台线程
                thread.Start();//开始线程
            }     
        }

        private void ViewSingleItem()
        {
            if (list_allItems.Count <= 0)
            {
                proBarFile.Dispatcher.Invoke(new Action(() => {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "没有文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            bool success = true;
            string fileId = null;
            string name = null;
            Lsv_File.Dispatcher.Invoke(new Action(() =>
            {
                if (selectFid.Count > 1 || selectFid.Count <= 0 && Lsv_File.SelectedItems.Count != 1)
                {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择一个文件!";
                    Lab_Exception.Visibility = Visibility.Visible;
                    success = false;
                }
                else
                {
                    if (selectFid.Count == 1)
                    {
                        fileId = selectFid[0];
                        foreach (var item in list_allItems)
                        {
                            if (item.singleFile.id == fileId)
                            {
                                name = item.singleFile.fileOldName;
                                break;
                            }
                        }
                    }
                    else
                    {
                        var item = (Lsv_SingleItem)Lsv_File.SelectedItem;
                        fileId = item.singleFile.id;
                        name = item.singleFile.fileOldName;
                    }
                    success = true;
                }
            }));

            if (!success)
            {
                return;
            }

            if (!fileId.Equals(string.Empty))
            {
                string save = tempDirectory;  //默认缓存目录
                if (!save.EndsWith(@"\"))
                {
                    save += @"\";
                }
                string path = save + System.IO.Path.GetFileName(name);

                using (var client = new HttpClient())
                {
                    var jsonList = new List<Json_Download>();

                    var mulContent = new MultipartFormDataContent();
                    var list = new List<KeyValuePair<string, string>>();
                    Net.GetKeyValuePairList("fileId", fileId, ref list);
                    Net.GetKeyValuePairList("token", token, ref list);
                    Net.GetKeyValueMultipartContent(list, ref mulContent);

                    try
                    {
                        var response = client.PostAsync(Net.Url_FileDownload, mulContent).Result.Content.ReadAsStringAsync().Result;
                        proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                        if (response.StartsWith("{\"error\":0"))
                        {
                            var json = Json_Download.JsonStrToList(response);
                            jsonList.Add(json);

                            if (tempDirectory.EndsWith(@"\"))  //创建缓存文件夹
                            {
                                tempDirectory = tempDirectory.Trim('\\');
                            }
                            if (!Directory.Exists(tempDirectory))
                            {
                                Directory.CreateDirectory(tempDirectory);
                            }

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(json.url);
                            HttpWebResponse res = request.GetResponse() as HttpWebResponse;
                            Stream responseStream = res.GetResponseStream();

                            if (name != null)
                            {
                                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                                byte[] bArr = new byte[1024];
                                int size = responseStream.Read(bArr, 0, bArr.Length);
                                while (size > 0)
                                {
                                    stream.Write(bArr, 0, size);
                                    size = responseStream.Read(bArr, 0, bArr.Length);
                                }
                                stream.Close();
                                responseStream.Close();
                                Process.Start(path);  //打开某个文件
                            }
                        }
                        else
                        {
                            var json = Json_Msg.JsonStrToList(response);
                            Lab_Exception.Dispatcher.Invoke(new Action(() => {
                                Lab_Exception.Content = json.msg;
                                Lab_Exception.Visibility = Visibility.Visible;
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        proBarFile.Dispatcher.Invoke(new Action(() => {
                            proBarFile.Visibility = Visibility.Hidden;
                            Lab_Exception.Content = ex.Message;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                    }
                }
            }
        }

        private void Btn_DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBarFile.Visibility = Visibility.Visible;
            Thread thread = new Thread(DownloadRemoteFile);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(Lsv_File);//开始线程
        }

        private void DownloadRemoteFile(object lvPara)
        {
            ListView lv = lvPara as ListView;

            if (list_allItems.Count <= 0)
            {
                proBarFile.Dispatcher.Invoke(new Action(() => {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "没有文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }
            if (selectFid.Count <= 0)
            {
                proBarFile.Dispatcher.Invoke(new Action(() => {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择需要下载的文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            bool success = true;
            Lab_SaveDirectory.Dispatcher.Invoke(new Action(() => {
            if (Lab_SaveDirectory.Content == null || Lab_SaveDirectory.Content.ToString().Equals(string.Empty))
                {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择文件保存目录";
                    Lab_Exception.Visibility = Visibility.Visible;
                    success = false;
                }
            }));
            if (!success)
            {
                return;
            }

            using (var client = new HttpClient())
            {
                var jsonList = new List<Json_Download>();
                bool finished = true;

                foreach (var item in selectFid)
                {
                    var mulContent = new MultipartFormDataContent();
                    var list = new List<KeyValuePair<string, string>>();
                    Net.GetKeyValuePairList("fileId", item, ref list);
                    Net.GetKeyValuePairList("token", token, ref list);
                    Net.GetKeyValueMultipartContent(list, ref mulContent);

                    try
                    {
                        var response = client.PostAsync(Net.Url_FileDownload, mulContent).Result.Content.ReadAsStringAsync().Result;
                        if (response.StartsWith("{\"error\":0"))
                        {
                            var json = Json_Download.JsonStrToList(response);
                            jsonList.Add(json);

                            if (defaultDirectory.EndsWith(@"\"))
                            {
                                defaultDirectory.Trim('\\');
                            }
                            if (!Directory.Exists(defaultDirectory))
                            {
                                Directory.CreateDirectory(defaultDirectory);
                            }

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(json.url);
                            HttpWebResponse res = request.GetResponse() as HttpWebResponse;
                            Stream responseStream = res.GetResponseStream();
                            string name = null;
                            foreach (var temp in list_allItems)
                            {
                                if (item.Equals(temp.singleFile.id))
                                {
                                    name = temp.singleFile.fileOldName;
                                    break;
                                }
                            }
                            if (name != null)
                            {
                                string path = defaultDirectory + "\\" + System.IO.Path.GetFileName(name);
                                for (int i = 1; File.Exists(path); i++)
                                {
                                    path = defaultDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(name) + "(" + i + ")" + System.IO.Path.GetExtension(name);
                                }
                                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                                byte[] bArr = new byte[1024];
                                int size = responseStream.Read(bArr, 0, bArr.Length);
                                while (size > 0)
                                {
                                    stream.Write(bArr, 0, size);
                                    size = responseStream.Read(bArr, 0, bArr.Length);
                                }
                                stream.Close();
                                responseStream.Close();
                            }
                        }
                        else
                        {
                            var json = Json_Msg.JsonStrToList(response);
                            Lab_Exception.Dispatcher.Invoke(new Action(() => {
                                Lab_Exception.Content = json.msg;
                                Lab_Exception.Visibility = Visibility.Visible;
                            }));
                            finished = false;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Lab_Exception.Dispatcher.Invoke(new Action(() => {
                            Lab_Exception.Content = ex.Message;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                        finished = false;
                        break;
                    }
                }

                foreach(var singleItem in list_allItems)
                {
                    singleItem.checkBox_IsChecked = false;
                }
                selectFid.Clear();

                proBarFile.Dispatcher.Invoke(new Action(() => {
                    Is_CheckedAll ca = new Is_CheckedAll();
                    ca.isCheckedAll = false;
                    Check_All.DataContext = ca;
                    Lsv_File.Items.Refresh();
                    proBarFile.Visibility = Visibility.Hidden;
                }));

                if (finished)
                {
                    MessageBox.Show("文件下载完成", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("文件下载出现错误", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            SetDefaultSaveDirectory(ref defaultDirectory);
        }

        private void SetDefaultSaveDirectory(ref string defaultDirectory)
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;//设置为选择文件夹
            cofd.RestoreDirectory = true;

            if (defaultDirectory != string.Empty)
            {
                cofd.DefaultDirectory = defaultDirectory;
            }
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                defaultDirectory = cofd.FileName;
            }
            Lab_SaveDirectory.Content = defaultDirectory;
        }

        private void Btn_PassItems_Click(object sender, RoutedEventArgs e)
        {
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(PassItems);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void PassItems()
        {
            if(list_SelectRow.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                MessageBox.Show("请选择记录", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (var index in list_SelectRow)
            {
                PassSingleApply(index);
            }
            proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
        }

        private void Btn_RejectItems_Click(object sender, RoutedEventArgs e)
        {
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(RejectItems);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void RejectItems()
        {
            if (list_SelectRow.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                MessageBox.Show("请选择记录", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (var index in list_SelectRow)
            {
                RejectSingleApply(index);
            }
            proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
        }


        private void MenuItem_View_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBarFile.Visibility = Visibility.Visible;
            Thread thread = new Thread(ViewSelectedItem);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void ViewSelectedItem()
        {
            if (list_allItems.Count <= 0)
            {
                proBarFile.Dispatcher.Invoke(new Action(() => {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "未选择文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            bool success = true;
            Lsv_SingleItem item = null;
            Lsv_File.Dispatcher.Invoke(new Action(() =>
            {
                if (Lsv_File.SelectedItems.Count != 1)
                {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择一个文件!";
                    Lab_Exception.Visibility = Visibility.Visible;
                    success = false;
                }
                else
                {
                    item = (Lsv_SingleItem)Lsv_File.SelectedItem;
                }
            }));

            if (!success || item == null)
            {
                return;
            }

            string fileId = item.singleFile.id;
            string name = item.singleFile.fileOldName;

            if (!fileId.Equals(string.Empty))
            {
                string save = tempDirectory;  //默认缓存目录
                if (!save.EndsWith(@"\"))
                {
                    save += @"\";
                }
                string path = save + System.IO.Path.GetFileName(name);

                using (var client = new HttpClient())
                {
                    var jsonList = new List<Json_Download>();

                    var mulContent = new MultipartFormDataContent();
                    var list = new List<KeyValuePair<string, string>>();
                    Net.GetKeyValuePairList("fileId", fileId, ref list);
                    Net.GetKeyValuePairList("token", token, ref list);
                    Net.GetKeyValueMultipartContent(list, ref mulContent);

                    try
                    {
                        var response = client.PostAsync(Net.Url_FileDownload, mulContent).Result.Content.ReadAsStringAsync().Result;
                        proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                        if (response.StartsWith("{\"error\":0"))
                        {
                            var json = Json_Download.JsonStrToList(response);
                            jsonList.Add(json);

                            if (tempDirectory.EndsWith(@"\"))  //创建缓存文件夹
                            {
                                tempDirectory = tempDirectory.Trim('\\');
                            }
                            if (!Directory.Exists(tempDirectory))
                            {
                                Directory.CreateDirectory(tempDirectory);
                            }

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(json.url);
                            HttpWebResponse res = request.GetResponse() as HttpWebResponse;
                            Stream responseStream = res.GetResponseStream();

                            if (name != null)
                            {
                                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                                byte[] bArr = new byte[1024];
                                int size = responseStream.Read(bArr, 0, bArr.Length);
                                while (size > 0)
                                {
                                    stream.Write(bArr, 0, size);
                                    size = responseStream.Read(bArr, 0, bArr.Length);
                                }
                                stream.Close();
                                responseStream.Close();
                                Process.Start(path);  //打开某个文件
                            }
                        }
                        else
                        {
                            var json = Json_Msg.JsonStrToList(response);
                            Lab_Exception.Dispatcher.Invoke(new Action(() => {
                                Lab_Exception.Content = json.msg;
                                Lab_Exception.Visibility = Visibility.Visible;
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        proBarFile.Dispatcher.Invoke(new Action(() => {
                            proBarFile.Visibility = Visibility.Hidden;
                            Lab_Exception.Content = ex.Message;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                    }
                }
            }
        }

        private void MenuItem_Download_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBarFile.Visibility = Visibility.Visible;
            Thread thread = new Thread(DownloadSelectedFile);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(Lsv_File);//开始线程
        }

        private void DownloadSelectedFile(object lvPara)
        {
            ListView lv = lvPara as ListView;

            if (list_allItems.Count <= 0)
            {
                proBarFile.Dispatcher.Invoke(new Action(() => {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "未选择文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            bool success = true;
            Lsv_SingleItem item = null;
            Lsv_File.Dispatcher.Invoke(new Action(() =>
            {
                if (lv.SelectedItems.Count != 1)
                {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择一个文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                    success = false;
                }
                else
                {
                    item = (Lsv_SingleItem)Lsv_File.SelectedItem;
                }
                if (Lab_SaveDirectory.Content.ToString().Equals(string.Empty))
                {
                    proBarFile.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择文件保存目录";
                    Lab_Exception.Visibility = Visibility.Visible;
                    success = false;
                }
            }));

            if (!success || item == null)
            {
                return;
            }

            using (var client = new HttpClient())
            {
                var jsonList = new List<Json_Download>();

                string fileId = item.singleFile.id;
                string name = item.singleFile.fileOldName;

                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("fileId", fileId, ref list);
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                try
                {
                    var response = client.PostAsync(Net.Url_FileDownload, mulContent).Result.Content.ReadAsStringAsync().Result;
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_Download.JsonStrToList(response);
                        jsonList.Add(json);

                        if (defaultDirectory.EndsWith(@"\"))
                        {
                            defaultDirectory.Trim('\\');
                        }
                        if (!Directory.Exists(defaultDirectory))
                        {
                            Directory.CreateDirectory(defaultDirectory);
                        }

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(json.url);
                        HttpWebResponse res = request.GetResponse() as HttpWebResponse;
                        Stream responseStream = res.GetResponseStream();

                        if (name != null && name != string.Empty)
                        {
                            string path = defaultDirectory + "\\" + System.IO.Path.GetFileName(name);
                            for (int i = 1; File.Exists(path); i++)
                            {
                                path = defaultDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(name) + "(" + i + ")" + System.IO.Path.GetExtension(name);
                            }
                            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                            byte[] bArr = new byte[1024];
                            int size = responseStream.Read(bArr, 0, bArr.Length);
                            while (size > 0)
                            {
                                stream.Write(bArr, 0, size);
                                size = responseStream.Read(bArr, 0, bArr.Length);
                            }
                            stream.Close();
                            responseStream.Close();
                            proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                            MessageBox.Show("文件下载完成", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Exception.Dispatcher.Invoke(new Action(() => {
                            Lab_Exception.Content = json.msg;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Lab_Exception.Dispatcher.Invoke(new Action(() => {
                        Lab_Exception.Content = ex.Message;
                        Lab_Exception.Visibility = Visibility.Visible;
                    }));
                }
                proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
            }
        }

        private void Dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AccordingDateDisplayDg();
        }

        private void AccordingDateDisplayDg()
        {
            list_allItems.Clear();
            selectFid.Clear();
            Lsv_File.Items.Refresh();

            if (list_NotChecked.Count <= 0)
            {
                Lab_Empty.Content = "没有记录";
                Lab_Empty.Visibility = Visibility.Visible;
                return;
            }

            if ((Dp_DebtStart.SelectedDate != null && !Dp_DebtStart.SelectedDate.ToString().Equals(string.Empty)) || (Dp_DebtEnd.SelectedDate != null && !Dp_DebtEnd.SelectedDate.ToString().Equals(string.Empty)))
            {
                List<Dg_NotAuditItem> list_Limited = null;

                if (list_Limited == null)
                {
                    list_Limited = new List<Dg_NotAuditItem>();//datagrid数据源
                }
                list_Limited.Clear();

                if (Dp_DebtStart.SelectedDate != null && !Dp_DebtStart.SelectedDate.ToString().Equals(string.Empty) && (Dp_DebtEnd.SelectedDate == null || Dp_DebtEnd.SelectedDate.ToString().Equals(string.Empty)))
                {
                    DateTime selectedStartTime = Convert.ToDateTime(Dp_DebtStart.SelectedDate);
                    list_Limited = list_NotChecked.FindAll(c => String.Compare(selectedStartTime.ToString("yyyy-MM-dd"), c.singleRow.debtStartTime.ToString("yyyy-MM-dd")) <= 0);
                }
                else if (Dp_DebtEnd.SelectedDate != null && !Dp_DebtEnd.SelectedDate.ToString().Equals(string.Empty) && (Dp_DebtStart.SelectedDate == null || Dp_DebtStart.SelectedDate.ToString().Equals(string.Empty)))
                {
                    DateTime selectedEndTime = Convert.ToDateTime(Dp_DebtEnd.SelectedDate);
                    list_Limited = list_NotChecked.FindAll(c => String.Compare(selectedEndTime.ToString("yyyy-MM-dd"), c.singleRow.debtEndTime.ToString("yyyy-MM-dd")) >= 0);
                }
                else
                {
                    DateTime selectedStartTime = Convert.ToDateTime(Dp_DebtStart.SelectedDate);
                    DateTime selectedEndTime = Convert.ToDateTime(Dp_DebtEnd.SelectedDate);
                    list_Limited = list_NotChecked.FindAll(c => String.Compare(selectedStartTime.ToString("yyyy-MM-dd"), c.singleRow.debtStartTime.ToString("yyyy-MM-dd")) <= 0);
                    list_Limited = list_Limited.FindAll(c => String.Compare(selectedEndTime.ToString("yyyy-MM-dd"), c.singleRow.debtEndTime.ToString("yyyy-MM-dd")) >= 0);
                }

                if (list_Limited.Count <= 0)
                {
                    MainViewModel model = new MainViewModel(list_Limited);
                    DataContext = model;
                    Dg_DebtNotChecked.ItemsSource = null;
                    Lab_Empty.Content = "没有记录";
                    Lab_Empty.Visibility = Visibility.Visible;
                    return;
                }
                else
                {
                    Lab_Empty.Visibility = Visibility.Hidden;
                    MainViewModel model = new MainViewModel(list_Limited);
                    DataContext = model;
                    Dg_DebtNotChecked.ItemsSource = model.FakeSource_NotAudit;
                    Dg_DebtNotChecked.Items.Refresh();
                }
            }
            else
            {
                Lab_Empty.Visibility = Visibility.Hidden;
                MainViewModel model = new MainViewModel(list_NotChecked);
                DataContext = model;
                Dg_DebtNotChecked.ItemsSource = model.FakeSource_NotAudit;
                Dg_DebtNotChecked.Items.Refresh();
            }
        }

        private void Btn_Export_Click(object sender, RoutedEventArgs e)
        {
            XSSFWorkbook workBook = new XSSFWorkbook();
            try
            {
                if (list_NotChecked == null || list_NotChecked.Count <= 0)
                {
                    throw new Exception("请检查数据是否为空");
                }

                XSSFSheet sheet = (XSSFSheet)workBook.CreateSheet();  //创建一个sheet

                //标题样式
                ICellStyle cellStyle = workBook.CreateCellStyle();//声明样式
                cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//水平居中
                cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//垂直居中
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                IFont font0 = workBook.CreateFont();//声明字体
                //font.Boldweight = (Int16)FontBoldWeight.Bold;//加粗
                font0.FontHeightInPoints = 16;//字体大小
                font0.FontName = "宋体";
                cellStyle.SetFont(font0);//加入单元格

                IRow row0 = sheet.CreateRow(0);//创建行
                row0.HeightInPoints = 37;//行高
                ICell cell0 = row0.CreateCell(0);//创建单元格
                cell0.SetCellValue("待审核表");//赋值
                cell0.CellStyle = cellStyle;//设置样式
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 18));//合并单元格（第几行，到第几行，第几列，到第几列）

                //内容样式1
                ICellStyle headingStyle = workBook.CreateCellStyle();//声明样式
                headingStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//水平居中
                headingStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//垂直居中
                headingStyle.BorderBottom = BorderStyle.Thin;
                headingStyle.BorderLeft = BorderStyle.Thin;
                headingStyle.BorderRight = BorderStyle.Thin;
                headingStyle.BorderTop = BorderStyle.Thin;
                IFont font1 = workBook.CreateFont();//声明字体
                font1.FontHeightInPoints = 9;//字体
                font1.FontName = "宋体";
                headingStyle.SetFont(font1);

                //内容样式2
                ICellStyle rightStyle = workBook.CreateCellStyle();//声明样式
                rightStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;//水平居右
                rightStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//垂直居中
                rightStyle.BorderBottom = BorderStyle.Thin;
                rightStyle.BorderLeft = BorderStyle.Thin;
                rightStyle.BorderRight = BorderStyle.Thin;
                rightStyle.BorderTop = BorderStyle.Thin;
                IFont font2 = workBook.CreateFont();//声明字体
                font2.FontHeightInPoints = 9;//字体
                font2.FontName = "Times New Roman";
                rightStyle.SetFont(font2);

                ICellStyle centerStyle = workBook.CreateCellStyle();//声明样式
                centerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//水平居右
                centerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//垂直居中
                centerStyle.BorderBottom = BorderStyle.Thin;
                centerStyle.BorderLeft = BorderStyle.Thin;
                centerStyle.BorderRight = BorderStyle.Thin;
                centerStyle.BorderTop = BorderStyle.Thin;
                IFont font3 = workBook.CreateFont();//声明字体
                font3.FontHeightInPoints = 9;//字体
                font3.FontName = "Times New Roman";
                centerStyle.SetFont(font3);

                string[] head = { "借款编号", "审核状态", @"金额/元", "申请时间", "起贷时间", "终贷时间", "基准利率", "利率浮动系数","利率调整方式", "还款方式", "借款单位", "申请用户", "借款类型", "债权机构", "关联银行", "期限分类方式", "增信方式", "进入政府债务系统", "备注" };

                IRow frow0 = sheet.CreateRow(1);  // 添加一行（一般第一行是表头）
                int num = 0;
                foreach (string item in head)
                {
                    ICell heading = frow0.CreateCell(num++);
                    heading.SetCellValue(item);
                    heading.CellStyle = headingStyle;
                }

                for (int i = 0; i < list_NotChecked.Count; i++)  //循环添加list中的内容放到表格里
                {
                    IRow frow1 = sheet.CreateRow(i + 2);  //之所以从i+1开始 因为第一行已经有表头了
                    frow1.CreateCell(0).SetCellValue(list_NotChecked[i].singleRow.debtId);
                    frow1.CreateCell(1).SetCellValue(Property.GetStatus(list_NotChecked[i].singleRow.status));
                    frow1.CreateCell(2).SetCellValue(list_NotChecked[i].amount);
                    frow1.CreateCell(3).SetCellValue(list_NotChecked[i].singleRow.applyTime.ToString());
                    frow1.CreateCell(4).SetCellValue(list_NotChecked[i].singleRow.debtStartTime.ToShortDateString());
                    frow1.CreateCell(5).SetCellValue(list_NotChecked[i].singleRow.debtEndTime.ToShortDateString());
                    frow1.CreateCell(6).SetCellValue(list_NotChecked[i].singleRow.baseInterest.ToString() + "%");
                    frow1.CreateCell(7).SetCellValue(list_NotChecked[i].singleRow.rateRatio.ToString());
                    frow1.CreateCell(8).SetCellValue(list_NotChecked[i].singleRow.interestRateAdjustTypeName.ToString());
                    frow1.CreateCell(9).SetCellValue(list_NotChecked[i].singleRow.payTypeName.ToString());
                    frow1.CreateCell(10).SetCellValue(list_NotChecked[i].singleRow.debtUnitName);
                    frow1.CreateCell(11).SetCellValue(list_NotChecked[i].singleRow.realName);
                    frow1.CreateCell(12).SetCellValue(list_NotChecked[i].singleRow.debtTypeName);
                    frow1.CreateCell(13).SetCellValue(list_NotChecked[i].singleRow.bondInstitutionName);
                    frow1.CreateCell(14).SetCellValue(list_NotChecked[i].singleRow.bankName);
                    frow1.CreateCell(15).SetCellValue(list_NotChecked[i].singleRow.termClassificationName.ToString());
                    frow1.CreateCell(16).SetCellValue(list_NotChecked[i].singleRow.creditUpdateName);
                    frow1.CreateCell(17).SetCellValue(Property.GetIsInGov(Convert.ToInt32(list_NotChecked[i].singleRow.isInGov)));
                    frow1.CreateCell(18).SetCellValue(list_NotChecked[i].singleRow.remark);
                    for (int j = 0; j <= 18; j++)
                    {
                        if (j == 0 || j == 3 || j == 4 || j == 5 || j == 6 || j == 7)
                        {
                            frow1.GetCell(j).CellStyle = centerStyle;
                        }
                        else if(j == 2)
                        {
                            frow1.GetCell(j).CellStyle = rightStyle;
                        }
                        else
                        {
                            frow1.GetCell(j).CellStyle = headingStyle;
                        }  
                    }
                }
                AutoColumnWidth(sheet, 18);
                
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
                save.Title = "请选择文件导出路径";
                save.FileName = @"待审核表(" + DateTime.Now.ToString("yyyyMMdd") + @")";

                if (save.ShowDialog() == true)
                {
                    string fileName = save.FileName;
                    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        workBook.Write(fs);  //写入文件
                        workBook.Close();  //关闭
                    }
                    MessageBox.Show("导出成功", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);     
                }
            }
            catch (Exception ex)
            {
                workBook.Close();
                MessageBox.Show("导出失败！" + ex.Message, "消息提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void AutoColumnWidth(ISheet sheet, int cols)
        {
            for (int col = 0; col <= cols; col++)
            {
                sheet.AutoSizeColumn(col);//自适应宽度，但是其实还是比实际文本要宽
                int columnWidth = sheet.GetColumnWidth(col) / 256;//获取当前列宽度
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    ICell cell = row.GetCell(col);
                    int contextLength = Encoding.UTF8.GetBytes(cell.ToString()).Length;//获取当前单元格的内容宽度
                    columnWidth = columnWidth <= contextLength ? contextLength : columnWidth;
                }
                sheet.SetColumnWidth(col, columnWidth * 256);//
            }
        }


        private void Btn_SaveItems_Click(object sender, RoutedEventArgs e)
        {
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(SaveItems);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }


        private void SaveItems()
        {
            if (selectSupportId.Equals(string.Empty) && list_SelectRow.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                MessageBox.Show("请选择记录", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string remark = string.Empty;
            Tbx_Comment.Dispatcher.Invoke(new Action(() => { remark = Tbx_Comment.Text; }));

            bool saved = false;
            if (list_SelectRow.Count > 0)
            {
                foreach (var index in list_SelectRow)
                {
                    if (!remark.Equals(ConfigHelper.GetAppConfig(list_NotChecked[index].singleRow.debtId)))
                    {
                        SaveSingleApply(index);
                        saved = true;
                    }
                }
            }
            else if (!selectSupportId.Equals(string.Empty))
            {
                if (!remark.Equals(ConfigHelper.GetAppConfig(selectSupportId)))
                {
                    SaveSingleApply(selectSupportId);
                    saved = true;
                }
            }
            proBar.Dispatcher.Invoke(new Action(() => {
                proBar.Visibility = Visibility.Hidden;
            }));

            if (saved)
            {
                MessageBox.Show("保存成功", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveSingleApply(object btnPara)
        {
            try
            {
                if (btnPara is int)
                {
                    int index = Convert.ToInt32(btnPara.ToString());
                    Tbx_Comment.Dispatcher.Invoke(new Action(() => {
                        ConfigHelper.UpdateAppConfig(list_NotChecked[index].singleRow.debtId, Tbx_Comment.Text);
                        Lab_Empty.Visibility = Visibility.Hidden;
                    }));
                }
                if (btnPara is string)
                {
                    Tbx_Comment.Dispatcher.Invoke(new Action(() => {
                        ConfigHelper.UpdateAppConfig(btnPara.ToString(), Tbx_Comment.Text);
                        Lab_Empty.Visibility = Visibility.Hidden;
                    }));
                }
            }
            catch (Exception ex)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    Lab_Empty.Content = ex.Message;
                    Lab_Empty.Visibility = Visibility.Visible;
                }));
            }
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int index = Convert.ToInt32(btn.Tag.ToString());
            Thread thread = new Thread(SaveSingleApply);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(index);//开始线程
        }

        
    }
}
