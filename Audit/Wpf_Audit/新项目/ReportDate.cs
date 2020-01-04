using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Audit
{
    class RateSchedule
    {
        private char tag;               //标志位
        private double baseInterest;    //自定义基准利率
        private double rateRatio;       //利率浮动系数	
        private string startDate;       //开始时间
        private string rateAdjustType;  //利率调整方式
        private string executeDate;     //生效时间

        public char Tag { get { return tag; } }
        public double BaseInterest
        {
            get { return baseInterest; }
            set { baseInterest = value; }
        }
        public double RateRatio
        {
            get { return rateRatio; }
            set { rateRatio = value; }
        }
        public string StartDate { get { return startDate; } }
        public string RateAdjustType { get { return rateAdjustType; } }
        public string ExecuteDate { get { return executeDate; } }
        public RateSchedule(char tag, double baseInterest, double rateRatio, string startDate,string rateAdjustType, string executeDate)
        {
            this.tag = tag;
            this.baseInterest = baseInterest;
            this.rateRatio = rateRatio;
            this.startDate = startDate;
            this.rateAdjustType = rateAdjustType;
            this.executeDate = executeDate;
        }
    }

    class ReceiveElement
    {
        private double perAmount;     //金额
        private double interestRate;  //利率
        private string payDeadline;   //付款截止日期
        private double balance;       //余额

        public ReceiveElement(double perAmount, double interestRate, string payDeadline, double balance)
        {
            this.perAmount = perAmount;
            this.interestRate = interestRate;
            this.payDeadline = payDeadline;
            this.balance = balance;
        }
    }

    class ReceiveItem
    {
        private double amount;         //每笔提款
        private string receiveDate;    //提款时间
        List<ReceiveElement> details;  //详情

        public ReceiveItem(double amount, string receiveDate, List<ReceiveElement> details)
        {
            this.amount = amount;
            this.receiveDate = receiveDate;
            this.details = details;
        }
    }
    class ReportDate
    {

        private double grossAmount;         //借款总额
        private List<ReceiveItem> receiveList;  //单笔提款

        public ReportDate()
        {
            grossAmount = 0;
            receiveList = new List<ReceiveItem>();
        }
        
        private Dictionary<string, double> PrePayPlan(double amount, List<Pay_Plan_Single> payplanLogs)
        {
            Dictionary<string, double> prePlan=new Dictionary<string, double>();  //截止日期+比率
            foreach(var item in payplanLogs)
            {
                try
                {
                    prePlan.Add(item.PayDate, item.Amount);
                }
                catch
                {
                    continue;
                } 
            }
            prePlan.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);  //升序排列

            return prePlan;
        }

        private List<RateSchedule> PreInterestRate(Debt_Apply_Single applyInfo, List<Base_Interest_Single> rateLogs, List<Change_Log_Single> changeLogs)
        {
            List<RateSchedule> preRate = new List<RateSchedule>();

            //添加初始信息
            preRate.Add(new RateSchedule('0', applyInfo.BaseInterest, applyInfo.RateRatio, applyInfo.DebtStartTime, applyInfo.RateAdjustType, applyInfo.DebtStartTime));

            //添加变更表信息
            foreach(var item in changeLogs)
            {
                if(string.Compare(item.ChangeDate, applyInfo.DebtStartTime) >= 0 && string.Compare(item.ChangeDate, applyInfo.DebtEndTime) <= 0)
                {
                    string executeDate;
                    switch(item.AdjustType.Trim())
                    {
                        case "当日": executeDate = Convert.ToDateTime(item.ChangeDate).ToString("yyyy-MM-dd"); break;
                        case "次日": executeDate=Convert.ToDateTime(item.ChangeDate).AddDays(1).ToString("yyyy-MM-dd"); break;
                        case "次月":
                            {
                                DateTime temp = Convert.ToDateTime(item.ChangeDate).AddMonths(1);
                                executeDate = Convert.ToDateTime(temp.Year + '-' + temp.Month + "-1").ToString("yyyy-MM-dd");
                            }; break;
                        case "次年":
                            {
                                DateTime temp = Convert.ToDateTime(item.ChangeDate).AddYears(1);
                                executeDate = Convert.ToDateTime(temp.Year + "-1-1").ToString("yyyy-MM-dd");
                            }; break;
                        case "年放款日":
                        default:executeDate = null; break;
                    }
                    preRate.Add(new RateSchedule('0', item.BaseInterest, item.RateRatio, item.ChangeDate, item.AdjustType, executeDate));
                }
            }
            preRate.RemoveAll(o => string.Compare(o.ExecuteDate, applyInfo.DebtEndTime) > 0 || o.ExecuteDate == null);
            preRate.Sort((x, y) => string.Compare(x.ExecuteDate, y.ExecuteDate));

            //添加基准利率表信息
            switch (applyInfo.BaseInterestType)
            {
                case 0://lpr
                case 1://央行
                    {
                        int currentNum = preRate.Count;
                        List<string> date = new List<string>();
                        foreach (var item in preRate)
                        {
                            date.Add(item.ExecuteDate);
                        }
                        date.Add(applyInfo.DebtEndTime);

                        for (int i=0;i<currentNum;i++)
                        {
                            foreach(var item in rateLogs)
                            {
                                if(string.Compare(item.StartTime, date[i]) >= 0 && string.Compare(item.StartTime, date[i+1]) < 0 && item.Type == applyInfo.BaseInterestType)
                                {
                                    string executeDate;
                                    switch (preRate[i].RateAdjustType.Trim())
                                    {
                                        case "当日": executeDate = Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd"); break;
                                        case "次日": executeDate = Convert.ToDateTime(item.StartTime).AddDays(1).ToString("yyyy-MM-dd"); break;
                                        case "次月":
                                            {
                                                DateTime temp = Convert.ToDateTime(item.StartTime).AddMonths(1);
                                                executeDate = Convert.ToDateTime(temp.Year + '-' + temp.Month + "-1").ToString("yyyy-MM-dd");
                                            }; break;
                                        case "次年":
                                            {
                                                DateTime temp = Convert.ToDateTime(item.StartTime).AddYears(1);
                                                executeDate = Convert.ToDateTime(temp.Year + "-1-1").ToString("yyyy-MM-dd");
                                            }; break;
                                        case "年放款日":
                                        default: executeDate = null; break;
                                    }
                                    preRate.Add(new RateSchedule('1', item.Rate, preRate[i].RateRatio, item.StartTime, preRate[i].RateAdjustType, executeDate));
                                }
                            }
                        }
                        preRate.RemoveAll(o => string.Compare(o.ExecuteDate, applyInfo.DebtEndTime) > 0 || o.ExecuteDate == null);
                        preRate.Sort((x, y) => string.Compare(x.ExecuteDate, y.ExecuteDate));

                        //调整利率
                        double tempInterest = preRate[0].BaseInterest;
                        double tempRatio = preRate[0].RateRatio;
                        for(int i = 1; i < preRate.Count; i++)
                        {
                            if (preRate[i].Tag == '0')
                            {
                                preRate[i].BaseInterest = tempInterest;
                                tempRatio = preRate[i].RateRatio;
                            }
                            else
                            {
                                tempInterest = preRate[i].BaseInterest;
                                preRate[i].RateRatio = tempRatio;
                            }
                        }
                    }
                    break;
                default: break;
            }
            return preRate;
        }

        private void GenerateSource(Debt_Apply_Single applyInfo, List<Pay_Plan_Single> payplanLogs, List<Change_Log_Single> changeLogs,
            List<Receive_Log_Single> receiveLogs,List<Payment_Log_Single> paymentLogs)
        {
            foreach (var item in receiveLogs)
            {
                grossAmount += item.Amount;
                receiveList.Add(new ReceiveItem(item.Amount,item.receiveDate,))
            }



        }
    }
}
