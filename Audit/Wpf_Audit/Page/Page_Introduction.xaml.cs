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

namespace Wpf_Audit
{
    /// <summary>
    /// Page_Introduction.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Introduction : Page
    {
        private User_SelfInfo user;
        private Win_Audit parent;
        private List<KeyValuePair<string, int>> list_BondInstitution = new List<KeyValuePair<string, int>>();
        public Page_Introduction(User_SelfInfo user, Win_Audit parent)
        {
            InitializeComponent();
            this.user = user;
            this.parent = parent;
        }

        public void ShowUserInfo()
        {
            Tbx_Name.Text = user.realName;
            //Tbx_Company.Text = user.companyName;
            Tbx_Department.Text = user.department;
            Tbx_Rank.Text = user.rank.ToString();
            Tbx_ContactInfo.Text = user.contactInfo;
        }

        private void Btn_PsdModify_Click(object sender, RoutedEventArgs e)
        {
            ModifySelfPsd();
        }

        private void ModifySelfPsd()
        {
            if (PsdBox_Old.Password.Equals(string.Empty))
            {
                MessageBox.Show("请输入原密码", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (PsdBox_New.Password.Equals(string.Empty) || PsdBox_Ensure.Password.Equals(string.Empty))
            {
                MessageBox.Show("请输入新密码", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (!PsdBox_New.Password.Equals(PsdBox_Ensure.Password))
            {
                MessageBox.Show("请确认两次输入的密码是否一致", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string oldPsd = PsdBox_Old.Password;
            string newPsd = PsdBox_New.Password;

            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("userId", user.userId, ref list);
                Net.GetKeyValuePairList("oldPass", oldPsd, ref list);
                Net.GetKeyValuePairList("newPass", newPsd, ref list);
                Net.GetKeyValuePairList("token", user.token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                try
                {
                    proBar.Visibility = Visibility.Visible;
                    var response = client.PostAsync(Net.Url_ResetPassword, mulContent).Result.Content.ReadAsStringAsync().Result;
                    proBar.Visibility = Visibility.Hidden;
                    if (response.StartsWith("{\"error\":0"))
                    {
                        MessageBox.Show("密码修改成功，请重新登录", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        parent.Close();
                        //关闭当前窗口，返回到登录界面     
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Json_Msg.ShowMsg(json);
                    }
                }
                catch (Exception ex)
                {
                    proBar.Visibility = Visibility.Hidden;
                    MessageBox.Show(ex.Message, "异常提醒", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void GetCompanyNameToCombobox()
        {
            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                Net.GetKeyValuePairList("token", user.token, ref list);
                Net.GetKeyValuePairList("type", "1", ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                var jsonList = new List<Json_BondInstitution>();
                list_BondInstitution.Clear();
                Tbx_Company.Dispatcher.Invoke(new Action(() => { Tbx_Company.Items.Clear(); }));

                try
                {
                    var response = client.PostAsync(Net.Url_BondInstitutions, mulContent).Result.Content.ReadAsStringAsync().Result;
                    proBar.Dispatcher.Invoke(new Action(() => { proBar.Visibility = Visibility.Hidden; }));
                    if (response.StartsWith("{\"error\":0"))
                    {
                        var json = Json_BondInstitution.JsonStrToList(response);
                        jsonList.Add(json);
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        list_BondInstitution.Add(new KeyValuePair<string, int>(user.companyName, user.companyId));
                        Tbx_Company.Dispatcher.Invoke(new Action(() => {
                            Tbx_Company.Items.Add(user.companyName);
                            Tbx_Company.Text = user.companyName;
                        }));
                        Json_Msg.ShowMsg(json);
                        return;
                    }
                }
                catch
                {
                    list_BondInstitution.Add(new KeyValuePair<string, int>(user.companyName, user.companyId));
                    proBar.Dispatcher.Invoke(new Action(() => {
                        proBar.Visibility = Visibility.Hidden;
                        Tbx_Company.Items.Add(user.companyName);
                        Tbx_Company.Text = user.companyName;
                    }));
                    return;
                }

                foreach (var j in jsonList)
                {
                    if (j.error == 0)
                    {
                        Tbx_Company.Dispatcher.Invoke(new Action(() => {
                            foreach (var item in j.data)
                            {
                                list_BondInstitution.Add(new KeyValuePair<string, int>(item.bondInstitutionName, item.bondInstitutionId));
                                Tbx_Company.Items.Add(item.bondInstitutionName);
                            }
                            Tbx_Company.Text = user.companyName;
                        }));
                    }
                }
            }
        }

        private void Btn_SelfMsgModify_Click(object sender, RoutedEventArgs e)
        {
            ChangeStyle();
        }

        private void Btn_SaveChange_Click(object sender, RoutedEventArgs e)
        {
            SaveStyle();
            SubmitChangeInfo();
        }

        private void SubmitChangeInfo()
        {
            if (Tbx_Company.Text.Equals(string.Empty))
            {
                MessageBox.Show("请输入公司名称", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Tbx_ContactInfo.Text.Equals(string.Empty))
            {
                MessageBox.Show("请输入联系方式", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Tbx_Department.Text.Equals(string.Empty))
            {
                MessageBox.Show("请输入部门名称", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Tbx_Name.Text.Equals(string.Empty))
            {
                MessageBox.Show("请输入用户名", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string companyName = Tbx_Company.Text;
            string contactInfo = Tbx_ContactInfo.Text;
            string department = Tbx_Department.Text;
            string name = Tbx_Name.Text;
            string rank = Tbx_Rank.Text;
            int companyId = user.companyId;

            if (companyName.Equals(user.companyName) && contactInfo.Equals(user.contactInfo) && department.Equals(user.department) && name.Equals(user.realName))
                return;

            using (var client = new HttpClient())
            {
                var mulContent = new MultipartFormDataContent();
                var list = new List<KeyValuePair<string, string>>();
                foreach(var item in list_BondInstitution)
                {
                    if(item.Key == companyName)
                    {
                        companyId = item.Value;
                        break;
                    }
                }

                Net.GetKeyValuePairList("userId", user.userId, ref list);
                Net.GetKeyValuePairList("realName", name, ref list);
                Net.GetKeyValuePairList("contact", contactInfo, ref list);
                Net.GetKeyValuePairList("department", department, ref list);
                Net.GetKeyValuePairList("companyId", companyId.ToString(), ref list);
                Net.GetKeyValuePairList("token", user.token, ref list);
                Net.GetKeyValueMultipartContent(list, ref mulContent);

                try
                {
                    proBar.Visibility = Visibility.Visible;
                    var response = client.PostAsync(Net.Url_ChangeUserInfo, mulContent).Result.Content.ReadAsStringAsync().Result;
                    proBar.Visibility = Visibility.Hidden;
                    if (response.StartsWith("{\"error\":0"))
                    {
                        user.contactInfo = contactInfo;
                        user.department = department;
                        user.realName = name;
                        user.companyId = companyId;
                        user.companyName = companyName;
                        parent.User.contactInfo = contactInfo;
                        parent.User.department = department;
                        parent.User.realName = name;
                        parent.User.companyId = companyId;
                        parent.User.companyName = companyName;
                        MessageBox.Show("个人信息修改成功", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var json = Json_Msg.JsonStrToList(response);
                        Json_Msg.ShowMsg(json);
                    }
                }
                catch (Exception ex)
                {
                    proBar.Visibility = Visibility.Hidden;
                    MessageBox.Show(ex.Message, "异常提醒", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void ChangeStyle()
        {
            Tbx_Name.Background = Brushes.White;
            Tbx_Name.BorderBrush = Brushes.Gray;
            Tbx_Name.BorderThickness = new Thickness(1, 1, 1, 1);
            Tbx_Name.IsReadOnly = false;
            Tbx_Department.Background = Brushes.White;
            Tbx_Department.BorderBrush = Brushes.Gray;
            Tbx_Department.BorderThickness = new Thickness(1, 1, 1, 1);
            Tbx_Department.IsReadOnly = false;
            Tbx_Company.Background = Brushes.White;
            Tbx_Company.BorderBrush = Brushes.Gray;
            Tbx_Company.BorderThickness = new Thickness(1, 1, 1, 1);
            Tbx_Company.IsEnabled = true;
            Tbx_Company.IsReadOnly = false;
            Tbx_ContactInfo.Background = Brushes.White;
            Tbx_ContactInfo.BorderBrush = Brushes.Gray;
            Tbx_ContactInfo.BorderThickness = new Thickness(1, 1, 1, 1);
            Tbx_ContactInfo.IsReadOnly = false;
        }

        void SaveStyle()
        {
            Tbx_Name.Background = Brushes.Transparent;
            Tbx_Name.BorderBrush = Brushes.Transparent;
            Tbx_Name.BorderThickness = new Thickness(0, 0, 0, 0);
            Tbx_Name.IsReadOnly = true;
            Tbx_Department.Background = Brushes.Transparent;
            Tbx_Department.BorderBrush = Brushes.Transparent;
            Tbx_Department.BorderThickness = new Thickness(0, 0, 0, 0);
            Tbx_Department.IsReadOnly = true;
            Tbx_Company.Background = Brushes.Transparent;
            Tbx_Company.BorderBrush = Brushes.Transparent;
            Tbx_Company.BorderThickness = new Thickness(0, 0, 0, 0);
            Tbx_Company.IsEnabled = false;
            Tbx_Company.IsReadOnly = true;
            Tbx_ContactInfo.Background = Brushes.Transparent;
            Tbx_ContactInfo.BorderBrush = Brushes.Transparent;
            Tbx_ContactInfo.BorderThickness = new Thickness(0, 0, 0, 0);
            Tbx_ContactInfo.IsReadOnly = true;
        }
    }
}
