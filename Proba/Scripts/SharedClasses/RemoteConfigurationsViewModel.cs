using System;

namespace Proba.Scripts.SharedClasses
{
    public class RemoteConfigurationsViewModel
    {
        public string configKey;
        public string configDescription;
        public string value;
        public DateTime startTime;
        public long startTicks;
        public DateTime endTime;
        public long endTicks;

        #region OS Filter

        public string os;
        public bool os_Exclude;
        #endregion

        #region Country Filter

        public string country;
        public bool country_Exclude;
        #endregion

        #region Build Filter

        public string build;
        public bool build_Exclude;
        #endregion

        #region Class Filter

        public string timeClass;
        public bool timeClass_Exclude;
        #endregion

        public int priority;
        public bool active;
    }
}
