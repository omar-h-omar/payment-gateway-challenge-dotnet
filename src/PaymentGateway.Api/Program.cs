using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.StartupConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddValidators();
builder.Services.AddHttpClient<IAcquiringBankClient, AcquiringBankClient>(c =>
{
    c.BaseAddress = builder.Configuration.GetSection("AcquiringBank:BaseUrl").Get<Uri>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddSingleton<IPaymentsService, PaymentsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }