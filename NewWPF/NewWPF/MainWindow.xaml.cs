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

namespace NewWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetData_JKLR();
        }

        static public string WholeToken = Login.ID + "|" + Login.user.token;
        public void onFailed()
        {
            MessageBox.Show("请检查网络环境后再试！");
        }

        private List<DebtList_Item> list_Debt = new List<DebtList_Item>();
        public void onSucceedJKLR(string result)
        {
            var jsonList = new List<DebtList_Msg>();
            MessageBox.Show(result);
            var json = DebtList_Msg.JsonStrToList(result);
            jsonList.Add(json);
            foreach (var j in jsonList)
            {
                if (j.error == 0)
                {
                    list_Debt.Clear();
                    foreach (var item in j.data)
                    {

                        list_Debt.Add(new DebtList_Item()
                        {
                            debtId = item.debtId,
                            amount = item.amount,
                            bondInsitutionId = item.bondInsitutionId,
                            debtUnitId = item.debtUnitId,
                            debtTypeName = item.debtTypeName,
                            relatedBank = item.relatedBank,
                            creditUpdateName = item.creditUpdateName,
                            applyTime = item.applyTime,
                            debtStartTime = item.debtStartTime,
                            debtEndTime = item.debtEndTime,
                            baseInterest = item.baseInterest,
                            isInGov = item.isInGov,
                            rateRatio = item.rateRatio,
                            rateAdjustType = item.rateAdjustType,
                            guarantor = item.guarantor,
                            payInterestDate = item.payInterestDate,
                            payInterestMonth = item.payInterestMonth,
                            term = item.term,
                            yearDay = item.yearDay,
                            remark = item.remark,
                        });
                    }
                    DG_JKCX.ItemsSource = list_Debt;
                    DG_JKCX.Items.Refresh();
                }
            }
        }
        public void GetData_JKLR()
        {
            new HttpTask().data("token", WholeToken)
                          .postAsync(Net.Url_queryDebtList, new HttpTask.OnSucceed(onSucceedJKLR), new HttpTask.OnFailed(onFailed));
            MessageBox.Show("nihaoa");
            MessageBox.Show(WholeToken);
        }
    }
}
