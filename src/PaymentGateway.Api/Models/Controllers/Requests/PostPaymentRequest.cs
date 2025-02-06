namespace PaymentGateway.Api.Models.Controllers.Requests;

public record PostPaymentRequest(
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string Cvv);