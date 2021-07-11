using System;

namespace Proba
{
    internal class ContentViewEventViewModel : BaseEventDataViewModel
    {
        public string ContentName;

        internal ContentViewEventViewModel(string contentName)
        {
            ContentName = contentName;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.ContentViewEventCreated(this);
        }

        internal ContentViewEventViewModel()
        {

        }
    }
}
