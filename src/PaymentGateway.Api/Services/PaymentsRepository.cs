using PaymentGateway.Api.Models.Controllers.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    public List<PaymentResponse> Payments = new();
    
    public void Add(PaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PaymentResponse Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}