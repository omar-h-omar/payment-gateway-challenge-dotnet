using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.HttpClients.Responses;

public record ProcessPaymentResponse(
    bool Authorized,
    [property: JsonPropertyName("authorization_code")]
    string AuthorizationCode);