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
    IPaymentsRepository paymentsRepository,
    IValidator<PostPaymentRequest> postPaymentRequestValidator,
    IAcquiringBankClient acquiringBankClient)
    : Controller
{
    [HttpGet("{id:guid}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<PaymentResponse> GetPayment(Guid id)
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

        var processPaymentRequest = new ProcessPaymentRequest(postPaymentRequest.CardNumber,
            $"{postPaymentRequest.ExpiryMonth:D2}/{postPaymentRequest.ExpiryYear:D4}",
            postPaymentRequest.Currency, postPaymentRequest.Amount, postPaymentRequest.Cvv.ToString());
        
        var acquiringBankResponse = await acquiringBankClient.ProcessPaymentAsync(processPaymentRequest);

        var postPaymentResponse = new PaymentResponse(
            string.IsNullOrEmpty(acquiringBankResponse.AuthorizationCode)
                ? Guid.NewGuid()
                : Guid.Parse(acquiringBankResponse.AuthorizationCode),
            acquiringBankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
            postPaymentRequest.CardNumber[^4..],
            postPaymentRequest.ExpiryMonth, postPaymentRequest.ExpiryYear,
            postPaymentRequest.Currency, postPaymentRequest.Amount);
        
        paymentsRepository.Add(postPaymentResponse);

        return new OkObjectResult(postPaymentResponse);
    }
}