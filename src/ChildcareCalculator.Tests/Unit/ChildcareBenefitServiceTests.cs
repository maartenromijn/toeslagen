using Xunit;
using ChildcareCalculator.Application.Services;
using ChildcareCalculator.Domain.Models;

namespace ChildcareCalculator.Tests.Unit;

public class ChildcareBenefitServiceTests
{
    private readonly ChildcareBenefitService _service;
    private readonly RoundingService _roundingService;

    public ChildcareBenefitServiceTests()
    {
        _roundingService = new RoundingService();
        _service = new ChildcareBenefitService(_roundingService);
    }

    [Theory]
    [InlineData(2027, 50000, 0.96, 0, 1.0, 0.96)] // Income <= 71903, should return 96%
    [InlineData(2027, 100000, 0.8, 0.051, 1.0, 0.851)] // Income > 71903 and <= 175430, should return min(0.96, 0.8 + 0.051)
    [InlineData(2027, 200000, 0.8, 0.026, 1.0, 0.826)] // Income > 175430, should return min(0.96, 0.8 + 0.026)
    [InlineData(2028, 50000, 0.8, 0.1, 1.0, 0.9)] // Other year: base + surcharge, capped at 96%
    public void CalculateAutomaticChildcareBenefitPercentage_ShouldReturnCorrectValue(
        int year,
        decimal assessmentIncome,
        decimal basePercentage,
        decimal policySurcharge,
        decimal incomeBoundaryIndexFactor,
        decimal expectedRounded)
    {
        var result = _service.CalculateAutomaticChildcareBenefitPercentage(
            year,
            assessmentIncome,
            basePercentage,
            policySurcharge,
            incomeBoundaryIndexFactor);
        
        var roundedResult = decimal.Round(result, 3);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Theory]
    [InlineData(0.9, 0.8, 0.9)] // Override provided
    [InlineData(null, 0.8, 0.8)] // No override, use automatic
    public void GetAppliedBenefitPercentage_ShouldReturnCorrectValue(
        decimal? overridePercentage,
        decimal automaticPercentage,
        decimal expected)
    {
        var result = _service.GetAppliedBenefitPercentage(overridePercentage, automaticPercentage);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10000, 0.9, 9000)] // 10000 * 0.9 = 9000
    [InlineData(10000, 0.5, 5000)]
    [InlineData(0, 0.9, 0)]
    [InlineData(10000, 0, 0)]
    public void CalculateChildcareBenefitAmount_ShouldReturnCorrectValue(
        decimal eligibleChildcareCost,
        decimal appliedBenefitPercentage,
        decimal expected)
    {
        var result = _service.CalculateChildcareBenefitAmount(eligibleChildcareCost, appliedBenefitPercentage);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10000, 9000, 1000)] // 10000 - 9000 = 1000
    [InlineData(10000, 0, 10000)]
    [InlineData(0, 0, 0)]
    public void CalculateOwnContributionYear_ShouldReturnCorrectValue(
        decimal actualChildcareCost,
        decimal childcareBenefitAmount,
        decimal expected)
    {
        var result = _service.CalculateOwnContributionYear(actualChildcareCost, childcareBenefitAmount);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(12000, 1000)] // 12000 / 12 = 1000
    [InlineData(0, 0)]
    [InlineData(14400, 1200)] // 14400 / 12 = 1200
    public void CalculateOwnContributionMonth_ShouldReturnCorrectValue(
        decimal ownContributionYear,
        decimal expected)
    {
        var result = _service.CalculateOwnContributionMonth(ownContributionYear);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateChildcareBenefit_ShouldReturnCompleteResult()
    {
        var input = new ChildcareBenefitInput
        {
            Year = 2027,
            AssessmentIncome = 50000,
            EligibleChildcareCost = 10000,
            ActualChildcareCost = 12000,
            BasePercentage = 0.96m,
            PolicySurcharge = 0,
            IncomeBoundaryIndexFactor = 1.0m,
            OverridePercentage = null
        };

        var result = _service.CalculateChildcareBenefit(input);

        Assert.Equal(2027, result.Year);
        Assert.Equal(50000, result.AssessmentIncome);
        Assert.Equal(10000, result.EligibleChildcareCost);
        Assert.Equal(12000, result.ActualChildcareCost);
        Assert.Equal(0.9, result.AutomaticPercentage); // 96% capped at 96%
        Assert.Equal(0.9, result.AppliedPercentage);
        Assert.Equal(9000, result.ChildcareBenefitAmount);
        Assert.Equal(3000, result.OwnContributionYear);
        Assert.Equal(250, result.OwnContributionMonth);
    }

    [Fact]
    public void CalculateChildcareBenefit_WithOverride_ShouldUseOverride()
    {
        var input = new ChildcareBenefitInput
        {
            Year = 2027,
            AssessmentIncome = 50000,
            EligibleChildcareCost = 10000,
            ActualChildcareCost = 12000,
            BasePercentage = 0.8m,
            PolicySurcharge = 0,
            IncomeBoundaryIndexFactor = 1.0m,
            OverridePercentage = 0.95m
        };

        var result = _service.CalculateChildcareBenefit(input);

        Assert.Equal(0.9, result.AutomaticPercentage); // Would be 96% without override
        Assert.Equal(0.95, result.AppliedPercentage); // Uses override
        Assert.Equal(9500, result.ChildcareBenefitAmount); // 10000 * 0.95
        Assert.Equal(2500, result.OwnContributionYear); // 12000 - 9500

        // Should have warning about override
        Assert.Contains("Using manual override for childcare benefit percentage", result.Warnings);
    }

    [Fact]
    public void CalculateChildcareBenefit_WithHighIncome_ShouldApplySurcharge()
    {
        var input = new ChildcareBenefitInput
        {
            Year = 2027,
            AssessmentIncome = 100000, // > 71903, <= 175430
            EligibleChildcareCost = 10000,
            ActualChildcareCost = 12000,
            BasePercentage = 0.8m,
            PolicySurcharge = 0,
            IncomeBoundaryIndexFactor = 1.0m,
            OverridePercentage = null
        };

        var result = _service.CalculateChildcareBenefit(input);

        // Should be min(0.96, 0.8 + 0.051) = 0.851
        Assert.Equal(0.851, decimal.Round(result.AutomaticPercentage, 3));
        Assert.Equal(0.851, decimal.Round(result.AppliedPercentage, 3));
    }

    [Fact]
    public void CalculateChildcareBenefit_WithVeryHighIncome_ShouldApplyLowerSurcharge()
    {
        var input = new ChildcareBenefitInput
        {
            Year = 2027,
            AssessmentIncome = 200000, // > 175430
            EligibleChildcareCost = 10000,
            ActualChildcareCost = 12000,
            BasePercentage = 0.8m,
            PolicySurcharge = 0,
            IncomeBoundaryIndexFactor = 1.0m,
            OverridePercentage = null
        };

        var result = _service.CalculateChildcareBenefit(input);

        // Should be min(0.96, 0.8 + 0.026) = 0.826
        Assert.Equal(0.826, decimal.Round(result.AutomaticPercentage, 3));
    }

    [Fact]
    public void LookupBandPercentage_WithEmptyBands_ShouldReturnZero()
    {
        var result = _service.LookupBandPercentage(50000, new List<ChildcareBenefitBand>());
        Assert.Equal(0, result);
    }

    [Fact]
    public void LookupBandPercentage_WithMatchingBand_ShouldReturnPercentage()
    {
        var bands = new List<ChildcareBenefitBand>
        {
            new ChildcareBenefitBand
            {
                BandNumber = 1,
                MinIncome = 0,
                MaxIncome = 50000,
                FirstChildPercentage = 0.96m,
                AdditionalChildPercentage = 0.96m
            },
            new ChildcareBenefitBand
            {
                BandNumber = 2,
                MinIncome = 50001,
                MaxIncome = 100000,
                FirstChildPercentage = 0.8m,
                AdditionalChildPercentage = 0.8m
            }
        };

        var result = _service.LookupBandPercentage(25000, bands);
        Assert.Equal(0.96m, result);

        result = _service.LookupBandPercentage(75000, bands);
        Assert.Equal(0.8m, result);
    }

    [Fact]
    public void LookupBandPercentage_WithIncomeAboveAllBands_ShouldReturnZero()
    {
        var bands = new List<ChildcareBenefitBand>
        {
            new ChildcareBenefitBand
            {
                BandNumber = 1,
                MinIncome = 0,
                MaxIncome = 50000,
                FirstChildPercentage = 0.96m,
                AdditionalChildPercentage = 0.96m
            }
        };

        var result = _service.LookupBandPercentage(100000, bands);
        Assert.Equal(0, result);
    }
}

// Define the input and result classes for testing
public class ChildcareBenefitInput
{
    public int Year { get; set; }
    public decimal AssessmentIncome { get; set; }
    public decimal EligibleChildcareCost { get; set; }
    public decimal ActualChildcareCost { get; set; }
    public decimal BasePercentage { get; set; }
    public decimal PolicySurcharge { get; set; }
    public decimal IncomeBoundaryIndexFactor { get; set; }
    public decimal? OverridePercentage { get; set; }
}

public class ChildcareBenefitResult
{
    public int Year { get; set; }
    public decimal AssessmentIncome { get; set; }
    public decimal EligibleChildcareCost { get; set; }
    public decimal ActualChildcareCost { get; set; }
    public decimal BasePercentage { get; set; }
    public decimal PolicySurcharge { get; set; }
    public decimal IncomeBoundaryIndexFactor { get; set; }
    public decimal? OverridePercentage { get; set; }
    public decimal AutomaticPercentage { get; set; }
    public decimal AppliedPercentage { get; set; }
    public decimal ChildcareBenefitAmount { get; set; }
    public decimal OwnContributionYear { get; set; }
    public decimal OwnContributionMonth { get; set; }
    public List<string> Warnings { get; set; } = new List<string>();
}
