using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class LeavePhase : EntityBase
{
    public int PhaseNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal WorkHoursPerWeek { get; set; }
    public LeaveType LeaveType { get; set; }
    public decimal LeaveHoursPerWeek { get; set; }
    public decimal ExtraHours { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Guid LeavePlanId { get; set; }
    public virtual LeavePlan LeavePlan { get; set; } = null!;
}
