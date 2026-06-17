using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Application.Enums;
using Microsoft.Extensions.DependencyInjection;
using Manner.Core.Attributes;

namespace Manner.Application.Calculators;

public class MannerCalculator(MannerCalculatorInput input) : IMannerCalculator
{
    private readonly FieldDetail _field = input.Field;
    private readonly ManureApplication _manureApplication = input.ManureApplication;
    private readonly ManureTypeDto _manureType = input.ManureType;
    private readonly ClimateDto _climate = input.Climate;
    private readonly CropTypeDto _cropType = input.CropType;
    private readonly IncorporationDelayDto? _incorporationDelay = input.IncorporationDelay;
    private readonly TopSoilDto _topSoil = input.TopSoil;
    private readonly SubSoilDto _subSoil = input.SubSoil;
    private readonly List<ClimateTypeDto> _climateTypes = input.ClimateTypes;
    private readonly DTOs.Outputs _outputs = new();
    private readonly ClimateCalculator _climateCalculator = new();
#pragma warning disable S1450
    private double _rainfallTotal;
#pragma warning restore S1450
    private double _evapotranspirationTotal;
    private readonly int _runType = input.RunType;
    private const string _arableKey = "Arable";
    private const string _grassKey = "Grass";
    public DTOs.Outputs MannerEngine
    {
        get
        {
            return _outputs;
        }
    }
    public void Calculate()
    {
        CalculateClimate();
        var mineralN1 = default(double);
        double mineralN2;
        double mineralN3;
        double mineralN4; // manure N remaining after losses through NH3 volatilisation + any nitrate N in the original manure application
        int incorporationCumulativeHours = _incorporationDelay?.CumulativeHours ?? 0;

        string cropUse = _cropType?.Use ?? string.Empty;
        if (_manureType != null)
        {
            this.CalculateNutrientsOutputsValues();

            // Available N
            // --------------------------------------------------------------
            double calculatedTotalN = (double)(_manureApplication.ApplicationRate.Value * _manureType.TotalN);
            // Readily Available N applied (NH4-N and uric acid N)
            // --------------------------------------------------------------
            // 18 Jan 2013 - Lizzie says "CalcPot = AppRate * (TotalAmmN + TotalUricN + TotalNitrateN)"
            double calculatedPotentialN = (double)(_manureApplication.ApplicationRate.Value * (_manureType.NH4N + _manureType.Uric + _manureType.NO3N));

            double potentialNAvailable = (double)(_manureApplication.ApplicationRate.Value * (_manureType.NH4N + _manureType.Uric));

            // Volatilised N
            // --------------------------------------------------------------
            double calculatedVolatilisedN = this.CalculateAmmoniaVolatilisation(potentialNAvailable, cropUse, incorporationCumulativeHours);

            // N2O Emission
            // --------------------------------------------------------------
            // N2O Emission is 1.74% of applied readily available N remaining following volatilisation
            double n2oEmission = calculatedTotalN - calculatedVolatilisedN;
            double calculatedN2O = this.CalculateN2OEmission(n2oEmission);

            // N2 Emission
            // --------------------------------------------------------------
            double calculatedN2 = this.CalculateN2Emission(calculatedN2O);

            // Autumn Crop Uptake - crop N value in kg/ha which is subtracted before mineralisation and leaching
            // --------------------------------------------------------------
            // Total nitrate N added here following conversation with F.Nicholson on 30/08/2006
            mineralN2 = calculatedPotentialN - calculatedVolatilisedN - calculatedN2 - calculatedN2O;
            if (mineralN2 < 0d)
                mineralN2 = 0d;

            double calculatedcropUptakeFactor = this.CalculateCropUptakeFactor(mineralN2, _manureApplication.ApplicationDate.Month);

            if (mineralN2 < calculatedcropUptakeFactor)
            {
                mineralN3 = 0d;
            }
            else
            {
                mineralN3 = mineralN2 - calculatedcropUptakeFactor;
            }

            // Mineralised N
            // --------------------------------------------------------------
            var mineralisedResult = this.CalculateMineralisedN(calculatedTotalN, calculatedPotentialN);
            double calculatedMineralisedN = mineralisedResult.MineralisedN;
            var mineralisedN2A = mineralisedResult.MineralisedN2A;
            var mineralisedN3 = mineralisedResult.OrganicN3;
            var cdd1 = mineralisedResult.Cdd1;
            var cdd2 = mineralisedResult.Cdd2;
            var cdd2a = mineralisedResult.Cdd2A;
            mineralN1 = mineralisedResult.MineralN1;

            mineralN4 = mineralN3 + mineralN1;
            // Leached N
            // -------------------------------------------------------------
            // Calculate soil volumetric water content
            double vmWaterTopSoil = _topSoil.VolumetricMeasure;
            double vmWaterTotal = vmWaterTopSoil + _subSoil.VolumetricMeasure;

            double calculatedLeachedN = CalculateLeachedN(mineralN4, vmWaterTotal, vmWaterTopSoil);
            double nMineralised4 = mineralN4 - calculatedLeachedN;

            // Modification required to multiply mineralisation by 2 for poultry only.
            calculatedMineralisedN *= ApplyMineralisationFactor();
            mineralisedN2A *= ApplyMineralisationFactor();

            // Calculate final results and assign to public variables
            CalculateFinalResults(calculatedTotalN, calculatedPotentialN, calculatedVolatilisedN, calculatedN2O, calculatedN2, calculatedcropUptakeFactor, calculatedMineralisedN, calculatedLeachedN);


            if (IsPaperCrumble(_manureType.ID))
            {
                CalculateNAvailableResultsPaperCrumble();
            }
            else
            {
                switch (cropUse ?? "")
                {
                    case _grassKey:
                        {
                            CalculateNAvailableResultsGrass(mineralN3, mineralisedN2A, calculatedcropUptakeFactor, calculatedMineralisedN, calculatedLeachedN);
                            break;
                        }
                    case _arableKey:
                    default:
                        {
                            _outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN + calculatedcropUptakeFactor) * 10.0d) / 10d;
                            _outputs.ResultantNAvailableSecondCut = 0d;
                            break;
                        }
                }
            }

            // -----------------------------------------------------------------------------------
            // Now need to consider what is going on for the following crop year, i.e. year 2.
            // Pass through the mineralisation from year 1 so that this can be used to calculate
            // the Organic N remaining.
            // -----------------------------------------------------------------------------------
            // Mineralised N for next crop
            // --------------------------------------------------------------
            double calculatedMineralisedNNextCrop = CalculateMineralisedNNextCrop(nMineralised4, vmWaterTotal, cdd1, cdd2, cdd2a, mineralisedN3);
            _outputs.ResultantNAvailableYear2 = (double)(int)Math.Round(calculatedMineralisedNNextCrop * 10.0d / 10d);

            CheckAndChangeNegativeNResults();
        }
        else
        {
            throw new InvalidOperationException("Manure not found");
        }

    }

    // Aug 2012  C Lam: added IsCalcRainfall parameter, to allow a user supplied value to be used
    private void CalculateClimate(bool haveSuppliedOwnClimateData = false, bool isCalcRainfall = true)
    {

        if (_climate != null)
        {
            _climateCalculator.GetClimate(_climate, _field.CropTypeID, _field.TopsoilID, _field.SubsoilID, _topSoil.AvailableWaterCapacity, _subSoil.AvailableWaterCapacity, haveSuppliedOwnClimateData);
        }

        if (isCalcRainfall && _manureApplication.ApplicationDate.Day > 0 && _manureApplication.EndOfDrainageDate.Day > 0)
        {
            this.CalculateRainfall(_manureApplication.ApplicationDate, _manureApplication.EndOfDrainageDate);
        }
    }

    /// <summary>
    /// CalcRainfall called to update Total Rainfall and Total Evap updates MannerTotalRain and MannerTotalEvap public variables
    /// Called when Application Date or End of Soil Drainage dates are changed Receives ApplicationDate and End of Soil Drainage Dates
    /// Works off monthly rainfall and AE values in climate array.
    /// </summary>
    /// <param name="applicationDate"></param>
    /// <param name="endSoilDrainageDate"></param>
    private void CalculateRainfall(DateOnly applicationDate, DateOnly endSoilDrainageDate)
    {
        // DO NOT ADD ONE MONTH TO THE DATE OF APPLICATION TO MIMIC EXISTING CODE AND MANNER PAPER
        DateTime appDate = new DateTime(applicationDate.Year, applicationDate.Month, applicationDate.Day, 0, 0, 0, DateTimeKind.Local);
        DateTime endDate = new DateTime(endSoilDrainageDate.Year, endSoilDrainageDate.Month, endSoilDrainageDate.Day, 0, 0, 0, DateTimeKind.Local);

        // #### NOTE -    Any manure application AFTER 31/07/98 is associated with the next years End of Soil Drainage
        if ((endDate - appDate).Days <= 0)
        {
            _rainfallTotal = 0d;
            _evapotranspirationTotal = 0d;
            return;
        }

        double appDateRain = GetClimateType(applicationDate.Month, _climateCalculator, Enumerations.ClimateDataType.Rainfall);
        double applicationDateAE = GetClimateType(applicationDate.Month, _climateCalculator, Enumerations.ClimateDataType.ActualEvapotranspiration);
        double soilDrainageRain = GetClimateType(endSoilDrainageDate.Month, _climateCalculator, Enumerations.ClimateDataType.Rainfall);
        double soilDrainageAE = GetClimateType(endSoilDrainageDate.Month, _climateCalculator, Enumerations.ClimateDataType.ActualEvapotranspiration);

        double sumRain = 0d;
        double sumEvap = 0d;
        double propstart = GetMonthProgress(appDate);
        double propend = GetMonthProgress(endDate);
        int monthDifference = GetMonthDifference(appDate, endDate);

        if (monthDifference > 0)
        {
            AddBoundedRainAndEvap(appDateRain, applicationDateAE, 1.0d - propstart, ref sumRain, ref sumEvap);
            AddBoundedRainAndEvap(soilDrainageRain, soilDrainageAE, propend, ref sumRain, ref sumEvap);
        }
        else if (monthDifference == 0)
        {
            AddBoundedRainAndEvap(soilDrainageRain, soilDrainageAE, propend - propstart, ref sumRain, ref sumEvap);
        }

        while (GetMonthDifference(appDate, endDate) > 1)
        {
            appDate = appDate.AddMonths(1);
            appDateRain = GetClimateType(appDate.Month, _climateCalculator, Enumerations.ClimateDataType.Rainfall);
            applicationDateAE = GetClimateType(appDate.Month, _climateCalculator, Enumerations.ClimateDataType.ActualEvapotranspiration);
            AddBoundedRainAndEvap(appDateRain, applicationDateAE, 1d, ref sumRain, ref sumEvap);
        }

        // always round up
        _rainfallTotal = (double)(long)Math.Round(sumRain + 0.5d);

        if (_rainfallTotal < 0d)
        {
            _rainfallTotal = 0d;
        }

        _evapotranspirationTotal = (double)(long)Math.Round(sumEvap + 0.5d);
    }

    private static int GetMonthDifference(DateTime startDate, DateTime endDate)
    {
        return ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
    }

    private static double GetMonthProgress(DateTime date)
    {
        var calendar = Thread.CurrentThread.CurrentCulture.Calendar;
        int year = calendar.GetYear(date);
        int month = calendar.GetMonth(date);
        double day = calendar.GetDayOfMonth(date);
        double daysInMonth = (new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Local).AddMonths(1) - new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Local)).TotalDays;

        return day / daysInMonth;
    }

    private static void AddBoundedRainAndEvap(double rain, double evapotranspiration, double proportion, ref double sumRain, ref double sumEvap)
    {
        double rainAmount = rain * proportion;
        sumRain += rainAmount;
        sumEvap += Math.Min(evapotranspiration, rain) * proportion;
    }

    private void CheckAndChangeNegativeNResults()
    {
        if (_manureType.ID != (int)Enums.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated && _outputs.ResultantNAvailable < 0d)
        {
            _outputs.ResultantNAvailable = 0d;
        }

        if (_outputs.MineralisedN < 0d)
        {
            _outputs.MineralisedN = 0d;
        }
        if (_outputs.ResultantNAvailableYear2 < 0d)
        {
            _outputs.ResultantNAvailableYear2 = 0d;
        }
        if (_outputs.ResultantNAvailableSecondCut < 0d)
        {
            _outputs.ResultantNAvailableSecondCut = 0d;
        }
    }

    /// <summary>
    /// Calculates the mineralised N available to the following crop.
    /// </summary>
    /// <param name="mineralisedN4">Mineralised N</param>
    /// <param name="volumetricWaterContentTotal"></param>
    /// <param name="cdd1">Cumulative Day Degrees for the months between Date of Application and 31st December in the first</param>
    /// <param name="cdd2">Cumulative Day Degrees for the months between 1st January (or Date of Application if the manure was applied between 1st January and 30th April) and 30th April</param>
    /// <param name="cdd2a">Cumulative Day Degrees for the months between Date of Application and 31st July</param>
    /// <param name="organicN3">Organic N remaining</param>
    /// <returns type="Double">Mineralised N value for next crop</returns>
    private double CalculateMineralisedNNextCrop(double mineralisedN4, double volumetricWaterContentTotal, double cdd1, double cdd2, double cdd2a, double organicN3)
    {

        try
        {

            string cropUse = _cropType.Use;

            // Step 1 - Calculate CDD3 between 1st August and 31 December
            // -----------------------------------------------------------------------------------------
            // No need to do calculations for the following because the numbers won't change for following crop
            // dCDD3 will always be equal to 811
            double dCDD3 = 811d;

            // NOrg3 is calculated in CalcMinN and set to a module level variable to make things easier.
            double dNOrganic3 = organicN3;

            // -----------------------------------------------------------------------------------------
            // Step 2 - Calculate NMin3
            // -----------------------------------------------------------------------------------------
            double dNMineralised3 = CalculateMineralisedNForPeriod(dCDD3, dNOrganic3);

            // '*****************************************************************************************
            // 'NMin3 now needs to go through the leaching module
            // '*****************************************************************************************
            double leachedFollowingCrop = CalculateLeachedNNext(dNMineralised3, volumetricWaterContentTotal);
            // Any N not leached becomes available for the following crop (NMin4)
            double dNMineralised4 = dNMineralised3 - leachedFollowingCrop;

            if (dNMineralised4 < 0d)
                dNMineralised4 = 0d;

            // -----------------------------------------------------------------------------------------
            // Step 3 - Calculate the amount of organic N remaining (NOrg4)
            // -----------------------------------------------------------------------------------------
            double dNOrganic4 = dNOrganic3 - dNMineralised3;

            // -----------------------------------------------------------------------------------------
            // Step 4 - Calculate CDD (CDD4) for the months between Date of End of Drainage in
            // Year 2 and End of Crop Uptake (ECU) in year 2.
            // -----------------------------------------------------------------------------------------
            var dCDD4 = default(double);
            if (cropUse == _grassKey)
            {
                dCDD4 = 100d;
            }
            else if (cropUse == _arableKey)
            {
                dCDD4 = 886d;
            }

            // -----------------------------------------------------------------------------------------
            // Step 5 - Calculate the total CDD to date
            // -----------------------------------------------------------------------------------------
            double dTotalCDD = cdd1 + cdd2 + cdd2a + dCDD3 + dCDD4;

            // -----------------------------------------------------------------------------------------
            // Step 6 - Calculate NMin5
            // -----------------------------------------------------------------------------------------
            double dNMineralised5;
            if (dTotalCDD < 2300d)
            {
                dNMineralised5 = CalculateMineralisedNForPeriod(dCDD4, dNOrganic4);
            }
            else
            {
                dNMineralised5 = CalculateMineralisedNForPeriod(dCDD4, dNOrganic4, 0.001223d, 0.00134d);

            }

            // -----------------------------------------------------------------------------------------
            // Step 7 - Calculate the amount of mineralised N available to the following crop
            // -----------------------------------------------------------------------------------------

            return dNMineralised4 + dNMineralised5;
        }

        catch (Exception)
        {
            return 0d;
        }

    }

    // ********************************************************************************
    // ** Method:         CalcLeachedNNext
    // ** Created:        Martina Gibbons 30/08/06 following conversation with F.Nicholson about how the leaching for the second year is
    // dealt with by the leaching module  Rewritten following code changes by E. Lord as per email on 30/06/07.
    // ** Parameters:     Nmin3 as double.  Pool of mineralised N susceptible to 
    // leaching for the following crop (Crop Year 2).
    // ** Return Value:   Double - Leached N for the following crop
    // ** Description:    Called from CalcMinNNext.  Calculates the leached N of the mineralised N susceptible to leaching for the following crop.
    // ********************************************************************************
    /// <summary>
    /// Calculates the leached N of the mineralised N susceptible to leaching for the following crop.
    /// </summary>
    /// <param name="nmin3"></param>
    /// <param name="vmTotal"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private double CalculateLeachedNNext(double nmin3, double vmTotal)
    {
        try
        {

            int iMonthApp;
            int iMonthEOD;
            int k;
            double dMinN4;
            double dVMTotal;
            double dHER;
            // Mostly matrix algorithm variables
            double dLRatio;
            double dLProp;
            dHER = 0d;

            dMinN4 = nmin3;      // passed into the function
            dVMTotal = vmTotal;  // also passed into the function

            // NOTE: Nitrification not required for the following crop

            // -------------------------------------------------------------------------------------------
            // Calculate the HER from effective application date to end of drainage.
            // Following conversation with F. Nicholson the effective application date
            // for crop year 2 is 1st August so month of application is set to August.
            // -------------------------------------------------------------------------------------------
            iMonthApp = 8;

            // Calculate from August to 31st of December
            while (iMonthApp < 13)
            {
                dHER += GetHer(iMonthApp);
                iMonthApp += 1;
            }

            // -------------------------------------------------------------------------------------------
            // and then calculate HER for the months up to the end of soil drainage
            // end of soil drainage will always be 31st March c.f. Mineralisation Technical Guide.
            // -------------------------------------------------------------------------------------------
            iMonthEOD = 3;

            var loopTo = iMonthEOD;
            for (k = 1; k <= loopTo; k++)
                dHER += GetHer(k);

            // Calculate 'leaching ratio'
            // check for divide by zero error
            if (dVMTotal <= 0d)
            {
                dLRatio = 0d;
            }
            else
            {
                // calculate leaching ratio dlRatio, constrained to be between 0 and 1.33
                dLRatio = Math.Max(0d, Math.Min(1.33d, dHER / dVMTotal));
            }

            // -------------------------------------------------------------------------------------------
            // Apply the SLIMMER function:
            // -------------------------------------------------------------------------------------------
            dLProp = 1.11d * dLRatio - 0.203d * dLRatio * dLRatio * dLRatio;

            // Constrain the result to lie between 0 and 1
            dLProp = Math.Max(0d, Math.Min(1d, dLProp));

            // -------------------------------------------------------------------------------------------
            // Return the value of the leached N for Nitrogen Crop Year 2
            // -------------------------------------------------------------------------------------------

            return dMinN4 * dLProp;
        }

        catch (Exception)
        {
            return 0d;
        }
    }

    private double GetHer(int month)
    {
        double iHer;
        var climateType = _climateTypes.FirstOrDefault(c => c.MonthNumber == month);
        iHer = Convert.ToDouble(climateType?.HER ?? 0m);
        return iHer;
    }

    private void CalculateFinalResults(double calculatedTotalN, double calculatedPotentialN, double calculatedVolatilisedN, double calculatedN2O, double calculatedN2, double cropUptakeFactor, double calculatedMineralisedN, double calculatedLeachedN)
    {
        _outputs.TotalNitrogenApplied = (long)Math.Round(calculatedTotalN * 10.0d) / 10d;
        _outputs.PotentialCropAvailableN = (int)Math.Round(calculatedPotentialN * 10.0d) / 10d;
        _outputs.NH3NLoss = (int)Math.Round(calculatedVolatilisedN * 10.0d) / 10d;
        _outputs.N2ONLoss = (int)Math.Round(calculatedN2O * 10.0d) / 10d;
        _outputs.N2NLoss = (int)Math.Round(calculatedN2 * 10.0d) / 10d;
        _outputs.MineralisedN = (int)Math.Round(calculatedMineralisedN * 10.0d) / 10d;
        _outputs.NO3NLoss = (int)Math.Round(calculatedLeachedN * 10.0d) / 10d;
        _outputs.CropUptake = cropUptakeFactor;
    }
    private void CalculateNutrientsOutputsValues()
    {
        _outputs.P2O5Total = Convert.ToDouble(_manureType.P2O5 * _manureApplication.ApplicationRate.Value);
        var percentageP2O5Available = _manureType.P2O5Available / 100m;
        _outputs.P2O5CropAvailable = Convert.ToDouble(_manureType.P2O5 * _manureApplication.ApplicationRate.Value * percentageP2O5Available);
        _outputs.K2OTotal = Convert.ToDouble(_manureType.K2O * _manureApplication.ApplicationRate.Value);
        var percentageK2OCropAvailable = _manureType.K2OAvailable / 100m;
        _outputs.K2OCropAvailable = Convert.ToDouble(_manureType.K2O * _manureApplication.ApplicationRate.Value * percentageK2OCropAvailable);
        _outputs.MgOTotal = Convert.ToDouble(_manureType.MgO * _manureApplication.ApplicationRate.Value);
        _outputs.SO3Total = Convert.ToDouble(_manureType.SO3 * _manureApplication.ApplicationRate.Value);
        _outputs.SO3CropAvailable = CalculateSO3CropAvailable();
    }

    private double? CalculateSO3CropAvailable()
    {
        double? so3 = null;
        if (_manureApplication.ApplicationDate.Month >= 8 && _manureApplication.ApplicationDate.Month <= 12)
        {
            if (_cropType.ID == (int)Enumerations.CropTypeEnum.Grass || _cropType.ID == (int)Enumerations.CropTypeEnum.SpringCerealOilseedRape || _cropType.ID == (int)Enumerations.CropTypeEnum.EarlyEstablishedWinterOilseedRape || _cropType.ID == (int)Enumerations.CropTypeEnum.LateEstablishedWinterOilseedRape)
            {
                if (_manureType.SO3AvaiableAutumnOsrGrass > 0)
                {
                    var percentageSO3AvaiableAutumnOsrGrass = _manureType.SO3AvaiableAutumnOsrGrass / 100m;
                    return Convert.ToDouble(_manureType.SO3 * _manureApplication.ApplicationRate.Value * percentageSO3AvaiableAutumnOsrGrass);
                }
            }
            else if (_manureType.SO3AvaiableAutumnOther > 0)
            {
                var percentageSO3AvaiableAutumnOther = _manureType.SO3AvaiableAutumnOther / 100m;
                return Convert.ToDouble(_manureType.SO3 * _manureApplication.ApplicationRate.Value * percentageSO3AvaiableAutumnOther);
            }
        }
        else if (_manureType.SO3AvailableSpring > 0)
        {
            var percentageSO3AvailableSpring = _manureType.SO3AvailableSpring / 100m;
            return Convert.ToDouble(_manureType.SO3 * _manureApplication.ApplicationRate.Value * percentageSO3AvailableSpring);
        }
        return so3;

    }


    private void CalculateNAvailableResultsGrass(double mineralN3, double nMineralised2A, double calculatedcropUptakeFactor, double calculatedMineralisedN, double calculatedLeachedN)
    {
        switch (_manureApplication.ApplicationDate.Month)
        {
            case (byte)1:
            case (byte)2:
            case (byte)3:
            case (byte)4:
                {
                    _outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN) * 10.0d) / 10d;
                    _outputs.ResultantNAvailableSecondCut = (int)Math.Round(nMineralised2A * 10.0d) / 10d;
                    break;
                }
            case (byte)5:
            case (byte)6:
            case (byte)7:
                {
                    _outputs.MineralisedN = (int)Math.Round(nMineralised2A * 10.0d) / 10d;
                    _outputs.ResultantNAvailable = (int)Math.Round((mineralN3 + nMineralised2A) * 10.0d) / 10d;
                    _outputs.ResultantNAvailableSecondCut = 0d;
                    break;
                }
            case (byte)8:
            case (byte)9:
            case (byte)10:
            case (byte)11:
            case (byte)12:
                {
                    _outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN + calculatedcropUptakeFactor) * 10.0d) / 10d;
                    _outputs.ResultantNAvailableSecondCut = (int)Math.Round(nMineralised2A * 10.0d) / 10d;
                    break;
                }
        }
    }


    private void CalculateNAvailableResultsPaperCrumble()
    {
        if (_manureType.ID == (int)Enums.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated)
        {
            _outputs.ResultantNAvailable = -0.8d * (double)_manureApplication.ApplicationRate.Value;
        }
        else
        {
            _outputs.ResultantNAvailable = 0d;
        }

        _outputs.ResultantNAvailableSecondCut = 0d;
        _outputs.ResultantNAvailableYear2 = 0d;
    }

    private double CalculateAmmoniaVolatilisation(double potentialNAvailable, string cropuse, int incorporationCumulativeHours)
    {
        double pvn0 = potentialNAvailable * ((double)_manureType.NMaxConstant / 100);
        // Soil moisture adjustment (cattle slurry and liquid digested sludge only)
        double pvn1 = ApplySoilMoistureAdjustment(pvn0);

        double pvn2 = ApplyLandUseAdjustment(pvn1, cropuse);
        double pvn3 = ApplyDryMatterAdjustment(pvn2);
        double pvn4 = ApplyApplicationTechniqueAdjustment(pvn3);
        double pvn5 = ApplyWindSpeedAdjustment(pvn4);

        (double pvn7, double temp1) = ApplyRainfallAdjustment(pvn5, incorporationCumulativeHours);

        (double pvn8, double temp2) = ApplyIncorporationTimingAdjustment(pvn7, incorporationCumulativeHours);

        double pvn9 = ApplyIncorporationTechniqueAdjustment(pvn8);

        // Total ammonia lost (kg/ha)
        return temp1 + temp2 + pvn9;
    }

    private double ApplySoilMoistureAdjustment(double pvn0)
    {
        
        if (!IsCattleOrLiquidSludge())
        {
            return pvn0;
        }

        return _manureApplication.TopsoilMoistureID switch
        {
            (int)Enums.Enumerations.TopsoilMoistureEnum.Dry => pvn0 * 1.3d,
            (int)Enums.Enumerations.TopsoilMoistureEnum.Moist => pvn0 * 0.7d,
            _ => pvn0
        };
    }

    private double ApplyLandUseAdjustment(double pvn1, string cropuse)
    {
        if (!IsCattleOrLiquidSludge())
        {
            return pvn1;
        }

        if (cropuse == _arableKey)
        {
            return pvn1 * 0.85d;
        }

        if (cropuse == _grassKey)
        {
            return pvn1 * 1.15d;
        }

        return pvn1;
    }

    private double ApplyDryMatterAdjustment(double pvn2)
    {
        bool isMoist = _manureApplication.TopsoilMoistureID == (int)Enums.Enumerations.TopsoilMoistureEnum.Moist;

        if (!isMoist)
        {
            return pvn2;
        }

        if (IsCattleOrLiquidSludge())
        {
            return ((8.3d * (double)_manureType.DryMatter + 50.2d) / 100d) * pvn2;
        }

        if (_manureType.ManureTypeCategoryID ==
            (int)Enums.Enumerations.ManureCategory.PigSlurry)
        {
            return ((12.3d * (double)_manureType.DryMatter + 50.8d) / 100d) * pvn2;
        }

        return pvn2;
    }

    private double ApplyApplicationTechniqueAdjustment(double pvn3)
    {
        if (!_manureType.IsLiquid)
        {
            return pvn3;
        }

        double proportion = GetApplicationMethodProportion();

        return pvn3 * proportion;
    }

    private double GetApplicationMethodProportion()
    {
        bool isDigestate =
            _manureType.ID ==
            (int)Enums.Enumerations.ManureTypes.DigestateWholeFoodBased;

        return _manureApplication.ApplicationMethodID switch
        {
            (int)Enums.Enumerations.ApplicationMethodEnum.DeepInjection => 0.1d,

            (int)Enums.Enumerations.ApplicationMethodEnum.ShallowInjection =>
                isDigestate ? 0.55d : 0.3d,

            (int)Enums.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingHose =>
                isDigestate ? 0.55d : 0.7d,

            (int)Enums.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingShoeShortGrass =>
                isDigestate ? 0.55d : 0.7d,

            (int)Enums.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingShoeLongGrass =>
                isDigestate ? 0.31d : 0.4d,

            _ => 1d
        };
    }

    private double ApplyWindSpeedAdjustment(double pvn4)
    {
        if (!IsSlurryType())
        {
            return pvn4;
        }

        return _manureApplication.WindspeedID switch
        {
            (int)Enums.Enumerations.WindSpeed.Moderate4to5BeaufortScale => pvn4 * 1.2d,

            (int)Enums.Enumerations.WindSpeed.StrongBreeze6to7BeaufortScale => pvn4 * 1.6d,

            _ => pvn4
        };
    }

    private (double pvn7, double temp1) ApplyRainfallAdjustment(double pvn5,int incorporationHours)
    {
        if (!IsSlurryType())
        {
            return (pvn5, 0d);
        }

        return _manureApplication.RainTypeID switch
        {
            (int)Enums.Enumerations.Rainfall.LightRainLessthan5mmWithin6Hours =>
                CalculateRainAdjustedPvn(pvn5, incorporationHours, 0.5d),

            (int)Enums.Enumerations.Rainfall.HeavyRainGreaterThan5mmWithin6hours =>
                CalculateRainAdjustedPvn(pvn5, incorporationHours, 0.3d),

            _ => (pvn5, 0d)
        };
    }

    private (double pvn7, double temp1) CalculateRainAdjustedPvn(double pvn5,int incorporationHours,double rainfallFactor)
    {
        if (incorporationHours <= 6)
        {
            return (pvn5, 0d);
        }

        double pvn6 = pvn5 * rainfallFactor;
        double km = GetKmValue();

        double temp1 = pvn6 * (6d / (6d + km));

        return (pvn6 - temp1, temp1);
    }

    private (double pvn8, double temp2) ApplyIncorporationTimingAdjustment(double pvn7, int incorporationHours)
    {
        if (_manureApplication.IncorporationMethodID == (int)Enums.Enumerations.MethodOfIncorporationEnum.NotIncorporated)
        {
            return (pvn7, 0d);
        }

        double km = GetIncorporationKmValue();
        double temp2 = pvn7 * (incorporationHours / (incorporationHours + km));

        return (pvn7 - temp2, temp2);
    }

    private double ApplyIncorporationTechniqueAdjustment(double pvn8)
    {
        return _manureApplication.IncorporationMethodID switch
        {
            (int)Enums.Enumerations.MethodOfIncorporationEnum.TineCultivator =>
                pvn8 * GetIncorporationFactor(0.3d, 0.3d, 0.7d),

            (int)Enums.Enumerations.MethodOfIncorporationEnum.Discs =>
                pvn8 * GetIncorporationFactor(0.2d, 0.2d, 0.3d),

            (int)Enums.Enumerations.MethodOfIncorporationEnum.RotaryCultivator =>
                pvn8 * GetIncorporationFactor(0.15d, 0.1d, 0.2d),

            (int)Enums.Enumerations.MethodOfIncorporationEnum.MouldboardPlough =>
                pvn8 * GetIncorporationFactor(0.1d, 0.05d, 0.1d),

            _ => pvn8
        };
    }

    private double GetIncorporationFactor(double slurryFactor, double poultryFactor, double fymFactor)
    {
        if (IsSlurryType())
        {
            return slurryFactor;
        }

        if (IsPoultryOrSolidSludge())
        {
            return poultryFactor;
        }

        return fymFactor;
    }

    private double GetKmValue()
    {
        if (_manureType.ID ==
            (int)Enums.Enumerations.ManureTypes.DigestateWholeFoodBased)
        {
            return 4.5d;
        }

        return _manureType.ManureTypeCategoryID switch
        {
            (int)Enums.Enumerations.ManureCategory.CattleSlurry => 7.5d,
            (int)Enums.Enumerations.ManureCategory.LiquidSludge => 7.5d,
            (int)Enums.Enumerations.ManureCategory.PigSlurry => 11.6d,
            _ => 14.9d
        };
    }

    private double GetIncorporationKmValue()
    {
        return _manureType.ManureTypeCategoryID switch
        {
            (int)Enums.Enumerations.ManureCategory.FYM => 14.9d,

            (int)Enums.Enumerations.ManureCategory.Poultry => 40.4d,

            (int)Enums.Enumerations.ManureCategory.SolidSludge => 40.4d,

            (int)Enums.Enumerations.ManureCategory.CattleSlurry => 7.5d,

            (int)Enums.Enumerations.ManureCategory.LiquidSludge => 7.5d,

            (int)Enums.Enumerations.ManureCategory.PigSlurry => GetKmValue(),

            _ => 14.9d
        };
    }

    private bool IsCattleOrLiquidSludge()
    {
        // If the selected manure is cattle slurry or liquid digested sludge 
        return _manureType.ManureTypeCategoryID ==
                   (int)Enums.Enumerations.ManureCategory.CattleSlurry
               || _manureType.ManureTypeCategoryID ==
                   (int)Enums.Enumerations.ManureCategory.LiquidSludge;
    }

    private bool IsSlurryType()
    {
        return _manureType.ManureTypeCategoryID ==
                   (int)Enums.Enumerations.ManureCategory.CattleSlurry
               || _manureType.ManureTypeCategoryID ==
                   (int)Enums.Enumerations.ManureCategory.PigSlurry
               || _manureType.ManureTypeCategoryID ==
                   (int)Enums.Enumerations.ManureCategory.LiquidSludge;
    }

    private bool IsPoultryOrSolidSludge()
    {
        return _manureType.ManureTypeCategoryID ==
                   (int)Enums.Enumerations.ManureCategory.Poultry
               || _manureType.ManureTypeCategoryID ==
                   (int)Enums.Enumerations.ManureCategory.SolidSludge;
    }

    /// <summary>
    /// Calculates the uptake of manure N by a crop planted after a late summer/autumn manure application.  
    /// Refer to the Plant Uptake Module Technical Guide (September 2003).
    /// </summary>
    /// <param name="mineralN2">RAN after losses through NH3 volat. + NO3-N</param>
    /// <param name="month"></param>
    /// <param name="cropType"></param>
    /// <returns type="Double">Manure N following plant uptake</returns>
    /// <remarks></remarks>
    private double CalculateCropUptakeFactor(double mineralN2, int month)
    {
        try
        {
            // if date of manure application was in late summer/autumn (1st July - 31st October) then subtract the Autumn
            // Crop N Uptake (kg/ha) for the selected crop  'Now changed to 1st August - 31st October (November 2007)

            double CropUpdateFactor;

            if (mineralN2 < GetCropUptakeFactor(month))
            {
                CropUpdateFactor = mineralN2;
            }
            else
            {
                CropUpdateFactor = GetCropUptakeFactor(month);
            }

            return CropUpdateFactor;
        }
        catch (Exception)
        {
           // do not throw exception as this is not a critical calculation and we do not want to stop the rest of the calculations from being completed.  Return zero for crop uptake factor if there is an error.
            return 0d;
        }

    }

    private int GetCropUptakeFactor(int month)
    {
        int CropuptakeFactor;
        if (month >= 8 && month <= 10)
        {
            if (_runType == (int)Enumerations.RunAs.PlanetEngland || _runType == (int)Enumerations.RunAs.PlanetScotland)
            {
                CropuptakeFactor = _cropType.CropUptakeFactor;
            }
            else
            {
                CropuptakeFactor = _manureApplication.AutumnCropNitrogenUptake.Value;
            }
        }
        else
        {
            CropuptakeFactor = 0;
        }

        return CropuptakeFactor;
    }

    private double CalculateN2OEmission(double mineralN1)
    {
        // ********************************************************************************
        // ** Method:         CalcN2OEmission
        // ** Created:        Martina Gibbons 09/02/2004
        // ** Parameters:     MineralN1 as double
        // ** Return Value:   Double - N2O Emission
        // ** Description:    Calculates N2O loss
        // ********************************************************************************
        // N2O Emission factor: 1.96% of the Mineral N(1) pool following ammonium-N volatilisation

        // 20/02/2006
        // N2O Emission factor: updated emission factor now 1.96 rather than 1.74 
        // c.f. Email from Fiona Nicholson on 16/02/2006 and updated technical guide

        // 07 Nov 2012 C Lam - Return zero for paper crumbles
        if (IsPaperCrumble(_manureType.ID))
        {
            return 0d;
        }

        double dN2OEmission;
        double N2OEmissionFactor;
        // AC Three separate EFs: Slurry (0.85), FYM (0.73) & poultry manure (1.44)
        if (_manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.PigSlurry)
        {
            // Slurry
            N2OEmissionFactor = 0.85d;
        }
        else if (_manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.Poultry)
        {
            // Poultry
            N2OEmissionFactor = 1.44d;
        }
        else if (_manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.FYM)
        {
            // FYM
            N2OEmissionFactor = 0.73d;
        }
        else
        {
            N2OEmissionFactor = 1.96d;
        }

        dN2OEmission = mineralN1 / 100d * N2OEmissionFactor;

        return dN2OEmission;

    }

    private static bool IsPaperCrumble(int manureTypeId)
    {
        bool isPaperCrumble = manureTypeId == (int)Enums.Enumerations.ManureTypes.PaperCrumble;
        isPaperCrumble = isPaperCrumble || manureTypeId == (int)Enums.Enumerations.ManureTypes.PaperCrumbleBiologicallyTreated;
        isPaperCrumble = isPaperCrumble || manureTypeId == (int)Enums.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated;
        return isPaperCrumble;
    }

    private double CalculateN2Emission(double calculatedN2O)
    {
        // ********************************************************************************
        // ** Method:         CalcN2Emission
        // ** Created:        Martina Gibbons 09/02/2004
        // ** Parameters:     N2OEmission as double
        // ** Return Value:   Double - N2 emission
        // ** Description:    Calculates denitrified N
        // ********************************************************************************
        // N2:N2O ratio: 2.9

        // 07 Nov 2012 C Lam - Return zero for paper crumbles
        if (IsPaperCrumble(_manureType.ID))
        {
            return 0d;
        }

        double dN2Emission;
        dN2Emission = calculatedN2O * 2.9d;
        return dN2Emission;
    }

    /// <summary>
    /// Calculates the mineralised N for the next crop.
    /// Refer to Mineralisation Module Technical Guide (November 2007).
    /// </summary>
    /// <param name="calculatedTotalN">Total N calculated</param>
    /// <param name="calculatedPotentialN">Potential N calculated</param>
    /// <param name="totalManureN">Total N in the manure</param>
    /// <param name="mineralN1"></param>
    /// <param name="organicN3">Organic N remaining</param>
    /// <param name="mineralisedN2a">Is the ‘N available subsequent crops’ (ie. to a second and subsequent cut or graze)</param>
    /// <param name="cdd1">Cumulative Day Degrees for the months between Date of Application and 31st December in the first</param>
    /// <param name="cdd2">Cumulative Day Degrees for the months between 1st January (or Date of Application if the manure was applied between 1st January and 30th April) and 30th April</param>
    /// <param name="cdd2a">Cumulative Day Degrees for the months between Date of Application and 31st July</param>
    /// <returns type="Double">Mineralised N value</returns>
    /// <remarks>
    /// Called from calcManner. Completely revised following concerns that it worked with crop uptake.
    /// Calculates the mineralised N for the next crop.  
    /// Refer to Mineralisation Module Technical Guide (November 2007)
    /// </remarks>
    private sealed record MineralisedNCalculationResult(double MineralisedN, double MineralN1, double OrganicN3, double MineralisedN2A, double Cdd1, double Cdd2, double Cdd2A);

    private MineralisedNCalculationResult CalculateMineralisedN(double calculatedTotalN, double calculatedPotentialN)
    {
        try
        {
            if (IsPaperCrumble(_manureType.ID))
            {
                return new MineralisedNCalculationResult(0d, 0d, 0d, 0d, 0d, 0d, 0d);
            }

            var dNOrganic2 = default(double);
            var dNOrganic2A = default(double);
            double dNOrganic3;
            var dNMineralised1 = default(double);
            var dNMineralised2 = default(double);
            var dNMineralised2A = default(double);
            var dCDD1 = default(double);
            var dCDD2 = default(double);
            var dCDD2A = default(double);

            string cropUse = _cropType.Use;

            // This array is a temporary one to allow the mineralisation and leaching to be tested before applying to specific parts of the country
            int[] tempArray = CreateTempArray();

            // ------------------------------------------------------------------------------------
            // N AVAILABLE TO THE NEXT CROP GROWN
            // ------------------------------------------------------------------------------------
            // Step 1 - Calculate initial organic N component (NOrg1) of the manure
            // ------------------------------------------------------------------------------------
            double organicN1 = calculatedTotalN - calculatedPotentialN;

            // --------------------------------------------------------------------------------------
            // Step 2 - Check the date of application
            // --------------------------------------------------------------------------------------
            int month = _manureApplication.ApplicationDate.Month;

            switch (cropUse ?? "")
            {
                case _grassKey:
                    {
                        if (month >= 8 && month < 13)
                        {
                            // Calculate the cumulative day degrees (CDD) for the months between the Date of Application and Date of End of Drainage in the first year.
                            dCDD1 = CalculateCddForMineralisedN(tempArray, month, 13, false);

                            // ------------------------------------------------------------------------------------
                            // Step 3 - Calculate the N available to the next crop grown.  This will always
                            // be less than 2300.  This is only calculated if the application is an autumn
                            // application
                            // ------------------------------------------------------------------------------------
                            // Check that CDD1 is less than 2300
                            if (dCDD1 >= 2300d)
                            {
                                dCDD1 = 2299d;
                            }
                            else
                            {
                                dNMineralised1 = CalculateMineralisedNForPeriod(dCDD1, organicN1);
                            }

                            // calculate the amount of organic N remaining
                            dNOrganic2 = organicN1 - dNMineralised1;

                            // Step 6 - Calculate NMin2
                            // ------------------------------------------------------------------------------------
                            month = 1;
                            dCDD2 = CalculateCddForMineralisedN(tempArray, month, 5, false);
                            dNMineralised2 = CalculateMineralisedNForPeriod(dCDD2, dNOrganic2);

                            // Now calculate the mineralisation for a subsequent cut/graze between
                            // 30th April and 31st July
                            dNOrganic2A = dNOrganic2 - dNMineralised2;

                            // Step 7 - Calculate NMin2A
                            // ------------------------------------------------------------------------------------
                            dCDD2A = 786d;
                            dNMineralised2A = CalculateMineralisedNForPeriod(dCDD2A, dNOrganic2A);
                        }
                        else if (_manureApplication.ApplicationDate.Month == 1 || _manureApplication.ApplicationDate.Month == 2 || _manureApplication.ApplicationDate.Month == 3 || _manureApplication.ApplicationDate.Month == 4)
                        {
                            // calculate the amount of organic N remaining
                            dNMineralised1 = 0d;
                            dNOrganic2 = organicN1 - dNMineralised1;

                            // Step 6 - Calculate NMin2
                            // ------------------------------------------------------------------------------------
                            dCDD2 = CalculateCddForMineralisedN(tempArray, month, 5, false);
                            dNMineralised2 = CalculateMineralisedNForPeriod(dCDD2, dNOrganic2);

                            // Now calculate the mineralisation for a subsequent cut/graze between
                            // 30th April and 31st July
                            dNOrganic2A = dNOrganic2 - dNMineralised2;

                            // Step 7 - Calculate NMin2A
                            // ------------------------------------------------------------------------------------
                            dCDD2A = 786d;
                            dNMineralised2A = CalculateMineralisedNForPeriod(dCDD2A, dNOrganic2A);
                        }

                        else if (_manureApplication.ApplicationDate.Month == 5 || _manureApplication.ApplicationDate.Month == 6 || _manureApplication.ApplicationDate.Month == 7)
                        {
                            dNMineralised1 = 0d;
                            dNMineralised2 = 0d;

                            dNOrganic2A = organicN1;

                            // Step 6 - Calculate NMin2
                            // ------------------------------------------------------------------------------------
                            dCDD2A = CalculateCddForMineralisedN(tempArray, month, 8, true);
                            dNMineralised2A = CalculateMineralisedNForPeriod(dCDD2A, dNOrganic2A);
                            // *******************
                            // ARABLE
                            // *******************
                        }
                        break;
                    }
                default:
                    {
                        // If date of application is between 1/08 and 31/12 then
                        if (month >= 8 && month < 13)
                        {
                            // Calculate the cumulative day degrees (CDD) for the months between the Date of Application and Date of End of Drainage in the first year.
                            dCDD1 = CalculateCddForMineralisedN(tempArray, month, 13, false);

                            // ------------------------------------------------------------------------------------
                            // Step 3 - Calculate the N available to the next crop grown.  This will always
                            // be less than 2300.  This is only calculated if the application is an autumn
                            // application
                            // ------------------------------------------------------------------------------------
                            // Check that CDD1 is less than 2300
                            if (dCDD1 >= 2300d)
                            {
                                dCDD1 = 2299d;
                            }
                            else
                            {
                                dNMineralised1 = CalculateMineralisedNForPeriod(dCDD1, organicN1);
                            }

                            // calculate the amount of organic N remaining
                            dNOrganic2 = organicN1 - dNMineralised1;

                            // Step 6 - Calculate NMin2
                            // ------------------------------------------------------------------------------------
                            month = 1;
                            dCDD2 = CalculateCddForMineralisedN(tempArray, month, 8, true);
                            dNMineralised2 = CalculateMineralisedNForPeriod(dCDD2, dNOrganic2);
                            dNMineralised2 = AdjustMineralisedN2ForArableCrop(ref dNMineralised2, 0.6d);
                        }
                        else if (_manureApplication.ApplicationDate.Month >= 1 || _manureApplication.ApplicationDate.Month <= 8)
                        {
                            // calculate the amount of organic N remaining
                            // no manure application was made between August and December
                            dNMineralised1 = 0d;
                            dNOrganic2 = organicN1 - dNMineralised1;

                            // Step 6 - Calculate NMin2
                            // ------------------------------------------------------------------------------------
                            dCDD2 = CalculateCddForMineralisedN(tempArray, month, 8, true);
                            dNMineralised2 = CalculateMineralisedNForPeriod(dCDD2, dNOrganic2);
                            dNMineralised2 = AdjustMineralisedN2ForArableCrop(ref dNMineralised2, 0.6d);
                        }
                        break;
                    }
            }
            // ------------------------------------------------------------------------------------
            // Step 4 - Calculate the amount of organic N remaining
            // ------------------------------------------------------------------------------------
            switch (cropUse ?? "")
            {
                case _grassKey:
                    {
                        dNOrganic3 = dNOrganic2A - dNMineralised2A;
                        break;
                    }
                case _arableKey:
                    {
                        dNOrganic3 = dNOrganic2 - dNMineralised2;
                        break;
                    }
                default:
                    {
                        dNOrganic3 = dNOrganic2 - dNMineralised2;
                        break;
                    }
            }

            return new MineralisedNCalculationResult(dNMineralised2, dNMineralised1, dNOrganic3, dNMineralised2A, dCDD1, dCDD2, dCDD2A);
        }

        catch (Exception)
        {
            return new MineralisedNCalculationResult(0d, 0d, 0d, 0d, 0d, 0d, 0d);
        }
    }

    private static int[] CreateTempArray()
    {
        var tempArray = new int[25];

        tempArray[1] = 0;
        tempArray[2] = 0;
        tempArray[3] = 18;
        tempArray[4] = 82;
        tempArray[5] = 183;
        tempArray[6] = 267;
        tempArray[7] = 336;
        tempArray[8] = 331;
        tempArray[9] = 259;
        tempArray[10] = 174;
        tempArray[11] = 47;
        tempArray[12] = 0;
        tempArray[13] = 0;
        tempArray[14] = 0;
        tempArray[15] = 18;
        tempArray[16] = 81;
        tempArray[17] = 184;
        tempArray[18] = 267;
        tempArray[19] = 225;
        tempArray[20] = 332;
        tempArray[21] = 258;
        tempArray[22] = 175;
        tempArray[23] = 46;
        tempArray[24] = 0;
        return tempArray;
    }

    private double CalculateCddForMineralisedN(int[] tempArray, int month, int maxMonth, bool limitTo2300)
    {
        // cumulative day degrees (CDD)
        var cdd = default(double);
        // If the date of application is before 15th of the month include that month in the calculation of CDD reset the variables

        if (_manureApplication.ApplicationDate.Day > 15)
        {
            month++;
        }

        while (month < maxMonth)
        {
            cdd += tempArray[month];
            month++;
        }

        // Check the cumulative day degrees don't go above 2300
        if (cdd >= 2300d && limitTo2300)
        {
            cdd = 2299d;
        }

        return cdd;
    }

    private double CalculateMineralisedNForPeriod(double cumulativeDayDegrees, double organicN, double percentagedMineralisedNFymCattleSlurry = 0.008339d, double percentagedMineralisedPultrySlurrySludgeAndDefault = 0.02306d)
    {
        double mineralisedN;

        if ((_manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.FYM || _manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.CattleSlurry) && !IsBiosolidLiquidDigested(_manureType.ID))
        {
            mineralisedN = percentagedMineralisedNFymCattleSlurry * cumulativeDayDegrees / 100d * organicN;
        }
        else if (_manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.None || IsPaperCrumble(_manureType.ID))
        {
            mineralisedN = 0d;
        }
        else if (_manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.Poultry || _manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.PigSlurry || _manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.SolidSludge || _manureType.ManureTypeCategoryID == (int)Enums.Enumerations.ManureCategory.LiquidSludge)
        {
            mineralisedN = percentagedMineralisedPultrySlurrySludgeAndDefault * cumulativeDayDegrees / 100d * organicN;
        }
        else
        {
            mineralisedN = percentagedMineralisedPultrySlurrySludgeAndDefault * cumulativeDayDegrees / 100d * organicN;
        }

        return mineralisedN;
    }

    // added C Lam Sep 2012
    private static bool IsBiosolidLiquidDigested(int mannerTypeID)
    {
        return mannerTypeID == (int)Enums.Enumerations.ManureTypes.BiosolidsLiquidDigested;
    }

    private double AdjustMineralisedN2ForArableCrop(ref double mineralisedN2, double adjustmentFactor)
    {
        // Now adjust the value of NMin2 depending on the crop type
        // For cereals or oilseed rape multiply NMin2 by 0.6
        switch (_cropType.ID)
        {
            case (int)Enums.Enumerations.CropTypeEnum.EarlySownWinterCereal:
            case (int)Enums.Enumerations.CropTypeEnum.LateSownWinterCereal:
            case (int)Enums.Enumerations.CropTypeEnum.EarlyEstablishedWinterOilseedRape:
            case (int)Enums.Enumerations.CropTypeEnum.LateEstablishedWinterOilseedRape:
            case (int)Enums.Enumerations.CropTypeEnum.SpringCerealOilseedRape:
            case (int)Enums.Enumerations.CropTypeEnum.Potatoes:
            case (int)Enums.Enumerations.CropTypeEnum.Sugarbeet:
            case (int)Enums.Enumerations.CropTypeEnum.Other:
                {

                    return mineralisedN2 * adjustmentFactor;
                }

            default:
                {
                    return mineralisedN2;
                }
        }
    }

    /// <summary>
    /// Calculates the leached N.
    /// </summary>
    /// <param name="mineralN4"></param>
    /// <param name="vmTotal"></param>
    /// <param name="vmTop"></param>
    /// <returns type="Double">Leached N</returns>
    /// <remarks>Removed from the calcManner routine to allow more flexibility</remarks>
    private double CalculateLeachedN(double mineralN4, double vmTotal, double vmTop)
    {

        // 07 Nov 2012 C Lam - Return zero for paper crumbles
        if (IsPaperCrumble(_manureType.ID))
        {
            return 0d;
        }

        DateOnly datCurApp;
        DateOnly datEndDrain;
        double dMinN4;
        int lNitrificationDelay;


        int monthApp;

        double dDefAdjustFactor;
        double dVMEffective;
        var dHER = default(double);
        double dHEREffective;

        double dSMD;      // soil moisture deficit
        double dSMDCurMonth;      // soil moisture deficit this month
        double dSMDPrevMonth;     // soil moisture deficit previous month

        // Mostly matrix algorithm variables
        double dLRatio;
        double dLFactor1;
        double dLIndex;
        double dLProp;

        int incorporationDelayHours;

        try
        {

            incorporationDelayHours = _incorporationDelay.Hours ?? 0;

            // Reset the Leached N variables to zero
            double calculateLeachedNRet = 0d;
            dHEREffective = 0d;

            dMinN4 = mineralN4;

            // STEP 1 - Calculate Nitrification
            // The nitrification delay is dealt with in the Nitrification Technical Guide.
            // ----------------------------------------------------------------------------------
            // Date of application
            datCurApp = _manureApplication.ApplicationDate;

            // Calculate the nitrification delay
            lNitrificationDelay = CalculateNitrificationDelay(datCurApp);

            // For simplicity the nitrification delay is added to the date of application
            // rather than treated as a range.
            // Reset the current application date to allow for this delay.
            datCurApp = datCurApp.AddDays(lNitrificationDelay);

            // set the application month variable to the date of the current manure application
            monthApp = datCurApp.Month;


            // set the variable for the date of end of drainage
            datEndDrain = _manureApplication.EndOfDrainageDate;

            // STEP 3 - Determine soil properties
            // ----------------------------------------------------------------------------------
            // Leaching calculation starts here   'check the date of application is less than the end of soil drainage
            if (datCurApp < datEndDrain)
            {

                // adjustment for moisture deficit and for incorporation, which differs from that for permeable soil because of the effects on bypass flow and surface runoff.

                // Get soil moisture deficit from MCDM.  Note: because MCDM calculates soil moisture deficit at the end of the month we use the SMD from
                // the previous month to the month of application.  Agreed following conversation with E. Lord on 26/7/2006.
                // As the count starts at zero then iMonthApp -.

                dSMDCurMonth = GetClimateType(monthApp, _climateCalculator, Enumerations.ClimateDataType.SoilMoistureDefecit);

                if (monthApp == 1)
                {
                    // if month is january take december as previous month
                    dSMDPrevMonth = GetClimateType(12, _climateCalculator, Enumerations.ClimateDataType.SoilMoistureDefecit);
                }
                else
                {
                    dSMDPrevMonth = GetClimateType(monthApp - 1, _climateCalculator, Enumerations.ClimateDataType.SoilMoistureDefecit);
                }

                // Even out the SMD factor depending on how far along the month we are.
                double SMDpropstart;
                SMDpropstart = (double)datCurApp.Day / (double)DateTime.DaysInMonth((int)_manureApplication.ApplicationDate.Year, (int)_manureApplication.ApplicationDate.Month);

                dSMD = dSMDPrevMonth + SMDpropstart * (dSMDCurMonth - dSMDPrevMonth);

                // Calculate the effective HER from effective application date to end of drainage.

                // If there was a soil moisture deficit then this affects adjustment factor

                if (dSMD > 0d)
                {
                    dHER = _manureApplication.RainfallPostApplication - _evapotranspirationTotal;
                    dHEREffective = dHER;
                }

                // STEP 4 - Apply appropriate leaching algorithm
                // ----------------------------------------------------------------------------------
                // The subsoil contains the word CLAY
                // use the drained clay soil leaching algorithm
                // ----------------------------------------------------------------------------------
                // if the result of the string search for the word clay is greater than 0 then we have a clay soil

                if (_subSoil.ID == (int)Enums.Enumerations.SoilType.Clay || _subSoil.ID == (int)Enums.Enumerations.SoilType.ClayLoam || _subSoil.ID == (int)Enums.Enumerations.SoilType.SandyClay || _subSoil.ID == (int)Enums.Enumerations.SoilType.SandyClayLoam || _subSoil.ID == (int)Enums.Enumerations.SoilType.SiltyClay || _subSoil.ID == (int)Enums.Enumerations.SoilType.SiltyClayLoam)
                {

                    double dLProp1;
                    double dLProp2;
                    double dLProp3;
                    double dLRatioMod;
                    var dLAdjust = default(double);
                    double dInc;      // adjustment for method of incorporation

                    // Check if manure was incorporated (any value other than 'not incorporated'.
                    // IncorpFlag = ……
                    switch (_manureApplication.IncorporationMethodID)
                    {
                        // If the manure has been ploughed down
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                            {
                                dInc = 0.9d;
                                break;
                            }
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                            {
                                dInc = 0.4d;
                                break;
                            }
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                            {
                                dInc = 0.4d;
                                break;
                            }
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.NotIncorporated: // "Not Incorporated"
                            {
                                dInc = 0d; // catch all, but shouldn't get here
                                break;
                            }
                        default:
                            {
                                dInc = 0d;
                                break;
                            }
                    }

                    // if delay before incorporation > 7 days then                    
                    if (incorporationDelayHours > 168)
                    {
                        dInc *= 0.5d;
                    }

                    // Calculate 'leaching ratio'
                    // check for divide by zero error
                    if (vmTotal <= 0d)
                    {
                        dLRatio = 0d;
                    }
                    else
                    {
                        dLRatio = dHEREffective / vmTotal;
                    }

                    // dLProp1 accounts for initial N loss due to surface and crack flow.
                    dLProp1 = Math.Min(0.13d, Math.Max(0d, dLRatio));

                    dLProp1 = dLProp1 * (1d - dInc) * (1d - Math.Min(dSMD / 50d, 1d));

                    // NB perhaps this can be omitted if the manure is incorporated, and this would simplify the incorporation adjustment
                    dLProp2 = 0.51d * dLRatio;

                    // Lprop3 covers non-linearity under very wet climatic conditions
                    // and is calculated for the range Lratio > 0.75
                    if (dLRatio > 0.75d)
                    {
                        dLProp3 = -0.335d * (dLRatio - 0.75d);
                    }
                    else
                    {
                        dLProp3 = 0d;
                    }

                    // Total proportion leached is the sum of the three.
                    // Lprop is constrained to be between 0 and 1
                    dLProp = Math.Min(1d, Math.Max(0d, dLProp1 + dLProp2 + dLProp3));


                    // Modifications to the result of Lprop to allow for deficit at the effective date of application, and incorporation, are as follows:
                    if (dSMD > 0d || dInc > 0d)
                    {
                        // If Lratio > 0.75 then Lratiomod = 0.75 else Lratiomod = Lratio
                        if (dLRatio > 0.75d)
                        {
                            dLRatioMod = 0.75d;
                        }
                        else
                        {
                            dLRatioMod = dLRatio;
                        }

                        // dLAdjust = dLRatioMod * dDefAdjustFactor
                        dDefAdjustFactor = 0.002d * dSMD;
                        dLAdjust = dLRatioMod * dDefAdjustFactor;
                    }

                    // These equations cause leaching to be reduced by up to 25% of total nitrate at risk (or up to about 40% of the nitrate actually lost from
                    // surface applications) when manure is applied to a soil with deficit, or is incorporated promptly.
                    // Then:
                    dLProp -= dLAdjust;
                    calculateLeachedNRet = dMinN4 * dLProp;
                }

                // ----------------------------------------------------------------------------------
                // Else the subsoil does not contain the word CLAY
                // use the MATRIX LEACHING ALGORITHM
                // ----------------------------------------------------------------------------------
                else
                {
                    // Calculate the EFFECTIVE water capacity through which nitrate has to move:
                    switch (_manureApplication.IncorporationMethodID)
                    {
                        // If the manure has been cultivated or ploughed down within a month ie at all
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                            {
                                dVMEffective = vmTotal - 0.5d * vmTop;
                                break;
                            }
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                            {
                                dVMEffective = vmTotal - 0.25d * vmTop;
                                break;
                            }
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                            {
                                dVMEffective = vmTotal - 0.25d * vmTop;
                                break;
                            }
                        // If it hasn't been incorporated then nothing changes
                        case (int)Enums.Enumerations.MethodOfIncorporationEnum.NotIncorporated: // "Not Incorporated"
                            {
                                dVMEffective = vmTotal; // catch all, but shouldn't get here
                                break;
                            }
                        default:
                            {
                                dVMEffective = vmTotal;
                                break;
                            }
                    }

                    dHEREffective = dHER + dSMD * 0.7d;

                    if (dHEREffective < 0d)
                    {
                        dHEREffective = 0d;
                    }

                    // Calculate leaching ratio
                    if (dVMEffective <= 0d)
                    {
                        dLRatio = 0d;
                    }
                    else
                    {
                        dLRatio = Math.Min(1.896d, dHEREffective / dVMEffective);
                    }

                    // Calculate Lfactor1
                    if (dLRatio <= 1d)
                    {
                        // Lfactor1 = (1-power(L, 0.5)
                        dLFactor1 = 1d - Math.Pow(dLRatio, 0.5d);
                    }
                    else
                    {
                        // Lfactor1 = (power(L, 0.5) -1)
                        dLFactor1 = Math.Pow(dLRatio, 0.5d) - 1d;
                    }

                    // Calculate leaching index
                    dLIndex = 2.27d * Math.Pow(dLFactor1, 3d) - 4.5d * Math.Pow(dLFactor1, 2d) + 2.7d * dLFactor1;

                    // Calculate proportion leached
                    if (dLRatio < 0.25d)
                    {
                        dLProp = 0d;
                    }
                    else if (dLRatio < 1d)
                    {
                        dLProp = 0.5d - dLIndex;
                    }
                    else
                    {
                        dLProp = 0.5d + dLIndex;
                    }

                    if (dLProp > 1d)
                        dLProp = 1d;
                    if (dLProp < 0d)
                        dLProp = 0d;

                    // Return Leached N value
                    // N leached (kg/ha) = (nitrate-N added for the period) * Lprop
                    calculateLeachedNRet = (double)dMinN4 * dLProp;
                }
            }
            else
            {
                // This is a bit of a catch all situation and allows the function to return a
                // value.  I'm also making the assumption that if the application date is
                // after the end of soil drainage no leaching can occur.
                calculateLeachedNRet = 0d;
            }
            return calculateLeachedNRet;
        }
        catch (Exception)
        {
            return 0d;
        }

    }


    /// <summary>
    /// This function returns a climate attribute for a particular month. You can get one of the following values (Soil Moisture Deficit,  rainfall, Potential Evapotranspiration, Actual Evapotranspiration)
    /// </summary>
    /// <param name="month"></param>
    /// <param name="climate"></param>
    /// <param name="climateType"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static double GetClimateType(int month, ClimateCalculator climate, Enumerations.ClimateDataType climateType)
    {
        var climateMonths = new DTOs.ClimateMonths();
        var retVal = default(double);

        switch (climateType)
        {
            case Enumerations.ClimateDataType.SoilMoistureDefecit:
                {
                    climateMonths = climate.SoilMoistureDeficit;
                    break;
                }
            case Enumerations.ClimateDataType.Rainfall:
                {
                    climateMonths = climate.Rain;
                    break;
                }
            case Enumerations.ClimateDataType.PotentialEvapotranspiration:
                {
                    climateMonths = climate.PotentialEvapotranspiration;
                    break;
                }
            case Enumerations.ClimateDataType.ActualEvapotranspiration:
                {
                    climateMonths = climate.ActualEvapotranspiration;
                    break;
                }
        }

        switch (month)
        {
            case 1:
                {
                    retVal = climateMonths.January;
                    break;
                }
            case 2:
                {
                    retVal = climateMonths.February;
                    break;
                }
            case 3:
                {
                    retVal = climateMonths.March;
                    break;
                }
            case 4:
                {
                    retVal = climateMonths.April;
                    break;
                }
            case 5:
                {
                    retVal = climateMonths.May;
                    break;
                }
            case 6:
                {
                    retVal = climateMonths.June;
                    break;
                }
            case 7:
                {
                    retVal = climateMonths.July;
                    break;
                }
            case 8:
                {
                    retVal = climateMonths.August;
                    break;
                }
            case 9:
                {
                    retVal = climateMonths.September;
                    break;
                }
            case 10:
                {
                    retVal = climateMonths.October;
                    break;
                }
            case 11:
                {
                    retVal = climateMonths.November;
                    break;
                }
            case 12:
                {
                    retVal = climateMonths.December;
                    break;
                }
        }

        return retVal;
    }

    /// <summary>
    /// Calculates the nitrification delay in days depending on the month of application of the manure. 
    /// Information for the calculation of the nitrification delay is contained in the Nitrification Delay Technical Guide of June 2004 
    /// </summary>
    /// <param name="dateOfApplication"></param>
    /// <returns type="Integer"></returns>
    /// <remarks></remarks>
    private static int CalculateNitrificationDelay(DateOnly dateOfApplication)
    {
        // ********************************************************************************
        DateOnly datDateofApplication;
        var lNoofDays = default(int);
        int monthNumber;

        datDateofApplication = dateOfApplication;

        // get the month of application from the date of application
        monthNumber = datDateofApplication.Month;
        // based on the month of application find the number of days for the nitrification delay
        switch (monthNumber)
        {
            case 1:
                {
                    lNoofDays = 21;
                    break;
                }
            case 2:
                {
                    lNoofDays = 18;
                    break;
                }
            case 3:
                {
                    lNoofDays = 14;
                    break;
                }
            case 4:
                {
                    lNoofDays = 11;
                    break;
                }
            case 5:
                {
                    lNoofDays = 7;
                    break;
                }
            case 6:
                {
                    lNoofDays = 6;
                    break;
                }
            case 7:
                {
                    lNoofDays = 5;
                    break;
                }
            case 8:
                {
                    lNoofDays = 5;
                    break;
                }
            case 9:
                {
                    lNoofDays = 5;
                    break;
                }
            case 10:
                {
                    lNoofDays = 6;
                    break;
                }
            case 11:
                {
                    lNoofDays = 12;
                    break;
                }
            case 12:
                {
                    lNoofDays = 15;
                    break;
                }
        }


        // Return the number of days for the nitrification delay
        return lNoofDays;

    }

    private double ApplyMineralisationFactor()
    {
        // EG Modification required to multiply mineralisation by 2 for poultry only.
        // some biosolids as set as manure category as poultry but these don't need the factor applied.
        if (_manureType.ID == (int)Enums.Enumerations.ManureTypes.BroilerTurkeyLitter || _manureType.ID == (int)Enums.Enumerations.ManureTypes.PoultryManure)
        {
            return 2d;
        }
        else
        {
            return 1d;
        }

    }
}
