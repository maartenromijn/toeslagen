using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class AnnualCalculation : EntityBase
{
    public int Year { get; set; }
    public CalculationStatus Status { get; set; }

    // Assessment income
    public decimal AssessmentIncome { get; set; }
    public decimal Person1TaxableIncome { get; set; }
    public decimal Person2TaxableIncome { get; set; }
    public decimal HomeImputedIncome { get; set; }
    public decimal DeductibleMortgageInterest { get; set; }
    public decimal OtherBox1Deductions { get; set; }

    // Childcare
    public decimal ChildcareBenefitPercentage { get; set; }
    public decimal? ChildcareBenefitOverride { get; set; }
    public decimal ChildcareBenefitAmount { get; set; }

    // Net income estimation
    public decimal EstimatedNetIncome { get; set; }

    // Navigation
    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;

    public virtual ICollection<CalculationExplanation> Explanations { get; set; } = new List<CalculationExplanation>();
}
