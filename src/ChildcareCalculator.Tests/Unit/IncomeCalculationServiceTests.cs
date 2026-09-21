using Xunit;
using ChildcareCalculator.Application.Services;
using ChildcareCalculator.Application.Services.Dtos;
using ChildcareCalculator.Domain.Enums;

namespace ChildcareCalculator.Tests.Unit;

public class IncomeCalculationServiceTests
{
    private readonly IncomeCalculationService _service;
    private readonly RoundingService _roundingService;

    public IncomeCalculationServiceTests()
    {
        _roundingService = new RoundingService();
        _service = new IncomeCalculationService(_roundingService);
    }

    [Theory]
    [InlineData(40, 2080)] // 40 hours/week * 52 weeks
    [InlineData(38, 1976)]
    [InlineData(0, 0)]
    public void CalculateAnnualContractHours_ShouldReturnCorrectValue(decimal contractHoursPerWeek, decimal expected)
    {
        var result = _service.CalculateAnnualContractHours(contractHoursPerWeek);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5842.81, 0.08, 268.78)] // Maarten's data from JSON
    [InlineData(3300, 0.08, 153.85)] // Gina's data from JSON
    [InlineData(0, 0, 0)]
    public void CalculateNormalDailyWage_ShouldReturnCorrectValue(
        decimal monthlySalary, 
        decimal holidayAllowancePercentage, 
        decimal expectedRounded)
    {
        var result = _service.CalculateNormalDailyWage(monthlySalary, holidayAllowancePercentage);
        // Round to 2 decimals for comparison
        var roundedResult = decimal.Round(result, 2);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Theory]
    [InlineData(268.78, 309.91, 0.7, 188.15)] // normal_daily_wage < uwv_max, so use normal_daily_wage
    [InlineData(400, 309.91, 0.7, 216.94)] // normal_daily_wage > uwv_max, so use uwv_max
    [InlineData(0, 309.91, 0.7, 0)]
    public void CalculateWazoDailyBenefit_ShouldReturnCorrectValue(
        decimal normalDailyWage, 
        decimal uwvMaximumDailyWage, 
        decimal uwvWazoPercentage,
        decimal expectedRounded)
    {
        var result = _service.CalculateWazoDailyBenefit(normalDailyWage, uwvMaximumDailyWage, uwvWazoPercentage);
        var roundedResult = decimal.Round(result, 2);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Theory]
    [InlineData(188.15, 268.78, 0.7)] // 188.15 / 268.78 = 0.7
    [InlineData(216.94, 400, 0.54235)] // 216.94 / 400 = 0.54235
    [InlineData(0, 100, 0)]
    public void CalculateWazoReplacementRatio_ShouldReturnCorrectValue(
        decimal wazoDailyBenefit, 
        decimal normalDailyWage,
        decimal expectedRounded)
    {
        var result = _service.CalculateWazoReplacementRatio(wazoDailyBenefit, normalDailyWage);
        var roundedResult = decimal.Round(result, 5);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Fact]
    public void CalculateWazoReplacementRatio_WithZeroNormalDailyWage_ShouldReturnZero()
    {
        var result = _service.CalculateWazoReplacementRatio(100, 0);
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData(70000, 2080, 0, 0, 0, 8, 70000)] // No leave, full salary
    [InlineData(70000, 2080, 1040, 0, 0, 8, 65000)] // 1040 unpaid leave hours
    [InlineData(70000, 2080, 0, 1040, 188.15, 8, 70000)] // 1040 paid WAZO hours, but no reduction in salary
    public void CalculateSalaryAfterLeave_ShouldReturnCorrectValue(
        decimal annualSalary,
        decimal annualContractHours,
        decimal unpaidLeaveHours,
        decimal paidWazoHours,
        decimal wazoDailyBenefit,
        decimal hoursPerDay,
        decimal expectedRounded)
    {
        var result = _service.CalculateSalaryAfterLeave(
            annualSalary, annualContractHours, unpaidLeaveHours, paidWazoHours, wazoDailyBenefit, hoursPerDay);
        var roundedResult = decimal.Round(result, 0);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Fact]
    public void CalculateSalaryAfterLeave_WithZeroHoursPerDay_ShouldReturnAnnualSalary()
    {
        var result = _service.CalculateSalaryAfterLeave(70000, 2080, 100, 100, 100, 0);
        Assert.Equal(70000, result);
    }

    [Theory]
    [InlineData(2080, 0, 0, 0.7, 1)] // No leave
    [InlineData(2080, 1040, 0, 0.7, 0.5)] // 50% unpaid leave
    [InlineData(2080, 0, 1040, 0.7, 0.85)] // 50% paid WAZO leave with 70% replacement
    public void CalculateProfitFactorAfterLeave_ShouldReturnCorrectValue(
        decimal annualContractHours,
        decimal unpaidLeaveHours,
        decimal paidWazoHours,
        decimal wazoReplacementRatio,
        decimal expectedRounded)
    {
        var result = _service.CalculateProfitFactorAfterLeave(
            annualContractHours, unpaidLeaveHours, paidWazoHours, wazoReplacementRatio);
        var roundedResult = decimal.Round(result, 2);
        Assert.Equal(expectedRounded, roundedResult);
    }

    [Theory]
    [InlineData(2080, 0, 0, 1)] // No leave
    [InlineData(2080, 1040, 0, 0.5)] // 50% leave
    [InlineData(2080, 520, 520, 0)] // 50% leave
    public void CalculateExcessProfitFactorAfterLeave_ShouldReturnCorrectValue(
        decimal annualContractHours,
        decimal unpaidLeaveHours,
        decimal paidWazoHours,
        decimal expected)
    {
        var result = _service.CalculateExcessProfitFactorAfterLeave(
            annualContractHours, unpaidLeaveHours, paidWazoHours);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateTaxableIncomeBeforeHome_ShouldReturnCorrectValue()
    {
        // Test with Maarten's data from JSON
        var employerSalaryAfterLeave = 5842.81m * 12; // Annual salary
        var holidayAllowance = employerSalaryAfterLeave * 0.08m;
        var wazoBenefit = 0m; // No WAZO benefit in this case
        var carBenefitMonthly = 689.58m;
        var taxableAllowanceMonthly = 67.01m;
        var profitAfterLeave = 4250m;
        var excessProfitAfterLeave = 4182.13m;
        var otherTaxableIncome = 0m;
        var manualCorrection = 0m;
        var pensionPremiumMonthly = 27.2m;
        var wiaWgaPremiumMonthly = 15.8m;

        var result = _service.CalculateTaxableIncomeBeforeHome(
            employerSalaryAfterLeave,
            holidayAllowance,
            wazoBenefit,
            carBenefitMonthly,
            taxableAllowanceMonthly,
            profitAfterLeave,
            excessProfitAfterLeave,
            otherTaxableIncome,
            manualCorrection,
            pensionPremiumMonthly,
            wiaWgaPremiumMonthly);

        // Expected: salary + holiday + car*12 + allowance*12 + profit + excess_profit - pension*12 - wia*12
        var expected = employerSalaryAfterLeave
            + holidayAllowance
            + (carBenefitMonthly * 12)
            + (taxableAllowanceMonthly * 12)
            + profitAfterLeave
            + excessProfitAfterLeave
            - (pensionPremiumMonthly * 12)
            - (wiaWgaPremiumMonthly * 12);

        var roundedResult = decimal.Round(result, 2);
        var roundedExpected = decimal.Round(expected, 2);
        Assert.Equal(roundedExpected, roundedResult);
    }

    [Fact]
    public void CalculateAnnualPersonIncome_WithNoLeave_ShouldReturnCorrectValues()
    {
        var input = new AnnualPersonIncomeInput
        {
            Year = 2027,
            PersonName = "Test Person",
            ContractHoursPerWeek = 40,
            MonthlySalary = 5000,
            HolidayAllowancePercentage = 0.08m,
            CarBenefitMonthly = 100,
            TaxableCarAllowanceMonthly = 50,
            ProfitDistributionFullYear = 5000,
            ExcessProfitFullYear = 2000,
            PensionPremiumMonthly = 100,
            WiaWgaPremiumMonthly = 50,
            ManualTaxableIncomeCorrection = 0,
            OtherTaxableIncome = 0,
            UwvMaximumDailyWage = 309.91m,
            UwvWazoPercentage = 0.7m,
            LeavePhases = new List<LeavePhaseDto>() // No leave
        };

        var result = _service.CalculateAnnualPersonIncome(input);

        // Verify base calculations
        Assert.Equal(2080, result.AnnualContractHours);
        Assert.NotEqual(0, result.NormalDailyWage);
        Assert.NotEqual(0, result.WazoDailyBenefit);
        Assert.Equal(0.7m, decimal.Round(result.WazoReplacementRatio, 5));

        // Verify salary components
        Assert.NotEqual(0, result.SalaryAfterLeave);
        Assert.NotEqual(0, result.HolidayAllowance);

        // Verify final taxable income
        Assert.NotEqual(0, result.TaxableIncome);
    }
}
