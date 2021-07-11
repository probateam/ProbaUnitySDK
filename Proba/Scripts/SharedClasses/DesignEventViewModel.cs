using System.Collections.Generic;

namespace Proba
{
    internal class DesignEventViewModel : BaseEventDataViewModel
    {
        public Dictionary<string, string> CustomDesigns;

        internal DesignEventViewModel(Dictionary<string, string> customDesigns)
        {
            CustomDesigns = customDesigns;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.DesignEventCreated(this);
        }

        internal DesignEventViewModel()
        {

        }
    }
}
