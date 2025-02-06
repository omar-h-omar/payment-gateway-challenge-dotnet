using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Controllers.Responses;
using PaymentGateway.Api.Models.HttpClients.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    PaymentsRepository paymentsRepository,
    IValidator<PostPaymentRequest> postPaymentRequestValidator,
    IAcquiringBankClient acquiringBankClient)
    : Controller
{
    [HttpGet("{id:guid}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<PaymentResponse> GetPaymentAsync(Guid id)
    {
        var payment = paymentsRepository.Get(id);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }
        
        return new OkObjectResult(payment);
    }
    
    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest postPaymentRequest)
    {
        var result = await postPaymentRequestValidator.ValidateAsync(postPaymentRequest);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }
        
        var processPaymentRequest = new ProcessPaymentRequest(cardNumber: postPaymentRequest.CardNumber,
            expiryDate: $"{postPaymentRequest.ExpiryMonth:D2}/{postPaymentRequest.ExpiryYear:D4}",
            amount: postPaymentRequest.Amount, currency: postPaymentRequest.Currency, cvv: postPaymentRequest.Cvv.ToString());
        
        var acquiringBankResponse = await acquiringBankClient.ProcessPaymentAsync(processPaymentRequest);
        
        var postPaymentResponse = new PaymentResponse
        {
            Id = string.IsNullOrEmpty(acquiringBankResponse.AuthorizationCode) ? Guid.NewGuid() : Guid.Parse(acquiringBankResponse.AuthorizationCode),
            Status = acquiringBankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
            CardNumberLastFour = int.Parse(postPaymentRequest.CardNumber[^4..]),
            ExpiryMonth = postPaymentRequest.ExpiryMonth,
            ExpiryYear = postPaymentRequest.ExpiryYear,
            Currency = postPaymentRequest.Currency,
            Amount = postPaymentRequest.Amount
        };
        
        paymentsRepository.Add(postPaymentResponse);

        return new OkObjectResult(postPaymentResponse);
    }
}