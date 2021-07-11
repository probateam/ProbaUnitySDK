using System;

namespace Proba
{
    internal class BusinessEventViewModel : BaseEventDataViewModel
    {
        public BusinessTypes BusinessType; 
        public double Value; 
        public string Currency; 
        public string ItemName; 
        public int TransactionCount; 
        public string CartName; 
        public string ExtraDetails; 
        public PaymentTypes PaymentType; 
        public bool SpecialEvent; 
        public string SpecialEventName; 
        public double Amount; 
        public bool VirtualCurrency; 

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
