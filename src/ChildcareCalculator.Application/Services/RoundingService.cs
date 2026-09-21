using ChildcareCalculator.Domain.Models;

namespace ChildcareCalculator.Application.Services;

public class RoundingService
{
    private readonly int _decimalPlaces;

    public RoundingService(int decimalPlaces = CalculationConstants.DisplayPrecision)
    {
        _decimalPlaces = decimalPlaces;
    }

    public decimal Round(decimal value)
    {
        return decimal.Round(value, _decimalPlaces, CalculationConstants.RoundingMode);
    }

    public decimal RoundToNearestCent(decimal value)
    {
        return decimal.Round(value, 2, CalculationConstants.RoundingMode);
    }

    public decimal RoundToNearestEuro(decimal value)
    {
        return decimal.Round(value, 0, CalculationConstants.RoundingMode);
    }

    public decimal Truncate(decimal value, int decimalPlaces)
    {
        var factor = decimal.Pow(10, decimalPlaces);
        return decimal.Truncate(value * factor) / factor;
    }

    public decimal RoundPercentage(decimal percentage)
    {
        // Percentages are stored as decimals (0.01 = 1%), round to 4 decimal places for precision
        return decimal.Round(percentage, 4, CalculationConstants.RoundingMode);
    }
}
