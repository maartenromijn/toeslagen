using ChildcareCalculator.Domain.Enums;

namespace ChildcareCalculator.Application.Services.Dtos;

public class AnnualPersonIncomeInput
{
    public int Year { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public decimal ContractHoursPerWeek { get; set; }
    public decimal MonthlySalary { get; set; }
    public decimal HolidayAllowancePercentage { get; set; }
    public decimal CarBenefitMonthly { get; set; }
    public decimal TaxableCarAllowanceMonthly { get; set; }
    public decimal ProfitDistributionFullYear { get; set; }
    public decimal ExcessProfitFullYear { get; set; }
    public decimal PensionPremiumMonthly { get; set; }
    public decimal WiaWgaPremiumMonthly { get; set; }
    public decimal ManualTaxableIncomeCorrection { get; set; }
    public decimal OtherTaxableIncome { get; set; }
    public decimal UwvMaximumDailyWage { get; set; }
    public decimal UwvWazoPercentage { get; set; }
    public List<LeavePhaseDto> LeavePhases { get; set; } = new List<LeavePhaseDto>();
}

public class LeavePhaseDto
{
    public int PhaseNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal WorkHoursPerWeek { get; set; }
    public LeaveType LeaveType { get; set; }
    public decimal LeaveHoursPerWeek { get; set; }
    public decimal ExtraHours { get; set; }
    public string? Notes { get; set; }
}
