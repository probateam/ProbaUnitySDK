using System;
using UnityEngine;

namespace Proba.Scripts.SharedClasses
{
    [Serializable]
    internal class BaseEventDataViewModel
    {
        //session id az server
        public string SessionHandle;
        //az server static in db
        public string UserId;

        public string Version;
        public string OS;
        public string Build;
        public string Class; //felan nist
        public string Nonce; //string random guid, c# rng
        public string Device;
        public string SDKVersion;
        public string Manufacturer; //dorostesh kon
        public EPlatforms Platform; //unity
        public bool ProbaGameCenter; //pas inam nist
        public bool LogOnGooglePlay; //felan na
        public string Engine; //
        public string ConnectionType;
        public string IOS_IDFA;
        public string Google_AID;
        public string Proba_GCID;
        public string Custom1; //max 15 ch
        public string Custom2;
        public string Custom3;
        public string Custom4;
        public string Custom5;
        public long ClientTs;
        public double Battery; //0-100
        public bool Charging;

        public string UserName; //register 
        public bool NewUser; //register
        public string UniqueId; //register IMEI

        internal BaseEventDataViewModel()
        {
            DeviceInfo.WriteBaseEventDataViewModel("Class", this);
        }
    }
}
