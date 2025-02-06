using PaymentGateway.Api.Models.HttpClients.Requests;
using PaymentGateway.Api.Models.HttpClients.Responses;

namespace PaymentGateway.Api.HttpClients;

public interface IAcquiringBankClient
{
    Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request);
}