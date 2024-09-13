using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Application.MannerLib;
using Manner.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
namespace Manner.Application.Calculators;
//[Service(ServiceLifetime.Transient)]
public class NutrientsCalculator(FieldDetail field, ClimateDto climate, CropTypeDto cropType, MannerApplication mannerApplication, ManureTypeDto manureType, IncorporationDelayDto incorporationDelay, TopSoilDto topSoil, SubSoilDto subSoil, List<ClimateTypeDto> climateTypes) : INutrientsCalculator
{
    private readonly FieldDetail _field = field;
    private readonly MannerApplication _mannerApplication = mannerApplication;
    private readonly ManureTypeDto _manureType = manureType;
    private readonly ClimateDto _climate = climate;
    private readonly CropTypeDto _cropType = cropType;
    private readonly IncorporationDelayDto _incorporationDelay = incorporationDelay;
    private readonly TopSoilDto _topSoil = topSoil;
    private readonly SubSoilDto _subSoil = subSoil;
    private readonly List<ClimateTypeDto> _climateTypes = climateTypes;
    private readonly DTOs.Outputs _outputs = new();
    private readonly ClimateCalculator _climateCalculator = new();

    public DTOs.Outputs CalculateNutrients()
    {
        var mineralN1 = default(double);
        double mineralN2;
        double mineralN3;
        double mineralN4; // manure N remaining after losses through NH3 volatilisation + any nitrate N in the original manure application
        int incorporationCumulativeHours = _incorporationDelay.CumulativeHours ?? 0;

        string cropUse = _cropType?.Use ?? string.Empty;
        if (_manureType != null)
        {
            this.CalculateNutrientsOutputsValues();

            // Available N
            // --------------------------------------------------------------
            double calculatedTotalN = (double)(_mannerApplication.ApplicationRate.Value * _manureType.TotalN);
            // Readily Available N applied (NH4-N and uric acid N)
            // --------------------------------------------------------------
            // 18 Jan 2013 - Lizzie says "CalcPot = AppRate * (TotalAmmN + TotalUricN + TotalNitrateN)"
            double calculatedPotentialN = (double)(_mannerApplication.ApplicationRate.Value * (_manureType.NH4N + _manureType.Uric + _manureType.NO3N));

            double potentialNAvailable = (double)(_mannerApplication.ApplicationRate.Value * (_manureType.NH4N + _manureType.Uric));

            // Volatilised N
            // --------------------------------------------------------------
            double calculatedVolatilisedN = this.CalculateAmmoniaVolatilisation(calculatedTotalN, potentialNAvailable, cropUse, incorporationCumulativeHours);

            // N2O Emission
            // --------------------------------------------------------------
            // N2O Emission is 1.74% of applied readily available N remaining following volatilisation
            double n2oEmission = calculatedPotentialN - calculatedVolatilisedN;
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
            double calculatedcropUptakeFactor = 0d;
            if (_cropType != null)
            {
                if (mineralN2 < _cropType.CropUptakeFactor)
                {
                    calculatedcropUptakeFactor = mineralN2;
                }
                else
                {
                    calculatedcropUptakeFactor = _cropType.CropUptakeFactor;
                }
            }

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
            var mineralisedN2A = default(double);
            var mineralisedN3 = default(double);
            var cdd1 = default(double);
            var cdd2 = default(double);
            var cdd2a = default(double);
            double calculatedMineralisedN = this.CalculateMineralisedN(calculatedTotalN, calculatedPotentialN, ref mineralN1, ref mineralisedN3, ref mineralisedN2A, ref cdd1, ref cdd2, ref cdd2a);

            mineralN4 = mineralN3 + mineralN1;
            // Leached N
            // -------------------------------------------------------------
            // Calculate soil volumetric water content
            double vmWaterTopSoil = _topSoil.VolumetricMeasure; // Convert.ToDouble(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "TopSoilVM", (int)MannerApplication.FieldData.Topsoil));
            double vmWaterTotal = vmWaterTopSoil + vmWaterTopSoil; //Convert.ToDouble(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "SubSoilVM", (int)MannerApplication.FieldData.Subsoil));

            double calculatedLeachedN = CalculateLeachedN(mineralN4, vmWaterTotal, vmWaterTopSoil);
            double nMineralised4 = mineralN4 - calculatedLeachedN;

            // Modification required to multiply mineralisation by 2 for poultry only.
            calculatedMineralisedN *= ApplyMineralisationFactor();
            mineralisedN2A *= ApplyMineralisationFactor();

            // Calculate final results and assign to public variables
            CalculateFinalResults(calculatedTotalN, calculatedPotentialN, calculatedVolatilisedN, calculatedN2O, calculatedN2, calculatedcropUptakeFactor, calculatedMineralisedN, calculatedLeachedN);


            if (this.IsPaperCrumble(_manureType.ID))
            {
                CalculateNAvailableResultsPaperCrumble();
            }
            else
            {
                switch (cropUse ?? "")
                {
                    case "Grass":
                        {
                            CalculateNAvailableResultsGrass(mineralN3, mineralisedN2A, calculatedcropUptakeFactor, calculatedMineralisedN, calculatedLeachedN);
                            break;
                        }
                    case "Arable":
                        {
                            _outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN + calculatedcropUptakeFactor) * 10.0d) / 10d;
                            _outputs.ResultantNAvailableSecondCut = 0d;
                            break;
                        }

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
            throw new Exception("Manure not found");
        }

        return _outputs;
    }

    // Aug 2012  C Lam: added IsCalcRainfall parameter, to allow a user supplied value to be used
    public void CalculateClimate(bool haveSuppliedOwnClimateData = false, bool isCalcRainfall = true)
    {
        try
        {
            if (_climate != null)
            {
                _climateCalculator.GetClimate(_climate, _field.CropTypeID, _field.TopsoilID, _field.SubsoilID, _topSoil.AvailableWaterCapacity, _subSoil.AvailableWaterCapacity, haveSuppliedOwnClimateData);
            }
            //else
            //{
            //    _climateCalculator.GetClimate(_climate, _mannerApplication.Easting, _mannerApplication.Northing, _field.CropTypeID, _field.TopsoilID, _field.SubsoilId, _topSoil.AvailableWaterCapacity, _subSoil.AvailableWaterCapacity, haveSuppliedOwnClimateData);
            //}

            if (isCalcRainfall)
            {
                if ((int)_mannerApplication.ApplicationDate.Day > 0 & (int)_mannerApplication.EndOfDrainageDate.Day > 0)
                {
                    this.CalculateRainfall(_mannerApplication.ApplicationDate, _mannerApplication.EndOfDrainageDate);
                }
            }
        }

        catch (Exception)
        {
            //TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw;
        }

    }

    private void CalculateRainfall(DateOnly applicationDate, DateOnly endSoilDrainageDate)
    {
        // CalcRainfall called to update Total Rainfall and Total Evap updates MannerTotalRain and MannerTotalEvap public variables
        // Called when Application Date or End of Soil Drainage dates are changed Receives ApplicationDate and End of Soil Drainage Dates
        // Works off monthly rainfall and AE values in climate array.

        try
        {

            DateTime appDate;
            DateTime endDate;
            var sumRain = default(double);
            var sumEvap = default(double);
            double propstart;
            double propend;
            double applicationDateAE, appDateRain;
            double soilDrainageAE, soilDrainageRain;
            //var objManData = new MannerLib.MannerData();

            appDateRain = this.GetClimateType(applicationDate.Month, _climateCalculator, Enumerations.ClimateDataType.Rainfall);
            applicationDateAE = this.GetClimateType(applicationDate.Month, _climateCalculator, Enumerations.ClimateDataType.ActualEvapotranspiration);

            soilDrainageRain = this.GetClimateType(endSoilDrainageDate.Month, _climateCalculator, Enumerations.ClimateDataType.Rainfall);
            soilDrainageAE = this.GetClimateType(endSoilDrainageDate.Month, _climateCalculator, Enumerations.ClimateDataType.ActualEvapotranspiration);


            // DO NOT ADD ONE MONTH TO THE DATE OF APPLICATION TO MIMIC EXISTING CODE AND MANNER PAPER
            appDate = new DateTime(applicationDate.Year, applicationDate.Month, applicationDate.Day);
            endDate = new DateTime(endSoilDrainageDate.Year, endSoilDrainageDate.Month, endSoilDrainageDate.Day);
            // #### NOTE -    Any manure application AFTER 31/07/98 is associated with the next years End of Soil Drainage

            if ((endDate - appDate).Days <= 0)
            {
                // if date of Application is after End of Soil Drainage then return zero rainfall and zero evap.
                _mannerApplication.RainfallTotal = 0d;
                _mannerApplication.EvapotranspirationTotal = 0d;
            }
            else
            {
                // else calculate rainfall
                propstart = Thread.CurrentThread.CurrentCulture.Calendar.GetDayOfMonth(appDate) /
                           (double)(new DateTime(Thread.CurrentThread.CurrentCulture.Calendar.GetYear(appDate),Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(appDate),1).AddMonths(1) -
                                   new DateTime(
                                       Thread.CurrentThread.CurrentCulture.Calendar.GetYear(appDate),
                                       Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(appDate),
                                       1)).TotalDays;

                // Calculate propend
                propend = Thread.CurrentThread.CurrentCulture.Calendar.GetDayOfMonth(endDate) /
                                (double)(new DateTime(
                                            Thread.CurrentThread.CurrentCulture.Calendar.GetYear(endDate),
                                            Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(endDate),
                                            1)
                                        .AddMonths(1) -
                                        new DateTime(
                                            Thread.CurrentThread.CurrentCulture.Calendar.GetYear(endDate),
                                            Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(endDate),
                                            1)).TotalDays;
                // check to make sure that month for app date and end soil drainage is not the same. If it is only to diff at end of period
                if (((endDate.Year - appDate.Year) * 12) + endDate.Month - appDate.Month > 0L)
                {
                    if (applicationDateAE > appDateRain)
                    {
                        sumRain = appDateRain * (1.0d - propstart);
                        sumEvap = sumRain;
                    }
                    else
                    {
                        sumRain = appDateRain * (1.0d - propstart);
                        sumEvap = applicationDateAE * (1.0d - propstart);
                    }

                    if (soilDrainageAE > soilDrainageRain)
                    {
                        sumRain += soilDrainageRain * propend;
                        sumEvap += soilDrainageRain * propend;
                    }
                    else
                    {
                        sumRain += soilDrainageRain * propend;
                        sumEvap += soilDrainageAE * propend;
                    }
                }

                else if (((endDate.Year - appDate.Year) * 12) + endDate.Month - appDate.Month == 0L)
                {

                    if (soilDrainageAE > soilDrainageRain)
                    {
                        sumRain += soilDrainageRain * (propend - propstart);
                        sumEvap += soilDrainageRain * (propend - propstart);
                    }
                    else
                    {
                        sumRain += soilDrainageRain * (propend - propstart);
                        sumEvap += soilDrainageAE * (propend - propstart);
                    }

                }

                while (((endDate.Year - appDate.Year) * 12) + endDate.Month - appDate.Month > 1)
                {
                    // Add one month to appDate
                    appDate = appDate.AddMonths(1);

                    appDateRain = this.GetClimateType(appDate.Month, _climateCalculator, Enumerations.ClimateDataType.Rainfall);
                    applicationDateAE = this.GetClimateType(appDate.Month, _climateCalculator, Enumerations.ClimateDataType.ActualEvapotranspiration);


                    if (applicationDateAE > appDateRain)
                    {
                        sumRain += appDateRain;
                        sumEvap += appDateRain;
                    }
                    else
                    {
                        sumRain += appDateRain;
                        sumEvap += applicationDateAE;
                    }

                }

                // always round up
                _mannerApplication.RainfallTotal = (double)(long)Math.Round(sumRain + 0.5d);

                if (_mannerApplication.RainfallTotal < 0d)
                {
                    _mannerApplication.RainfallTotal = 0d;
                }
                _mannerApplication.EvapotranspirationTotal = (double)(long)Math.Round(sumEvap + 0.5d);

            }

            //TS.TraceEvent(TraceEventType.Information, 0, "Calculated rainfall:  " + MannerApplication.Weather.RainfallTotal);
            //TS.TraceEvent(TraceEventType.Information, 0, "Calculated Evapotranspiration:  " + MannerApplication.Weather.EvapotranspirationTotal);
        }


        catch (Exception)
        {
            //TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw;
        }

    }

    private void CheckAndChangeNegativeNResults()
    {
        if (_manureType.ID != (int)MannerLib.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated)
        {
            if (_outputs.ResultantNAvailable < 0d)
                _outputs.ResultantNAvailable = 0d;
        }

        if (_outputs.MineralisedN < 0d)
            _outputs.MineralisedN = 0d;
        if (_outputs.ResultantNAvailableYear2 < 0d)
            _outputs.ResultantNAvailableYear2 = 0d;
        if (_outputs.ResultantNAvailableSecondCut < 0d)
            _outputs.ResultantNAvailableSecondCut = 0d;
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
            //var ObjManData = new MannerLib.MannerData();
            string cropUse = _cropType.Use;  //ObjManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Crops, "CROPID", "CROPUSE", (int)MannerApplication.FieldData.CropTypeEnum);

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
            if (cropUse == "Grass")
            {
                dCDD4 = 100d;
            }
            else if (cropUse == "Arable")
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
            //TS.TraceEvent(TraceEventType.Information, 0, "Calculates the mineralised N available to the following crop.:  " + (dNMineralised4 + dNMineralised5));
            return dNMineralised4 + dNMineralised5;
        }

        catch (Exception)
        {
            // TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            //throw ex;
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
    /// <param name="NMin3"></param>
    /// <param name="VMTotal"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private double CalculateLeachedNNext(double NMin3, double VMTotal)
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

            dMinN4 = NMin3;      // passed into the function
            dVMTotal = VMTotal;  // also passed into the function

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

            //TS.TraceEvent(TraceEventType.Information, 0, "Calculates the leached N of the mineralised N susceptible to leaching for the following crop.:  " + dMinN4 * dLProp);

            return dMinN4 * dLProp;
        }

        catch (Exception)
        {
            //TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            //throw ex;
            return 0d;
        }

    }

    private double GetHer(int month)
    {
        double iHer;
        //var objManData = new MannerLib.MannerData();
        var climateType = _climateTypes.FirstOrDefault(c => c.MonthNumber == month);
        iHer = Convert.ToDouble(climateType?.HER ?? 0m); // Convert.ToDouble(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Climate, "MonthNo", "HER", month));

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
        _outputs.P2O5Total = Convert.ToDouble(_manureType.P2O5 * _mannerApplication.ApplicationRate.Value);
        _outputs.K2OTotal = Convert.ToDouble(_manureType.K2O * _mannerApplication.ApplicationRate.Value);
        _outputs.MgOTotal = Convert.ToDouble(_manureType.MgO * _mannerApplication.ApplicationRate.Value);
        _outputs.SO3Total = Convert.ToDouble(_manureType.SO3 * _mannerApplication.ApplicationRate.Value);
        _outputs.P2O5CropAvailable = Convert.ToDouble(_manureType.P2O5 * _mannerApplication.ApplicationRate.Value * (_manureType.P2O5Available / 100));
        _outputs.K2OCropAvailable = Convert.ToDouble(_manureType.K2O * _mannerApplication.ApplicationRate.Value * (_manureType.K2OAvailable / 100));

    }

    private void CalculateNAvailableResultsGrass(double mineralN3, double nMineralised2A, double calculatedcropUptakeFactor, double calculatedMineralisedN, double calculatedLeachedN)
    {
        switch (_mannerApplication.ApplicationDate.Month)
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
        if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated)
        {
            _outputs.ResultantNAvailable = -0.8d * (double)_mannerApplication.ApplicationRate.Value;
        }
        else
        {
            _outputs.ResultantNAvailable = 0d;
        }

        _outputs.ResultantNAvailableSecondCut = 0d;
        _outputs.ResultantNAvailableYear2 = 0d;
    }

    private double CalculateAmmoniaVolatilisation(double totalNAvailable, double potentialNAvailable, string cropuse, int incorporationCumulativeHours)
    {
        try
        {
            double dPVN0; // Potentially Volatilisable N - step 0
            double dPVN1; // Potentially volatilisable N - step 1
                          // soil moisture status (cattle slurry only) - step 2
            double dPVN2; // land use adjustment (cattle slurry only) - step 3
            double dPVN3; // dry matter adjustment (slurry only) - step 4
            double dPVN4; // application technique adjustment (slurry only) - step 5
            double dPVN5; // wind-speed adjustment (slurry only) - step 6
            double dPVN6; // temporary rainfall adjustment (slurry only) - step 7
            var dPVN7 = default(double); // final rainfall adjustment (slurry only) - step 7
            var dPVN8 = default(double); // incorporation timing (all manures) - step 8
                                         // (PVN7 - amount of ammonia lost up to the point of incorporation)
            double dPVN9; // incorporation technique (all manures)
                          // PVN8 figure adjusted for incorporation technique
                          // data consistent with the current version of NARSES

            var dTemp1 = default(double); // temporary variable
            var dTemp2 = default(double); // temporary variable

            //var objManData = new MannerLib.MannerData();
            //string cropuse;
            //bool iApplicationManType;
            double nmaxConstant;
            //int IncorporationCumulativeHours;
            //cropuse = objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Crops, "CROPID", "CropUse", (int)MannerApplication.FieldData.CropTypeEnum);
            //iApplicationManType =  Convert.ToInt32(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.ApplicationMethod, "ApplicationMethodID", "MANTYPE", (int)MannerApplication.Application.ApplicationMethodEnum));
            //IncorporationCumulativeHours = Convert.ToInt32(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.ApplicationDelay, "ID", "CumulativeHours", (int)MannerApplication.Application.DelayToIncorporationEnum));
            nmaxConstant = (double)_manureType.NMaxConstant; // MannerApplication.ManureType.NMaxConst;


            // Potentially Volatilisable N
            dPVN0 = potentialNAvailable * (nmaxConstant / 100);

            // Soil moisture adjustment (cattle slurry and liquid digested sludge only)
            // ------------------------------------------------------------------------
            // If the selected manure is cattle slurry or liquid digested sludge and the soil moisture status is dry then

            if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry && _mannerApplication.TopsoilMoistureID == (int)MannerLib.Enumerations.TopsoilMoistureEnum.Dry) || (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge && _mannerApplication.TopsoilMoistureID == (int)MannerLib.Enumerations.TopsoilMoistureEnum.Dry))
            {
                dPVN1 = dPVN0 * 1.3d;
            }
            // Else if the soil moisture status is moist then
            else if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry && _mannerApplication.TopsoilMoistureID == (int)MannerLib.Enumerations.TopsoilMoistureEnum.Moist) || (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge && _mannerApplication.TopsoilMoistureID == (int)MannerLib.Enumerations.TopsoilMoistureEnum.Moist))
            {
                dPVN1 = dPVN0 * 0.7d;
            }
            else
            {
                // Else all other manures remain unchanged
                dPVN1 = dPVN0;
            }

            // REMOVED SEASONAL ADJUSTMENT BECAUSE IT DIDN'T SEEM TO ALLOW FOR SOIL MOISTURE
            // WHICH HAS A GREATER EFFECT THAN SEASON - UPDATED TECHNICAL GUIDE

            // Land use adjustment(cattle slurry and liquid digested sludge only)
            // ------------------------------------------------------------------------
            // If the selected manure is cattle slurry or liquid digested sludge and the land use is arable

            if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry && cropuse == "Arable") || (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge && cropuse == "Arable"))
            {
                dPVN2 = dPVN1 * 0.85d;
            }
            // else if the manure is cattle slurry or liquid digest sludge and the land use is grass
            else if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry && cropuse == "Grass") || (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge && cropuse == "Grass"))
            {
                dPVN2 = dPVN1 * 1.15d;
            }
            // Else all other manures remain unchanged
            else
            {
                dPVN2 = dPVN1;
            }

            if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry && cropuse == "Arable") || (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge && cropuse == "Arable"))
            {
                dPVN2 = dPVN1 * 0.85d;
            }
            // else if the manure is cattle slurry or liquid digest sludge and the land use is grass
            else if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry & cropuse == "Grass") || (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge && cropuse == "Grass"))
            {
                dPVN2 = dPVN1 * 1.15d;
            }
            // Else all other manures remain unchanged
            else
            {
                dPVN2 = dPVN1;
            }

            // Dry matter adjustment (slurry or liquid digested sludge only)
            // -------------------------------------------------------------------------
            // if cattle slurry then
            if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry && _mannerApplication.TopsoilMoistureID == (int)MannerLib.Enumerations.TopsoilMoistureEnum.Moist) || (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge && _mannerApplication.TopsoilMoistureID == (int)MannerLib.Enumerations.TopsoilMoistureEnum.Moist))
            {
                // Not ((month(CurAppDate)) >= 5 And (month(CurAppDate)) <= 7) Then
                dPVN3 = (8.3d * (double)_manureType.DryMatter + 50.2d) / 100d * dPVN2;
            }
            // else if pig slurry then
            else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry && _mannerApplication.TopsoilMoistureID == (int)MannerLib.Enumerations.TopsoilMoistureEnum.Moist)
            {
                dPVN3 = (12.3d * (double)_manureType.DryMatter + 50.8d) / 100d * dPVN2;
            }
            // else all other manures and for dry soil PVN3 remains unchanged
            else
            {
                dPVN3 = dPVN2;
            }

            // Application technique (slurry or liquid digested sludge only)
            // -------------------------------------------------------------------------
            // If the application method is for a slurry then we are dealing with either a cattle, pig slurry or liquid digested sludge
            if (_manureType.IsLiquid)
            {
                // Select the application method name
                switch (_mannerApplication.ApplicationMethodID)
                {
                    // Adjust PVN4 depending on the application type
                    case (int)MannerLib.Enumerations.ApplicationMethodEnum.DeepInjection: // "Deep Injection"
                        {
                            double proportionOfNMax = 0.1d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 1d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case (int)MannerLib.Enumerations.ApplicationMethodEnum.ShallowInjection: // "Shallow Injection"
                        {
                            double proportionOfNMax = 0.3d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 0.55d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case (int)MannerLib.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingHose: // "Band Spreader - Trailing Hose"
                        {
                            double proportionOfNMax = 0.7d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 0.55d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case (int)MannerLib.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingShoeShortGrass: // "Band Spreader - Trailing Shoe (Short Grass)"
                        {
                            double proportionOfNMax = 0.7d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 0.55d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case (int)MannerLib.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingShoeLongGrass: // "Band Spreader - Trailing Shoe (Long Grass)"
                        {
                            double proportionOfNMax = 0.4d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 0.31d;
                            }

                            // else - in this case broadcast spreading
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    default:
                        {
                            dPVN4 = dPVN3;
                            break;
                        }
                }
            }
            else
            {
                // for all other slurries and application methods there is no adjustment
                dPVN4 = dPVN3;
            }

            // Windspeed (slurry only)
            // -------------------------------------------------------------------------
            // if the manure type is a slurry (of any kind) or a liquid digested sludge
            if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
            {

                // Adjust the NMax for wind speed
                switch (_mannerApplication.WindspeedID)
                {

                    case (int)MannerLib.Enumerations.WindSpeed.Moderate4to5BeaufortScale:  // "Moderate (4-5 Beaufort Scale)"
                        {
                            dPVN5 = dPVN4 * 1.2d;
                            break;
                        }

                    case (int)MannerLib.Enumerations.WindSpeed.StrongBreeze6to7BeaufortScale: // "Strong Breeze (6-7 Beaufort Scale)"
                        {
                            dPVN5 = dPVN4 * 1.6d;
                            break;
                        }

                    default:
                        {
                            dPVN5 = dPVN4;
                            break;
                        }

                }
            }
            else
            {
                // For all other manure types
                dPVN5 = dPVN4;
            }

            // Rainfall adjustment (slurry only)
            // -------------------------------------------------------------------------
            if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry)
            {

                switch (_mannerApplication.RainTypeID)
                {

                    // this uses the Michaelis Menton equation
                    // dPVN6 is used as Nmax
                    // time (t) is 6 hours
                    // dTemp1 is the amount of ammonia lost after 6 hours
                    // (referred to as A1 in the technical guide)
                    // Ammonia lost at time (t) = Nmax * (t/(t+Km))

                    // Values of Km
                    // Cattle slurry = 7.5
                    // Pig slurry = 11.6
                    // FYM (cattle, pig and duck) = 14.9
                    // Poultry manure = 40.4
                    // DigestateWholeFoodBased = 4.5

                    // Light rainfall adjustment
                    case (int)MannerLib.Enumerations.Rainfall.LightRainLessthan5mmWithin6Hours: // "Light rain (<5 mm) within 6 hours"
                        {
                            if (incorporationCumulativeHours <= 6)
                            {
                                dPVN7 = dPVN5;
                            }
                            else
                            {
                                dPVN6 = dPVN5 * 0.5d;

                                // If the manure is cattle slurry or liquid digested sludge
                                if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry | _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
                                {
                                    dTemp1 = dPVN6 * (6d / (6d + 7.5d));
                                    dPVN7 = dPVN6 - dTemp1;
                                }
                                // Else if the manure is pig slurry
                                else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry)
                                {
                                    double KM = 11.6d;
                                    // A.C new algorithim for Digistate Whole Food based
                                    if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                                    {
                                        KM = 4.5d;
                                    }
                                    dTemp1 = dPVN6 * (6d / (6d + KM));
                                    dPVN7 = dPVN6 - dTemp1;
                                }
                            }

                            break;
                        }

                    // Heavy rainfall adjustment
                    case (int)MannerLib.Enumerations.Rainfall.HeavyRainGreaterThan5mmWithin6hours: // "Heavy rain (>5 mm) within 6 hours"
                        {
                            if (incorporationCumulativeHours <= 6)
                            {
                                dPVN7 = dPVN5;
                            }
                            else
                            {
                                dPVN6 = dPVN5 * 0.3d;

                                // (Incorporation(IncorporationSel).CumulativeHours
                                // If the manure is cattle slurry or liquid digested sludge
                                if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry | _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
                                {
                                    dTemp1 = dPVN6 * (6d / (6d + 7.5d));
                                    dPVN7 = dPVN6 - dTemp1;
                                }
                                // Elseif the manure is pig slurry
                                else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry)
                                {
                                    double KM = 11.6d;
                                    // A.C new algorithim for Digistate Whole Food based
                                    if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                                    {
                                        KM = 4.5d;
                                    }
                                    dTemp1 = dPVN6 * (6d / (6d + KM));
                                    dPVN7 = dPVN6 - dTemp1;
                                }
                                // Else when there is no rainfall within 6 hours of spreading
                            }

                            break;
                        }

                    default:
                        {
                            dPVN7 = dPVN5;
                            break;
                        }

                }
            }
            else
            {
                // for all other slurries and manures it remains the same
                dPVN7 = dPVN5;
            }


            // Incorporation Timing (all manures)
            // -------------------------------------------------------------------------
            // If the manure is not incorporated then there is no adjustment for incorporation timing
            if (_mannerApplication.IncorporationMethodID == (int)MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated) // "Not Incorporated" Then
            {
                dPVN8 = dPVN7; // PVN8 = PVN7
            }
            else
            {
                // Else an adjustment is made for different manure types and different incorporation timings.
                switch (_manureType.ManureTypeCategoryID)
                {
                    // Use Michaelis-Menton equation 1 in the Technical Guide.
                    // Ammonia lost at time (t) = Nmax * (t/(t + Km))

                    // KM
                    // Cattle slurry:             7.5
                    // Pig slurry:                11.6
                    // FYM(cattle, pig and duck): 14.9
                    // Poultry manure:            40.4
                    // Digestate Whole Food Based 4.5

                    case (int)MannerLib.Enumerations.ManureCategory.FYM:  // Manure: FYM
                        {
                            dTemp2 = dPVN7 * (incorporationCumulativeHours / (incorporationCumulativeHours + 14.9d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case (int)MannerLib.Enumerations.ManureCategory.Poultry:  // Manure: Poultry
                        {
                            dTemp2 = dPVN7 * (incorporationCumulativeHours / (incorporationCumulativeHours + 40.4d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case (int)MannerLib.Enumerations.ManureCategory.CattleSlurry:  // Manure: Cattle Slurry
                        {
                            dTemp2 = dPVN7 * (incorporationCumulativeHours / (incorporationCumulativeHours + 7.5d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case (int)MannerLib.Enumerations.ManureCategory.PigSlurry:  // Manure: Pig Slurry
                        {
                            double KM = 11.6d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                KM = 4.5d;
                            }
                            dTemp2 = dPVN7 * (incorporationCumulativeHours / (incorporationCumulativeHours + KM));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case (int)MannerLib.Enumerations.ManureCategory.SolidSludge: // Solid sludge (treated the same as poultry manure c.f. e-mail 12/07/07)
                        {
                            dTemp2 = dPVN7 * (incorporationCumulativeHours / (incorporationCumulativeHours + 40.4d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case (int)MannerLib.Enumerations.ManureCategory.LiquidSludge: // Liquid sludge (treated the same as cattle slurry c.f. e-mail 12/07/07)
                        {
                            dTemp2 = dPVN7 * (incorporationCumulativeHours / (incorporationCumulativeHours + 7.5d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                }
            }

            // Incorporation Technique (all manures)
            // -------------------------------------------------------------------------
            // Need to make an adjustment based on the incorporation method and the manure type.  These data are consistent with NARSES, except for the
            // rotavator data which are not included in NARSES.  This "second phase" of ammonia loss after incorporation is PVN9.

            switch (_mannerApplication.IncorporationMethodID) // IncorporationMethods(IncMethodSel).Name
            {

                case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                    {
                        // Slurry or liquid digested sludge
                        if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.3d;
                        }
                        // Poultry or solid sewage sludges
                        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.Poultry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.SolidSludge)
                        {
                            dPVN9 = dPVN8 * 0.3d;
                        }
                        // FYM
                        else
                        {
                            dPVN9 = dPVN8 * 0.7d;
                        }

                        break;
                    }

                case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.Discs: // "Discs"
                    {
                        // Slurry or liquid digested sludge
                        if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.2d;
                        }
                        // Poultry or solid sewage sludges
                        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.Poultry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.SolidSludge)
                        {
                            dPVN9 = dPVN8 * 0.2d;
                        }
                        // FYM
                        else
                        {
                            dPVN9 = dPVN8 * 0.3d;
                        }

                        break;
                    }

                case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                    {
                        // Slurry or liquid digested sludge
                        if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.15d;
                        }
                        // Poultry or solid sewage sludges
                        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.Poultry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.SolidSludge)
                        {
                            dPVN9 = dPVN8 * 0.1d;
                        }
                        // FYM
                        else
                        {
                            dPVN9 = dPVN8 * 0.2d;
                        }

                        break;
                    }

                case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                    {
                        // Slurry or liquid digested sludge
                        if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.1d;
                        }
                        // Poultry or solid sewage sludges
                        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.Poultry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.SolidSludge)
                        {
                            dPVN9 = dPVN8 * 0.05d;
                        }
                        // FYM
                        else
                        {
                            dPVN9 = dPVN8 * 0.1d;

                            // Everything else
                        }

                        break;
                    }

                default:
                    {
                        dPVN9 = dPVN8;
                        break;
                    }

            }

            //TS.TraceEvent(TraceEventType.Information, 0, "Total ammonia volatilisation (kg/ha):  " + (dTemp1 + dTemp2 + dPVN9));

            // Total ammonia lost (kg/ha)
            return dTemp1 + dTemp2 + dPVN9;
        }
        catch (Exception)
        {
            //TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw;
        }


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
        if (this.IsPaperCrumble(_manureType.ID))
        {
            return 0d;
        }

        double dN2OEmission;
        double N2OEmissionFactor;
        // AC Three separate EFs: Slurry (0.85), FYM (0.73) & poultry manure (1.44)
        if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry)
        {
            // Slurry
            N2OEmissionFactor = 0.85d;
        }
        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.Poultry)
        {
            // Poultry
            N2OEmissionFactor = 1.44d;
        }
        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.FYM)
        {
            // FYM
            N2OEmissionFactor = 0.73d;
        }
        else
        {
            N2OEmissionFactor = 1.96d;
        }

        dN2OEmission = mineralN1 / 100d * N2OEmissionFactor;

        //TS.TraceEvent(TraceEventType.Information, 0, "Calculated N2O loss:  " + dN2OEmission);
        return dN2OEmission;

    }

    private bool IsPaperCrumble(int manureTypeId)
    {
        bool isPaperCrumble = manureTypeId == (int)MannerLib.Enumerations.ManureTypes.PaperCrumble;
        isPaperCrumble = isPaperCrumble || manureTypeId == (int)MannerLib.Enumerations.ManureTypes.PaperCrumbleBiologicallyTreated;
        isPaperCrumble = isPaperCrumble || manureTypeId == (int)MannerLib.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated;
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
        if (this.IsPaperCrumble(_manureType.ID))
        {
            return 0d;
        }

        double dN2Emission;

        dN2Emission = calculatedN2O * 2.9d;

        //TS.TraceEvent(TraceEventType.Information, 0, "denitrified N:  " + dN2Emission);
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
    private double CalculateMineralisedN(double calculatedTotalN, double calculatedPotentialN, ref double mineralN1, ref double organicN3, ref double mineralisedN2a, ref double cdd1, ref double cdd2, ref double cdd2a)
    {
        try
        {
            if (this.IsPaperCrumble(_manureType.ID))
            {
                mineralN1 = 0d;
                organicN3 = 0d;
                mineralisedN2a = 0d;
                cdd1 = 0d;
                cdd2 = 0d;
                cdd2a = 0d;
                return 0d;
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


            //var objMannerData = new MannerLib.MannerData();
            string cropUse = _cropType.Use; // objMannerData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Crops, "CROPID", "CropUse", (int)MannerApplication.FieldData.CropTypeEnum);

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
            int month = (int)_mannerApplication.ApplicationDate.Month;

            switch (cropUse ?? "")
            {
                case "Grass":
                    {
                        if (month >= 8 & month < 13)
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

                        else if (_mannerApplication.ApplicationDate.Month == 1 || _mannerApplication.ApplicationDate.Month == 2 || _mannerApplication.ApplicationDate.Month == 3 || _mannerApplication.ApplicationDate.Month == 4)
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

                        else if (_mannerApplication.ApplicationDate.Month == 5 || _mannerApplication.ApplicationDate.Month == 6 | _mannerApplication.ApplicationDate.Month == 7)
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
                        if (month >= 8 & month < 13)
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

                        else if (_mannerApplication.ApplicationDate.Month >= 1 | _mannerApplication.ApplicationDate.Month <= 8)
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
                case "Grass":
                    {
                        dNOrganic3 = dNOrganic2A - dNMineralised2A;
                        break;
                    }
                case "Arable":
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

            // Set a module level variables 
            mineralN1 = dNMineralised1;
            cdd1 = dCDD1;
            cdd2 = dCDD2;
            cdd2a = dCDD2A;
            mineralisedN2a = dNMineralised2A;
            organicN3 = dNOrganic3;

            // Return total mineralised N for the next crop
            //TS.TraceEvent(TraceEventType.Information, 0, "Calculates the mineralised N for the next crop:  " + (dNMineralised1 + dNMineralised2));

            return dNMineralised2;
        }

        catch (Exception)
        {
            //TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            //throw ex;
            return 0d;
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

        if ((int)_mannerApplication.ApplicationDate.Day > 15)
        {
            month++;
        }

        while (month < maxMonth)
        {
            cdd += tempArray[month];
            month++;
        }

        // Check the cumulative day degrees don't go above 2300
        if (cdd >= 2300d & limitTo2300)
        {
            cdd = 2299d;
        }

        return cdd;
    }

    private double CalculateMineralisedNForPeriod(double cumulativeDayDegrees, double organicN, double percentagedMineralisedNFymCattleSlurry = 0.008339d, double percentagedMineralisedPultrySlurrySludgeAndDefault = 0.02306d)
    {
        double mineralisedN;

        if ((_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.FYM || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry) && !this.IsBiosolidLiquidDigested(_manureType.ID))
        {
            mineralisedN = percentagedMineralisedNFymCattleSlurry * cumulativeDayDegrees / 100d * organicN;
        }
        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.None || this.IsPaperCrumble(_manureType.ID))
        {
            mineralisedN = 0d;
        }
        else if (_manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.Poultry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.PigSlurry || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.SolidSludge || _manureType.ManureTypeCategoryID == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
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
    private bool IsBiosolidLiquidDigested(int mannerTypeID)
    {
        return mannerTypeID == (int)MannerLib.Enumerations.ManureTypes.BiosolidsLiquidDigested;
    }

    private double AdjustMineralisedN2ForArableCrop(ref double mineralisedN2, double adjustmentFactor)
    {
        // Now adjust the value of NMin2 depending on the crop type
        // For cereals or oilseed rape multiply NMin2 by 0.6
        switch (_cropType.ID)
        {
            case (int)MannerLib.Enumerations.CropTypeEnum.EarlySownWinterCereal:
            case (int)MannerLib.Enumerations.CropTypeEnum.LateSownWinterCereal:
            case (int)MannerLib.Enumerations.CropTypeEnum.EarlyEstablishedWinterOilseedRape:
            case (int)MannerLib.Enumerations.CropTypeEnum.LateEstablishedWinterOilseedRape:
            case (int)MannerLib.Enumerations.CropTypeEnum.SpringCerealOilseedRape:
            case (int)MannerLib.Enumerations.CropTypeEnum.Potatoes:
            case (int)MannerLib.Enumerations.CropTypeEnum.Sugarbeet:
            case (int)MannerLib.Enumerations.CropTypeEnum.Other:
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
        if (this.IsPaperCrumble(_manureType.ID))
        {
            return 0d;
        }

        DateOnly datCurApp;
        DateOnly datEndDrain;
        double dMinN4;
        int lNitrificationDelay;


        int monthApp;
        int monthEOD;

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

        //var objManData = new MannerLib.MannerData();
        int IncorporationDelayHours;

        try
        {

            IncorporationDelayHours = _incorporationDelay.Hours ?? 0; //Convert.ToInt32(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.ApplicationDelay, "ID", "Hours", (int)MannerApplication.Application.DelayToIncorporationEnum));
                                                                      // Reset the Leached N variables to zero
            double calculateLeachedNRet = 0d;
            dHEREffective = 0d;

            dMinN4 = (double)mineralN4;

            // STEP 1 - Calculate Nitrification
            // The nitrification delay is dealt with in the Nitrification Technical Guide.
            // ----------------------------------------------------------------------------------
            // Date of application
            datCurApp = _mannerApplication.ApplicationDate;

            // Calculate the nitrification delay
            lNitrificationDelay = CalculateNitrificationDelay(datCurApp);

            // For simplicity the nitrification delay is added to the date of application
            // rather than treated as a range.
            // Reset the current application date to allow for this delay.
            datCurApp = datCurApp.AddDays(lNitrificationDelay);

            // set the application month variable to the date of the current manure application
            monthApp = datCurApp.Month; // System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(datCurApp);

            // set the variable for the date of end of drainage
            datEndDrain = _mannerApplication.EndOfDrainageDate;
            // 
            // 'Find the month of the end of soil drainage
            monthEOD = datEndDrain.Month;// System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(datEndDrain);

            // STEP 3 - Determine soil properties
            // ----------------------------------------------------------------------------------
            // Leaching calculation starts here   'check the date of application is less than the end of soil drainage
            if (datCurApp < datEndDrain)
            {

                // adjustment for moisture deficit and for incorporation, which differs from that for permeable soil because of the effects on bypass flow and surface runoff.

                // Get soil moisture deficit from MCDM.  Note: because MCDM calculates soil moisture deficit at the end of the month we use the SMD from
                // the previous month to the month of application.  Agreed following conversation with E. Lord on 26/7/2006.
                // As the count starts at zero then iMonthApp -.

                dSMDCurMonth = this.GetClimateType(monthApp, _climateCalculator, Enumerations.ClimateDataType.SoilMoistureDefecit);

                if (monthApp == 1)
                {
                    // if month is january take december as previous month
                    dSMDPrevMonth = this.GetClimateType(12, _climateCalculator, Enumerations.ClimateDataType.SoilMoistureDefecit);
                }
                else
                {
                    dSMDPrevMonth = this.GetClimateType(monthApp - 1, _climateCalculator, Enumerations.ClimateDataType.SoilMoistureDefecit);
                }

                // Even out the SMD factor depending on how far along the month we are.
                double SMDpropstart;
                SMDpropstart = (double)datCurApp.Day / (double)DateTime.DaysInMonth((int)_mannerApplication.ApplicationDate.Year, (int)_mannerApplication.ApplicationDate.Month);

                dSMD = dSMDPrevMonth + SMDpropstart * (dSMDCurMonth - dSMDPrevMonth);

                // Calculate the effective HER from effective application date to end of drainage.

                // If there was a soil moisture deficit then this affects adjustment factor

                if (dSMD > 0d)
                {
                    dHER = _mannerApplication.RainfallTotal - _mannerApplication.EvapotranspirationTotal;
                    dHEREffective = dHER;
                }

                // STEP 4 - Apply appropriate leaching algorithm
                // ----------------------------------------------------------------------------------
                // The subsoil contains the word CLAY
                // use the drained clay soil leaching algorithm
                // ----------------------------------------------------------------------------------
                // if the result of the string search for the word clay is greater than 0 then we have a clay soil

                if (_subSoil.ID == (int)MannerLib.Enumerations.SoilType.Clay || _subSoil.ID == (int)MannerLib.Enumerations.SoilType.ClayLoam || _subSoil.ID == (int)MannerLib.Enumerations.SoilType.SandyClay || _subSoil.ID == (int)MannerLib.Enumerations.SoilType.SandyClayLoam || _subSoil.ID == (int)MannerLib.Enumerations.SoilType.SiltyClay || _subSoil.ID == (int)MannerLib.Enumerations.SoilType.SiltyClayLoam)
                {

                    double dLProp1;
                    double dLProp2;
                    double dLProp3;
                    double dLRatioMod;
                    var dLAdjust = default(double);
                    double dInc;      // adjustment for method of incorporation

                    // Check if manure was incorporated (any value other than 'not incorporated'.
                    // IncorpFlag = ……
                    switch (_mannerApplication.IncorporationMethodID)
                    {
                        // If the manure has been ploughed down
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                            {
                                dInc = 0.9d;
                                break;
                            }
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                            {
                                dInc = 0.4d;
                                break;
                            }
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                            {
                                dInc = 0.4d;
                                break;
                            }
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated: // "Not Incorporated"
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
                    // dInc = dInc * .5
                    if (IncorporationDelayHours > 168)
                    {
                        dInc *= 0.5d;
                    }
                    //else
                    //{
                    //    dInc = dInc;
                    //}

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
                    if (dSMD > 0d | dInc > 0d)
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
                    switch (_mannerApplication.IncorporationMethodID)
                    {
                        // If the manure has been cultivated or ploughed down within a month ie at all
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                            {
                                dVMEffective = vmTotal - 0.5d * vmTop;
                                break;
                            }
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                            {
                                dVMEffective = vmTotal - 0.25d * vmTop;
                                break;
                            }
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                            {
                                dVMEffective = vmTotal - 0.25d * vmTop;
                                break;
                            }
                        // If it hasn't been incorporated then nothing changes
                        case (int)MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated: // "Not Incorporated"
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


            // TS.TraceEvent(TraceEventType.Information, 0, "Calculate leached N:  " + CalculateLeachedNRet);
            return calculateLeachedNRet;
        }
        catch (Exception)
        {
            //TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            //throw ex;
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
    public double GetClimateType(int month, ClimateCalculator climate, Enumerations.ClimateDataType climateType)
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
    private int CalculateNitrificationDelay(DateOnly dateOfApplication)
    {
        // ********************************************************************************
        DateOnly datDateofApplication;
        var lNoofDays = default(int);
        int monthNumber;

        datDateofApplication = dateOfApplication;

        // get the month of application from the date of application
        monthNumber = datDateofApplication.Month; // System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(datDateofApplication);

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

        //TS.TraceEvent(TraceEventType.Information, 0, "based on the month of application find the number of days for the nitrification delay:  " + lNoofDays);
        // Return the number of days for the nitrification delay
        return lNoofDays;

    }

    private double ApplyMineralisationFactor()
    {
        // EG Modification required to multiply mineralisation by 2 for poultry only.
        // some biosolids as set as manure category as poultry but these don't need the factor applied.
        if (_manureType.ID == (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter || _manureType.ID == (int)MannerLib.Enumerations.ManureTypes.PoultryManure)
        {
            return 2d;
        }
        else
        {
            return 1d;
        }

    }
}
