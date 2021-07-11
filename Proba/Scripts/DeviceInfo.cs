using System;
using UnityEngine;

namespace Proba
{
    public static class DeviceInfo
    {
        public static string SDKVersion => ConfigurationModel.CurrentAPIVersion;
        public static string OSInfo => Application.platform.ToString();
        public static string DeviceName => SystemInfo.deviceName;
        public static string ApplicationBuild => Application.version;
        public static string ApplicationVersion => Application.version;
        public static string Manufacturer => SystemInfo.deviceModel;
        public static string EngineName => Application.unityVersion;
        public static string ConnectionType => Application.internetReachability.ToString();
        public static string DeviceUniqueId => SystemInfo.deviceUniqueIdentifier;
#if UNITY_ANDROID
        public static EPlatforms Platform => EPlatforms.Android;
#elif UNITY_IOS
        public static EPlatforms Platform => EPlatforms.IOS;
#else
        public static EPlatforms Platform => EPlatforms.None ;
#endif

        internal static void WriteBaseEventDataViewModel<T>(string Class, T eventData) where T : BaseEventDataViewModel
        {
            eventData.SDKVersion = ConfigurationModel.CurrentAPIVersion;
            eventData.OS = OSInfo;
            eventData.Battery = SystemInfo.batteryLevel;
            if (eventData.Battery < 0)
                eventData.Battery = 0;
            eventData.Charging = SystemInfo.batteryStatus == BatteryStatus.Charging;
            eventData.ClientTs = DateTime.UtcNow.Ticks;
            eventData.ConnectionType = ConnectionType;
            eventData.Device = DeviceName;
            eventData.Engine = EngineName;
            eventData.Manufacturer = Manufacturer;
            eventData.Nonce = Guid.NewGuid().ToString();
            eventData.Platform = Platform;
            eventData.Version = ApplicationVersion;
            eventData.Build = ApplicationBuild;
            eventData.Google_AID = string.Empty;
            eventData.IOS_IDFA = string.Empty;
            eventData.LogOnGooglePlay = default;
            eventData.ProbaGameCenter = default;
            eventData.Proba_GCID = string.Empty;

            eventData.UserId = !string.IsNullOrEmpty(PlayerPrefs.GetString("ProbaUserID")) ? PlayerPrefs.GetString("ProbaUserID") : Guid.Empty.ToString();
            eventData.SessionHandle = !string.IsNullOrEmpty(PlayerPrefs.GetString("ProbaSessionID")) ? PlayerPrefs.GetString("ProbaSessionID") : Guid.Empty.ToString();
            eventData.UserName = !string.IsNullOrEmpty(PlayerPrefs.GetString("ProbaUserName")) ? PlayerPrefs.GetString("ProbaUserName") : "";
            eventData.Class = Class;
            eventData.UniqueId = DeviceUniqueId;
        }
    }
}