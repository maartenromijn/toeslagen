using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class ChildcareBenefitPolicy : EntityBase
{
    public int Year { get; set; }
    public PolicyValueStatus Status { get; set; }
    public string? Description { get; set; }

    // Income boundary index factor
    public decimal IncomeBoundaryIndexFactor { get; set; }

    // Middle income surcharge
    public decimal MiddleIncomeSurcharge { get; set; }

    // Maximum hourly rates
    public decimal MaximumHourlyRate { get; set; }

    // Navigation
    public virtual ICollection<ChildcareBenefitBand> Bands { get; set; } = new List<ChildcareBenefitBand>();
}
