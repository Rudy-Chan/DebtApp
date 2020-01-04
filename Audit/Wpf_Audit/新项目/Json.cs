using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

namespace Wpf_Audit
{
    class Json
    {
        private int error;   //错误码
        private string msg;  //错误信息

        public int Error { get { return error; } }
        public string Msg { get { return msg; } }

        public static void JsonStrToObject(string json)
        {
            //反序列化
            var model= JsonConvert.DeserializeObject<Json>(json);
            MessageBox.Show(model.msg, "异常提醒", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    class Debt_Apply_Single
    {
        private string debtId;
        private double amount;               //金额
        private string bondInstitutionName;  //债权单位名称
        private int debtUnitId;              //借款单位Id
        private string debtTypeName;         //贷款类型
        private string relatedBank;          //关联银行
        private string creditUpdateName;     //增信方式
        private string applyTime;            //录入时间
        private string debtStartTime;        //起贷时间（第一次提款）
        private string debtEndTime;          //终贷时间
        private int baseInterestType;        //基准利率类型0:LPR1:基准利率2:自定义	
        private double baseInterest;         //自定义基准利率
        private int isInGov;                 //进入政府债务系统
        private double rateRatio;            //利率浮动系数
        private string rateAdjustType;       //利率调整方式
        private string guarantor;            //担保方
        private string payInterestDate;      //付息时间
        private int payInterestMonth;        //付息期数
        private int term;                    //贷款期限(月)
        private int yearDay;                 //每年天数
        private int status;                  //状态
        private string userName;             //操作员
        private string remark;

        public string DebtId { get { return debtId; } }
        public double Amount { get { return amount; } }
        public string BondInstitutionName { get { return bondInstitutionName; } }
        public int DebtUnitId { get { return debtUnitId; } }
        public string DebtTypeName { get { return debtTypeName; } }
        public string RelatedBank { get { return relatedBank; } }
        public string CreditUpdateName { get { return creditUpdateName; } }
        public string ApplyTime { get { return applyTime; } }
        public string DebtStartTime { get { return debtStartTime; } }
        public string DebtEndTime { get { return debtEndTime; } }
        public int BaseInterestType { get { return baseInterestType; } }
        public double BaseInterest { get { return baseInterest; } }
        public int IsInGov { get { return isInGov; } }
        public double RateRatio { get { return rateRatio; } }
        public string RateAdjustType { get { return rateAdjustType; } }
        public string Guarantor { get { return guarantor; } }
        public string PayInterestDate { get { return payInterestDate; } }
        public int PayInterestMonth { get { return payInterestMonth; } }      
        public int Term { get { return term; } }
        public int YearDay { get { return yearDay; } }
        public int Status { get { return status; } }
        public string UserName { get { return userName; } }
        public string Remark { get { return remark; } }
        public static Debt_Apply_Single JsonStrToObject(string json)
        {
            return JsonConvert.DeserializeObject<Debt_Apply_Single>(json);
        }
    }

    class Base_Interest_Single
    {
        private int id;
        private int month;              //利率末区间（月）
        private double rate;            //利率 
        private string startTime;       //开始时间
        private int bondInstitutionId;  //债权单位id
        private int type;               //0:LPR 1:央行基准利率

        public int Id { get { return id; } }
        public int Month { get { return month; } }
        public double Rate { get { return rate; } }
        public string StartTime { get { return startTime; } }
        public int BondInstitutionId { get { return bondInstitutionId; } }
        public int Type { get { return type; } }
        public static Base_Interest_Single JsonStrToObject(string json)
        {
            return JsonConvert.DeserializeObject<Base_Interest_Single>(json);
        }
    }

    class Change_Log_Single
    {
        private string changeId;
        private string debtId;        //贷款编号
        private double baseInterest;  //自定义基准利率
        private double rateRatio;     //浮动利率
        private int debtUnitId;       //借款单位Id
        private string adjustType;    //利率调整方式(当日、次日、次月、次年、每年放款日)
        private string applyTime;     //变更录入时间
        private string changeDate;    //变更开始时间
        private int status;           //状态
        private string userName;      //操作用户
        private string remark;

        public string ChangeId { get { return changeId; } }
        public string DebtId { get { return debtId; } }
        public double BaseInterest { get { return baseInterest; } }
        public double RateRatio { get { return rateRatio; } }
        public int DebtUnitId { get { return debtUnitId; } }
        public string AdjustType { get { return adjustType; } }
        public string ApplyTime { get { return applyTime; } }
        public string ChangeDate { get { return changeDate; } }
        public int Status { get { return status; } }
        public string UserName { get { return userName; } }
        public string Remark { get { return remark; } }
        public static Change_Log_Single JsonStrToObject(string json)
        {
            return JsonConvert.DeserializeObject<Change_Log_Single>(json);
        }
    }

    class Pay_Plan_Single
    {
        private long id;
        private string supportId;  //贷款id或变更id	
        private string payDate;    //还款日期
        private double amount;     //还款金额

        public long Id { get { return id; } }
        public string SupportId { get { return supportId; } }
        public string PayDate { get { return payDate; } }
        public double Amount { get { return amount; } }
        public static Pay_Plan_Single JsonStrToObject(string json)
        {
            return JsonConvert.DeserializeObject<Pay_Plan_Single>(json);
        }
    }

    class Receive_Log_Single
    {
        private string receiveId;
        private double amount;       //提款金额
        private string debtId;       //贷款编号
        private string userName;     //操作员
        private string receiveDate;  //提款日期
        private string applyTime;    //录入日期
        private int status;          //状态
        private string remark;       //备注

        public string ReceiveId { get { return receiveId; } }
        public double Amount { get { return amount; } }
        public string DebtId { get { return debtId; } }
        public string UserName { get { return userName; } }
        public string ReceiveDate { get { return receiveDate; } }
        public string ApplyTime { get { return applyTime; } }
        public int Status { get { return status; } }
        public string Remark { get { return remark; } }
        public static Receive_Log_Single JsonStrToObject(string json)
        {
            return JsonConvert.DeserializeObject<Receive_Log_Single>(json);
        }
    }

    class Payment_Log_Single
    {
        private string paymentId;
        private string debtId;     //贷款编号
        private double amount;     //金额
        private string payDate;    //还款时间
        private string userName;   //操作员
        private string applyTime;  //录入时间
        private int type;          //类型(0：付息1：还本2：罚金)
        private int status;        //状态
        private string remark;     //备注

        public string PaymentId { get { return paymentId; } }
        public string DebtId { get { return debtId; } }
        public double Amount { get { return amount; } }
        public string PayDate { get { return payDate; } }
        public string UserName { get { return userName; } }
        public string ApplyTime { get { return applyTime; } }
        public int Type { get { return type; } }
        public int Status { get { return status; } }
        public string Remark { get { return remark; } }
        public static Payment_Log_Single JsonStrToObject(string json)
        {
            return JsonConvert.DeserializeObject<Payment_Log_Single>(json);
        }
    }
    


}
