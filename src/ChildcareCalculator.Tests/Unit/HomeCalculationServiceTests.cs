using Xunit;
using ChildcareCalculator.Application.Services;
using ChildcareCalculator.Domain.Models;

namespace ChildcareCalculator.Tests.Unit;

public class HomeCalculationServiceTests
{
    private readonly HomeCalculationService _service;
    private readonly RoundingService _roundingService;

    public HomeCalculationServiceTests()
    {
        _roundingService = new RoundingService();
        _service = new HomeCalculationService(_roundingService);
    }

    [Theory]
    [InlineData(700000, 0.0035, 2450)] // 700000 * 0.0035 = 2450
    [InlineData(500000, 0.0035, 1750)] // 500000 * 0.0035 = 1750
    [InlineData(0, 0.0035, 0)]
    [InlineData(1000000, 0, 0)]
    public void CalculateHomeImputedIncome_ShouldReturnCorrectValue(
        decimal wozValue,
        decimal imputedRentalValuePercentage,
        decimal expected)
    {
        var result = _service.CalculateHomeImputedIncome(wozValue, imputedRentalValuePercentage);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(2450, 3000, -550)] // 2450 - 3000 = -550
    [InlineData(3000, 2450, 550)] // 3000 - 2450 = 550
    [InlineData(2450, 2450, 0)]
    [InlineData(0, 0, 0)]
    public void CalculateHomeBalance_ShouldReturnCorrectValue(
        decimal homeImputedIncome,
        decimal deductibleMortgageInterest,
        decimal expected)
    {
        var result = _service.CalculateHomeBalance(homeImputedIncome, deductibleMortgageInterest);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateHomeSettings_ShouldReturnCompleteSettings()
    {
        var wozValue = 700000m;
        var imputedRentalValuePercentage = 0.0035m;
        var otherBox1Deductions = 1000m;
        var deductibleMortgageInterest = 3000m;

        var result = _service.CalculateHomeSettings(
            wozValue,
            imputedRentalValuePercentage,
            otherBox1Deductions,
            deductibleMortgageInterest);

        // Verify properties
        Assert.Equal(wozValue, result.WozValue);
        Assert.Equal(imputedRentalValuePercentage, result.ImputedRentalValuePercentage);
        Assert.Equal(otherBox1Deductions, result.OtherBox1Deductions);

        // Verify calculated property
        var expectedImputedIncome = wozValue * imputedRentalValuePercentage;
        Assert.Equal(expectedImputedIncome, result.HomeImputedIncome);
    }

    [Fact]
    public void CalculateHomeSettings_WithZeroValues_ShouldReturnZeroResults()
    {
        var result = _service.CalculateHomeSettings(0, 0, 0, 0);

        Assert.Equal(0, result.WozValue);
        Assert.Equal(0, result.ImputedRentalValuePercentage);
        Assert.Equal(0, result.OtherBox1Deductions);
        Assert.Equal(0, result.HomeImputedIncome);
    }

    [Theory]
    [InlineData(700000, 0.0035, 3000, 2450)]
    [InlineData(500000, 0.0035, 2000, 1750)]
    public void CalculateHomeBalance_WithRealisticValues_ShouldCalculateCorrectly(
        decimal wozValue,
        decimal imputedRentalValuePercentage,
        decimal deductibleMortgageInterest,
        decimal expectedHomeBalance)
    {
        var homeImputedIncome = wozValue * imputedRentalValuePercentage;
        var result = _service.CalculateHomeBalance(homeImputedIncome, deductibleMortgageInterest);
        Assert.Equal(expectedHomeBalance, result);
    }
}
