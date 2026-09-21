namespace ChildcareCalculator.Application.Services;

public static class CalculationConstants
{
    // Number of working days in a year (standard for Dutch calculations)
    public const int WorkingDaysPerYear = 261;

    // Number of weeks in a year
    public const int WeeksPerYear = 52;

    // Number of months in a year
    public const int MonthsPerYear = 12;

    // Precision for decimal calculations
    public const int DecimalPrecision = 28;
    public const int DisplayPrecision = 2;

    // Rounding mode for financial calculations
    public const MidpointRounding RoundingMode = MidpointRounding.AwayFromZero;
}
