using Microsoft.Extensions.Options;
using ShainingOpt.Models;
using ShainingOpt.Services.Configurations;
using ShainingOpt.Services.Interfaces;
using Yandex.Checkout.V3;

namespace ShainingOpt.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly Client _client;

        public PaymentService(IOptions<YooKassaSettings> options)
        {
            var settings = options.Value;

            _client = new Client(settings.ShopId, settings.SecretKey);

        }

        public Payment CreatePayment(decimal totalSum, string returnUrl, string description, int orderId)
        {
            var payment = new NewPayment
            {
                Amount = new Yandex.Checkout.V3.Amount { Value = totalSum, Currency = "RUB" },
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = returnUrl
                },
                Description = description,
                Capture = true,
                Metadata = new Dictionary<string, string>
                {
                    {"OrderId", orderId.ToString() }
                }

            };

            return _client.CreatePayment(payment);
        }


    }
}
