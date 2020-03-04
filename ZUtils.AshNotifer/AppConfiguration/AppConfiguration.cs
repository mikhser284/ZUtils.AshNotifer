using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ZUtils.AshNotifer.AppConfiguration
{
    /// <summary> Режим работы приложения </summary>
    public enum ApplicationMode
    {
        /// <summary> Только прием сообщений </summary>
        OnlyReceive = 0,

        /// <summary> Прием и передача сообщений </summary>
        SendAndReceive = 1,
    }

    [JsonObject("AppConfiguration")]
    public class AppConfiguration
    {
        public const String AppName = "ZUtils.AshNotifer";
        public const String AppProfile = "Default";
        private static readonly String AppConfigFilePath_EnvVarName = $"AppConfigFilePath_{AppName.Replace('.', '_')}";
        private static readonly String AppConfigFilePath_EnvVarDefaultValue = $@"C:\_AppConfigs\{AppName}\{AppProfile}\{AppName} - Main.config";

        /// <summary> Путь к LDAP </summary>
        [JsonProperty("LdapPath", Order = 1)]
        public String LdapPath { get; private set; }

        /// <summary> Режим работы приложения </summary>
        public ApplicationMode AppMode { get; private set; }

        /// <summary> IP aдрес основного сервера (c портом) </summary>
        [JsonProperty("AddressOfMainServer")]
        public String AddressOfMainServer { get; private set; }

        /// <summary> № передающего порта </summary>
        [JsonProperty("PortOfTransmitter")]
        public String PortOfTransmitter { get; private set; }

        /// <summary> № принимающего порта </summary>
        [JsonProperty("PortOfReceiver")]
        public String PortOfReceiver { get; private set; }

    }
}
