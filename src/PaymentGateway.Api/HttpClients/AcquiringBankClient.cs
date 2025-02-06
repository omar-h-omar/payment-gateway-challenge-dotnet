using PaymentGateway.Api.Models.HttpClients.Requests;
using PaymentGateway.Api.Models.HttpClients.Responses;

namespace PaymentGateway.Api.HttpClients;

public class AcquiringBankClient(HttpClient client) : IAcquiringBankClient
{
    public async Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        var response = await client.PostAsJsonAsync("payments", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProcessPaymentResponse>();
    }
}