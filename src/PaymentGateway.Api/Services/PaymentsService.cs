using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Controllers.Responses;
using PaymentGateway.Api.Models.HttpClients.Requests;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Services;

public class PaymentsService(IPaymentsRepository paymentsRepository, IAcquiringBankClient acquiringBankClient) : IPaymentsService
{
    public PaymentResponse GetPayment(Guid id) => paymentsRepository.Get(id);

    public async Task<PaymentResponse> ProcessPaymentAsync(PostPaymentRequest paymentRequest)
    {
        var processPaymentRequest = new ProcessPaymentRequest(paymentRequest.CardNumber,
            $"{paymentRequest.ExpiryMonth:D2}/{paymentRequest.ExpiryYear:D4}",
            paymentRequest.Currency, paymentRequest.Amount, paymentRequest.Cvv);
        
        var processPaymentResponse = await acquiringBankClient.ProcessPaymentAsync(processPaymentRequest);

        var paymentResponse = new PaymentResponse(
            string.IsNullOrEmpty(processPaymentResponse.AuthorizationCode)
                ? Guid.NewGuid()
                : Guid.Parse(processPaymentResponse.AuthorizationCode),
            processPaymentResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
            paymentRequest.CardNumber[^4..],
            paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear,
            paymentRequest.Currency, paymentRequest.Amount);
        
        paymentsRepository.Add(paymentResponse);

        return paymentResponse;
    }
}