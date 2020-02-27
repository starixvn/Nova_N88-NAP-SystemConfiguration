namespace Fanex.NAP.Admin.Configurations
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

        public static string VueJSEnvironment => GetConfigValue("VueJSEnvironment") ?? "PRO";

        public static int OWPSiteID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetConfigValue("OWPSiteId"));
                }
                catch
                {
                    throw new InvalidOperationException($"Missing config for OWPSiteID");
                }
            }
        }

        public static string GetConfigValue(string configName) => AppSettingValues[configName];
    }
}