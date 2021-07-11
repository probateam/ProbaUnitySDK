using System;
using UnityEngine;

namespace Proba
{
    internal class StartSessionViewModel : BaseEventDataViewModel
    {
        public long SessionCount;
        public long FirstSessionTime;
        public UserType UserType;

        internal StartSessionViewModel()
        {
            
            try
            {
                FirstSessionTime = Convert.ToInt64(PlayerPrefs.GetString("ProbaFirstSessionTime"));
            }
            catch (FormatException)
            {
                FirstSessionTime = System.DateTime.UtcNow.Ticks;
                PlayerPrefs.SetString("ProbaFirstSessionTime", FirstSessionTime.ToString());
            }

            var currentTime = DateTime.UtcNow.Ticks;
            var difference = new TimeSpan(currentTime - FirstSessionTime);
            if (difference.Days == 0)
            {
                UserType = UserType.New;
            }
            else if (difference.Days > 0)
            {
                UserType = UserType.Returning;
            }
            else
            {
                UserType = UserType.None;
            }

            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
        }

        internal StartSessionViewModel(bool callEvent)
        {
            try
            {
                SessionCount = Convert.ToInt64(PlayerPrefs.GetString("ProbaSessionCount"));
                PlayerPrefs.SetString("ProbaSessionCount", (SessionCount + 1).ToString());
            }
            catch (FormatException)
            {
                SessionCount = 1;
                PlayerPrefs.SetString("ProbaSessionCount", (SessionCount + 1).ToString());
            }

            try
            {
                FirstSessionTime = Convert.ToInt64(PlayerPrefs.GetString("ProbaFirstSessionTime"));
            }
            catch (FormatException)
            {
                FirstSessionTime = System.DateTime.UtcNow.Ticks;
                PlayerPrefs.SetString("ProbaFirstSessionTime", FirstSessionTime.ToString());
            }

            var currentTime = DateTime.UtcNow.Ticks;
            var difference = new TimeSpan(currentTime - FirstSessionTime);
            if (difference.Days == 0)
            {
                UserType = UserType.New;
            }
            else if (difference.Days > 0)
            {
                UserType = UserType.Returning;
            }
            else
            {
                UserType = UserType.None;
            }

            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            if (callEvent)
            {
                Broker.StartSessionCreated(this);
            }
        }
    }
}
