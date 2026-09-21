using Microsoft.EntityFrameworkCore;
using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Domain.Models.Base;

namespace ChildcareCalculator.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Household and Scenario
    public DbSet<HouseholdScenario> HouseholdScenarios { get; set; } = null!;

    // Persons and Income
    public DbSet<Person> Persons { get; set; } = null!;
    public DbSet<AnnualPersonIncome> AnnualPersonIncomes { get; set; } = null!;
    public DbSet<LeavePlan> LeavePlans { get; set; } = null!;
    public DbSet<LeavePhase> LeavePhases { get; set; } = null!;
    public DbSet<ExtraIncome> ExtraIncomes { get; set; } = null!;

    // Childcare
    public DbSet<ChildcareSettings> ChildcareSettings { get; set; } = null!;
    public DbSet<ChildcareCalculation> ChildcareCalculations { get; set; } = null!;

    // Mortgage and Home
    public DbSet<Mortgage> Mortgages { get; set; } = null!;
    public DbSet<MortgagePart> MortgageParts { get; set; } = null!;
    public DbSet<MonthlyMortgageResult> MonthlyMortgageResults { get; set; } = null!;
    public DbSet<AnnualMortgageResult> AnnualMortgageResults { get; set; } = null!;
    public DbSet<HomeSettings> HomeSettings { get; set; } = null!;

    // Policy
    public DbSet<TaxPolicy> TaxPolicies { get; set; } = null!;
    public DbSet<ChildcareBenefitPolicy> ChildcareBenefitPolicies { get; set; } = null!;
    public DbSet<ChildcareBenefitBand> ChildcareBenefitBands { get; set; } = null!;
    public DbSet<PolicyAssumption> PolicyAssumptions { get; set; } = null!;

    // Calculations and Results
    public DbSet<AnnualCalculation> AnnualCalculations { get; set; } = null!;
    public DbSet<HouseholdCalculationResult> HouseholdCalculationResults { get; set; } = null!;
    public DbSet<CalculationExplanation> CalculationExplanations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure all entity base properties
        ConfigureEntityBase(modelBuilder);

        // Configure specific entity relationships
        ConfigureHouseholdScenario(modelBuilder);
        ConfigurePerson(modelBuilder);
        ConfigureAnnualPersonIncome(modelBuilder);
        ConfigureLeavePlan(modelBuilder);
        ConfigureLeavePhase(modelBuilder);
        ConfigureExtraIncome(modelBuilder);
        ConfigureChildcareSettings(modelBuilder);
        ConfigureChildcareCalculation(modelBuilder);
        ConfigureMortgage(modelBuilder);
        ConfigureMortgagePart(modelBuilder);
        ConfigureMonthlyMortgageResult(modelBuilder);
        ConfigureAnnualMortgageResult(modelBuilder);
        ConfigureHomeSettings(modelBuilder);
        ConfigureTaxPolicy(modelBuilder);
        ConfigureChildcareBenefitPolicy(modelBuilder);
        ConfigureChildcareBenefitBand(modelBuilder);
        ConfigurePolicyAssumption(modelBuilder);
        ConfigureAnnualCalculation(modelBuilder);
        ConfigureHouseholdCalculationResult(modelBuilder);
        ConfigureCalculationExplanation(modelBuilder);

        // Seed data
        SeedData(modelBuilder);
    }

    private void ConfigureEntityBase(ModelBuilder modelBuilder)
    {
        // Configure base properties for all entities that inherit from EntityBase
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>("Id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("CreatedAtUtc")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<int>("Version")
                    .HasDefaultValue(1);
            }
        }
    }

    private void ConfigureHouseholdScenario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HouseholdScenario>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
            entity.Property(h => h.Description).HasMaxLength(1000);
            entity.Property(h => h.IsActive).HasDefaultValue(true);

            // Navigation properties
            entity.HasMany(h => h.Persons)
                .WithOne(p => p.HouseholdScenario)
                .HasForeignKey(p => p.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.Mortgage)
                .WithOne(m => m.HouseholdScenario)
                .HasForeignKey<Mortgage>(m => m.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(h => h.HomeSettings)
                .WithOne(hs => hs.HouseholdScenario)
                .HasForeignKey(hs => hs.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(h => h.ChildcareSettings)
                .WithOne(cs => cs.HouseholdScenario)
                .HasForeignKey(cs => cs.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(h => h.ChildcareCalculations)
                .WithOne(cc => cc.HouseholdScenario)
                .HasForeignKey(cc => cc.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(h => h.CalculationResults)
                .WithOne(cr => cr.HouseholdScenario)
                .HasForeignKey(cr => cr.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(h => h.PolicyAssumptions)
                .WithOne(pa => pa.HouseholdScenario)
                .HasForeignKey(pa => pa.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(h => h.AnnualPersonIncomes)
                .WithOne(api => api.HouseholdScenario)
                .HasForeignKey(api => api.HouseholdScenarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePerson(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).HasMaxLength(100);
            entity.Property(p => p.ContractHoursPerWeek).HasColumnType("decimal(10,2)");
            entity.Property(p => p.MonthlySalary).HasColumnType("decimal(18,2)");
            entity.Property(p => p.HolidayAllowancePercentage).HasColumnType("decimal(5,4)");
            entity.Property(p => p.CarBenefitMonthly).HasColumnType("decimal(18,2)");
            entity.Property(p => p.TaxableCarAllowanceMonthly).HasColumnType("decimal(18,2)");
            entity.Property(p => p.ProfitDistributionFullYear).HasColumnType("decimal(18,2)");
            entity.Property(p => p.ExcessProfitFullYear).HasColumnType("decimal(18,2)");
            entity.Property(p => p.PensionPremiumMonthly).HasColumnType("decimal(18,2)");
            entity.Property(p => p.WiaWgaPremiumMonthly).HasColumnType("decimal(18,2)");
            entity.Property(p => p.UwvMaximumDailyWage).HasColumnType("decimal(18,2)");
            entity.Property(p => p.UwvWazoPercentage).HasColumnType("decimal(5,4)");
            entity.Property(p => p.ManualTaxableIncomeCorrection).HasColumnType("decimal(18,2)");

            // Navigation properties
            entity.HasOne(p => p.LeavePlan)
                .WithOne(lp => lp.Person)
                .HasForeignKey<LeavePlan>(lp => lp.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.AnnualIncomes)
                .WithOne(api => api.Person)
                .HasForeignKey(api => api.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.ExtraIncomes)
                .WithOne(ei => ei.Person)
                .HasForeignKey(ei => ei.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureAnnualPersonIncome(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnnualPersonIncome>(entity =>
        {
            entity.HasKey(api => api.Id);
            entity.Property(api => api.Year).IsRequired();
            entity.Property(api => api.MonthlySalary).HasColumnType("decimal(18,2)");
            entity.Property(api => api.ContractHoursPerWeek).HasColumnType("decimal(10,2)");
            entity.Property(api => api.HolidayAllowancePercentage).HasColumnType("decimal(5,4)");
            entity.Property(api => api.CarBenefitMonthly).HasColumnType("decimal(18,2)");
            entity.Property(api => api.TaxableCarAllowanceMonthly).HasColumnType("decimal(18,2)");
            entity.Property(api => api.ProfitDistributionFullYear).HasColumnType("decimal(18,2)");
            entity.Property(api => api.ExcessProfitFullYear).HasColumnType("decimal(18,2)");
            entity.Property(api => api.PensionPremiumMonthly).HasColumnType("decimal(18,2)");
            entity.Property(api => api.WiaWgaPremiumMonthly).HasColumnType("decimal(18,2)");
            entity.Property(api => api.ManualTaxableIncomeCorrection).HasColumnType("decimal(18,2)");

            // Calculated values
            entity.Property(api => api.AnnualContractHours).HasColumnType("decimal(10,2)");
            entity.Property(api => api.NormalDailyWage).HasColumnType("decimal(18,2)");
            entity.Property(api => api.WazoDailyBenefit).HasColumnType("decimal(18,2)");
            entity.Property(api => api.WazoReplacementRatio).HasColumnType("decimal(5,4)");
            entity.Property(api => api.SalaryAfterLeave).HasColumnType("decimal(18,2)");
            entity.Property(api => api.ProfitAfterLeave).HasColumnType("decimal(18,2)");
            entity.Property(api => api.ExcessProfitAfterLeave).HasColumnType("decimal(18,2)");
            entity.Property(api => api.HolidayAllowance).HasColumnType("decimal(18,2)");
            entity.Property(api => api.WazoBenefit).HasColumnType("decimal(18,2)");
            entity.Property(api => api.CarBenefitAnnual).HasColumnType("decimal(18,2)");
            entity.Property(api => api.TaxableAllowanceAnnual).HasColumnType("decimal(18,2)");
            entity.Property(api => api.TaxableIncomeBeforeHome).HasColumnType("decimal(18,2)");
            entity.Property(api => api.TaxableIncome).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per person per year
            entity.HasIndex(api => new { api.PersonId, api.Year }).IsUnique();
        });
    }

    private void ConfigureLeavePlan(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeavePlan>(entity =>
        {
            entity.HasKey(lp => lp.Id);
            entity.Property(lp => lp.Name).HasMaxLength(200);
            entity.Property(lp => lp.ContractHoursPerWeek).HasColumnType("decimal(10,2)");
            entity.Property(lp => lp.MaxPaidParentalLeaveFormula).HasMaxLength(100);
            entity.Property(lp => lp.WorkHours3Days).HasColumnType("decimal(10,2)");
            entity.Property(lp => lp.WorkHours4Days).HasColumnType("decimal(10,2)");
            entity.Property(lp => lp.WorkHoursAfterPaidLeave).HasColumnType("decimal(10,2)");

            // Navigation properties
            entity.HasMany(lp => lp.Phases)
                .WithOne(phase => phase.LeavePlan)
                .HasForeignKey(phase => phase.LeavePlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureLeavePhase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeavePhase>(entity =>
        {
            entity.HasKey(phase => phase.Id);
            entity.Property(phase => phase.PhaseNumber).IsRequired();
            entity.Property(phase => phase.WorkHoursPerWeek).HasColumnType("decimal(10,2)");
            entity.Property(phase => phase.LeaveHoursPerWeek).HasColumnType("decimal(10,2)");
            entity.Property(phase => phase.ExtraHours).HasColumnType("decimal(10,2)");
            entity.Property(phase => phase.Notes).HasMaxLength(500);
        });
    }

    private void ConfigureExtraIncome(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExtraIncome>(entity =>
        {
            entity.HasKey(ei => ei.Id);
            entity.Property(ei => ei.Year).IsRequired();
            entity.Property(ei => ei.Description).HasMaxLength(200);
            entity.Property(ei => ei.TaxableAmount).HasColumnType("decimal(18,2)");
            entity.Property(ei => ei.Include).HasDefaultValue(true);
        });
    }

    private void ConfigureChildcareSettings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChildcareSettings>(entity =>
        {
            entity.HasKey(cs => cs.Id);
            entity.Property(cs => cs.Year).IsRequired();
            entity.Property(cs => cs.ChildcareWeeks).IsRequired();
            entity.Property(cs => cs.DaysPerWeek).IsRequired();
            entity.Property(cs => cs.HoursPerDay).HasColumnType("decimal(10,2)");
            entity.Property(cs => cs.ActualHourlyRate).HasColumnType("decimal(18,2)");
            entity.Property(cs => cs.MaximumHourlyRate).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per scenario per year
            entity.HasIndex(cs => new { cs.HouseholdScenarioId, cs.Year }).IsUnique();
        });
    }

    private void ConfigureChildcareCalculation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChildcareCalculation>(entity =>
        {
            entity.HasKey(cc => cc.Id);
            entity.Property(cc => cc.Year).IsRequired();
            entity.Property(cc => cc.ChildcareHoursPerYear).HasColumnType("decimal(10,2)");
            entity.Property(cc => cc.ActualChildcareCost).HasColumnType("decimal(18,2)");
            entity.Property(cc => cc.EligibleChildcareCost).HasColumnType("decimal(18,2)");
            entity.Property(cc => cc.ChildcareBenefitPercentage).HasColumnType("decimal(5,4)");
            entity.Property(cc => cc.ChildcareBenefitAmount).HasColumnType("decimal(18,2)");
            entity.Property(cc => cc.OwnContributionYear).HasColumnType("decimal(18,2)");
            entity.Property(cc => cc.OwnContributionMonth).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per scenario per year
            entity.HasIndex(cc => new { cc.HouseholdScenarioId, cc.Year }).IsUnique();
        });
    }

    private void ConfigureMortgage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mortgage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.CalculationMethod).HasMaxLength(100);

            // Navigation properties
            entity.HasMany(m => m.Parts)
                .WithOne(mp => mp.Mortgage)
                .HasForeignKey(mp => mp.MortgageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureMortgagePart(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MortgagePart>(entity =>
        {
            entity.HasKey(mp => mp.Id);
            entity.Property(mp => mp.PartNumber).IsRequired();
            entity.Property(mp => mp.OriginalPrincipal).HasColumnType("decimal(18,2)");
            entity.Property(mp => mp.OutstandingAtReference).HasColumnType("decimal(18,2)");
            entity.Property(mp => mp.InterestRate).HasColumnType("decimal(5,4)");
            entity.Property(mp => mp.RegularPaymentMonthly).HasColumnType("decimal(18,2)");
            entity.Property(mp => mp.ExtraPaymentMonthly).HasColumnType("decimal(18,2)");

            // Navigation properties
            entity.HasMany(mp => mp.MonthlyResults)
                .WithOne(mmr => mmr.MortgagePart)
                .HasForeignKey(mmr => mmr.MortgagePartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(mp => mp.AnnualResults)
                .WithOne(amr => amr.MortgagePart)
                .HasForeignKey(amr => amr.MortgagePartId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureMonthlyMortgageResult(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonthlyMortgageResult>(entity =>
        {
            entity.HasKey(mmr => mmr.Id);
            entity.Property(mmr => mmr.Year).IsRequired();
            entity.Property(mmr => mmr.Month).IsRequired();
            entity.Property(mmr => mmr.OpeningBalance).HasColumnType("decimal(18,2)");
            entity.Property(mmr => mmr.MonthlyInterest).HasColumnType("decimal(18,2)");
            entity.Property(mmr => mmr.MonthlyPrincipal).HasColumnType("decimal(18,2)");
            entity.Property(mmr => mmr.ClosingBalance).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per part per year per month
            entity.HasIndex(mmr => new { mmr.MortgagePartId, mmr.Year, mmr.Month }).IsUnique();
        });
    }

    private void ConfigureAnnualMortgageResult(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnnualMortgageResult>(entity =>
        {
            entity.HasKey(amr => amr.Id);
            entity.Property(amr => amr.Year).IsRequired();
            entity.Property(amr => amr.OpeningBalance).HasColumnType("decimal(18,2)");
            entity.Property(amr => amr.TotalInterestPaid).HasColumnType("decimal(18,2)");
            entity.Property(amr => amr.RegularRepayment).HasColumnType("decimal(18,2)");
            entity.Property(amr => amr.ExtraRepayment).HasColumnType("decimal(18,2)");
            entity.Property(amr => amr.ClosingBalance).HasColumnType("decimal(18,2)");
            entity.Property(amr => amr.AverageBalance).HasColumnType("decimal(18,2)");
            entity.Property(amr => amr.DeductibleInterest).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per part per year
            entity.HasIndex(amr => new { amr.MortgagePartId, amr.Year }).IsUnique();
        });
    }

    private void ConfigureHomeSettings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HomeSettings>(entity =>
        {
            entity.HasKey(hs => hs.Id);
            entity.Property(hs => hs.Year).IsRequired();
            entity.Property(hs => hs.WozValue).HasColumnType("decimal(18,2)");
            entity.Property(hs => hs.ImputedRentalValuePercentage).HasColumnType("decimal(5,4)");
            entity.Property(hs => hs.OtherBox1Deductions).HasColumnType("decimal(18,2)");
            entity.Property(hs => hs.HomeImputedIncome).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per scenario per year
            entity.HasIndex(hs => new { hs.HouseholdScenarioId, hs.Year }).IsUnique();
        });
    }

    private void ConfigureTaxPolicy(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaxPolicy>(entity =>
        {
            entity.HasKey(tp => tp.Id);
            entity.Property(tp => tp.Year).IsRequired();
            entity.Property(tp => tp.Status).IsRequired();
            entity.Property(tp => tp.Description).HasMaxLength(500);

            // Tax brackets
            entity.Property(tp => tp.Bracket1Limit).HasColumnType("decimal(18,2)");
            entity.Property(tp => tp.Bracket2Limit).HasColumnType("decimal(18,2)");
            entity.Property(tp => tp.Rate1).HasColumnType("decimal(5,4)");
            entity.Property(tp => tp.Rate2).HasColumnType("decimal(5,4)");
            entity.Property(tp => tp.Rate3).HasColumnType("decimal(5,4)");

            // Tax credits
            entity.Property(tp => tp.GeneralTaxCreditMax).HasColumnType("decimal(18,2)");
            entity.Property(tp => tp.GeneralTaxCreditPhaseoutStart).HasColumnType("decimal(18,2)");
            entity.Property(tp => tp.GeneralTaxCreditPhaseoutRate).HasColumnType("decimal(5,4)");
            entity.Property(tp => tp.EmploymentCreditPhaseoutStart).HasColumnType("decimal(18,2)");
            entity.Property(tp => tp.EmploymentCreditPhaseoutRate).HasColumnType("decimal(5,4)");
            entity.Property(tp => tp.EmploymentCreditPct1).HasColumnType("decimal(5,4)");
            entity.Property(tp => tp.EmploymentCreditAmount1).HasColumnType("decimal(18,2)");
            entity.Property(tp => tp.EmploymentCreditPct2).HasColumnType("decimal(5,4)");
            entity.Property(tp => tp.EmploymentCreditAmount2).HasColumnType("decimal(18,2)");
            entity.Property(tp => tp.EmploymentCreditPct3).HasColumnType("decimal(5,4)");
            entity.Property(tp => tp.EmploymentCreditCorrection2027Plus).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per year
            entity.HasIndex(tp => tp.Year).IsUnique();

            // Navigation properties
            entity.HasMany(tp => tp.EmploymentCreditLimits)
                .WithOne()
                .HasForeignKey("TaxPolicyId")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureChildcareBenefitPolicy(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChildcareBenefitPolicy>(entity =>
        {
            entity.HasKey(cbp => cbp.Id);
            entity.Property(cbp => cbp.Year).IsRequired();
            entity.Property(cbp => cbp.Status).IsRequired();
            entity.Property(cbp => cbp.Description).HasMaxLength(500);
            entity.Property(cbp => cbp.IncomeBoundaryIndexFactor).HasColumnType("decimal(10,8)");
            entity.Property(cbp => cbp.MiddleIncomeSurcharge).HasColumnType("decimal(5,4)");
            entity.Property(cbp => cbp.MaximumHourlyRate).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per year
            entity.HasIndex(cbp => cbp.Year).IsUnique();

            // Navigation properties
            entity.HasMany(cbp => cbp.Bands)
                .WithOne(band => band.ChildcareBenefitPolicy)
                .HasForeignKey(band => band.ChildcareBenefitPolicyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureChildcareBenefitBand(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChildcareBenefitBand>(entity =>
        {
            entity.HasKey(band => band.Id);
            entity.Property(band => band.BandNumber).IsRequired();
            entity.Property(band => band.MinIncome).HasColumnType("decimal(18,2)");
            entity.Property(band => band.MaxIncome).HasColumnType("decimal(18,2)");
            entity.Property(band => band.FirstChildPercentage).HasColumnType("decimal(5,4)");
            entity.Property(band => band.AdditionalChildPercentage).HasColumnType("decimal(5,4)");
        });
    }

    private void ConfigurePolicyAssumption(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PolicyAssumption>(entity =>
        {
            entity.HasKey(pa => pa.Id);
            entity.Property(pa => pa.Key).IsRequired().HasMaxLength(100);
            entity.Property(pa => pa.Description).HasMaxLength(500);
            entity.Property(pa => pa.Status).IsRequired();
            entity.Property(pa => pa.Value).HasMaxLength(100);
            entity.Property(pa => pa.Year);
            entity.Property(pa => pa.Unit).HasMaxLength(50);
            entity.Property(pa => pa.Source).HasMaxLength(500);
        });
    }

    private void ConfigureAnnualCalculation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnnualCalculation>(entity =>
        {
            entity.HasKey(ac => ac.Id);
            entity.Property(ac => ac.Year).IsRequired();
            entity.Property(ac => ac.Status).IsRequired();

            // Assessment income
            entity.Property(ac => ac.AssessmentIncome).HasColumnType("decimal(18,2)");
            entity.Property(ac => ac.Person1TaxableIncome).HasColumnType("decimal(18,2)");
            entity.Property(ac => ac.Person2TaxableIncome).HasColumnType("decimal(18,2)");
            entity.Property(ac => ac.HomeImputedIncome).HasColumnType("decimal(18,2)");
            entity.Property(ac => ac.DeductibleMortgageInterest).HasColumnType("decimal(18,2)");
            entity.Property(ac => ac.OtherBox1Deductions).HasColumnType("decimal(18,2)");

            // Childcare
            entity.Property(ac => ac.ChildcareBenefitPercentage).HasColumnType("decimal(5,4)");
            entity.Property(ac => ac.ChildcareBenefitOverride).HasColumnType("decimal(5,4)");
            entity.Property(ac => ac.ChildcareBenefitAmount).HasColumnType("decimal(18,2)");

            // Net income
            entity.Property(ac => ac.EstimatedNetIncome).HasColumnType("decimal(18,2)");

            // Composite key to ensure one record per scenario per year
            entity.HasIndex(ac => new { ac.HouseholdScenarioId, ac.Year }).IsUnique();

            // Navigation properties
            entity.HasMany(ac => ac.Explanations)
                .WithOne(ce => ce.AnnualCalculation)
                .HasForeignKey(ce => ce.AnnualCalculationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureHouseholdCalculationResult(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HouseholdCalculationResult>(entity =>
        {
            entity.HasKey(hcr => hcr.Id);
            entity.Property(hcr => hcr.Name).HasMaxLength(200);
            entity.Property(hcr => hcr.CalculationDate).IsRequired();

            // Summary values
            entity.Property(hcr => hcr.TotalAssessmentIncome).HasColumnType("decimal(18,2)");
            entity.Property(hcr => hcr.TotalChildcareCost).HasColumnType("decimal(18,2)");
            entity.Property(hcr => hcr.TotalEligibleChildcareCost).HasColumnType("decimal(18,2)");
            entity.Property(hcr => hcr.TotalChildcareBenefit).HasColumnType("decimal(18,2)");
            entity.Property(hcr => hcr.TotalOwnContributionYear).HasColumnType("decimal(18,2)");
            entity.Property(hcr => hcr.TotalOwnContributionMonth).HasColumnType("decimal(18,2)");
            entity.Property(hcr => hcr.TotalNetIncome).HasColumnType("decimal(18,2)");

            // Navigation properties
            entity.HasMany(hcr => hcr.AnnualCalculations)
                .WithOne(ac => ac.HouseholdCalculationResult)
                .HasForeignKey(ac => ac.HouseholdCalculationResultId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureCalculationExplanation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalculationExplanation>(entity =>
        {
            entity.HasKey(ce => ce.Id);
            entity.Property(ce => ce.CalculationId).HasMaxLength(100);
            entity.Property(ce => ce.FormulaId).HasMaxLength(100);
            entity.Property(ce => ce.FormulaExpression).HasMaxLength(500);
            entity.Property(ce => ce.FormulaDescription).HasMaxLength(1000);
            entity.Property(ce => ce.InputValuesJson).HasColumnType("text");
            entity.Property(ce => ce.IntermediateResultsJson).HasColumnType("text");
            entity.Property(ce => ce.ResultValue).HasColumnType("decimal(18,2)");
            entity.Property(ce => ce.ResultDescription).HasMaxLength(500);
            entity.Property(ce => ce.RoundingRule).HasMaxLength(100);
            entity.Property(ce => ce.IsProvisional);
            entity.Property(ce => ce.WarningMessage).HasMaxLength(1000);
            entity.Property(ce => ce.PolicyVersion).HasMaxLength(50);
        });
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed initial tax policies for 2026-2030
        var taxPolicies = new List<TaxPolicy>
        {
            new TaxPolicy
            {
                Id = Guid.NewGuid(),
                Year = 2026,
                Status = PolicyValueStatus.Final,
                Description = "Tax policy 2026",
                Bracket1Limit = 38883,
                Bracket2Limit = 78426,
                Rate1 = 0.3575m,
                Rate2 = 0.3756m,
                Rate3 = 0.495m,
                GeneralTaxCreditMax = 3115,
                GeneralTaxCreditPhaseoutStart = 29736,
                GeneralTaxCreditPhaseoutRate = 0.06398m,
                EmploymentCreditLimits = new decimal[] { 11965, 25845, 45592 },
                EmploymentCreditMaxPhaseoutStart = 5685,
                EmploymentCreditPhaseoutStart = 45592,
                EmploymentCreditPhaseoutRate = 0.0651m,
                EmploymentCreditPct1 = 0.08324m,
                EmploymentCreditAmount1 = 996,
                EmploymentCreditPct2 = 0.31009m,
                EmploymentCreditAmount2 = 5300,
                EmploymentCreditPct3 = 0.0195m,
                EmploymentCreditCorrection2027Plus = 0
            },
            new TaxPolicy
            {
                Id = Guid.NewGuid(),
                Year = 2027,
                Status = PolicyValueStatus.Final,
                Description = "Tax policy 2027",
                Bracket1Limit = 39247,
                Bracket2Limit = 78426,
                Rate1 = 0.3623m,
                Rate2 = 0.3816m,
                Rate3 = 0.495m,
                GeneralTaxCreditMax = 3154,
                GeneralTaxCreditPhaseoutStart = 30912,
                GeneralTaxCreditPhaseoutRate = 0.06638m,
                EmploymentCreditLimits = new decimal[] { 11965, 25845, 45592 },
                EmploymentCreditMaxPhaseoutStart = 5929,
                EmploymentCreditPhaseoutStart = 47834,
                EmploymentCreditPhaseoutRate = 0.0651m,
                EmploymentCreditPct1 = 0.08324m,
                EmploymentCreditAmount1 = 996,
                EmploymentCreditPct2 = 0.31009m,
                EmploymentCreditAmount2 = 5300,
                EmploymentCreditPct3 = 0.0195m,
                EmploymentCreditCorrection2027Plus = 244
            }
        };

        modelBuilder.Entity<TaxPolicy>().HasData(taxPolicies);

        // Seed childcare benefit policies
        var childcareBenefitPolicies = new List<ChildcareBenefitPolicy>
        {
            new ChildcareBenefitPolicy
            {
                Id = Guid.NewGuid(),
                Year = 2026,
                Status = PolicyValueStatus.Final,
                Description = "Childcare benefit policy 2026",
                IncomeBoundaryIndexFactor = 1.0m,
                MiddleIncomeSurcharge = 0,
                MaximumHourlyRate = 11.6m
            },
            new ChildcareBenefitPolicy
            {
                Id = Guid.NewGuid(),
                Year = 2027,
                Status = PolicyValueStatus.Provisional,
                Description = "Childcare benefit policy 2027 (provisional)",
                IncomeBoundaryIndexFactor = 1.03469834323226m,
                MiddleIncomeSurcharge = 0.051m,
                MaximumHourlyRate = 12.05m
            }
        };

        modelBuilder.Entity<ChildcareBenefitPolicy>().HasData(childcareBenefitPolicies);

        // Note: ChildcareBenefitBand data would be added here for the full implementation
    }
}
