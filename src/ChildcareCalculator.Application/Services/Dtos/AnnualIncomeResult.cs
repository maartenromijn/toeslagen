namespace ChildcareCalculator.Application.Services.Dtos;

public class AnnualIncomeResult
{
    public int Year { get; set; }
    public string PersonName { get; set; } = string.Empty;

    // Base calculations
    public decimal AnnualContractHours { get; set; }
    public decimal NormalDailyWage { get; set; }
    public decimal WazoDailyBenefit { get; set; }
    public decimal WazoReplacementRatio { get; set; }

    // Salary components
    public decimal SalaryAfterLeave { get; set; }
    public decimal HolidayAllowance { get; set; }
    public decimal WazoBenefit { get; set; }
    public decimal CarBenefitAnnual { get; set; }
    public decimal TaxableAllowanceAnnual { get; set; }

    // Profit distribution
    public decimal ProfitAfterLeave { get; set; }
    public decimal ExcessProfitAfterLeave { get; set; }

    // Final taxable income
    public decimal TaxableIncomeBeforeHome { get; set; }
    public decimal TaxableIncome { get; set; }

    // Warnings
    public List<string> Warnings { get; set; } = new List<string>();
}
