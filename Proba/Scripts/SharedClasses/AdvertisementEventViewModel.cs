namespace Proba
{
    internal class AdvertisementEventViewModel : BaseEventDataViewModel
    {
        public string AddId;
        public bool Skipped;
        public AdActions Action;
        public bool FirstTime;
        public AdFailShowReasons FailShowReason;
        public int Duration;
        public string SDKName;
        public AdTypes Type;
        public string Placement;
        public double Amount;

        internal AdvertisementEventViewModel(string addId, bool skipped, AdActions action, bool firstTime,
            AdFailShowReasons failShowReason, int duration, string sdkName, AdTypes type, string placement, double amount)
        {
            AddId = addId;
            Skipped = skipped;
            Action = action;
            FirstTime = firstTime;
            FailShowReason = failShowReason;
            Duration = duration;
            SDKName = sdkName;
            Type = type;
            Placement = placement;
            Amount = amount;

            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.AdvertisementEventCreated(this);
        }

        internal AdvertisementEventViewModel()
        {

        }
    }
}
