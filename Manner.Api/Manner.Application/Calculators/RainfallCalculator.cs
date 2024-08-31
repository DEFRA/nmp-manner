using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Calculators;

[Service(ServiceLifetime.Transient)]
public class RainfallCalculator : IRainfallCalculator
{
    public decimal CalculateRainfallPostApplication(ClimateDto climate, DateOnly applicationDate, DateOnly endOfSoilDrainageDate)
    {
        if (endOfSoilDrainageDate <= applicationDate)
        {
            return 0;
        }

        decimal totalRainfall = 0;

        // Calculate proportional rainfall for the start and end months
        decimal startMonthRainfall = CalculateProportionalRainfall(applicationDate, true, climate);
        decimal endMonthRainfall = CalculateProportionalRainfall(endOfSoilDrainageDate, false, climate);

        totalRainfall += startMonthRainfall + endMonthRainfall;

        // Get the month and year of the application and end dates
        int startMonthIndex = applicationDate.Month;
        int endMonthIndex = endOfSoilDrainageDate.Month;
        int startYear = applicationDate.Year;
        int endYear = endOfSoilDrainageDate.Year;

        // Handle if the start and end dates are in different years
        if (endYear > startYear || (endYear == startYear && endMonthIndex > startMonthIndex))
        {
            // Add rainfall for the months between the application date and end soil drainage date
            for (int year = startYear; year <= endYear; year++)
            {
                int monthStart = (year == startYear) ? startMonthIndex + 1 : 1;
                int monthEnd = (year == endYear) ? endMonthIndex - 1 : 12;

                for (int month = monthStart; month <= monthEnd; month++)
                {
                    totalRainfall += GetMonthlyRainfall(month, climate);
                }
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

