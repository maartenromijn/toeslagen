using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class MonthlyMortgageResult : EntityBase
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal MonthlyInterest { get; set; }
    public decimal MonthlyPrincipal { get; set; }
    public decimal ClosingBalance { get; set; }

    // Navigation
    public Guid MortgagePartId { get; set; }
    public virtual MortgagePart MortgagePart { get; set; } = null!;
}
