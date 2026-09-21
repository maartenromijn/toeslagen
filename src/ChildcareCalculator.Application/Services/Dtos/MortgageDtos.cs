namespace ChildcareCalculator.Application.Services.Dtos;

public class MonthlyMortgageResultDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal MonthlyInterest { get; set; }
    public decimal MonthlyPrincipal { get; set; }
    public decimal ClosingBalance { get; set; }
}

public class AnnualMortgageResultDto
{
    public int Year { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalInterestPaid { get; set; }
    public decimal TotalPrincipalPaid { get; set; }
    public decimal AverageBalance { get; set; }
    public decimal DeductibleInterest { get; set; }
}

public class MortgagePartCalculationResult
{
    public Guid PartId { get; set; }
    public int PartNumber { get; set; }
    public List<MonthlyMortgageResultDto> MonthlyResults { get; set; } = new List<MonthlyMortgageResultDto>();
    public List<AnnualMortgageResultDto> AnnualResults { get; set; } = new List<AnnualMortgageResultDto>();
}

public class MortgageCalculationResult
{
    public Guid MortgageId { get; set; }
    public DateOnly CalculationStartDate { get; set; }
    public int YearsToCalculate { get; set; }
    public List<MortgagePartCalculationResult> PartResults { get; set; } = new List<MortgagePartCalculationResult>();
    public List<AnnualMortgageResultDto> AggregatedAnnualResults { get; set; } = new List<AnnualMortgageResultDto>();
}
