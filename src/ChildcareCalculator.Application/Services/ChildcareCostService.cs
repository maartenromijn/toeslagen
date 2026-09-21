using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Application.Services.Dtos;

namespace ChildcareCalculator.Application.Services;

public class ChildcareCostService
{
    private readonly RoundingService _roundingService;

    public ChildcareCostService(RoundingService roundingService)
    {
        _roundingService = roundingService;
    }

    /// <summary>
    /// Calculate childcare hours per year
    /// Formula: childcare_weeks * days_per_week * hours_per_day
    /// </summary>
    public decimal CalculateChildcareHoursPerYear(int childcareWeeks, int daysPerWeek, decimal hoursPerDay)
    {
        return childcareWeeks * daysPerWeek * hoursPerDay;
    }

    /// <summary>
    /// Calculate actual childcare cost
    /// Formula: childcare_hours_year * actual_hourly_rate
    /// </summary>
    public decimal CalculateActualChildcareCost(decimal childcareHoursPerYear, decimal actualHourlyRate)
    {
        return _roundingService.RoundToNearestCent(childcareHoursPerYear * actualHourlyRate);
    }

    /// <summary>
    /// Calculate eligible childcare cost (subsidiabele kosten)
    /// Formula: childcare_hours_year * min(actual_hourly_rate, maximum_hourly_rate)
    /// </summary>
    public decimal CalculateEligibleChildcareCost(
        decimal childcareHoursPerYear,
        decimal actualHourlyRate,
        decimal maximumHourlyRate)
    {
        var cappedRate = Math.Min(actualHourlyRate, maximumHourlyRate);
        return _roundingService.RoundToNearestCent(childcareHoursPerYear * cappedRate);
    }

    /// <summary>
    /// Calculate childcare cost for a year
    /// </summary>
    public ChildcareCostResult CalculateChildcareCost(ChildcareCostInput input)
    {
        var result = new ChildcareCostResult
        {
            Year = input.Year,
            ChildcareWeeks = input.ChildcareWeeks,
            DaysPerWeek = input.DaysPerWeek,
            HoursPerDay = input.HoursPerDay,
            ActualHourlyRate = input.ActualHourlyRate,
            MaximumHourlyRate = input.MaximumHourlyRate
        };

        result.ChildcareHoursPerYear = CalculateChildcareHoursPerYear(
            input.ChildcareWeeks,
            input.DaysPerWeek,
            input.HoursPerDay);

        result.ActualChildcareCost = CalculateActualChildcareCost(
            result.ChildcareHoursPerYear,
            input.ActualHourlyRate);

        result.EligibleChildcareCost = CalculateEligibleChildcareCost(
            result.ChildcareHoursPerYear,
            input.ActualHourlyRate,
            input.MaximumHourlyRate);

        return result;
    }
}

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
