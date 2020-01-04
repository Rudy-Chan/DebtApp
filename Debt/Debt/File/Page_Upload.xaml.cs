using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace Debt
{
    /// <summary>
    /// Page_Upload.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Upload : Page
    {
        private List<Data_File> list = new List<Data_File>();
        private int succeed = 0, total = 0;

        public Page_Upload()
        {
            InitializeComponent();
        }

        private void Btn_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            Lab_Info.Visibility = Visibility.Hidden;
            AddLocalFile();
        }

        public void AddLocalFile()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == true)
                {
                    Dg_File.ItemsSource = null;
                    if (ofd.FileNames != null && ofd.FileNames.Length > 0)
                    {
                        foreach (string fileName in ofd.FileNames)
                        {
                            list.Add(new Data_File(list.Count.ToString(), fileName));
                        }
                        Dg_File.ItemsSource = list;
                        Dg_File.Items.Refresh();
                        Lab_Count.Content = list.Count + "项";
                    }
                }
            }
            catch (Exception ex)
            {
                Lab_Info.Content = ex.Message;
                Lab_Info.Visibility = Visibility.Visible;
                Lab_Count.Content = list.Count + "项";
            }
        }

        private async void Btn_Upload_File_Click(object sender, RoutedEventArgs e)
        {
            Lab_Info.Visibility = Visibility.Hidden;
            probar.Visibility = Visibility.Visible;

            await uploadFilesToServer();
        }

        private async Task uploadFilesToServer()
        {
            if (list.Count <= 0)
            {
                Lab_Info.Content= "请选择文件";
                Lab_Info.Visibility = Visibility.Visible;
                probar.Visibility = Visibility.Hidden;
                return;
            }

            succeed = 0;
            total = list.Count;
            probar.Visibility = Visibility.Visible;

            for (int i = list.Count, j = 0; i > 0; i--, j++)
            {
                var file = list[j];
                string path = file.Directory + @"\" + file.Name;
                if (!File.Exists(path))
                {
                    Lab_Info.Content = "未找到文件" + file.Name + "，请重新确认";
                    Lab_Info.Visibility = Visibility.Visible;
                    probar.Visibility = Visibility.Hidden;
                    return;
                }
                try
                {
                    await new HttpTask().data("token", "0002|79c3f938d4fb164e62c0a1e0442151a3")
                      .data("supportId", "A201912220003")
                      .data("fileName", file.Name)
                      .data(path)
                      .postAsync(Url.UploadFile, OnSucceed, OnFailed);
                }
                catch(Exception ex)
                {
                    probar.Visibility = Visibility.Hidden;
                    MessageBox.Show(ex.Message, "异常提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnSucceed(string result)
        {
            succeed++;
            total--;
            if (succeed == list.Count)
            {
                probar.Visibility = Visibility.Hidden;
                Dg_File.ItemsSource = null;
                list.Clear();
                //上传成功提示
                MessageBox.Show(result, "上传成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void OnFailed()
        {
            total--;
            if(total <= 0 && succeed != list.Count)
            {
                probar.Visibility = Visibility.Hidden;
                Dg_File.ItemsSource = null;
                list.Clear();
                //上传失败提示
                MessageBox.Show("请检查网络状况或本机防火墙设置", "上传失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Btn_ClearList_Click(object sender, RoutedEventArgs e)
        {
            Dg_File.ItemsSource = null;
            list.Clear();
        }

        private void Dg_File_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(list.Count > 0 && Dg_File.SelectedItems.Count == 1)
            {
                Lab_Info.Visibility = Visibility.Hidden;
                ContextMenu context = new ContextMenu();
                MenuItem[] items = new MenuItem[2];
                for (int i = 0; i < 2; i++)
                {
                    items[i] = new MenuItem();
                    context.Items.Add(items[i]);
                }
                items[0].Header = "移除";
                items[0].Click += new RoutedEventHandler(Remove_Click);
                items[1].Header = "查看";
                items[1].Click += new RoutedEventHandler(View_Click);
                context.IsOpen = true;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var item = Dg_File.SelectedItem as Data_File;
            Dg_File.ItemsSource = null;
            list.Remove(item);
            Dg_File.ItemsSource = list;
            Dg_File.Items.Refresh();
            Lab_Count.Content = list.Count + "项";
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            var item = Dg_File.SelectedItem as Data_File;
            if (File.Exists(item.Directory + @"\" + item.Name))
            {
                Process.Start(item.Directory + @"\" + item.Name);  //打开某个文件
            }
            else
            {
                Lab_Info.Content = "未找到文件" + item.Name;
                Lab_Info.Visibility = Visibility.Visible;
            }
        }

    }
}
