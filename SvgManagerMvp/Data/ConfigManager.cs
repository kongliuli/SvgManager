using System.Configuration;

namespace SvgManagerMvp.Data
{
    public class ConfigManager
    {
        public static string GetConnectionString()
        {
            // 从配置文件读取连接字符串
            var config = ConfigurationManager.ConnectionStrings["DamengConnection"];
            return config?.ConnectionString ?? "";
        }

        public static void SaveConnectionString(string connectionString)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = configFile.ConnectionStrings;
            
            if (connectionStringsSection.ConnectionStrings["DamengConnection"] != null)
            {
                connectionStringsSection.ConnectionStrings["DamengConnection"].ConnectionString = connectionString;
            }
            else
            {
                connectionStringsSection.ConnectionStrings.Add(
                    new ConnectionStringSettings("DamengConnection", connectionString, "DmProvider"));
            }
            
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}