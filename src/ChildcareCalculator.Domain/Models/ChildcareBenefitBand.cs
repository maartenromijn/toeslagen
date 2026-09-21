using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class ChildcareBenefitBand : EntityBase
{
    public int BandNumber { get; set; }
    public decimal MinIncome { get; set; }
    public decimal MaxIncome { get; set; }
    public decimal FirstChildPercentage { get; set; }
    public decimal AdditionalChildPercentage { get; set; }

    // Navigation
    public Guid ChildcareBenefitPolicyId { get; set; }
    public virtual ChildcareBenefitPolicy ChildcareBenefitPolicy { get; set; } = null!;
}
