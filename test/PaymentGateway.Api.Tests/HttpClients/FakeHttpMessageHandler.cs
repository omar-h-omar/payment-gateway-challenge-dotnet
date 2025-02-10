namespace PaymentGateway.Api.Tests.HttpClients;

public class FakeHttpMessageHandler : DelegatingHandler
{
    public HttpResponseMessage Response { get; set; }
    public HttpRequestMessage Request { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Request = request;
        return await Task.FromResult(Response);
    }
}