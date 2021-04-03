using System;

namespace Proba.Scripts.SharedClasses
{
    public class UserAchievementViewModel
    {
        public string AchievementName;
        public string AchievementId;
        public long Score;
        public int Step;
        public bool Deactive;
        public long AchievementDate;

        internal UserAchievementViewModel()
        {
        }
    }
}
