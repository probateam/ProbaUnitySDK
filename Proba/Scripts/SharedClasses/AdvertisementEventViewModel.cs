using ProbaDotnetSDK.SharedEnums;

namespace Proba.Scripts.SharedClasses
{
    internal class AdvertisementEventViewModel : BaseEventDataViewModel
    {
        public string AddId; //*
        public bool Skipped; //
        public AdActions Action; //*
        public bool FirstTime; //khob
        public AdFailShowReasons FailShowReason; //*
        public int Duration; //
        public string SDKName; //
        public AdTypes Type; //
        public string Placement; //koja dide
        public double amount;

        internal AdvertisementEventViewModel(string addId, bool skipped, bool clicked)
        {
            AddId = addId;
            Skipped = skipped;
            //Clicked = clicked;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.AdvertisementEventCreated(this);
        }

        internal AdvertisementEventViewModel()
        {

        }
    }
}
