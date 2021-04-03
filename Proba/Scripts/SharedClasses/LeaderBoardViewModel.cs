using Newtonsoft.Json;
using UnityEditor;

namespace Proba.Scripts.SharedClasses
{
    public class LeaderBoardViewModel
    {
        public string LeaderBoardName;
        public string LeaderBoardDescription;
        public bool Descending;
        public ELeaderBoardUnitTypes LeaderBoardUnitType;
        public string DefaultUnit;
        public bool TamperProtection;
        public long LimitHigh;
        public long LimitLow;
        public int Order;
        public string Icon;
        public bool Deactive;
        public string LeaderBoardEnName;
        public long UserNumber;
        public string ID;
    }
}
