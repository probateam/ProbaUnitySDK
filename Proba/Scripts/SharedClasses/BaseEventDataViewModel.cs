using System;

namespace Proba
{
    [Serializable]
    public class BaseEventDataViewModel
    {
        public string SessionHandle;
        public string UserId;

        public string Version;
        public string OS;
        public string Build;
        public string Class;
        public string Nonce;
        public string Device;
        public string SDKVersion;
        public string Manufacturer;
        public EPlatforms Platform;
        public bool ProbaGameCenter;
        public bool LogOnGooglePlay;
        public string Engine;
        public string ConnectionType;
        public string IOS_IDFA;
        public string Google_AID;
        public string Proba_GCID;
        public string Custom1;
        public string Custom2;
        public string Custom3;
        public string Custom4;
        public string Custom5;
        public long ClientTs;
        public double Battery;
        public bool Charging;

        public string UserName;
        public bool NewUser;
        public string UniqueId;

        internal BaseEventDataViewModel()
        {
            DeviceInfo.WriteBaseEventDataViewModel("Class", this);
        }
    }
}
