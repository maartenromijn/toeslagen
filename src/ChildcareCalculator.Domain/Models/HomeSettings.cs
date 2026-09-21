using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class HomeSettings : EntityBase
{
    public int Year { get; set; }
    public decimal WozValue { get; set; }
    public decimal ImputedRentalValuePercentage { get; set; }
    public decimal OtherBox1Deductions { get; set; }

    // Calculated
    public decimal HomeImputedIncome { get; set; }

    // Navigation
    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;
}
