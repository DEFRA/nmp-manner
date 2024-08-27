using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;
public class MannerDll
{
    private NutrientsType OTemp;
    private DryMatter ODMTemp;
    internal MannerLib.Manner MannerApplication { get; set; } = new MannerLib.Manner();

    public Climate ClimateObject = new Climate();
    public TraceSource TS = new TraceSource("MannerDll");
    private ArrayList m_ManureTypeList;

    #region Properties

    public int CountryCode
    {
        get
        {
            return MannerApplication.CountryCode;
        }
        set
        {
            MannerApplication.CountryCode = value;
        }
    }

    public int RunType
    {
        get
        {
            return MannerApplication.RunType;
        }
        set
        {
            MannerApplication.RunType = value;

            var objData = new MannerLib.MannerData();
            m_ManureTypeList = objData.GetManureTypes(MannerApplication.RunType);

        }
    }

    public string ManureName
    {
        get
        {
            return MannerApplication.ManureType.ManureNameString;
        }
    }

    public string LiquidOrSolid
    {
        get
        {
            return MannerApplication.ManureType.LiquidOrSolid;
        }
    }

    public int ManureType
    {
        get
        {
            return MannerApplication.ManureType.ManureID;
        }
        set
        {
            SetManureType(value);

        }
    }
    public bool AutoCalculate { get; set; }
    public bool IsMannerLoading { get; set; }

    // This property is private as requirments have changed and we no longer accept RB209 manures
    private int ManureTypeRb209
    {
        get
        {
            return MannerApplication.ManureType.ManureID;
        }
        set
        {
            TranslateToMannerManureType(value);
        }
    }

    public int CropTypeRb209
    {
        get
        {
            return (int)MannerApplication.FieldData.CropTypeEnum;
        }
        set
        {
            int mannerCropID = TranslateToMannerCropType(value);
            if (mannerCropID != -1)
            {
                MannerApplication.FieldData.CropTypeEnum = (MannerLib.Enumerations.CropTypeEnum)mannerCropID;
                //MannerApplication.FieldData.CropTypeEnum.IsValidCropTypeEngland = true;
                MannerApplication.FieldData.CropTypeName = MannerLib.Enumerations.CropTypeEnumToString(MannerApplication.FieldData.CropTypeEnum);

                if (ValidateMannerSetCorrectly())
                {
                    if (AutoCalculate)
                    {
                        CalculateManner();
                    }
                }
            }
        }
    }
    #region Gets and sets for nutrients
    public DryMatter DryMatter
    {
        get
        {
            ODMTemp.LocalDrymatter = MannerApplication.ManureType.DryMatter;
            return ODMTemp;

        }
        set
        {


        }
    }

    public NutrientsType TotalN
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.TotalN;
            return OTemp;

        }
        set
        {
            MannerApplication.ManureType.TotalN.Value = value.Value;
        }
    }

    public NutrientsType P2O5
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.P2O5;
            return OTemp;
        }

        set
        {
            MannerApplication.ManureType.P2O5.Value = value.Value;

            if (AutoCalculate)
            {
                CalculateManner();
            }

        }
    }

    public NutrientsType K2O
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.K2O;

            return OTemp;
        }
        set
        {
            MannerApplication.ManureType.K2O.Value = value.Value;

            if (AutoCalculate)
            {
                CalculateManner();
            }

        }
    }

    public NutrientsType MgO
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.MgO;
            return OTemp;

        }
        set
        {
            MannerApplication.ManureType.MgO.Value = value.Value;

            if (AutoCalculate)
            {
                CalculateManner();
            }

        }
    }

    public NutrientsType SO3
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.SO3;

            return OTemp;
        }
        set
        {
            MannerApplication.ManureType.SO3.Value = value.Value;

            if (AutoCalculate)
            {
                CalculateManner();
            }

        }
    }

    public NutrientsType UricAcidN
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.UricAcidN;

            return OTemp;
        }
        set
        {
            MannerApplication.ManureType.UricAcidN.Value = value.Value;

            if (AutoCalculate)
            {
                CalculateManner();
            }

        }
    }

    public NutrientsType NH4_N
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.NH4N;

            return OTemp;
        }
        set
        {
            MannerApplication.ManureType.NH4N.Value = value.Value;

            if (AutoCalculate)
            {
                CalculateManner();
            }
        }
    }

    public NutrientsType NitrateN
    {
        get
        {
            OTemp.LocalNutrient = MannerApplication.ManureType.NitrateN;

            return OTemp;
        }
        set
        {
            MannerApplication.ManureType.NitrateN.Value = value.Value;

            if (AutoCalculate)
            {
                CalculateManner();
            }

        }
    }
    #endregion

    #region Gets and sets for application

    public double ApplicationRate
    {
        get
        {
            return MannerApplication.Application.Rate;
        }
        set
        {

            if (value < 0d | value > 999d)
            {
                throw new Exception("Application rate must not be greater than 999");
            }
            else
            {
                MannerApplication.Application.Rate = value;

                if (ValidateMannerSetCorrectly())
                {
                    if (AutoCalculate)
                    {
                        CalculateManner();
                    }
                }

            }

        }
    }

    // C Lam: added Oct 2012
    public double ApplicationRateGrass
    {
        get
        {
            return MannerApplication.Application.RateGrass;
        }
    }

    // C Lam: added Nov 2012
    public double ApplicationRateArable
    {
        get
        {
            return MannerApplication.Application.RateArable;
        }
    }

    public int ApplicationMethodId
    {
        get
        {
            return (int)MannerApplication.Application.ApplicationMethodEnum;
        }

        set
        {

            MannerApplication.Application.ApplicationMethodEnum = (MannerLib.Enumerations.ApplicationMethodEnum)value;
            MannerApplication.Application.ApplicationMethodString = MannerLib.Enumerations.ApplicationMethodToString(MannerApplication.Application.ApplicationMethodEnum);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string ApplicationMethodName
    {
        get
        {
            return MannerApplication.Application.ApplicationMethodString;
        }

    }

    public int MethodOfIncorporationId
    {
        get
        {
            return (int)MannerApplication.Application.MethodOfIncorporationEnum;
        }
        set
        {
            MannerApplication.Application.MethodOfIncorporationEnum = (MannerLib.Enumerations.MethodOfIncorporationEnum)value;
            MannerApplication.Application.MethodOfIncorporationString = MannerLib.Enumerations.MethodOfIncorporationToString(MannerApplication.Application.MethodOfIncorporationEnum);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string MethodOfIncorporationName
    {
        get
        {
            return MannerApplication.Application.MethodOfIncorporationString;
        }
    }

    public int DelayToIncorporationId
    {
        get
        {
            return (int)MannerApplication.Application.DelayToIncorporationEnum;
        }
        set
        {
            MannerApplication.Application.DelayToIncorporationEnum = (MannerLib.Enumerations.DelayToIncorporationEnum)value;
            MannerApplication.Application.DelayToIncorporationString = MannerLib.Enumerations.DelayToIncorporationToString(MannerApplication.Application.DelayToIncorporationEnum);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string DelayToIncorporationName
    {
        get
        {
            return MannerApplication.Application.DelayToIncorporationString;
        }
    }
    #endregion

    #region Gets and sets for Weather
    public DateTime DateOfApplication
    {
        get
        {
            return MannerApplication.Weather.DateOfApplication;
        }
        set
        {
            MannerApplication.Weather.DateOfApplication = new DateTime((short)value.Year, (byte)value.Month, (byte)value.Day);

            // if running for planet England or planet Scotland then for months May, June and July set topsoil moisture to Dry else set it to moist.
            if (RunType == (int)MannerLib.MannerData.RunAs.PlanetEngland | RunType == (int)MannerLib.MannerData.RunAs.PlanetScotland)
            {
                if (value.Month >= 5 & value.Month <= 7)
                {
                    MannerApplication.FieldData.TopsoilMoistureEnum = MannerLib.Enumerations.TopsoilMoistureEnum.Dry;
                }
                else
                {
                    MannerApplication.FieldData.TopsoilMoistureEnum = MannerLib.Enumerations.TopsoilMoistureEnum.Moist;
                }
            }

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    if ((int)MannerApplication.Weather.DateOfApplication.Day > 0 & (int)MannerApplication.Weather.EndOfSoilDrainage.Day > 0)
                    {
                        this.CalculateRainfall(MannerApplication.Weather.DateOfApplication, MannerApplication.Weather.EndOfSoilDrainage);
                    }
                    CalculateManner();
                }
            }

        }
    }

    public DateTime EndOfSoilDrainageDate
    {
        get
        {
            return MannerApplication.Weather.EndOfSoilDrainage;
        }
        set
        {

            // If value.Month < 8 Then
            if (value.Month < 5) // 17 Jan 2013 Lizzie says "allow an end of soil drainage date between 1st Jan and 30th Apr"
            {
                MannerApplication.Weather.EndOfSoilDrainage = new DateTime((short)value.Year, (byte)value.Month, (byte)value.Day);

                if (ValidateMannerSetCorrectly())
                {
                    if (AutoCalculate)
                    {
                        if ((int)MannerApplication.Weather.DateOfApplication.Day > 0 & (int)MannerApplication.Weather.EndOfSoilDrainage.Day > 0)
                        {
                            this.CalculateRainfall(MannerApplication.Weather.DateOfApplication, MannerApplication.Weather.EndOfSoilDrainage);
                        }
                        CalculateManner();
                    }
                }
            }
            else
            {
                // Throw New Exception("The date for End of soil drainage must be between 1st Jan and 31st July")
                throw new Exception("The date for End of soil drainage must be between 1st Jan and 30th Apr");
            }

        }
    }

    public int CropNUptake
    {
        get
        {
            return MannerApplication.FieldData.CropNUptake;
        }
        set
        {

            if (value > 100)
            {
                throw new Exception("Crop N Uptake has a maximum permitted value of 100 kg/ha");
            }
            else if (value < 0)
            {
                throw new Exception("Crop N Uptake must be greater than (or equal to) zero");
            }
            else
            {
                MannerApplication.FieldData.CropNUptake = value;

                if (ValidateMannerSetCorrectly())
                {
                    if (AutoCalculate)
                    {
                        CalculateManner();
                    }
                }

            }

        }
    }

    public double RainfallAmount
    {
        get
        {
            return MannerApplication.Weather.RainfallTotal;
        }
        set
        {

            if (value > 9999d)
            {
                throw new Exception("Total Rain has a maximum permitted value of 9999 mm");
            }
            else if (value < 0d)
            {
                throw new Exception("Total Rain must be greater than (or equal to) zero");
            }
            else
            {
                MannerApplication.Weather.RainfallTotal = value;

                if (ValidateMannerSetCorrectly())
                {
                    if (AutoCalculate)
                    {
                        CalculateManner();
                    }
                }
            }
        }
    }

    public int WindSpeedId
    {
        get
        {
            return (int)MannerApplication.Weather.WindspeedEnum;
        }
        set
        {
            MannerApplication.Weather.WindspeedEnum = (MannerLib.Enumerations.WindSpeed)value;
            MannerApplication.Weather.WindspeedString = MannerLib.Enumerations.WindSpeedToString(MannerApplication.Weather.WindspeedEnum);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string WindSpeedName
    {
        get
        {
            return MannerApplication.Weather.WindspeedString;
        }
    }

    public int RainfallId
    {
        get
        {
            return (int)MannerApplication.Weather.RainfallEnum;
        }
        set
        {
            MannerApplication.Weather.RainfallEnum = (MannerLib.Enumerations.Rainfall)value;
            MannerApplication.Weather.RainfallString = MannerLib.Enumerations.RainfallToString(MannerApplication.Weather.RainfallEnum);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string RainfallName
    {
        get
        {
            return MannerApplication.Weather.RainfallString;
        }

    }

    public double EvapotranspirationTotal
    {
        get
        {
            return MannerApplication.Weather.EvapotranspirationTotal;
        }
        set
        {
            MannerApplication.Weather.EvapotranspirationTotal = value;

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    #region Set Climate arrays
    public void set_SetMonthlyRainfallValues(int month, double value)
    {
        ClimateObject.MCDMObject.set_Rainfall(month, value);
    }

    public bool SetMonthlyClimateValues(double[] rainfall, double[] maxTemp, double[] minTemp, double[] rainDays, double[] sunHours, double[] windSpeed)
    {
        int iMonth;
        if (rainfall.GetUpperBound(0) == 11)
        {
            for (iMonth = 0; iMonth <= 11; iMonth++)
                ClimateObject.MCDMObject.set_Rainfall(iMonth, rainfall[iMonth]);
        }
        else
        {
            throw new ArgumentException("The input parameter rainfall does not contain sufficient monthly data.");
        }


        if (maxTemp.GetUpperBound(0) == 11)
        {
            for (iMonth = 0; iMonth <= 11; iMonth++)
                ClimateObject.MCDMObject.set_MaxTemp(iMonth, maxTemp[iMonth]);
        }
        else
        {
            throw new ArgumentException("The input parameter MaxTemp does not contain sufficient monthly data.");
        }

        if (minTemp.GetUpperBound(0) == 11)
        {
            for (iMonth = 0; iMonth <= 11; iMonth++)
                ClimateObject.MCDMObject.set_MinTemp(iMonth, minTemp[iMonth]);
        }
        else
        {
            throw new ArgumentException("The input parameter MinTemp does not contain sufficient monthly data.");
        }

        if (rainDays.GetUpperBound(0) == 11)
        {
            for (iMonth = 0; iMonth <= 11; iMonth++)
                ClimateObject.MCDMObject.set_RainDays(iMonth, rainDays[iMonth]);
        }
        else
        {
            throw new ArgumentException("The input parameter RainDays does not contain sufficient monthly data.");
        }

        if (sunHours.GetUpperBound(0) == 11)
        {
            for (iMonth = 0; iMonth <= 11; iMonth++)
                ClimateObject.MCDMObject.set_SunHours(iMonth, sunHours[iMonth]);
        }
        else
        {
            throw new ArgumentException("The input parameter SunHours does not contain sufficient monthly data.");
        }

        if (windSpeed.GetUpperBound(0) == 11)
        {
            for (iMonth = 0; iMonth <= 11; iMonth++)
                ClimateObject.MCDMObject.set_WindSpeed(iMonth, windSpeed[iMonth]);
        }
        else
        {
            throw new ArgumentException("The input parameter WindSpeed does not contain sufficient monthly data.");
        }

        if (!string.IsNullOrEmpty(MannerApplication.Postcode))
        {
            ClimateObject.GetClimate(MannerApplication.RunType, MannerApplication.Postcode, MannerApplication.FieldData.CropTypeEnum, MannerApplication.FieldData.Topsoil, MannerApplication.FieldData.Subsoil, true);
        }
        else
        {
            ClimateObject.GetClimate(MannerApplication.RunType, MannerApplication.Easting, MannerApplication.Northing, MannerApplication.FieldData.CropTypeEnum, MannerApplication.FieldData.Topsoil, MannerApplication.FieldData.Subsoil, true);
        }

        return default;

    }
    #endregion

    #endregion

    #region Gets and sets for Field Data

    public string FieldID
    {
        get
        {
            return MannerApplication.FieldData.FieldID;
        }
        set
        {
            MannerApplication.FieldData.FieldID = value;

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public int CropTypeID
    {
        get
        {
            return (int)MannerApplication.FieldData.CropTypeEnum;
        }
        set
        {
            MannerApplication.FieldData.CropTypeEnum = (MannerLib.Enumerations.CropTypeEnum)value;
            //MannerApplication.FieldData.CropTypeEnum.IsValidCropTypeEngland = true;
            MannerApplication.FieldData.CropTypeName = MannerLib.Enumerations.CropTypeEnumToString(MannerApplication.FieldData.CropTypeEnum);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string CropTypeName
    {
        get
        {
            return MannerApplication.FieldData.CropTypeName;
        }
    }

    public int TopsoilId
    {
        get
        {
            return (int)MannerApplication.FieldData.Topsoil;
        }
        set
        {
            MannerApplication.FieldData.Topsoil = (Enumerations.SoilType) value;
            //MannerApplication.FieldData.TopsoilName = MannerApplication.FieldData.TopsoilName;

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string TopsoilName
    {
        get
        {
            return MannerApplication.FieldData.TopsoilName;
        }
    }

    public int SubsoilId
    {
        get
        {
            return (int)MannerApplication.FieldData.Subsoil;
        }

        set
        {

            MannerApplication.FieldData.Subsoil =(Enumerations.SoilType) value;
            MannerApplication.FieldData.SubsoilName = MannerLib.Enumerations.SoilTypeToString(MannerApplication.FieldData.Subsoil);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string SubsoilName
    {
        get
        {
            return MannerApplication.FieldData.SubsoilName;
        }
    }

    public int TopsoilMoistureId
    {
        get
        {
            return (int)MannerApplication.FieldData.TopsoilMoistureEnum;

        }
        set
        {
            MannerApplication.FieldData.TopsoilMoistureEnum = (MannerLib.Enumerations.TopsoilMoistureEnum)value;

            MannerApplication.FieldData.TopsoilMoistureName = MannerLib.Enumerations.TopsoilMoistureToString(MannerApplication.FieldData.TopsoilMoistureEnum);

            if (ValidateMannerSetCorrectly())
            {
                if (AutoCalculate)
                {
                    CalculateManner();
                }
            }

        }
    }

    public string TopsoilMoistureName
    {
        get
        {
            return MannerApplication.FieldData.TopsoilMoistureName;
        }
    }

    #endregion

    #region Output properties

    public double TotalNitrogenAppliedOutput
    {
        get
        {
            return MannerApplication.Outputs.TotalNitrogenApplied;
        }
    }
    public double PotentialCropAvailableNOutput
    {
        get
        {
            return MannerApplication.Outputs.PotentialCropAvailableN;
        }
    }
    public double NH3_NLossOutput
    {
        get
        {
            return MannerApplication.Outputs.NH3NLoss;
        }
    }

    public double N20_NLossOutput
    {
        get
        {
            return MannerApplication.Outputs.N2ONLoss;
        }
    }

    public double N2_NLossOutput
    {
        get
        {
            return MannerApplication.Outputs.N2NLoss;
        }
    }

    public double N03_NLossOutput
    {
        get
        {
            return MannerApplication.Outputs.NO3NLoss;
        }
    }

    public double MineralisedNOutput
    {
        get
        {
            return MannerApplication.Outputs.MineralisedN;
        }
    }

    public double ResultantNAvailableOutput
    {
        get
        {
            return MannerApplication.Outputs.ResultantNAvailable;
        }
    }

    public double ResultantNAvailableSecondCutOutput
    {
        get
        {
            return MannerApplication.Outputs.ResultantNAvailableSecondCut;
        }
    }

    public double ResultantNAvailableYear2Output
    {
        get
        {
            return MannerApplication.Outputs.ResultantNAvailableYear2;
        }
    }

    public double P2O5_CropAvailableOutput
    {
        get
        {
            return MannerApplication.Outputs.P2O5CropAvailable;
        }
    }

    public double P2O5_TotalOutput
    {
        get
        {
            return MannerApplication.Outputs.P2O5Total;
        }
    }

    public double K2O_CropAvailableOutput
    {
        get
        {
            return MannerApplication.Outputs.K2OCropAvailable;
        }
    }

    public double K2O_TotalOutput
    {
        get
        {
            return MannerApplication.Outputs.K2OTotal;
        }
    }

    public double SO3_TotalOutput
    {
        get
        {
            return MannerApplication.Outputs.SO3Total;
        }
    }

    public double MgO_TotalOutput
    {
        get
        {
            return MannerApplication.Outputs.MgOTotal;
        }
    }

    public double PotentialEconomicValueOutput
    {
        get
        {
            return MannerApplication.Outputs.PotentialEconomicValue;
        }
    }
    #endregion

    #endregion

    /// <summary>
    /// Creates an instance of Manner. Requires country code, Easting and Northing to retrieve the climate data, RunAs parameter to set the manner default types returned.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <param name="postcode"></param>
    /// <param name="runAs"></param>
    /// <remarks></remarks>
    public MannerDll(int countryCode, string postcode, MannerLib.MannerData.RunAs runAs)
    {
        OTemp = new NutrientsType();
        ODMTemp = new DryMatter();
        MannerApplication.CountryCode = countryCode;
        MannerApplication.Postcode = postcode;
        MannerApplication.RunType = (int)runAs;
        AutoCalculate = false;
        // m_ClimateFileXml = XMLClimateFile
        SetMannerCropDefaults();
        SetDateDefaults();

        SetManureType(-1);
        // populate manure types 
        var objData = new MannerLib.MannerData();
        m_ManureTypeList = objData.GetManureTypes(MannerApplication.RunType);
        OTemp.NutrientValueChanged += OTemp_NutrientValueChanged;
        OTemp.NutrientOutOfRange += OTemp_DryMatterOutofRange;
        ODMTemp.DryMatterOutOfRange += ODMTemp_DryMatterOutofRange;
        ODMTemp.DryMatterValueChanged += ODMTemp_DryMatterValueChanged;
    }


    /// <summary>
    /// Creates an instance of Manner. Requires country code, Easting and Northing to retrieve the climate data, RunAs parameter to set the manner default types returned.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <param name="easting"></param>
    /// <param name="northing"></param>
    /// <param name="runAs"></param>
    /// <remarks></remarks>
    public MannerDll(int countryCode, int easting, int northing, MannerLib.MannerData.RunAs runAs)
    {
        OTemp = new NutrientsType();
        ODMTemp = new DryMatter();

        MannerApplication.CountryCode = countryCode;
        MannerApplication.Easting = easting;
        MannerApplication.Northing = northing;
        MannerApplication.RunType = (int)runAs;
        AutoCalculate = false;
        // m_ClimateFileXml = XMLClimateFile
        SetMannerCropDefaults();
        SetDateDefaults();

        SetManureType(-1);
        // populate manure types 
        var objData = new MannerLib.MannerData();
        m_ManureTypeList = objData.GetManureTypes(MannerApplication.RunType);
        OTemp.NutrientValueChanged += OTemp_NutrientValueChanged;
        OTemp.NutrientOutOfRange += OTemp_DryMatterOutofRange;
        ODMTemp.DryMatterOutOfRange += ODMTemp_DryMatterOutofRange;
        ODMTemp.DryMatterValueChanged += ODMTemp_DryMatterValueChanged;

    }


    /// <summary>
    /// This method returns an array list of manure type defaults. 
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetManureTypesList()
    {
        try
        {
            return m_ManureTypeList;
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Returns the list of application method defaults.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetApplicationMethodTypesList()
    {
        try
        {
            var objdata = new MannerLib.MannerData();
            return objdata.GetApplicationMethodTypes(MannerApplication.RunType, (int)MannerApplication.ManureType.ManureCategory);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }


    }

    /// <summary>
    /// Returns a list of incorporation methods defaults, this list is dependant on the type of application method that has been selected.
    /// </summary>
    /// <param name="ApplicationMethod"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetApplicationMethodOfIncorporationList(int ApplicationMethod)
    {
        try
        {
            var objdata = new MannerLib.MannerData();
            return objdata.GetApplicationMethodOfIncorporation(MannerApplication.RunType, ApplicationMethod);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }

    }

    /// <summary>
    /// Returns a list of Incorporation Delay defaults,  this list is dependant on the type of application method that has been selected.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetApplicationIncorporationDelayList()
    {
        try
        {
            var objData = new MannerLib.MannerData();
            bool isManureLiquid = MannerApplication.ManureType.LiquidOrSolid == "L";
            return objData.GetApplicationIncorporationDelay(MannerApplication.RunType, (int)MannerApplication.Application.ApplicationMethodEnum, (int)MannerApplication.Application.MethodOfIncorporationEnum, (int)MannerApplication.ManureType.ManureNameEnum, isManureLiquid);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// returns a list of crop type defaults
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetCropTypesList()
    {

        try
        {
            var objData = new MannerLib.MannerData();
            return objData.GetCropTypes(MannerApplication.RunType);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }

    }

    /// <summary>
    /// Returns a list of top soil defaults
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetTopsoilList()
    {
        try
        {
            var objdata = new MannerLib.MannerData();
            return objdata.GetTopsoil(MannerApplication.RunType);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }

    }

    /// <summary>
    /// returns a list of sub soil defaults
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetSubsoilList()
    {
        try
        {
            var objData = new MannerLib.MannerData();
            return objData.GetSubsoil(MannerApplication.RunType);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }
    }
    /// <summary>
    /// returns a list of soil moisture defaults
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetSoilMoistureList()
    {
        try
        {
            var objData = new MannerLib.MannerData();
            return objData.GetSoilMoisture(MannerApplication.RunType);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }
    }
    /// <summary>
    /// Returns a list of wind speed defaults
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetWindSpeedList()
    {
        try
        {
            var objData = new MannerLib.MannerData();
            return objData.GetWindSpeed(MannerApplication.RunType);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// returns a list of default rainfall conditions.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ArrayList GetRainfallList()
    {
        try
        {
            var objData = new MannerLib.MannerData();
            return objData.GetRainfall(MannerApplication.RunType);
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="manureID"></param>
    /// <remarks></remarks>
    private void SetManureType(int manureID)
    {
        try
        {

            if (manureID != -1)
            {
                var ObjData = new MannerLib.MannerData();
                var argmanner = MannerApplication;
                ObjData.PopulateManureType(manureID, ref argmanner);
                MannerApplication = argmanner;

                if (MannerApplication.ManureType.DryMatter.Value != 0d)
                {
                    this.SetDryMatter(MannerApplication.ManureType.DryMatter.Value); // Put this in as Lizzie wanted the exactly values from the relationship.
                }
                SetApplicationMethodDefaults();
            }
            else
            {
                MannerApplication.ManureType.ManureID = manureID;
            }
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
        }
    }

    /// <summary>
    /// Map manure type from RB209 to internal manner ID to use one of the current ones
    /// </summary>
    /// <param name="manureRB2009ID"></param>
    /// <remarks></remarks>
    private void TranslateToMannerManureType(int manureRB2009ID)
    {
        try
        {
            if (manureRB2009ID != -1)
            {
                int internalMannerManureTypeID = -1;
                switch (manureRB2009ID)
                {
                    case 1:
                        {
                            internalMannerManureTypeID = 1; // CattleFYMOld
                            break;
                        }
                    case 2:
                        {
                            internalMannerManureTypeID = 0; // CattleFYMFresh
                            break;
                        }
                    case 3:
                        {
                            internalMannerManureTypeID = 3; // PigFYMOld
                            break;
                        }
                    case 4:
                        {
                            internalMannerManureTypeID = 2; // PigFYMFresh
                            break;
                        }
                    case 5:
                        {
                            internalMannerManureTypeID = 5; // SheepFYMOld
                            break;
                        }
                    case 6:
                        {
                            internalMannerManureTypeID = 4; // SheepFYMFresh
                            break;
                        }
                    case 7:
                        {
                            internalMannerManureTypeID = 7; // DuckFYMOld
                            break;
                        }
                    case 8:
                        {
                            internalMannerManureTypeID = 6; // DuckFYMFresh
                            break;
                        }
                    case 9:
                    case 10:
                        {
                            internalMannerManureTypeID = 30; // HorseFYMOld and HorseFYMFresh
                            break;
                        }
                    case 11:
                    case 12:
                        {
                            internalMannerManureTypeID = 41; // GoatFYMOld and GoatFYMFresh
                            break;
                        }
                    case var @case when 13 <= @case && @case <= 16:
                        {
                            internalMannerManureTypeID = -1; // PoultryManure20Percent to PoultryManure80Percent
                            break;
                        }
                    case var case1 when 17 <= case1 && case1 <= 19:
                        {
                            internalMannerManureTypeID = 45; // CattleSlurry2Percent to CattleSlurry10Percent
                            break;
                        }
                    case 20:
                        {
                            internalMannerManureTypeID = 19; // DirtyWater
                            break;
                        }
                    case 21:
                        {
                            internalMannerManureTypeID = 13; // SeparatedCattleSlurryStrainerBox
                            break;
                        }
                    case 22:
                        {
                            internalMannerManureTypeID = 14; // SeparatedCattleSlurryMechanicalSeparator
                            break;
                        }
                    case 23:
                        {
                            internalMannerManureTypeID = 15; // SeparatedCattleSlurryWeepingWall
                            break;
                        }
                    case 24:
                        {
                            internalMannerManureTypeID = 16; // SeparatedCattleSlurryMechanicalSeparator
                            break;
                        }
                    case var case2 when 25 <= case2 && case2 <= 27:
                        {
                            internalMannerManureTypeID = 12; // PigSlurry2Percent to PigSlurry6Percent
                            break;
                        }
                    case 28:
                        {
                            internalMannerManureTypeID = 18; // SeparatedPigSlurryLiquidPortion
                            break;
                        }
                    case 29:
                        {
                            internalMannerManureTypeID = 17; // SeparatedPigSlurrySolidPortion
                            break;
                        }
                    case 30:
                        {
                            internalMannerManureTypeID = 21; // BiosolidsDigestedCake
                            break;
                        }
                    case 31:
                        {
                            internalMannerManureTypeID = 22; // BiosolidsThermallyDried
                            break;
                        }
                    case 32:
                        {
                            internalMannerManureTypeID = 23; // BiosolidsLimeStabilised
                            break;
                        }
                    case 33:
                        {
                            internalMannerManureTypeID = 42; // BiosolidsComposted
                            break;
                        }
                    case 34:
                        {
                            internalMannerManureTypeID = 24; // GreenCompost
                            break;
                        }
                    case 35:
                        {
                            internalMannerManureTypeID = 32; // GreenFoodCompost
                            break;
                        }
                    case 36:
                        {
                            internalMannerManureTypeID = 46; // DigestateWholeFoodBased
                            break;
                        }
                    case var case3 when 37 <= case3 && case3 <= 41:
                        {
                            internalMannerManureTypeID = -1; // DigestateSeparatedLiquorFoodBased to DigestateSeparatedFibreFarmSourced
                            break;
                        }
                    case 42:
                        {
                            internalMannerManureTypeID = 33; // PaperCrumbleChemicallyPhysicallyTreated
                            break;
                        }
                    case 43:
                        {
                            internalMannerManureTypeID = 34; // PaperCrumbleBiologicallyTreated
                            break;
                        }
                    case 44:
                        {
                            internalMannerManureTypeID = 35; // SpentMushroomCompost
                            break;
                        }
                    case 45:
                        {
                            internalMannerManureTypeID = 36; // WaterTreatmentCake
                            break;
                        }
                    case 46:
                        {
                            internalMannerManureTypeID = 27; // FoodWasteDairy
                            break;
                        }
                    case 47:
                        {
                            internalMannerManureTypeID = 28; // FoodWasteSoftDrinks
                            break;
                        }
                    case 48:
                        {
                            internalMannerManureTypeID = 29; // FoodWasteBrewing
                            break;
                        }
                    case 49:
                        {
                            internalMannerManureTypeID = 26; // FoodWasteGeneral
                            break;
                        }
                }
                SetManureType(internalMannerManureTypeID);
            }
            else
            {
                MannerApplication.ManureType.ManureID = manureRB2009ID;
            }
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
        }
    }

    private int TranslateToMannerCropType(int cropRb2009ID)
    {
        try
        {
            int internalMannerCropTypeID = -1;
            if (cropRb2009ID != -1)
            {

                switch (cropRb2009ID)
                {
                    case 0:
                    case 1:
                    case 4:
                    case 6:
                    case 8:
                    case 52:
                    case 53:
                    case var @case when 57 <= @case && @case <= 59:
                        {
                            internalMannerCropTypeID = 2;
                            break;
                        }
                    case 2:
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                    case var case1 when 171 <= case1 && case1 <= 174:
                    case 21:
                    case 50:
                    case 51:
                    case var case2 when 54 <= case2 && case2 <= 56:
                        {
                            internalMannerCropTypeID = 6;
                            break;
                        }
                    case 20:
                        {
                            internalMannerCropTypeID = 4;
                            break;
                        }
                    case var case3 when 22 <= case3 && case3 <= 25:
                    case 27:
                    case 28:
                    case 175:
                    case 176:
                    case 187:
                    case var case4 when 40 <= case4 && case4 <= 45:
                    case 188:
                    case 189:
                    case 191:
                    case 194:
                    case 195:
                    case 60:
                    case var case5 when 61 <= case5 && case5 <= 75:
                    case var case6 when 77 <= case6 && case6 <= 79:
                    case 181:
                    case var case7 when 90 <= case7 && case7 <= 94:
                    case 182:
                    case var case8 when 110 <= case8 && case8 <= 125:
                    case 177:
                    case 178:
                    case 170:
                    case 184:
                    case 185:
                    case 192:
                    case 193:
                    case 76:
                    case 179:
                    case 180:
                        {
                            internalMannerCropTypeID = 9;
                            break;
                        }
                    case 26:
                        {
                            internalMannerCropTypeID = 8;
                            break;
                        }
                    case 140:
                        {
                            internalMannerCropTypeID = 1;
                            break;
                        }
                    case var case9 when 160 <= case9 && case9 <= 163:
                        {
                            internalMannerCropTypeID = 7;
                            break;
                        }
                }
                CropTypeID = internalMannerCropTypeID;
            }
            return internalMannerCropTypeID;
        }
        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Critical, 0, ex.Message);
        }

        return default;
    }
    // Public Sub SetManureTypeOld(ByVal ManureID As Integer)
    // Try
    // Dim ObjData As New MannerData
    // ObjData.PopulateManureTypeOld(ManureID, MannerApplication)
    // Catch ex As Exception
    // ts.TraceEvent(TraceEventType.Critical, 0, ex.Message)
    // End Try
    // End Sub

    /// <summary>
    /// Sets the value of dry matter %. This method then recalcs total N, NH4-N, P2O5, K2O, MgO as they each have a relationship with dry matter, They are recalcualted based on equations.
    /// </summary>
    /// <param name="DryMatter"></param>
    /// <remarks></remarks>
    private void SetDryMatter(double DryMatter)
    {

        MannerApplication.ManureType.DryMatter.Value = DryMatter;

        CalculateNitrogenContent();
        CalculateUricContent();
        CalculateNH4NContent();
        CalculatePContent();
        CalculateKContent();
        CalculateSContent();
        CalculateMgContent();

    }

    // C Lam Sep 2012 - added
    private void SetTotalN(double TotalN)
    {

        MannerApplication.ManureType.TotalN.Value = TotalN;

        CalculateUricContent();
        CalculateNH4NContent();

    }

    /// <summary>
    /// Calculates Nitrogen based on the relationship it has with dry matter.
    /// </summary>
    /// <remarks></remarks>
    private void CalculateNitrogenContent()
    {
        double newTotalN;

        switch (MannerApplication.ManureType.ManureID)
        {

            case (int)MannerLib.Enumerations.ManureTypes.BeefSlurry:
            case (int)MannerLib.Enumerations.ManureTypes.DairySlurry:
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurry:
                {
                    newTotalN = 0.25d * MannerApplication.ManureType.DryMatter.Value + 1.1d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PigSlurry:
                {
                    newTotalN = 0.39d * MannerApplication.ManureType.DryMatter.Value + 2.04d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {
                    newTotalN = 0.46d * MannerApplication.ManureType.DryMatter.Value + 0.2d; // Was 0.29 * MannerApplication.ManureType.DryMatter.Value + 8.85
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {
                    newTotalN = 0.44d * MannerApplication.ManureType.DryMatter.Value + 3.6d;
                    break;
                }

            default:
                {
                    newTotalN = MannerApplication.ManureType.TotalN.Value;
                    break;
                }
        }

        MannerApplication.ManureType.TotalN.Value = newTotalN;
        MannerApplication.ManureType.TotalN.DisplayValue = this.GetDisplayValue(MannerApplication.ManureType.TotalN.DisplayValue, newTotalN);

    }

    private string GetDisplayValue(string OldDispVal, double NewCalcVal)
    {
        string sDispVal;
        if (NewCalcVal == 0d & string.IsNullOrEmpty(OldDispVal))
        {
            sDispVal = "";
        }
        else
        {
            sDispVal = NewCalcVal.ToString();
        }

        return sDispVal;
    }

    /// <summary>
    /// calculates Uric based on the relationship it has with dry matter.
    /// </summary>
    /// <remarks></remarks>
    private void CalculateUricContent()
    {
        double newUric;

        switch (MannerApplication.ManureType.ManureID)
        {

            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {
                    newUric = (MannerApplication.ManureType.TotalN.Value * 0.5d - 0.2d) * 0.4d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {
                    newUric = (MannerApplication.ManureType.TotalN.Value * 0.35d - 0.2d) * 0.4d;
                    break;
                }

            default:
                {
                    newUric = MannerApplication.ManureType.UricAcidN.Value;
                    break;
                }
        }

        MannerApplication.ManureType.UricAcidN.Value = newUric;
        MannerApplication.ManureType.UricAcidN.DisplayValue = this.GetDisplayValue(MannerApplication.ManureType.UricAcidN.DisplayValue, newUric);

    }
    /// <summary>
    /// Calculates NH4 based on the relationship it has with dry matter.
    /// </summary>
    /// <remarks></remarks>
    private void CalculateNH4NContent()
    {
        double newNH4N;

        switch (MannerApplication.ManureType.ManureID)
        {

            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {

                    newNH4N = 0.08d * MannerApplication.ManureType.DryMatter.Value + 4.54d; // ((MannerApplication.ManureType.TotalN.Value_ * 0.5) - 0.2) * 0.6
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {

                    newNH4N = (MannerApplication.ManureType.TotalN.Value * 0.35d - 0.2d) * 0.6d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.DairySlurry:
            case (int)MannerLib.Enumerations.ManureTypes.BeefSlurry:
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurry:
                {

                    newNH4N = (58.5d - 2.28d * MannerApplication.ManureType.DryMatter.Value) / 100d * MannerApplication.ManureType.TotalN.Value;
                    break;
                }
            case (int)MannerLib.Enumerations.ManureTypes.PigSlurry:
                {

                    newNH4N = (82d - 3.03d * MannerApplication.ManureType.DryMatter.Value) / 100d * MannerApplication.ManureType.TotalN.Value;
                    break;
                }

            default:
                {

                    newNH4N = MannerApplication.ManureType.NH4N.Value;
                    break;
                }
        }

        MannerApplication.ManureType.NH4N.Value = newNH4N;
        MannerApplication.ManureType.NH4N.DisplayValue = this.GetDisplayValue(MannerApplication.ManureType.NH4N.DisplayValue, newNH4N);

    }
    /// <summary>
    /// calculates P based on the relationship it has with dry matter.
    /// </summary>
    /// <remarks></remarks>
    private void CalculatePContent()
    {
        double newP;

        switch (MannerApplication.ManureType.ManureID)
        {

            case (int)MannerLib.Enumerations.ManureTypes.BeefSlurry:
            case (int)MannerLib.Enumerations.ManureTypes.DairySlurry:
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurry:
                {

                    newP = 0.15d * MannerApplication.ManureType.DryMatter.Value + 0.3d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PigSlurry:
                {

                    newP = 0.36d * MannerApplication.ManureType.DryMatter.Value + 0.04d; // Was 0.4 * MannerApplication.ManureType.DryMatter.Value + 0.2
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {

                    newP = 0.22d * MannerApplication.ManureType.DryMatter.Value + 3.62d; // Was 0.33 * MannerApplication.ManureType.DryMatter.Value + 2.45
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {

                    newP = 0.37d * MannerApplication.ManureType.DryMatter.Value + 2.8d;
                    break;
                }

            default:
                {

                    newP = MannerApplication.ManureType.P2O5.Value;
                    break;
                }
        }

        MannerApplication.ManureType.P2O5.Value = newP;
        MannerApplication.ManureType.P2O5.DisplayValue = this.GetDisplayValue(MannerApplication.ManureType.P2O5.DisplayValue, newP);

    }
    /// <summary>
    /// calculates K based on the relationship it has with dry matter.
    /// </summary>
    /// <remarks></remarks>
    private void CalculateKContent()
    {

        double newK;

        switch (MannerApplication.ManureType.ManureID)
        {
            case (int)MannerLib.Enumerations.ManureTypes.BeefSlurry:
            case (int)MannerLib.Enumerations.ManureTypes.DairySlurry:
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurry:
                {
                    newK = 0.22d * MannerApplication.ManureType.DryMatter.Value + 1.25d; // Was 0.2 * MannerApplication.ManureType.DryMatter.Value + 2
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PigSlurry:
                {
                    newK = 0.2d * MannerApplication.ManureType.DryMatter.Value + 1.44d; // Was 0.2 * MannerApplication.ManureType.DryMatter.Value + 1.6
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {
                    newK = 0.3d * MannerApplication.ManureType.DryMatter.Value + 2.48d; // Was 0.27 * MannerApplication.ManureType.DryMatter.Value + 0.05
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {
                    newK = 0.19d * MannerApplication.ManureType.DryMatter.Value + 6.6d;
                    break;
                }

            default:
                {
                    newK = MannerApplication.ManureType.K2O.Value;
                    break;
                }

        }

        MannerApplication.ManureType.K2O.Value = newK;
        MannerApplication.ManureType.K2O.DisplayValue = this.GetDisplayValue(MannerApplication.ManureType.K2O.DisplayValue, newK);

    }
    /// <summary>
    /// calculates So3 based on the relationship it has with dry matter.
    /// </summary>
    /// <remarks></remarks>
    private void CalculateSContent()
    {
        double newS;
        bool bupdate;
        bupdate = false;

        switch (MannerApplication.ManureType.ManureID)
        {

            case (int)MannerLib.Enumerations.ManureTypes.BeefSlurry:
            case (int)MannerLib.Enumerations.ManureTypes.DairySlurry:
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurry:
                {

                    newS = 0.0875d * MannerApplication.ManureType.DryMatter.Value + 0.15d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PigSlurry:
                {

                    newS = 0.125d * MannerApplication.ManureType.DryMatter.Value + 0.47d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {

                    newS = 0.13d * MannerApplication.ManureType.DryMatter.Value + 0.39d; // Was 0.11 * MannerApplication.ManureType.DryMatter.Value + 0.15
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {

                    newS = 0.14d * MannerApplication.ManureType.DryMatter.Value - 0.4d;
                    break;
                }

            default:
                {

                    newS = MannerApplication.ManureType.SO3.Value;
                    break;
                }

        }

        MannerApplication.ManureType.SO3.Value = newS;
        MannerApplication.ManureType.SO3.DisplayValue = this.GetDisplayValue(MannerApplication.ManureType.SO3.DisplayValue, newS);

    }
    /// <summary>
    /// calculates Mg based on the relationship it has with dry matter.
    /// </summary>
    /// <remarks></remarks>
    private void CalculateMgContent()
    {
        double newM;

        switch (MannerApplication.ManureType.ManureID)
        {

            case (int)MannerLib.Enumerations.ManureTypes.BeefSlurry:
            case (int)MannerLib.Enumerations.ManureTypes.DairySlurry:
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurry:
                {

                    newM = 0.0875d * MannerApplication.ManureType.DryMatter.Value + 0.04d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PigSlurry:
                {

                    newM = 0.15d * MannerApplication.ManureType.DryMatter.Value + 0.1d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {

                    newM = 0.08d * MannerApplication.ManureType.DryMatter.Value + 1.1d;
                    break;
                }

            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {

                    newM = 0.06d * MannerApplication.ManureType.DryMatter.Value + 0.8d;
                    break;
                }

            default:
                {

                    newM = MannerApplication.ManureType.MgO.Value;
                    break;
                }

        }

        MannerApplication.ManureType.MgO.Value = newM;
        MannerApplication.ManureType.MgO.DisplayValue = this.GetDisplayValue(MannerApplication.ManureType.MgO.DisplayValue, newM);

    }

    // *****************************************************************************
    // ** Method:         calcManner
    // ** Created:        Steven Anthony
    // ** Modified:       Martina Gibbons 07/04/03
    // ** Parameters:     DateofApp       - current application date of manure
    // **                 AppRate         - manure application rate
    // **                 DMPerc          - % dry matter
    // **                 TotalN          - total N applied
    // **                 TotalAmmN       - total ammonium N
    // **                 TotalUricN      - total uric acid N
    // **                 TotalNitrateN   - total nitrate N
    // ** Return Value:   Boolean
    // ** Description:    CalcManner updates the MANNER results as any influencing
    // **                 factor changes. Function is called if there are changes to
    // **                 the interface OR changes are made by remote calls to the
    // **                 program.
    // *****************************************************************************
    /// <summary>
    /// Produces the Manner results and updates the class Outputs with the results.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool CalculateManner()
    {
        bool CalculateMannerRet = default;
        try
        {

            CalculateMannerRet = false;

            var mineralN1 = default(double);
            double mineralN2;
            double mineralN3;
            double mineralN4; // manure N remaining after losses through NH3 volatilisation + any nitrate N in the original manure application

            var objManData = new MannerLib.MannerData();

            string cropUse = objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Crops, "CROPID", "CropUse", (int)MannerApplication.FieldData.CropTypeEnum);

            CalculateNutrientsOutputsValues();

            // Available N
            // --------------------------------------------------------------
            double calculatedTotalN = MannerApplication.Application.Rate * MannerApplication.ManureType.TotalN.Value;

            // Readily Available N applied (NH4-N and uric acid N)
            // --------------------------------------------------------------
            // 18 Jan 2013 - Lizzie says "CalcPot = AppRate * (TotalAmmN + TotalUricN + TotalNitrateN)"
            double calculatedPotentialN = MannerApplication.Application.Rate * (MannerApplication.ManureType.NH4N.Value + MannerApplication.ManureType.UricAcidN.Value + MannerApplication.ManureType.NitrateN.Value);

            // Volatilised N
            // --------------------------------------------------------------
            double calculatedVolatilisedN = this.CalculateAmmoniaVolatilisation(calculatedTotalN, MannerApplication.Application.Rate * (MannerApplication.ManureType.NH4N.Value + MannerApplication.ManureType.UricAcidN.Value));

            // N2O Emission
            // --------------------------------------------------------------
            // N2O Emission is 1.74% of applied readily available N remaining following volatilisation
            double n2oEmission = calculatedPotentialN - calculatedVolatilisedN;
            double calculatedN2O = CalculateN2OEmission(n2oEmission);

            // N2 Emission
            // --------------------------------------------------------------
            double calculatedN2 = CalculateN2Emission(calculatedN2O);

            // Autumn Crop Uptake - crop N value in kg/ha which is subtracted before mineralisation and leaching
            // --------------------------------------------------------------
            // Total nitrate N added here following conversation with F.Nicholson on 30/08/2006
            mineralN2 = calculatedPotentialN - calculatedVolatilisedN - calculatedN2 - calculatedN2O;
            if (mineralN2 < 0d)
                mineralN2 = 0d;

            double calculatedcropUptakeFactor = this.CalculateCropUptakeFactor(mineralN2, (int)MannerApplication.Weather.DateOfApplication.Month, MannerApplication.FieldData.CropTypeEnum);

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
            double calculatedMineralisedN = this.CalculateMineralisedN(calculatedTotalN, calculatedPotentialN, MannerApplication.ManureType.TotalN.Value, ref mineralN1, ref mineralisedN3, ref mineralisedN2A, ref cdd1, ref cdd2, ref cdd2a);

            mineralN4 = mineralN3 + mineralN1;


            // Leached N
            // -------------------------------------------------------------
            // Calculate soil volumetric water content
            double vMWaterTopSoil = Convert.ToDouble(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "TopSoilVM", (int)MannerApplication.FieldData.Topsoil));
            double vMWaterTotal = vMWaterTopSoil + Convert.ToDouble(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "SubSoilVM", (int)MannerApplication.FieldData.Subsoil));

            double calculatedLeachedN = CalculateLeachedN(mineralN4, vMWaterTotal, vMWaterTopSoil);
            double nMineralised4 = mineralN4 - calculatedLeachedN;

            // Modification required to multiply mineralisation by 2 for poultry only.
            calculatedMineralisedN = calculatedMineralisedN * ApplyMineralisationFactor();
            mineralisedN2A = mineralisedN2A * ApplyMineralisationFactor();

            // Calculate final results and assign to public variables
            CalculateFinalResults(calculatedTotalN, calculatedPotentialN, calculatedVolatilisedN, calculatedN2O, calculatedN2, calculatedcropUptakeFactor, calculatedMineralisedN, calculatedLeachedN);


            if (this.IsPaperCrumble(MannerApplication.ManureType.ManureNameEnum))
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
                            MannerApplication.Outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN + calculatedcropUptakeFactor) * 10.0d) / 10d;
                            MannerApplication.Outputs.ResultantNAvailableSecondCut = 0d;
                            break;
                        }

                    default:
                        {
                            MannerApplication.Outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN + calculatedcropUptakeFactor) * 10.0d) / 10d;
                            MannerApplication.Outputs.ResultantNAvailableSecondCut = 0d;
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
            double calculatedMineralisedNNextCrop = CalculateMineralisedNNextCrop(nMineralised4, vMWaterTotal, cdd1, cdd2, cdd2a, mineralisedN3);
            MannerApplication.Outputs.ResultantNAvailableYear2 = (double)(int)Math.Round(calculatedMineralisedNNextCrop * 10.0d / 10d);

            CheckAndChangeNegativeNResults();

            CalculateMannerRet = true;
        }

        catch (Exception ex)
        {
            CalculateMannerRet = false;
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
        }

        return CalculateMannerRet;
    }

    private void CheckAndChangeNegativeNResults()
    {
        if (MannerApplication.ManureType.ManureNameEnum != MannerLib.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated)
        {
            if (MannerApplication.Outputs.ResultantNAvailable < 0d)
                MannerApplication.Outputs.ResultantNAvailable = 0d;
        }

        if (MannerApplication.Outputs.MineralisedN < 0d)
            MannerApplication.Outputs.MineralisedN = 0d;
        if (MannerApplication.Outputs.ResultantNAvailableYear2 < 0d)
            MannerApplication.Outputs.ResultantNAvailableYear2 = 0d;
        if (MannerApplication.Outputs.ResultantNAvailableSecondCut < 0d)
            MannerApplication.Outputs.ResultantNAvailableSecondCut = 0d;
    }

    private void CalculateNAvailableResultsGrass(double mineralN3, double nMineralised2A, double calculatedcropUptakeFactor, double calculatedMineralisedN, double calculatedLeachedN)
    {
        switch (MannerApplication.Weather.DateOfApplication.Month)
        {

            case (byte)1:
            case (byte)2:
            case (byte)3:
            case (byte)4:
                {
                    MannerApplication.Outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN) * 10.0d) / 10d;
                    MannerApplication.Outputs.ResultantNAvailableSecondCut = (int)Math.Round(nMineralised2A * 10.0d) / 10d;
                    break;
                }
            case (byte)5:
            case (byte)6:
            case (byte)7:
                {
                    MannerApplication.Outputs.MineralisedN = (int)Math.Round(nMineralised2A * 10.0d) / 10d;
                    MannerApplication.Outputs.ResultantNAvailable = (int)Math.Round((mineralN3 + nMineralised2A) * 10.0d) / 10d;
                    MannerApplication.Outputs.ResultantNAvailableSecondCut = 0d;
                    break;
                }
            case (byte)8:
            case (byte)9:
            case (byte)10:
            case (byte)11:
            case (byte)12:
                {
                    MannerApplication.Outputs.ResultantNAvailable = (int)Math.Round((mineralN3 - calculatedLeachedN + calculatedMineralisedN + calculatedcropUptakeFactor) * 10.0d) / 10d;
                    MannerApplication.Outputs.ResultantNAvailableSecondCut = (int)Math.Round(nMineralised2A * 10.0d) / 10d;
                    break;
                }
        }
    }

    private void CalculateNAvailableResultsPaperCrumble()
    {
        if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated)
        {
            MannerApplication.Outputs.ResultantNAvailable = -0.8d * MannerApplication.Application.Rate;
        }
        else
        {
            MannerApplication.Outputs.ResultantNAvailable = 0d;
        }

        MannerApplication.Outputs.ResultantNAvailableSecondCut = 0d;
        MannerApplication.Outputs.ResultantNAvailableYear2 = 0d;
    }

    private void CalculateFinalResults(double calculatedTotalN, double calculatedPotentialN, double calculatedVolatilisedN, double calculatedN2O, double calculatedN2, double cropUptakeFactor, double calculatedMineralisedN, double calculatedLeachedN)
    {
        MannerApplication.Outputs.TotalNitrogenApplied = (long)Math.Round(calculatedTotalN * 10.0d) / 10d;
        MannerApplication.Outputs.PotentialCropAvailableN = (int)Math.Round(calculatedPotentialN * 10.0d) / 10d;
        MannerApplication.Outputs.NH3NLoss = (int)Math.Round(calculatedVolatilisedN * 10.0d) / 10d;
        MannerApplication.Outputs.N2ONLoss = (int)Math.Round(calculatedN2O * 10.0d) / 10d;
        MannerApplication.Outputs.N2NLoss = (int)Math.Round(calculatedN2 * 10.0d) / 10d;
        MannerApplication.Outputs.MineralisedN = (int)Math.Round(calculatedMineralisedN * 10.0d) / 10d;
        MannerApplication.Outputs.NO3NLoss = (int)Math.Round(calculatedLeachedN * 10.0d) / 10d;
        MannerApplication.Outputs.CropUptake = cropUptakeFactor;
    }

    private void CalculateNutrientsOutputsValues()
    {
        MannerApplication.Outputs.P2O5Total = MannerApplication.ManureType.P2O5.Value * MannerApplication.Application.Rate;
        MannerApplication.Outputs.K2OTotal = MannerApplication.ManureType.K2O.Value * MannerApplication.Application.Rate;
        MannerApplication.Outputs.MgOTotal = MannerApplication.ManureType.MgO.Value * MannerApplication.Application.Rate;
        MannerApplication.Outputs.SO3Total = MannerApplication.ManureType.SO3.Value * MannerApplication.Application.Rate;
        MannerApplication.Outputs.P2O5CropAvailable = MannerApplication.ManureType.P2O5.Value * MannerApplication.Application.Rate * ((double)MannerApplication.ManureType.P2O5.AvailablePercent / 100d);
        MannerApplication.Outputs.K2OCropAvailable = MannerApplication.ManureType.K2O.Value * MannerApplication.Application.Rate * ((double)MannerApplication.ManureType.K2O.AvailablePercent / 100d);
    }

    private double ApplyMineralisationFactor()
    {
        // EG Modification required to multiply mineralisation by 2 for poultry only.
        // some biosolids as set as manure category as poultry but these don't need the factor applied.
        if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter | MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.PoultryManure)
        {
            return 2d;
        }
        else
        {
            return 1d;
        }

    }



    // *****************************************************************************
    // ** Method:         CalcVolatAmm
    // ** Created:        Martina Gibbons 14/04/03
    // ** Parameters:     DateofApp           - current application date of manure
    // **                 AppRate             - manure application rate
    // **                 DMPerc              - % dry matter
    // **                 TotalNAvailable     - total N available to the crop
    // **                 PotentialNAvailable - potential N available to the crop
    // **                 TotalUricN          - total uric acid N
    // ** Return Value:   Double - total ammonia lost
    // ** Description:    Called from calcManner.  Calculates the ammonia volatilisation.
    // **********************************************************************************
    /// <summary>
    /// Calculates the ammonia volatilisation.
    /// </summary>
    /// <param name="totalNAvailable"></param>
    /// <param name="potentialNAvailable"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private double CalculateAmmoniaVolatilisation(double totalNAvailable, double potentialNAvailable)
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

            var objManData = new MannerLib.MannerData();
            string cropuse;
            int iApplicationManType;
            double nmaxConstant;
            int IncorporationCumulativeHours;
            cropuse = objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Crops, "CROPID", "CropUse", (int)MannerApplication.FieldData.CropTypeEnum);
            iApplicationManType = Convert.ToInt32(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.ApplicationMethod, "ApplicationMethodID", "MANTYPE", (int)MannerApplication.Application.ApplicationMethodEnum));
            IncorporationCumulativeHours = Convert.ToInt32(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.ApplicationDelay, "ID", "CumulativeHours", (int)MannerApplication.Application.DelayToIncorporationEnum));
            nmaxConstant = MannerApplication.ManureType.NMaxConst;


            // Potentially Volatilisable N
            dPVN0 = potentialNAvailable * (nmaxConstant / 100d);

            // Soil moisture adjustment (cattle slurry and liquid digested sludge only)
            // ------------------------------------------------------------------------
            // If the selected manure is cattle slurry or liquid digested sludge and the soil moisture status is dry then

            if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry & MannerApplication.FieldData.TopsoilMoistureEnum == MannerLib.Enumerations.TopsoilMoistureEnum.Dry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge & MannerApplication.FieldData.TopsoilMoistureEnum == MannerLib.Enumerations.TopsoilMoistureEnum.Dry)
            {
                dPVN1 = dPVN0 * 1.3d;
            }
            // Else if the soil moisture status is moist then
            else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry & MannerApplication.FieldData.TopsoilMoistureEnum == MannerLib.Enumerations.TopsoilMoistureEnum.Moist | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge & MannerApplication.FieldData.TopsoilMoistureEnum == MannerLib.Enumerations.TopsoilMoistureEnum.Moist)
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

            if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry & cropuse == "Arable" | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge & cropuse == "Arable")
            {
                dPVN2 = dPVN1 * 0.85d;
            }
            // else if the manure is cattle slurry or liquid digest sludge and the land use is grass
            else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry & cropuse == "Grass" | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge & cropuse == "Grass")
            {
                dPVN2 = dPVN1 * 1.15d;
            }
            // Else all other manures remain unchanged
            else
            {
                dPVN2 = dPVN1;
            }



            if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry & cropuse == "Arable" | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge & cropuse == "Arable")
            {
                dPVN2 = dPVN1 * 0.85d;
            }
            // else if the manure is cattle slurry or liquid digest sludge and the land use is grass
            else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry & cropuse == "Grass" | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge & cropuse == "Grass")
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
            if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry & MannerApplication.FieldData.TopsoilMoistureEnum == MannerLib.Enumerations.TopsoilMoistureEnum.Moist | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge & MannerApplication.FieldData.TopsoilMoistureEnum == MannerLib.Enumerations.TopsoilMoistureEnum.Moist)
            {
                // Not ((month(CurAppDate)) >= 5 And (month(CurAppDate)) <= 7) Then
                dPVN3 = (8.3d * MannerApplication.ManureType.DryMatter.Value + 50.2d) / 100d * dPVN2;
            }
            // else if pig slurry then
            else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry & MannerApplication.FieldData.TopsoilMoistureEnum == MannerLib.Enumerations.TopsoilMoistureEnum.Moist)
            {
                dPVN3 = (12.3d * MannerApplication.ManureType.DryMatter.Value + 50.8d) / 100d * dPVN2;
            }
            // else all other manures and for dry soil PVN3 remains unchanged
            else
            {
                dPVN3 = dPVN2;
            }

            // Application technique (slurry or liquid digested sludge only)
            // -------------------------------------------------------------------------
            // If the application method is for a slurry then we are dealing with either a cattle, pig slurry or liquid digested sludge
            if (iApplicationManType == 4)
            {
                // Select the application method name
                switch (MannerApplication.Application.ApplicationMethodEnum)
                {
                    // Adjust PVN4 depending on the application type
                    case MannerLib.Enumerations.ApplicationMethodEnum.DeepInjection: // "Deep Injection"
                        {
                            double proportionOfNMax = 0.1d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 1d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case MannerLib.Enumerations.ApplicationMethodEnum.ShallowInjection: // "Shallow Injection"
                        {
                            double proportionOfNMax = 0.3d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 0.55d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case MannerLib.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingHose: // "Band Spreader - Trailing Hose"
                        {
                            double proportionOfNMax = 0.7d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 0.55d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case MannerLib.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingShoeShortGrass: // "Band Spreader - Trailing Shoe (Short Grass)"
                        {
                            double proportionOfNMax = 0.7d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                proportionOfNMax = 0.55d;
                            }
                            dPVN4 = dPVN3 * proportionOfNMax;
                            break;
                        }

                    case MannerLib.Enumerations.ApplicationMethodEnum.BandSpreaderTrailingShoeLongGrass: // "Band Spreader - Trailing Shoe (Long Grass)"
                        {
                            double proportionOfNMax = 0.4d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
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
            if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
            {

                // Adjust the NMax for wind speed
                switch (MannerApplication.Weather.WindspeedEnum)
                {

                    case MannerLib.Enumerations.WindSpeed.Moderate4to5BeaufortScale:  // "Moderate (4-5 Beaufort Scale)"
                        {
                            dPVN5 = dPVN4 * 1.2d;
                            break;
                        }

                    case MannerLib.Enumerations.WindSpeed.StrongBreeze6to7BeaufortScale: // "Strong Breeze (6-7 Beaufort Scale)"
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
            if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry)
            {

                switch (MannerApplication.Weather.RainfallEnum)
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
                    case MannerLib.Enumerations.Rainfall.LightRainLessthan5mmWithin6Hours: // "Light rain (<5 mm) within 6 hours"
                        {
                            if (IncorporationCumulativeHours <= 6)
                            {
                                dPVN7 = dPVN5;
                            }
                            else
                            {
                                dPVN6 = dPVN5 * 0.5d;

                                // If the manure is cattle slurry or liquid digested sludge
                                if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
                                {
                                    dTemp1 = dPVN6 * (6d / (6d + 7.5d));
                                    dPVN7 = dPVN6 - dTemp1;
                                }
                                // Else if the manure is pig slurry
                                else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry)
                                {
                                    double KM = 11.6d;
                                    // A.C new algorithim for Digistate Whole Food based
                                    if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
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
                    case MannerLib.Enumerations.Rainfall.HeavyRainGreaterThan5mmWithin6hours: // "Heavy rain (>5 mm) within 6 hours"
                        {
                            if (IncorporationCumulativeHours <= 6)
                            {
                                dPVN7 = dPVN5;
                            }
                            else
                            {
                                dPVN6 = dPVN5 * 0.3d;

                                // (Incorporation(IncorporationSel).CumulativeHours
                                // If the manure is cattle slurry or liquid digested sludge
                                if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
                                {
                                    dTemp1 = dPVN6 * (6d / (6d + 7.5d));
                                    dPVN7 = dPVN6 - dTemp1;
                                }
                                // Elseif the manure is pig slurry
                                else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry)
                                {
                                    double KM = 11.6d;
                                    // A.C new algorithim for Digistate Whole Food based
                                    if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
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
            if (MannerApplication.Application.MethodOfIncorporationEnum == MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated) // "Not Incorporated" Then
            {
                dPVN8 = dPVN7; // PVN8 = PVN7
            }
            else
            {
                // Else an adjustment is made for different manure types and different incorporation timings.
                switch (MannerApplication.ManureType.ManureCategory)
                {
                    // Use Michaelis-Menton equation 1 in the Technical Guide.
                    // Ammonia lost at time (t) = Nmax * (t/(t + Km))

                    // KM
                    // Cattle slurry:             7.5
                    // Pig slurry:                11.6
                    // FYM(cattle, pig and duck): 14.9
                    // Poultry manure:            40.4
                    // Digestate Whole Food Based 4.5

                    case MannerLib.Enumerations.ManureCategory.FYM:  // Manure: FYM
                        {
                            dTemp2 = dPVN7 * (IncorporationCumulativeHours / (IncorporationCumulativeHours + 14.9d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case MannerLib.Enumerations.ManureCategory.Poultry:  // Manure: Poultry
                        {
                            dTemp2 = dPVN7 * (IncorporationCumulativeHours / (IncorporationCumulativeHours + 40.4d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case MannerLib.Enumerations.ManureCategory.CattleSlurry:  // Manure: Cattle Slurry
                        {
                            dTemp2 = dPVN7 * (IncorporationCumulativeHours / (IncorporationCumulativeHours + 7.5d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case MannerLib.Enumerations.ManureCategory.PigSlurry:  // Manure: Pig Slurry
                        {
                            double KM = 11.6d;
                            // A.C new algorithim for Digistate Whole Food based
                            if (MannerApplication.ManureType.ManureNameEnum == MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased)
                            {
                                KM = 4.5d;
                            }
                            dTemp2 = dPVN7 * (IncorporationCumulativeHours / (IncorporationCumulativeHours + KM));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case MannerLib.Enumerations.ManureCategory.SolidSludge: // Solid sludge (treated the same as poultry manure c.f. e-mail 12/07/07)
                        {
                            dTemp2 = dPVN7 * (IncorporationCumulativeHours / (IncorporationCumulativeHours + 40.4d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                    case MannerLib.Enumerations.ManureCategory.LiquidSludge: // Liquid sludge (treated the same as cattle slurry c.f. e-mail 12/07/07)
                        {
                            dTemp2 = dPVN7 * (IncorporationCumulativeHours / (IncorporationCumulativeHours + 7.5d));
                            dPVN8 = dPVN7 - dTemp2;
                            break;
                        }
                }
            }

            // Incorporation Technique (all manures)
            // -------------------------------------------------------------------------
            // Need to make an adjustment based on the incorporation method and the manure type.  These data are consistent with NARSES, except for the
            // rotavator data which are not included in NARSES.  This "second phase" of ammonia loss after incorporation is PVN9.

            switch (MannerApplication.Application.MethodOfIncorporationEnum) // IncorporationMethods(IncMethodSel).Name
            {

                case MannerLib.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                    {
                        // Slurry or liquid digested sludge
                        if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.3d;
                        }
                        // Poultry or solid sewage sludges
                        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.Poultry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.SolidSludge)
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

                case MannerLib.Enumerations.MethodOfIncorporationEnum.Discs: // "Discs"
                    {
                        // Slurry or liquid digested sludge
                        if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.2d;
                        }
                        // Poultry or solid sewage sludges
                        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.Poultry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.SolidSludge)
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

                case MannerLib.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                    {
                        // Slurry or liquid digested sludge
                        if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.15d;
                        }
                        // Poultry or solid sewage sludges
                        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.Poultry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.SolidSludge)
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

                case MannerLib.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                    {
                        // Slurry or liquid digested sludge
                        if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
                        {
                            dPVN9 = dPVN8 * 0.1d;
                        }
                        // Poultry or solid sewage sludges
                        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.Poultry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.SolidSludge)
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

            TS.TraceEvent(TraceEventType.Information, 0, "Total ammonia volatilisation (kg/ha):  " + (dTemp1 + dTemp2 + dPVN9));

            // Total ammonia lost (kg/ha)
            return dTemp1 + dTemp2 + dPVN9;
        }

        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
        }


    }
    /// <summary>
    /// Calculates N2O loss
    /// </summary>
    /// <param name="mineralN1"></param>
    /// <returns></returns>
    /// <remarks></remarks>
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
        if (this.IsPaperCrumble(MannerApplication.ManureType.ManureNameEnum))
        {
            return 0d;
        }

        double dN2OEmission;
        double N2OEmissionFactor;
        // AC Three separate EFs: Slurry (0.85), FYM (0.73) & poultry manure (1.44)
        if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry)
        {
            // Slurry
            N2OEmissionFactor = 0.85d;
        }
        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.Poultry)
        {
            // Poultry
            N2OEmissionFactor = 1.44d;
        }
        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.FYM)
        {
            // FYM
            N2OEmissionFactor = 0.73d;
        }
        else
        {
            N2OEmissionFactor = 1.96d;
        }

        dN2OEmission = mineralN1 / 100d * N2OEmissionFactor;

        TS.TraceEvent(TraceEventType.Information, 0, "Calculated N2O loss:  " + dN2OEmission);
        return dN2OEmission;

    }

    /// <summary>
    /// Calculates denitrified N
    /// </summary>
    /// <param name="N2OEmission"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private double CalculateN2Emission(double N2OEmission)
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
        if (this.IsPaperCrumble(MannerApplication.ManureType.ManureNameEnum))
        {
            return 0d;
        }

        double dN2Emission;

        dN2Emission = N2OEmission * 2.9d;

        TS.TraceEvent(TraceEventType.Information, 0, "denitrified N:  " + dN2Emission);
        return dN2Emission;

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
    private double CalculateCropUptakeFactor(double mineralN2, int month, MannerLib.Enumerations.CropTypeEnum cropType)
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

            TS.TraceEvent(TraceEventType.Information, 0, "Calculated the uptake of manure N by a crop planted after a late summer/autumn manure application.:  " + CropUpdateFactor);

            return CropUpdateFactor;
        }

        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
            return 0d;
        }

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
    private double CalculateMineralisedN(double calculatedTotalN, double calculatedPotentialN, double totalManureN, ref double mineralN1, ref double organicN3, ref double mineralisedN2a, ref double cdd1, ref double cdd2, ref double cdd2a)
    {

        try
        {
            if (this.IsPaperCrumble(MannerApplication.ManureType.ManureNameEnum))
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


            var objMannerData = new MannerLib.MannerData();
            string cropUse = objMannerData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Crops, "CROPID", "CropUse", (int)MannerApplication.FieldData.CropTypeEnum);

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
            int month = (int)MannerApplication.Weather.DateOfApplication.Month;

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

                        else if ((int)MannerApplication.Weather.DateOfApplication.Month == 1 | (int)MannerApplication.Weather.DateOfApplication.Month == 2 | (int)MannerApplication.Weather.DateOfApplication.Month == 3 | (int)MannerApplication.Weather.DateOfApplication.Month == 4)
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

                        else if ((int)MannerApplication.Weather.DateOfApplication.Month == 5 | (int)MannerApplication.Weather.DateOfApplication.Month == 6 | (int)MannerApplication.Weather.DateOfApplication.Month == 7)
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

                        else if ((int)MannerApplication.Weather.DateOfApplication.Month >= 1 | (int)MannerApplication.Weather.DateOfApplication.Month <= 8)
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
            TS.TraceEvent(TraceEventType.Information, 0, "Calculates the mineralised N for the next crop:  " + (dNMineralised1 + dNMineralised2));

            return dNMineralised2;
        }

        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
            return 0d;
        }
    }

    private double CalculateCddForMineralisedN(int[] tempArray, int month, int maxMonth, bool limitTo2300)
    {
        // cumulative day degrees (CDD)
        var cdd = default(double);
        // If the date of application is before 15th of the month include that month in the calculation of CDD reset the variables

        if ((int)MannerApplication.Weather.DateOfApplication.Day > 15)
        {
            month = month + 1;
        }

        while (month < maxMonth)
        {
            cdd = cdd + tempArray[month];
            month = month + 1;
        }

        // Check the cumulative day degrees don't go above 2300
        if (cdd >= 2300d & limitTo2300)
        {
            cdd = 2299d;
        }

        return cdd;
    }

    private double AdjustMineralisedN2ForArableCrop(ref double mineralisedN2, double adjustmentFactor)
    {
        // Now adjust the value of NMin2 depending on the crop type
        // For cereals or oilseed rape multiply NMin2 by 0.6
        switch (MannerApplication.FieldData.CropTypeEnum)
        {
            case MannerLib.Enumerations.CropTypeEnum.EarlySownWinterCereal:
            case MannerLib.Enumerations.CropTypeEnum.LateSownWinterCereal:
            case MannerLib.Enumerations.CropTypeEnum.EarlyEstablishedWinterOilseedRape:
            case MannerLib.Enumerations.CropTypeEnum.LateEstablishedWinterOilseedRape:
            case MannerLib.Enumerations.CropTypeEnum.SpringCerealOilseedRape:
            case MannerLib.Enumerations.CropTypeEnum.Potatoes:
            case MannerLib.Enumerations.CropTypeEnum.Sugarbeet:
            case MannerLib.Enumerations.CropTypeEnum.Other:
                {

                    return mineralisedN2 * adjustmentFactor;
                }

            default:
                {
                    return mineralisedN2;
                }
        }
    }

    private double CalculateMineralisedNForPeriod(double cumulativeDayDegrees, double organicN, double percentagedMineralisedNFymCattleSlurry = 0.008339d, double percentagedMineralisedPultrySlurrySludgeAndDefault = 0.02306d)
    {
        double mineralisedN;

        if ((MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.FYM | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.CattleSlurry) & !this.IsBiosolidLiquidDigested(MannerApplication.ManureType.ManureNameEnum))
        {
            mineralisedN = percentagedMineralisedNFymCattleSlurry * cumulativeDayDegrees / 100d * organicN;
        }
        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.None | this.IsPaperCrumble(MannerApplication.ManureType.ManureNameEnum))
        {
            mineralisedN = 0d;
        }
        else if (MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.Poultry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.PigSlurry | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.SolidSludge | MannerApplication.ManureType.ManureCategory == MannerLib.Enumerations.ManureCategory.LiquidSludge)
        {
            mineralisedN = percentagedMineralisedPultrySlurrySludgeAndDefault * cumulativeDayDegrees / 100d * organicN;
        }
        else
        {
            mineralisedN = percentagedMineralisedPultrySlurrySludgeAndDefault * cumulativeDayDegrees / 100d * organicN;
        }

        return mineralisedN;
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


    /// <summary>
    /// Calculates the leached N.
    /// </summary>
    /// <param name="MineralN4"></param>
    /// <param name="dVMTotal"></param>
    /// <param name="dVMTop"></param>
    /// <returns type="Double">Leached N</returns>
    /// <remarks>Removed from the calcManner routine to allow more flexibility</remarks>
    private double CalculateLeachedN(double MineralN4, double dVMTotal, double dVMTop)
    {
        double CalculateLeachedNRet = default;

        // 07 Nov 2012 C Lam - Return zero for paper crumbles
        if (this.IsPaperCrumble(MannerApplication.ManureType.ManureNameEnum))
        {
            return 0d;
        }

        DateTime datCurApp;
        DateTime datEndDrain;
        double dMinN4;
        long lNitrificationDelay;


        int iMonthApp;
        int iMonthEOD;

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

        var objManData = new MannerLib.MannerData();
        int IncorporationDelayHours;

        try
        {

            IncorporationDelayHours = Convert.ToInt32(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.ApplicationDelay, "ID", "Hours", (int)MannerApplication.Application.DelayToIncorporationEnum));
            // Reset the Leached N variables to zero
            CalculateLeachedNRet = 0d;
            dHEREffective = 0d;

            dMinN4 = MineralN4;

            // STEP 1 - Calculate Nitrification
            // The nitrification delay is dealt with in the Nitrification Technical Guide.
            // ----------------------------------------------------------------------------------
            // Date of application
            datCurApp = MannerApplication.Weather.DateOfApplication;

            // Calculate the nitrification delay
            lNitrificationDelay = CalculateNitrificationDelay(datCurApp);

            // For simplicity the nitrification delay is added to the date of application
            // rather than treated as a range.
            // Reset the current application date to allow for this delay.
            datCurApp = datCurApp.AddDays(lNitrificationDelay);

            // set the application month variable to the date of the current manure application
            iMonthApp = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(datCurApp);

            // set the variable for the date of end of drainage
            datEndDrain = MannerApplication.Weather.EndOfSoilDrainage;
            // 
            // 'Find the month of the end of soil drainage
            iMonthEOD = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(datEndDrain);

            // STEP 3 - Determine soil properties
            // ----------------------------------------------------------------------------------
            // Leaching calculation starts here   'check the date of application is less than the end of soil drainage
            if (datCurApp < datEndDrain)
            {

                // adjustment for moisture deficit and for incorporation, which differs from that for permeable soil because of the effects on bypass flow and surface runoff.

                // Get soil moisture deficit from MCDM.  Note: because MCDM calculates soil moisture deficit at the end of the month we use the SMD from
                // the previous month to the month of application.  Agreed following conversation with E. Lord on 26/7/2006.
                // As the count starts at zero then iMonthApp is OK to use - no need to subtract a month.

                dSMDCurMonth = objManData.GetClimateType(iMonthApp, ClimateObject, MannerLib.MannerData.ClimateType.SoilMoistureDefecit);

                if (iMonthApp == 1)
                {
                    // if month is january take december as previous month
                    dSMDPrevMonth = objManData.GetClimateType(12, ClimateObject, MannerLib.MannerData.ClimateType.SoilMoistureDefecit);
                }
                else
                {
                    dSMDPrevMonth = objManData.GetClimateType(iMonthApp - 1, ClimateObject, MannerLib.MannerData.ClimateType.SoilMoistureDefecit);
                }

                // Even out the SMD factor depending on how far along the month we are.
                double SMDpropstart;
                SMDpropstart = (double)datCurApp.Day / (double)DateTime.DaysInMonth((int)MannerApplication.Weather.DateOfApplication.Year, (int)MannerApplication.Weather.DateOfApplication.Month);

                dSMD = dSMDPrevMonth + SMDpropstart * (dSMDCurMonth - dSMDPrevMonth);

                // Calculate the effective HER from effective application date to end of drainage.

                // If there was a soil moisture deficit then this affects adjustment factor

                if (dSMD > 0d)
                {
                    dHER = MannerApplication.Weather.RainfallTotal - MannerApplication.Weather.EvapotranspirationTotal;
                    dHEREffective = dHER;
                }

                // STEP 4 - Apply appropriate leaching algorithm
                // ----------------------------------------------------------------------------------
                // The subsoil contains the word CLAY
                // use the drained clay soil leaching algorithm
                // ----------------------------------------------------------------------------------
                // if the result of the string search for the word clay is greater than 0 then we have a clay soil

                if (MannerApplication.FieldData.Subsoil == MannerLib.Enumerations.SoilType.Clay | MannerApplication.FieldData.Subsoil == MannerLib.Enumerations.SoilType.ClayLoam | MannerApplication.FieldData.Subsoil == MannerLib.Enumerations.SoilType.SandyClay | MannerApplication.FieldData.Subsoil == MannerLib.Enumerations.SoilType.SandyClayLoam | MannerApplication.FieldData.Subsoil == MannerLib.Enumerations.SoilType.SiltyClay | MannerApplication.FieldData.Subsoil == MannerLib.Enumerations.SoilType.SiltyClayLoam)
                {

                    double dLProp1;
                    double dLProp2;
                    double dLProp3;
                    double dLRatioMod;
                    var dLAdjust = default(double);
                    double dInc;      // adjustment for method of incorporation

                    // Check if manure was incorporated (any value other than 'not incorporated'.
                    // IncorpFlag = ……
                    switch (MannerApplication.Application.MethodOfIncorporationEnum)
                    {
                        // If the manure has been ploughed down
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                            {
                                dInc = 0.9d;
                                break;
                            }
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                            {
                                dInc = 0.4d;
                                break;
                            }
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                            {
                                dInc = 0.4d;
                                break;
                            }
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated: // "Not Incorporated"
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
                        dInc = dInc * 0.5d;
                    }
                    else
                    {
                        dInc = dInc;
                    }

                    // Calculate 'leaching ratio'
                    // check for divide by zero error
                    if (dVMTotal <= 0d)
                    {
                        dLRatio = 0d;
                    }
                    else
                    {
                        dLRatio = dHEREffective / dVMTotal;
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
                    dLProp = dLProp - dLAdjust;

                    CalculateLeachedNRet = dMinN4 * dLProp;
                }

                // ----------------------------------------------------------------------------------
                // Else the subsoil does not contain the word CLAY
                // use the MATRIX LEACHING ALGORITHM
                // ----------------------------------------------------------------------------------
                else
                {
                    // Calculate the EFFECTIVE water capacity through which nitrate has to move:
                    switch (MannerApplication.Application.MethodOfIncorporationEnum)
                    {
                        // If the manure has been cultivated or ploughed down within a month ie at all
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.MouldboardPlough: // "Mouldboard Plough"
                            {
                                dVMEffective = dVMTotal - 0.5d * dVMTop;
                                break;
                            }
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.TineCultivator: // "Tine Cultivator"
                            {
                                dVMEffective = dVMTotal - 0.25d * dVMTop;
                                break;
                            }
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.RotaryCultivator: // "Rotary Cultivator"
                            {
                                dVMEffective = dVMTotal - 0.25d * dVMTop;
                                break;
                            }
                        // If it hasn't been incorporated then nothing changes
                        case MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated: // "Not Incorporated"
                            {
                                dVMEffective = dVMTotal; // catch all, but shouldn't get here
                                break;
                            }

                        default:
                            {
                                dVMEffective = dVMTotal;
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
                    CalculateLeachedNRet = dMinN4 * dLProp;
                }
            }
            else
            {
                // This is a bit of a catch all situation and allows the function to return a
                // value.  I'm also making the assumption that if the application date is
                // after the end of soil drainage no leaching can occur.
                CalculateLeachedNRet = 0d;

            }


            TS.TraceEvent(TraceEventType.Information, 0, "Calculate leached N:  " + CalculateLeachedNRet);
            return CalculateLeachedNRet;
        }


        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
            return 0d;
        }

    }
    /// <summary>
    /// Calculates the nitrification delay in days depending on the month of application of the manure. 
    /// Information for the calculation of the nitrification delay is contained in the Nitrification Delay Technical Guide of June 2004 
    /// </summary>
    /// <param name="dateOfApplication"></param>
    /// <returns type="Integer"></returns>
    /// <remarks></remarks>
    private int CalculateNitrificationDelay(DateTime dateOfApplication)
    {
        // ********************************************************************************
        DateTime datDateofApplication;
        var lNoofDays = default(int);
        int iMonth;

        datDateofApplication = dateOfApplication;

        // get the month of application from the date of application
        iMonth = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(datDateofApplication);

        // based on the month of application find the number of days for the nitrification delay
        switch (iMonth)
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

        TS.TraceEvent(TraceEventType.Information, 0, "based on the month of application find the number of days for the nitrification delay:  " + lNoofDays);
        // Return the number of days for the nitrification delay
        return lNoofDays;

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
            var ObjManData = new MannerLib.MannerData();
            string cropUse = ObjManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Crops, "CROPID", "CROPUSE", (int)MannerApplication.FieldData.CropTypeEnum);

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
            TS.TraceEvent(TraceEventType.Information, 0, "Calculates the mineralised N available to the following crop.:  " + (dNMineralised4 + dNMineralised5));
            return dNMineralised4 + dNMineralised5;
        }

        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
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
        double CalculateLeachedNNextRet = default;

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

            // Reset the Leached N variables to zero
            CalculateLeachedNNextRet = 0d;
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
                dHER = dHER + GetHer(iMonthApp);
                iMonthApp = iMonthApp + 1;
            }

            // -------------------------------------------------------------------------------------------
            // and then calculate HER for the months up to the end of soil drainage
            // end of soil drainage will always be 31st March c.f. Mineralisation Technical Guide.
            // -------------------------------------------------------------------------------------------
            iMonthEOD = 3;

            var loopTo = iMonthEOD;
            for (k = 1; k <= loopTo; k++)
                dHER = dHER + GetHer(k);

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

            TS.TraceEvent(TraceEventType.Information, 0, "Calculates the leached N of the mineralised N susceptible to leaching for the following crop.:  " + dMinN4 * dLProp);

            return dMinN4 * dLProp;
        }

        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
            return 0d;
        }

    }

    private double GetHer(int month)
    {
        double iHer;
        var objManData = new MannerLib.MannerData();
        iHer = Convert.ToDouble(objManData.GetDataField(MannerApplication.RunType, MannerLib.MannerData.XmlLookups.Climate, "MonthNo", "HER", month));
        return iHer;
    }

    private void CalculateRainfall(DateTime ApplicationDate, DateTime EndSD)
    {
        // CalcRainfall called to update Total Rainfall and Total Evap updates MannerTotalRain and MannerTotalEvap public variables
        // Called when Application Date or End of Soil Drainage dates are changed Receives ApplicationDate and End of Soil Drainage Dates
        // Works off monthly rainfall and AE values in climate array.

        try
        {

            DateTime AppDate;
            var sumRain = default(double);
            var sumEvap = default(double);
            double propstart;
            double propend;
            double ApplicationDateAE, AppDateRain;
            double SoilDrainageAE, SoilDrainageRain;
            var objManData = new MannerLib.MannerData();

            AppDateRain = objManData.GetClimateType(ApplicationDate.Month, ClimateObject, MannerLib.MannerData.ClimateType.Rainfall);
            ApplicationDateAE = objManData.GetClimateType(ApplicationDate.Month, ClimateObject, MannerLib.MannerData.ClimateType.ActualEvapotranspiration);

            SoilDrainageRain = objManData.GetClimateType(EndSD.Month, ClimateObject, MannerLib.MannerData.ClimateType.Rainfall);
            SoilDrainageAE = objManData.GetClimateType(EndSD.Month, ClimateObject, MannerLib.MannerData.ClimateType.ActualEvapotranspiration);


            // DO NOT ADD ONE MONTH TO THE DATE OF APPLICATION TO MIMIC EXISTING CODE AND MANNER PAPER
            AppDate = ApplicationDate;

            // #### NOTE -    Any manure application AFTER 31/07/98 is associated with the next years End of Soil Drainage

            if (DateAndTime.DateDiff(DateInterval.DayOfYear, AppDate, EndSD) <= 0L)
            {
                // if date of Application is after End of Soil Drainage then return zero rainfall and zero evap.
                MannerApplication.Weather.RainfallTotal = 0d;
                MannerApplication.Weather.EvapotranspirationTotal = 0d;
            }
            else
            {
                // else calculate rainfall
                propstart = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetDayOfMonth(AppDate) / (double)DateAndTime.DateDiff("d", DateAndTime.DateSerial(System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(AppDate), System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(AppDate), 1), DateAndTime.DateSerial(System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateAndTime.DateAdd("m", 1d, AppDate)), System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(DateAndTime.DateAdd("m", 1d, AppDate)), 1));
                propend = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetDayOfMonth(EndSD) / (double)DateAndTime.DateDiff("d", DateAndTime.DateSerial(System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(EndSD), System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(EndSD), 1), DateAndTime.DateSerial(System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateAndTime.DateAdd("m", 1d, EndSD)), System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(DateAndTime.DateAdd("m", 1d, EndSD)), 1));

                // check to make sure that month for app date and end soil drainage is not the same. If it is only to diff at end of period
                if (DateAndTime.DateDiff("m", AppDate, EndSD) > 0L)
                {

                    if (ApplicationDateAE > AppDateRain)
                    {
                        sumRain = AppDateRain * (1.0d - propstart);
                        sumEvap = sumRain;
                    }
                    else
                    {
                        sumRain = AppDateRain * (1.0d - propstart);
                        sumEvap = ApplicationDateAE * (1.0d - propstart);
                    }

                    if (SoilDrainageAE > SoilDrainageRain)
                    {
                        sumRain = sumRain + SoilDrainageRain * propend;
                        sumEvap = sumEvap + SoilDrainageRain * propend;
                    }
                    else
                    {
                        sumRain = sumRain + SoilDrainageRain * propend;
                        sumEvap = sumEvap + SoilDrainageAE * propend;
                    }
                }

                else if (DateAndTime.DateDiff("m", AppDate, EndSD) == 0L)
                {

                    if (SoilDrainageAE > SoilDrainageRain)
                    {
                        sumRain = sumRain + SoilDrainageRain * (propend - propstart);
                        sumEvap = sumEvap + SoilDrainageRain * (propend - propstart);
                    }
                    else
                    {
                        sumRain = sumRain + SoilDrainageRain * (propend - propstart);
                        sumEvap = sumEvap + SoilDrainageAE * (propend - propstart);
                    }

                }

                while (DateAndTime.DateDiff("m", AppDate, EndSD) > 1L)
                {

                    AppDate = DateAndTime.DateAdd("m", 1d, AppDate);

                    AppDateRain = objManData.GetClimateType(AppDate.Month, ClimateObject, MannerLib.MannerData.ClimateType.Rainfall);
                    ApplicationDateAE = objManData.GetClimateType(AppDate.Month, ClimateObject, MannerLib.MannerData.ClimateType.ActualEvapotranspiration);


                    if (ApplicationDateAE > AppDateRain)
                    {
                        sumRain = sumRain + AppDateRain;
                        sumEvap = sumEvap + AppDateRain;
                    }
                    else
                    {
                        sumRain = sumRain + AppDateRain;
                        sumEvap = sumEvap + ApplicationDateAE;
                    }

                }

                // always round up
                MannerApplication.Weather.RainfallTotal = (double)(long)Math.Round(sumRain + 0.5d);

                if (MannerApplication.Weather.RainfallTotal < 0d)
                {
                    MannerApplication.Weather.RainfallTotal = 0d;
                }
                MannerApplication.Weather.EvapotranspirationTotal = (double)(long)Math.Round(sumEvap + 0.5d);

            }

            TS.TraceEvent(TraceEventType.Information, 0, "Calculated rainfall:  " + MannerApplication.Weather.RainfallTotal);
            TS.TraceEvent(TraceEventType.Information, 0, "Calculated Evapotranspiration:  " + MannerApplication.Weather.EvapotranspirationTotal);
        }


        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
        }

    }

    /// <summary>
    /// Calculates the economic value of the manure, accepts parameters for the price of n, p and K as pence per kg.
    /// </summary>
    /// <param name="currentNitrogenCost"></param>
    /// <param name="currentPhosphorusCost"></param>
    /// <param name="currentPotassiumCost"></param>
    /// <remarks></remarks>
    public void CalculatePotentialEconomicValue(double currentNitrogenCost, double currentPhosphorusCost, double currentPotassiumCost)
    {

        double dNValue;
        double dPValue;
        double dKValue;
        double MannerAppliedP, MannerAppliedK;


        if (MannerApplication.FieldData.CropTypeEnum == MannerLib.Enumerations.CropTypeEnum.Grass) // if the crop type selected is grass
        {
            dNValue = (MannerApplication.Outputs.ResultantNAvailable + MannerApplication.Outputs.ResultantNAvailableSecondCut) * currentNitrogenCost / 100d;
        }
        else // for all other crop types only need to deal with one crop available N figure
        {
            dNValue = MannerApplication.Outputs.ResultantNAvailable * currentNitrogenCost / 100d;
        }

        if (MannerApplication.Outputs.P2O5Total < 0d)
        {
            MannerAppliedP = 0d;
        }
        else
        {
            MannerAppliedP = MannerApplication.Outputs.P2O5Total;
        }

        if (MannerApplication.Outputs.K2OTotal < 0d)
        {
            MannerAppliedK = 0d;
        }
        else
        {
            MannerAppliedK = MannerApplication.Outputs.K2OTotal;
        }

        dPValue = MannerAppliedP * currentPhosphorusCost / 100d;
        dKValue = MannerAppliedK * currentPotassiumCost / 100d;

        MannerApplication.Outputs.PotentialEconomicValue = dNValue + dPValue + dKValue;

        TS.TraceEvent(TraceEventType.Information, 0, "Potential Economic value:  " + MannerApplication.Outputs.PotentialEconomicValue);

    }

    // Aug 2012  C Lam: added IsCalcRainfall parameter, to allow a user supplied value to be used
    public void CalculateClimate(bool HaveSuppliedOwnClimateData = false, bool IsCalcRainfall = true)
    {

        try
        {

            if (!string.IsNullOrEmpty(MannerApplication.Postcode))
            {
                ClimateObject.GetClimate(MannerApplication.RunType, MannerApplication.Postcode, MannerApplication.FieldData.CropTypeEnum, MannerApplication.FieldData.Topsoil, MannerApplication.FieldData.Subsoil, HaveSuppliedOwnClimateData);
            }
            else
            {
                ClimateObject.GetClimate(MannerApplication.RunType, MannerApplication.Easting, MannerApplication.Northing, MannerApplication.FieldData.CropTypeEnum, MannerApplication.FieldData.Topsoil, MannerApplication.FieldData.Subsoil, HaveSuppliedOwnClimateData);
            }

            if (IsCalcRainfall)
            {
                if ((int)MannerApplication.Weather.DateOfApplication.Day > 0 & (int)MannerApplication.Weather.EndOfSoilDrainage.Day > 0)
                {
                    this.CalculateRainfall(MannerApplication.Weather.DateOfApplication, MannerApplication.Weather.EndOfSoilDrainage);
                }
            }
        }

        catch (Exception ex)
        {
            TS.TraceEvent(TraceEventType.Error, 0, ex.Message);
            throw ex;
        }

    }

    public bool IsManureHighReadilyAvailableN()
    {
        switch (MannerApplication.ManureType.ManureID)
        {
            case (int)MannerLib.Enumerations.ManureTypes.PoultryManure:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurry:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.PigSlurry:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurryWeepingWallLiquid:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurryStrainerBoxLiquid:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.CattleSlurryMechanicallySeparatedLiquid:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.PigSlurrySeparatedLiquid:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.BiosolidsLiquidDigested:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.DigestateWholeCattleSlurryBased:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.DigestateWholeFoodBased:
                {
                    return true;
                }
            case (int)MannerLib.Enumerations.ManureTypes.DigestateWholePigSlurryBased:
                {
                    return true;
                }
        }
        return false;
    }

    public double GetActualEvapotranspiration(int month)
    {
        var actualEvapotranspiration = ClimateObject.ActualEvapotranspiration;
        switch (month)
        {
            case 1:
                {
                    return actualEvapotranspiration.January;
                }
            case 2:
                {
                    return actualEvapotranspiration.February;
                }
            case 3:
                {
                    return actualEvapotranspiration.March;
                }
            case 4:
                {
                    return actualEvapotranspiration.April;
                }
            case 5:
                {
                    return actualEvapotranspiration.May;
                }
            case 6:
                {
                    return actualEvapotranspiration.June;
                }
            case 7:
                {
                    return actualEvapotranspiration.July;
                }
            case 8:
                {
                    return actualEvapotranspiration.August;
                }
            case 9:
                {
                    return actualEvapotranspiration.September;
                }
            case 10:
                {
                    return actualEvapotranspiration.October;
                }
            case 11:
                {
                    return actualEvapotranspiration.November;
                }
            case 12:
                {
                    return actualEvapotranspiration.December;
                }
        }

        return default;
    }

    public double GetRainfall(int month)
    {
        var rain = ClimateObject.Rain;
        switch (month)
        {
            case 1:
                {
                    return rain.January;
                }
            case 2:
                {
                    return rain.February;
                }
            case 3:
                {
                    return rain.March;
                }
            case 4:
                {
                    return rain.April;
                }
            case 5:
                {
                    return rain.May;
                }
            case 6:
                {
                    return rain.June;
                }
            case 7:
                {
                    return rain.July;
                }
            case 8:
                {
                    return rain.August;
                }
            case 9:
                {
                    return rain.September;
                }
            case 10:
                {
                    return rain.October;
                }
            case 11:
                {
                    return rain.November;
                }
            case 12:
                {
                    return rain.December;
                }
        }

        return default;
    }

    // Nov 2012 - C Lam
    public void SetMannerLocation(int countrycode, string Postcode)
    {

        MannerApplication.Postcode = Postcode;
        MannerApplication.CountryCode = countrycode;
        CalculateClimate();

    }

    public void SetMannerLocation(int countrycode, int easting, int northing)
    {

        MannerApplication.Easting = easting;
        MannerApplication.Northing = northing;
        MannerApplication.CountryCode = countrycode;
        CalculateClimate();

    }

    private int GetCropUptakeFactor(int month)
    {

        int CropuptakeFactor;
        var ManData = new MannerLib.MannerData();

        if (month >= 8 & month <= 10)
        {

            if (RunType == (int)MannerLib.MannerData.RunAs.PlanetEngland | RunType == (int)MannerLib.MannerData.RunAs.PlanetScotland)
            {
                CropuptakeFactor = ManData.GetCropUptakeFactorDefault(MannerApplication.FieldData.CropTypeEnum);
            }
            else
            {
                CropuptakeFactor = MannerApplication.FieldData.CropNUptake;
            }
        }

        else
        {
            CropuptakeFactor = 0;
        }

        TS.TraceEvent(TraceEventType.Information, 0, "Crop update factor :" + CropuptakeFactor);
        return CropuptakeFactor;

    }

    public void WriteToXMLFile(string path)
    {
        //TODO: Need to revisit this line of code.
       // MannerApplication..ToXmlFile(path);
    }

    private void OTemp_NutrientValueChanged()
    {

        // 18 Jan 2013 - no sure what i was trying to do here... but I don't think it's right.
        // SetTotalN(MannerApplication.ManureType.TotalN.Value_) ' C Lam Sep 2012 - added (mimic when dry matter changes)

        if (ValidateMannerSetCorrectly())
        {
            if (AutoCalculate)
            {
                CalculateManner();
            }
        }

    }

    private void ODMTemp_DryMatterOutofRange(string srange)
    {
        throw new Exception("Value is out of range: " + srange);
    }

    private void OTemp_DryMatterOutofRange(string srange)
    {
        throw new Exception("Value is out of range: " + srange);
    }

    private void ODMTemp_DryMatterValueChanged()
    {

        this.SetDryMatter(MannerApplication.ManureType.DryMatter.Value);

        if (ValidateMannerSetCorrectly())
        {
            if (AutoCalculate)
            {
                CalculateManner();
            }
        }
    }

    public bool ValidateMannerSetCorrectly()
    {
        bool canproc = true;

        if (!IsMannerLoading)
        {

            if ((int)MannerApplication.Application.ApplicationMethodEnum == default(int))
            {
                canproc = false;
                throw new Exception("Application method not set");
            }

            if ((int)MannerApplication.Application.MethodOfIncorporationEnum == default(int))
            {
                canproc = false;
                throw new Exception("Method of incorporation not set");
            }

            if ((int)MannerApplication.Application.DelayToIncorporationEnum == default(int))
            {
                canproc = false;
                throw new Exception("Delay to incorporation not set");
            }

            if (MannerApplication.FieldData.CropTypeEnum == default(int))
            {
                canproc = false;
                throw new Exception("Crop not set");
            }

            if (MannerApplication.FieldData.Topsoil == default(int))
            {
                canproc = false;
                throw new Exception("Top soil not set");
            }

            if (MannerApplication.FieldData.Subsoil == default(int))
            {
                canproc = false;
                throw new Exception("Sub soil not set");
            }

            if ((int)MannerApplication.FieldData.TopsoilMoistureEnum == default(int))
            {
                canproc = false;
                throw new Exception("Top soil moisture not set");
            }

        }

        return canproc;

    }

    private void SetApplicationMethodDefaults()
    {

        switch (MannerApplication.ManureType.ManureCategory)
        {

            case MannerLib.Enumerations.ManureCategory.CattleSlurry:
                {
                    ApplicationMethodId = 2;
                    MethodOfIncorporationId = 3;
                    DelayToIncorporationId = 2;
                    break;
                }

            case MannerLib.Enumerations.ManureCategory.FYM:
                {
                    ApplicationMethodId = 1;
                    MethodOfIncorporationId = 3;
                    DelayToIncorporationId = 2;
                    break;
                }

            case MannerLib.Enumerations.ManureCategory.LiquidSludge:
                {
                    ApplicationMethodId = 1;
                    MethodOfIncorporationId = 3;
                    DelayToIncorporationId = 2;
                    break;
                }

            case MannerLib.Enumerations.ManureCategory.PigSlurry:
                {
                    ApplicationMethodId = 2;
                    MethodOfIncorporationId = 3;
                    DelayToIncorporationId = 2;
                    break;
                }


            case MannerLib.Enumerations.ManureCategory.Poultry:
                {
                    ApplicationMethodId = 1;
                    MethodOfIncorporationId = 3;
                    DelayToIncorporationId = 2;
                    break;
                }

            case MannerLib.Enumerations.ManureCategory.SolidSludge:
                {
                    ApplicationMethodId = 1;
                    MethodOfIncorporationId = 3;
                    DelayToIncorporationId = 2;
                    break;
                }

        }

    }

    private void SetMannerCropDefaults()
    {

        MannerApplication.FieldData.CropTypeEnum = MannerLib.Enumerations.CropTypeEnum.Grass;
        MannerApplication.FieldData.Topsoil = MannerLib.Enumerations.SoilType.SandyClayLoam;
        MannerApplication.FieldData.Subsoil = MannerLib.Enumerations.SoilType.ClayLoam;
        MannerApplication.FieldData.TopsoilMoistureEnum = MannerLib.Enumerations.TopsoilMoistureEnum.Moist;

    }

    /// <summary>
    /// new dates for application date.  Now set to last available 1st September instead of 1st November.
    /// end of soil drainage set to end of March rather than 01 April in the next year.
    /// </summary>
    /// <remarks></remarks>
    private void SetDateDefaults()
    {

        int lYear, lMonth, lPrevYear, lNextYear;
        lYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Now);
        lMonth = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(DateTime.Now);

        lPrevYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateAndTime.DateAdd("yyyy", -1, DateTime.Now));
        lNextYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateAndTime.DateAdd("yyyy", 1d, DateTime.Now));

        if (lMonth < 11)     // date of application was last year
        {

            MannerApplication.Weather.EndOfSoilDrainage= new DateTime((short)System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Now), (byte)3, (byte)31);
            MannerApplication.Weather.DateOfApplication= new DateTime((short)lPrevYear, (byte)9, (byte)1);
        }

        else                        // date of application was this year
        {

            MannerApplication.Weather.EndOfSoilDrainage = new DateTime((short)lNextYear, (byte)3, (byte)31);
            MannerApplication.Weather.DateOfApplication = new DateTime((short)System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Now), (byte)9, (byte)1);

        }

    }

    // added C Lam Sep 2012
    private bool IsBiosolidLiquidDigested(MannerLib.Enumerations.ManureTypes mantype)
    {
        return mantype == MannerLib.Enumerations.ManureTypes.BiosolidsLiquidDigested;
    }

    // added C Lam Sep 2012
    private bool IsPaperCrumble(MannerLib.Enumerations.ManureTypes mantype)
    {
        bool blnIsPaperCrumble = mantype == MannerLib.Enumerations.ManureTypes.PaperCrumble;
        blnIsPaperCrumble = blnIsPaperCrumble | mantype == MannerLib.Enumerations.ManureTypes.PaperCrumbleBiologicallyTreated;
        blnIsPaperCrumble = blnIsPaperCrumble | mantype == MannerLib.Enumerations.ManureTypes.PaperCrumbleChemicallyPhysicallyTreated;
        return blnIsPaperCrumble;
    }

    // Dec 2012 - Soil drainage code from Eish
    public double GetSoilMoistureDefecit(int month)
    {
        var SMD = ClimateObject.SoilMoistureDeficit;
        switch (month)
        {
            case 1:
                {
                    return SMD.January;
                }
            case 2:
                {
                    return SMD.February;
                }
            case 3:
                {
                    return SMD.March;
                }
            case 4:
                {
                    return SMD.April;
                }
            case 5:
                {
                    return SMD.May;
                }
            case 6:
                {
                    return SMD.June;
                }
            case 7:
                {
                    return SMD.July;
                }
            case 8:
                {
                    return SMD.August;
                }
            case 9:
                {
                    return SMD.September;
                }
            case 10:
                {
                    return SMD.October;
                }
            case 11:
                {
                    return SMD.November;
                }
            case 12:
                {
                    return SMD.December;
                }
        }

        return default;
    }

    // Dec 2012 - Soil drainage code from Eish
    public double GetSoilDrainage(int month)
    {
        var SD = ClimateObject.SoilDrainage;
        switch (month)
        {
            case 1:
                {
                    return SD.January;
                }
            case 2:
                {
                    return SD.February;
                }
            case 3:
                {
                    return SD.March;
                }
            case 4:
                {
                    return SD.April;
                }
            case 5:
                {
                    return SD.May;
                }
            case 6:
                {
                    return SD.June;
                }
            case 7:
                {
                    return SD.July;
                }
            case 8:
                {
                    return SD.August;
                }
            case 9:
                {
                    return SD.September;
                }
            case 10:
                {
                    return SD.October;
                }
            case 11:
                {
                    return SD.November;
                }
            case 12:
                {
                    return SD.December;
                }
        }

        return default;
    }

}
