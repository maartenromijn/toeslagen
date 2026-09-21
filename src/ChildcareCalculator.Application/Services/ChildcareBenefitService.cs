using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Application.Services.Dtos;

namespace ChildcareCalculator.Application.Services;

public class ChildcareBenefitService
{
    private readonly RoundingService _roundingService;

    public ChildcareBenefitService(RoundingService roundingService)
    {
        _roundingService = roundingService;
    }

    /// <summary>
    /// Calculate childcare benefit percentage based on assessment income
    /// Formula from JSON: year == 2027 ? (assessment_income <= 71903 ? 0.96 : min(0.96, base_kot_percentage + (assessment_income <= 175430 ? 0.051 : 0.026))) : min(0.96, base_kot_percentage + policy_surcharge)
    /// </summary>
    public decimal CalculateAutomaticChildcareBenefitPercentage(
        int year,
        decimal assessmentIncome,
        decimal basePercentage,
        decimal policySurcharge,
        decimal incomeBoundaryIndexFactor)
    {
        // Adjust assessment income to 2026 boundaries
        var incomeAt2026Boundaries = assessmentIncome / incomeBoundaryIndexFactor;

        if (year == 2027)
        {
            if (assessmentIncome <= 71903)
            {
                return 0.96m; // 96% for incomes <= 71903
            }
            else if (assessmentIncome <= 175430)
            {
                return Math.Min(0.96m, basePercentage + 0.051m); // +5.1 percentage points
            }
            else
            {
                return Math.Min(0.96m, basePercentage + 0.026m); // +2.6 percentage points
            }
        }
        else
        {
            // For other years: base_percentage + policy_surcharge, capped at 96%
            return Math.Min(0.96m, basePercentage + policySurcharge);
        }
    }

    /// <summary>
    /// Get the applied childcare benefit percentage (with override if provided)
    /// Formula: coalesce(kot_percentage_override, automatic_kot_percentage)
    /// </summary>
    public decimal GetAppliedBenefitPercentage(decimal? overridePercentage, decimal automaticPercentage)
    {
        return overridePercentage ?? automaticPercentage;
    }

    /// <summary>
    /// Calculate childcare benefit amount
    /// Formula: eligible_childcare_cost * used_kot_percentage
    /// </summary>
    public decimal CalculateChildcareBenefitAmount(
        decimal eligibleChildcareCost,
        decimal appliedBenefitPercentage)
    {
        return _roundingService.RoundToNearestCent(eligibleChildcareCost * appliedBenefitPercentage);
    }

    /// <summary>
    /// Calculate own contribution (eigen bijdrage)
    /// Formula: actual_childcare_cost - childcare_allowance
    /// </summary>
    public decimal CalculateOwnContributionYear(
        decimal actualChildcareCost,
        decimal childcareBenefitAmount)
    {
        return _roundingService.RoundToNearestCent(actualChildcareCost - childcareBenefitAmount);
    }

    /// <summary>
    /// Calculate monthly own contribution
    /// Formula: own_contribution_year / 12
    /// </summary>
    public decimal CalculateOwnContributionMonth(decimal ownContributionYear)
    {
        return _roundingService.RoundToNearestCent(ownContributionYear / CalculationConstants.MonthsPerYear);
    }

    /// <summary>
    /// Calculate childcare benefit for a year
    /// </summary>
    public ChildcareBenefitResult CalculateChildcareBenefit(ChildcareBenefitInput input)
    {
        var result = new ChildcareBenefitResult
        {
            Year = input.Year,
            EligibleChildcareCost = input.EligibleChildcareCost,
            ActualChildcareCost = input.ActualChildcareCost,
            BasePercentage = input.BasePercentage,
            PolicySurcharge = input.PolicySurcharge,
            IncomeBoundaryIndexFactor = input.IncomeBoundaryIndexFactor,
            AssessmentIncome = input.AssessmentIncome,
            OverridePercentage = input.OverridePercentage
        };

        // Calculate automatic percentage
        result.AutomaticPercentage = CalculateAutomaticChildcareBenefitPercentage(
            input.Year,
            input.AssessmentIncome,
            input.BasePercentage,
            input.PolicySurcharge,
            input.IncomeBoundaryIndexFactor);

        // Get applied percentage (with override)
        result.AppliedPercentage = GetAppliedBenefitPercentage(
            input.OverridePercentage,
            result.AutomaticPercentage);

        // Calculate benefit amount
        result.ChildcareBenefitAmount = CalculateChildcareBenefitAmount(
            input.EligibleChildcareCost,
            result.AppliedPercentage);

        // Calculate own contribution
        result.OwnContributionYear = CalculateOwnContributionYear(
            input.ActualChildcareCost,
            result.ChildcareBenefitAmount);

        result.OwnContributionMonth = CalculateOwnContributionMonth(result.OwnContributionYear);

        // Add warnings if applicable
        if (input.OverridePercentage.HasValue)
        {
            result.Warnings.Add("Using manual override for childcare benefit percentage");
        }

        if (result.AppliedPercentage > 0.96m)
        {
            result.Warnings.Add("Benefit percentage capped at 96%");
        }

        return result;
    }

    /// <summary>
    /// Lookup childcare benefit percentage from band table
    /// </summary>
    public decimal LookupBandPercentage(decimal incomeAt2026Boundaries, List<ChildcareBenefitBand> bands)
    {
        if (bands.Count == 0)
            return 0;

        foreach (var band in bands.OrderBy(b => b.MinIncome))
        {
            if (incomeAt2026Boundaries >= band.MinIncome && 
                (band.MaxIncome == 0 || incomeAt2026Boundaries <= band.MaxIncome))
            {
                return band.FirstChildPercentage;
            }
        }

        // If no band found, return 0
        return 0;
    }
}

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
