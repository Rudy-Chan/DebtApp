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

namespace Wpf_WH
{
    /// <summary>
    /// Page_File.xaml 的交互逻辑
    /// </summary>
    public partial class Page_File : Page
    {
        private string token;
        public Page_File(string token)
        {
            this.token = token;
           
            InitializeComponent();
        }


        
        //软删除文件列表
        private List<Dg_FileItem> list_File = new List<Dg_FileItem>();//文件表
        public void ShowDeletedFile()
        {
            new HttpTask().data("token", token)
                          //.data("name", FileDle_realName_textBox.Text)
                          //.data("startTime", DP_file_st.SelectedDate.ToString())
                          //.data("endTime", DP_file_et.SelectedDate.ToString())
                          .postAsync(Net.Url_FileMsg, new HttpTask.OnSucceed(onSucceed_List), new HttpTask.OnFailed(onFailed_List));
        }

        public void onSucceed_List(string result)
        {
            MessageBox.Show(result);
            var jsonList = new List<Json_FileMsg>();
            if (result.StartsWith("{\"error\":0"))
            {
                var json = Json_FileMsg.JsonStrToList(result);
                jsonList.Add(json);

            }
            else
            {
                var json = Json_Msg.JsonStrToList(result);
                Json_Msg.ShowMsg(json);
            }
            foreach (var j in jsonList)
            {
                if (j.error == 0)
                {
                    list_File.Clear();
                    foreach (var item in j.data)
                    {
                        list_File.Add(new Dg_FileItem()
                        {
                            id = item.id,
                            filePath=item.filePath,
                            fileSupportId = item.fileSupportId,
                            userName = item.userName,
                            fileName = item.fileName,
                            fileSize = item.fileSize,
                            uploadTime = item.uploadTime,
                            fileType = item.fileType,
                            name=item.name,
                            
                        });
                    }
                    Dg_FileDeleted.ItemsSource = list_File;
                    Dg_FileDeleted.Items.Refresh();
                }
            }
        }
        public void onFailed_List()
        {
            MessageBox.Show("请检查网络环境后再试！");
        }
        
        private void Btn_queryFile_Click(object sender, RoutedEventArgs e)
        {
            ShowDeletedFile();
        }



        //删除文件
        private void Btn_FileDelete_Click(object sender, RoutedEventArgs e)
        {
            int num = Dg_FileDeleted.SelectedIndex;

            new HttpTask().data("token", token)
                          .data("fileId", list_File[num].id.ToString())
                          .postAsync(Net.Url_deleteFile, new HttpTask.OnSucceed(onSucceed_DelFile), new HttpTask.OnFailed(onFailed_DelFile));
            ShowDeletedFile();
        }
        public void onSucceed_DelFile(string result)
        {
            MessageBox.Show("所选项已解锁");
        }
        public void onFailed_DelFile()
        {
            MessageBox.Show("请检查网络环境后再试！");
        }

    }
}
