using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewWPF
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
        public string userName { get; set; }
        public string department { get; set; }
        public int rank { get; set; }
        public int companyId { get; set; }
        public string companyName { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string auditRank { get; set; }

        public string token { get; set; }

        public User_SelfInfo() { }
        public User_SelfInfo(string department, int rank, int companyId, string companyName, string name, string phone, string auditRank, string token)
        {
            //this.userId = userId;
            this.department = department;
            this.rank = rank;
            this.companyId = companyId;
            this.companyName = companyName;
            this.name = name;
            this.phone = phone;
            this.auditRank = auditRank;
            this.token = token;
        }
    }

    class Json_Login
    {
        public int error { get; set; }
        public string userId { get; set; }
        public string department { get; set; }
        public int rank { get; set; }
        public int companyId { get; set; }
        public string companyName { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string auditRank { get; set; }

        public string token { get; set; }
        public static Json_Login JsonStrToList(string json)
        {
            //反序列化
            Json_Login model = new Json_Login();
            model = JsonConvert.DeserializeObject<Json_Login>(json);
            return model;
        }

    }

    class Json_Information
    {
        public int error { get; set; }

        public List<Json_debtTypes_Item> debtTypes { get; set; }
        public List<Json_creditTypes_Item> creditTypes { get; set; }
        public List<Json_bondInstitutions_Item> bondInstitutions { get; set; }
        public static Json_Information JsonStrToList(string json)
        {
            //反序列化
            Json_Information model = new Json_Information();
            model = JsonConvert.DeserializeObject<Json_Information>(json);
            return model;
        }

    }

    class Json_debtTypes_Item
    {
        public string debtType { get; set; }
    }

    class Json_creditTypes_Item
    {
        public string value { get; set; }
    }

    class Json_bondInstitutions_Item
    {
        public int id { get; set; }
        public string value { get; set; }
    }

    class DebtList_Item
    {
        public string debtId { get; set; }
        public int amount { get; set; }
        public int bondInsitutionId { get; set; }
        public int debtUnitId { get; set; }
        public string debtTypeName { get; set; }
        public int relatedBank { get; set; }
        public string creditUpdateName { get; set; }
        public string applyTime { get; set; }
        public string debtStartTime { get; set; }
        public string debtEndTime { get; set; }
        public int baseInterestId { get; set; }
        public string baseInterest { get; set; }
        public string isInGov { get; set; }
        public string rateRatio { get; set; }
        public string rateAdjustType { get; set; }
        public string guarantor { get; set; }
        public string payInterestDate { get; set; }
        public string payInterestMonth { get; set; }
        public string term { get; set; }
        public string yearDay { get; set; }
        public string status { get; set; }
        public string userName { get; set; }
        public string remark { get; set; }
        public string name { get; set; }
    }

    class DebtList_Msg
    {
        public int error { get; set; }
        public List<DebtList_Item> data { get; set; }

        public static DebtList_Msg JsonStrToList(string json)
        {
            //反序列化
            DebtList_Msg model = new DebtList_Msg();
            model = JsonConvert.DeserializeObject<DebtList_Msg>(json);
            return model;
        }
    }
}