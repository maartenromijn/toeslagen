using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class AnnualMortgageResult : EntityBase
{
    public int Year { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalInterestPaid { get; set; }
    public decimal RegularRepayment { get; set; }
    public decimal ExtraRepayment { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal AverageBalance { get; set; }
    public decimal DeductibleInterest { get; set; }

    // Navigation
    public Guid MortgagePartId { get; set; }
    public virtual MortgagePart MortgagePart { get; set; } = null!;
}
