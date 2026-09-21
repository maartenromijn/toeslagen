using Xunit;
using ChildcareCalculator.Application.Services;

namespace ChildcareCalculator.Tests.Unit;

public class ChildcareCostServiceTests
{
    private readonly ChildcareCostService _service;
    private readonly RoundingService _roundingService;

    public ChildcareCostServiceTests()
    {
        _roundingService = new RoundingService();
        _service = new ChildcareCostService(_roundingService);
    }

    [Theory]
    [InlineData(49, 3, 11, 1717)] // 49 * 3 * 11 = 1717
    [InlineData(52, 5, 10, 2600)] // 52 * 5 * 10 = 2600
    [InlineData(0, 0, 0, 0)]
    [InlineData(52, 0, 10, 0)]
    public void CalculateChildcareHoursPerYear_ShouldReturnCorrectValue(
        int childcareWeeks,
        int daysPerWeek,
        decimal hoursPerDay,
        decimal expected)
    {
        var result = _service.CalculateChildcareHoursPerYear(childcareWeeks, daysPerWeek, hoursPerDay);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1717, 11.8, 20260.6)] // 1717 * 11.8 = 20260.6
    [InlineData(2600, 10, 26000)] // 2600 * 10 = 26000
    [InlineData(0, 10, 0)]
    [InlineData(100, 0, 0)]
    public void CalculateActualChildcareCost_ShouldReturnCorrectValue(
        decimal childcareHoursPerYear,
        decimal actualHourlyRate,
        decimal expectedRounded)
    {
        var result = _service.CalculateActualChildcareCost(childcareHoursPerYear, actualHourlyRate);
        var roundedResult = decimal.Round(result, 1);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Theory]
    [InlineData(1717, 11.8, 11.6, 20030.2)] // 1717 * min(11.8, 11.6) = 1717 * 11.6 = 20030.2
    [InlineData(1717, 11.4, 11.6, 19583.8)] // 1717 * min(11.4, 11.6) = 1717 * 11.4 = 19583.8
    [InlineData(1717, 12.0, 11.6, 20030.2)] // 1717 * min(12.0, 11.6) = 1717 * 11.6 = 20030.2
    public void CalculateEligibleChildcareCost_ShouldReturnCorrectValue(
        decimal childcareHoursPerYear,
        decimal actualHourlyRate,
        decimal maximumHourlyRate,
        decimal expectedRounded)
    {
        var result = _service.CalculateEligibleChildcareCost(
            childcareHoursPerYear,
            actualHourlyRate,
            maximumHourlyRate);
        var roundedResult = decimal.Round(result, 1);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Fact]
    public void CalculateChildcareCost_ShouldReturnCompleteResult()
    {
        var input = new ChildcareCostInput
        {
            Year = 2027,
            ChildcareWeeks = 49,
            DaysPerWeek = 3,
            HoursPerDay = 11,
            ActualHourlyRate = 11.8m,
            MaximumHourlyRate = 11.6m
        };

        var result = _service.CalculateChildcareCost(input);

        Assert.Equal(2027, result.Year);
        Assert.Equal(49, result.ChildcareWeeks);
        Assert.Equal(3, result.DaysPerWeek);
        Assert.Equal(11, result.HoursPerDay);
        Assert.Equal(11.8m, result.ActualHourlyRate);
        Assert.Equal(11.6m, result.MaximumHourlyRate);

        // Verify calculated values
        var expectedHours = 49 * 3 * 11;
        Assert.Equal(expectedHours, result.ChildcareHoursPerYear);

        var expectedActualCost = expectedHours * 11.8m;
        Assert.Equal(_roundingService.RoundToNearestCent(expectedActualCost), result.ActualChildcareCost);

        var expectedEligibleCost = expectedHours * Math.Min(11.8m, 11.6m);
        Assert.Equal(_roundingService.RoundToNearestCent(expectedEligibleCost), result.EligibleChildcareCost);
    }

    [Fact]
    public void CalculateChildcareCost_WithActualRateBelowMaximum_ShouldUseActualRate()
    {
        var input = new ChildcareCostInput
        {
            Year = 2027,
            ChildcareWeeks = 52,
            DaysPerWeek = 5,
            HoursPerDay = 8,
            ActualHourlyRate = 10m,
            MaximumHourlyRate = 12m
        };

        var result = _service.CalculateChildcareCost(input);

        // Since actual rate (10) is below maximum (12), both costs should be the same
        Assert.Equal(result.ActualChildcareCost, result.EligibleChildcareCost);
    }

    [Fact]
    public void CalculateChildcareCost_WithActualRateAboveMaximum_ShouldCapEligibleCost()
    {
        var input = new ChildcareCostInput
        {
            Year = 2027,
            ChildcareWeeks = 52,
            DaysPerWeek = 5,
            HoursPerDay = 8,
            ActualHourlyRate = 15m,
            MaximumHourlyRate = 12m
        };

        var result = _service.CalculateChildcareCost(input);

        // Actual cost should be higher than eligible cost
        Assert.True(result.ActualChildcareCost > result.EligibleChildcareCost);

        // Eligible cost should use maximum rate
        var expectedHours = 52 * 5 * 8;
        var expectedEligibleCost = expectedHours * 12m;
        Assert.Equal(_roundingService.RoundToNearestCent(expectedEligibleCost), result.EligibleChildcareCost);
    }

    [Fact]
    public void CalculateChildcareCost_WithZeroHours_ShouldReturnZeroCosts()
    {
        var input = new ChildcareCostInput
        {
            Year = 2027,
            ChildcareWeeks = 0,
            DaysPerWeek = 0,
            HoursPerDay = 0,
            ActualHourlyRate = 10m,
            MaximumHourlyRate = 12m
        };

        var result = _service.CalculateChildcareCost(input);

        Assert.Equal(0, result.ChildcareHoursPerYear);
        Assert.Equal(0, result.ActualChildcareCost);
        Assert.Equal(0, result.EligibleChildcareCost);
    }
}

// Define the input and result classes for testing
public class ChildcareCostInput
{
    public int Year { get; set; }
    public int ChildcareWeeks { get; set; }
    public int DaysPerWeek { get; set; }
    public decimal HoursPerDay { get; set; }
    public decimal ActualHourlyRate { get; set; }
    public decimal MaximumHourlyRate { get; set; }
}

public class ChildcareCostResult
{
    public int Year { get; set; }
    public int ChildcareWeeks { get; set; }
    public int DaysPerWeek { get; set; }
    public decimal HoursPerDay { get; set; }
    public decimal ActualHourlyRate { get; set; }
    public decimal MaximumHourlyRate { get; set; }
    public decimal ChildcareHoursPerYear { get; set; }
    public decimal ActualChildcareCost { get; set; }
    public decimal EligibleChildcareCost { get; set; }
}
