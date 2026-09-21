using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class ChildcareSettings : EntityBase
{
    public int Year { get; set; }
    public int ChildcareWeeks { get; set; }
    public int DaysPerWeek { get; set; }
    public decimal HoursPerDay { get; set; }
    public decimal ActualHourlyRate { get; set; }
    public decimal MaximumHourlyRate { get; set; }
    public ChildcareType ChildcareType { get; set; }

    // Navigation
    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;
}
