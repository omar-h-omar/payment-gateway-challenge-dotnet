using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.HttpClients.Requests;

public class ProcessPaymentRequest(string cardNumber, string expiryDate, int amount, string currency, string cvv)
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; } = cardNumber;

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; } = expiryDate;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = currency;

    [JsonPropertyName("amount")]
    public int Amount { get; set; } = amount;

    [JsonPropertyName("cvv")]
    public string Cvv { get; set; } = cvv;
}