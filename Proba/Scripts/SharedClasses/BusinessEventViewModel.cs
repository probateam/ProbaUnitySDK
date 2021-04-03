using System;

namespace Proba.Scripts.SharedClasses
{
    internal class BusinessEventViewModel : BaseEventDataViewModel
    {
        public BusinessTypes BusinessType; //* ->
        public double Value; //meghdar
        public string Currency; //IRR
        public string ItemName; //chizi ke kharide
        public int TransactionCount; // yeki az oon 14ta
        public string CartName; //kodom forshgah (akhare game, avale game , ...)
        public string ExtraDetails; //100 ch
        public PaymentTypes PaymentType; // ->
        public bool SpecialEvent; //
        public string SpecialEventName; //
        public double Amount; //meghdare pool ya tedad item
        public bool VirtualCurrency; //ingame kharj karde

        internal BusinessEventViewModel(BusinessTypes businessType, double value, string currency, string itemName, int transactionCount,
            string cartName, string extraDetails, PaymentTypes paymentType, bool specialEvent, string specialEventName, double amount, bool virtualCurrency)
        {
            BusinessType = businessType;
            Value = value;
            Currency = currency;
            ItemName = itemName;
            TransactionCount = transactionCount;
            CartName = cartName;
            ExtraDetails = extraDetails;
            PaymentType = paymentType;
            SpecialEvent = specialEvent;
            SpecialEventName = specialEventName;
            Amount = amount;
            VirtualCurrency = virtualCurrency;
            DeviceInfo.WriteBaseEventDataViewModel(this.GetType().Name, this);
            Broker.BusinessEventCreated(this);
        }

        internal BusinessEventViewModel()
        {

        }
    }
}
