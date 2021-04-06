using System;

namespace Proba.Scripts.SharedClasses
{
    public class LeaderBoardUserViewModel
    {
        public string LeaderBoardId;
        public long Score;
        public string UserName;
        public bool Deactive;
        public long LastUpdate;
        public int Rank;
        public bool Self;

        internal LeaderBoardUserViewModel()
        {

        }
    }
}