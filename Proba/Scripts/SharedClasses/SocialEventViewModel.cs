using System;

namespace Proba.Scripts.SharedClasses
{
    internal class SocialEventViewModel : BaseEventDataViewModel
    {
        public string socialMediaName;
        public SocialEvenTypes socialEvenType;
        public int value; //if rate

        internal SocialEventViewModel(string socialMediaName, SocialEvenTypes socialEvenType, int value)
        {
            this.socialMediaName = socialMediaName;
            this.socialEvenType = socialEvenType;
            this.value = value;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.SocialEventCreated(this);
        }

        internal SocialEventViewModel()
        {

        }
    }
}
