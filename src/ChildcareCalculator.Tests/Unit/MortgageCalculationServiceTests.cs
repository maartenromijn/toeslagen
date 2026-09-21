using Xunit;
using ChildcareCalculator.Application.Services;
using ChildcareCalculator.Domain.Models;

namespace ChildcareCalculator.Tests.Unit;

public class MortgageCalculationServiceTests
{
    private readonly MortgageCalculationService _service;
    private readonly RoundingService _roundingService;

    public MortgageCalculationServiceTests()
    {
        _roundingService = new RoundingService();
        _service = new MortgageCalculationService(_roundingService);
    }

    [Fact]
    public void CalculateMonthlySchedule_ShouldReturnCorrectMonthlyResults()
    {
        // Create a mortgage part
        var mortgagePart = new MortgagePart
        {
            PartNumber = 1,
            OriginalPrincipal = 200000,
            OutstandingAtReference = 200000,
            InterestRate = 0.035m, // 3.5%
            RegularPaymentMonthly = 1000,
            ExtraPaymentMonthly = 0,
            RateRevisionDate = new DateOnly(2030, 1, 1),
            EndDate = new DateOnly(2050, 1, 1)
        };

        var startDate = new DateOnly(2027, 1, 1);
        var yearsToCalculate = 5;

        var results = _service.CalculateMonthlySchedule(mortgagePart, startDate, yearsToCalculate);

        // Verify results
        Assert.NotEmpty(results);
        Assert.Equal(yearsToCalculate * 12, results.Count);

        // Check first month
        var firstMonth = results[0];
        Assert.Equal(2027, firstMonth.Year);
        Assert.Equal(1, firstMonth.Month);
        Assert.Equal(200000, firstMonth.OpeningBalance);

        // Calculate expected interest for first month
        var expectedInterest = 200000 * 0.035m / 12;
        Assert.Equal(_roundingService.RoundToNearestCent(expectedInterest), firstMonth.MonthlyInterest);

        // Calculate expected principal for first month
        var expectedPrincipal = 1000 - expectedInterest;
        Assert.Equal(_roundingService.RoundToNearestCent(expectedPrincipal), firstMonth.MonthlyPrincipal);

        // Check that balance decreases
        var secondMonth = results[1];
        Assert.Equal(firstMonth.ClosingBalance, secondMonth.OpeningBalance);
        Assert.True(secondMonth.OpeningBalance < firstMonth.OpeningBalance);
    }

    [Fact]
    public void CalculateMonthlySchedule_WithExtraPayment_ShouldApplyExtraPayment()
    {
        var mortgagePart = new MortgagePart
        {
            PartNumber = 1,
            OriginalPrincipal = 100000,
            OutstandingAtReference = 100000,
            InterestRate = 0.04m, // 4%
            RegularPaymentMonthly = 500,
            ExtraPaymentMonthly = 100,
            RateRevisionDate = new DateOnly(2030, 1, 1),
            EndDate = new DateOnly(2040, 1, 1)
        };

        var startDate = new DateOnly(2027, 1, 1);
        var yearsToCalculate = 3;

        var results = _service.CalculateMonthlySchedule(mortgagePart, startDate, yearsToCalculate);

        // Check first month
        var firstMonth = results[0];
        var expectedInterest = 100000 * 0.04m / 12;
        var expectedPrincipal = 500 + 100 - expectedInterest; // regular + extra - interest
        
        Assert.Equal(_roundingService.RoundToNearestCent(expectedPrincipal), firstMonth.MonthlyPrincipal);
    }

    [Fact]
    public void CalculateMonthlySchedule_ShouldStopWhenBalanceReachesZero()
    {
        var mortgagePart = new MortgagePart
        {
            PartNumber = 1,
            OriginalPrincipal = 1000,
            OutstandingAtReference = 1000,
            InterestRate = 0.0m, // 0% interest for simplicity
            RegularPaymentMonthly = 500,
            ExtraPaymentMonthly = 0,
            RateRevisionDate = new DateOnly(2030, 1, 1),
            EndDate = new DateOnly(2040, 1, 1)
        };

        var startDate = new DateOnly(2027, 1, 1);
        var yearsToCalculate = 10;

        var results = _service.CalculateMonthlySchedule(mortgagePart, startDate, yearsToCalculate);

        // Should stop after 2 months when balance reaches zero
        Assert.Equal(2, results.Count);

        // Second month should have closing balance of 0
        var secondMonth = results[1];
        Assert.Equal(0, secondMonth.ClosingBalance);
    }

    [Fact]
    public void CalculateAnnualResults_ShouldAggregateMonthlyResults()
    {
        // Create monthly results
        var monthlyResults = new List<MortgageCalculationService.MonthlyMortgageResultDto>
        {
            new MortgageCalculationService.MonthlyMortgageResultDto
            {
                Year = 2027,
                Month = 1,
                OpeningBalance = 100000,
                MonthlyInterest = 300,
                MonthlyPrincipal = 200,
                ClosingBalance = 99800
            },
            new MortgageCalculationService.MonthlyMortgageResultDto
            {
                Year = 2027,
                Month = 2,
                OpeningBalance = 99800,
                MonthlyInterest = 299.40m,
                MonthlyPrincipal = 200.60m,
                ClosingBalance = 99600
            },
            // Add more months for a full year...
            new MortgageCalculationService.MonthlyMortgageResultDto
            {
                Year = 2027,
                Month = 12,
                OpeningBalance = 95000,
                MonthlyInterest = 285,
                MonthlyPrincipal = 215,
                ClosingBalance = 94785
            }
        };

        // For simplicity, let's just test with 2 months
        var annualResults = _service.CalculateAnnualResults(monthlyResults);

        Assert.Single(annualResults);
        var result = annualResults[0];

        Assert.Equal(2027, result.Year);
        Assert.Equal(100000, result.OpeningBalance);
        Assert.Equal(99600, result.ClosingBalance);
        Assert.Equal(300 + 299.40m, result.TotalInterestPaid);
        Assert.Equal(200 + 200.60m, result.TotalPrincipalPaid);
        Assert.Equal((100000 + 99600) / 2, result.AverageBalance);
    }

    [Fact]
    public void CalculateMortgage_ShouldHandleMultipleParts()
    {
        var mortgage = new Mortgage
        {
            ReferenceDate = new DateOnly(2026, 9, 1),
            CalculationMethod = "monthly_annuity_schedule",
            Parts = new List<MortgagePart>
            {
                new MortgagePart
                {
                    PartNumber = 1,
                    OriginalPrincipal = 100000,
                    OutstandingAtReference = 100000,
                    InterestRate = 0.03m,
                    RegularPaymentMonthly = 500,
                    ExtraPaymentMonthly = 0,
                    RateRevisionDate = new DateOnly(2030, 1, 1),
                    EndDate = new DateOnly(2040, 1, 1)
                },
                new MortgagePart
                {
                    PartNumber = 2,
                    OriginalPrincipal = 50000,
                    OutstandingAtReference = 50000,
                    InterestRate = 0.04m,
                    RegularPaymentMonthly = 300,
                    ExtraPaymentMonthly = 0,
                    RateRevisionDate = new DateOnly(2030, 1, 1),
                    EndDate = new DateOnly(2040, 1, 1)
                }
            }
        };

        var startDate = new DateOnly(2027, 1, 1);
        var yearsToCalculate = 3;

        var result = _service.CalculateMortgage(mortgage, startDate, yearsToCalculate);

        // Verify results
        Assert.NotNull(result);
        Assert.Equal(2, result.PartResults.Count);
        Assert.NotEmpty(result.AggregatedAnnualResults);

        // Verify each part has results
        foreach (var partResult in result.PartResults)
        {
            Assert.NotEmpty(partResult.MonthlyResults);
            Assert.NotEmpty(partResult.AnnualResults);
        }
    }

    [Fact]
    public void CalculateDeductibleMortgageInterest_ShouldReturnCorrectValue()
    {
        var annualResults = new List<MortgageCalculationService.AnnualMortgageResultDto>
        {
            new MortgageCalculationService.AnnualMortgageResultDto
            {
                Year = 2027,
                DeductibleInterest = 3600
            },
            new MortgageCalculationService.AnnualMortgageResultDto
            {
                Year = 2028,
                DeductibleInterest = 3400
            }
        };

        var interest2027 = _service.CalculateDeductibleMortgageInterest(annualResults, 2027);
        var interest2028 = _service.CalculateDeductibleMortgageInterest(annualResults, 2028);
        var interest2029 = _service.CalculateDeductibleMortgageInterest(annualResults, 2029);

        Assert.Equal(3600, interest2027);
        Assert.Equal(3400, interest2028);
        Assert.Equal(0, interest2029); // Not in list
    }

    [Fact]
    public void CalculateMonthlySchedule_ShouldStopAtEndDate()
    {
        var mortgagePart = new MortgagePart
        {
            PartNumber = 1,
            OriginalPrincipal = 200000,
            OutstandingAtReference = 200000,
            InterestRate = 0.035m,
            RegularPaymentMonthly = 1000,
            ExtraPaymentMonthly = 0,
            RateRevisionDate = new DateOnly(2030, 1, 1),
            EndDate = new DateOnly(2028, 6, 1) // End date in middle of 2028
        };

        var startDate = new DateOnly(2027, 1, 1);
        var yearsToCalculate = 10;

        var results = _service.CalculateMonthlySchedule(mortgagePart, startDate, yearsToCalculate);

        // Should stop at June 2028 (17 months: Jan 2027 - May 2028 = 17 months)
        // Actually, let's calculate: Jan 2027 to May 2028 is 17 months
        // But end date is June 1, 2028, so May 2028 is the last full month
        Assert.Equal(17, results.Count);
    }
}
