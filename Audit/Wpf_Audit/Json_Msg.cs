using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf_Audit
{
    class Json_Msg
    {
        public int error { get; set; }
        public string msg { get; set; }
        public static Json_Msg JsonStrToList(string json)
        {
            //反序列化
            Json_Msg model = new Json_Msg();
            model = JsonConvert.DeserializeObject<Json_Msg>(json);
            return model;
        }

        public static void ShowMsg(Json_Msg jmsg)
        {
            MessageBox.Show(jmsg.msg, "异常提醒", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class User_SelfInfo
    {
        public string userId { get; set; }
        public string department { get; set; }
        public int rank { get; set; }
        public int companyId { get; set; }
        public string companyName { get; set; }
        public string realName { get; set; }
        public string contactInfo { get; set; }
        public string token { get; set; }

        public User_SelfInfo() { }
        public User_SelfInfo(string userId, string department, int rank, int companyId, string companyName, string realName, string contactInfo, string token)
        {
            this.userId = userId;
            this.department = department;
            this.rank = rank;
            this.companyId = companyId;
            this.companyName = companyName;
            this.realName = realName;
            this.contactInfo = contactInfo;
            this.token = token;
        }
    }

    class BondInstitution
    {
        public int bondInstitutionId { get; set; }
        public string bondInstitutionName { get; set; }
        public string legalPersonName { get; set; }
        public string phoneNum { get; set; }
        public string address { get; set; }
        public string creditCode { get; set; }
        public int type { get; set; }
    }

    class Json_BondInstitution
    {
        public int error { get; set; }
        public List<BondInstitution> data { get; set; }
        public static Json_BondInstitution JsonStrToList(string json)
        {
            //反序列化
            Json_BondInstitution model = new Json_BondInstitution();
            model = JsonConvert.DeserializeObject<Json_BondInstitution>(json);
            return model;
        }
    }


    class Json_AuditPassItem
    {
        public int auditId { get; set; }
        public int auditRank { get; set; }
        public int isPassed { get; set; }
        public string remark { get; set; }
        public string supportId { get; set; }
        public string userId { get; set; }
        public DateTime auditDate { get; set; }
        public float amount { get; set; }
        public DateTime debtStartTime { get; set; }
        public DateTime debtEndTime { get; set; }
    }

    class Json_AuditPassInfo
    {
        public int error { get; set; }
        public List<Json_AuditPassItem> data { get; set; }
        public static Json_AuditPassInfo JsonStrToList(string json)
        {
            //反序列化
            Json_AuditPassInfo model = new Json_AuditPassInfo();
            model = JsonConvert.DeserializeObject<Json_AuditPassInfo>(json);
            return model;
        }
    }

    class Json_AuditApplyItem
    {
        public string debtId { get; set; }
        public float amount { get; set; }
        public string debtTypeName { get; set; }
        public string bondInstitutionName { get; set; }
        public string bankName { get; set; }
        public string termClassificationName { get; set; }
        public string creditUpdateName { get; set; }
        public int status { get; set; }
        public DateTime applyTime { get; set; }
        public DateTime debtStartTime { get; set; }
        public DateTime debtEndTime { get; set; }
        public float baseInterest { get; set; }
        public int operatorId { get; set; }
        public string remark { get; set; }
        public string isInGov { get; set; }
        public float rateRatio { get; set; }
        public string interestRateAdjustTypeName { get; set; }
        public string payTypeName { get; set; }
        public string debtUnitName { get; set; }
        public string realName { get; set; }
    }

    class Json_AuditApplyInfo
    {
        public int error { get; set; }
        public List<Json_AuditApplyItem> data { get; set; }
        public static Json_AuditApplyInfo JsonStrToList(string json)
        {
            //反序列化
            Json_AuditApplyInfo model = new Json_AuditApplyInfo();
            model = JsonConvert.DeserializeObject<Json_AuditApplyInfo>(json);
            return model;
        }
    }

    class Json_SingleAuditChangeInfo
    {
        public int error { get; set; }
        public string loanChangeId { get; set; }
        public string debtId { get; set; }
        public DateTime changeDate { get; set; }
        public string remark { get; set; }
        public int operatorId { get; set; }
        public float amount { get; set; }
        public string payTypeName { get; set; }
        public float changedInterestRate { get; set; }
        public int status { get; set; }
        public DateTime debtEndTime { get; set; }
        public DateTime applyTime { get; set; }
        public string adjustType { get; set; }
        public string realName { get; set; }

        public static Json_SingleAuditChangeInfo JsonStrToList(string json)
        {
            //反序列化
            Json_SingleAuditChangeInfo model = new Json_SingleAuditChangeInfo();
            model = JsonConvert.DeserializeObject<Json_SingleAuditChangeInfo>(json);
            return model;
        }
    }

    class Json_SingleAuditApplyInfo
    {
        public int error { get; set; }
        public string debtId { get; set; }
        public float amount { get; set; }
        public string debtTypeName { get; set; }
        public string bondInstitutionName { get; set; }
        public string bankName { get; set; }
        public string termClassificationName { get; set; }
        public string creditUpdateName { get; set; }
        public int status { get; set; }
        public DateTime applyTime { get; set; }
        public DateTime debtStartTime { get; set; }
        public DateTime debtEndTime { get; set; }
        public float baseInterest { get; set; }
        public int operatorId { get; set; }
        public string remark { get; set; }
        public string isInGov { get; set; }
        public float rateRatio { get; set; }
        public string interestRateAdjustTypeName { get; set; }
        public string payTypeName { get; set; }
        public string debtUnitName { get; set; }
        public string realName { get; set; }

        public static Json_SingleAuditApplyInfo JsonStrToList(string json)
        {
            //反序列化
            Json_SingleAuditApplyInfo model = new Json_SingleAuditApplyInfo();
            model = JsonConvert.DeserializeObject<Json_SingleAuditApplyInfo>(json);
            return model;
        }
    }

    class Json_File
    {
        public string id { get; set; }
        public string fileOldName { get; set; }
        public string fileSize { get; set; }
        public DateTime uploadTime { get; set; }
    }

    class Json_FilesView
    {
        public int error { get; set; }
        public List<Json_File> data { get; set; }

        public static Json_FilesView JsonStrToList(string json)
        {
            //反序列化
            Json_FilesView model = new Json_FilesView();
            model = JsonConvert.DeserializeObject<Json_FilesView>(json);
            return model;
        }
    }

    class Json_Download
    {
        public int error { get; set; }
        public string url { get; set; }

        public static Json_Download JsonStrToList(string json)
        {
            //反序列化
            Json_Download model = new Json_Download();
            model = JsonConvert.DeserializeObject<Json_Download>(json);
            return model;
        }
    }

    class Json_AuditChangeItem
    {
        public string loanChangeId { get; set; }
        public string debtId { get; set; }
        public DateTime changeDate { get; set; }
        public string remark { get; set; }
        public int operatorId { get; set; }
        public float amount { get; set; }
        public string payTypeName { get; set; }
        public float changedInterestRate { get; set; }
        public int status { get; set; }
        public DateTime debtStartTime { get; set; }
        public DateTime debtEndTime { get; set; }
        public DateTime applyTime { get; set; }
        public string adjustType { get; set; }
        public string realName { get; set; }
    }

    class Json_AuditChangeInfo
    {
        public int error { get; set; }
        public List<Json_AuditChangeItem > data { get; set; }
        public static Json_AuditChangeInfo JsonStrToList(string json)
        {
            //反序列化
            Json_AuditChangeInfo model = new Json_AuditChangeInfo();
            model = JsonConvert.DeserializeObject<Json_AuditChangeInfo>(json);
            return model;
        }
    }

    class Json_Operation
    {
        public string row_number { get; set; }
        public int Id { get; set; }
        public string operation { get; set; }
        public string userId { get; set; }
        public DateTime operationDate { get; set; }
        public string mac { get; set; }
        public string ip { get; set; }
        public int type { get; set; }
        public string realName { get; set; }
    }

    class Json_OperationLog
    {
        public int error { get; set; }
        public string page { get; set; }
        public string totalPage { get; set; }
        public string rows { get; set; }
        public List<Json_Operation> data { get; set; }
        public static Json_OperationLog JsonStrToList(string json)
        {
            //反序列化
            Json_OperationLog model = new Json_OperationLog();
            model = JsonConvert.DeserializeObject<Json_OperationLog>(json);
            return model;
        }
    }

}
