using Yandex.Checkout.V3;

namespace ShainingOpt.ViewModels
{
    public class YooKassaNotification
    {
        public string Event { get; set; }

        public PaymentObject Object { get; set; }
    }
    public class PaymentObject
    {
        public string Id { get; set; }

        public string Status { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public Amount Amount { get; set; }
    }
    public class Amount
    {
        public string Value { get; set; }
        public string Currency { get; set; }
    }
}
