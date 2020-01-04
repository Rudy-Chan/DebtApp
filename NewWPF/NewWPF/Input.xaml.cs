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
    /// Input.xaml 的交互逻辑
    /// </summary>
    public partial class Input : Window
    {
        public Input()
        {
            InitializeComponent();
            GetInformation();
        }

        static public string WholeToken = Login.ID + "|" + Login.user.token;
        public void onSucceed(string result)
        {
            var jsonList = new List<Json_Information>();
            MessageBox.Show(result);
            var json = Json_Information.JsonStrToList(result);
            jsonList.Add(json);
            foreach (var j in jsonList)
            {
                if (j.error == 0)
                {
                    foreach (var item in j.debtTypes)
                    {
                        comboBoxJKLX.Items.Add(item.debtType);
                    }
                    foreach (var item in j.creditTypes)
                    {
                        comboBoxZXFS.Items.Add(item.value);
                    }
                    foreach (var item in j.bondInstitutions)
                    {
                        comboBoxJDJG.Items.Add(item.value);
                    }
                }
            }
        }

        public void onFailed()
        {
            MessageBox.Show("请检查网络环境后再试！");
        }

        public void GetInformation()
        {
            new HttpTask().data("token", WholeToken)
                          .postAsync(Net.Url_getInformation, new HttpTask.OnSucceed(onSucceed), new HttpTask.OnFailed(onFailed));
            MessageBox.Show("nihaoa");
            MessageBox.Show(WholeToken);
        }

        
    }
}
