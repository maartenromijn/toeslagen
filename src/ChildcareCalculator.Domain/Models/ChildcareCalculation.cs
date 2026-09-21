using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class ChildcareCalculation : EntityBase
{
    public int Year { get; set; }

    // Calculated values
    public decimal ChildcareHoursPerYear { get; set; }
    public decimal ActualChildcareCost { get; set; }
    public decimal EligibleChildcareCost { get; set; }
    public decimal ChildcareBenefitPercentage { get; set; }
    public decimal ChildcareBenefitAmount { get; set; }
    public decimal OwnContributionYear { get; set; }
    public decimal OwnContributionMonth { get; set; }

    // Navigation
    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;
}
