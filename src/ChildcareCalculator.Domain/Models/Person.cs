using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class Person : EntityBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Employment contract settings
    public decimal ContractHoursPerWeek { get; set; }
    public int WorkDaysPerWeek { get; set; }
    public decimal MonthlySalary { get; set; }
    public decimal HolidayAllowancePercentage { get; set; }

    // Car benefits
    public decimal CarBenefitMonthly { get; set; }
    public decimal TaxableCarAllowanceMonthly { get; set; }

    // Profit distribution
    public decimal ProfitDistributionFullYear { get; set; }
    public decimal ExcessProfitFullYear { get; set; }

    // Deductions
    public decimal PensionPremiumMonthly { get; set; }
    public decimal WiaWgaPremiumMonthly { get; set; }

    // WAZO settings (policy assumptions)
    public decimal UwvMaximumDailyWage { get; set; }
    public decimal UwvWazoPercentage { get; set; }

    // Manual corrections
    public decimal ManualTaxableIncomeCorrection { get; set; }

    // Navigation properties
    public virtual ICollection<AnnualPersonIncome> AnnualIncomes { get; set; } = new List<AnnualPersonIncome>();
    public virtual LeavePlan? LeavePlan { get; set; }
    public virtual ICollection<ExtraIncome> ExtraIncomes { get; set; } = new List<ExtraIncome>();

    // Household reference
    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;
}
