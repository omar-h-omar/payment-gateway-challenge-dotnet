using System.Net;

namespace PaymentGateway.Api.Tests.HttpMocks;

public record HttpMock(
    string HttpMethod,
    string RequestPath,
    HttpStatusCode ResponseCode,
    object ResponseBody,
    object RequestBody);