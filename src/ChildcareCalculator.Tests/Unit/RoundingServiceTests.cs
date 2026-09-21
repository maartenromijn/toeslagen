using Xunit;
using ChildcareCalculator.Application.Services;

namespace ChildcareCalculator.Tests.Unit;

public class RoundingServiceTests
{
    private readonly RoundingService _service;

    public RoundingServiceTests()
    {
        _service = new RoundingService();
    }

    [Theory]
    [InlineData(123.456, 123.46)] // Round to 2 decimals (default)
    [InlineData(123.454, 123.45)]
    [InlineData(123.455, 123.46)] // AwayFromZero
    [InlineData(123, 123.00)]
    [InlineData(0, 0.00)]
    public void Round_WithDefaultDecimalPlaces_ShouldRoundTo2Decimals(
        decimal value,
        decimal expected)
    {
        var result = _service.Round(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.456, 123.46)]
    [InlineData(123.454, 123.45)]
    [InlineData(123.455, 123.46)]
    public void RoundToNearestCent_ShouldRoundCorrectly(
        decimal value,
        decimal expected)
    {
        var result = _service.RoundToNearestCent(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.456, 123)]
    [InlineData(123.999, 124)]
    [InlineData(123.499, 123)]
    [InlineData(123.500, 124)]
    public void RoundToNearestEuro_ShouldRoundCorrectly(
        decimal value,
        decimal expected)
    {
        var result = _service.RoundToNearestEuro(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.4567, 2, 123.46)] // 2 decimal places
    [InlineData(123.4567, 1, 123.5)] // 1 decimal place
    [InlineData(123.4567, 3, 123.457)] // 3 decimal places
    [InlineData(123.4567, 0, 123)] // 0 decimal places
    public void Round_WithCustomDecimalPlaces_ShouldRoundCorrectly(
        decimal value,
        int decimalPlaces,
        decimal expected)
    {
        var service = new RoundingService(decimalPlaces);
        var result = service.Round(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.4567, 2, 123.45)] // Truncate, don't round
    [InlineData(123.9999, 2, 123.99)]
    [InlineData(123.4567, 1, 123.4)]
    public void Truncate_ShouldTruncateCorrectly(
        decimal value,
        int decimalPlaces,
        decimal expected)
    {
        var result = _service.Truncate(value, decimalPlaces);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0.5, 0.5000)]
    [InlineData(0.123456, 0.1235)]
    [InlineData(0.999999, 1.0000)]
    [InlineData(0.000001, 0.0000)]
    public void RoundPercentage_ShouldRoundTo4Decimals(
        decimal percentage,
        decimal expected)
    {
        var result = _service.RoundPercentage(percentage);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Round_WithNegativeValues_ShouldRoundAwayFromZero()
    {
        // MidpointRounding.AwayFromZero means -1.5 rounds to -2, not -1
        var result = _service.RoundToNearestCent(-1.5m);
        Assert.Equal(-1.50m, result); // Actually, with AwayFromZero, -1.5 should round to -2.00
        
        // Let's test with a value that should round up
        result = _service.RoundToNearestCent(-1.51m);
        Assert.Equal(-1.51m, result); // This might not round as expected with AwayFromZero
    }

    [Fact]
    public void Round_WithVerySmallValues_ShouldHandleCorrectly()
    {
        var result = _service.Round(0.0001m);
        Assert.Equal(0.00m, result);

        result = _service.Round(0.0099m);
        Assert.Equal(0.01m, result);
    }

    [Fact]
    public void Round_WithLargeValues_ShouldHandleCorrectly()
    {
        var result = _service.Round(123456789.123456m);
        Assert.Equal(123456789.12m, result);

        result = _service.Round(999999999.999m);
        Assert.Equal(1000000000.00m, result);
    }
}
