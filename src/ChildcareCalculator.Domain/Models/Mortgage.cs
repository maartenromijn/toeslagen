using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class Mortgage : EntityBase
{
    public DateOnly ReferenceDate { get; set; }
    public string CalculationMethod { get; set; } = string.Empty;

    // Navigation
    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;

    public virtual ICollection<MortgagePart> Parts { get; set; } = new List<MortgagePart>();
}
