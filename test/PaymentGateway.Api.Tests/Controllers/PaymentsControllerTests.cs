using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private readonly IValidator<PostPaymentRequest> _validator;
    private readonly IPaymentsService _paymentsService;
    private readonly PaymentsController _controller;
    private readonly Random _random = new();
    
    public PaymentsControllerTests()
    {
        _validator = Substitute.For<IValidator<PostPaymentRequest>>();
        _paymentsService = Substitute.For<IPaymentsService>();
        _controller = new PaymentsController(_validator, _paymentsService);
    }
    
    [Fact]
    public void GetPayment_CallsGetPayment()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        _controller.GetPayment(id);
        
        // Assert
        _paymentsService.Received(1).GetPayment(Arg.Is(id));
    }
    
    [Fact]
    public async Task PostPaymentAsync_CallsValidateAsync()
    {
        // Arrange
        var request = new PostPaymentRequest("2222405343248877", _random.Next(1, 12),
            _random.Next(2026, 2030), "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());
        _validator.ValidateAsync(request).Returns(new ValidationResult { Errors = [new ValidationFailure()] });
        
        // Act
        await _controller.PostPaymentAsync(request);
        
        // Assert
        await _validator.Received(1).ValidateAsync(Arg.Is(request));
    }
    
    [Fact]
    public async Task PostPaymentAsync_CallsProcessPaymentAsync_WhenRequestIsValid()
    {
        // Arrange
        var request = new PostPaymentRequest("2222405343248877", _random.Next(1, 12),
            _random.Next(2026, 2030), "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());
        _validator.ValidateAsync(request).Returns(new ValidationResult());
        
        // Act
        await _controller.PostPaymentAsync(request);
        
        // Assert
        await _paymentsService.Received(1).ProcessPaymentAsync(Arg.Is(request));
    }
    
    [Fact]
    public async Task PostPaymentAsync_DoesNotCallProcessPaymentAsync_WhenRequestIsInValid()
    {
        // Arrange
        var request = new PostPaymentRequest("2222405343248877", _random.Next(1, 12),
            _random.Next(2026, 2030), "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());
        _validator.ValidateAsync(request).Returns(new ValidationResult { Errors = [new ValidationFailure()] });
        
        // Act
        await _controller.PostPaymentAsync(request);
        
        // Assert
        await _paymentsService.DidNotReceive().ProcessPaymentAsync(Arg.Any<PostPaymentRequest>());
    }
}