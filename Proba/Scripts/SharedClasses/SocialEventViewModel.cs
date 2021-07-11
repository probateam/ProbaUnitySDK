namespace Proba
{
    internal class SocialEventViewModel : BaseEventDataViewModel
    {
        public string socialMediaName;
        public SocialEvenTypes socialEvenType;
        public int value; 

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
