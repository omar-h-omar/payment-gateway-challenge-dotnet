using FluentValidation;

using PaymentGateway.Api.Models.Controllers.Requests;

namespace PaymentGateway.Api.Models.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    public PostPaymentRequestValidator()
    {
        RuleFor(x => x.CardNumber).NotEmpty().Length(14, 19).Must(cardNumber => cardNumber?.All(char.IsDigit) ?? false)
            .WithMessage("Card number must only contain numeric characters");
        RuleFor(x => x.ExpiryMonth).NotEmpty().InclusiveBetween(1, 12).DependentRules(() =>
        {
            RuleFor(x => x.ExpiryYear).NotEmpty()
                .Must((request, expiryYear) => new DateTime(expiryYear, request.ExpiryMonth, 1) > DateTime.Now)
                .WithMessage("Payment date must be in the future");
        });
        RuleFor(x => x.Currency).NotEmpty().Must(currency => currency is "GBP" or "USD" or "EUR")
            .WithMessage("Currency must be GBP, USD or EUR");
        RuleFor(x => x.Amount).NotEmpty();
        RuleFor(x => x.Cvv).NotEmpty().Length(3, 4).Must(cvv => cvv?.All(char.IsDigit) ?? false)
            .WithMessage("Cvv must only contain numeric characters");
    }
}