using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NPOI.SS.UserModel;
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
    /// Page_Checked.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Checked : Page
    {
        private List<Dg_AuditItem> list_Checked = new List<Dg_AuditItem>();
        private List<Lsv_SingleItem> list_allItems = new List<Lsv_SingleItem>();//listview数据源
        private List<string> selectFid = new List<string>();//保存所选文件的ID

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

        public Page_Checked(string token, string userName)
        {
            InitializeComponent();
            this.token = token;
            this.userName = userName;
            Lab_SaveDirectory.Content = defaultDirectory;
        }

        public void GetCheckedDebtApplication()
        {
            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_AuditPassInfo>();
                try
                {
                    var response = client.PostAsync(Net.Url_DebtChecked, mulContent).Result.Content.ReadAsStringAsync().Result;
                    
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_AuditPassInfo.JsonStrToList(response);
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
                        list_Checked.Clear();
                        foreach (var item in j.data)
                        {
                            list_Checked.Add(new Dg_AuditItem()
                            {
                                rowIndex = list_Checked.Count,
                                singleRow = item,
                                amount = string.Format("{0:N}", item.amount),
                                auditUser = userName
                            });
                        }
                        
                        proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                        if (list_Checked.Count <= 0)
                        {
                            Lab_Empty.Dispatcher.Invoke(new Action(() => {
                                Dg_DebtChecked.ItemsSource = null;
                                Lab_Empty.Content = "没有记录";
                                Lab_Empty.Visibility = Visibility.Visible;
                            }));
                        }
                        else
                        {
                            Lab_Empty.Dispatcher.Invoke(new Action(() => {
                                Lab_Empty.Visibility = Visibility.Hidden;
                                MainViewModel model= new MainViewModel(list_Checked);
                                DataContext = model;
                                Dg_DebtChecked.ItemsSource = model.FakeSource_Audit;
                                Dg_DebtChecked.Items.Refresh();
                            }));
                        }
                    }
                }
            }
        }


        private void Lsv_File_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Lsv_File.SelectedItems.Count == 1)
            {
                Lab_Exception.Visibility = Visibility.Hidden;
                proBarFile.Visibility = Visibility.Visible;
                Thread thread = new Thread(ViewSingleItem);
                thread.IsBackground = true;//设置为后台线程
                thread.Start();//开始线程
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

            Is_CheckedAll ca = new Is_CheckedAll();
            int checknum = 0;

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

        private void Btn_View_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBarFile.Visibility = Visibility.Visible;
            Thread thread = new Thread(ViewSingleItem);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        
        private void ViewSingleItem()
        {
            if(list_allItems.Count <= 0)
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
                    if(selectFid.Count == 1)
                    {
                        fileId = selectFid[0];
                        foreach (var item in list_allItems)
                        {
                            if(item.singleFile.id == fileId)
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

            if(!success)
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

        private void Btn_Download_Click(object sender, RoutedEventArgs e)
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
                proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                Lab_Exception.Dispatcher.Invoke(new Action(() =>
                {
                    Lab_Exception.Content = "没有文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }
            if (selectFid.Count <= 0)
            {
                proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                Lab_Exception.Dispatcher.Invoke(new Action(() =>
                {
                    Lab_Exception.Content = "请选择需要下载的文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            bool success = true;
            Lab_SaveDirectory.Dispatcher.Invoke(new Action(() => {
                if (Lab_SaveDirectory.Content.ToString().Equals(string.Empty))
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

                foreach (var singleItem in list_allItems)
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

        private void Dg_DebtChecked_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBarFile.Visibility = Visibility.Hidden;

            if (Dg_DebtChecked.SelectedItem != null)
            {
                proBarFile.Visibility = Visibility.Visible;
                Thread thread = new Thread(AddRemoteFileToListView);
                thread.IsBackground = true;//设置为后台线程
                thread.Start(((Dg_AuditItem)Dg_DebtChecked.SelectedItem).rowIndex);//开始线程
            }
        }

        private void AddRemoteFileToListView(object dgPara)
        {
            int index = int.Parse(dgPara.ToString());
            string supportId = list_Checked[index].singleRow.supportId;
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
                    }

                }
                catch (Exception ex)
                {
                    Lab_Exception.Dispatcher.Invoke(new Action(() => {
                        Lab_Exception.Content = ex.Message;
                        Lab_Exception.Visibility = Visibility.Visible;
                    }));
                    proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                    return;
                }

                if (supportId.StartsWith("A"))
                {
                    Json_SingleAuditApplyInfo singleMsg = GetSingleApplyInfo(supportId);
                    if(singleMsg != null)
                    {
                        UpdateDetail(singleMsg, index);
                    }
                }
                else
                {
                    Json_SingleAuditChangeInfo changeMsg= GetSingleChangeInfo(supportId);
                    Json_SingleAuditApplyInfo singleMsg = GetSingleApplyInfo(changeMsg.debtId);
                    if (changeMsg != null && singleMsg != null)
                    {
                        singleMsg.debtEndTime = changeMsg.debtEndTime;
                        singleMsg.remark = changeMsg.remark;
                        singleMsg.applyTime = changeMsg.applyTime;
                        singleMsg.payTypeName = changeMsg.payTypeName;
                        singleMsg.status = changeMsg.status;
                        singleMsg.baseInterest = changeMsg.changedInterestRate;
                        singleMsg.rateRatio = 1;
                        singleMsg.interestRateAdjustTypeName = changeMsg.adjustType;
                        singleMsg.operatorId = changeMsg.operatorId;
                        UpdateDetail(singleMsg, index);
                    } 
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
                            proBarFile.Visibility = Visibility.Hidden;
                            Lsv_File.ItemsSource = list_allItems;
                            Lsv_File.Items.Refresh();
                        }));
                    }
                }
                if(list_allItems.Count <= 0)
                {
                    Lab_Exception.Dispatcher.Invoke(new Action(() => {
                        Lab_Exception.Content = "没有文件";
                        Lab_Exception.Visibility = Visibility.Visible;
                    }));
                }
            }
        }

        private Json_SingleAuditChangeInfo GetSingleChangeInfo(string changeId)
        {
            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("changeId", changeId, ref list);
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_SingleAuditChangeInfo>();
                
                try
                {
                    var response = client.PostAsync(Net.Url_SingleChangeInfo, mulContent).Result.Content.ReadAsStringAsync().Result;
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_SingleAuditChangeInfo.JsonStrToList(response);
                        jsonList.Add(json);
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Empty.Dispatcher.Invoke(new Action(() =>
                        {
                            Lab_Empty.Content = json.msg;
                            Lab_Empty.Visibility = Visibility.Visible;
                        }));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Lab_Empty.Dispatcher.Invoke(new Action(() =>
                    {
                        proBarFile.Visibility = Visibility.Hidden;
                        Lab_Empty.Content = ex.Message;
                        Lab_Empty.Visibility = Visibility.Visible;
                    }));
                    return null;
                }

                foreach (var j in jsonList)
                {
                    if (j.error == 0 && jsonList.Count == 1)
                    {
                        return j;
                    }
                }
                return null;
            }
        }

        private Json_SingleAuditApplyInfo GetSingleApplyInfo(string loanId)
        {
            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("loanId", loanId, ref list);
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_SingleAuditApplyInfo>();
                try
                {
                    var response = client.PostAsync(Net.Url_SingleApplyInfo, mulContent).Result.Content.ReadAsStringAsync().Result;
                    
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_SingleAuditApplyInfo.JsonStrToList(response);
                        jsonList.Add(json);
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Empty.Dispatcher.Invoke(new Action(() =>
                        {
                            Lab_Empty.Content = json.msg;
                            Lab_Empty.Visibility = Visibility.Visible;
                        }));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Lab_Empty.Dispatcher.Invoke(new Action(() =>
                    {
                        proBarFile.Visibility = Visibility.Hidden;
                        Lab_Empty.Content = ex.Message;
                        Lab_Empty.Visibility = Visibility.Visible;
                    }));
                    return null;
                }

                foreach (var j in jsonList)
                {
                    if (j.error == 0 && jsonList.Count == 1)
                    {
                        return j;
                    }
                }
                return null;
            }
        }

        private void UpdateDetail(Json_SingleAuditApplyInfo singleApply,int index)
        {
            Tbx_AuditDate.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    Tbx_AuditDate.Text = list_Checked[index].singleRow.auditDate.ToString();
                    Tbx_AuditId.Text = list_Checked[index].singleRow.auditId.ToString();
                    Tbx_AuditRank.Text = Property.GetRank(list_Checked[index].singleRow.auditRank);
                    Tbx_DebtId.Text = list_Checked[index].singleRow.supportId;
                    Tbx_IsPassed.Text = Property.GetIsPassed(list_Checked[index].singleRow.isPassed);
                    Tbx_Remark.Text = list_Checked[index].singleRow.remark;
                    Tbx_UserId.Text = userName;

                    Tbk_Amount.Text = list_Checked[index].amount;
                    Tbk_ApplyTime.Text = singleApply.applyTime.ToString();
                    Tbk_BankName.Text = singleApply.bankName;
                    Tbk_BaseInterest.Text = singleApply.baseInterest.ToString() + "%";
                    Tbk_BondInstituationName.Text = singleApply.bondInstitutionName;
                    Tbk_CreditUpdateName.Text = singleApply.creditUpdateName;
                    Tbk_DebtEndTime.Text = singleApply.debtEndTime.ToShortDateString();
                    Tbk_DebtStartTime.Text = singleApply.debtStartTime.ToShortDateString();
                    Tbk_AdjustType.Text = singleApply.interestRateAdjustTypeName;
                    Tbk_DebtTypeName.Text = singleApply.debtTypeName;
                    Tbk_DebtUnitName.Text = singleApply.debtUnitName;
                    Tbk_IsInGov.Text = Property.GetIsInGov(Convert.ToInt32(singleApply.isInGov));
                    Tbk_OperatorId.Text = singleApply.realName.ToString();
                    Tbk_PayTypeName.Text = singleApply.payTypeName.ToString();
                    Tbk_RateRatio.Text = singleApply.rateRatio.ToString();
                    Tbk_Status.Text = Property.GetStatus(singleApply.status);
                    Tbk_TermClassificationName.Text = singleApply.termClassificationName.ToString();
                    Tbk_Remark.Text = singleApply.remark;
                }
                catch { }   
            }));
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
                /*
                if (File.Exists(path))
                {
                    proBarFile.Dispatcher.Invoke(new Action(() => { proBarFile.Visibility = Visibility.Hidden; }));
                    Process.Start(path);
                    return;
                }
                */
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
                            for(int i=1; File.Exists(path);i++)
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

            if (list_Checked.Count <= 0)
            {
                Lab_Empty.Content = "没有记录";
                Lab_Empty.Visibility = Visibility.Visible;
                return;
            }

            if ((Dp_DebtStart.SelectedDate != null && !Dp_DebtStart.SelectedDate.ToString().Equals(string.Empty)) || (Dp_DebtEnd.SelectedDate != null && !Dp_DebtEnd.SelectedDate.ToString().Equals(string.Empty)))
            {
                List<Dg_AuditItem> list_Limited = null;

                if (list_Limited == null)
                {
                    list_Limited = new List<Dg_AuditItem>();//datagrid数据源
                }
                list_Limited.Clear();

                if (Dp_DebtStart.SelectedDate != null && !Dp_DebtStart.SelectedDate.ToString().Equals(string.Empty) && (Dp_DebtEnd.SelectedDate == null || Dp_DebtEnd.SelectedDate.ToString().Equals(string.Empty)))
                {
                    DateTime selectedStartTime = Convert.ToDateTime(Dp_DebtStart.SelectedDate);
                    list_Limited = list_Checked.FindAll(c => String.Compare(selectedStartTime.ToString("yyyy-MM-dd"), c.singleRow.debtStartTime.ToString("yyyy-MM-dd")) <= 0);
                }
                else if (Dp_DebtEnd.SelectedDate != null && !Dp_DebtEnd.SelectedDate.ToString().Equals(string.Empty) && (Dp_DebtStart.SelectedDate == null || Dp_DebtStart.SelectedDate.ToString().Equals(string.Empty)))
                {
                    DateTime selectedEndTime = Convert.ToDateTime(Dp_DebtEnd.SelectedDate);
                    list_Limited = list_Checked.FindAll(c => String.Compare(selectedEndTime.ToString("yyyy-MM-dd"), c.singleRow.debtEndTime.ToString("yyyy-MM-dd")) >= 0);
                }
                else
                {
                    DateTime selectedStartTime = Convert.ToDateTime(Dp_DebtStart.SelectedDate);
                    DateTime selectedEndTime = Convert.ToDateTime(Dp_DebtEnd.SelectedDate);
                    list_Limited = list_Checked.FindAll(c => String.Compare(selectedStartTime.ToString("yyyy-MM-dd"), c.singleRow.debtStartTime.ToString("yyyy-MM-dd")) <= 0);
                    list_Limited = list_Limited.FindAll(c => String.Compare(selectedEndTime.ToString("yyyy-MM-dd"), c.singleRow.debtEndTime.ToString("yyyy-MM-dd")) >= 0);
                }

                if (list_Limited.Count <= 0)
                {
                    MainViewModel model = new MainViewModel(list_Limited);
                    DataContext = model;
                    Dg_DebtChecked.ItemsSource = null;
                    Lab_Empty.Content = "没有记录";
                    Lab_Empty.Visibility = Visibility.Visible;
                    return;
                }
                else
                {
                    Lab_Empty.Visibility = Visibility.Hidden;
                    MainViewModel model = new MainViewModel(list_Limited);
                    DataContext = model;
                    Dg_DebtChecked.ItemsSource = model.FakeSource_Audit;
                    Dg_DebtChecked.Items.Refresh();
                }
            }
            else
            {
                Lab_Empty.Visibility = Visibility.Hidden;
                MainViewModel model= new MainViewModel(list_Checked);
                DataContext = model;
                Dg_DebtChecked.ItemsSource = model.FakeSource_Audit;
                Dg_DebtChecked.Items.Refresh();
            }
        }

        private void Btn_Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (list_Checked == null || list_Checked.Count <= 0)
                {
                    throw new Exception("请检查数据是否为空");
                }

                XSSFWorkbook workBook = new XSSFWorkbook();  //实例化XSSF
                XSSFSheet sheet = (XSSFSheet)workBook.CreateSheet();  //创建一个sheet

                IRow frow0 = sheet.CreateRow(0);  // 添加一行（一般第一行是表头）
                frow0.CreateCell(0).SetCellValue("审核编号");
                frow0.CreateCell(1).SetCellValue("借款编号");
                frow0.CreateCell(2).SetCellValue("金额");
                frow0.CreateCell(3).SetCellValue("是否通过");
                frow0.CreateCell(4).SetCellValue("起贷时间");
                frow0.CreateCell(5).SetCellValue("终贷时间");
                frow0.CreateCell(6).SetCellValue("审核人员");
                frow0.CreateCell(7).SetCellValue("审核等级");
                frow0.CreateCell(8).SetCellValue("审核时间");
                frow0.CreateCell(9).SetCellValue("审核意见");

                for (int i = 0; i < list_Checked.Count; i++)  //循环添加list中的内容放到表格里
                {
                    IRow frow1 = sheet.CreateRow(i + 1);  //之所以从i+1开始 因为第一行已经有表头了
                    frow1.CreateCell(0).SetCellValue(list_Checked[i].singleRow.auditId.ToString());
                    frow1.CreateCell(1).SetCellValue(list_Checked[i].singleRow.supportId);
                    frow1.CreateCell(2).SetCellValue(list_Checked[i].amount);
                    frow1.CreateCell(3).SetCellValue(Property.GetIsPassed(list_Checked[i].singleRow.isPassed));
                    frow1.CreateCell(4).SetCellValue(list_Checked[i].singleRow.debtStartTime.ToString());
                    frow1.CreateCell(5).SetCellValue(list_Checked[i].singleRow.debtEndTime.ToString());
                    frow1.CreateCell(6).SetCellValue(userName);
                    frow1.CreateCell(7).SetCellValue(Property.GetRank(list_Checked[i].singleRow.auditRank));
                    frow1.CreateCell(8).SetCellValue(list_Checked[i].singleRow.auditDate.ToString());
                    frow1.CreateCell(9).SetCellValue(list_Checked[i].singleRow.remark);
                }

                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
                save.Title = "请选择文件导出路径";
                save.FileName = @"已审核表(" + DateTime.Now.ToString("yyyyMMdd") + @")";

                if (save.ShowDialog() == true)
                {
                    string fileName = save.FileName;
                    try
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        {
                            workBook.Write(fs);  //写入文件
                            workBook.Close();  //关闭
                        }
                        MessageBox.Show("导出成功", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch
                    {
                        workBook.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败！" + ex.Message, "消息提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
