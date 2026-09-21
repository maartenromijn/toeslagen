using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Domain.Models;

public class HouseholdScenario : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    // Navigation properties
    public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
    public virtual Mortgage? Mortgage { get; set; }
    public virtual ICollection<HomeSettings> HomeSettings { get; set; } = new List<HomeSettings>();
    public virtual ICollection<ChildcareSettings> ChildcareSettings { get; set; } = new List<ChildcareSettings>();
    public virtual ICollection<ChildcareCalculation> ChildcareCalculations { get; set; } = new List<ChildcareCalculation>();
    public virtual ICollection<HouseholdCalculationResult> CalculationResults { get; set; } = new List<HouseholdCalculationResult>();
    public virtual ICollection<PolicyAssumption> PolicyAssumptions { get; set; } = new List<PolicyAssumption>();
    public virtual ICollection<AnnualPersonIncome> AnnualPersonIncomes { get; set; } = new List<AnnualPersonIncome>();

    // Policy version references
    public string? TaxPolicyVersion { get; set; }
    public string? ChildcareBenefitPolicyVersion { get; set; }
}
