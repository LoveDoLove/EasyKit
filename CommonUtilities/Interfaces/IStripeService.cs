using Stripe;
using Stripe.Checkout;

namespace CommonUtilities.Interfaces;

public interface IStripeService
{
    Task<TaxRate?> CreateTaxRateAsync(TaxRateCreateOptions taxRateCreateOptions);
    Task<Customer?> CreateCustomerAsync(CustomerCreateOptions customerCreateOptions);
    Task<Session?> CreateCheckoutSessionAsync(SessionCreateOptions sessionCreateOptions);
    Task<Session?> GetCheckoutSessionAsync(string sessionId);
    Task<PaymentIntent?> GetPaymentIntentAsync(string paymentIntentId);
    Task<Charge?> GetChargeAsync(string chargeId);
    Task<Customer?> GetCustomerAsync(string customerId);
    Task<Refund?> RefundPaymentIntentAsync(string paymentIntentId, decimal amount);
}