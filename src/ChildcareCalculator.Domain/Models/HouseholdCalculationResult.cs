using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class HouseholdCalculationResult : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public DateTime CalculationDate { get; set; } = DateTime.UtcNow;

    // Summary values
    public decimal TotalAssessmentIncome { get; set; }
    public decimal TotalChildcareCost { get; set; }
    public decimal TotalEligibleChildcareCost { get; set; }
    public decimal TotalChildcareBenefit { get; set; }
    public decimal TotalOwnContributionYear { get; set; }
    public decimal TotalOwnContributionMonth { get; set; }
    public decimal TotalNetIncome { get; set; }

    // Navigation
    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;

    public virtual ICollection<AnnualCalculation> AnnualCalculations { get; set; } = new List<AnnualCalculation>();
}
