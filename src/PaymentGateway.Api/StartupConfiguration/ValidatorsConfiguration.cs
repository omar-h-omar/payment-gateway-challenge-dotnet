using FluentValidation;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Validators;

namespace PaymentGateway.Api.StartupConfiguration;

public static class ValidatorsConfiguration
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>();
    }
}