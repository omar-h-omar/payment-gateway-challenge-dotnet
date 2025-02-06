using System.Net;

using PaymentGateway.Api.Models.HttpClients.Responses;

namespace PaymentGateway.Api.Tests.HttpMocks.AcquiringBank;

public static class Payment
{
    public static HttpMock GetMock() => new HttpMock("POST", "/payments", HttpStatusCode.OK,
        new ProcessPaymentResponse(true, Guid.NewGuid().ToString()), null);
}