using FluentValidation;

using PaymentGateway.Api.Models.Controllers.Requests;

namespace PaymentGateway.Api.Models.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    public PostPaymentRequestValidator()
    {
        RuleFor(x => x.CardNumber).NotEmpty().Length(14, 19).Must(cardNumber => cardNumber.All(char.IsDigit))
            .WithMessage("Card number must only contain numeric characters");
        RuleFor(x => x.ExpiryMonth).NotEmpty().InclusiveBetween(1, 12).DependentRules(() =>
        {
            RuleFor(x => x.ExpiryYear).NotEmpty()
                .Must((request, expiryYear) => new DateTime(expiryYear, request.ExpiryMonth, 1) > DateTime.Now)
                .WithMessage("Payment date must be in the future");
        });
        RuleFor(x => x.Currency).NotEmpty().Must(currency => currency is "GBP" or "USD" or "EUR");
        RuleFor(x => x.Amount).NotEmpty();
        RuleFor(x => x.Cvv).NotEmpty().Must(cvv => cvv.ToString().Length is 3 or 4)
            .WithMessage("Cvv must be 3 to 4 characters long");
    }
}