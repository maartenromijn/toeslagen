using ChildcareCalculator.Domain.Models;
using ChildcareCalculator.Domain.Enums;
using ChildcareCalculator.Application.Services.Dtos;

namespace ChildcareCalculator.Application.Services;

public class IncomeCalculationService
{
    private readonly RoundingService _roundingService;

    public IncomeCalculationService(RoundingService roundingService)
    {
        _roundingService = roundingService;
    }

    /// <summary>
    /// Calculate annual contract hours from weekly contract hours
    /// Formula: contract_hours_week * 52
    /// </summary>
    public decimal CalculateAnnualContractHours(decimal contractHoursPerWeek)
    {
        return contractHoursPerWeek * CalculationConstants.WeeksPerYear;
    }

    /// <summary>
    /// Calculate normal daily wage
    /// Formula: monthly_salary * 12 * (1 + holiday_allowance_pct) / 261
    /// </summary>
    public decimal CalculateNormalDailyWage(decimal monthlySalary, decimal holidayAllowancePercentage)
    {
        var annualSalary = monthlySalary * CalculationConstants.MonthsPerYear;
        var holidayFactor = 1 + holidayAllowancePercentage;
        return annualSalary * holidayFactor / CalculationConstants.WorkingDaysPerYear;
    }

    /// <summary>
    /// Calculate WAZO daily benefit
    /// Formula: min(normal_daily_wage, uwv_maximum_daily_wage) * uwv_wazo_pct
    /// </summary>
    public decimal CalculateWazoDailyBenefit(decimal normalDailyWage, decimal uwvMaximumDailyWage, decimal uwvWazoPercentage)
    {
        var cappedDailyWage = Math.Min(normalDailyWage, uwvMaximumDailyWage);
        return cappedDailyWage * uwvWazoPercentage;
    }

    /// <summary>
    /// Calculate WAZO replacement ratio
    /// Formula: wazo_daily_benefit / normal_daily_wage
    /// </summary>
    public decimal CalculateWazoReplacementRatio(decimal wazoDailyBenefit, decimal normalDailyWage)
    {
        if (normalDailyWage == 0)
            return 0; // Avoid division by zero
        
        return wazoDailyBenefit / normalDailyWage;
    }

    /// <summary>
    /// Calculate salary after leave for a given year
    /// Formula: annual_salary * (1 - unpaid_leave_hours / annual_contract_hours - paid_wazo_hours / annual_contract_hours) 
    ///          + paid_wazo_hours / hours_per_day * wazo_daily_benefit
    /// </summary>
    public decimal CalculateSalaryAfterLeave(
        decimal annualSalary,
        decimal annualContractHours,
        decimal unpaidLeaveHours,
        decimal paidWazoHours,
        decimal wazoDailyBenefit,
        decimal hoursPerDay)
    {
        if (hoursPerDay == 0)
            return annualSalary; // Avoid division by zero
        
        var leaveFactor = 1 - (unpaidLeaveHours / annualContractHours) - (paidWazoHours / annualContractHours);
        var salaryAfterLeave = annualSalary * leaveFactor;
        var wazoBenefit = (paidWazoHours / hoursPerDay) * wazoDailyBenefit;
        
        return salaryAfterLeave + wazoBenefit;
    }

    /// <summary>
    /// Calculate profit distribution after leave
    /// Formula: 1 - unpaid_leave_hours / annual_contract_hours - paid_wazo_hours / annual_contract_hours * (1 - wazo_replacement_ratio)
    /// </summary>
    public decimal CalculateProfitFactorAfterLeave(
        decimal annualContractHours,
        decimal unpaidLeaveHours,
        decimal paidWazoHours,
        decimal wazoReplacementRatio)
    {
        var factor = 1 - (unpaidLeaveHours / annualContractHours) - 
                     (paidWazoHours / annualContractHours) * (1 - wazoReplacementRatio);
        return factor;
    }

    /// <summary>
    /// Calculate excess profit factor after leave
    /// Formula: 1 - (unpaid_leave_hours + paid_wazo_hours) / annual_contract_hours
    /// </summary>
    public decimal CalculateExcessProfitFactorAfterLeave(
        decimal annualContractHours,
        decimal unpaidLeaveHours,
        decimal paidWazoHours)
    {
        var factor = 1 - ((unpaidLeaveHours + paidWazoHours) / annualContractHours);
        return factor;
    }

    /// <summary>
    /// Calculate taxable income before home ownership adjustments
    /// Formula: employer_salary_after_leave + holiday_allowance + wazo_benefit + car_benefit*12 + 
    ///          taxable_allowance*12 + profit_after_leave + excess_profit_after_leave + 
    ///          other_taxable_income + manual_correction - employee_pension - employee_wia
    /// </summary>
    public decimal CalculateTaxableIncomeBeforeHome(
        decimal employerSalaryAfterLeave,
        decimal holidayAllowance,
        decimal wazoBenefit,
        decimal carBenefitMonthly,
        decimal taxableAllowanceMonthly,
        decimal profitAfterLeave,
        decimal excessProfitAfterLeave,
        decimal otherTaxableIncome,
        decimal manualCorrection,
        decimal employeePensionMonthly,
        decimal employeeWiaMonthly)
    {
        var carBenefitAnnual = carBenefitMonthly * CalculationConstants.MonthsPerYear;
        var taxableAllowanceAnnual = taxableAllowanceMonthly * CalculationConstants.MonthsPerYear;
        var pensionDeduction = employeePensionMonthly * CalculationConstants.MonthsPerYear;
        var wiaDeduction = employeeWiaMonthly * CalculationConstants.MonthsPerYear;

        return employerSalaryAfterLeave
            + holidayAllowance
            + wazoBenefit
            + carBenefitAnnual
            + taxableAllowanceAnnual
            + profitAfterLeave
            + excessProfitAfterLeave
            + otherTaxableIncome
            + manualCorrection
            - pensionDeduction
            - wiaDeduction;
    }

    /// <summary>
    /// Calculate annual income for a person with leave phases
    /// </summary>
    public AnnualIncomeResult CalculateAnnualPersonIncome(AnnualPersonIncomeInput input)
    {
        var result = new AnnualIncomeResult
        {
            Year = input.Year,
            PersonName = input.PersonName
        };

        // Calculate base values
        result.AnnualContractHours = CalculateAnnualContractHours(input.ContractHoursPerWeek);
        result.NormalDailyWage = CalculateNormalDailyWage(input.MonthlySalary, input.HolidayAllowancePercentage);
        result.WazoDailyBenefit = CalculateWazoDailyBenefit(
            result.NormalDailyWage, 
            input.UwvMaximumDailyWage, 
            input.UwvWazoPercentage);
        result.WazoReplacementRatio = CalculateWazoReplacementRatio(
            result.WazoDailyBenefit, 
            result.NormalDailyWage);

        // Calculate annual salary
        var annualSalary = input.MonthlySalary * CalculationConstants.MonthsPerYear;
        
        // Calculate leave hours from phases
        var (totalUnpaidLeaveHours, totalPaidWazoHours, totalWorkedHours, hoursPerDay) = 
            CalculateLeaveHoursFromPhases(input.LeavePhases, input.ContractHoursPerWeek);

        // Calculate salary after leave
        result.SalaryAfterLeave = CalculateSalaryAfterLeave(
            annualSalary,
            result.AnnualContractHours,
            totalUnpaidLeaveHours,
            totalPaidWazoHours,
            result.WazoDailyBenefit,
            hoursPerDay);

        // Calculate holiday allowance
        result.HolidayAllowance = annualSalary * input.HolidayAllowancePercentage;

        // Calculate WAZO benefit
        result.WazoBenefit = totalPaidWazoHours / hoursPerDay * result.WazoDailyBenefit;

        // Calculate car benefit annual
        result.CarBenefitAnnual = input.CarBenefitMonthly * CalculationConstants.MonthsPerYear;

        // Calculate taxable allowance annual
        result.TaxableAllowanceAnnual = input.TaxableCarAllowanceMonthly * CalculationConstants.MonthsPerYear;

        // Calculate profit after leave
        var profitFactor = CalculateProfitFactorAfterLeave(
            result.AnnualContractHours,
            totalUnpaidLeaveHours,
            totalPaidWazoHours,
            result.WazoReplacementRatio);
        result.ProfitAfterLeave = input.ProfitDistributionFullYear * profitFactor;

        // Calculate excess profit after leave
        var excessProfitFactor = CalculateExcessProfitFactorAfterLeave(
            result.AnnualContractHours,
            totalUnpaidLeaveHours,
            totalPaidWazoHours);
        result.ExcessProfitAfterLeave = input.ExcessProfitFullYear * excessProfitFactor;

        // Calculate taxable income before home
        result.TaxableIncomeBeforeHome = CalculateTaxableIncomeBeforeHome(
            result.SalaryAfterLeave,
            result.HolidayAllowance,
            result.WazoBenefit,
            input.CarBenefitMonthly,
            input.TaxableCarAllowanceMonthly,
            result.ProfitAfterLeave,
            result.ExcessProfitAfterLeave,
            input.OtherTaxableIncome,
            input.ManualTaxableIncomeCorrection,
            input.PensionPremiumMonthly,
            input.WiaWgaPremiumMonthly);

        // Calculate final taxable income (before home adjustments)
        result.TaxableIncome = result.TaxableIncomeBeforeHome;

        return result;
    }

    /// <summary>
    /// Calculate leave hours from leave phases for a specific year
    /// </summary>
    private (decimal unpaidLeaveHours, decimal paidWazoHours, decimal workedHours, decimal hoursPerDay) 
        CalculateLeaveHoursFromPhases(IEnumerable<LeavePhaseDto> phases, decimal contractHoursPerWeek)
    {
        decimal totalUnpaidLeaveHours = 0;
        decimal totalPaidWazoHours = 0;
        decimal totalWorkedHours = 0;
        decimal hoursPerDay = contractHoursPerWeek / 5; // Assuming 5 work days per week

        foreach (var phase in phases)
        {
            // Calculate weeks in phase
            var weeksInPhase = CalculateWeeksInPhase(phase.StartDate, phase.EndDate);
            
            if (phase.LeaveType == LeaveType.PaidParental)
            {
                totalPaidWazoHours += phase.LeaveHoursPerWeek * weeksInPhase;
            }
            else if (phase.LeaveType == LeaveType.UnpaidParental)
            {
                totalUnpaidLeaveHours += phase.LeaveHoursPerWeek * weeksInPhase;
            }
            
            totalWorkedHours += phase.WorkHoursPerWeek * weeksInPhase;
        }

        return (totalUnpaidLeaveHours, totalPaidWazoHours, totalWorkedHours, hoursPerDay);
    }

    /// <summary>
    /// Calculate number of weeks between two dates
    /// </summary>
    private decimal CalculateWeeksInPhase(DateOnly startDate, DateOnly endDate)
    {
        var days = (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
        return (decimal)(days / 7);
    }
}
