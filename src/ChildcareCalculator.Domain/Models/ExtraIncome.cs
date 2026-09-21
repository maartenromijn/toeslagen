using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class ExtraIncome : EntityBase
{
    public int Year { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TaxableAmount { get; set; }
    public bool Include { get; set; } = true;

    // Navigation
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
}
