using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Calculators;

[Service(ServiceLifetime.Transient)]
public class RainfallCalculator : IRainfallCalculator
{
    public decimal CalculateRainfallPostApplication(ClimateDto climate, DateOnly applicationDate, DateOnly endSoilDrainageDate)
    {
        if (endSoilDrainageDate <= applicationDate)
        {
            return 0;
        }

        if (IsSameMonth(applicationDate, endSoilDrainageDate))
        {
            return Math.Ceiling(CalculateRainfallBetweenDatesInSameMonth(applicationDate, endSoilDrainageDate, climate));
        }

        decimal totalRainfall =
            CalculateRainfallForNumberOfDays(applicationDate, true, climate) +
            CalculateRainfallForNumberOfDays(endSoilDrainageDate, false, climate) +
            CalculateRainfallForWholeMonthsBetween(applicationDate, endSoilDrainageDate, climate);

        return Math.Ceiling(totalRainfall);
    }

    private static bool IsSameMonth(DateOnly firstDate, DateOnly secondDate)
    {
        return firstDate.Month == secondDate.Month && firstDate.Year == secondDate.Year;
    }

    private static decimal CalculateRainfallBetweenDatesInSameMonth(DateOnly applicationDate, DateOnly endSoilDrainageDate, ClimateDto climate)
    {
        int daysBetween = endSoilDrainageDate.Day - applicationDate.Day;
        decimal monthlyRainfall = GetMonthlyRainfall(applicationDate.Month, climate);

        return monthlyRainfall / DateTime.DaysInMonth(applicationDate.Year, applicationDate.Month) * daysBetween;
    }

    private static decimal CalculateRainfallForNumberOfDays(DateOnly date, bool isStartDate, ClimateDto climate)
    {
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        int daysBetween = isStartDate ? daysInMonth - date.Day : date.Day;
        return (daysInMonth == daysBetween) ? GetMonthlyRainfall(date.Month, climate) : (GetMonthlyRainfall(date.Month, climate) / daysInMonth) * daysBetween;
    }

    private static decimal CalculateRainfallForWholeMonthsBetween(DateOnly applicationDate, DateOnly endSoilDrainageDate, ClimateDto climate)
    {
        decimal rainfall = 0;
        DateOnly month = new DateOnly(applicationDate.Year, applicationDate.Month, 1).AddMonths(1);
        DateOnly endMonth = new DateOnly(endSoilDrainageDate.Year, endSoilDrainageDate.Month, 1);

        while (month < endMonth)
        {
            rainfall += GetMonthlyRainfall(month.Month, climate);
            month = month.AddMonths(1);
        }

        return rainfall;
    }

    private static decimal GetMonthlyRainfall(int month, ClimateDto climate)
    {
        return month switch
        {
            1 => climate.MeanTotalRainFallJan,
            2 => climate.MeanTotalRainFallFeb,
            3 => climate.MeanTotalRainFallMar,
            4 => climate.MeanTotalRainFallApr,
            5 => climate.MeanTotalRainFallMay,
            6 => climate.MeanTotalRainFallJun,
            7 => climate.MeanTotalRainFallJul,
            8 => climate.MeanTotalRainFallAug,
            9 => climate.MeanTotalRainFallSep,
            10 => climate.MeanTotalRainFallOct,
            11 => climate.MeanTotalRainFallNov,
            12 => climate.MeanTotalRainFallDec,
            _ => 0
        };
    }
}

