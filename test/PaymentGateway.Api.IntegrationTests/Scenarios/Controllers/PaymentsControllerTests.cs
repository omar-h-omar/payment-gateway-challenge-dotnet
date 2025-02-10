using System.Net;
using System.Net.Http.Json;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Controllers.Responses;
using PaymentGateway.Api.Tests.HttpMocks.AcquiringBank;
using PaymentGateway.Api.Tests.TestInfrastructure;

namespace PaymentGateway.Api.Tests.Scenarios.Controllers;

public class PaymentsControllerTests(TestServer testServer) : IClassFixture<TestServer>
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task GetPayment_RetrievesAPaymentSuccessfully_WhenPaymentIsFound()
    {
        // Arrange
        var payment = new PaymentResponse(Guid.NewGuid(), PaymentStatus.Authorized, _random.Next(1111, 9999).ToString(),
            _random.Next(1, 12), _random.Next(2023, 2030), "GBP", _random.Next(1, 10000));
        testServer.PaymentsRepository.Add(payment);

        // Act
        var response = await testServer.Client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(payment.Id, paymentResponse.Id);
    }

    [Fact]
    public async Task GetPayment_ReturnsNotFound_WhenPaymentNotFound()
    {
        // Act
        var response = await testServer.Client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task PostPaymentAsync_ReturnsBadRequest_WhenRequestIsNotValid()
    {
        // Act
        var response = await testServer.Client.PostAsJsonAsync<PostPaymentRequest>("/api/Payments", null!);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task PostPaymentAsync_ReturnsAPaymentSuccessfully_WhenRequestIsValid()
    {
        // Arrange
        var postPaymentRequest = new PostPaymentRequest("2222405343248877", _random.Next(1, 12),
            _random.Next(2026, 2030), "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());
        testServer.AcquiringBankServer.AddMock(Payment.GetMock());
        
        // Act
        var response = await testServer.Client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(postPaymentRequest.CardNumber[^4..], paymentResponse.CardNumberLastFour);
    }
}