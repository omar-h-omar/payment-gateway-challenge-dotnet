using NSubstitute;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Controllers.Responses;
using PaymentGateway.Api.Models.HttpClients.Requests;
using PaymentGateway.Api.Models.HttpClients.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

public class PaymentsServiceTests
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IAcquiringBankClient _acquiringBankClient;
    private readonly IPaymentsService _service;
    private readonly Random _random = new();
    
    public PaymentsServiceTests()
    {
        _paymentsRepository = Substitute.For<IPaymentsRepository>();
        _acquiringBankClient = Substitute.For<IAcquiringBankClient>();
        _service = new PaymentsService(_paymentsRepository, _acquiringBankClient);
    }
    
    [Fact]
    public void GetPayment_CallsGetAndReturnsPayment()
    {
        // Arrange
        var id = Guid.NewGuid();
        var payment = new PaymentResponse(id, PaymentStatus.Authorized, _random.Next(1111, 9999).ToString(),
            _random.Next(1, 12), _random.Next(2026, 2030), "GBP", _random.Next(1, 1000));
        _paymentsRepository.Get(Arg.Any<Guid>()).Returns(payment);
        
        // Act
        var result = _service.GetPayment(id);
        
        // Assert
        _paymentsRepository.Received(1).Get(Arg.Is(id));
        Assert.Equal(payment, result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_CallsProcessPaymentAsyncAndReturnsPayment()
    {
        // Arrange
        var cardNumber = "2222405343248877";
        var request = new PostPaymentRequest(cardNumber, _random.Next(1, 12),
            _random.Next(2026, 2030), "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());

        _acquiringBankClient.ProcessPaymentAsync(Arg.Any<ProcessPaymentRequest>())
            .Returns(new ProcessPaymentResponse(true, Guid.NewGuid().ToString()));

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        await _acquiringBankClient.Received(1)
            .ProcessPaymentAsync(Arg.Is<ProcessPaymentRequest>(r => r.CardNumber == cardNumber));
        Assert.Equal(request.CardNumber[^4..], result.CardNumberLastFour);
        Assert.Equal(PaymentStatus.Authorized, result.Status);
    }
}