using System;

namespace Proba.Scripts.Client
{
    internal class BasicData
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CurrentSessionId { get; set; }
        //public string OldSessionId { get; set; }
        public long SessionCount { get; set; }
        public long PurchesesCount { get; set; }
        public long VirtualPurchesesCount { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime FirstSessionStartTime { get; set; }
        public DateTime CurrentSessionStartTime { get; set; }
        //public DateTime OldSessionStartTime { get; set; }
        public long OverallPlayTime { get; set; }
        public bool HasActiveSession { get; set; }
        public string CurrentSessionLocation { get; set; }
        //public string OldSessionLocation { get; set; }
        public string GameCenterUserName { get; set; }
        public string CurrentUserName { get; set; }
        public bool HasError { get; set; }
        public SessionData SessionData { set; get; }
        public string ABTest { set; get; }

        internal BasicData()
        {

        }
    }
}
