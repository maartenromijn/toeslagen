using Xunit;
using ChildcareCalculator.Application.Services;

namespace ChildcareCalculator.Tests.Unit;

public class AssessmentIncomeServiceTests
{
    private readonly AssessmentIncomeService _service;
    private readonly RoundingService _roundingService;

    public AssessmentIncomeServiceTests()
    {
        _roundingService = new RoundingService();
        _service = new AssessmentIncomeService(_roundingService);
    }

    [Theory]
    [InlineData(50000, 40000, 2500, 3000, 1000, 92500)] // 50000 + 40000 + 2500 - 3000 - 1000 = 92500
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(100000, 0, 0, 0, 0, 100000)]
    public void CalculateAssessmentIncome_ShouldReturnCorrectValue(
        decimal person1TaxableIncome,
        decimal person2TaxableIncome,
        decimal homeImputedIncome,
        decimal deductibleMortgageInterest,
        decimal otherBox1Deductions,
        decimal expected)
    {
        var result = _service.CalculateAssessmentIncome(
            person1TaxableIncome,
            person2TaxableIncome,
            homeImputedIncome,
            deductibleMortgageInterest,
            otherBox1Deductions);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateAssessmentIncome_WithRealisticValues_ShouldCalculateCorrectly()
    {
        // Using values from the JSON
        var person1TaxableIncome = 5842.81m * 12 + 4250 + 4182.13m; // Maarten's approximate annual income
        var person2TaxableIncome = 3300 * 12; // Gina's approximate annual income
        var homeImputedIncome = 700000 * 0.0035m; // 700000 * 0.35%
        var deductibleMortgageInterest = 3000; // Estimated
        var otherBox1Deductions = 0;

        var result = _service.CalculateAssessmentIncome(
            person1TaxableIncome,
            person2TaxableIncome,
            homeImputedIncome,
            deductibleMortgageInterest,
            otherBox1Deductions);

        var expected = person1TaxableIncome + person2TaxableIncome + homeImputedIncome - deductibleMortgageInterest;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateAssessmentIncomeInput_ShouldReturnCompleteResult()
    {
        var input = new AssessmentIncomeInput
        {
            Year = 2027,
            Person1TaxableIncome = 50000,
            Person2TaxableIncome = 40000,
            HomeImputedIncome = 2500,
            DeductibleMortgageInterest = 3000,
            OtherBox1Deductions = 1000
        };

        var result = _service.CalculateAssessmentIncome(input);

        Assert.Equal(2027, result.Year);
        Assert.Equal(50000, result.Person1TaxableIncome);
        Assert.Equal(40000, result.Person2TaxableIncome);
        Assert.Equal(2500, result.HomeImputedIncome);
        Assert.Equal(3000, result.DeductibleMortgageInterest);
        Assert.Equal(1000, result.OtherBox1Deductions);
        Assert.Equal(92500, result.AssessmentIncome);
    }

    [Theory]
    [InlineData(100000, 1.0, 100000)] // 100000 / 1.0 = 100000
    [InlineData(100000, 1.1, 90909.09)] // 100000 / 1.1 ≈ 90909.09
    [InlineData(0, 1.0, 0)]
    public void CalculateIncomeAt2026Boundaries_ShouldReturnCorrectValue(
        decimal assessmentIncome,
        decimal incomeBoundaryIndexFactor,
        decimal expectedRounded)
    {
        var result = _service.CalculateIncomeAt2026Boundaries(assessmentIncome, incomeBoundaryIndexFactor);
        var roundedResult = decimal.Round(result, 2);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Fact]
    public void CalculateIncomeAt2026Boundaries_WithZeroFactor_ShouldReturnAssessmentIncome()
    {
        var result = _service.CalculateIncomeAt2026Boundaries(100000, 0);
        Assert.Equal(100000, result);
    }
}

// Define the input and result classes for testing
public class AssessmentIncomeInput
{
    public int Year { get; set; }
    public decimal Person1TaxableIncome { get; set; }
    public decimal Person2TaxableIncome { get; set; }
    public decimal HomeImputedIncome { get; set; }
    public decimal DeductibleMortgageInterest { get; set; }
    public decimal OtherBox1Deductions { get; set; }
}

public class AssessmentIncomeResult
{
    public int Year { get; set; }
    public decimal Person1TaxableIncome { get; set; }
    public decimal Person2TaxableIncome { get; set; }
    public decimal HomeImputedIncome { get; set; }
    public decimal DeductibleMortgageInterest { get; set; }
    public decimal OtherBox1Deductions { get; set; }
    public decimal AssessmentIncome { get; set; }
}
