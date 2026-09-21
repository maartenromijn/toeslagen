using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChildcareCalculator.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HouseholdScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TaxPolicyVersion = table.Column<string>(type: "TEXT", nullable: true),
                    ChildcareBenefitPolicyVersion = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdScenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mortgages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReferenceDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CalculationMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mortgages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mortgages_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContractHoursPerWeek = table.Column<decimal>(type: "TEXT", nullable: false),
                    WorkDaysPerWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    MonthlySalary = table.Column<decimal>(type: "TEXT", nullable: false),
                    HolidayAllowancePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    CarBenefitMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxableCarAllowanceMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProfitDistributionFullYear = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExcessProfitFullYear = table.Column<decimal>(type: "TEXT", nullable: false),
                    PensionPremiumMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    WiaWgaPremiumMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    UwvMaximumDailyWage = table.Column<decimal>(type: "TEXT", nullable: false),
                    UwvWazoPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    ManualTaxableIncomeCorrection = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnualPersonIncomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    MonthlySalary = table.Column<decimal>(type: "TEXT", nullable: false),
                    ContractHoursPerWeek = table.Column<decimal>(type: "TEXT", nullable: false),
                    HolidayAllowancePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    CarBenefitMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxableCarAllowanceMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProfitDistributionFullYear = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExcessProfitFullYear = table.Column<decimal>(type: "TEXT", nullable: false),
                    PensionPremiumMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    WiaWgaPremiumMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    ManualTaxableIncomeCorrection = table.Column<decimal>(type: "TEXT", nullable: false),
                    AnnualContractHours = table.Column<decimal>(type: "TEXT", nullable: false),
                    NormalDailyWage = table.Column<decimal>(type: "TEXT", nullable: false),
                    WazoDailyBenefit = table.Column<decimal>(type: "TEXT", nullable: false),
                    WazoReplacementRatio = table.Column<decimal>(type: "TEXT", nullable: false),
                    SalaryAfterLeave = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProfitAfterLeave = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExcessProfitAfterLeave = table.Column<decimal>(type: "TEXT", nullable: false),
                    HolidayAllowance = table.Column<decimal>(type: "TEXT", nullable: false),
                    WazoBenefit = table.Column<decimal>(type: "TEXT", nullable: false),
                    CarBenefitAnnual = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxableAllowanceAnnual = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxableIncomeBeforeHome = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxableIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualPersonIncomes", x => x.Id);
                    table.UniqueConstraint("AK_AnnualPersonIncomes_PersonId_Year", x => new { x.PersonId, x.Year });
                    table.ForeignKey(
                        name: "FK_AnnualPersonIncomes_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnualPersonIncomes_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChildcareBenefitPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IncomeBoundaryIndexFactor = table.Column<decimal>(type: "TEXT", nullable: false),
                    MiddleIncomeSurcharge = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaximumHourlyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildcareBenefitPolicies", x => x.Id);
                    table.UniqueConstraint("AK_ChildcareBenefitPolicies_Year", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "ChildcareSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    ChildcareWeeks = table.Column<int>(type: "INTEGER", nullable: false),
                    DaysPerWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    HoursPerDay = table.Column<decimal>(type: "TEXT", nullable: false),
                    ActualHourlyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaximumHourlyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChildcareType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildcareSettings", x => x.Id);
                    table.UniqueConstraint("AK_ChildcareSettings_HouseholdScenarioId_Year", x => new { x.HouseholdScenarioId, x.Year });
                    table.ForeignKey(
                        name: "FK_ChildcareSettings_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraIncomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Include = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraIncomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraIncomes_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    WozValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    ImputedRentalValuePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    OtherBox1Deductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    HomeImputedIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeSettings", x => x.Id);
                    table.UniqueConstraint("AK_HomeSettings_HouseholdScenarioId_Year", x => new { x.HouseholdScenarioId, x.Year });
                    table.ForeignKey(
                        name: "FK_HomeSettings_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeavePlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContractHoursPerWeek = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaxPaidParentalLeaveFormula = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ChildBirthDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PaidLeaveLastDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    WorkHours3Days = table.Column<decimal>(type: "TEXT", nullable: false),
                    WorkHours4Days = table.Column<decimal>(type: "TEXT", nullable: false),
                    WorkHoursAfterPaidLeave = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePlans_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyAssumptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyAssumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyAssumptions_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Bracket1Limit = table.Column<decimal>(type: "TEXT", nullable: false),
                    Bracket2Limit = table.Column<decimal>(type: "TEXT", nullable: false),
                    Rate1 = table.Column<decimal>(type: "TEXT", nullable: false),
                    Rate2 = table.Column<decimal>(type: "TEXT", nullable: false),
                    Rate3 = table.Column<decimal>(type: "TEXT", nullable: false),
                    GeneralTaxCreditMax = table.Column<decimal>(type: "TEXT", nullable: false),
                    GeneralTaxCreditPhaseoutStart = table.Column<decimal>(type: "TEXT", nullable: false),
                    GeneralTaxCreditPhaseoutRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditMaxPhaseoutStart = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditPhaseoutStart = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditPhaseoutRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditPct1 = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditAmount1 = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditPct2 = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditAmount2 = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditPct3 = table.Column<decimal>(type: "TEXT", nullable: false),
                    EmploymentCreditCorrection2027Plus = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxPolicies", x => x.Id);
                    table.UniqueConstraint("AK_TaxPolicies_Year", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "ChildcareCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    ChildcareHoursPerYear = table.Column<decimal>(type: "TEXT", nullable: false),
                    ActualChildcareCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    EligibleChildcareCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChildcareBenefitPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChildcareBenefitAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    OwnContributionYear = table.Column<decimal>(type: "TEXT", nullable: false),
                    OwnContributionMonth = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildcareCalculations", x => x.Id);
                    table.UniqueConstraint("AK_ChildcareCalculations_HouseholdScenarioId_Year", x => new { x.HouseholdScenarioId, x.Year });
                    table.ForeignKey(
                        name: "FK_ChildcareCalculations_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdCalculationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalAssessmentIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalChildcareCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalEligibleChildcareCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalChildcareBenefit = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalOwnContributionYear = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalOwnContributionMonth = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalNetIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdCalculationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HouseholdCalculationResults_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MortgageParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MortgageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PartNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalPrincipal = table.Column<decimal>(type: "TEXT", nullable: false),
                    OutstandingAtReference = table.Column<decimal>(type: "TEXT", nullable: false),
                    InterestRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    RegularPaymentMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExtraPaymentMonthly = table.Column<decimal>(type: "TEXT", nullable: false),
                    RateRevisionDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MortgageParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MortgageParts_Mortgages_MortgageId",
                        column: x => x.MortgageId,
                        principalTable: "Mortgages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChildcareBenefitBands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChildcareBenefitPolicyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BandNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    MinIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaxIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    FirstChildPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    AdditionalChildPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildcareBenefitBands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChildcareBenefitBands_ChildcareBenefitPolicies_ChildcareBenefitPolicyId",
                        column: x => x.ChildcareBenefitPolicyId,
                        principalTable: "ChildcareBenefitPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeavePhases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeavePlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PhaseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    WorkHoursPerWeek = table.Column<decimal>(type: "TEXT", nullable: false),
                    LeaveType = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaveHoursPerWeek = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExtraHours = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePhases_LeavePlans_LeavePlanId",
                        column: x => x.LeavePlanId,
                        principalTable: "LeavePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnualCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseholdCalculationResultId = table.Column<Guid>(type: "TEXT", nullable: true),
                    HouseholdScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    Person1TaxableIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    Person2TaxableIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    HomeImputedIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    DeductibleMortgageInterest = table.Column<decimal>(type: "TEXT", nullable: false),
                    OtherBox1Deductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChildcareBenefitPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChildcareBenefitOverride = table.Column<decimal>(type: "TEXT", nullable: true),
                    ChildcareBenefitAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    EstimatedNetIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualCalculations", x => x.Id);
                    table.UniqueConstraint("AK_AnnualCalculations_HouseholdScenarioId_Year", x => new { x.HouseholdScenarioId, x.Year });
                    table.ForeignKey(
                        name: "FK_AnnualCalculations_HouseholdCalculationResults_HouseholdCalculationResultId",
                        column: x => x.HouseholdCalculationResultId,
                        principalTable: "HouseholdCalculationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnualCalculations_HouseholdScenarios_HouseholdScenarioId",
                        column: x => x.HouseholdScenarioId,
                        principalTable: "HouseholdScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnualMortgageResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MortgagePartId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalInterestPaid = table.Column<decimal>(type: "TEXT", nullable: false),
                    RegularRepayment = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExtraRepayment = table.Column<decimal>(type: "TEXT", nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    AverageBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    DeductibleInterest = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualMortgageResults", x => x.Id);
                    table.UniqueConstraint("AK_AnnualMortgageResults_MortgagePartId_Year", x => new { x.MortgagePartId, x.Year });
                    table.ForeignKey(
                        name: "FK_AnnualMortgageResults_MortgageParts_MortgagePartId",
                        column: x => x.MortgagePartId,
                        principalTable: "MortgageParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyMortgageResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MortgagePartId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    MonthlyInterest = table.Column<decimal>(type: "TEXT", nullable: false),
                    MonthlyPrincipal = table.Column<decimal>(type: "TEXT", nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyMortgageResults", x => x.Id);
                    table.UniqueConstraint("AK_MonthlyMortgageResults_MortgagePartId_Year_Month", x => new { x.MortgagePartId, x.Year, x.Month });
                    table.ForeignKey(
                        name: "FK_MonthlyMortgageResults_MortgageParts_MortgagePartId",
                        column: x => x.MortgagePartId,
                        principalTable: "MortgageParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalculationExplanations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnnualCalculationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CalculationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FormulaId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FormulaExpression = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FormulaDescription = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    InputValuesJson = table.Column<string>(type: "TEXT", nullable: false),
                    IntermediateResultsJson = table.Column<string>(type: "TEXT", nullable: false),
                    ResultValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    ResultDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    RoundingRule = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsProvisional = table.Column<bool>(type: "INTEGER", nullable: false),
                    WarningMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PolicyVersion = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationExplanations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalculationExplanations_AnnualCalculations_AnnualCalculationId",
                        column: x => x.AnnualCalculationId,
                        principalTable: "AnnualCalculations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Insert seed data
            migrationBuilder.InsertData(
                table: "TaxPolicies",
                columns: new[] { "Id", "Year", "Status", "Description", "Bracket1Limit", "Bracket2Limit", "Rate1", "Rate2", "Rate3", "GeneralTaxCreditMax", "GeneralTaxCreditPhaseoutStart", "GeneralTaxCreditPhaseoutRate", "EmploymentCreditMaxPhaseoutStart", "EmploymentCreditPhaseoutStart", "EmploymentCreditPhaseoutRate", "EmploymentCreditPct1", "EmploymentCreditAmount1", "EmploymentCreditPct2", "EmploymentCreditAmount2", "EmploymentCreditPct3", "EmploymentCreditCorrection2027Plus", "CreatedAtUtc", "Version" },
                values: new object[]
                {
                    Guid.NewGuid(), 2026, 2, "Tax policy 2026", 38883m, 78426m, 0.3575m, 0.3756m, 0.495m, 3115m, 29736m, 0.06398m, 5685m, 45592m, 0.0651m, 0.08324m, 996m, 0.31009m, 5300m, 0.0195m, 0m, DateTime.UtcNow, 1
                });

            migrationBuilder.InsertData(
                table: "TaxPolicies",
                columns: new[] { "Id", "Year", "Status", "Description", "Bracket1Limit", "Bracket2Limit", "Rate1", "Rate2", "Rate3", "GeneralTaxCreditMax", "GeneralTaxCreditPhaseoutStart", "GeneralTaxCreditPhaseoutRate", "EmploymentCreditMaxPhaseoutStart", "EmploymentCreditPhaseoutStart", "EmploymentCreditPhaseoutRate", "EmploymentCreditPct1", "EmploymentCreditAmount1", "EmploymentCreditPct2", "EmploymentCreditAmount2", "EmploymentCreditPct3", "EmploymentCreditCorrection2027Plus", "CreatedAtUtc", "Version" },
                values: new object[]
                {
                    Guid.NewGuid(), 2027, 2, "Tax policy 2027", 39247m, 78426m, 0.3623m, 0.3816m, 0.495m, 3154m, 30912m, 0.06638m, 5929m, 47834m, 0.0651m, 0.08324m, 996m, 0.31009m, 5300m, 0.0195m, 244m, DateTime.UtcNow, 1
                });

            migrationBuilder.InsertData(
                table: "ChildcareBenefitPolicies",
                columns: new[] { "Id", "Year", "Status", "Description", "IncomeBoundaryIndexFactor", "MiddleIncomeSurcharge", "MaximumHourlyRate", "CreatedAtUtc", "Version" },
                values: new object[]
                {
                    Guid.NewGuid(), 2026, 2, "Childcare benefit policy 2026", 1.0m, 0m, 11.6m, DateTime.UtcNow, 1
                });

            migrationBuilder.InsertData(
                table: "ChildcareBenefitPolicies",
                columns: new[] { "Id", "Year", "Status", "Description", "IncomeBoundaryIndexFactor", "MiddleIncomeSurcharge", "MaximumHourlyRate", "CreatedAtUtc", "Version" },
                values: new object[]
                {
                    Guid.NewGuid(), 2027, 1, "Childcare benefit policy 2027 (provisional)", 1.03469834323226m, 0.051m, 12.05m, DateTime.UtcNow, 1
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualCalculations_HouseholdCalculationResultId",
                table: "AnnualCalculations",
                column: "HouseholdCalculationResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationExplanations");

            migrationBuilder.DropTable(
                name: "AnnualMortgageResults");

            migrationBuilder.DropTable(
                name: "MonthlyMortgageResults");

            migrationBuilder.DropTable(
                name: "AnnualCalculations");

            migrationBuilder.DropTable(
                name: "ChildcareBenefitBands");

            migrationBuilder.DropTable(
                name: "LeavePhases");

            migrationBuilder.DropTable(
                name: "ChildcareCalculations");

            migrationBuilder.DropTable(
                name: "HouseholdCalculationResults");

            migrationBuilder.DropTable(
                name: "MortgageParts");

            migrationBuilder.DropTable(
                name: "PolicyAssumptions");

            migrationBuilder.DropTable(
                name: "TaxPolicies");

            migrationBuilder.DropTable(
                name: "ChildcareBenefitPolicies");

            migrationBuilder.DropTable(
                name: "LeavePlans");

            migrationBuilder.DropTable(
                name: "HomeSettings");

            migrationBuilder.DropTable(
                name: "ChildcareSettings");

            migrationBuilder.DropTable(
                name: "ExtraIncomes");

            migrationBuilder.DropTable(
                name: "AnnualPersonIncomes");

            migrationBuilder.DropTable(
                name: "Mortgages");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "HouseholdScenarios");
        }
    }
}
