using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Controllers.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IValidator<PostPaymentRequest> postPaymentRequestValidator,
    IPaymentsService paymentsService): Controller
{
    [HttpGet("{id:guid}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<PaymentResponse> GetPayment(Guid id)
    {
        var payment = paymentsService.GetPayment(id);

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
        var validationResult = await postPaymentRequestValidator.ValidateAsync(postPaymentRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var postPaymentResponse = await paymentsService.ProcessPaymentAsync(postPaymentRequest);
        return new OkObjectResult(postPaymentResponse);
    }
}