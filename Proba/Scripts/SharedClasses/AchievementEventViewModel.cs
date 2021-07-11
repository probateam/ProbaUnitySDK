using System;
using System.Collections.Generic;

namespace Proba
{
    internal class AchievementEventViewModel : BaseEventDataViewModel
    {
        public AchievementTypes AchievementType; 
        public string GameLevelName1; 
        public string GameLevelName2;
        public string GameLevelName3;
        public string GameLevelName4;
        public List<string> RelatedProgressionEventIds; 
        public long NewPlayerLevel;
        public long PrevRank;
        public long NewRank;
        public string LeaderBoardName;
        public bool ArenaMode;
        public string ArenaName;

        internal AchievementEventViewModel(AchievementTypes achievementType, string gameLevelName1, string gameLevelName2,
            string gameLevelName3, string gameLevelName4, List<string> relatedProgressionEventIds, long newPlayerLevel, long prevRank,
            long newRank, string leaderBoardName, bool arenaMode, string arenaName)
        {
            AchievementType = achievementType;
            GameLevelName1 = gameLevelName1;
            GameLevelName2 = gameLevelName2;
            GameLevelName3 = gameLevelName3;
            GameLevelName4 = gameLevelName4;
            RelatedProgressionEventIds = relatedProgressionEventIds;
            NewPlayerLevel = newPlayerLevel;
            PrevRank = prevRank;
            NewRank = newRank;
            LeaderBoardName = leaderBoardName;
            ArenaMode = arenaMode;
            ArenaName = arenaName;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.AchievementEventCreated(this);
        }

        internal AchievementEventViewModel()
        {

        }
    }
}
