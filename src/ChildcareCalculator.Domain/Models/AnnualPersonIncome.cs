using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class AnnualPersonIncome : EntityBase
{
    public int Year { get; set; }
    public decimal MonthlySalary { get; set; }
    public decimal ContractHoursPerWeek { get; set; }
    public decimal HolidayAllowancePercentage { get; set; }
    public decimal CarBenefitMonthly { get; set; }
    public decimal TaxableCarAllowanceMonthly { get; set; }
    public decimal ProfitDistributionFullYear { get; set; }
    public decimal ExcessProfitFullYear { get; set; }
    public decimal PensionPremiumMonthly { get; set; }
    public decimal WiaWgaPremiumMonthly { get; set; }
    public decimal ManualTaxableIncomeCorrection { get; set; }

    // Calculated values
    public decimal AnnualContractHours { get; set; }
    public decimal NormalDailyWage { get; set; }
    public decimal WazoDailyBenefit { get; set; }
    public decimal WazoReplacementRatio { get; set; }
    public decimal SalaryAfterLeave { get; set; }
    public decimal ProfitAfterLeave { get; set; }
    public decimal ExcessProfitAfterLeave { get; set; }
    public decimal HolidayAllowance { get; set; }
    public decimal WazoBenefit { get; set; }
    public decimal CarBenefitAnnual { get; set; }
    public decimal TaxableAllowanceAnnual { get; set; }
    public decimal TaxableIncomeBeforeHome { get; set; }
    public decimal TaxableIncome { get; set; }

    // Navigation
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;

    public Guid HouseholdScenarioId { get; set; }
    public virtual HouseholdScenario HouseholdScenario { get; set; } = null!;
}
