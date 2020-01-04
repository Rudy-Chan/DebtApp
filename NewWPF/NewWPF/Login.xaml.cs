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
using System.Windows.Shapes;

namespace NewWPF
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>

    class UserName
    {
        public string id { get; set; }
        public UserName(string id)
        {
            this.id = id;
        }
    }

    public partial class Login : Window
    {
        private string serverIp;
        private string alluser;
        private List<UserName> list_users = new List<UserName>();
        public static User_SelfInfo user { get; set; }

        public Login()
        {
            serverIp = ConfigHelper.GetAppConfig("IP");
            alluser = ConfigHelper.GetAppConfig("User");
            if (serverIp != null && serverIp != string.Empty)
                SetUrl(serverIp);
            InitializeComponent();
        }

        public static string ID;
        private void SetUrl(string IP)
        {
            try
            {
                Net.Url_Login = @"http://" + IP.Trim() + @"/api/user.php?act=login";
                Net.Url_Logout = @"http://" + IP.Trim() + @"/api/user.php?act=logout";
                Net.Url_getInformation= @"http://" + IP.Trim() + @"/api/deal.php?act=getInformation";
                Net.Url_queryDebtList = @"http://" + IP.Trim() + @"/api/deal.php?act=queryDebtList";
                //Net.Url_UserGetUserStatus = @"http://" + IP.Trim() + "/api/user.php?act=getUserStatus";
                //Net.Url_ResetPassword = @"http://" + IP.Trim() + "/api/user.php?act=resetPassword";
                //Net.Url_ChangeUserInfo = @"http://" + IP.Trim() + "/api/user.php?act=changeUserInfo";

                //Net.Url_FileUpload = @"http://" + IP.Trim() + "/api/file.php?act=upload";
                //Net.Url_FileView = @"http://" + IP.Trim() + "/api/file.php?act=getFileList";
                //Net.Url_FileDownload = @"http://" + IP.Trim() + "/api/file.php?act=download";
                //Net.Url_FileDelete = @"http://" + IP.Trim() + "/api/file.php?act=delete";

                //Net.Url_DebtChecked = @"http://" + IP.Trim() + "/api/audit.php?act=getAuditList";
                //Net.Url_DebtNotChecked = @"http://" + IP.Trim() + "/api/audit.php?act=getAuditApplyList";
                //Net.Url_DebtChanged = @"http://" + IP.Trim() + "/api/audit.php?act=getChangeAuditList";
                //Net.Url_DebtApplyPass = @"http://" + IP.Trim() + "/api/audit.php?act=passApply";
                //Net.Url_DebtRejectApply = @"http://" + IP.Trim() + "/api/audit.php?act=rejectApply";

                //Net.Url_SingleApplyInfo = @"http://" + IP.Trim() + "/api/deal.php?act=getApplyInfo";
                //Net.Url_SingleChangeInfo = @"http://" + IP.Trim() + "/api/deal.php?act=getChangeInfo";

                //Net.Url_UserLogin = @"http://" + IP.Trim() + "/api/user.php?act=login";
                //Net.Url_UserLogout = @"http://" + IP.Trim() + "/api/user.php?act=logout";


                //Net.Url_ApplyLoan = @"http://" + IP.Trim() + "/api/deal.php?act=applyLoan"; // 申请贷款

                //Net.Url_ChangeLoanApply = @"http://" + IP.Trim() + "/api/deal.php?act=changeLoanApply"; //修改贷款申请信息
                //Net.Url_CancelLoanApply = @"http://" + IP.Trim() + "/api/deal.php?act=cancelLoanApply"; //取消/删除贷款申请
                //Net.Url_ConfirmLoanApply = @"http://" + IP.Trim() + "/api/deal.php?act=confirmLoanApply";//确认贷款申请/提交审核
                //Net.Url_GetBasicInterest = @"http://" + IP.Trim() + "/api/deal.php?act=getBasicInterest "; //获取基准利率

                //Net.Url_ApplyChange = @"http://" + IP.Trim() + "/api/deal.php?act=applyChange"; //  biangeng
                //Net.Url_CancelChangeApply = @"http://" + IP.Trim() + "/api/deal.php?act=cancelChangeApply"; // quxiao biangeng
                //Net.Url_ChangeChangeApply = @"http://" + IP.Trim() + "/api/deal.php?act=changeChange"; // xiugaibiangeng
                //Net.Url_ConfirmChangeApply = @"http://" + IP.Trim() + "/api/deal.php?act=confirmChangeApply "; // 申请贷款


                //Net.Url_ApplyPayment = @"http://" + IP.Trim() + "/api/deal.php?act=applyPayment"; //付款款录入
                //Net.Url_ChangePayment = @"http://" + IP.Trim() + "/api/deal.php?act=changePayment"; // 付款信息修改   
                //Net.Url_CancelPayment = @"http://" + IP.Trim() + "/api/deal.php?act=cancelPayment"; // 软删除已录入付款信息

                ////Net.Url_getApplyList1 = @"http://" + IP.Trim() + "/api/deal.php?act=getApplyList";
                //Net.Url_ApplyReceive = @"http://" + IP.Trim() + "/api/deal.php?act=applyReceive"; //收款款录入
                //Net.Url_ChangeReceive = @"http://" + IP.Trim() + "/api/deal.php?act=changeReceive"; // 收款信息修改   
                //Net.Url_CancelReceive = @"http://" + IP.Trim() + "/api/deal.php?act=cancelReceive"; // 软删除已录入收款信息

                //Net.Url_getBondInstitutions = @"http://" + IP.Trim() + "/api/deal.php?act=getBondInstitutions"; //获取债权单位列表
                //Net.Url_getClassifications = @"http://" + IP.Trim() + "/api/deal.php?act=getTermClassifications"; //获取期限分类列表
                //Net.Url_getDebtTypes = @"http://" + IP.Trim() + "/api/deal.php?act=getDebtTypes"; //获取贷款方式列表
                //Net.Url_getPayTypes = @"http://" + IP.Trim() + "/api/deal.php?act=getPayTypes"; //获取还款方式列表
                //Net.Url_getRelatedBanks = @"http://" + IP.Trim() + "/api/deal.php?act=getRelatedBanks"; //获取关联银行列表
                //Net.Url_getCreditTypes = @"http://" + IP.Trim() + "/api/deal.php?act=getCreditTypes"; //获取增信方式列表
                //Net.Url_getFloatingInterests = @"http://" + IP.Trim() + "/api/deal.php?act=getFloatingInterests";//获取浮动利率系数
                //Net.Url_getInterestRateAdjustType = @"http://" + IP.Trim() + "/api/deal.php?act=getInterestRateAdjustType"; //获取利率调整方式类型

                //Net.Url_getApplyList = @"http://" + IP.Trim() + "/api/deal.php?act=getApplyList"; //获取贷款列表
                //Net.Url_getReceiveList = @"http://" + IP.Trim() + "/api/deal.php?act=getReceiveList";  //获取用户发起的收款申请列表
                //Net.Url_getPaymentList = @"http://" + IP.Trim() + "/api/deal.php?act=getPaymentList";  //获取用户发起的付款申请列表
                //Net.Url_getChangeList = @"http://" + IP.Trim() + "/api/deal.php?act=getChangeList";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static string ServerIpAddress = ConfigHelper.GetAppConfig("IP");
        public void onSucceed(string result)
        {
            MessageBox.Show(result);
            if (result.StartsWith("{\"error\":0"))
            {
                var json = Json_Login.JsonStrToList(result);
                Login.user = new User_SelfInfo(json.department, json.rank, json.companyId, json.companyName, json.name, json.phone,json.auditRank, json.token);

                string[] users;
                if (alluser != null && !alluser.Equals(string.Empty))
                {
                    users = alluser.Split('|');
                    alluser = Cbx_User.Text + '|';
                    foreach (var item in users)
                    {
                        if (Cbx_User.Text == item || item == string.Empty)
                        {
                            continue;
                        }
                        else
                        {
                            alluser = alluser + item + '|';
                        }
                    }
                    alluser = alluser.Trim('|');
                }
                else
                {
                    alluser = Cbx_User.Text;
                }
                ConfigHelper.UpdateAppConfig("User", alluser);
                this.Hide();
                Window mainWin = new MainWindow();
                mainWin.Show();
            }
        }

        public void onFailed()
        {
            MessageBox.Show("请检查网络环境后再试！");
        }
        private void login(string userName, string password)
        {

            new HttpTask().data("userName", userName)
                          .data("password", password)
                          .postAsync(Net.Url_Login, new HttpTask.OnSucceed(onSucceed), new HttpTask.OnFailed(onFailed));
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Min_Clock(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Cbx_User_DropDownClosed(object sender, EventArgs e)
        {
            if ((Cbx_User.SelectedItem as UserName) != null)
                Cbx_User.Text = (Cbx_User.SelectedItem as UserName).id;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button closeButton = sender as Button;
                list_users.RemoveAll(u => u.id == closeButton.Tag.ToString());
                alluser = string.Empty;
                foreach (var item in list_users)
                {
                    alluser = alluser + item.id + '|';
                }
                alluser = alluser.Trim('|');
                ConfigHelper.UpdateAppConfig("User", alluser);
                this.Cbx_User.ItemsSource = list_users;
                Cbx_User.Items.Refresh();
            }
            catch { }
        }

        private void login(object sender, RoutedEventArgs e)
        {
            if (Cbx_User.Text.Equals(string.Empty) || softkeyboard.Pbx_Password.Password.Equals(string.Empty))
                return;
            ID = Cbx_User.Text;
            login(Cbx_User.Text, softkeyboard.Pbx_Password.Password);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)//按键是否按下
            {
                //执行移动
                this.DragMove();//移动窗口
            }
        }

        private void UI_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Cbx_User.Items.Clear();
                Tbx_IPset.Text = serverIp;
                if (alluser != null && !alluser.Equals(string.Empty))
                {
                    string[] users = alluser.Split('|');
                    foreach (var item in users)
                    {
                        list_users.Add(new UserName(item));
                    }
                    Cbx_User.ItemsSource = list_users;
                    Cbx_User.Text = users[0];
                    //Cbx_User.SelectedValue = users[0];
                }
            }
            catch { }
        }

        private void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            Cbx_User.Text = null;
            softkeyboard.Pbx_Password.Password = null;
        }

        private void Btn_IpSet_Click(object sender, RoutedEventArgs e)
        {
            if (IPSET.Visibility == Visibility.Hidden)
                IPSET.Visibility = Visibility.Visible;
            else
                IPSET.Visibility = Visibility.Hidden;
        }

        private void Btn_Ip_Click(object sender, RoutedEventArgs e)
        {
            if (serverIp != null)
            {
                ConfigHelper.UpdateIPConfig("IP", Tbx_IPset.Text);
                MessageBox.Show("设置成功！");
            }
        }



    }
}
