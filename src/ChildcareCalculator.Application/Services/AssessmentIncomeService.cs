using ChildcareCalculator.Domain.Models;

namespace ChildcareCalculator.Application.Services;

public class AssessmentIncomeService
{
    private readonly RoundingService _roundingService;

    public AssessmentIncomeService(RoundingService roundingService)
    {
        _roundingService = roundingService;
    }

    /// <summary>
    /// Calculate assessment income (toetsingsinkomen)
    /// Formula: person1_taxable_income + person2_taxable_income + home_imputed_income 
    ///          - deductible_mortgage_interest - other_box1_deductions
    /// </summary>
    public decimal CalculateAssessmentIncome(
        decimal person1TaxableIncome,
        decimal person2TaxableIncome,
        decimal homeImputedIncome,
        decimal deductibleMortgageInterest,
        decimal otherBox1Deductions)
    {
        return _roundingService.RoundToNearestCent(
            person1TaxableIncome + person2TaxableIncome + homeImputedIncome
            - deductibleMortgageInterest - otherBox1Deductions);
    }

    /// <summary>
    /// Calculate assessment income for a household
    /// </summary>
    public AssessmentIncomeResult CalculateAssessmentIncome(AssessmentIncomeInput input)
    {
        var result = new AssessmentIncomeResult
        {
            Year = input.Year,
            Person1TaxableIncome = input.Person1TaxableIncome,
            Person2TaxableIncome = input.Person2TaxableIncome,
            HomeImputedIncome = input.HomeImputedIncome,
            DeductibleMortgageInterest = input.DeductibleMortgageInterest,
            OtherBox1Deductions = input.OtherBox1Deductions
        };

        result.AssessmentIncome = CalculateAssessmentIncome(
            input.Person1TaxableIncome,
            input.Person2TaxableIncome,
            input.HomeImputedIncome,
            input.DeductibleMortgageInterest,
            input.OtherBox1Deductions);

        return result;
    }

    /// <summary>
    /// Calculate income at 2026 boundaries for indexation
    /// Formula: assessment_income / income_boundary_index_factor
    /// </summary>
    public decimal CalculateIncomeAt2026Boundaries(decimal assessmentIncome, decimal incomeBoundaryIndexFactor)
    {
        if (incomeBoundaryIndexFactor == 0)
            return assessmentIncome; // Avoid division by zero

        return assessmentIncome / incomeBoundaryIndexFactor;
    }
}

public class AssessmentIncomeInput
{
    public int Year { get; set; }
    public decimal Person1TaxableIncome { get; set; }
    public decimal Person2TaxableIncome { get; set; }
    public decimal HomeImputedIncome { get; set; }
    public decimal DeductibleMortgageInterest { get; set; }
    public decimal OtherBox1Deductions { get; set; }
}

public class AssessmentIncomeResult
{
    public int Year { get; set; }
    public decimal Person1TaxableIncome { get; set; }
    public decimal Person2TaxableIncome { get; set; }
    public decimal HomeImputedIncome { get; set; }
    public decimal DeductibleMortgageInterest { get; set; }
    public decimal OtherBox1Deductions { get; set; }
    public decimal AssessmentIncome { get; set; }
}
