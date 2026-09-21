using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class CalculationExplanation : EntityBase
{
    public string CalculationId { get; set; } = string.Empty;
    public string FormulaId { get; set; } = string.Empty;
    public string FormulaExpression { get; set; } = string.Empty;
    public string FormulaDescription { get; set; } = string.Empty;

    // Input values used
    public string InputValuesJson { get; set; } = string.Empty;

    // Intermediate results
    public string IntermediateResultsJson { get; set; } = string.Empty;

    // Result
    public decimal ResultValue { get; set; }
    public string ResultDescription { get; set; } = string.Empty;

    // Metadata
    public string? RoundingRule { get; set; }
    public bool IsProvisional { get; set; }
    public string? WarningMessage { get; set; }
    public string? PolicyVersion { get; set; }

    // Navigation
    public Guid AnnualCalculationId { get; set; }
    public virtual AnnualCalculation AnnualCalculation { get; set; } = null!;
}
