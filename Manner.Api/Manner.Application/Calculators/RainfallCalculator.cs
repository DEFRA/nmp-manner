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

        decimal totalRainfall = 0;

        decimal startMonthRainfall = CalculateProportionalRainfall(applicationDate, true, climate);
        decimal endMonthRainfall = CalculateProportionalRainfall(endSoilDrainageDate, false, climate);

        totalRainfall += startMonthRainfall + endMonthRainfall;

        int startMonthIndex = applicationDate.Month;
        int endMonthIndex = endSoilDrainageDate.Month;

        if (endMonthIndex > startMonthIndex)
        {
            for (int i = startMonthIndex; i < endMonthIndex - 1; i++)
            {
                totalRainfall += GetMonthlyRainfall(i + 1, climate);
            }
        }

        return Math.Ceiling(totalRainfall);
    }

    private decimal CalculateProportionalRainfall(DateOnly date, bool isStartMonth, ClimateDto climate)
    {
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        decimal proportion = isStartMonth
            ? 1 - ((decimal)date.Day / daysInMonth)
            : (decimal)date.Day / daysInMonth;

        return GetMonthlyRainfall(date.Month, climate) * proportion;
    }

    private decimal GetMonthlyRainfall(int month, ClimateDto climate)
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

