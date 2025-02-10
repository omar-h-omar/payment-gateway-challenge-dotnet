using PaymentGateway.Api.Models.Controllers.Requests;
using PaymentGateway.Api.Models.Validators;

namespace PaymentGateway.Api.Tests.Validators;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _validator = new();

    [Theory]
    [InlineData(null, "'Card Number' must not be empty.")]
    [InlineData("", "'Card Number' must not be empty.")]
    [InlineData(" ", "'Card Number' must not be empty.")]
    [InlineData("1", "'Card Number' must be between 14 and 19 characters. You entered 1 characters.")]
    [InlineData("12345678901234567890", "'Card Number' must be between 14 and 19 characters. You entered 20 characters.")]
    [InlineData("aaaaaaaaaaaaaa", "Card number must only contain numeric characters")]
    public async Task ValidateAsync_ReturnsFalse_WhenCardNumberIsInvalid(string cardNumber, string expectedError)
    {
        // Arrange
        var request = new PostPaymentRequest(cardNumber, 12, 2026, "GBP", 100, "123");
        
        // Act
        var result = await _validator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.Errors.FirstOrDefault(error => error.ErrorMessage == expectedError));
    }
    
    [Theory]
    [InlineData(null, 2025, "'Expiry Month' must not be empty.")]
    [InlineData(0, 2025, "'Expiry Month' must not be empty.")]
    [InlineData(4, 0, "'Expiry Year' must not be empty.")]
    [InlineData(4, null, "'Expiry Year' must not be empty.")]
    [InlineData(13, 2025, "'Expiry Month' must be between 1 and 12. You entered 13.")]
    [InlineData(4, 1990, "Payment date must be in the future")]
    public async Task ValidateAsync_ReturnsFalse_WhenPaymentDateIsInvalid(int expiryMonth, int expiryYear, string expectedError)
    {
        // Arrange
        var request = new PostPaymentRequest("2222405343248877", expiryMonth, expiryYear, "GBP", 100, "123");
        
        // Act
        var result = await _validator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.Errors.FirstOrDefault(error => error.ErrorMessage == expectedError));
    }
    
    [Theory]
    [InlineData(null, "'Currency' must not be empty.")]
    [InlineData("", "'Currency' must not be empty.")]
    [InlineData(" ", "'Currency' must not be empty.")]
    [InlineData("test", "Currency must be GBP, USD or EUR")]
    public async Task ValidateAsync_ReturnsFalse_WhenCurrencyIsInvalid(string currency, string expectedError)
    {
        // Arrange
        var request = new PostPaymentRequest("2222405343248877", 12, 2026, currency, 100, "123");
        
        // Act
        var result = await _validator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.Errors.FirstOrDefault(error => error.ErrorMessage == expectedError));
    }
    
    [Theory]
    [InlineData(null, "'Amount' must not be empty.")]
    [InlineData(0, "'Amount' must not be empty.")]
    public async Task ValidateAsync_ReturnsFalse_WhenAmountIsInvalid(int amount, string expectedError)
    {
        // Arrange
        var request = new PostPaymentRequest("2222405343248877", 12, 2026, "GBP", amount, "123");
        
        // Act
        var result = await _validator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.Errors.FirstOrDefault(error => error.ErrorMessage == expectedError));
    }
    
    [Theory]
    [InlineData(null, "'Cvv' must not be empty.")]
    [InlineData("", "'Cvv' must not be empty.")]
    [InlineData(" ", "'Cvv' must not be empty.")]
    [InlineData("1", "'Cvv' must be between 3 and 4 characters. You entered 1 characters.")]
    [InlineData("12345", "'Cvv' must be between 3 and 4 characters. You entered 5 characters.")]
    [InlineData("cvv", "Cvv must only contain numeric characters")]
    public async Task ValidateAsync_ReturnsFalse_WhenCvvIsInvalid(string cvv, string expectedError)
    {
        // Arrange
        var request = new PostPaymentRequest("2222405343248877", 12, 2026, "GBP", 100, cvv);
        
        // Act
        var result = await _validator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.Errors.FirstOrDefault(error => error.ErrorMessage == expectedError));
    }
}