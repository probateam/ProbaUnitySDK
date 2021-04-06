using System;

namespace Proba.Scripts.SharedClasses
{
    internal class ProgressionEventViewModel : BaseEventDataViewModel
    {
        //
        public ProgressionTypes ProgressionType; 
        public int Attempts; 
        public double Score; 
        public string GameLevelName1; 
        public string GameLevelName2;
        public string GameLevelName3;
        public string GameLevelName4;
        public string ProgressionId; 
        public bool ArenaMode;
        public string ArenaName;
        public byte first = 0;

        internal ProgressionEventViewModel(ProgressionTypes progressionType, int attempts, double score, string gameLevelName1, string gameLevelName2,
            string gameLevelName3, string gameLevelName4, string progressionId, bool arenaMode, string arenaName)
        {
            ProgressionType = progressionType;
            Attempts = attempts;
            Score = score;
            GameLevelName1 = gameLevelName1;
            GameLevelName2 = gameLevelName2;
            GameLevelName3 = gameLevelName3;
            GameLevelName4 = gameLevelName4;
            ProgressionId = progressionId;
            ArenaMode = arenaMode;
            ArenaName = arenaName;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.ProgressionEventCreated(this);
        }

        internal ProgressionEventViewModel()
        {

        }
    }
}
