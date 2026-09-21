using Xunit;
using System.Text.Json;
using ChildcareCalculator.Application.Services;
using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChildcareCalculator.Tests.Integration;

public class JsonImportExportTests : IDisposable
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly JsonImportExportService _jsonService;
    private readonly RoundingService _roundingService;

    public JsonImportExportTests()
    {
        // Use in-memory database for testing
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _roundingService = new RoundingService();
        
        // Create services with mock dependencies
        var incomeService = new IncomeCalculationService(_roundingService);
        var leaveService = new LeaveCalculationService(_roundingService);
        var childcareCostService = new ChildcareCostService(_roundingService);
        var childcareBenefitService = new ChildcareBenefitService(_roundingService);
        var assessmentIncomeService = new AssessmentIncomeService(_roundingService);
        var homeService = new HomeCalculationService(_roundingService);
        var mortgageService = new MortgageCalculationService(_roundingService);

        _jsonService = new JsonImportExportService(
            _roundingService,
            incomeService,
            leaveService,
            childcareCostService,
            childcareBenefitService,
            assessmentIncomeService,
            homeService,
            mortgageService);
    }

    [Fact]
    public async Task ImportScenarioFromJsonAsync_ShouldImportBasicScenario()
    {
        // Read the sample JSON file
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Verify basic scenario properties
        Assert.NotNull(scenario);
        Assert.Equal("Test Scenario | Source: Kinderopvangtoeslag_rekentool_2026-2030_netto_met_bijtelling_en_verlof.xlsx", scenario.Description);
        Assert.True(scenario.IsActive);

        // Verify persons were imported
        Assert.NotEmpty(scenario.Persons);
        Assert.Contains(scenario.Persons, p => p.FirstName == "Maarten");
        Assert.Contains(scenario.Persons, p => p.FirstName == "Gina");
    }

    [Fact]
    public async Task ImportScenarioFromJsonAsync_ShouldImportChildcareSettings()
    {
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Verify childcare settings were imported
        Assert.NotEmpty(scenario.ChildcareSettings);
        
        // Check that settings for multiple years were imported
        var years = scenario.ChildcareSettings.Select(cs => cs.Year).Distinct().ToList();
        Assert.Contains(years, 2027);
        Assert.Contains(years, 2028);
        Assert.Contains(years, 2029);
        Assert.Contains(years, 2030);

        // Check specific values for 2027
        var settings2027 = scenario.ChildcareSettings.FirstOrDefault(cs => cs.Year == 2027);
        Assert.NotNull(settings2027);
        Assert.Equal(49, settings2027.ChildcareWeeks);
        Assert.Equal(3, settings2027.DaysPerWeek);
        Assert.Equal(11, settings2027.HoursPerDay);
        Assert.Equal(11.8m, settings2027.ActualHourlyRate);
        Assert.Equal(11.6m, settings2027.MaximumHourlyRate);
    }

    [Fact]
    public async Task ImportScenarioFromJsonAsync_ShouldImportHomeSettings()
    {
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Verify home settings were imported
        Assert.NotEmpty(scenario.HomeSettings);

        // Check specific values for 2027
        var settings2027 = scenario.HomeSettings.FirstOrDefault(hs => hs.Year == 2027);
        Assert.NotNull(settings2027);
        Assert.Equal(700000, settings2027.WozValue);
        Assert.Equal(0.0035m, settings2027.ImputedRentalValuePercentage);
        Assert.Equal(0, settings2027.OtherBox1Deductions);
        
        // Verify calculated value
        var expectedImputedIncome = 700000 * 0.0035m;
        Assert.Equal(expectedImputedIncome, settings2027.HomeImputedIncome);
    }

    [Fact]
    public async Task ImportScenarioFromJsonAsync_ShouldImportPersonsWithAnnualIncomes()
    {
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Find Maarten
        var maarten = scenario.Persons.FirstOrDefault(p => p.FirstName == "Maarten");
        Assert.NotNull(maarten);

        // Verify Maarten's properties
        Assert.Equal(40, maarten.ContractHoursPerWeek);
        Assert.Equal(0.08m, maarten.HolidayAllowancePercentage);
        Assert.Equal(689.58m, maarten.CarBenefitMonthly);
        Assert.Equal(67.01m, maarten.TaxableCarAllowanceMonthly);
        Assert.Equal(4250, maarten.ProfitDistributionFullYear);
        Assert.Equal(4182.13m, maarten.ExcessProfitFullYear);
        Assert.Equal(27.2m, maarten.PensionPremiumMonthly);
        Assert.Equal(15.8m, maarten.WiaWgaPremiumMonthly);
        Assert.Equal(309.91m, maarten.UwvMaximumDailyWage);
        Assert.Equal(0.7m, maarten.UwvWazoPercentage);

        // Verify annual incomes were created
        Assert.NotEmpty(maarten.AnnualIncomes);
        var income2027 = maarten.AnnualIncomes.FirstOrDefault(ai => ai.Year == 2027);
        Assert.NotNull(income2027);
        Assert.Equal(5842.81m, income2027.MonthlySalary);
    }

    [Fact]
    public async Task ImportScenarioFromJsonAsync_ShouldImportLeavePlans()
    {
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Find Maarten's leave plan
        var maarten = scenario.Persons.FirstOrDefault(p => p.FirstName == "Maarten");
        Assert.NotNull(maarten);
        Assert.NotNull(maarten.LeavePlan);

        var leavePlan = maarten.LeavePlan;
        Assert.Equal(40, leavePlan.ContractHoursPerWeek);
        Assert.Equal("9 * contract_hours_week", leavePlan.MaxPaidParentalLeaveFormula);
        Assert.Equal(new DateOnly(2026, 8, 19), leavePlan.ChildBirthDate);
        Assert.Equal(new DateOnly(2027, 8, 18), leavePlan.PaidLeaveLastDate);

        // Verify phases were imported
        Assert.NotEmpty(leavePlan.Phases);
        Assert.Equal(4, leavePlan.Phases.Count);

        // Check first phase
        var phase1 = leavePlan.Phases.FirstOrDefault(p => p.PhaseNumber == 1);
        Assert.NotNull(phase1);
        Assert.Equal(new DateOnly(2026, 11, 19), phase1.StartDate);
        Assert.Equal(new DateOnly(2026, 12, 18), phase1.EndDate);
        Assert.Equal(24, phase1.WorkHoursPerWeek);
        Assert.Equal(LeaveType.PaidParental, phase1.LeaveType);
        Assert.Equal(16, phase1.LeaveHoursPerWeek);
    }

    [Fact]
    public async Task ImportScenarioFromJsonAsync_ShouldImportMortgage()
    {
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Verify mortgage was imported
        Assert.NotNull(scenario.Mortgage);
        Assert.Equal(new DateOnly(2026, 9, 1), scenario.Mortgage.ReferenceDate);
        Assert.Equal("monthly_annuity_schedule", scenario.Mortgage.CalculationMethod);

        // Verify mortgage parts were imported
        Assert.NotEmpty(scenario.Mortgage.Parts);
        Assert.Equal(2, scenario.Mortgage.Parts.Count);

        // Check first part
        var part1 = scenario.Mortgage.Parts.FirstOrDefault(p => p.PartNumber == 1);
        Assert.NotNull(part1);
        Assert.Equal(213500, part1.OriginalPrincipal);
        Assert.Equal(203878.83m, part1.OutstandingAtReference);
        Assert.Equal(0.0378m, part1.InterestRate);
        Assert.Equal(992.76m, part1.RegularPaymentMonthly);
        Assert.Equal(0, part1.ExtraPaymentMonthly);
        Assert.Equal(new DateOnly(2034, 4, 1), part1.RateRevisionDate);
        Assert.Equal(new DateOnly(2054, 4, 1), part1.EndDate);
    }

    [Fact]
    public async Task ImportScenarioFromJsonAsync_ShouldImportPolicyAssumptions()
    {
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Verify policy assumptions were imported
        Assert.NotEmpty(scenario.PolicyAssumptions);

        // Check for specific policy keys
        var indexFactor = scenario.PolicyAssumptions.FirstOrDefault(p => p.Key == "indexfactor_inkomensgrenzen" && p.Year == 2027);
        Assert.NotNull(indexFactor);
        Assert.Equal(PolicyValueStatus.Provisional, indexFactor.Status);
        Assert.Equal("1.03469834323226", indexFactor.Value);

        var surcharge = scenario.PolicyAssumptions.FirstOrDefault(p => p.Key == "middeninkomens_opslag" && p.Year == 2027);
        Assert.NotNull(surcharge);
        Assert.Equal("0.051", surcharge.Value);
    }

    [Fact]
    public async Task ExportScenarioToJson_ShouldExportCompleteScenario()
    {
        // First import a scenario
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");
        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Export it
        var exportedJson = _jsonService.ExportScenarioToJson(scenario);

        // Verify it's valid JSON
        Assert.NotNull(exportedJson);
        var jsonDocument = JsonDocument.Parse(exportedJson);
        Assert.NotNull(jsonDocument);

        // Verify metadata
        var root = jsonDocument.RootElement;
        Assert.True(root.TryGetProperty("schemaVersion", out _));
        Assert.True(root.TryGetProperty("applicationVersion", out _));
        Assert.True(root.TryGetProperty("exportedAtUtc", out _));
        Assert.True(root.TryGetProperty("scenarioId", out _));
        Assert.True(root.TryGetProperty("scenarioName", out _));

        // Verify inputs
        Assert.True(root.TryGetProperty("inputs", out var inputs));
        Assert.True(inputs.TryGetProperty("opvang", out _));
        Assert.True(inputs.TryGetProperty("woning", out _));
        Assert.True(inputs.TryGetProperty("beleid", out _));
        Assert.True(inputs.TryGetProperty("personen", out _));

        // Verify other sections
        Assert.True(root.TryGetProperty("mortgage", out _));
        Assert.True(root.TryGetProperty("leave_plans", out _));
        Assert.True(root.TryGetProperty("extra_income", out _));
    }

    [Fact]
    public async Task ExportScenarioToJson_ShouldIncludeMetadata()
    {
        // First import a scenario
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");
        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Export it
        var exportedJson = _jsonService.ExportScenarioToJson(scenario);

        // Parse and verify metadata
        var jsonDocument = JsonDocument.Parse(exportedJson);
        var root = jsonDocument.RootElement;

        // Verify metadata
        var schemaVersion = root.GetProperty("schemaVersion").GetString();
        Assert.Equal(JsonImportExportService.CurrentSchemaVersion, schemaVersion);

        var applicationVersion = root.GetProperty("applicationVersion").GetString();
        Assert.Equal(JsonImportExportService.ApplicationVersion, applicationVersion);

        Assert.True(root.TryGetProperty("exportedAtUtc", out _));
        Assert.True(root.TryGetProperty("scenarioId", out _));
        Assert.True(root.TryGetProperty("scenarioName", out _));

        // Verify metadata section
        Assert.True(root.TryGetProperty("metadata", out var metadata));
        Assert.Equal("ChildcareCalculator", metadata.GetProperty("source").GetString());
        Assert.Equal("full_scenario", metadata.GetProperty("exportType").GetString());
        Assert.Equal("nl-NL", metadata.GetProperty("language").GetString());
        Assert.Equal("EUR", metadata.GetProperty("currency").GetString());
    }

    [Fact]
    public async Task ExportScenarioToJson_ShouldExportChildcareSettings()
    {
        // First import a scenario
        var json = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");
        var scenario = await _jsonService.ImportScenarioFromJsonAsync(json, "Test Scenario");

        // Export it
        var exportedJson = _jsonService.ExportScenarioToJson(scenario);

        // Parse and verify childcare settings
        var jsonDocument = JsonDocument.Parse(exportedJson);
        var root = jsonDocument.RootElement;
        var inputs = root.GetProperty("inputs");
        var opvang = inputs.GetProperty("opvang");

        // Verify all childcare settings are present
        Assert.True(opvang.TryGetProperty("opvangweken_per_jaar", out _));
        Assert.True(opvang.TryGetProperty("dagen_per_week", out _));
        Assert.True(opvang.TryGetProperty("uren_per_dag", out _));
        Assert.True(opvang.TryGetProperty("werkelijk_uurtarief", out _));
        Assert.True(opvang.TryGetProperty("maximum_uurprijs", out _));

        // Verify values for 2027
        var weeks = opvang.GetProperty("opvangweken_per_jaar");
        Assert.True(weeks.TryGetProperty("values", out var weeksValues));
        Assert.Equal(49, weeksValues.GetProperty("2027").GetInt32());
    }

    [Fact]
    public async Task RoundTripTest_ImportThenExportShouldPreserveData()
    {
        // Read original JSON
        var originalJson = await File.ReadAllTextAsync("/workspace/github__maartenromijn__toeslagen/sample.json");

        // Import
        var scenario = await _jsonService.ImportScenarioFromJsonAsync(originalJson, "Roundtrip Test");

        // Export
        var exportedJson = _jsonService.ExportScenarioToJson(scenario);

        // Re-import
        var reImportedScenario = await _jsonService.ImportScenarioFromJsonAsync(exportedJson, "Re-imported");

        // Verify basic properties are preserved
        Assert.Equal(scenario.Name, reImportedScenario.Name);
        Assert.Equal(scenario.Persons.Count, reImportedScenario.Persons.Count);
        
        // Verify Maarten's data
        var originalMaarten = scenario.Persons.First(p => p.FirstName == "Maarten");
        var reImportedMaarten = reImportedScenario.Persons.First(p => p.FirstName == "Maarten");
        
        Assert.Equal(originalMaarten.ContractHoursPerWeek, reImportedMaarten.ContractHoursPerWeek);
        Assert.Equal(originalMaarten.HolidayAllowancePercentage, reImportedMaarten.HolidayAllowancePercentage);

        // Verify childcare settings
        var originalSettings2027 = scenario.ChildcareSettings.First(cs => cs.Year == 2027);
        var reImportedSettings2027 = reImportedScenario.ChildcareSettings.First(cs => cs.Year == 2027);
        
        Assert.Equal(originalSettings2027.ChildcareWeeks, reImportedSettings2027.ChildcareWeeks);
        Assert.Equal(originalSettings2027.DaysPerWeek, reImportedSettings2027.DaysPerWeek);
        Assert.Equal(originalSettings2027.HoursPerDay, reImportedSettings2027.HoursPerDay);
        Assert.Equal(originalSettings2027.ActualHourlyRate, reImportedSettings2027.ActualHourlyRate);
        Assert.Equal(originalSettings2027.MaximumHourlyRate, reImportedSettings2027.MaximumHourlyRate);
    }

    public void Dispose()
    {
        // Clean up in-memory database
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            context.Database.EnsureDeleted();
        }
    }
}
