using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class TaxPolicy : EntityBase
{
    public int Year { get; set; }
    public PolicyValueStatus Status { get; set; }
    public string? Description { get; set; }

    // Tax brackets
    public decimal Bracket1Limit { get; set; }
    public decimal Bracket2Limit { get; set; }
    public decimal Rate1 { get; set; }
    public decimal Rate2 { get; set; }
    public decimal Rate3 { get; set; }

    // General tax credit
    public decimal GeneralTaxCreditMax { get; set; }
    public decimal GeneralTaxCreditPhaseoutStart { get; set; }
    public decimal GeneralTaxCreditPhaseoutRate { get; set; }

    // Employment credit
    public decimal[] EmploymentCreditLimits { get; set; } = Array.Empty<decimal>();
    public decimal EmploymentCreditMaxPhaseoutStart { get; set; }
    public decimal EmploymentCreditPhaseoutStart { get; set; }
    public decimal EmploymentCreditPhaseoutRate { get; set; }
    public decimal EmploymentCreditPct1 { get; set; }
    public decimal EmploymentCreditAmount1 { get; set; }
    public decimal EmploymentCreditPct2 { get; set; }
    public decimal EmploymentCreditAmount2 { get; set; }
    public decimal EmploymentCreditPct3 { get; set; }
    public decimal EmploymentCreditCorrection2027Plus { get; set; }
}
