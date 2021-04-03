using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Proba.Scripts.Configuration
{
    internal static class ConfigurationModel
    {
        public static readonly string BaseURL = "https://api.proba.ir";
        public static readonly string ServerIPAddress = "api.proba.ir";
        public static string ConfigPath = "Assets/ProbaConfig.asset";
        public static readonly string CurrentAPIVersion = "v1";
        public static readonly List<string> CompatibleAPIVersions = new List<string>(new List<string> { "v0", "v1" });
    }
}
