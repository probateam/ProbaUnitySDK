using System;

namespace Proba
{
    internal class TrophyRequest
    {
        public string Nonce;
        public string UserId;
        public string LeaderBoardId;
        public long TrophyScore;
        public string UserName;
        public string AchievementId;
        public int AchievementStep;

        internal TrophyRequest()
        {
            Nonce = Guid.NewGuid().ToString();
            UserId = Guid.Empty.ToString();
            LeaderBoardId = Guid.Empty.ToString();
            AchievementId = Guid.Empty.ToString();
        }
    }
}
