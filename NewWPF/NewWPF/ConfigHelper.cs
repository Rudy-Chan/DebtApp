using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.RegularExpressions;

namespace NewWPF
{
    public static class ConfigHelper
    {
        ///<summary> 
        ///返回*.exe.config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="settingName"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string settingName)
        {
            try
            {
                string settingString = null;
                if (ConfigurationManager.AppSettings.AllKeys.Contains(settingName))
                    settingString = ConfigurationManager.AppSettings[settingName].ToString();
                return settingString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有配置数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllConfig()
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (string key in ConfigurationManager.AppSettings.AllKeys)
                    dict.Add(key, ConfigurationManager.AppSettings[key]);
                return dict;
            }
            catch
            {
                return null;
            }
        }

        ///<summary>  
        ///在*.exe.config文件中appSettings配置节增加一对键值对  
        ///</summary>  
        ///<param name="settingName"></param>  
        ///<param name="settingValue"></param>  
        public static void UpdateAppConfig(string settingName, string settingValue)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (!config.AppSettings.Settings.AllKeys.Contains(settingName))
                {
                    config.AppSettings.Settings.Add(settingName, settingValue);
                }
                else
                {
                    config.AppSettings.Settings[settingName].Value = settingValue;
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }

        ///<summary>  
        ///在*.exe.config文件中appSettings配置节删除一对键值对  
        ///</summary>  
        ///<param name="settingName"></param>  
        public static void DeleteAppConfig(string settingName)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config.AppSettings.Settings.AllKeys.Contains(settingName))
                {
                    config.AppSettings.Settings.Remove(settingName);
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }

        // 修改system.serviceModel下所有服务终结点的IP地址
        public static void UpdateIPConfig(string settingName, string serverIP)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                string pattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
                string replacement = string.Format("{0}", serverIP);
                serverIP = Regex.Replace(serverIP, pattern, replacement);

                if (!config.AppSettings.Settings.AllKeys.Contains(settingName))
                {
                    config.AppSettings.Settings.Add(settingName, serverIP);
                }
                else
                {
                    config.AppSettings.Settings[settingName].Value = serverIP;
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }


    }
}
