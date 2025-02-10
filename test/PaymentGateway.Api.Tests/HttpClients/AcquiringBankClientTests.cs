using System.Net;
using System.Text;
using Newtonsoft.Json;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.HttpClients.Requests;
using PaymentGateway.Api.Models.HttpClients.Responses;

namespace PaymentGateway.Api.Tests.HttpClients;

public class AcquiringBankClientTests
{
    private const string BaseUrl = "http://temp.org";
    private readonly Random _random = new();
    
    [Fact]
    public async Task ProcessPaymentAsync_ReturnsProcessPaymentResponse_WhenResponseIsOK()
    {
        // Arrange
        var request = new ProcessPaymentRequest("2222405343248877", $"{_random.Next(1, 12) / _random.Next(2026, 2030)}",
            "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());
        var client = SetupClient(HttpStatusCode.OK, new ProcessPaymentResponse(true, Guid.NewGuid().ToString()));
        
        // Act
        var result = await client.ProcessPaymentAsync(request);
        
        // Assert
        Assert.True(result.Authorized);
    }
    
    [Fact]
    public async Task ProcessPaymentAsync_ThrowsHttpRequestException_WhenResponseIsBadRequest()
    {
        // Arrange
        var request = new ProcessPaymentRequest("2222405343248877", $"{_random.Next(1, 12) / _random.Next(2026, 2030)}",
            "GBP", _random.Next(1, 10000), _random.Next(001, 9999).ToString());
        var client = SetupClient<object>(HttpStatusCode.BadRequest);
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => client.ProcessPaymentAsync(request));
    }
    
    private IAcquiringBankClient SetupClient<T>(HttpStatusCode statusCode = HttpStatusCode.OK,
        T result = null) where T: class
    {
        var content = result is string s ? s : JsonConvert.SerializeObject(result);
        var fakeHandler = new FakeHttpMessageHandler
        {
            Response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            }
        };

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri(BaseUrl)
        };
        
        return new AcquiringBankClient(httpClient);
    }
}