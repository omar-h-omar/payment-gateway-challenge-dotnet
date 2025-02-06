using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Controllers.Responses;
using PaymentGateway.Api.Models.HttpClients.Requests;
using PaymentGateway.Api.Models.HttpClients.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IValidator<PostPaymentRequest> _validator;
    private readonly IAcquiringBankClient _acquiringBankClient;
    private readonly PaymentsController _controller;
    private readonly Random _random = new();
    
    public PaymentsControllerTests()
    {
        _paymentsRepository = Substitute.For<IPaymentsRepository>();
        _validator = Substitute.For<IValidator<PostPaymentRequest>>();
        _acquiringBankClient = Substitute.For<IAcquiringBankClient>();
        _controller = new PaymentsController(_paymentsRepository, _validator, _acquiringBankClient);
    }
    
    [Fact]
    public void GetPayment_CallsGet()
    {
        // Arrange
        var id = Guid.NewGuid();
        var payment = new PaymentResponse(id, PaymentStatus.Authorized, _random.Next(1111, 9999).ToString(),
            _random.Next(1, 12), _random.Next(2026, 2030), "GBP", _random.Next(1, 1000));
        
        // Act
        _controller.GetPayment(id);
        
        // Assert
        _paymentsRepository.Received(1).Get(Arg.Is(id));
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
        await _validator.Received(1).ValidateAsync(request);
    }
    
    [Fact]
    public async Task PostPaymentAsync_CallsProcessPaymentAsync_WhenRequestIsValid()
    {
        // Arrange
        var cardNumber = "2222405343248877";
        var request = new PostPaymentRequest(cardNumber, _random.Next(1, 12),
            _random.Next(2026, 2030), "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());

        _validator.ValidateAsync(request).Returns(new ValidationResult());
        _acquiringBankClient.ProcessPaymentAsync(Arg.Any<ProcessPaymentRequest>())
            .Returns(new ProcessPaymentResponse(true, Guid.NewGuid().ToString()));
        
        // Act
        await _controller.PostPaymentAsync(request);
        
        // Assert
        await _acquiringBankClient.Received(1).ProcessPaymentAsync(Arg.Is<ProcessPaymentRequest>(r => r.CardNumber == cardNumber)); }
}