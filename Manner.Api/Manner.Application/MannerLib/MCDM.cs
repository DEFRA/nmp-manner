using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;
public class MCDM
{
    // MCDM - Mean Climate Drainage Model
    // 
    // Dr Steven Anthony
    // Environment Modelling and GIS Group
    // ADAS Consulting Ltd
    // 
    // 22nd July, 2004
    // 
    // -------------------------------------------------------------------------------
    // The MCDM class is intended to calculate monthly values of potential
    // and actual evapotranspiration, soil moisture deficit and soil drainage
    // given as input monthly values of long-term mean climate data and a
    // choice of crop type and soil textures for a UK location defined by
    // easting, northing and altitude. The model also predicts mean soil
    // temperatures at a reference depth and observation hour using the
    // homogenous conductor model.
    // 
    // The method of calculating potential evapotranspiration is based on
    // the PENMAN-MONTEITH equation, using UK specific parameters taken from
    // the MORECS and IRRIGUIDE models. Actual evapotranspiration is calculated
    // using a modified version of the THOMAS monthly book-keeping methodology.
    // Modifications have been made to allow for efficient evaporation of water
    // intercepted by the crop canopy, prior to evapotranspiration from the
    // soil moisture store. Surface runoff is calculated as a function of
    // rainfall intensity, using a gamma function to describe the statistical
    // distribution of rainfall per rain day. The parameters of the gamma
    // distribution are determined from empirical analyses of UK weather time
    // series carried out by the author.
    // 
    // The climate data required by the model can be input via property
    // accessor functions, or by the specification of a climate file
    // containing the appropriate data in the form of an ADO recordset.
    // 
    // 
    // ---------------------------------------------------------------------
    // 
    // Relevent References:
    // 
    // Thomas, H. A. (1981) Improved methods for national water assessment.
    // Report No. WR15249270, U.S. Water Resourc. Counc., Washington D.C.

    // Alley, W. M. (1984) On the treatment of evapotranspiration, soil
    // moisture accounting and aquifer recharge in monthly water balance
    // models. Wat. Resourc. Res., 20(8), 1137-1149.
    // 
    // Hough, M. N. and Jones, R. J. A. (1997) The United Kingdom Meteorological
    // Office rainfall and evaporation calculation system: MORECS version 2.0 -
    // an overview. Hydrol. and Earth Sys. Sci., 1(2), 227-239.
    // 
    // Bailey, R. J. and Spackman, E. (1996) A model for estimating soil
    // moisture changes as an aid to irrigation scheduling and eCrop-water use
    // studies: I. Operational details and description. Soil Use and Management,
    // 12, 122-128.
    // 
    // Cowley, J. P. (1978) The distribution over Great Britain of global
    // solar irradiation on a horizontal surface. The Meteorological Magazine,
    // 1277(107), 357-372.
    // 
    // Gilman, K. (1977) Movement of heat in soils. Institute of Hydrology,
    // Report no. 44. Wallingford, pp. 44.
    // 
    // Monteith, J. L. (1981) Evaporation and surface temperature. Quarterly
    // Journal of the Royal Meteorological Society, 107, 451, 1-27.
    // 
    // Soil pedotransfer functions ...
    // 
    // Met. Office (1968) Averages of earth temperature at depths of 30 cm an
    // 122 cm for the United Kingdom, 1931-60. Bulletin No. 794, HMSO, London.
    // 
    // Met. Office (1976) Averages of bright sunshine for the United Kingdom,
    // 1941-70, Bulletin No. 884, HMSO, London.
    // 
    // Met. Office (1963) Averages of temperatures for Great Britain and
    // Northern Ireland. Bulletin No. 735, HMSO, London.
    // 
    // Bristow, K. L. (2002) Thermal conductivity. In Dane, J. H. and Topp, G. C.
    // (Editors) Methods of soil analysis, Part 4 - Physical methods. Soil
    // Science Society of America Book Series No. 5, Madison, Wisconsin, 1209-1226.
    // 
    // McIlveen, R. (1986) Basic meteorology - a physical outline. van Nostrand
    // Reinhold, Wokingham, 457 pp.
    // 
    // ---------------------------------------------------------------------


    // -- Constant declarations ------------------------------------------------------

    private const double PI = 3.1415926d;

    // -- Enumeration type declarations ----------------------------------------------

    public enum LandCover
    {
        BareSoil = 0,
        ManagedGrass = 1,
        WinterWheat = 2
    }

    public enum SoilLayer
    {
        Topsoil = 0,
        Subsoil = 1
    }

    public enum AvailableWater
    {
        TopsoilAWC = 0,
        SubsoilAWC = 1
    }

    public enum Texture
    {
        Sand = 0,
        LoamySand = 1,
        SandyLoam = 2,
        FineSandyLoam = 3,
        SandySiltLoam = 4,
        SiltLoam = 5,
        SiltyClayLoam = 6,
        SandyClayLoam = 7,
        ClayLoam = 8,
        SandyClay = 9,
        SiltyClay = 10,
        Clay = 11,
        Organic = 12,
        Peaty = 13,
        Peat = 14,
        Chalk = 15,
        RocknotChalk = 16
    }

    // -- Property variable declarations ------------------------------------------------------

    public double _latitude; // Latitude of location (deg)
    public double _altitude; // Altitude of location (m)

    private double _topSand; // Soil percentage sand content (%)
    private double _topClay; // Soil percentage clay content (%)
    private double _topSilt; // Soil percentage silt content (%)
    private double _topDensity; // Soil bulk density (g/cm3)
    private double _topCarbon; // Soil percentage carbon content (%)

    private double _subSand; // Soil percentage sand content (%)
    private double _subClay; // Soil percentage clay content (%)
    private double _subSilt; // Soil percentage silt content (%)
    private double _subDensity; // Soil bulk density (g/cm3)
    private double _subCarbon; // Soil percentage carbon content (%)

    private readonly double[] _windSpeed = new double[12]; // Monthly mean daily wind speed (m/s)
    private readonly double[] _sunHours = new double[12]; // Monthly mean number of sun hours per day (hrs)
    private readonly double[] _minTemp = new double[12]; // Monthly mean minimum daily air temperature (oC)
    private readonly double[] _maxTemp = new double[12]; // Monthly mean maximum daily air temperature (oC)
    private readonly double[] _rainDays = new double[12]; // Monthly mean number of rain days (n; 0-30)
    private readonly double[] _rainfall = new double[12]; // Monthly mean total rainfall (mm)
    private readonly double[] _meanTemp = new double[12]; // Monthly mean temperature (oC)

    private readonly Texture _texture;
    private LandCover _landCover; // Land cover type (enum)
    private double _diffusivity; // Soil heat diffusivity (m2 /s)
    private double _hour; // Hour of observation of soil temperatures (n; 0-24)
    private double _depth; // Depth of observation of soil temperatures (n)

    private double _topSoilAWC;
    private double _subSoilAWC;

    // -- Calculated variable declarations -------------------------------------------

    private bool _dirty; // Flag indicating user update of property (b)

    private readonly double[] _potentialEvapotranspiration = new double[12]; // Monthly mean potential evapotranspiration (mm)
    private readonly double[] _canopyEvaporation = new double[12]; // Monthly mean actual evaporation from plant canopy (mm)
    private readonly double[] _surfaceRunOff = new double[12]; // Monthly mean surface runoff (mm)
    private readonly double[] _soilDrainage = new double[12]; // Monthly mean soil drainage (mm)
    private readonly double[] _soilMoistureDeficit = new double[12]; // Monthly mean soil moisture deficit (mm)
    private readonly double[] _actualEvapotranspiration = new double[12]; // Monthly mean actual evapotranspiration (mm)
    private readonly double[] _soilTemperature = new double[12]; // Monthly mean soil temperature (oC)
    private readonly double[] _radiation = new double[12]; // Monthly mean daily net radiation (MJ/m2)
    private readonly double[] _delta = new double[12]; // Monthly mean leaf temperature correction (MJ/m2)

    private readonly double[] _her = new double[12]; //HER

    private readonly double[] _leafAreaIndex = new double[12]; // Monthly mean plant leaf area index (n)
    private readonly double[] _height = new double[12]; // Monthly mean plant canopy height (m)
    private readonly double[] _canopyResistance = new double[12]; // Monthly mean plant canopy resistance (s/m)

    private readonly double[] _diurnalAve = new double[12]; // Diurnal soil temperature mean (oC)
    private readonly double[] _diurnalAmpOne = new double[12]; // Diurnal soil temperature first harmonic amplitude (oC)
    private readonly double[] _diurnalPhaseOne = new double[12]; // Diurnal soil temperature first harmonic phase shift (rad)
    private readonly double[] _diurnalAmpTwo = new double[12]; // Diurnal soil temperature second harmonic amplitude (oC)
    private readonly double[] _diurnalPhaseTwo = new double[12]; // Diurnal soil temperature second phase shift (rad)
    private readonly double[] _diurnalMax = new double[12]; // Monthly maximum diurnal soil temperature (oC)
    private readonly double[] _diurnalMin = new double[12]; // Monthly minimum diurnal soil temperature (oC)

    private short _topSoilAWCHard;
    private short _topSoilAWCEasy;
    private short _subSoilAWCHard;
    private short _subSoilAWCEasy;

    private bool _euniceTexture;

    private readonly string _climateDataFile; // Input climate data file (s)

    // Added by RE on the 25/11/09
    private bool _dataSetforRainFall = false;
    private bool _dataSetforRainDays = false;
    private bool _dataSetforWindSpeed = false;
    private bool _dataSetforSunHours = false;
    private bool _dataSetforMaxTemp = false;
    private bool _dataSetforMinTemp = false;

    // -- Public property accessors --------------------------------------------------

    public double Diffusivity
    {
        set
        {

            if (value != -1 & value <= 0d)
            {
                throw new Exception("Invalid Diffusivity Value");
            }
            else
            {
                _diffusivity = value;
            }

        }
    }

    public double get_Sand(SoilLayer soilLayer)
    {
        double SandRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            SandRet = _topSand;
        }
        else
        {
            SandRet = _subSand;
        }

        return SandRet;
    }

    public double get_Clay(SoilLayer soilLayer)
    {
        double ClayRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            ClayRet = _topClay;
        }
        else
        {
            ClayRet = _subClay;
        }

        return ClayRet;
    }

    public double get_Density(SoilLayer soilLayer)
    {
        double DensityRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            DensityRet = _topDensity;
        }
        else
        {
            DensityRet = _subDensity;
        }

        return DensityRet;
    }

    public double get_Carbon(SoilLayer soilLayer)
    {
        double CarbonRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            CarbonRet = _topCarbon;
        }
        else
        {
            CarbonRet = _subCarbon;
        }

        return CarbonRet;
    }

    public double get_WindSpeed(int monthIndex = -1)
    {
        double WindSpeedRet = default;
        if (monthIndex == -1)
        {
            WindSpeedRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                WindSpeedRet = WindSpeedRet + _windSpeed[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            WindSpeedRet = _windSpeed[monthIndex];
        }

        return WindSpeedRet;

    }

    public void set_WindSpeed(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else if (value < 0d)
        {
            throw new Exception("Invalid Wind Speed Value");
        }
        else
        {
            _windSpeed[monthIndex] = value;
            _dirty = true;
            _dataSetforWindSpeed = true;
        }
    }

    public double get_MinTemp(int monthIndex = -1)
    {
        double MinTempRet = default;

        if (monthIndex == -1)
        {
            MinTempRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                MinTempRet = MinTempRet + _minTemp[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            MinTempRet = _minTemp[monthIndex];
        }

        return MinTempRet;

    }

    public void set_MinTemp(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else if (value < -100 | value > 100d)
        {
            throw new Exception("Invalid Minimum Temperature Value");
        }
        else
        {
            _minTemp[monthIndex] = value;
            _dirty = true;
            _dataSetforMinTemp = true;
        }
    }

    public double get_MaxTemp(int monthIndex = -1)
    {
        double MaxTempRet = default;

        if (monthIndex == -1)
        {
            MaxTempRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                MaxTempRet = MaxTempRet + _maxTemp[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            MaxTempRet = _maxTemp[monthIndex];
        }

        return MaxTempRet;

    }

    public void set_MaxTemp(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else if (value < -100 | value > 100d)
        {
            throw new Exception("Invalid Maximum Temperature Value");
        }
        else
        {
            _maxTemp[monthIndex] = value;
            _dirty = true;
            _dataSetforMaxTemp = true;
        }
    }


    public double get_MeanTemp(int monthIndex = -1)
    {
        double MeanTempRet = default;

        if (monthIndex == -1)
        {
            MeanTempRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                MeanTempRet = MeanTempRet + _meanTemp[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            MeanTempRet = _meanTemp[monthIndex];
        }

        return MeanTempRet;

    }

    public void set_MeanTemp(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else if (value < -100 | value > 100d)
        {
            throw new Exception("Invalid Mean Temperature Value");
        }
        else
        {
            _meanTemp[monthIndex] = value;
            _dirty = true;
        }
    }

    public bool EuniceTexture
    {
        set
        {
            _euniceTexture = value;
        }
    }

    public double get_SunHours(int monthIndex = -1)
    {
        double SunHoursRet = default;

        if (monthIndex == -1)
        {
            SunHoursRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                SunHoursRet = SunHoursRet + _sunHours[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            SunHoursRet = _sunHours[monthIndex];
        }

        return SunHoursRet;

    }

    public void set_SunHours(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else if (value < 0d | value > 24d)
        {
            throw new Exception("Invalid Sun Hours Value");
        }
        else
        {
            _sunHours[monthIndex] = value;
            _dirty = true;
            _dataSetforSunHours = true;
        }
    }


    public double get_RainDays(int monthIndex = -1)
    {
        double RainDaysRet = default;

        if (monthIndex == -1)
        {
            RainDaysRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                RainDaysRet = RainDaysRet + _rainDays[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            RainDaysRet = _rainDays[monthIndex];
        }

        return RainDaysRet;

    }

    public void set_RainDays(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else if (value < 0d | value > 30d)
        {
            throw new Exception("Invalid Rain Days Value");
        }
        else
        {
            _rainDays[monthIndex] = value;
            _dirty = true;
            _dataSetforRainDays = true;
        }
    }

    public double get_Rainfall(int monthIndex = -1)
    {
        double RainfallRet = default;

        if (monthIndex == -1)
        {
            RainfallRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                RainfallRet = RainfallRet + _rainfall[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            RainfallRet = _rainfall[monthIndex];
        }

        return RainfallRet;

    }

    public void set_Rainfall(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month value");
        }
        else if (value < 0d | value > 1000d)
        {
            throw new Exception("Invalid Rainfall Value");
        }
        else
        {
            _rainfall[monthIndex] = value;
            _dirty = true;
            _dataSetforRainFall = true;
        }
    }

    public LandCover LandCoverType
    {
        get
        {
            LandCover LandCoverTypeRet = default;
            LandCoverTypeRet = _landCover;
            return LandCoverTypeRet;
        }
    }

    public Texture PercentageAWC
    {
        get
        {
            Texture PercentageAWCRet = default;
            PercentageAWCRet = _texture;
            return PercentageAWCRet;
        }
    }

    public double ObservationSoilTemperatureDepth
    {
        get
        {
            double ObservationSoilTemperatureDepthRet = default;
            ObservationSoilTemperatureDepthRet = _depth;
            return ObservationSoilTemperatureDepthRet;
        }
    }

    public double ObservationSoilTemperatureHour
    {
        get
        {
            double ObservationSoilTemperatureHourRet = default;
            ObservationSoilTemperatureHourRet = _hour;
            return ObservationSoilTemperatureHourRet;
        }
    }

    public double Altitude
    {
        get
        {
            double AltitudeRet = default;
            AltitudeRet = _altitude;
            return AltitudeRet;
        }
    }

    public double Latitude
    {
        get
        {
            double LatitudeRet = default;
            LatitudeRet = _latitude;
            return LatitudeRet;
        }
    }

    // -- Public calculated property accessors -------------------------------------------------

    public double get_CanopyEvaporation(int monthIndex = -1)
    {
        double CanopyEvaporationRet = default;

        if (monthIndex == -1)
        {
            CanopyEvaporationRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                CanopyEvaporationRet = CanopyEvaporationRet + _canopyEvaporation[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            if (_dirty)
                Calculate();
            CanopyEvaporationRet = _canopyEvaporation[monthIndex];
        }

        return CanopyEvaporationRet;

    }

    public double get_PotentialEvapotranspiration(int monthIndex = -1)
    {
        double PotentialEvapotranspirationRet = default;

        if (monthIndex == -1)
        {
            PotentialEvapotranspirationRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                PotentialEvapotranspirationRet = PotentialEvapotranspirationRet + _potentialEvapotranspiration[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            if (_dirty)
                Calculate();
            PotentialEvapotranspirationRet = _potentialEvapotranspiration[monthIndex];
        }

        return PotentialEvapotranspirationRet;

    }

    public double get_ActualEvapotranspiration(int monthIndex = -1)
    {
        double ActualEvapotranspirationRet = default;

        if (monthIndex == -1)
        {
            ActualEvapotranspirationRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                ActualEvapotranspirationRet = ActualEvapotranspirationRet + _actualEvapotranspiration[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            if (_dirty)
                Calculate();
            ActualEvapotranspirationRet = _actualEvapotranspiration[monthIndex];
        }

        return ActualEvapotranspirationRet;

    }

    public double get_SurfaceRunOff(int monthIndex = -1)
    {
        double SurfaceRunOffRet = default;

        if (monthIndex == -1)
        {
            SurfaceRunOffRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                SurfaceRunOffRet = SurfaceRunOffRet + _surfaceRunOff[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            if (_dirty)
                Calculate();
            SurfaceRunOffRet = _surfaceRunOff[monthIndex];
        }

        return SurfaceRunOffRet;

    }

    public double get_SoilDrainage(int monthIndex = -1)
    {
        double SoilDrainageRet = default;

        if (monthIndex == -1)
        {
            SoilDrainageRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                SoilDrainageRet = SoilDrainageRet + _soilDrainage[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            if (_dirty)
                Calculate();
            SoilDrainageRet = _soilDrainage[monthIndex];
        }

        return SoilDrainageRet;

    }

    public double get_SoilMoistureDeficit(int monthIndex = -1)
    {
        double SoilMoistureDeficitRet = default;

        if (monthIndex == -1)
        {
            SoilMoistureDeficitRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                SoilMoistureDeficitRet = SoilMoistureDeficitRet + _soilMoistureDeficit[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            if (_dirty)
                Calculate();
            SoilMoistureDeficitRet = _soilMoistureDeficit[monthIndex];
        }

        return SoilMoistureDeficitRet;

    }

    public double get_SoilTemperature(int monthIndex = -1)
    {
        double SoilTemperatureRet = default;

        if (monthIndex == -1)
        {
            SoilTemperatureRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                SoilTemperatureRet = SoilTemperatureRet + _soilTemperature[monthIndex] / 12d;
        }
        else if (monthIndex < 0 | monthIndex > 11)
        {
            throw new Exception("Invalid Month Index");
        }
        else
        {
            if (_dirty)
                Calculate();
            SoilTemperatureRet = _soilTemperature[monthIndex];
        }

        return SoilTemperatureRet;

    }

    // -- Public property subroutines --------------------------------------------------

    public void SetLandCover(LandCover LandCover)
    {

        if ((int)LandCover < 0 | (int)LandCover > 2)
        {
            throw new Exception("Invalid Land Cover");
        }
        else
        {
            CalculateLandcover(LandCover);
            _dirty = true;
        }

    }

    public void SetLocation(double latitude, double altitude = -1)
    {

        if (latitude < 0d | latitude > 90d)
        {
            throw new Exception("Invalid Latitude Value");
        }
        else if (altitude != -1 & altitude < 0d)
        {
            throw new Exception("Invalid Altitude Value");
        }
        else
        {

            _latitude = latitude;

            if (altitude == -1)
            {
                _altitude = 0d;
            }
            else
            {
                _altitude = altitude;
            }

            _dirty = true;

        }

    }

    public void SetObservation(ref double depth, int hour = -1)
    {

        if (depth < 0d)
        {
            throw new Exception("Invalid Observation Depth");
        }
        else if (hour != -1 & (hour < 0 | hour > 24))
        {
            throw new Exception("Invalid Observation Hour");
        }
        else
        {

            _depth = depth;
            _hour = hour;

            _dirty = true;

        }

    }

    public void SetAWC(double TopSoilAWC, double SubSoilAWC)
    {

        _topSoilAWC = TopSoilAWC;
        _subSoilAWC = SubSoilAWC;

    }

    public void SetPercentTopsoilAWC(Texture Soil, short TopSoilAWCHard, short TopSoilAWCEasy)
    {

        _topSoilAWCHard = TopSoilAWCHard;
        _topSoilAWCEasy = TopSoilAWCEasy;

    }

    public void SetPercentSubsoilAWC(Texture Soil, short SubSoilAWCHard, short SubSoilAWCEasy)
    {

        _subSoilAWCHard = SubSoilAWCHard;
        _subSoilAWCEasy = SubSoilAWCEasy;

    }


    public void SetSoil(SoilLayer soilLayer, double sand, double silt, double clay, double Density = -1, double Carbon = -1)
    {

        if (sand + silt + clay != 100d)
        {
            throw new Exception("Invalid Particle Size Distribution");
        }
        else if (sand < 1d | sand > 100d | silt < 1d | silt > 100d | clay < 1d | clay > 100d)
        {
            throw new Exception("Invalid Particle Size Distribution");
        }
        else if (Carbon != -1 & (Carbon < 1d | Carbon > 100d))
        {
            throw new Exception("Invalid Carbon Content");
        }
        else if (Density != -1 & (Density < 1d | Density > 2d))
        {
            throw new Exception("Invalid Bulk Density");
        }
        else
        {

            if (soilLayer == SoilLayer.Topsoil)
            {
                _topSand = sand;
                _topSilt = silt;
                _topClay = clay;
            }
            else
            {
                _subSand = sand;
                _subSilt = silt;
                _subClay = clay;
            }

            if (Carbon != -1)
            {
                if (soilLayer == SoilLayer.Topsoil)
                {
                    _topCarbon = Carbon;
                }
                else
                {
                    _subCarbon = Carbon;
                }
            }
            else if (soilLayer == SoilLayer.Topsoil)
            {
                _topCarbon = 1d;
            }
            else
            {
                _subCarbon = 1d;
            }

            if (Density != -1)
            {
                if (soilLayer == SoilLayer.Topsoil)
                {
                    _topDensity = Density;
                }
                else
                {
                    _subDensity = Density;
                }
            }
            else if (soilLayer == SoilLayer.Topsoil)
            {
                _topDensity = 1.2d;
            }
            else
            {
                _subDensity = 1.2d;
            }

            _dirty = true;

        }

    }

    // -- Public function declarations --------------------------------------------------------
    public bool RetrieveClimate(DataSet ClimateDataset, string Postcode)
    {
        bool RetrieveClimateRet = default;
        // This function retrieves climate data from the given ClimateFile
        // for the location specified by  easting and northing
        // parameter values. If the climate file does not exist, or is corrupt, or the map location does not exist in the file, then the function returns false.

        bool bNeedToRetriveData = true;

        RetrieveClimateRet = false;

        try
        {

            if (_dataSetforMaxTemp & _dataSetforMinTemp & _dataSetforRainDays & _dataSetforRainFall & _dataSetforSunHours & _dataSetforWindSpeed)
            {
                bNeedToRetriveData = false;
            }

            if (bNeedToRetriveData)
            {
                // Find map location index:
                PopulateClimateObj(ClimateDataset, Postcode);
            }
            return true;
        }

        catch (Exception ex)
        {

        }

        return RetrieveClimateRet;

    }

    public bool RetrieveClimate(string ClimateXMLFileName, int Easting, int Northing)
    {
        bool RetrieveClimateRet = default;
        // This function retrieves climate data from the given Climate Dataset
        // for the location specified by  easting and northing parameter values. If the climate file does not exist, or is corrupt, or the map location does not exist in the file, then the function returns false.

        int ID;
        var ClimateDataset = new DataSet();
        bool bNeedToRetriveData = true;

        RetrieveClimateRet = false;

        try
        {


            if (_dataSetforMaxTemp & _dataSetforMinTemp & _dataSetforRainDays & _dataSetforRainFall & _dataSetforSunHours & _dataSetforWindSpeed)
            {
                bNeedToRetriveData = false;
            }

            if (bNeedToRetriveData)
            {
                // Find map location index:
                ID = (int)Math.Round(Conversion.Int(Easting / 10000d) * 1000d + Conversion.Int(Northing / 10000d));
                ClimateDataset.ReadXml(ClimateXMLFileName);

                PopulateClimateObj(ClimateDataset, ID.ToString());
            }
            return true;
        }

        catch (Exception ex)
        {

        }

        return RetrieveClimateRet;

    }

    public bool RetrieveClimate(DataSet ClimateDataset, int Easting, int Northing)
    {
        bool RetrieveClimateRet = default;
        // This function retrieves climate data from the given ClimateFile
        // for the location specified by  easting and northing
        // parameter values. If the climate file does not exist, or is corrupt, or the map location does not exist in the file, then the function returns false.

        int ID;
        bool bNeedToRetriveData = true;

        RetrieveClimateRet = false;

        try
        {

            if (_dataSetforMaxTemp & _dataSetforMinTemp & _dataSetforRainDays & _dataSetforRainFall & _dataSetforSunHours & _dataSetforWindSpeed)
            {
                bNeedToRetriveData = false;
            }

            if (bNeedToRetriveData)
            {
                // Find map location index:
                ID = (int)Math.Round(Conversion.Int(Easting / 10000d) * 1000d + Conversion.Int(Northing / 10000d));
                PopulateClimateObj(ClimateDataset, ID.ToString());
            }
            return true;
        }

        catch (Exception ex)
        {

        }

        return RetrieveClimateRet;

    }

    private void PopulateClimateObj(DataSet DS, string Postcode)
    {

        try
        {

            var DV = DS.Tables[0].DefaultView;
            DV.RowFilter = "POSTCODE = '" + Postcode + "'";
            // Retrieve climate data:

            // Site altitude (m)
            _altitude = Convert.ToDouble(DV[0]["ALTITUDE"]);

            // Mean daily windspeed (m/s):
            _windSpeed[0] = Convert.ToDouble(DV[0]["MeanWS_Jan"]);
            _windSpeed[1] = Convert.ToDouble(DV[0]["MeanWS_Feb"]);
            _windSpeed[2] = Convert.ToDouble(DV[0]["MeanWS_Mar"]);
            _windSpeed[3] = Convert.ToDouble(DV[0]["MeanWS_Apr"]);
            _windSpeed[4] = Convert.ToDouble(DV[0]["MeanWS_May"]);
            _windSpeed[5] = Convert.ToDouble(DV[0]["MeanWS_Jun"]);
            _windSpeed[6] = Convert.ToDouble(DV[0]["MeanWS_Jul"]);
            _windSpeed[7] = Convert.ToDouble(DV[0]["MeanWS_Aug"]);
            _windSpeed[8] = Convert.ToDouble(DV[0]["MeanWS_Sep"]);
            _windSpeed[9] = Convert.ToDouble(DV[0]["MeanWS_Oct"]);
            _windSpeed[10] = Convert.ToDouble(DV[0]["MeanWS_Nov"]);
            _windSpeed[11] = Convert.ToDouble(DV[0]["MeanWS_Dec"]);

            // Mean total rainfall (mm):
            _rainfall[0] = Convert.ToDouble(DV[0]["MeanPR_Jan"]);
            _rainfall[1] = Convert.ToDouble(DV[0]["MeanPR_Feb"]);
            _rainfall[2] = Convert.ToDouble(DV[0]["MeanPR_Mar"]);
            _rainfall[3] = Convert.ToDouble(DV[0]["MeanPR_Apr"]);
            _rainfall[4] = Convert.ToDouble(DV[0]["MeanPR_May"]);
            _rainfall[5] = Convert.ToDouble(DV[0]["MeanPR_Jun"]);
            _rainfall[6] = Convert.ToDouble(DV[0]["MeanPR_Jul"]);
            _rainfall[7] = Convert.ToDouble(DV[0]["MeanPR_Aug"]);
            _rainfall[8] = Convert.ToDouble(DV[0]["MeanPR_Sep"]);
            _rainfall[9] = Convert.ToDouble(DV[0]["MeanPR_Oct"]);
            _rainfall[10] = Convert.ToDouble(DV[0]["MeanPR_Nov"]);
            _rainfall[11] = Convert.ToDouble(DV[0]["MeanPR_Dec"]);

            // Mean number of wet days (n):
            _rainDays[0] = Min(Convert.ToDouble(DV[0]["MeanRD_Jan"]), 30d);
            _rainDays[1] = Min(Convert.ToDouble(DV[0]["MeanRD_Feb"]), 30d);
            _rainDays[2] = Min(Convert.ToDouble(DV[0]["MeanRD_Mar"]), 30d);
            _rainDays[3] = Min(Convert.ToDouble(DV[0]["MeanRD_Apr"]), 30d);
            _rainDays[4] = Min(Convert.ToDouble(DV[0]["MeanRD_May"]), 30d);
            _rainDays[5] = Min(Convert.ToDouble(DV[0]["MeanRD_Jun"]), 30d);
            _rainDays[6] = Min(Convert.ToDouble(DV[0]["MeanRD_Jul"]), 30d);
            _rainDays[7] = Min(Convert.ToDouble(DV[0]["MeanRD_Aug"]), 30d);
            _rainDays[8] = Min(Convert.ToDouble(DV[0]["MeanRD_Sep"]), 30d);
            _rainDays[9] = Min(Convert.ToDouble(DV[0]["MeanRD_Oct"]), 30d);
            _rainDays[10] = Min(Convert.ToDouble(DV[0]["MeanRD_Nov"]), 30d);
            _rainDays[11] = Min(Convert.ToDouble(DV[0]["MeanRD_Dec"]), 30d);

            // Mean sun hours (hrs):
            _sunHours[0] = Convert.ToDouble(DV[0]["MeanSH_Jan"]) / 30d;
            _sunHours[1] = Convert.ToDouble(DV[0]["MeanSH_Feb"]) / 30d;
            _sunHours[2] = Convert.ToDouble(DV[0]["MeanSH_Mar"]) / 30d;
            _sunHours[3] = Convert.ToDouble(DV[0]["MeanSH_Apr"]) / 30d;
            _sunHours[4] = Convert.ToDouble(DV[0]["MeanSH_May"]) / 30d;
            _sunHours[5] = Convert.ToDouble(DV[0]["MeanSH_Jun"]) / 30d;
            _sunHours[6] = Convert.ToDouble(DV[0]["MeanSH_Jul"]) / 30d;
            _sunHours[7] = Convert.ToDouble(DV[0]["MeanSH_Aug"]) / 30d;
            _sunHours[8] = Convert.ToDouble(DV[0]["MeanSH_Sep"]) / 30d;
            _sunHours[9] = Convert.ToDouble(DV[0]["MeanSH_Oct"]) / 30d;
            _sunHours[10] = Convert.ToDouble(DV[0]["MeanSH_Nov"]) / 30d;
            _sunHours[11] = Convert.ToDouble(DV[0]["MeanSH_Dec"]) / 30d;

            // Mean daily min temp (oC):
            _minTemp[0] = Convert.ToDouble(DV[0]["MeanMN_Jan"]);
            _minTemp[1] = Convert.ToDouble(DV[0]["MeanMN_Feb"]);
            _minTemp[2] = Convert.ToDouble(DV[0]["MeanMN_Mar"]);
            _minTemp[3] = Convert.ToDouble(DV[0]["MeanMN_Apr"]);
            _minTemp[4] = Convert.ToDouble(DV[0]["MeanMN_May"]);
            _minTemp[5] = Convert.ToDouble(DV[0]["MeanMN_Jun"]);
            _minTemp[6] = Convert.ToDouble(DV[0]["MeanMN_Jul"]);
            _minTemp[7] = Convert.ToDouble(DV[0]["MeanMN_Aug"]);
            _minTemp[8] = Convert.ToDouble(DV[0]["MeanMN_Sep"]);
            _minTemp[9] = Convert.ToDouble(DV[0]["MeanMN_Oct"]);
            _minTemp[10] = Convert.ToDouble(DV[0]["MeanMN_Nov"]);
            _minTemp[11] = Convert.ToDouble(DV[0]["MeanMN_Dec"]);

            // Mean daily max temp (oC):
            _maxTemp[0] = Convert.ToDouble(DV[0]["MeanMX_Jan"]);
            _maxTemp[1] = Convert.ToDouble(DV[0]["MeanMX_Feb"]);
            _maxTemp[2] = Convert.ToDouble(DV[0]["MeanMX_Mar"]);
            _maxTemp[3] = Convert.ToDouble(DV[0]["MeanMX_Apr"]);
            _maxTemp[4] = Convert.ToDouble(DV[0]["MeanMX_May"]);
            _maxTemp[5] = Convert.ToDouble(DV[0]["MeanMX_Jun"]);
            _maxTemp[6] = Convert.ToDouble(DV[0]["MeanMX_Jul"]);
            _maxTemp[7] = Convert.ToDouble(DV[0]["MeanMX_Aug"]);
            _maxTemp[8] = Convert.ToDouble(DV[0]["MeanMX_Sep"]);
            _maxTemp[9] = Convert.ToDouble(DV[0]["MeanMX_Oct"]);
            _maxTemp[10] = Convert.ToDouble(DV[0]["MeanMX_Nov"]);
            _maxTemp[11] = Convert.ToDouble(DV[0]["MeanMX_Dec"]);
        }
        catch (Exception ex)
        {
            // MsgBox(ex.Message)
            throw ex;
        }
    }

    public void Clear()
    {

        object obj;
        int iMonth;

        // Set default observation depth / hour:
        double argdepth = 1.22d;
        SetObservation(ref argdepth, 9);

        // Set default diffusivity:
        Diffusivity = -1;

        // Default location attributes:
        SetLocation(52.5d, 0d);

        // Default land cover:
        SetLandCover(LandCover.ManagedGrass);

        // Default soil attributes:
        SetSoil(SoilLayer.Topsoil, 20d, 20d, 60d, 1.2d, 1d);
        SetSoil(SoilLayer.Subsoil, 20d, 20d, 60d, 1.2d, 1d);

        // Default sun hours (hrs)
        var sunHours = new object[] { 53, 66, 106, 138, 189, 191, 183, 176, 141, 106, 66, 47 };
        for (iMonth = 0; iMonth <= 11; iMonth++)
        {
            double d = Convert.ToDouble((int)sunHours[iMonth]) / 30.0;
            set_SunHours(iMonth,d );
        }
           

        // Default wind speed (m/s)

        var windSpeed = new object[] { 5.1d, 5, 5.3d, 4.8d, 4.6d, 4.2d, 4.1d, 4.2d, 4.3d, 4.4d, 4.8d, 4.9d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            set_WindSpeed(iMonth, Convert.ToDouble((int)windSpeed[iMonth]));

        // Default min temp (oC)
        var minTemp = new object[] { 0.6d, 0.5d, 1.6d, 3.8d, 6.5d, 9.7d, 11.8d, 11.5d, 9.5d, 6.1d, 3.5d, 1.7d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            set_MinTemp(iMonth, Convert.ToDouble(minTemp[iMonth]));

        // Default max temp (oC)
        var maxTemp = new object[] { 6.3d, 7.2d, 10.5d, 13.7d, 17.2d, 20.5d, 22.2d, 22, 19.2d, 14.6d, 9.9d, 7.3d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            set_MaxTemp(iMonth, Convert.ToDouble(maxTemp[iMonth]));

        // Default rain days (n)

        var rainDays = new object[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            set_RainDays(iMonth, Convert.ToDouble(rainDays[iMonth]));

        // Default rainfall (mm)
        var rainfall = new object[] { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 };
        for (iMonth = 0; iMonth <= 11; iMonth++)

            set_Rainfall(iMonth, Convert.ToDouble(rainfall[iMonth]));

        // Set calculation edit status:
        _dirty = true;
        _dataSetforMaxTemp = false;
        _dataSetforMinTemp = false;
        _dataSetforRainDays = false;
        _dataSetforRainFall = false;
        _dataSetforSunHours = false;
        _dataSetforWindSpeed = false;

    }

    // -- Private calculation functions ------------------------------------------------------

    private void CalculateClimateFit(ref double[] Data, ref double Average, ref double Amplitude, ref double Phase)
    {

        // Fit the first fourier harmonic to monthly mean data
        // and return the coefficients.

        double Sum;
        double SumCos;
        double SumSin;
        int Index;

        Sum = 0d;
        for (Index = 0; Index <= 11; Index++)
            Sum = Sum + Data[Index];

        Average = Sum / 12d;

        SumCos = 0d;
        SumSin = 0d;

        for (Index = 0; Index <= 11; Index++)
        {
            SumCos = SumCos + (Data[Index] - Average) * Math.Cos(2d * PI * (Index + 0.5d) / 12d);
            SumSin = SumSin + (Data[Index] - Average) * Math.Sin(2d * PI * (Index + 0.5d) / 12d);
        }

        SumCos = SumCos * 2d / 12d;
        SumSin = SumSin * 2d / 12d;

        Phase = CalculateATan2(-SumSin, SumCos);

        Amplitude = Math.Sqrt(Math.Pow(SumCos, 2d) + Math.Pow(SumSin, 2d));

    }

    private double CalculateATan2(double Opp, double Adj)
    {
        double CalculateATan2Ret = default;

        double Angle;

        if (Math.Abs(Adj) < 0.0001d)
        {
            Angle = PI / 2d;
        }
        else
        {
            Angle = Math.Abs(Math.Atan(Opp / Adj));
        }

        if (Adj < 0d)
        {
            Angle = PI - Angle;
        }

        if (Opp < 0d)
        {
            Angle = -1 * Angle;
        }

        CalculateATan2Ret = Angle;
        return CalculateATan2Ret;

    }

    private void CalculateSoilTemperature()
    {

        // Calculate the monthly mean soil temperatures at the reference
        // depth and observation hour, by application of the homogenous
        // conductor model with a correction for screen temperature.

        int iMonth;
        var SurfaceTemperature = new double[12];
        var MeanSurface = default(double);
        var MeanAmplitude = default(double);
        var MeanPhase = default(double);
        var SeasonalAmplitude = default(double);
        var SeasonalPhase = default(double);

        for (iMonth = 0; iMonth <= 11; iMonth++)
        {

            // Monthly mean air temperature:
            SurfaceTemperature[iMonth] = 0.5d * (_minTemp[iMonth] + _maxTemp[iMonth]);

            // Correction for screen temperature:
            SurfaceTemperature[iMonth] = SurfaceTemperature[iMonth] - (2.45d * ((_canopyEvaporation[iMonth] + _actualEvapotranspiration[iMonth]) / 30d) - _radiation[iMonth]) / _delta[iMonth];

        }

        // Fit first fourier harmonic to monthly mean soil surface temperatures:
        CalculateClimateFit(ref SurfaceTemperature, ref MeanSurface, ref MeanAmplitude, ref MeanPhase);

        // Calculate seasonal depth corrections to phase and amplitude:
        CalculateSeasonalCorrection(_depth, ref SeasonalAmplitude, ref SeasonalPhase);

        // Calculate monthly mean soil temperatures at depth:
        for (iMonth = 0; iMonth <= 11; iMonth++)
        {

            _soilTemperature[iMonth] = MeanSurface + MeanAmplitude * SeasonalAmplitude * Math.Cos((iMonth + 0.5d) * 2d * PI / 12d + MeanPhase + SeasonalPhase);

            // Observation hour correction:
            if (_hour != -1)
            {
                _soilTemperature[iMonth] = _soilTemperature[iMonth] + CalculateDayTemp(_depth, iMonth, (int)Math.Round(_hour));
            }

        }

    }

    private double CalculateDayTemp(double Depth, int iMonth, int iHour)
    {
        double CalculateDayTempRet = default;

        // Calculate adjustment to monthly mean soil temperature to
        // represent diurnal variation of soil temperature.

        double DampingDepth12hr;
        double DampingDepth24hr;

        DampingDepth12hr = Math.Sqrt(12.0d * 3600.0d * CalculateDiffusivity() / PI);
        DampingDepth24hr = Math.Sqrt(24.0d * 3600.0d * CalculateDiffusivity() / PI);

        if (_landCover == LandCover.ManagedGrass)
        {
            Depth = Depth + 0.1d;
        }

        CalculateDayTempRet = _diurnalAve[iMonth] + Math.Exp(-1 * Depth / DampingDepth24hr) * Math.Cos(iHour * 2 * PI / 24d + _diurnalPhaseOne[iMonth] - Depth / DampingDepth24hr) * _diurnalAmpOne[iMonth] + Math.Exp(-1 * Depth / DampingDepth12hr) * Math.Cos(iHour * 2 * PI / 12d + _diurnalPhaseTwo[iMonth] - Depth / DampingDepth24hr) * _diurnalAmpTwo[iMonth];

        CalculateDayTempRet = (CalculateDayTempRet - _diurnalMin[iMonth]) / (_diurnalMax[iMonth] - _diurnalMin[iMonth]);

        if (CalculateDayTempRet < 0.5d)
        {
            CalculateDayTempRet = 0.5d - CalculateDayTempRet;
            CalculateDayTempRet = -1 * CalculateDayTempRet * (_maxTemp[iMonth] - _minTemp[iMonth]);
        }
        else
        {
            CalculateDayTempRet = (CalculateDayTempRet - 0.5d) * (_maxTemp[iMonth] - _minTemp[iMonth]);
        }

        return CalculateDayTempRet;

    }

    private void CalculateSeasonalCorrection(double Depth, ref double Amplitude, ref double Phase)
    {

        // Calculate seasonal damped amplitude and phase shift
        // of soil temperatures at observation depth.

        double SeasonalDampingDepth;
        double ProfileDiff;

        // Depth correction for ManagedGrass cover:
        if (_landCover == LandCover.ManagedGrass)
        {
            Depth = Depth + 0.1d;
        }

        ProfileDiff = CalculateDiffusivity();

        SeasonalDampingDepth = Math.Pow(ProfileDiff * 24.0d * 3600.0d * 365.0d / PI, 0.5d);

        Amplitude = Math.Exp(-1 * Depth / SeasonalDampingDepth);
        Phase = -Depth / SeasonalDampingDepth;

    }

    public double CalculateDiffusivity()
    {
        double CalculateDiffusivityRet = default;

        double Density;

        if (_depth > 0.3d)
        {
            Density = (0.3d * _topDensity + (_depth - 0.3d) * _subDensity) / _depth;
        }
        else
        {
            Density = _topDensity;
        }

        if (_diffusivity == -1)
        {
            CalculateDiffusivityRet = 0.000001d * CalculateThermalConductivity() / (Density * CalculateHeatCapacity());
        }
        else
        {
            CalculateDiffusivityRet = _diffusivity;
        }

        return CalculateDiffusivityRet;

    }

    private double CalculateThermalConductivity()
    {
        double CalculateThermalConductivityRet = default;

        // Calculate thermal conductivity of soil profile (J /  s m oC)
        // according to the method of Bristow (2002).

        double Sand;
        double Clay;
        double Silt;

        var Soil = default(double);
        var Air = default(double);
        var Water = default(double);

        double A;
        double B;
        double C;
        double D;
        double E;

        CalculateSoilAirWater(ref Soil, ref Air, ref Water);

        if (_depth > 0.3d)
        {
            Sand = 0.01d * (0.3d * _topSand + (_depth - 0.3d) * _subSand) / _depth;
            Silt = 0.01d * (0.3d * _topSilt + (_depth - 0.3d) * _subSilt) / _depth;
            Clay = 0.01d * (0.3d * _topClay + (_depth - 0.3d) * _subClay) / _depth;
        }
        else
        {
            Sand = _topSand * 0.01d;
            Silt = _topSilt * 0.01d;
            Clay = _topClay * 0.01d;
        }

        A = (0.57d + 1.73d * Sand + 0.93d * Silt) / (1d - 0.74d * Sand - 0.49d * Silt) - 2.8d * Soil * (1d - Soil);
        B = 2.8d * Soil;
        C = 1d + 2.6d / Math.Pow(Clay, 0.5d);
        D = 0.03d + 0.7d * Math.Pow(Soil, 2d);
        E = 4d;

        CalculateThermalConductivityRet = A + B * Water - (A - D) * Math.Exp(-Math.Pow(C * Water, E));
        return CalculateThermalConductivityRet;

    }

    private double CalculateHeatCapacity()
    {
        double CalculateHeatCapacityRet = default;

        // Calculate heat capacity of soil profile ( MJ / m3 oC), assuming
        // that the soil is at field capacity.

        var Air = default(double);
        var Soil = default(double);
        var Water = default(double);

        CalculateSoilAirWater(ref Soil, ref Air, ref Water);

        CalculateHeatCapacityRet = 1.92d * Soil + 1d * Air + Water * 4.18d;
        return CalculateHeatCapacityRet;

    }

    private void CalculateLandcover(LandCover LandCover)
    {

        // Calculate monthly mean values of plant height, canopy resistance
        // and height, according to the selected land cover type.

        object Var;
        int Index;

        _landCover = LandCover;

        switch (_landCover)
        {

            case LandCover.BareSoil:
                {
                    var zero = new object[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (Index = 0; Index <= 11; Index++)
                        _leafAreaIndex[Index] = Convert.ToDouble(zero[Index]);

                    var hundred = new object[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                    for (Index = 0; Index <= 11; Index++)
                        _canopyResistance[Index] = Convert.ToDouble(hundred[Index]);

                    var zeroPointZeroFive = new object[] { 0.05d, 0.05d, 0.05d, 0.05d, 0.05d, 0.05d, 0.05d, 0.05d, 0.05d, 0.05d, 0.05d, 0.05d };
                    for (Index = 0; Index <= 11; Index++)
                        _height[Index] = Convert.ToDouble(zeroPointZeroFive[Index]);
                    break;
                }

            case LandCover.ManagedGrass:
                {

                    var lfai = new object[] { 2, 2, 3, 4, 5, 5, 5, 5, 4, 3, 2.5d, 2 };
                    for (Index = 0; Index <= 11; Index++)
                        _leafAreaIndex[Index] = Convert.ToDouble(lfai[Index]);

                    var cr = new object[] { 70, 70, 70, 70, 70, 70, 70, 70, 70, 70, 70, 70 };
                    for (Index = 0; Index <= 11; Index++)
                        _canopyResistance[Index] = Convert.ToDouble(cr[Index]);

                    var height = new object[] { 0.15d, 0.15d, 0.15d, 0.15d, 0.15d, 0.15d, 0.15d, 0.15d, 0.15d, 0.15d, 0.15d, 0.15d };
                    for (Index = 0; Index <= 11; Index++)
                        _height[Index] = Convert.ToDouble(height[Index]);
                    break;
                }

            case LandCover.WinterWheat:
                {

                    var lfai = new object[] { 0.5d, 0.5d, 0.6d, 1.8d, 3.3d, 4.7d, 5, 3.2d, 0, 0.3d, 0.3d, 0.5d };
                    for (Index = 0; Index <= 11; Index++)
                        _leafAreaIndex[Index] = Convert.ToDouble(lfai[Index]);

                   var cr = new object[] { 40, 40, 40, 40, 40, 40, 125, 300, 100, 40, 40, 40 };
                    for (Index = 0; Index <= 11; Index++)
                        _canopyResistance[Index] = Convert.ToDouble(cr[Index]);

                    var height = new object[] { 0.08d, 0.08d, 0.08d, 0.14d, 0.35d, 0.7d, 0.8d, 0.5d, 0.05d, 0.08d, 0.08d, 0.08d };
                    for (Index = 0; Index <= 11; Index++)
                        _height[Index] = Convert.ToDouble(height[Index]);
                    break;
                }

        }

    }

    private void CalculatePotentialEvapotranspiration(int iMonth, ref double PET, ref double RATIO, ref double DELTA, ref double RADIATION)
    {

        // Calculate monthly mean potential evapotranspiration (mm/day) based
        // on Penman-Monteith formula for the currently selected land cover,
        // and the efficiency ratio of evaporation from the plant canopy.

        double RelativeDistance;
        double SolarDeclination;
        double SunSetHourAngle;
        double JulianDay;
        double Latitude;
        double DayLength;
        double Albedo;

        double ActualVapourPressure;
        double ExtraterrestrialRadiation;
        double ShortwaveRadiation;
        double LongwaveRadiation;

        double VapourSlope;
        double SaturationVapourPressure;
        double Correction;
        double AirDensity;
        double AirPressure;
        double AeroResistance;
        double SurfaceResistance;

        double AngstromA;
        double AngstromB;
        double G;

        // Calculate soil heat loss:
        if (iMonth == 0)
        {
            G = 0.07d * (_minTemp[1] + _maxTemp[1] - _minTemp[11] - _maxTemp[11]) * 0.5d;
        }
        else if (iMonth == 11)
        {
            G = 0.07d * (_minTemp[0] + _maxTemp[0] - _minTemp[10] - _maxTemp[10]) * 0.5d;
        }
        else
        {
            G = 0.07d * (_minTemp[iMonth + 1] + _maxTemp[iMonth + 1] - _minTemp[iMonth - 1] - _maxTemp[iMonth - 1]) * 0.5d;
        }

        // Calculate combined plant and soil albedo, taking account
        // of rainfall effect on soil albedo:
        if (_topSand > 50d)
        {
            Albedo = 0.1d;
        }
        else if (_topClay > 50d)
        {
            Albedo = 0.3d;
        }
        else
        {
            Albedo = 0.2d;
        }

        Albedo = _rainDays[iMonth] * Albedo * 0.5d / 30d + (30d - _rainDays[iMonth]) * Albedo / 30d;

        if (_leafAreaIndex[iMonth] > 4d)
        {
            Albedo = 0.25d;
        }
        else
        {
            Albedo = Albedo + 0.25d * (0.25d - Albedo) * _leafAreaIndex[iMonth];
        }

        // Calculate incident radiation budget:
        Latitude = _latitude * 2d * PI / 360d;
        JulianDay = 30.42d * (iMonth + 1) - 15.23d;

        SolarDeclination = 0.409d * Math.Sin(0.0172d * JulianDay - 1.39d);
        RelativeDistance = 1d + 0.033d * Math.Cos(0.0172d * JulianDay);

        SunSetHourAngle = -1 * Math.Tan(Latitude) * Math.Tan(SolarDeclination);
        SunSetHourAngle = Math.Atan(-SunSetHourAngle / Math.Sqrt(-SunSetHourAngle * SunSetHourAngle + 1.0d)) + 2.0d * Math.Atan(1.0d);
        DayLength = 7.64d * SunSetHourAngle;

        ExtraterrestrialRadiation = 37.6d * RelativeDistance * (SunSetHourAngle * Math.Sin(Latitude) * Math.Sin(SolarDeclination) + Math.Cos(Latitude) * Math.Cos(SolarDeclination) * Math.Sin(SunSetHourAngle));

        // Seasonal variation of angstrom coefficients:
        AngstromA = 0.235d;
        AngstromB = 0.5058d + 0.0278d * Math.Cos((JulianDay - 140.9d) * 2d * PI / 365d) + 0.0074d * Math.Cos(2d * (JulianDay - 84.7d) * 2d * PI / 365d);

        // North south geographical variation of angstrom coefficients:
        AngstromB = AngstromB - 0.05d + 0.1d * ((_latitude - 49.81d) / 0.00000913d) / 1000000d;

        ShortwaveRadiation = (1d - Albedo) * ExtraterrestrialRadiation * (AngstromA + AngstromB * _sunHours[iMonth] / DayLength);

        ActualVapourPressure = 0.611d * Math.Exp(17.27d * _minTemp[iMonth] / (_minTemp[iMonth] + 237.3d));

        LongwaveRadiation = -0.0000000049d * (0.2d + 0.8d * _sunHours[iMonth] / DayLength) * (0.34d - 0.14d * Math.Sqrt(ActualVapourPressure)) * (Math.Pow(273d + _maxTemp[iMonth], 4d) + Math.Pow(273d + _minTemp[iMonth], 4d)) * 0.5d;

        SaturationVapourPressure = Math.Exp(17.27d * _minTemp[iMonth] / (_minTemp[iMonth] + 237.3d)) + Math.Exp(17.27d * _maxTemp[iMonth] / (_maxTemp[iMonth] + 237.3d));
        SaturationVapourPressure = SaturationVapourPressure * 0.611d * 0.5d;

        VapourSlope = 4098.0d * SaturationVapourPressure / Math.Pow(0.5d * (_minTemp[iMonth] + _maxTemp[iMonth]) + 237.3d, 2d);

        Correction = 4.0d * 0.98d * 0.0000000049d * Math.Pow(273.1d + 0.5d * (_minTemp[iMonth] + _maxTemp[iMonth]), 3d);

        AirPressure = 101.3d * Math.Pow((293d - 0.0065d * 0d) / 293d, 5.26d);
        AirDensity = 1000.0d * AirPressure / (287.0d * 1.01d * (0.5d * (_minTemp[iMonth] + _maxTemp[iMonth]) + 273.0d));

        AeroResistance = 6.25d / _windSpeed[iMonth] * Math.Log(10.0d / (0.1d * _height[iMonth])) * Math.Log(6.0d / (0.1d * _height[iMonth]));

        // Calculate surface resistance as a function of crop canopy resistance,
        // leaf area and soil resistance on wet and dry days:
        SurfaceResistance = Math.Pow(0.7d, _leafAreaIndex[iMonth]) / (_rainDays[iMonth] * 100d / 30d + (30d - _rainDays[iMonth]) * 600d / 30d);
        SurfaceResistance = SurfaceResistance + (1d - Math.Pow(0.7d, _leafAreaIndex[iMonth])) / _canopyResistance[iMonth];
        SurfaceResistance = 1d / SurfaceResistance;

        // Screen temperature correction method for calculation of PET:
        PET = VapourSlope * (ShortwaveRadiation + LongwaveRadiation - G) + 86.4d * AirDensity * 1.013d * (SaturationVapourPressure - ActualVapourPressure) * (1d + Correction * AeroResistance / (86.4d * AirDensity * 1.013d)) / AeroResistance;
        PET = PET / (VapourSlope + 0.00163d * AirPressure / 2.45d * (1d + SurfaceResistance / AeroResistance) * (1d + Correction * AeroResistance / (86.4d * AirDensity * 1.013d)));
        PET = PET / 2.45d;

        // Efficiency of evaporation from plant canopy:
        RATIO = VapourSlope + 0.00163d * AirPressure / 2.45d * (1.0d + SurfaceResistance / AeroResistance);
        RATIO = RATIO / (VapourSlope + 0.00163d * AirPressure / 2.45d);

        // Surface temperature correction data:
        RADIATION = ShortwaveRadiation + LongwaveRadiation - G;
        DELTA = 86.4d * AirDensity * 1.013d / AeroResistance + Correction;

    }

    private bool CalculateGammaQuantile(double dProbability, double dBeta, double dAlpha, ref double dQuantile)
    {
        bool CalculateGammaQuantileRet = default;

        // Calculate the quantile corresponding to a probability
        // of non-exceedance for a two-parameter gamma function
        // using the Wilson-Hilferty (1931) transformation or
        // the modified transformation of Kirby (1972).

        // Default return:
        CalculateGammaQuantileRet = false;

        double tmp_dC;
        var tmp_dU = default(double);
        var tmp_dK = default(double);
        double tmp_dQ;
        double tmp_dA;
        double tmp_dB;
        double tmp_dG;
        double tmp_dH;

        try
        {


            tmp_dC = dAlpha / Math.Abs(dAlpha) * 2d / Math.Pow(dBeta, 0.5d);

            if (CalculateNormalVariate(dProbability, ref tmp_dU) == false)
                return CalculateGammaQuantileRet;

            if (tmp_dC < 1d)
            {

                tmp_dK = tmp_dC * (tmp_dU - tmp_dC / 6d) / 6d + 1d;
                tmp_dK = Math.Pow(tmp_dK, 3d);
                tmp_dK = tmp_dK - 1d;
                tmp_dK = tmp_dK * 2d / tmp_dC;
            }

            else if (tmp_dC < 9.75d)
            {

                tmp_dG = -0.00385205d + 1.00426d * tmp_dC + 0.00651207d * Math.Pow(tmp_dC, 2d) + -0.0149166d * Math.Pow(tmp_dC, 3d) + 0.00163945d * Math.Pow(tmp_dC, 4d) + -0.0000583804d * Math.Pow(tmp_dC, 5d);

                tmp_dA = 0.00199447d + 0.48489d * tmp_dC + 0.0230935d * Math.Pow(tmp_dC, 2d) + -0.0152435d * Math.Pow(tmp_dC, 3d) + 0.00160597d * Math.Pow(tmp_dC, 4d) + -0.000055869d * Math.Pow(tmp_dC, 5d);

                tmp_dB = 0.990562d + 0.0319647d * tmp_dC + -0.0274231d * Math.Pow(tmp_dC, 2d) + 0.00777405d * Math.Pow(tmp_dC, 3d) + -0.000571184d * Math.Pow(tmp_dC, 4d) + 0.0000142077d * Math.Pow(tmp_dC, 5d);

                tmp_dA = 1d / tmp_dA;

                tmp_dH = Math.Pow(tmp_dB - 2d / tmp_dC / tmp_dA, 1d / 3d);

                tmp_dK = 1d - Math.Pow(tmp_dG / 6d, 2d) + tmp_dU * (tmp_dG / 6d);

                if (tmp_dK < tmp_dH)
                {
                    tmp_dK = tmp_dH;
                }

                tmp_dK = Math.Pow(tmp_dK, 3d);
                tmp_dK = (tmp_dK - tmp_dB) * tmp_dA;
            }

            else
            {
                Information.Err().Raise(Constants.vbObjectError + 100, Description: "Algorithm Range Error");
            }

            tmp_dQ = dAlpha * dBeta + tmp_dK * Math.Pow(Math.Pow(dAlpha, 2d) * dBeta, 0.5d);

            dQuantile = tmp_dQ;

            // Return value:
            CalculateGammaQuantileRet = true;
        }

        catch (Exception ex)
        {
            throw new Exception("Error: " + Information.Err().Description);
        }

        return CalculateGammaQuantileRet;

    }

    private bool CalculateNormalVariate(double dProbability, ref double dVariate)
    {
        bool CalculateNormalVariateRet = default;

        // Calculate the standard normal variate corresponding to a
        // probability of non-exceedence, using the approximate
        // solution of Abramowitz and Stegun (1965).

        // Default return:
        CalculateNormalVariateRet = false;

        double tmp_dW;
        double tmp_dP;
        double tmp_dU;
        double tmp_dDen;
        double tmp_dNum;

        try
        {



            tmp_dP = 1d - dProbability;
            if (tmp_dP > 0.5d)
            {
                tmp_dP = 1d - tmp_dP;
            }

            tmp_dW = Math.Pow(-2.0d * Math.Log(tmp_dP), 0.5d);

            tmp_dDen = 1.0d + 1.432788d * tmp_dW + 0.189269d * Math.Pow(tmp_dW, 2d) + 0.001308d * Math.Pow(tmp_dW, 3d);

            tmp_dNum = 2.515517d + 0.802853d * tmp_dW + 0.010328d * Math.Pow(tmp_dW, 2d);

            tmp_dU = tmp_dW - tmp_dNum / tmp_dDen;

            if (1d - dProbability > 0.5d)
            {
                tmp_dU = -tmp_dU;
            }

            dVariate = tmp_dU;

            // Return Value:
            CalculateNormalVariateRet = true;
        }

        catch (Exception ex)
        {
            throw new Exception("Error: " + Information.Err().Description);
        }

        return CalculateNormalVariateRet;

    }


    private double CalculateRunOff(int iMonth, double Rainfall)
    {
        double CalculateRunOffRet = default;

        CalculateRunOffRet = 0d;
        return CalculateRunOffRet;

        // Awaiting appropriate runoff function. The code below illustrates
        // how the USDA SCS Curve Number method might be used.

        // ' Calculate the surface runoff threshold:
        // Dim CurveStore as double
        // CurveStore = 254 * ((100 / m_CurveNumber) - 1)
        // 
        // ' Compare threshold with quantile:
        // If (Rainfall > (0.2 * CurveStore)) Then
        // CalcRunOff = ((Rainfall - (0.2 * CurveStore)) ^ 2) / (Rainfall + 0.8 * CurveStore)
        // Else
        // CalcRunOff = 0
        // End If

    }

    private void CalculateSoilAirWater(ref double Soil, ref double Air, ref double Water)
    {

        // Calculate the proportional soil, air and water contents
        // of the soil at field capacity, above the depth of soil
        // temperature observation.

        double Phi05;

        if (_depth > 0.3d)
        {
            Soil = (0.3d * _topDensity + (_depth - 0.3d) * _subDensity) / _depth;
        }
        else
        {
            Soil = _topDensity;
        }

        Soil = Soil / 2.65d;
        Air = 1d - Soil;

        // Water content at field capacity:
        Phi05 = 47d + 0.25d * _topClay + 0.1d * _topSilt + 1.12d * _topCarbon - 16.52d * _topDensity;

        if (_depth > 0.3d)
        {

            Water = Phi05 * 0.3d;
            Phi05 = 37.2d + 0.35d * _subClay + 0.12d * _subSilt - 11.73d * _subDensity;
            Water = Water + (_depth - 0.3d) * Phi05;
            Water = Water / _depth;
        }

        else
        {

            Water = Phi05;

        }

        Water = Water * 0.01d;
        Water = Max(0d, Water);
        Water = Min(Air, Water);
        Air = Air - Water;

    }

    private void CalculateAvailableWater(ref double AM02, ref double AM15)
    {

        // Calculate the plant available water held in the soil profile
        // between field capacity and a tension of 2 bar (AM02) and
        // between the tensions of 2 and 15 bar (AM15), using a particle
        // size pedotransfer function and in proportion to the rooting depth
        // of the current landcover.

        var Phi05 = default(double);
        var Phi15 = default(double);
        var Phi02 = default(double);

        // Initialise available water contents:
        AM02 = 0d;
        AM15 = 0d;

        if (_euniceTexture == true)
        {
            AM02 = AM02 + _topSoilAWCEasy * 0.01d * 300d;
            AM15 = AM15 + (_topSoilAWCHard - _topSoilAWCEasy) * 0.01d * 300d;
        }
        else
        {
            // Top soil water content:
            Phi05 = 47d + 0.25d * _topClay + 0.1d * _topSilt + 1.12d * _topCarbon - 16.52d * _topDensity;
            Phi02 = 8.7d + 0.45d * _topClay + 0.11d * _topSilt + 1.03d * _topCarbon;
            Phi15 = 2.94d + 0.83d * _topClay - 0.0054d * Math.Pow(_topClay, 2d);

            Phi05 = Max(0d, Phi05);
            Phi05 = Min(100d, Phi05);

            Phi02 = Max(0d, Phi02);
            Phi02 = Min(Phi05, Phi02);

            Phi15 = Max(0d, Phi15);
            Phi15 = Min(Phi02, Phi15);

            // All land cover types can access all the water
            // held in the top soil (0-300mm):
            AM02 = AM02 + (Phi05 - Phi02) * 0.01d * 300d;
            AM15 = AM15 + (Phi02 - Phi15) * 0.01d * 300d;

            // Sub soil water content:
            Phi05 = 37.2d + 0.35d * _subClay + 0.12d * _subSilt - 11.73d * _subDensity;
            Phi02 = 7.57d + 0.48d * _subClay + 0.11d * _subSilt;
            Phi15 = 1.48d + 0.84d * _subClay - 0.0054d * Math.Pow(_subClay, 2d);

            Phi05 = Max(0d, Phi05);
            Phi05 = Min(100d, Phi05);

            Phi02 = Max(0d, Phi02);
            Phi02 = Min(Phi05, Phi02);

            Phi15 = Max(0d, Phi15);
            Phi15 = Min(Phi02, Phi15);

        }

        // Sub soil water content is dependent upon the depth
        // of rooting (300-?mm):
        if (_euniceTexture == true)
        {

            switch (_landCover)
            {

                case LandCover.BareSoil:
                    {
                        AM02 = AM02 + _subSoilAWCEasy * 0.01d * 0d;
                        AM15 = AM15 + (_subSoilAWCHard - _subSoilAWCEasy) * 0.01d * 0d;
                        break;
                    }

                case LandCover.ManagedGrass:
                    {
                        AM02 = AM02 + _subSoilAWCEasy * 0.01d * 700d;
                        AM15 = AM15 + (_subSoilAWCHard - _subSoilAWCEasy) * 0.01d * 400d;
                        break;
                    }

                case LandCover.WinterWheat:
                    {
                        AM02 = AM02 + _subSoilAWCEasy * 0.01d * 900d;
                        AM15 = AM15 + (_subSoilAWCHard - _subSoilAWCEasy) * 0.01d * 200d;
                        break;
                    }

            }
        }
        else
        {

            switch (_landCover)
            {

                case LandCover.BareSoil:
                    {
                        AM02 = AM02 + (Phi05 - Phi02) * 0.01d * 0d;
                        AM15 = AM15 + (Phi02 - Phi15) * 0.01d * 0d;
                        break;
                    }

                case LandCover.ManagedGrass:
                    {
                        AM02 = AM02 + (Phi05 - Phi02) * 0.01d * 700d;
                        AM15 = AM15 + (Phi02 - Phi15) * 0.01d * 400d;
                        break;
                    }

                case LandCover.WinterWheat:
                    {
                        AM02 = AM02 + (Phi05 - Phi02) * 0.01d * 900d;
                        AM15 = AM15 + (Phi02 - Phi15) * 0.01d * 200d;
                        break;
                    }

            }
        }
    }

    private double Max(double LHS, double RHS)
    {
        double MaxRet = default;
        if (LHS > RHS)
        {
            MaxRet = LHS;
        }
        else
        {
            MaxRet = RHS;
        }

        return MaxRet;
    }

    private double Min(double LHS, double RHS)
    {
        double MinRet = default;
        if (LHS < RHS)
        {
            MinRet = LHS;
        }
        else
        {
            MinRet = RHS;
        }

        return MinRet;
    }

    // -- Public calculation functions -------------------------------------------------------

    public void Calculate()
    {

        // Calculate monthly mean values of potential evapotranspiration and
        // efficiency of canopy evaporation, and radiation and delta modifiers
        // for calculate of surface temperature:

        int iMonth;
        var PotentialEvapotranspiration = new double[12];
        var CanopyEfficiency = new double[12];

        for (iMonth = 0; iMonth <= 11; iMonth++)
        {
            CalculatePotentialEvapotranspiration(iMonth, ref PotentialEvapotranspiration[iMonth], ref CanopyEfficiency[iMonth], ref _delta[iMonth], ref _radiation[iMonth]);
            _potentialEvapotranspiration[iMonth] = PotentialEvapotranspiration[iMonth] * 30d;
        }

        // Calculate net rainfall, surface runoff and effective potential evapotranspiration
        // after evaporation of intercepted water from the plant canopy, by integration
        // across the distribution of rainfall per rain day for each month:

        double Probability;
        double alpha;
        double beta;

        var Interception = new double[12];
        var Rainfall = new double[12];
        var SurfaceRunOff = new double[12];
        double Intercept;
        double RunOff;
        var Quantile = default(double);

        for (iMonth = 0; iMonth <= 11; iMonth++)
        {

            Interception[iMonth] = 0d;
            SurfaceRunOff[iMonth] = 0d;

            // Gamma rainfall distribution parameters:
            alpha = 1d + _rainfall[iMonth] / _rainDays[iMonth];
            beta = _rainfall[iMonth] / _rainDays[iMonth] / alpha;

            // Integration across 100 discrete steps:
            for (Probability = 0.005d; Probability <= 0.995d; Probability += 0.01d)
            {

                CalculateGammaQuantile(Probability, beta, alpha, ref Quantile);
                Quantile = Max(Quantile, 0.1d);

                // Calculate canopy interception and evaporation:
                Intercept = Min(Quantile, _leafAreaIndex[iMonth] * 0.2d);
                Intercept = Min(PotentialEvapotranspiration[iMonth] * CanopyEfficiency[iMonth], Intercept);

                // Intercept = 0

                Interception[iMonth] = Interception[iMonth] + Intercept;
                Quantile = Quantile - Intercept;

                // Calculate surface runoff:
                RunOff = CalculateRunOff(iMonth, Quantile);

                SurfaceRunOff[iMonth] = SurfaceRunOff[iMonth] + RunOff;

            }

            SurfaceRunOff[iMonth] = _rainDays[iMonth] * SurfaceRunOff[iMonth] / 100d;
            Interception[iMonth] = _rainDays[iMonth] * Interception[iMonth] / 100d;

            // Calculate effective rainfall and potential evapotranspiration:
            Rainfall[iMonth] = _rainfall[iMonth] - SurfaceRunOff[iMonth] - Interception[iMonth];
            PotentialEvapotranspiration[iMonth] = _potentialEvapotranspiration[iMonth] - Interception[iMonth] / CanopyEfficiency[iMonth];

            // Store results:
            _surfaceRunOff[iMonth] = SurfaceRunOff[iMonth];
            _canopyEvaporation[iMonth] = Interception[iMonth];

        }

        // Calculate actual evapotranspiration and soil drainage by the
        // monthly book-keeping methodology of Thomas:

        var AM02 = default(double);
        var AM15 = default(double);

        // Calculate plant available water:
        CalculateAvailableWater(ref AM02, ref AM15);

        // Apply Thomas book-keeping methodology:
        int Index;
        var ActualEvapotranspiration = new double[12];
        var SoilMoistureDeficit = new double[12];
        var SoilDrainage = new double[12];
        var Drainage = default(double);
        int Iteration;
        bool Flag;

        for (iMonth = 0; iMonth <= 11; iMonth++)
        {
            SoilMoistureDeficit[iMonth] = 0d;
            SoilDrainage[iMonth] = 0d;
        }

        // Iteration changed from 499 to 999 based on SGA's revised model
        // MMG 29 January 2007
        double tmp_W;
        double tmp_Y;
        double tmp_B;
        var tmp_HER = new double[12];
        double tmp_A;
        double tmp_Ratio;
        for (Iteration = 0; Iteration <= 999; Iteration++)
        {

            Flag = true;

            for (iMonth = 0; iMonth <= 11; iMonth++)
            {


                // Revised model calculations;
                // Debug.Print m_TopSoilAWC + m_SubSoilAWC
                tmp_B = AM02 + AM15 + 25d; // m_TopSoilAWC + m_SubSoilAWC
                tmp_A = 1d;

                Index = iMonth - 1;
                if (Index == -1)
                    Index = 11;

                tmp_W = Rainfall[iMonth] + SoilMoistureDeficit[Index] - SurfaceRunOff[iMonth];

                tmp_Y = (tmp_W + tmp_B) / (2d * tmp_A) - Math.Pow(Math.Pow((tmp_W + tmp_B) / (2d * tmp_A), 2d) - tmp_W * tmp_B / tmp_A, 0.5d);

                SoilMoistureDeficit[iMonth] = tmp_Y * Math.Exp(-1 * PotentialEvapotranspiration[iMonth] / tmp_B);

                SoilDrainage[iMonth] = tmp_W - tmp_Y;

                ActualEvapotranspiration[iMonth] = tmp_Y - SoilMoistureDeficit[iMonth];

                SoilMoistureDeficit[iMonth] = SoilMoistureDeficit[Index] - ActualEvapotranspiration[iMonth];
                SoilMoistureDeficit[iMonth] = SoilMoistureDeficit[iMonth] + Rainfall[iMonth] - SurfaceRunOff[iMonth];

                if (SoilMoistureDeficit[iMonth] > tmp_B)
                {
                    SoilDrainage[iMonth] = SoilMoistureDeficit[iMonth] - tmp_B;
                    SoilMoistureDeficit[iMonth] = tmp_B;
                }
                else
                {
                    SoilDrainage[iMonth] = 0d;
                }

                // If (ActiveWorkbook.ActiveSheet.Cells(15, 12) = True) Then

                tmp_Ratio = get_RainDays(iMonth) / 30d;

                SoilDrainage[iMonth] = tmp_Ratio * SoilDrainage[iMonth] + (1d - tmp_Ratio) * (tmp_W - tmp_Y);

                SoilMoistureDeficit[iMonth] = SoilMoistureDeficit[Index] + Rainfall[iMonth] - SurfaceRunOff[iMonth] - ActualEvapotranspiration[iMonth] - SoilDrainage[iMonth];

                if (SoilMoistureDeficit[iMonth] > tmp_B)
                {
                    SoilDrainage[iMonth] = SoilDrainage[iMonth] + (SoilMoistureDeficit[iMonth] - tmp_B);
                    SoilMoistureDeficit[iMonth] = tmp_B;
                }
                // End If

                SoilDrainage[iMonth] = SoilDrainage[iMonth] + SurfaceRunOff[iMonth];

                // Drainage = SoilDrainage(Month)
                // 
                // Index = Month - 1
                // If Index = -1 Then Index = 11
                // 
                // ' Calculate Wi ...
                // tmp_dTempOne = Rainfall(Month) + SoilMoistureDeficit(Index)
                // 
                // ' Calculate Yi ...
                // tmp_dTempTwo = (tmp_dTempOne + ((AM02 + AM15))) / 2 - _
                // '                            (((tmp_dTempOne + (AM02 + AM15)) * 0.5) ^ 2 _
                // '                            - tmp_dTempOne * (AM02 + AM15)) ^ 0.5
                // 
                // SoilMoistureDeficit(Month) = tmp_dTempTwo * Exp(-1# * PotentialEvapotranspiration(Month) _
                // '                                        / (AM02 + AM15))
                // 
                // ActualEvapotranspiration(Month) = tmp_dTempTwo - SoilMoistureDeficit(Month)
                // 
                // SoilDrainage(Month) = tmp_dTempOne - tmp_dTempTwo

                if (Math.Abs(SoilDrainage[iMonth] - Drainage) > 1d)
                {
                    Flag = false;
                }

            }

            // MMG Removed flag 29-01-07
            // Originally put in to speed up PSYCHIC
            // If Flag = True Then Exit For

        }

        // Calculate true SMD and zero negative soil drainage (small number problem),
        // and round output data. Note that soil moisture deficit is for end of month.

        for (iMonth = 0; iMonth <= 11; iMonth++)
        {

            if (SoilDrainage[iMonth] < 0d)
            {
                SoilDrainage[iMonth] = 0d;
            }

            SoilMoistureDeficit[iMonth] = AM02 + AM15 + 25d - SoilMoistureDeficit[iMonth];
            SoilMoistureDeficit[iMonth] = Math.Round(SoilMoistureDeficit[iMonth], 1);
            _soilMoistureDeficit[iMonth] = SoilMoistureDeficit[iMonth];

            SoilDrainage[iMonth] = Math.Round(SoilDrainage[iMonth], 1);
            _soilDrainage[iMonth] = SoilDrainage[iMonth];

            ActualEvapotranspiration[iMonth] = Math.Round(ActualEvapotranspiration[iMonth], 1);
            _actualEvapotranspiration[iMonth] = ActualEvapotranspiration[iMonth];

        }

        CalculateSoilTemperature();

        // Update calculation edit status:
        _dirty = false;

    }

    // -- Private class constructor / destructor ---------------------------------------------

    private void InitialiseParams()
    {

        // Reference parameters for the calculation of the diurnal variation
        // of air temperatures. For each month, air temperatures are assumed
        // to follow a fixed hourly pattern, varying between the daily minimum
        // and maximum, described by two fourier harmonics. The parameters of the
        // harmonics were derived empirically by the author from an analysis of
        // hourly temperature measurements at Cambridge.

        object Var;
        int iMonth;
        double iHour;
        double Value;


        var dava = new object[] { 0.488d, 0.481d, 0.474d, 0.454d, 0.469d, 0.481d, 0.473d, 0.261d, 0.453d, 0.472d, 0.478d, 0.5d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            _diurnalAve[iMonth] = Convert.ToDouble(dava[iMonth]);

        var ampone = new object[] { -0.232d, -0.305d, -0.359d, -0.389d, -0.416d, -0.41d, -0.429d, -0.416d, -0.385d, -0.332d, -0.278d, -0.191d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            _diurnalAmpOne[iMonth] = Convert.ToDouble(ampone[iMonth]);

        var phaseone = new object[] { -0.768d, -0.821d, -0.819d, -0.957d, -0.977d, -0.974d, -1.072d, -1.052d, -0.964d, -0.924d, -0.663d, -0.791d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            _diurnalPhaseOne[iMonth] = Convert.ToDouble(phaseone[iMonth]);

        var amptwo = new object[] { 0.109d, 0.129d, 0.109d, 0.071d, 0.034d, 0.014d, 0.015d, 0.053d, 0.106d, 0.124d, 0.129d, 0.105d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            _diurnalAmpTwo[iMonth] = Convert.ToDouble(amptwo[iMonth]);

        var phasetwo = new object[] { -0.917d, -0.945d, -0.966d, -1.202d, -1.117d, -0.886d, -1.065d, -1.207d, -1.223d, -0.164d, -0.765d, -0.864d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            _diurnalPhaseTwo[iMonth] = Convert.ToDouble(phasetwo[iMonth]);

        for (iMonth = 0; iMonth <= 11; iMonth++)
        {

            _diurnalMax[iMonth] = -999;
            _diurnalMin[iMonth] = -999;

            for (iHour = 1d; iHour <= 24d; iHour++)
            {

                Value = _diurnalAve[iMonth] + Math.Cos(iHour * 2d * PI / 24d + _diurnalPhaseOne[iMonth]) * _diurnalAmpOne[iMonth] + Math.Cos(iHour * 2d * PI / 12d + _diurnalPhaseTwo[iMonth]) * _diurnalAmpTwo[iMonth];

                if (_diurnalMax[iMonth] == -999 | Value > _diurnalMax[iMonth])
                {
                    _diurnalMax[iMonth] = Value;
                }

                if (_diurnalMin[iMonth] == -999 | Value < _diurnalMin[iMonth])
                {
                    _diurnalMin[iMonth] = Value;
                }

            }

        }

    }

    private void ClassInitializeRenamed()
    {
        InitialiseParams();
        Clear();

    }
    public MCDM() : base()
    {
        ClassInitializeRenamed();
    }

    private void ClassTerminateRenamed()
    {
        Clear();
    }
    ~MCDM()
    {
        ClassTerminateRenamed();
    }
}
