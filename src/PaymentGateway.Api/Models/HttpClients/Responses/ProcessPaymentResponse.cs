using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.HttpClients.Responses;

public class ProcessPaymentResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}