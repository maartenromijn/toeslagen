using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class LeavePlan : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public decimal ContractHoursPerWeek { get; set; }
    public string MaxPaidParentalLeaveFormula { get; set; } = string.Empty;
    public DateOnly ChildBirthDate { get; set; }
    public DateOnly PaidLeaveLastDate { get; set; }
    public decimal WorkHours3Days { get; set; }
    public decimal WorkHours4Days { get; set; }
    public decimal WorkHoursAfterPaidLeave { get; set; }

    // Navigation
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<LeavePhase> Phases { get; set; } = new List<LeavePhase>();
}
