using System;
using System.Collections.Generic;
using System.Linq;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Win_Audit : Window
    {
        private string serverIp;
        private User_SelfInfo user;
        public User_SelfInfo User
        {
            get { return user; }
            set { user = value; }
        }

        private Page_Checked page1 = null;
        private Page_NotChecked page2 = null;
        private Page_Changed page3 = null;
        private Page_Introduction page4 = null;
        private Page_OperationLog page5 = null;

        public Win_Audit(string serverIp, User_SelfInfo user)
        {
            this.serverIp = serverIp;
            this.user = user;
            SetUrl(serverIp);
            InitializeComponent();
        }

        private void SetUrl(string IP)
        {
            if (IP == null || IP.Equals(string.Empty))
            {
                MessageBox.Show("请设置服务器IP", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Net.Url_Logout = @"http://" + IP.Trim() + @"/api/user.php?act=logout";
            Net.Url_ResetPassword = @"http://" + IP.Trim() + @"/api/user.php?act=resetPassword";
            Net.Url_ChangeUserInfo = @"http://" + IP.Trim() + @"/api/user.php?act=changeUserInfo";
            Net.Url_OperationLogInfo = @"http://" + IP.Trim() + @"/api/user.php?act=getOperationList";
            Net.Url_FileUpload = @"http://" + IP.Trim() + @"/api/file.php?act=upload";
            Net.Url_FileView = @"http://" + IP.Trim() + @"/api/file.php?act=getFileList";
            Net.Url_FileDownload = @"http://" + IP.Trim() + @"/api/file.php?act=download";
            Net.Url_FileDelete = @"http://" + IP.Trim() + @"/api/file.php?act=delete";
            Net.Url_DebtChecked = @"http://" + IP.Trim() + @"/api/audit.php?act=getAuditList";
            Net.Url_DebtNotChecked = @"http://" + IP.Trim() + @"/api/audit.php?act=getAuditApplyList";
            Net.Url_DebtChanged = @"http://" + IP.Trim() + @"/api/audit.php?act=getChangeAuditList";
            Net.Url_DebtApplyPass = @"http://" + IP.Trim() + @"/api/audit.php?act=passApply";
            Net.Url_DebtRejectApply = @"http://" + IP.Trim() + @"/api/audit.php?act=rejectApply";
            Net.Url_SingleApplyInfo = @"http://" + IP.Trim() + @"/api/deal.php?act=getApplyInfo";
            Net.Url_SingleChangeInfo = @"http://" + IP.Trim() + @"/api/deal.php?act=getChangeInfo";
            Net.Url_BondInstitutions = @"http://" + IP.Trim() + @"/api/deal.php?act=getBondInstitutions";
        }

        private void Tree_Logout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("是否退出登录", "消息提示", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                LocalUserLogout();
                Application.Current.Shutdown();
            }
        }

        private void LocalUserLogout()
        {
            if (user.userId != null && user.token != null)
            {
                using (var client = new HttpClient())
                {
                    var mulContent = new MultipartFormDataContent();
                    var list = new List<KeyValuePair<string, string>>();
                    Net.GetKeyValuePairList("userId", user.userId.Trim(), ref list);
                    Net.GetKeyValuePairList("token", user.token.Trim(), ref list);
                    Net.GetKeyValueMultipartContent(list, ref mulContent);

                    try
                    {
                        var response = client.PostAsync(Net.Url_Logout, mulContent).Result.Content.ReadAsStringAsync().Result;
                    }
                    catch
                    {
                        MessageBox.Show("请检查网络连接，稍后重试", "异常提醒", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Tree_Checked_Selected(object sender, RoutedEventArgs e)
        {
            if (page1 == null)
            {
                page1 = new Page_Checked(user.token, user.realName);
            }
            page1.userName = user.realName;
            page1.Lab_Empty.Visibility = Visibility.Hidden;
            page1.Lab_Exception.Visibility = Visibility.Hidden;
            page1.proBar.Visibility = Visibility.Visible;
            page1.Dp_DebtStart.SelectedDate = null;
            page1.Dp_DebtEnd.SelectedDate = null;
            Change_Page.Content = new Frame() { Content = page1 };
            Thread thread = new Thread(page1.GetCheckedDebtApplication);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void Tree_NotChecked_Selected(object sender, RoutedEventArgs e)
        {
            if (page2 == null)
            {
                page2 = new Page_NotChecked(user.token, user.realName);
            }
            page2.userName = user.realName;
            page2.Lab_Empty.Visibility = Visibility.Hidden;
            page2.Lab_Exception.Visibility = Visibility.Hidden;
            page2.proBar.Visibility = Visibility.Visible;
            page2.Dp_DebtStart.SelectedDate = null;
            page2.Dp_DebtEnd.SelectedDate = null;
            Change_Page.Content = new Frame() { Content = page2 };
            Thread thread = new Thread(page2.GetUnCheckedDebtApplication);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void Tree_Changed_Selected(object sender, RoutedEventArgs e)
        {
            if (page3 == null)
            {
                page3 = new Page_Changed(user.token, user.realName);
            }
            page3.userName = user.realName;
            page3.Lab_Empty.Visibility = Visibility.Hidden;
            page3.Lab_Exception.Visibility = Visibility.Hidden;
            page3.proBar.Visibility = Visibility.Visible;
            page3.Dp_DebtStart.SelectedDate = null;
            page3.Dp_DebtEnd.SelectedDate = null;
            Change_Page.Content = new Frame() { Content = page3 };
            Thread thread = new Thread(page3.GetChangingApplication);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void Tree_Introduction_Selected(object sender, RoutedEventArgs e)
        {
            if (page4 == null)
            {
                page4 = new Page_Introduction(user, this);
            }
            page4.ShowUserInfo();
            Change_Page.Content = new Frame() { Content = page4 };
            page4.proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(page4.GetCompanyNameToCombobox);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LocalUserLogout();
            Application.Current.Shutdown();
        }

        private void UI_Loaded(object sender, RoutedEventArgs e)
        {
            if (page1 == null)
            {
                page1 = new Page_Checked(user.token, user.realName);
            }
            page1.userName = user.realName;
            Change_Page.Content = new Frame() { Content = page1 };
            page1.Lab_Empty.Visibility = Visibility.Hidden;
            page1.Lab_Exception.Visibility = Visibility.Hidden;
            page1.proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(page1.GetCheckedDebtApplication);
            thread.IsBackground = true;//设置为后台线程
            thread.Start();//开始线程
        }

        private void Tree_OperationLog_Selected(object sender, RoutedEventArgs e)
        {
            if (page5 == null)
            {
                page5 = new Page_OperationLog(user.token);
            }
            page5.Lab_Empty.Visibility = Visibility.Hidden;
            page5.Dp_TimeStart.SelectedDate = null;
            page5.Dp_TimeEnd.SelectedDate = null;
            Change_Page.Content = new Frame() { Content = page5 };
            page5.proBar.Visibility = Visibility.Visible;
            Thread thread = new Thread(page5.GetUserOperationLog);
            thread.IsBackground = true;//设置为后台线程
            thread.Start("1");//开始线程
        }
    }
}
