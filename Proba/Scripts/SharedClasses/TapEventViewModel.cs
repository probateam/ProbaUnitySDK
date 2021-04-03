using System;

namespace Proba.Scripts.SharedClasses
{
    internal class TapEventViewModel : BaseEventDataViewModel
    {
        public TapTypes TapType;
        public string BtnName;
        public double StartX, StartY, EndX, EndY;

        internal TapEventViewModel(TapTypes tapType, string btnName, double startX, double startY, double endX, double endY)
        {
            BtnName = btnName;
            TapType = tapType;
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.TapEventViewCreated(this);
        }

        internal TapEventViewModel()
        {

        }
    }
}
