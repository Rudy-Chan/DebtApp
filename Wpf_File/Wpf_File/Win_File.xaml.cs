using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace Wpf_File
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Win_File : Window
    {
        private static Win_File instance = null;
        private List<Lsv_SingleItem> list_allItems = new List<Lsv_SingleItem>();//listview数据源
        private List<string> selectFid = new List<string>();//保存所选文件的ID

        private string userId;
        private string supportId;
        private string token;

        private string defaultDirectory = @"D:\DebtPlatform\FileRecv";
        public string DefaultDirectory
        {
            get { return defaultDirectory; }
            set { defaultDirectory = value; }
        }

        private string tempDirectory = @"D:\DebtPlatform\AppData";
        public string TempDirectory
        {
            get { return tempDirectory; }
            set { tempDirectory = value; }
        }

        public static Win_File CreateWindow(string serverIp, string userId, string supportId, string token)
        {
            if(instance == null)
            {
                instance = new Win_File(serverIp, userId, supportId, token);
            }
            return instance;
        }

        private Win_File(string serverIp, string userId, string supportId, string token)
        {

            Net.IP = serverIp;
            this.userId = userId;
            this.supportId = supportId;
            this.token = token;
            InitializeComponent();
            Lab_Savepath.Content = defaultDirectory;
        }

        private void Check_All_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Lsv_File.ItemsSource = null;

            if(list_allItems.Count <= 0)
            {
                Lab_Exception.Content = "没有文件";
                Lab_Exception.Visibility = Visibility.Visible;
                return;
            }

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
            string fid = cb.Tag.ToString();   //获取该行id 

            if (cb.IsChecked == true)
            {
                selectFid.Add(fid);  //如果选中就保存id          
            }
            else
            {
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

        private void Btn_Select_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            AddLocalFileToListView();
            Lab_Num.Content = Lsv_File.Items.Count + "项";
        }

        // 将文件添加到ListView中显示
        public void AddLocalFileToListView()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == true)
                {
                    if (ofd.FileNames != null && ofd.FileNames.Length > 0)
                    {
                        Lsv_File.ItemsSource = null;
                        Btn_Upload.IsEnabled = true;
                        Btn_Download.IsEnabled = false;
                        Btn_Delete.IsEnabled = false;

                        list_allItems.Clear();
                        foreach (string fileName in ofd.FileNames)
                        {
                            if (System.IO.Path.GetExtension(fileName).Equals(".php") || System.IO.Path.GetExtension(fileName).Equals(".htm")
                                || System.IO.Path.GetExtension(fileName).Equals(".html") || System.IO.Path.GetExtension(fileName).Equals(".dll"))
                            {
                                Lab_Exception.Content = "存在不可上传的文件，已自动过滤";
                                Lab_Exception.Visibility = Visibility.Visible;
                                continue;
                            }

                            long sizeNum = new FileInfo(fileName).Length;
                            string size = Common.ConvertSize((int)sizeNum);
                            int index = list_allItems.Count;
                            Lsv_SingleItem item = new Lsv_SingleItem()
                            {
                                rowIndex = index,
                                checkBox_IsChecked = false,
                                checkBox_IsEnabled = false,
                                singleFile = new Json_File() { fileOldName = fileName, fileSize = size, id = string.Empty, uploadTime = DateTime.Now }
                            };
                            list_allItems.Add(item);
                        }
                        Lsv_File.ItemsSource = list_allItems;
                        Lsv_File.Items.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Lab_Exception.Content = ex.Message;
                Lab_Exception.Visibility = Visibility.Visible;
            }
        }

        private void Btn_Upload_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(uploadFilesToServer);
            thread.IsBackground = true;//设置为后台线程
            thread.Start("uploadFile");//开始线程
        }

        public void uploadFilesToServer(object para)
        {
            string str = para as string;
            using (var client = new HttpClient())
            {
                var files = Net.GetHashSet(Lsv_File);
                if (files.Count <= 0)
                {
                    proBar.Dispatcher.Invoke(new Action(() => {
                        proBar.Visibility = Visibility.Hidden;
                        Lab_Exception.Content = "请选择文件";
                        Lab_Exception.Visibility = Visibility.Visible;
                    }));
                    return;
                }

                var jsonList = new List<Json_Upload>();
                bool success = true;

                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        success = false;
                        proBar.Dispatcher.Invoke(new Action(() => {
                            proBar.Visibility = Visibility.Hidden;
                            Lab_Exception.Content = "当前上传的文件已删除，请重新确认";
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                        break;
                    }

                    var mulContent = new MultipartFormDataContent();
                    var list = new List<KeyValuePair<string, string>>();

                    Net.GetKeyValuePairList("userId", userId, ref list);
                    Net.GetKeyValuePairList("supportId", supportId, ref list);
                    Net.GetKeyValuePairList("fileName", System.IO.Path.GetFileName(file), ref list);
                    Net.GetKeyValuePairList("token", token, ref list);
                    Net.GetKeyValueMultipartContent(list, ref mulContent);
                    Net.GetFileMultipartContent(file, str, ref mulContent);

                    try
                    {
                        var response = client.PostAsync(Net.Url_FileUpload, mulContent).Result.Content.ReadAsStringAsync().Result;
                        if (response.StartsWith("{\"error\":0"))
                        {
                            var json = Json_Upload.JsonStrToList(response);
                            jsonList.Add(json);
                            list_allItems.RemoveAll(s => (s.singleFile.fileOldName == file));
                            Lsv_File.Dispatcher.Invoke(new Action(() => {
                                Lsv_File.Items.Refresh();
                            })); 
                        }
                        else
                        {
                            var json = Json_Msg.JsonStrToList(response);
                            success = false;
                            proBar.Dispatcher.Invoke(new Action(() => {
                                proBar.Visibility = Visibility.Hidden;
                                Lab_Exception.Content = "文件上传出现错误，请重新上传  " + "错误代码:" + json.error + "  " + json.msg;
                                Lab_Exception.Visibility = Visibility.Visible;
                            }));
                            break;
                        }

                    }
                    catch (Exception ex)
                    {
                        success = false;
                        proBar.Dispatcher.Invoke(new Action(() => {
                            proBar.Visibility = Visibility.Hidden;
                            Lab_Exception.Content = ex.Message;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                        break;
                    }
                }

                Lab_Num.Dispatcher.Invoke(new Action(() => { Lab_Num.Content = Lsv_File.Items.Count + "项"; }));
                if (success)
                {
                    proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                    MessageBox.Show("文件上传成功", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Btn_View_Click(object sender, RoutedEventArgs e)
        {
            proBar.Visibility = Visibility.Visible;
            Lab_Exception.Visibility = Visibility.Hidden;
            Thread thread = new Thread(AddRemoteFileToListView);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        //获取文件列表
        public void AddRemoteFileToListView()
        {
            Lsv_File.Dispatcher.Invoke(new Action(() => {
                Lsv_File.ItemsSource = null;
                Btn_Upload.IsEnabled = false;
                Btn_Download.IsEnabled = true;
                Btn_Delete.IsEnabled = true;
            }));

            using (var client = new HttpClient())
            {
                list_allItems.Clear();
                selectFid.Clear();
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("supportId", supportId, ref list);
                Net.GetKeyValuePairList("token", token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_FilesView>();
                try
                {
                    var response = client.PostAsync(Net.Url_FileView, mulContent).Result.Content.ReadAsStringAsync().Result;
                    proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_FilesView.JsonStrToList(response);
                        jsonList.Add(json);
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Exception.Dispatcher.Invoke(new Action(() => {
                            Lab_Exception.Content = "错误代码:" + json.error + "  " + json.msg;
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
                        proBar.Visibility = Visibility.Hidden;
                    }));
                    return;
                }
                //对json对象进行处理
                foreach (var j in jsonList)
                {
                    if (j.error == 0)
                    {
                        foreach (var item in j.data)
                        {
                            item.fileSize = Common.ConvertSize(Convert.ToInt32(item.fileSize));
                            int rowNum = list_allItems.Count;
                            list_allItems.Add(new Lsv_SingleItem() { rowIndex = rowNum, checkBox_IsEnabled = true, checkBox_IsChecked = false, singleFile = item });
                        }
                        Lsv_File.Dispatcher.Invoke(new Action(() => {
                            Lsv_File.ItemsSource = list_allItems;
                            Lsv_File.Items.Refresh();
                        }));
                    }
                }
                Lab_Num.Dispatcher.Invoke(new Action(() => { Lab_Num.Content = Lsv_File.Items.Count + "项"; }));
                if (list_allItems.Count <= 0)
                {
                    Lab_Exception.Dispatcher.Invoke(new Action(() => {
                        Lab_Exception.Content = "没有文件";
                        Lab_Exception.Visibility = Visibility.Visible;
                    }));
                }
            }
        }

        private void Btn_Download_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(DownloadRemoteFile);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(Lsv_File);//开始线程
        }

        private void DownloadRemoteFile(object lvPara)
        {
            ListView lv = lvPara as ListView;

            if (list_allItems.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "没有文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }
            if (selectFid.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择需要下载的文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            bool success = true;
            Lab_Savepath.Dispatcher.Invoke(new Action(() => {
                if (Lab_Savepath.Content == null || Lab_Savepath.Content.ToString().Equals(string.Empty))
                {
                    proBar.Visibility = Visibility.Hidden;
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
                                Lab_Exception.Content = "错误代码:" + json.error + "  " + json.msg;
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

                proBar.Dispatcher.Invoke(new Action(() => {
                    Is_CheckedAll ca = new Is_CheckedAll();
                    ca.isCheckedAll = false;
                    Check_All.DataContext = ca;
                    Lab_Num.Content = Lsv_File.Items.Count + "项";
                    Lsv_File.Items.Refresh();
                    proBar.Visibility = Visibility.Hidden;
                }));

                if (finished)
                {
                    MessageBox.Show("文件下载完成", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("文件下载出现错误", "消息提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            Thread thread = new Thread(DeleteFileFromServer);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(Lsv_File);//开始线程
        }

        private void DeleteFileFromServer(object lvPara)
        {
            ListView lv = lvPara as ListView;

            if (list_allItems.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "没有文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }
            if (selectFid.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择需要删除的文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            if (MessageBoxResult.Yes == MessageBox.Show("是否删除", "消息提示", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                using (var client = new HttpClient())
                {
                    var jsonList = new List<Json_Delete>();

                    string fileId = string.Empty;
                    foreach (var item in selectFid)
                    {
                        fileId += item.Trim() + '|';
                    }
                    fileId.Trim('|');

                    var mulContent = new MultipartFormDataContent();
                    var list = new List<KeyValuePair<string, string>>();
                    Net.GetKeyValuePairList("userId", userId, ref list);
                    Net.GetKeyValuePairList("fileId", fileId, ref list);
                    Net.GetKeyValuePairList("supportId", supportId, ref list);
                    Net.GetKeyValuePairList("token", token, ref list);
                    Net.GetKeyValueMultipartContent(list, ref mulContent);

                    try
                    {
                        proBar.Dispatcher.Invoke(new Action(() => {
                            proBar.Visibility = Visibility.Visible;
                        }));
                        var response = client.PostAsync(Net.Url_FileDelete, mulContent).Result.Content.ReadAsStringAsync().Result;
                        if (response.StartsWith("{\"error\":0"))
                        {
                            string[] str = fileId.Split('|');
                            foreach(var t in str)
                            {
                                list_allItems.RemoveAll(s => (s.singleFile.id == t));
                                selectFid.RemoveAll(s => (s == t));
                            }
                            var json = Json_Delete.JsonStrToList(response);
                            jsonList.Add(json);
                            for (int i = 0; i < list_allItems.Count; i++)
                            {
                                list_allItems[i].rowIndex = i;
                            }
                        }
                        else
                        {
                            var json = Json_Msg.JsonStrToList(response);
                            proBar.Dispatcher.Invoke(new Action(() => {
                                proBar.Visibility = Visibility.Hidden;
                                Lab_Exception.Content = "错误代码:" + json.error + "  " + json.msg;
                                Lab_Exception.Visibility = Visibility.Visible;
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        proBar.Dispatcher.Invoke(new Action(() => {
                            proBar.Visibility = Visibility.Hidden;
                            Lab_Exception.Content = ex.Message;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                    }

                    Lsv_File.Dispatcher.Invoke(new Action(() => {
                        Lsv_File.Items.Refresh();
                    }));
                }
            }
            proBar.Dispatcher.Invoke(new Action(() => {
                Lab_Num.Content = Lsv_File.Items.Count + "项";
                proBar.Visibility = Visibility.Hidden;
            }));
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
            Lab_Savepath.Content = defaultDirectory;
        }

        private void Lsv_File_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(ViewFileFromUrl);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void MenuItem_View_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(ViewFileFromUrl);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void ViewFileFromUrl()
        {
            if (list_allItems.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "没有文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                }));
                return;
            }

            Lsv_SingleItem item = null;

            if (list_allItems[0].singleFile.id.Equals(string.Empty))
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
                    item = (Lsv_SingleItem)Lsv_File.SelectedItem;
                }));
                if(item != null && File.Exists(item.singleFile.fileOldName))
                {
                    Process.Start(item.singleFile.fileOldName);  //打开某个文件
                }
                return; 
            }

            bool success = true;
            Lsv_File.Dispatcher.Invoke(new Action(() =>
            {
                if (Lsv_File.SelectedItems.Count != 1)
                {
                    proBar.Visibility = Visibility.Hidden;
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
                        proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
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
                                Lab_Exception.Content = "错误代码:" + json.error + "  " + json.msg;
                                Lab_Exception.Visibility = Visibility.Visible;
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        proBar.Dispatcher.Invoke(new Action(() => {
                            proBar.Visibility = Visibility.Hidden;
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
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(DownloadSingleItem);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(Lsv_File);//开始线程
        }

        private void DownloadSingleItem(object lvPara)
        {
            ListView lv = lvPara as ListView;

            if (list_allItems.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
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
                    proBar.Visibility = Visibility.Hidden;
                    Lab_Exception.Content = "请选择一个文件";
                    Lab_Exception.Visibility = Visibility.Visible;
                    success = false;
                }
                else
                {
                    item = (Lsv_SingleItem)Lsv_File.SelectedItem;
                }
                if (Lab_Savepath.Content.ToString().Equals(string.Empty))
                {
                    proBar.Visibility = Visibility.Hidden;
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
                            Stream stream = new FileStream(defaultDirectory + "\\" + System.IO.Path.GetFileName(name), FileMode.Create, FileAccess.Write);
                            byte[] bArr = new byte[1024];
                            int size = responseStream.Read(bArr, 0, bArr.Length);
                            while (size > 0)
                            {
                                stream.Write(bArr, 0, size);
                                size = responseStream.Read(bArr, 0, bArr.Length);
                            }
                            stream.Close();
                            responseStream.Close();
                            proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                            MessageBox.Show("文件下载完成", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Lab_Exception.Dispatcher.Invoke(new Action(() => {
                            Lab_Exception.Content = "错误代码:" + json.error + "  " + json.msg;
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
                proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
            }
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            Lab_Exception.Visibility = Visibility.Hidden;
            proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(DeleteSingleItem);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void DeleteSingleItem()
        {
            if (list_allItems.Count <= 0)
            {
                proBar.Dispatcher.Invoke(new Action(() => {
                    proBar.Visibility = Visibility.Hidden;
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
                    proBar.Visibility = Visibility.Hidden;
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

            if (!item.singleFile.id.Equals(string.Empty))
            {
                using (var client = new HttpClient())
                {
                    var jsonList = new List<Json_Delete>();

                    var mulContent = new MultipartFormDataContent();
                    var list = new List<KeyValuePair<string, string>>();
                    Net.GetKeyValuePairList("userId", userId, ref list);
                    Net.GetKeyValuePairList("fileId", item.singleFile.id, ref list);
                    Net.GetKeyValuePairList("supportId", supportId, ref list);
                    Net.GetKeyValuePairList("token", token, ref list);
                    Net.GetKeyValueMultipartContent(list, ref mulContent);

                    try
                    {
                        var response = client.PostAsync(Net.Url_FileDelete, mulContent).Result.Content.ReadAsStringAsync().Result;
                        proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                        if (response.StartsWith("{\"error\":0"))
                        {
                            Lsv_File.Dispatcher.Invoke(new Action(() => {
                                Lsv_File.ItemsSource = null;
                            }));
                            selectFid.RemoveAll(s => (s == item.singleFile.id));
                            var json = Json_Delete.JsonStrToList(response);
                            jsonList.Add(json);
                            list_allItems.RemoveAt(item.rowIndex);
                            for (int i = 0; i < list_allItems.Count; i++)
                            {
                                list_allItems[i].rowIndex = i;
                            }
                        }
                        else
                        {
                            var json = Json_Msg.JsonStrToList(response);
                            Lab_Exception.Dispatcher.Invoke(new Action(() => {
                                Lab_Exception.Content = "错误代码:" + json.error + "  " + json.msg;
                                Lab_Exception.Visibility = Visibility.Visible;
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        proBar.Dispatcher.Invoke(new Action(() => {
                            proBar.Visibility = Visibility.Hidden;
                            Lab_Exception.Content = ex.Message;
                            Lab_Exception.Visibility = Visibility.Visible;
                        }));
                    }
                }
            }
            Lsv_File.Dispatcher.Invoke(new Action(() => {
                Lsv_File.ItemsSource = list_allItems;
                Lsv_File.Items.Refresh();
                Lab_Num.Content = Lsv_File.Items.Count + "项";
            }));          
        }

        private void Lsv_File_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Lsv_File.SelectedItems.Count <= 0 || Lsv_File.SelectedItems.Count != 1)
            {
                Lsv_File.ContextMenu = (ContextMenu)this.Resources["Menu_Local"];
                Lsv_File.ContextMenu.IsEnabled = false;
                Lsv_File.ContextMenu.Visibility = Visibility.Hidden;
            }
            else
            {
                var item = (Lsv_SingleItem)Lsv_File.SelectedItem;
                if (item.singleFile.id.Equals(string.Empty))
                {
                    Lsv_File.ContextMenu = (ContextMenu)this.Resources["Menu_Local"];
                }
                else
                {
                    Lsv_File.ContextMenu = (ContextMenu)this.Resources["Menu_Remote"];
                }
                Lsv_File.ContextMenu.IsEnabled = true;
                Lsv_File.ContextMenu.Visibility = Visibility.Visible;
                Lsv_File.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
            }
        }

        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (list_allItems.Count <= 0)
            {
                Lab_Exception.Content = "没有文件";
                Lab_Exception.Visibility = Visibility.Visible;
                return;
            }

            if (Lsv_File.SelectedItems.Count != 1)
            {
                return;
            }

            if(Lsv_File.SelectedItem != null)
            {
                try
                {
                    Lsv_SingleItem item = (Lsv_SingleItem)Lsv_File.SelectedItem;
                    Lsv_File.ItemsSource = null;
                    list_allItems.Remove(item);
                    for (int i = 0; i < list_allItems.Count; i++)
                    {
                        list_allItems[i].rowIndex = i;
                    }
                    Lsv_File.ItemsSource = list_allItems;
                    Lsv_File.Items.Refresh();
                }
                catch { }   
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            instance = null;
        }
    }
}
