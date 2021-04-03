using System;
using System.Collections.Generic;
using UnityEngine;

namespace Proba.Scripts.SharedClasses
{
    internal class EndSessionViewModel
    {
        public string UserId; //server
        public string SessionId; //server

        public long ClientTs; //end time
        public long SessionLength; //daghightar age pause shod

        public string OS; //
        public string Location; //server

        public bool Error;

        public string ErrorData;
        public List<LogModel> Logs { get; set; }

        public double Battery; //end session
        //session flag

        internal EndSessionViewModel(string userID, string sessionId)
        {
            UserId = userID;
            SessionId = sessionId;
            ClientTs = DateTime.UtcNow.Ticks;
            SessionLength = (long)Time.time;
            OS = SystemInfo.operatingSystem;
            Battery = SystemInfo.batteryLevel;
            if (Battery < 0)
                Battery = 0;
            Broker.EndSessionCreated(this);
        }

        internal EndSessionViewModel()
        {
            ClientTs = DateTime.UtcNow.Ticks;
            SessionLength = (long)Time.time;
            OS = SystemInfo.operatingSystem;
            Battery = SystemInfo.batteryLevel;
            if (Battery < 0)
                Battery = 0;
            //Broker.EndSessionCreated(this);
        }
    }
}
