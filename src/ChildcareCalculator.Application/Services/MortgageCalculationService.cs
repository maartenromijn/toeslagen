using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Application.Services.Dtos;

namespace ChildcareCalculator.Application.Services;

public class MortgageCalculationService
{
    private readonly RoundingService _roundingService;

    public MortgageCalculationService(RoundingService roundingService)
    {
        _roundingService = roundingService;
    }

    /// <summary>
    /// Calculate monthly mortgage schedule for a mortgage part
    /// </summary>
    public List<MonthlyMortgageResultDto> CalculateMonthlySchedule(MortgagePart mortgagePart, DateOnly startDate, int yearsToCalculate)
    {
        var results = new List<MonthlyMortgageResultDto>();
        var currentBalance = mortgagePart.OutstandingAtReference;
        var currentDate = startDate;

        for (int year = 0; year < yearsToCalculate; year++)
        {
            for (int month = 1; month <= CalculationConstants.MonthsPerYear; month++)
            {
                // Check if we've reached the end date
                if (currentDate.Year > mortgagePart.EndDate.Year ||
                    (currentDate.Year == mortgagePart.EndDate.Year && currentDate.Month > mortgagePart.EndDate.Month))
                {
                    break;
                }

                // Calculate monthly interest
                var monthlyInterest = currentBalance * mortgagePart.InterestRate / CalculationConstants.MonthsPerYear;

                // Calculate monthly principal payment
                var monthlyPrincipal = Math.Min(
                    currentBalance,
                    mortgagePart.RegularPaymentMonthly + mortgagePart.ExtraPaymentMonthly - monthlyInterest);

                // Calculate closing balance
                var closingBalance = currentBalance - monthlyPrincipal;

                results.Add(new MonthlyMortgageResultDto
                {
                    Year = currentDate.Year,
                    Month = currentDate.Month,
                    OpeningBalance = _roundingService.RoundToNearestCent(currentBalance),
                    MonthlyInterest = _roundingService.RoundToNearestCent(monthlyInterest),
                    MonthlyPrincipal = _roundingService.RoundToNearestCent(monthlyPrincipal),
                    ClosingBalance = _roundingService.RoundToNearestCent(closingBalance)
                });

                currentBalance = closingBalance;

                // Move to next month
                currentDate = currentDate.AddMonths(1);

                // If balance reaches zero, stop
                if (currentBalance <= 0)
                    break;
            }

            if (currentBalance <= 0)
                break;
        }

        return results;
    }

    /// <summary>
    /// Calculate annual mortgage results from monthly schedule
    /// </summary>
    public List<AnnualMortgageResultDto> CalculateAnnualResults(List<MonthlyMortgageResultDto> monthlyResults)
    {
        var annualResults = new List<AnnualMortgageResultDto>();

        var groupedByYear = monthlyResults.GroupBy(m => m.Year);

        foreach (var yearGroup in groupedByYear)
        {
            var year = yearGroup.Key;
            var monthlyData = yearGroup.OrderBy(m => m.Month).ToList();

            if (monthlyData.Count == 0)
                continue;

            var openingBalance = monthlyData.First().OpeningBalance;
            var closingBalance = monthlyData.Last().ClosingBalance;
            var totalInterestPaid = monthlyData.Sum(m => m.MonthlyInterest);
            var totalPrincipalPaid = monthlyData.Sum(m => m.MonthlyPrincipal);

            // Calculate average balance
            var averageBalance = (openingBalance + closingBalance) / 2;

            annualResults.Add(new AnnualMortgageResultDto
            {
                Year = year,
                OpeningBalance = _roundingService.RoundToNearestCent(openingBalance),
                ClosingBalance = _roundingService.RoundToNearestCent(closingBalance),
                TotalInterestPaid = _roundingService.RoundToNearestCent(totalInterestPaid),
                TotalPrincipalPaid = _roundingService.RoundToNearestCent(totalPrincipalPaid),
                AverageBalance = _roundingService.RoundToNearestCent(averageBalance),
                DeductibleInterest = _roundingService.RoundToNearestCent(totalInterestPaid)
            });
        }

        return annualResults;
    }

    /// <summary>
    /// Calculate mortgage results for all parts and aggregate
    /// </summary>
    public MortgageCalculationResult CalculateMortgage(Mortgage mortgage, DateOnly calculationStartDate, int yearsToCalculate)
    {
        var result = new MortgageCalculationResult
        {
            MortgageId = mortgage.Id,
            CalculationStartDate = calculationStartDate,
            YearsToCalculate = yearsToCalculate
        };

        foreach (var part in mortgage.Parts)
        {
            var monthlyResults = CalculateMonthlySchedule(part, calculationStartDate, yearsToCalculate);
            var annualResults = CalculateAnnualResults(monthlyResults);

            result.PartResults.Add(new MortgagePartCalculationResult
            {
                PartId = part.Id,
                PartNumber = part.PartNumber,
                MonthlyResults = monthlyResults,
                AnnualResults = annualResults
            });
        }

        // Aggregate results across all parts
        result.AggregatedAnnualResults = AggregateAnnualResults(result.PartResults);

        return result;
    }

    /// <summary>
    /// Aggregate annual results across all mortgage parts
    /// </summary>
    private List<AnnualMortgageResultDto> AggregateAnnualResults(List<MortgagePartCalculationResult> partResults)
    {
        var aggregated = new List<AnnualMortgageResultDto>();

        // Get all unique years
        var allYears = partResults
            .SelectMany(p => p.AnnualResults.Select(a => a.Year))
            .Distinct()
            .OrderBy(y => y);

        foreach (var year in allYears)
        {
            var yearResults = partResults
                .SelectMany(p => p.AnnualResults.Where(a => a.Year == year))
                .ToList();

            if (yearResults.Count == 0)
                continue;

            var totalOpeningBalance = yearResults.Sum(r => r.OpeningBalance);
            var totalClosingBalance = yearResults.Sum(r => r.ClosingBalance);
            var totalInterestPaid = yearResults.Sum(r => r.TotalInterestPaid);
            var totalPrincipalPaid = yearResults.Sum(r => r.TotalPrincipalPaid);
            var averageBalance = yearResults.Average(r => r.AverageBalance);

            aggregated.Add(new AnnualMortgageResultDto
            {
                Year = year,
                OpeningBalance = _roundingService.RoundToNearestCent(totalOpeningBalance),
                ClosingBalance = _roundingService.RoundToNearestCent(totalClosingBalance),
                TotalInterestPaid = _roundingService.RoundToNearestCent(totalInterestPaid),
                TotalPrincipalPaid = _roundingService.RoundToNearestCent(totalPrincipalPaid),
                AverageBalance = _roundingService.RoundToNearestCent(averageBalance),
                DeductibleInterest = _roundingService.RoundToNearestCent(totalInterestPaid)
            });
        }

        return aggregated;
    }

    /// <summary>
    /// Calculate deductible mortgage interest for a year
    /// </summary>
    public decimal CalculateDeductibleMortgageInterest(List<AnnualMortgageResultDto> annualResults, int year)
    {
        var yearResult = annualResults.FirstOrDefault(r => r.Year == year);
        return yearResult?.DeductibleInterest ?? 0;
    }
}
