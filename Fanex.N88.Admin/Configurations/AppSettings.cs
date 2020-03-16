namespace Fanex.N88.Admin.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        [ThreadStatic]
        private static NameValueCollection appSettingValues;

        internal static NameValueCollection AppSettingValues
        {
            get { return appSettingValues ?? ConfigurationManager.AppSettings; }

            set { appSettingValues = value; }
        }

        public static string GetConfigValue(string configName) => AppSettingValues[configName];
    }
}