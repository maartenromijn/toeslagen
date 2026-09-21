using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Application.Services.Dtos;

namespace ChildcareCalculator.Application.Services;

public class LeaveCalculationService
{
    private readonly RoundingService _roundingService;

    public LeaveCalculationService(RoundingService roundingService)
    {
        _roundingService = roundingService;
    }

    /// <summary>
    /// Calculate leave hours for a specific year from leave phases
    /// </summary>
    public LeaveCalculationResult CalculateLeaveForYear(int year, LeavePlan leavePlan, decimal contractHoursPerWeek)
    {
        var result = new LeaveCalculationResult
        {
            Year = year,
            ContractHoursPerWeek = contractHoursPerWeek,
            LeavePlanName = leavePlan.Name
        };

        decimal totalUnpaidLeaveHours = 0;
        decimal totalPaidWazoHours = 0;
        decimal totalWorkedHours = 0;
        decimal hoursPerDay = contractHoursPerWeek / 5; // Assuming 5 work days per week

        foreach (var phase in leavePlan.Phases)
        {
            // Check if phase overlaps with the year
            var phaseYearStart = phase.StartDate.Year;
            var phaseYearEnd = phase.EndDate.Year;

            if (phaseYearEnd < year || phaseYearStart > year)
                continue; // Phase doesn't overlap with this year

            // Calculate overlap
            var phaseStart = phase.StartDate.Year < year ? new DateOnly(year, 1, 1) : phase.StartDate;
            var phaseEnd = phase.EndDate.Year > year ? new DateOnly(year, 12, 31) : phase.EndDate;

            // Calculate weeks in phase for this year
            var weeksInPhase = CalculateWeeksInPhase(phaseStart, phaseEnd);

            if (phase.LeaveType == LeaveType.PaidParental)
            {
                result.PaidWazoHours += phase.LeaveHoursPerWeek * weeksInPhase;
                totalPaidWazoHours += phase.LeaveHoursPerWeek * weeksInPhase;
            }
            else if (phase.LeaveType == LeaveType.UnpaidParental)
            {
                result.UnpaidLeaveHours += phase.LeaveHoursPerWeek * weeksInPhase;
                totalUnpaidLeaveHours += phase.LeaveHoursPerWeek * weeksInPhase;
            }

            result.WorkedHours += phase.WorkHoursPerWeek * weeksInPhase;
            totalWorkedHours += phase.WorkHoursPerWeek * weeksInPhase;

            result.Phases.Add(new LeavePhaseResult
            {
                PhaseNumber = phase.PhaseNumber,
                StartDate = phase.StartDate,
                EndDate = phase.EndDate,
                LeaveType = phase.LeaveType,
                LeaveHoursPerWeek = phase.LeaveHoursPerWeek,
                WorkHoursPerWeek = phase.WorkHoursPerWeek,
                WeeksInYear = weeksInPhase,
                LeaveHoursInYear = phase.LeaveHoursPerWeek * weeksInPhase,
                WorkHoursInYear = phase.WorkHoursPerWeek * weeksInPhase
            });
        }

        // Calculate annual contract hours
        result.AnnualContractHours = contractHoursPerWeek * CalculationConstants.WeeksPerYear;
        result.HoursPerDay = hoursPerDay;

        // Validate leave hours
        ValidateLeaveHours(result, totalUnpaidLeaveHours, totalPaidWazoHours, totalWorkedHours);

        return result;
    }

    /// <summary>
    /// Validate leave hours and add warnings if needed
    /// </summary>
    private void ValidateLeaveHours(
        LeaveCalculationResult result,
        decimal totalUnpaidLeaveHours,
        decimal totalPaidWazoHours,
        decimal totalWorkedHours)
    {
        // Check if total worked hours + leave hours exceed contract hours
        var totalLeaveHours = totalUnpaidLeaveHours + totalPaidWazoHours;
        var expectedTotalHours = result.AnnualContractHours;
        var actualTotalHours = totalWorkedHours + totalLeaveHours;

        if (actualTotalHours > expectedTotalHours * 1.01m) // Allow 1% tolerance
        {
            result.Warnings.Add("Total worked hours + leave hours exceed annual contract hours");
        }

        // Check for negative hours
        if (totalUnpaidLeaveHours < 0 || totalPaidWazoHours < 0 || totalWorkedHours < 0)
        {
            result.Warnings.Add("Negative leave or work hours detected");
        }

        // Check if leave hours exceed maximum paid parental leave
        var maxPaidLeaveHours = 9 * result.ContractHoursPerWeek; // From JSON: max_paid_parental_leave_formula = "9 * contract_hours_week"
        if (totalPaidWazoHours > maxPaidLeaveHours)
        {
            result.Warnings.Add("Paid parental leave hours exceed maximum allowed (9 * contract hours per week)");
        }
    }

    /// <summary>
    /// Calculate number of weeks between two dates
    /// </summary>
    private decimal CalculateWeeksInPhase(DateOnly startDate, DateOnly endDate)
    {
        var days = (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
        return (decimal)(days / 7);
    }
}
