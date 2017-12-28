using System.Configuration;


namespace RMQ.Utility
{
    public static class AppSettingConfig
    {
        public static string getAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }
        public static int getAppSettings(string key, string defaultValue)
        {
            return int.Parse(ConfigurationManager.AppSettings[key] ?? defaultValue);
        }
        public static ushort getAppSettingsUshort(string key, string defaultValue)
        {
            return ushort.Parse(ConfigurationManager.AppSettings[key] ?? defaultValue);
        }
    }
}
