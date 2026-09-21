using Xunit;
using ChildcareCalculator.Application.Services;
using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Domain.Enums;

namespace ChildcareCalculator.Tests.Unit;

public class LeaveCalculationServiceTests
{
    private readonly LeaveCalculationService _service;
    private readonly RoundingService _roundingService;

    public LeaveCalculationServiceTests()
    {
        _roundingService = new RoundingService();
        _service = new LeaveCalculationService(_roundingService);
    }

    [Fact]
    public void CalculateLeaveForYear_WithNoPhases_ShouldReturnZeroHours()
    {
        var leavePlan = new LeavePlan
        {
            Name = "Test Plan",
            ContractHoursPerWeek = 40,
            Phases = new List<LeavePhase>()
        };

        var result = _service.CalculateLeaveForYear(2027, leavePlan, 40);

        Assert.Equal(2027, result.Year);
        Assert.Equal(40, result.ContractHoursPerWeek);
        Assert.Equal(2080, result.AnnualContractHours);
        Assert.Equal(0, result.PaidWazoHours);
        Assert.Equal(0, result.UnpaidLeaveHours);
        Assert.Equal(0, result.WorkedHours);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void CalculateLeaveForYear_WithSinglePhaseInYear_ShouldCalculateCorrectly()
    {
        var leavePlan = new LeavePlan
        {
            Name = "Test Plan",
            ContractHoursPerWeek = 40,
            Phases = new List<LeavePhase>
            {
                new LeavePhase
                {
                    PhaseNumber = 1,
                    StartDate = new DateOnly(2027, 1, 1),
                    EndDate = new DateOnly(2027, 12, 31),
                    WorkHoursPerWeek = 24,
                    LeaveType = LeaveType.PaidParental,
                    LeaveHoursPerWeek = 16
                }
            }
        };

        var result = _service.CalculateLeaveForYear(2027, leavePlan, 40);

        Assert.Equal(2027, result.Year);
        Assert.Equal(40, result.ContractHoursPerWeek);
        Assert.Equal(2080, result.AnnualContractHours);
        Assert.Equal(16 * 52, result.PaidWazoHours); // 16 hours/week * 52 weeks
        Assert.Equal(0, result.UnpaidLeaveHours);
        Assert.Equal(24 * 52, result.WorkedHours); // 24 hours/week * 52 weeks
        Assert.Single(result.Phases);
        Assert.Equal(52, result.Phases[0].WeeksInYear);
    }

    [Fact]
    public void CalculateLeaveForYear_WithPhaseCrossingYearBoundary_ShouldCalculatePartialYear()
    {
        var leavePlan = new LeavePlan
        {
            Name = "Test Plan",
            ContractHoursPerWeek = 40,
            Phases = new List<LeavePhase>
            {
                new LeavePhase
                {
                    PhaseNumber = 1,
                    StartDate = new DateOnly(2026, 11, 1),
                    EndDate = new DateOnly(2027, 2, 28),
                    WorkHoursPerWeek = 24,
                    LeaveType = LeaveType.PaidParental,
                    LeaveHoursPerWeek = 16
                }
            }
        };

        var result = _service.CalculateLeaveForYear(2027, leavePlan, 40);

        Assert.Equal(2027, result.Year);
        // Phase runs from Jan 1 to Feb 28 in 2027 (8 weeks)
        var expectedWeeks = 8m; // Approximately 8 weeks in Jan-Feb
        Assert.InRange(result.PaidWazoHours, 16 * 7, 16 * 9); // Should be around 8 weeks
        Assert.Equal(0, result.UnpaidLeaveHours);
        Assert.InRange(result.WorkedHours, 24 * 7, 24 * 9);
    }

    [Fact]
    public void CalculateLeaveForYear_WithMixedLeaveTypes_ShouldSeparateCorrectly()
    {
        var leavePlan = new LeavePlan
        {
            Name = "Test Plan",
            ContractHoursPerWeek = 40,
            Phases = new List<LeavePhase>
            {
                new LeavePhase
                {
                    PhaseNumber = 1,
                    StartDate = new DateOnly(2027, 1, 1),
                    EndDate = new DateOnly(2027, 6, 30),
                    WorkHoursPerWeek = 32,
                    LeaveType = LeaveType.PaidParental,
                    LeaveHoursPerWeek = 8
                },
                new LeavePhase
                {
                    PhaseNumber = 2,
                    StartDate = new DateOnly(2027, 7, 1),
                    EndDate = new DateOnly(2027, 12, 31),
                    WorkHoursPerWeek = 36,
                    LeaveType = LeaveType.UnpaidParental,
                    LeaveHoursPerWeek = 4
                }
            }
        };

        var result = _service.CalculateLeaveForYear(2027, leavePlan, 40);

        Assert.Equal(2027, result.Year);
        // First phase: ~26 weeks with 8 paid leave hours/week
        // Second phase: ~26 weeks with 4 unpaid leave hours/week
        Assert.InRange(result.PaidWazoHours, 8 * 25, 8 * 27);
        Assert.InRange(result.UnpaidLeaveHours, 4 * 25, 4 * 27);
        Assert.InRange(result.WorkedHours, 32 * 25 + 36 * 25, 32 * 27 + 36 * 27);
    }

    [Fact]
    public void CalculateLeaveForYear_WithExcessHours_ShouldAddWarning()
    {
        var leavePlan = new LeavePlan
        {
            Name = "Test Plan",
            ContractHoursPerWeek = 40,
            Phases = new List<LeavePhase>
            {
                new LeavePhase
                {
                    PhaseNumber = 1,
                    StartDate = new DateOnly(2027, 1, 1),
                    EndDate = new DateOnly(2027, 12, 31),
                    WorkHoursPerWeek = 20,
                    LeaveType = LeaveType.PaidParental,
                    LeaveHoursPerWeek = 30 // 20 + 30 = 50 > 40 contract hours
                }
            }
        };

        var result = _service.CalculateLeaveForYear(2027, leavePlan, 40);

        Assert.Contains("Total worked hours + leave hours exceed annual contract hours", result.Warnings);
    }

    [Fact]
    public void CalculateLeaveForYear_WithNegativeHours_ShouldAddWarning()
    {
        var leavePlan = new LeavePlan
        {
            Name = "Test Plan",
            ContractHoursPerWeek = 40,
            Phases = new List<LeavePhase>
            {
                new LeavePhase
                {
                    PhaseNumber = 1,
                    StartDate = new DateOnly(2027, 1, 1),
                    EndDate = new DateOnly(2027, 12, 31),
                    WorkHoursPerWeek = -10, // Negative hours
                    LeaveType = LeaveType.PaidParental,
                    LeaveHoursPerWeek = 50
                }
            }
        };

        var result = _service.CalculateLeaveForYear(2027, leavePlan, 40);

        Assert.Contains("Negative leave or work hours detected", result.Warnings);
    }

    [Fact]
    public void CalculateLeaveForYear_WithExcessPaidLeave_ShouldAddWarning()
    {
        var leavePlan = new LeavePlan
        {
            Name = "Test Plan",
            ContractHoursPerWeek = 40,
            Phases = new List<LeavePhase>
            {
                new LeavePhase
                {
                    PhaseNumber = 1,
                    StartDate = new DateOnly(2027, 1, 1),
                    EndDate = new DateOnly(2027, 12, 31),
                    WorkHoursPerWeek = 20,
                    LeaveType = LeaveType.PaidParental,
                    LeaveHoursPerWeek = 20 // 40 hours/week total, but max is 9 * 40 = 360 hours
                },
                new LeavePhase
                {
                    PhaseNumber = 2,
                    StartDate = new DateOnly(2027, 1, 1),
                    EndDate = new DateOnly(2027, 12, 31),
                    WorkHoursPerWeek = 20,
                    LeaveType = LeaveType.PaidParental,
                    LeaveHoursPerWeek = 20 // Another 360 hours, total 720 > 360
                }
            }
        };

        var result = _service.CalculateLeaveForYear(2027, leavePlan, 40);

        Assert.Contains("Paid parental leave hours exceed maximum allowed (9 * contract hours per week)", result.Warnings);
    }

    [Fact]
    public void CalculateWeeksInPhase_ShouldReturnCorrectValue()
    {
        var startDate = new DateOnly(2027, 1, 1);
        var endDate = new DateOnly(2027, 1, 7);

        var result = _service.CalculateWeeksInPhase(startDate, endDate);

        Assert.Equal(1, result); // Exactly 1 week
    }
}
