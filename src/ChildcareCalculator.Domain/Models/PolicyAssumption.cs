using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class PolicyAssumption : EntityBase
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PolicyValueStatus Status { get; set; }
    public string? Value { get; set; }
    public int? Year { get; set; }
    public string? Unit { get; set; }
    public string? Source { get; set; }

    // Navigation
    public Guid? HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario? HouseholdScenario { get; set; }
}
