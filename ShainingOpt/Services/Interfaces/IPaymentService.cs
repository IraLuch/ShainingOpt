using Yandex.Checkout.V3;

namespace ShainingOpt.Services.Interfaces
{
    public interface IPaymentService
    {
        Payment CreatePayment(decimal totalSum, string returnUrl, string description, int orderId);
    }
}