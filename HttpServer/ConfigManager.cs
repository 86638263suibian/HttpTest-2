using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpServer
{
    internal static class ConfigManager
    {

        internal static string Host { get; set; }

        const string _ConfigFileName = "HttpServer.exe.config";

        const string _HostElementName = "Host";

        internal static void Refresh()
        {
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + _ConfigFileName
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            var settings = config.AppSettings.Settings;
       
            var setting = settings[_HostElementName];
            if (setting != null && !string.IsNullOrWhiteSpace(setting.Value))
            {
                Host = setting.Value;
            }
        }

        internal static void Save()
        {
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + _ConfigFileName
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            var settings = config.AppSettings.Settings;
            var setting = settings[_HostElementName];

            if (setting != null)
            {
                setting.Value = Host;
            }
            else
            {
                settings.Add(_HostElementName, Host);
            }

            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
