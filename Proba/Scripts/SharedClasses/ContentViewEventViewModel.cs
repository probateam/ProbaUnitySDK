using System;

namespace Proba.Scripts.SharedClasses
{
    internal class ContentViewEventViewModel : BaseEventDataViewModel
    {
        public string ContentName; //esme scene
        //age hame bashe 
        //age na method seda mikone

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
