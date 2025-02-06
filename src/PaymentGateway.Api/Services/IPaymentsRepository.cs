using PaymentGateway.Api.Models.Controllers.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    void Add(PaymentResponse payment);
    PaymentResponse Get(Guid id);
}