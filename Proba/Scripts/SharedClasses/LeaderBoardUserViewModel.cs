using System;

namespace Proba.Scripts.SharedClasses
{
    public class LeaderBoardUserViewModel
    {
        public string leaderBoardId;
        public long score;
        public string userName;
        public bool deactive;
        public long lastUpdate;
        public int rank;
        public bool self;

        internal LeaderBoardUserViewModel()
        {

        }
    }
}