using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Controllers.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsService
{
    PaymentResponse GetPayment(Guid id);
    Task<PaymentResponse> ProcessPaymentAsync(PostPaymentRequest paymentRequest);
}