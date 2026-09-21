using System.Text.Json;
using System.Text.Json.Serialization;
using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Application.Services.Dtos;

namespace ChildcareCalculator.Application.Services;

public class JsonImportExportService
{
    private readonly RoundingService _roundingService;
    private readonly IncomeCalculationService _incomeCalculationService;
    private readonly LeaveCalculationService _leaveCalculationService;
    private readonly ChildcareCostService _childcareCostService;
    private readonly ChildcareBenefitService _childcareBenefitService;
    private readonly AssessmentIncomeService _assessmentIncomeService;
    private readonly HomeCalculationService _homeCalculationService;
    private readonly MortgageCalculationService _mortgageCalculationService;

    public JsonImportExportService(
        RoundingService roundingService,
        IncomeCalculationService incomeCalculationService,
        LeaveCalculationService leaveCalculationService,
        ChildcareCostService childcareCostService,
        ChildcareBenefitService childcareBenefitService,
        AssessmentIncomeService assessmentIncomeService,
        HomeCalculationService homeCalculationService,
        MortgageCalculationService mortgageCalculationService)
    {
        _roundingService = roundingService;
        _incomeCalculationService = incomeCalculationService;
        _leaveCalculationService = leaveCalculationService;
        _childcareCostService = childcareCostService;
        _childcareBenefitService = childcareBenefitService;
        _assessmentIncomeService = assessmentIncomeService;
        _homeCalculationService = homeCalculationService;
        _mortgageCalculationService = mortgageCalculationService;
    }

    public const string CurrentSchemaVersion = "1.0.0";
    public const string ApplicationVersion = "0.1.0";

    /// <summary>
    /// Import a scenario from JSON
    /// </summary>
    public async Task<HouseholdScenario> ImportScenarioFromJsonAsync(string json, string scenarioName = "Imported Scenario")
    {
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        var scenario = new HouseholdScenario
        {
            Name = scenarioName,
            Description = "Imported from JSON",
            IsActive = true,
            ValidFrom = DateTime.UtcNow
        };

        // Parse metadata
        if (root.TryGetProperty("metadata", out var metadataElement))
        {
            scenario.Description += $" | Source: {metadataElement.GetProperty("source_file").GetString()}";
        }

        // Parse inputs
        if (root.TryGetProperty("inputs", out var inputsElement))
        {
            await ParseInputsAsync(scenario, inputsElement);
        }

        // Parse mortgage
        if (root.TryGetProperty("mortgage", out var mortgageElement))
        {
            ParseMortgage(scenario, mortgageElement);
        }

        // Parse leave plans
        if (root.TryGetProperty("leave_plans", out var leavePlansElement))
        {
            ParseLeavePlans(scenario, leavePlansElement);
        }

        // Parse extra income
        if (root.TryGetProperty("extra_income", out var extraIncomeElement))
        {
            ParseExtraIncome(scenario, extraIncomeElement);
        }

        // Parse static parameters (policy)
        if (root.TryGetProperty("static_parameters", out var staticParamsElement))
        {
            ParseStaticParameters(scenario, staticParamsElement);
        }

        return scenario;
    }

    private async Task ParseInputsAsync(HouseholdScenario scenario, JsonElement inputsElement)
    {
        // Parse childcare settings
        if (inputsElement.TryGetProperty("opvang", out var opvangElement))
        {
            ParseChildcareSettings(scenario, opvangElement);
        }

        // Parse home settings
        if (inputsElement.TryGetProperty("woning", out var woningElement))
        {
            ParseHomeSettings(scenario, woningElement);
        }

        // Parse policy assumptions
        if (inputsElement.TryGetProperty("beleid", out var beleidElement))
        {
            ParsePolicyAssumptions(scenario, beleidElement);
        }

        // Parse persons
        if (inputsElement.TryGetProperty("personen", out var personenElement))
        {
            await ParsePersonsAsync(scenario, personenElement);
        }
    }

    private void ParseChildcareSettings(HouseholdScenario scenario, JsonElement opvangElement)
    {
        var years = new[] { 2026, 2027, 2028, 2029, 2030 };

        foreach (var year in years)
        {
            var childcareSettings = new ChildcareSettings
            {
                Year = year,
                HouseholdScenario = scenario,
                HouseholdScenarioId = scenario.Id
            };

            if (opvangElement.TryGetProperty("opvangweken_per_jaar", out var weeksElement) &&
                weeksElement.TryGetProperty("values", out var weeksValuesElement) &&
                weeksValuesElement.TryGetProperty(year.ToString(), out var weeksValueElement))
            {
                childcareSettings.ChildcareWeeks = weeksValueElement.GetInt32();
            }

            if (opvangElement.TryGetProperty("dagen_per_week", out var daysElement) &&
                daysElement.TryGetProperty("values", out var daysValuesElement) &&
                daysValuesElement.TryGetProperty(year.ToString(), out var daysValueElement))
            {
                childcareSettings.DaysPerWeek = daysValueElement.GetInt32();
            }

            if (opvangElement.TryGetProperty("uren_per_dag", out var hoursElement) &&
                hoursElement.TryGetProperty("values", out var hoursValuesElement) &&
                hoursValuesElement.TryGetProperty(year.ToString(), out var hoursValueElement))
            {
                childcareSettings.HoursPerDay = hoursValueElement.GetDecimal();
            }

            if (opvangElement.TryGetProperty("werkelijk_uurtarief", out var rateElement) &&
                rateElement.TryGetProperty("values", out var rateValuesElement) &&
                rateValuesElement.TryGetProperty(year.ToString(), out var rateValueElement))
            {
                childcareSettings.ActualHourlyRate = rateValueElement.GetDecimal();
            }

            if (opvangElement.TryGetProperty("maximum_uurprijs", out var maxRateElement) &&
                maxRateElement.TryGetProperty("values", out var maxRateValuesElement) &&
                maxRateValuesElement.TryGetProperty(year.ToString(), out var maxRateValueElement))
            {
                childcareSettings.MaximumHourlyRate = maxRateValueElement.GetDecimal();
            }

            scenario.ChildcareSettings.Add(childcareSettings);
        }
    }

    private void ParseHomeSettings(HouseholdScenario scenario, JsonElement woningElement)
    {
        var years = new[] { 2026, 2027, 2028, 2029, 2030 };

        foreach (var year in years)
        {
            var homeSettings = new HomeSettings
            {
                Year = year,
                HouseholdScenario = scenario,
                HouseholdScenarioId = scenario.Id
            };

            if (woningElement.TryGetProperty("woz_waarde", out var wozElement) &&
                wozElement.TryGetProperty("values", out var wozValuesElement) &&
                wozValuesElement.TryGetProperty(year.ToString(), out var wozValueElement))
            {
                homeSettings.WozValue = wozValueElement.GetDecimal();
            }

            if (woningElement.TryGetProperty("eigenwoningforfait_percentage", out var forfaitElement) &&
                forfaitElement.TryGetProperty("values", out var forfaitValuesElement) &&
                forfaitValuesElement.TryGetProperty(year.ToString(), out var forfaitValueElement))
            {
                homeSettings.ImputedRentalValuePercentage = forfaitValueElement.GetDecimal();
                homeSettings.HomeImputedIncome = homeSettings.WozValue * homeSettings.ImputedRentalValuePercentage;
            }

            if (woningElement.TryGetProperty("overige_aftrek_box1", out var deductionElement) &&
                deductionElement.TryGetProperty("values", out var deductionValuesElement) &&
                deductionValuesElement.TryGetProperty(year.ToString(), out var deductionValueElement))
            {
                homeSettings.OtherBox1Deductions = deductionValueElement.GetDecimal();
            }

            scenario.HomeSettings.Add(homeSettings);
        }
    }

    private void ParsePolicyAssumptions(HouseholdScenario scenario, JsonElement beleidElement)
    {
        var years = new[] { 2026, 2027, 2028, 2029, 2030 };

        foreach (var year in years)
        {
            if (beleidElement.TryGetProperty("indexfactor_inkomensgrenzen", out var indexElement) &&
                indexElement.TryGetProperty("values", out var indexValuesElement) &&
                indexValuesElement.TryGetProperty(year.ToString(), out var indexValueElement))
            {
                var policy = new PolicyAssumption
                {
                    Key = "indexfactor_inkomensgrenzen",
                    Description = "Income boundary index factor",
                    Status = PolicyValueStatus.Provisional,
                    Year = year,
                    Value = indexValueElement.GetDecimal().ToString(),
                    Unit = "factor",
                    HouseholdScenario = scenario,
                    HouseholdScenarioId = scenario.Id
                };
                scenario.PolicyAssumptions.Add(policy);
            }

            if (beleidElement.TryGetProperty("middeninkomens_opslag", out var surchargeElement) &&
                surchargeElement.TryGetProperty("values", out var surchargeValuesElement) &&
                surchargeValuesElement.TryGetProperty(year.ToString(), out var surchargeValueElement))
            {
                var policy = new PolicyAssumption
                {
                    Key = "middeninkomens_opslag",
                    Description = "Middle income surcharge",
                    Status = PolicyValueStatus.Provisional,
                    Year = year,
                    Value = surchargeValueElement.GetDecimal().ToString(),
                    Unit = "ratio",
                    HouseholdScenario = scenario,
                    HouseholdScenarioId = scenario.Id
                };
                scenario.PolicyAssumptions.Add(policy);
            }

            if (beleidElement.TryGetProperty("toeslag_override", out var overrideElement) &&
                overrideElement.TryGetProperty("values", out var overrideValuesElement) &&
                overrideValuesElement.TryGetProperty(year.ToString(), out var overrideValueElement))
            {
                if (overrideValueElement.ValueKind != JsonValueKind.Null)
                {
                    var policy = new PolicyAssumption
                    {
                        Key = "toeslag_override",
                        Description = "Childcare benefit override percentage",
                        Status = PolicyValueStatus.UserInput,
                        Year = year,
                        Value = overrideValueElement.GetDecimal().ToString(),
                        Unit = "ratio",
                        HouseholdScenario = scenario,
                        HouseholdScenarioId = scenario.Id
                    };
                    scenario.PolicyAssumptions.Add(policy);
                }
            }
        }
    }

    private async Task ParsePersonsAsync(HouseholdScenario scenario, JsonElement personenElement)
    {
        foreach (var personElement in personenElement.EnumerateObject())
        {
            var personName = personElement.Name;
            var person = new Person
            {
                FirstName = personName,
                LastName = string.Empty,
                HouseholdScenario = scenario,
                HouseholdScenarioId = scenario.Id
            };

            // Parse person properties
            if (personElement.Value.TryGetProperty("bruto_maandsalaris", out var salaryElement) &&
                salaryElement.TryGetProperty("values", out var salaryValuesElement))
            {
                foreach (var yearElement in salaryValuesElement.EnumerateObject())
                {
                    var year = yearElement.Name;
                    var monthlySalary = yearElement.Value.GetDecimal();

                    // Create annual income for this year
                    var annualIncome = new AnnualPersonIncome
                    {
                        Year = int.Parse(year),
                        MonthlySalary = monthlySalary,
                        Person = person,
                        PersonId = person.Id,
                        HouseholdScenario = scenario,
                        HouseholdScenarioId = scenario.Id
                    };

                    // Set contract hours
                    if (personElement.Value.TryGetProperty("contracturen_week", out var contractHoursElement) &&
                        contractHoursElement.TryGetProperty("values", out var contractHoursValuesElement) &&
                        contractHoursValuesElement.TryGetProperty(year, out var contractHoursValueElement))
                    {
                        annualIncome.ContractHoursPerWeek = contractHoursValueElement.GetDecimal();
                        person.ContractHoursPerWeek = annualIncome.ContractHoursPerWeek;
                    }

                    // Set holiday allowance
                    if (personElement.Value.TryGetProperty("vakantiegeld_pct", out var holidayElement) &&
                        holidayElement.TryGetProperty("values", out var holidayValuesElement) &&
                        holidayValuesElement.TryGetProperty(year, out var holidayValueElement))
                    {
                        annualIncome.HolidayAllowancePercentage = holidayValueElement.GetDecimal();
                        person.HolidayAllowancePercentage = annualIncome.HolidayAllowancePercentage;
                    }

                    // Set car benefit
                    if (personElement.Value.TryGetProperty("fiscale_bijtelling_auto_pm", out var carBenefitElement) &&
                        carBenefitElement.TryGetProperty("values", out var carBenefitValuesElement) &&
                        carBenefitValuesElement.TryGetProperty(year, out var carBenefitValueElement))
                    {
                        annualIncome.CarBenefitMonthly = carBenefitValueElement.GetDecimal();
                        person.CarBenefitMonthly = annualIncome.CarBenefitMonthly;
                    }

                    // Set taxable car allowance
                    if (personElement.Value.TryGetProperty("belaste_autovergoeding_pm", out var taxableCarElement) &&
                        taxableCarElement.TryGetProperty("values", out var taxableCarValuesElement) &&
                        taxableCarValuesElement.TryGetProperty(year, out var taxableCarValueElement))
                    {
                        annualIncome.TaxableCarAllowanceMonthly = taxableCarValueElement.GetDecimal();
                        person.TaxableCarAllowanceMonthly = annualIncome.TaxableCarAllowanceMonthly;
                    }

                    // Set profit distribution
                    if (personElement.Value.TryGetProperty("winstuitkering_volledig_jaar", out var profitElement) &&
                        profitElement.TryGetProperty("values", out var profitValuesElement) &&
                        profitValuesElement.TryGetProperty(year, out var profitValueElement))
                    {
                        annualIncome.ProfitDistributionFullYear = profitValueElement.GetDecimal();
                        person.ProfitDistributionFullYear = annualIncome.ProfitDistributionFullYear;
                    }

                    // Set excess profit
                    if (personElement.Value.TryGetProperty("overwinst_volledig_jaar", out var excessProfitElement) &&
                        excessProfitElement.TryGetProperty("values", out var excessProfitValuesElement) &&
                        excessProfitValuesElement.TryGetProperty(year, out var excessProfitValueElement))
                    {
                        annualIncome.ExcessProfitFullYear = excessProfitValueElement.GetDecimal();
                        person.ExcessProfitFullYear = annualIncome.ExcessProfitFullYear;
                    }

                    // Set pension premium
                    if (personElement.Value.TryGetProperty("pensioenpremie_pm", out var pensionElement) &&
                        pensionElement.TryGetProperty("values", out var pensionValuesElement) &&
                        pensionValuesElement.TryGetProperty(year, out var pensionValueElement))
                    {
                        annualIncome.PensionPremiumMonthly = pensionValueElement.GetDecimal();
                        person.PensionPremiumMonthly = annualIncome.PensionPremiumMonthly;
                    }

                    // Set WIA/WGA premium
                    if (personElement.Value.TryGetProperty("wia_wga_pm", out var wiaElement) &&
                        wiaElement.TryGetProperty("values", out var wiaValuesElement) &&
                        wiaValuesElement.TryGetProperty(year, out var wiaValueElement))
                    {
                        annualIncome.WiaWgaPremiumMonthly = wiaValueElement.GetDecimal();
                        person.WiaWgaPremiumMonthly = annualIncome.WiaWgaPremiumMonthly;
                    }

                    // Set WAZO settings
                    if (personElement.Value.TryGetProperty("uwv_maximumdagloon", out var uwvMaxElement) &&
                        uwvMaxElement.TryGetProperty("values", out var uwvMaxValuesElement) &&
                        uwvMaxValuesElement.TryGetProperty(year, out var uwvMaxValueElement))
                    {
                        person.UwvMaximumDailyWage = uwvMaxValueElement.GetDecimal();
                    }

                    if (personElement.Value.TryGetProperty("uwv_wazo_pct", out var uwvWazoElement) &&
                        uwvWazoElement.TryGetProperty("values", out var uwvWazoValuesElement) &&
                        uwvWazoValuesElement.TryGetProperty(year, out var uwvWazoValueElement))
                    {
                        person.UwvWazoPercentage = uwvWazoValueElement.GetDecimal();
                    }

                    // Set manual correction
                    if (personElement.Value.TryGetProperty("handmatige_correctie_belastbaar", out var manualElement) &&
                        manualElement.TryGetProperty("values", out var manualValuesElement) &&
                        manualValuesElement.TryGetProperty(year, out var manualValueElement))
                    {
                        annualIncome.ManualTaxableIncomeCorrection = manualValueElement.GetDecimal();
                        person.ManualTaxableIncomeCorrection = annualIncome.ManualTaxableIncomeCorrection;
                    }

                    scenario.AnnualPersonIncomes.Add(annualIncome);
                }
            }

            scenario.Persons.Add(person);
        }
    }

    private void ParseMortgage(HouseholdScenario scenario, JsonElement mortgageElement)
    {
        var mortgage = new Mortgage
        {
            HouseholdScenario = scenario,
            HouseholdScenarioId = scenario.Id
        };

        if (mortgageElement.TryGetProperty("peildatum", out var dateElement))
        {
            mortgage.ReferenceDate = DateOnly.Parse(dateElement.GetString());
        }

        if (mortgageElement.TryGetProperty("calculation", out var calculationElement))
        {
            mortgage.CalculationMethod = calculationElement.GetString();
        }

        if (mortgageElement.TryGetProperty("parts", out var partsElement))
        {
            foreach (var partElement in partsElement.EnumerateArray())
            {
                var part = new MortgagePart
                {
                    Mortgage = mortgage,
                    MortgageId = mortgage.Id
                };

                if (partElement.TryGetProperty("id", out var idElement))
                {
                    part.PartNumber = idElement.GetInt32();
                }

                if (partElement.TryGetProperty("original_principal", out var principalElement))
                {
                    part.OriginalPrincipal = principalElement.GetDecimal();
                }

                if (partElement.TryGetProperty("outstanding_at_reference", out var outstandingElement))
                {
                    part.OutstandingAtReference = outstandingElement.GetDecimal();
                }

                if (partElement.TryGetProperty("interest_rate", out var rateElement))
                {
                    part.InterestRate = rateElement.GetDecimal();
                }

                if (partElement.TryGetProperty("regular_payment_month", out var paymentElement))
                {
                    part.RegularPaymentMonthly = paymentElement.GetDecimal();
                }

                if (partElement.TryGetProperty("extra_payment_month", out var extraPaymentElement))
                {
                    part.ExtraPaymentMonthly = extraPaymentElement.GetDecimal();
                }

                if (partElement.TryGetProperty("rate_revision_date", out var revisionElement))
                {
                    part.RateRevisionDate = DateOnly.Parse(revisionElement.GetString());
                }

                if (partElement.TryGetProperty("end_date", out var endDateElement))
                {
                    part.EndDate = DateOnly.Parse(endDateElement.GetString());
                }

                mortgage.Parts.Add(part);
            }
        }

        scenario.Mortgage = mortgage;
    }

    private void ParseLeavePlans(HouseholdScenario scenario, JsonElement leavePlansElement)
    {
        foreach (var personLeaveElement in leavePlansElement.EnumerateObject())
        {
            var personName = personLeaveElement.Name;
            var person = scenario.Persons.FirstOrDefault(p => p.FirstName == personName);

            if (person == null)
                continue;

            var leavePlan = new LeavePlan
            {
                Name = $"{personName} Leave Plan",
                Person = person,
                PersonId = person.Id
            };

            if (personLeaveElement.Value.TryGetProperty("contract_hours_week", out var contractHoursElement))
            {
                leavePlan.ContractHoursPerWeek = contractHoursElement.GetDecimal();
            }

            if (personLeaveElement.Value.TryGetProperty("max_paid_parental_leave_formula", out var maxPaidElement))
            {
                leavePlan.MaxPaidParentalLeaveFormula = maxPaidElement.GetString();
            }

            if (personLeaveElement.Value.TryGetProperty("child_birth_date", out var birthDateElement))
            {
                leavePlan.ChildBirthDate = DateOnly.Parse(birthDateElement.GetString());
            }

            if (personLeaveElement.Value.TryGetProperty("paid_leave_last_date", out var paidLeaveLastElement))
            {
                leavePlan.PaidLeaveLastDate = DateOnly.Parse(paidLeaveLastElement.GetString());
            }

            if (personLeaveElement.Value.TryGetProperty("work_hours_3_days", out var workHours3Element))
            {
                leavePlan.WorkHours3Days = workHours3Element.GetDecimal();
            }

            if (personLeaveElement.Value.TryGetProperty("work_hours_4_days", out var workHours4Element))
            {
                leavePlan.WorkHours4Days = workHours4Element.GetDecimal();
            }

            if (personLeaveElement.Value.TryGetProperty("work_hours_after_paid_leave", out var workHoursAfterElement))
            {
                leavePlan.WorkHoursAfterPaidLeave = workHoursAfterElement.GetDecimal();
            }

            if (personLeaveElement.Value.TryGetProperty("phases", out var phasesElement))
            {
                foreach (var phaseElement in phasesElement.EnumerateArray())
                {
                    var phase = new LeavePhase
                    {
                        LeavePlan = leavePlan,
                        LeavePlanId = leavePlan.Id
                    };

                    if (phaseElement.TryGetProperty("phase", out var phaseNumElement))
                    {
                        phase.PhaseNumber = phaseNumElement.GetInt32();
                    }

                    if (phaseElement.TryGetProperty("start", out var startElement))
                    {
                        phase.StartDate = DateOnly.Parse(startElement.GetString());
                    }

                    if (phaseElement.TryGetProperty("end", out var endElement))
                    {
                        phase.EndDate = DateOnly.Parse(endElement.GetString());
                    }

                    if (phaseElement.TryGetProperty("work_hours_week", out var workHoursElement))
                    {
                        phase.WorkHoursPerWeek = workHoursElement.GetDecimal();
                    }

                    if (phaseElement.TryGetProperty("leave_type", out var leaveTypeElement))
                    {
                        var leaveTypeStr = leaveTypeElement.GetString();
                        if (Enum.TryParse<LeaveType>(leaveTypeStr.Replace("_", ""), true, out var leaveType))
                        {
                            phase.LeaveType = leaveType;
                        }
                        else
                        {
                            phase.LeaveType = LeaveType.Other;
                        }
                    }

                    if (phaseElement.TryGetProperty("leave_hours_week", out var leaveHoursElement))
                    {
                        phase.LeaveHoursPerWeek = leaveHoursElement.GetDecimal();
                    }

                    if (phaseElement.TryGetProperty("extra_hours", out var extraHoursElement))
                    {
                        phase.ExtraHours = extraHoursElement.GetDecimal();
                    }

                    if (phaseElement.TryGetProperty("notes", out var notesElement))
                    {
                        phase.Notes = notesElement.GetString();
                    }

                    leavePlan.Phases.Add(phase);
                }
            }

            person.LeavePlan = leavePlan;
        }
    }

    private void ParseExtraIncome(HouseholdScenario scenario, JsonElement extraIncomeElement)
    {
        if (extraIncomeElement.TryGetProperty("rows", out var rowsElement))
        {
            foreach (var rowElement in rowsElement.EnumerateArray())
            {
                var extraIncome = new ExtraIncome();

                if (rowElement.TryGetProperty("year", out var yearElement))
                {
                    extraIncome.Year = yearElement.GetInt32();
                }

                if (rowElement.TryGetProperty("person", out var personElement))
                {
                    var personName = personElement.GetString();
                    var person = scenario.Persons.FirstOrDefault(p => p.FirstName == personName);
                    if (person != null)
                    {
                        extraIncome.Person = person;
                        extraIncome.PersonId = person.Id;
                    }
                }

                if (rowElement.TryGetProperty("description", out var descElement))
                {
                    extraIncome.Description = descElement.GetString();
                }

                if (rowElement.TryGetProperty("taxable_amount", out var amountElement))
                {
                    extraIncome.TaxableAmount = amountElement.GetDecimal();
                }

                if (rowElement.TryGetProperty("include", out var includeElement))
                {
                    extraIncome.Include = includeElement.GetBoolean();
                }

                // Add to person's extra incomes
                if (extraIncome.Person != null)
                {
                    extraIncome.Person.ExtraIncomes.Add(extraIncome);
                }
            }
        }
    }

    private void ParseStaticParameters(HouseholdScenario scenario, JsonElement staticParamsElement)
    {
        // Parse tax model
        if (staticParamsElement.TryGetProperty("net_income_tax_model", out var taxModelElement))
        {
            ParseTaxModel(scenario, taxModelElement);
        }
    }

    private void ParseTaxModel(HouseholdScenario scenario, JsonElement taxModelElement)
    {
        if (taxModelElement.TryGetProperty("years", out var yearsElement))
        {
            var years = new List<int>();
            foreach (var yearElement in yearsElement.EnumerateArray())
            {
                years.Add(yearElement.GetInt32());
            }

            // Parse each tax parameter
            ParseTaxBrackets(scenario, taxModelElement, years);
            ParseTaxCredits(scenario, taxModelElement, years);
        }
    }

    private void ParseTaxBrackets(HouseholdScenario scenario, JsonElement taxModelElement, List<int> years)
    {
        // This is a simplified parsing - in a full implementation, we would create TaxPolicy objects
        // For now, we'll just store the values as policy assumptions

        if (taxModelElement.TryGetProperty("bracket_1_limit", out var bracket1Element) &&
            bracket1Element.TryGetProperty("values", out var bracket1ValuesElement))
        {
            foreach (var year in years)
            {
                if (bracket1ValuesElement.TryGetProperty(year.ToString(), out var valueElement))
                {
                    var policy = new PolicyAssumption
                    {
                        Key = "bracket_1_limit",
                        Description = "Tax bracket 1 limit",
                        Status = PolicyValueStatus.Static,
                        Year = year,
                        Value = valueElement.GetDecimal().ToString(),
                        Unit = "EUR",
                        HouseholdScenario = scenario,
                        HouseholdScenarioId = scenario.Id
                    };
                    scenario.PolicyAssumptions.Add(policy);
                }
            }
        }

        // Similar parsing for other brackets...
    }

    private void ParseTaxCredits(HouseholdScenario scenario, JsonElement taxModelElement, List<int> years)
    {
        // Parse general tax credit
        if (taxModelElement.TryGetProperty("general_tax_credit_max", out var creditMaxElement) &&
            creditMaxElement.TryGetProperty("values", out var creditMaxValuesElement))
        {
            foreach (var year in years)
            {
                if (creditMaxValuesElement.TryGetProperty(year.ToString(), out var valueElement))
                {
                    var policy = new PolicyAssumption
                    {
                        Key = "general_tax_credit_max",
                        Description = "General tax credit maximum",
                        Status = PolicyValueStatus.Static,
                        Year = year,
                        Value = valueElement.GetDecimal().ToString(),
                        Unit = "EUR",
                        HouseholdScenario = scenario,
                        HouseholdScenarioId = scenario.Id
                    };
                    scenario.PolicyAssumptions.Add(policy);
                }
            }
        }

        // Similar parsing for other credits...
    }

    /// <summary>
    /// Export a scenario to JSON
    /// </summary>
    public string ExportScenarioToJson(HouseholdScenario scenario)
    {
        var exportObject = new
        {
            schemaVersion = CurrentSchemaVersion,
            applicationVersion = ApplicationVersion,
            exportedAtUtc = DateTime.UtcNow,
            scenarioId = scenario.Id,
            scenarioName = scenario.Name,
            scenarioDescription = scenario.Description,
            metadata = new
            {
                source = "ChildcareCalculator",
                exportType = "full_scenario",
                language = "nl-NL",
                currency = "EUR"
            },
            inputs = new
            {
                opvang = ExportChildcareSettings(scenario),
                woning = ExportHomeSettings(scenario),
                beleid = ExportPolicyAssumptions(scenario),
                personen = ExportPersons(scenario)
            },
            mortgage = ExportMortgage(scenario.Mortgage),
            leave_plans = ExportLeavePlans(scenario),
            extra_income = ExportExtraIncome(scenario),
            static_parameters = new { } // Could be populated from policy assumptions
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.Strict
        };

        return JsonSerializer.Serialize(exportObject, options);
    }

    private object ExportChildcareSettings(HouseholdScenario scenario)
    {
        var years = scenario.ChildcareSettings
            .GroupBy(c => c.Year)
            .Select(g => g.Key)
            .OrderBy(y => y)
            .ToList();

        return new
        {
            opvangweken_per_jaar = CreateYearValueObject(scenario.ChildcareSettings, years, c => c.ChildcareWeeks),
            dagen_per_week = CreateYearValueObject(scenario.ChildcareSettings, years, c => c.DaysPerWeek),
            uren_per_dag = CreateYearValueObject(scenario.ChildcareSettings, years, c => c.HoursPerDay),
            werkelijk_uurtarief = CreateYearValueObject(scenario.ChildcareSettings, years, c => c.ActualHourlyRate),
            maximum_uurprijs = CreateYearValueObject(scenario.ChildcareSettings, years, c => c.MaximumHourlyRate)
        };
    }

    private object ExportHomeSettings(HouseholdScenario scenario)
    {
        var years = scenario.HomeSettings
            .GroupBy(h => h.Year)
            .Select(g => g.Key)
            .OrderBy(y => y)
            .ToList();

        return new
        {
            woz_waarde = CreateYearValueObject(scenario.HomeSettings, years, h => h.WozValue),
            eigenwoningforfait_percentage = CreateYearValueObject(scenario.HomeSettings, years, h => h.ImputedRentalValuePercentage),
            overige_aftrek_box1 = CreateYearValueObject(scenario.HomeSettings, years, h => h.OtherBox1Deductions)
        };
    }

    private object ExportPolicyAssumptions(HouseholdScenario scenario)
    {
        var years = scenario.PolicyAssumptions
            .Where(p => p.Year.HasValue)
            .GroupBy(p => p.Year.Value)
            .Select(g => g.Key)
            .OrderBy(y => y)
            .ToList();

        return new
        {
            indexfactor_inkomensgrenzen = CreateYearValueObjectFromPolicy(scenario.PolicyAssumptions, years, "indexfactor_inkomensgrenzen"),
            middeninkomens_opslag = CreateYearValueObjectFromPolicy(scenario.PolicyAssumptions, years, "middeninkomens_opslag"),
            toeslag_override = CreateYearValueObjectFromPolicy(scenario.PolicyAssumptions, years, "toeslag_override")
        };
    }

    private object ExportPersons(HouseholdScenario scenario)
    {
        var personsObject = new System.Text.Json.Nodes.JsonObject();

        foreach (var person in scenario.Persons)
        {
            var personObject = new System.Text.Json.Nodes.JsonObject();

            // Add salary
            var years = scenario.AnnualPersonIncomes
                .Where(a => a.PersonId == person.Id)
                .GroupBy(a => a.Year)
                .Select(g => g.Key)
                .OrderBy(y => y)
                .ToList();

            personObject["bruto_maandsalaris"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.MonthlySalary);

            personObject["contracturen_week"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.ContractHoursPerWeek);

            personObject["vakantiegeld_pct"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.HolidayAllowancePercentage);

            personObject["fiscale_bijtelling_auto_pm"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.CarBenefitMonthly);

            personObject["belaste_autovergoeding_pm"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.TaxableCarAllowanceMonthly);

            personObject["winstuitkering_volledig_jaar"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.ProfitDistributionFullYear);

            personObject["overwinst_volledig_jaar"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.ExcessProfitFullYear);

            personObject["pensioenpremie_pm"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.PensionPremiumMonthly);

            personObject["wia_wga_pm"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.WiaWgaPremiumMonthly);

            personObject["uwv_maximumdagloon"] = CreateYearValueObject(scenario.Persons.Where(p => p.Id == person.Id).ToList(),
                years, p => p.UwvMaximumDailyWage);

            personObject["uwv_wazo_pct"] = CreateYearValueObject(scenario.Persons.Where(p => p.Id == person.Id).ToList(),
                years, p => p.UwvWazoPercentage);

            personObject["handmatige_correctie_belastbaar"] = CreateYearValueObjectFromAnnualIncomes(
                scenario.AnnualPersonIncomes.Where(a => a.PersonId == person.Id).ToList(),
                years,
                a => a.ManualTaxableIncomeCorrection);

            personsObject[person.FirstName] = personObject;
        }

        return personsObject;
    }

    private object? ExportMortgage(Mortgage? mortgage)
    {
        if (mortgage == null)
            return null;

        return new
        {
            peildatum = mortgage.ReferenceDate.ToString("yyyy-MM-dd"),
            calculation = mortgage.CalculationMethod,
            parts = mortgage.Parts.Select(p => new
            {
                id = p.PartNumber,
                original_principal = p.OriginalPrincipal,
                outstanding_at_reference = p.OutstandingAtReference,
                interest_rate = p.InterestRate,
                regular_payment_month = p.RegularPaymentMonthly,
                extra_payment_month = p.ExtraPaymentMonthly,
                rate_revision_date = p.RateRevisionDate.ToString("yyyy-MM-dd"),
                end_date = p.EndDate.ToString("yyyy-MM-dd")
            }).ToList()
        };
    }

    private object ExportLeavePlans(HouseholdScenario scenario)
    {
        var leavePlansObject = new System.Text.Json.Nodes.JsonObject();

        foreach (var person in scenario.Persons)
        {
            if (person.LeavePlan == null)
                continue;

            var leavePlanObject = new System.Text.Json.Nodes.JsonObject();

            leavePlanObject["contract_hours_week"] = person.LeavePlan.ContractHoursPerWeek;
            leavePlanObject["max_paid_parental_leave_formula"] = person.LeavePlan.MaxPaidParentalLeaveFormula;
            leavePlanObject["child_birth_date"] = person.LeavePlan.ChildBirthDate.ToString("yyyy-MM-dd");
            leavePlanObject["paid_leave_last_date"] = person.LeavePlan.PaidLeaveLastDate.ToString("yyyy-MM-dd");
            leavePlanObject["work_hours_3_days"] = person.LeavePlan.WorkHours3Days;
            leavePlanObject["work_hours_4_days"] = person.LeavePlan.WorkHours4Days;
            leavePlanObject["work_hours_after_paid_leave"] = person.LeavePlan.WorkHoursAfterPaidLeave;

            var phases = person.LeavePlan.Phases.Select(p => new
            {
                phase = p.PhaseNumber,
                start = p.StartDate.ToString("yyyy-MM-dd"),
                end = p.EndDate.ToString("yyyy-MM-dd"),
                work_hours_week = p.WorkHoursPerWeek,
                leave_type = p.LeaveType.ToString().ToLower().Replace("parental", "parental"),
                leave_hours_week = p.LeaveHoursPerWeek,
                extra_hours = p.ExtraHours,
                notes = p.Notes
            }).ToList();

            leavePlanObject["phases"] = JsonSerializer.SerializeToNode(phases);

            leavePlansObject[person.FirstName] = leavePlanObject;
        }

        return leavePlansObject;
    }

    private object ExportExtraIncome(HouseholdScenario scenario)
    {
        var rows = new List<object>();

        foreach (var person in scenario.Persons)
        {
            foreach (var extraIncome in person.ExtraIncomes)
            {
                rows.Add(new
                {
                    year = extraIncome.Year,
                    person = person.FirstName,
                    description = extraIncome.Description,
                    taxable_amount = extraIncome.TaxableAmount,
                    include = extraIncome.Include
                });
            }
        }

        return new { rows };
    }

    private object CreateYearValueObject<T>(IEnumerable<T> items, List<int> years, Func<T, object> valueSelector)
    {
        var valuesObject = new System.Text.Json.Nodes.JsonObject();
        var category = "user_input"; // Default category

        foreach (var year in years)
        {
            var item = items.FirstOrDefault(i => GetYear(i) == year);
            if (item != null)
            {
                valuesObject[year.ToString()] = JsonSerializer.SerializeToNode(valueSelector(item));
            }
        }

        return new { category, values = valuesObject, unit = GetUnit(valueSelector) };

        int GetYear(T item)
        {
            if (item is ChildcareSettings cs) return cs.Year;
            if (item is HomeSettings hs) return hs.Year;
            if (item is Person p) return 0; // Person doesn't have year, use first year
            return 0;
        }

        string GetUnit(Func<T, object> selector)
        {
            // Simplified - in a real implementation, we'd have unit metadata
            return "";
        }
    }

    private object CreateYearValueObjectFromAnnualIncomes(List<AnnualPersonIncome> incomes, List<int> years, Func<AnnualPersonIncome, object> valueSelector)
    {
        var valuesObject = new System.Text.Json.Nodes.JsonObject();

        foreach (var year in years)
        {
            var income = incomes.FirstOrDefault(i => i.Year == year);
            if (income != null)
            {
                valuesObject[year.ToString()] = JsonSerializer.SerializeToNode(valueSelector(income));
            }
        }

        return new { category = "user_input", values = valuesObject, unit = "" };
    }

    private object CreateYearValueObjectFromPolicy(IEnumerable<PolicyAssumption> policies, List<int> years, string key)
    {
        var valuesObject = new System.Text.Json.Nodes.JsonObject();
        var policy = policies.FirstOrDefault(p => p.Key == key);

        if (policy != null)
        {
            var category = policy.Category.ToString().ToLower().Replace("_", "-");

            foreach (var year in years)
            {
                var yearPolicy = policies.FirstOrDefault(p => p.Key == key && p.Year == year);
                if (yearPolicy != null && decimal.TryParse(yearPolicy.Value, out var decimalValue))
                {
                    valuesObject[year.ToString()] = JsonSerializer.SerializeToNode(decimalValue);
                }
            }

            return new { category, values = valuesObject, unit = policy.Unit };
        }

        return new { category = "policy_assumption", values = valuesObject, unit = "" };
    }
}
