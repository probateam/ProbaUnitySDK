using System;

namespace Proba
{
    public class RemoteConfigurationsViewModel
    {
        public string ConfigKey;
        public string ConfigDescription;
        public string Value;
        public DateTime StartTime;
        public long StartTicks;
        public DateTime EndTime;
        public long EndTicks;

        #region OS Filter

        public string OS;
        public bool OS_Exclude;
        #endregion

        #region Country Filter

        public string Country;
        public bool Country_Exclude;
        #endregion

        #region Build Filter

        public string Build;
        public bool Build_Exclude;
        #endregion

        #region Class Filter

        public string TimeClass;
        public bool TimeClass_Exclude;
        #endregion

        public int Priority;
    }
}
