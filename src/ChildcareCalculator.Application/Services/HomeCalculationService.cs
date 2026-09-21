using ChildcareCalculator.Domain.Models;

namespace ChildcareCalculator.Application.Services;

public class HomeCalculationService
{
    private readonly RoundingService _roundingService;

    public HomeCalculationService(RoundingService roundingService)
    {
        _roundingService = roundingService;
    }

    /// <summary>
    /// Calculate home imputed income (eigenwoningforfait)
    /// Formula: woz_value * imputed_rental_value_pct
    /// </summary>
    public decimal CalculateHomeImputedIncome(decimal wozValue, decimal imputedRentalValuePercentage)
    {
        return _roundingService.RoundToNearestCent(wozValue * imputedRentalValuePercentage);
    }

    /// <summary>
    /// Calculate home balance (saldo eigen woning)
    /// Formula: home_imputed_income - deductible_mortgage_interest
    /// </summary>
    public decimal CalculateHomeBalance(decimal homeImputedIncome, decimal deductibleMortgageInterest)
    {
        return _roundingService.RoundToNearestCent(homeImputedIncome - deductibleMortgageInterest);
    }

    /// <summary>
    /// Calculate home settings for a year
    /// </summary>
    public HomeSettings CalculateHomeSettings(
        decimal wozValue,
        decimal imputedRentalValuePercentage,
        decimal otherBox1Deductions,
        decimal deductibleMortgageInterest)
    {
        var homeImputedIncome = CalculateHomeImputedIncome(wozValue, imputedRentalValuePercentage);
        var homeBalance = CalculateHomeBalance(homeImputedIncome, deductibleMortgageInterest);

        return new HomeSettings
        {
            WozValue = wozValue,
            ImputedRentalValuePercentage = imputedRentalValuePercentage,
            OtherBox1Deductions = otherBox1Deductions,
            HomeImputedIncome = homeImputedIncome
        };
    }
}
