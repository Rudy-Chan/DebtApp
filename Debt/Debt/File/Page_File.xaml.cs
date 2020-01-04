using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Page_File : Window
    {
        private Page_Upload page_upload;
        private Page_View page_view;
        private Page_Download page_download;

        public Page_File()
        {
            InitializeComponent();
        }

        private void Btn_Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Btn_Max_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Normal)
            {
                WindowState = WindowState.Normal;
            }
            else
                WindowState = WindowState.Maximized;
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Heading_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Btn_Upload_Ctl_Click(object sender, RoutedEventArgs e)
        {
            if(page_upload==null)
            {
                page_upload = new Page_Upload();
            }
            Dynamic_Page.Content = new Frame() { Content = page_upload };
        }

        private void Btn_View_Ctl_Click(object sender, RoutedEventArgs e)
        {
            if (page_view == null)
            {
                page_view = new Page_View();
            }
            Dynamic_Page.Content = new Frame() { Content = page_view };
        }

        private void Btn_Download_Ctl_Click(object sender, RoutedEventArgs e)
        {
            if (page_download == null)
            {
                page_download = new Page_Download();
            }
            Dynamic_Page.Content = new Frame() { Content = page_download };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (page_upload == null)
            {
                page_upload = new Page_Upload();
            }
            Dynamic_Page.Content = new Frame() { Content = page_upload };
        }

        private void Btn_Skin_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
