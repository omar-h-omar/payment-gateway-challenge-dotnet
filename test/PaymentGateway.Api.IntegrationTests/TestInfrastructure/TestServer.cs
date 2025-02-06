using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.TestInfrastructure;

public class TestServer :  IAsyncLifetime
{
    public HttpClient Client;
    public IPaymentsRepository PaymentsRepository;
    
    private WebApplicationFactory<Program> _webApplicationFactory;
    public HttpMockServer AcquiringBankServer { get; private set; }
    
    public Task InitializeAsync()
    {
        AcquiringBankServer = new HttpMockServer(8080);
        _webApplicationFactory = new WebApplicationFactory<Program>();

        Client = _webApplicationFactory.CreateClient();
        PaymentsRepository = _webApplicationFactory.Server.Services.GetRequiredService<IPaymentsRepository>();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _webApplicationFactory.DisposeAsync();
    }
}