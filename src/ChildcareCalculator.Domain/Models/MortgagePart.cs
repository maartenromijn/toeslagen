using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class MortgagePart : EntityBase
{
    public int PartNumber { get; set; }
    public decimal OriginalPrincipal { get; set; }
    public decimal OutstandingAtReference { get; set; }
    public decimal InterestRate { get; set; }
    public decimal RegularPaymentMonthly { get; set; }
    public decimal ExtraPaymentMonthly { get; set; }
    public DateOnly RateRevisionDate { get; set; }
    public DateOnly EndDate { get; set; }

    // Navigation
    public Guid MortgageId { get; set; }
    public virtual Mortgage Mortgage { get; set; } = null!;

    public virtual ICollection<MonthlyMortgageResult> MonthlyResults { get; set; } = new List<MonthlyMortgageResult>();
    public virtual ICollection<AnnualMortgageResult> AnnualResults { get; set; } = new List<AnnualMortgageResult>();
}
