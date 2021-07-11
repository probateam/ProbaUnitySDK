using System.Collections.Generic;

namespace Proba
{
    public static class ConfigurationModel
    {
        public static readonly string BaseURL = "https://api.proba.ir";
        public static readonly string ServerIPAddress = "api.proba.ir";
        public static string ConfigPath = "Assets/Resources/ProbaConfig.asset";
        public static readonly string CurrentAPIVersion = "v1";
        public static readonly List<string> CompatibleAPIVersions = new List<string>(new List<string> { "v0", "v1" });
    }
}
