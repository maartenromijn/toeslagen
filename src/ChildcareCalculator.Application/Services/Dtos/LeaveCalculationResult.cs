using ChildcareCalculator.Domain.Enums;

namespace ChildcareCalculator.Application.Services.Dtos;

public class LeaveCalculationResult
{
    public int Year { get; set; }
    public string LeavePlanName { get; set; } = string.Empty;
    public decimal ContractHoursPerWeek { get; set; }
    public decimal AnnualContractHours { get; set; }
    public decimal HoursPerDay { get; set; }
    public decimal PaidWazoHours { get; set; }
    public decimal UnpaidLeaveHours { get; set; }
    public decimal WorkedHours { get; set; }
    public List<LeavePhaseResult> Phases { get; set; } = new List<LeavePhaseResult>();
    public List<string> Warnings { get; set; } = new List<string>();
}

public class LeavePhaseResult
{
    public int PhaseNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public LeaveType LeaveType { get; set; }
    public decimal LeaveHoursPerWeek { get; set; }
    public decimal WorkHoursPerWeek { get; set; }
    public decimal WeeksInYear { get; set; }
    public decimal LeaveHoursInYear { get; set; }
    public decimal WorkHoursInYear { get; set; }
}
