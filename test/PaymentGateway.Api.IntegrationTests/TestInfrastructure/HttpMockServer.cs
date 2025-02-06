using System.Diagnostics;
using Newtonsoft.Json;
using PaymentGateway.Api.Tests.HttpMocks;
using WireMock.Matchers;
using WireMock.Server;
using WireMock.Settings;

namespace PaymentGateway.Api.Tests.TestInfrastructure;

public class HttpMockServer(int port) : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start(new WireMockServerSettings()
    {
        Port = port,
        StartAdminInterface = Debugger.IsAttached
    });
    
    public void AddMock(HttpMock mock)
    {
        var request = WireMock.RequestBuilders.Request.Create()
            .UsingMethod(mock.HttpMethod)
            .WithPath(mock.RequestPath);

        if (mock.RequestBody != null)
        {
            request.WithBody(new JsonPartialMatcher(
                JsonConvert.SerializeObject(mock.RequestBody)));
        }

        _server
            .Given(request)
            .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithStatusCode((int)mock.ResponseCode)
                .WithBodyAsJson(mock.ResponseBody));
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}