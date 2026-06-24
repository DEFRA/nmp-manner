using Manner.Application.DTOs;
namespace Manner.Application.Calculators;

public class MeanClimateDrainageModel
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

#pragma warning disable S1104
    public double _latitude; // Latitude of location (deg)
    public double _altitude; // Altitude of location (m)
#pragma warning restore S1104
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
        
    private const double DefaultCarbonValue = 1d;
    private const double SentinelTolerance = 0.0001d;
        
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

    // Added by RE on the 25/11/09
    private bool _dataSetforRainFall = false;
    private bool _dataSetforRainDays = false;
    private bool _dataSetforWindSpeed = false;
    private bool _dataSetforSunHours = false;
    private bool _dataSetforMaxTemp = false;
    private bool _dataSetforMinTemp = false;

    private const string _invalidMonthIndex = "Invalid Month Index";

    // -- Public property accessors --------------------------------------------------

    public double Diffusivity
    {
        get => _diffusivity;
        set
        {
            const double epsilon = 1e-6;

            bool isMinusOne = Math.Abs(value + 1) < epsilon;

            if (!isMinusOne && value <= 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Diffusivity must be -1 (special case) or a positive number."
                );
            }

            _diffusivity = value;
        }
    }

    public double GetSand(SoilLayer soilLayer)
    {
        double sandRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            sandRet = _topSand;
        }
        else
        {
            sandRet = _subSand;
        }

        return sandRet;
    }

    public double GetClay(SoilLayer soilLayer)
    {
        double clayRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            clayRet = _topClay;
        }
        else
        {
            clayRet = _subClay;
        }

        return clayRet;
    }

    public double GetDensity(SoilLayer soilLayer)
    {
        double densityRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            densityRet = _topDensity;
        }
        else
        {
            densityRet = _subDensity;
        }

        return densityRet;
    }

    public double GetCarbon(SoilLayer soilLayer)
    {
        double carbonRet = default;
        if (soilLayer == SoilLayer.Topsoil)
        {
            carbonRet = _topCarbon;
        }
        else
        {
            carbonRet = _subCarbon;
        }

        return carbonRet;
    }

    public double GetWindSpeed(int monthIndex = -1)
    {
        double windSpeedRet = default;
        if (monthIndex == -1)
        {
            windSpeedRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                windSpeedRet = windSpeedRet + _windSpeed[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            windSpeedRet = _windSpeed[monthIndex];
        }

        return windSpeedRet;

    }

    public void SetWindSpeed(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else if (value < 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Wind Speed Value");
        }
        else
        {
            _windSpeed[monthIndex] = value;
            _dirty = true;
            _dataSetforWindSpeed = true;
        }
    }

    public double GetMinTemp(int monthIndex = -1)
    {
        double minTempRet = default;

        if (monthIndex == -1)
        {
            minTempRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                minTempRet = minTempRet + _minTemp[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            minTempRet = _minTemp[monthIndex];
        }

        return minTempRet;

    }

    public void SetMinTemp(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else if (value < -100 || value > 100d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Minimum Temperature Value");
        }
        else
        {
            _minTemp[monthIndex] = value;
            _dirty = true;
            _dataSetforMinTemp = true;
        }
    }

    public double GetMaxTemp(int monthIndex = -1)
    {
        double maxTempRet = default;

        if (monthIndex == -1)
        {
            maxTempRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            { 
                maxTempRet = maxTempRet + _maxTemp[monthIndex] / 12d; 
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            maxTempRet = _maxTemp[monthIndex];
        }

        return maxTempRet;

    }

    public void SetMaxTemp(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else if (value < -100 || value > 100d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Maximum Temperature Value");
        }
        else
        {
            _maxTemp[monthIndex] = value;
            _dirty = true;
            _dataSetforMaxTemp = true;
        }
    }


    public double GetMeanTemp(int monthIndex = -1)
    {
        double meanTempRet = default;

        if (monthIndex == -1)
        {
            meanTempRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                meanTempRet = meanTempRet + _meanTemp[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            meanTempRet = _meanTemp[monthIndex];
        }

        return meanTempRet;

    }

    public void SetMeanTemp(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else if (value < -100 || value > 100d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Mean Temperature Value");
        }
        else
        {
            _meanTemp[monthIndex] = value;
            _dirty = true;
        }
    }

    public void SetEuniceTexture(bool value)
    {
        _euniceTexture = value;
    }

    public double GetSunHours(int monthIndex = -1)
    {
        double sunHoursRet = default;

        if (monthIndex == -1)
        {
            sunHoursRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                sunHoursRet = sunHoursRet + _sunHours[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, "Invalid Month Index");
        }
        else
        {
            sunHoursRet = _sunHours[monthIndex];
        }

        return sunHoursRet;
    }

    public void SetSunHours(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else if (value < 0d || value > 24d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Sun Hours Value");
        }
        else
        {
            _sunHours[monthIndex] = value;
            _dirty = true;
            _dataSetforSunHours = true;
        }
    }


    public double GetRainDays(int monthIndex = -1)
    {
        double rainDaysRet = default;

        if (monthIndex == -1)
        {
            rainDaysRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                rainDaysRet = rainDaysRet + _rainDays[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            rainDaysRet = _rainDays[monthIndex];
        }

        return rainDaysRet;
    }

    public void SetRainDays(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else if (value < 0d || value > 30d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Rain Days Value");
        }
        else
        {
            _rainDays[monthIndex] = value;
            _dirty = true;
            _dataSetforRainDays = true;
        }
    }

    public double GetRainfall(int monthIndex = -1)
    {
        double rainfallRet = default;

        if (monthIndex == -1)
        {
            rainfallRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
                rainfallRet = rainfallRet + _rainfall[monthIndex] / 12d;
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            rainfallRet = _rainfall[monthIndex];
        }

        return rainfallRet;

    }

    public void SetRainfall(int monthIndex = -1, double value = default)
    {
        if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else if (value < 0d || value > 1000d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Rainfall Value");
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
            LandCover landCoverTypeRet = default;
            landCoverTypeRet = _landCover;
            return landCoverTypeRet;
        }
    }

    public Texture PercentageAWC
    {
        get
        {
            Texture percentageAWCRet = default;
            percentageAWCRet = _texture;
            return percentageAWCRet;
        }
    }

    public double ObservationSoilTemperatureDepth
    {
        get
        {
            double observationSoilTemperatureDepthRet = default;
            observationSoilTemperatureDepthRet = _depth;
            return observationSoilTemperatureDepthRet;
        }
    }

    public double ObservationSoilTemperatureHour
    {
        get
        {
            double observationSoilTemperatureHourRet = default;
            observationSoilTemperatureHourRet = _hour;
            return observationSoilTemperatureHourRet;
        }
    }

    public double Altitude
    {
        get
        {
            double altitudeRet = default;
            altitudeRet = _altitude;
            return altitudeRet;
        }
    }

    public double Latitude
    {
        get
        {
            double latitudeRet = default;
            latitudeRet = _latitude;
            return latitudeRet;
        }
    }

    // -- Public calculated property accessors -------------------------------------------------

    public double GetCanopyEvaporation(int monthIndex = -1)
    {
        double canopyEvaporationRet = default;

        if (monthIndex == -1)
        {
            canopyEvaporationRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                canopyEvaporationRet = canopyEvaporationRet + _canopyEvaporation[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            if (_dirty)
            {
                Calculate();
            }
            canopyEvaporationRet = _canopyEvaporation[monthIndex];
        }

        return canopyEvaporationRet;
    }

    public double GetPotentialEvapotranspiration(int monthIndex = -1)
    {
        double potentialEvapotranspirationRet = default;

        if (monthIndex == -1)
        {
            potentialEvapotranspirationRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                potentialEvapotranspirationRet = potentialEvapotranspirationRet + _potentialEvapotranspiration[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            if (_dirty)
            {
                Calculate();
            }

            potentialEvapotranspirationRet = _potentialEvapotranspiration[monthIndex];
        }
        return potentialEvapotranspirationRet;
    }

    public double GetActualEvapotranspiration(int monthIndex = -1)
    {
        double actualEvapotranspirationRet = default;

        if (monthIndex == -1)
        {
            actualEvapotranspirationRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                actualEvapotranspirationRet = actualEvapotranspirationRet + _actualEvapotranspiration[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            if (_dirty)
            {
                Calculate();
            }
            actualEvapotranspirationRet = _actualEvapotranspiration[monthIndex];
        }

        return actualEvapotranspirationRet;
    }

    public double GetSurfaceRunOff(int monthIndex = -1)
    {
        double surfaceRunOffRet = default;

        if (monthIndex == -1)
        {
            surfaceRunOffRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                surfaceRunOffRet = surfaceRunOffRet + _surfaceRunOff[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            if (_dirty)
            {
                Calculate();
            }
            surfaceRunOffRet = _surfaceRunOff[monthIndex];
        }

        return surfaceRunOffRet;

    }

    public double GetSoilDrainage(int monthIndex = -1)
    {
        double soilDrainageRet = default;

        if (monthIndex == -1)
        {
            soilDrainageRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                soilDrainageRet = soilDrainageRet + _soilDrainage[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            if (_dirty)
            {
                Calculate();
            }
            soilDrainageRet = _soilDrainage[monthIndex];
        }

        return soilDrainageRet;
    }

    public double GetSoilMoistureDeficit(int monthIndex = -1)
    {
        double soilMoistureDeficitRet = default;

        if (monthIndex == -1)
        {
            soilMoistureDeficitRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                soilMoistureDeficitRet = soilMoistureDeficitRet + _soilMoistureDeficit[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            if (_dirty)
            {
                Calculate();
            }
            soilMoistureDeficitRet = _soilMoistureDeficit[monthIndex];
        }

        return soilMoistureDeficitRet;
    }

    public double GetSoilTemperature(int monthIndex = -1)
    {
        double soilTemperatureRet = default;

        if (monthIndex == -1)
        {
            soilTemperatureRet = 0d;
            for (monthIndex = 0; monthIndex <= 11; monthIndex++)
            {
                soilTemperatureRet = soilTemperatureRet + _soilTemperature[monthIndex] / 12d;
            }
        }
        else if (monthIndex < 0 || monthIndex > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(monthIndex), monthIndex, _invalidMonthIndex);
        }
        else
        {
            if (_dirty)
            {
                Calculate();
            }
            soilTemperatureRet = _soilTemperature[monthIndex];
        }

        return soilTemperatureRet;
    }

    public void SetLandCover(LandCover landCover)
    {

        if ((int)landCover < 0 || (int)landCover > 2)
        {
            throw new ArgumentOutOfRangeException(nameof(landCover), landCover, "Invalid Land Cover");
        }
        else
        {
            CalculateLandcover(landCover);
            _dirty = true;
        }
    }

    public void SetLocation(double latitude, double altitude = -1)
    {
        const double epsilon = 1e-6;

        bool isMinusOne = Math.Abs(altitude + 1) < epsilon;
        if (latitude < 0d || latitude > 90d)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), latitude, "Invalid Latitude Value");
        }
        else if (!isMinusOne && altitude < 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(altitude), altitude, "Invalid Altitude Value");
        }
        else
        {
            _latitude = latitude;

            if (isMinusOne)
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
            throw new ArgumentOutOfRangeException(nameof(depth), depth, "Invalid Observation Depth");
        }
        else if (hour != -1 && (hour < 0 || hour > 24))
        {
            throw new ArgumentOutOfRangeException(nameof(hour), hour, "Invalid Observation Hour");
        }
        else
        {
            _depth = depth;
            _hour = hour;
            _dirty = true;
        }
    }


    public void SetPercentTopsoilAWC(Texture soil, short topSoilAWCHard, short topSoilAWCEasy)
    {
        _topSoilAWCHard = topSoilAWCHard;
        _topSoilAWCEasy = topSoilAWCEasy;
    }

    public void SetPercentSubsoilAWC(Texture soil, short subSoilAWCHard, short subSoilAWCEasy)
    {
        _subSoilAWCHard = subSoilAWCHard;
        _subSoilAWCEasy = subSoilAWCEasy;
    }

    public void SetSoil(SoilLayer soilLayer, double sand, double silt, double clay, double density = -1, double carbon = -1)
    {
        ValidateSoilInputs(sand, silt, clay,carbon,density);

        bool isTop = soilLayer == SoilLayer.Topsoil;

        SetTexture(isTop, sand, silt, clay);
        SetCarbon(isTop, carbon);
        SetDensity(isTop, density);

        _dirty = true;
    }

    private const double Epsilon = 0.0001;

    private static bool IsApproximately(double value, double target) =>
        Math.Abs(value - target) < Epsilon;

    private static bool IsProvided(double value, double sentinel = -1d) =>
        Math.Abs(value - sentinel) > Epsilon;

    private static bool IsInRange(double value, double min, double max) =>
        value >= min && value <= max;

    private static void ValidateSoilInputs(double sand, double silt, double clay, double carbon, double density)
    {
        if (!IsApproximately(sand + silt + clay, 100d) ||
            !IsInRange(sand, 1d, 100d) ||
            !IsInRange(silt, 1d, 100d) ||
            !IsInRange(clay, 1d, 100d))
        {
            throw new ArgumentOutOfRangeException(nameof(sand), sand, "Invalid Particle Size Distribution");
        }

        if (IsProvided(carbon) && !IsInRange(carbon, 1d, 100d))
        {
            throw new ArgumentOutOfRangeException(nameof(carbon), carbon, "Invalid Carbon Content");
        }

        if (IsProvided(density) && !IsInRange(density, 1d, 2d))
        {
            throw new ArgumentOutOfRangeException(nameof(density), density, "Invalid Bulk Density");
        }
    }

    
    private void SetTexture(bool isTop, double sand, double silt, double clay)
    {
        if (isTop)
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
    }

    

    private void SetCarbon(bool isTop, double carbon)
    {
        double value = Math.Abs(carbon + 1d) > SentinelTolerance
            ? carbon
            : DefaultCarbonValue;

        if (isTop)
        {
            _topCarbon = value;
        }
        else
        {
            _subCarbon = value;
        }
    }

    private void SetDensity(bool isTop, double density)
    {
        double value = IsProvided(density) ? density : 1.2d;

        if (isTop)
        {
            _topDensity = value;
        }
        else
        {
            _subDensity = value;
        }
    }

    public bool RetrieveClimate(ClimateDto climate)
    {
        bool retrieveClimateRet = false;
        // This function retrieves climate data from the given ClimateFile
        // for the location specified by  easting and northing
        // parameter values. If the climate file does not exist, or is corrupt, or the map location does not exist in the file, then the function returns false.

        bool needToRetriveData = true;
        try
        {
            if (_dataSetforMaxTemp && _dataSetforMinTemp && _dataSetforRainDays && _dataSetforRainFall && _dataSetforSunHours && _dataSetforWindSpeed)
            {
                needToRetriveData = false;
            }

            if (needToRetriveData)
            {
                // Find map location index:
                PopulateClimateObj(climate);
            }
            retrieveClimateRet = true;
        }
        catch (Exception)
        {
            retrieveClimateRet = false;
        }

        return retrieveClimateRet;

    }

    public bool RetrieveClimate(ClimateDto climate, int easting, int northing)
    {
        bool retrieveClimateRet = false;
        // This function retrieves climate data from the given Climate Dataset
        // for the location specified by  easting and northing parameter values. If the climate file does not exist, or is corrupt, or the map location does not exist in the file, then the function returns false.


        bool needToRetriveData = true;        

        try
        {

            if (_dataSetforMaxTemp && _dataSetforMinTemp && _dataSetforRainDays && _dataSetforRainFall && _dataSetforSunHours && _dataSetforWindSpeed)
            {
                needToRetriveData = false;
            }

            if (needToRetriveData)
            {                
                PopulateClimateObj(climate);
            }
            retrieveClimateRet = true;
        }
        catch (Exception)
        {
            retrieveClimateRet = false;
        }

        return retrieveClimateRet;
    }

    private void PopulateClimateObj(ClimateDto climate)
    {
        try
        {
            // Site altitude (m)
            _altitude = Convert.ToDouble(climate.Altitude);

            // Mean daily windspeed (m/s):
            _windSpeed[0] = Convert.ToDouble(climate.MeanWindSpeedJan);
            _windSpeed[1] = Convert.ToDouble(climate.MeanWindSpeedFeb);
            _windSpeed[2] = Convert.ToDouble(climate.MeanWindSpeedMar);
            _windSpeed[3] = Convert.ToDouble(climate.MeanWindSpeedApr);
            _windSpeed[4] = Convert.ToDouble(climate.MeanWindSpeedMay);
            _windSpeed[5] = Convert.ToDouble(climate.MeanWindSpeedJun);
            _windSpeed[6] = Convert.ToDouble(climate.MeanWindSpeedJul);
            _windSpeed[7] = Convert.ToDouble(climate.MeanWindSpeedAug);
            _windSpeed[8] = Convert.ToDouble(climate.MeanWindSpeedSep);
            _windSpeed[9] = Convert.ToDouble(climate.MeanWindSpeedOct);
            _windSpeed[10] = Convert.ToDouble(climate.MeanWindSpeedNov);
            _windSpeed[11] = Convert.ToDouble(climate.MeanWindSpeedDec);

            // Mean total rainfall (mm):
            _rainfall[0] = Convert.ToDouble(climate.MeanTotalRainFallJan);
            _rainfall[1] = Convert.ToDouble(climate.MeanTotalRainFallFeb);
            _rainfall[2] = Convert.ToDouble(climate.MeanTotalRainFallMar);
            _rainfall[3] = Convert.ToDouble(climate.MeanTotalRainFallApr);
            _rainfall[4] = Convert.ToDouble(climate.MeanTotalRainFallMay);
            _rainfall[5] = Convert.ToDouble(climate.MeanTotalRainFallJun);
            _rainfall[6] = Convert.ToDouble(climate.MeanTotalRainFallJul);
            _rainfall[7] = Convert.ToDouble(climate.MeanTotalRainFallAug);
            _rainfall[8] = Convert.ToDouble(climate.MeanTotalRainFallSep);
            _rainfall[9] = Convert.ToDouble(climate.MeanTotalRainFallOct);
            _rainfall[10] = Convert.ToDouble(climate.MeanTotalRainFallNov);
            _rainfall[11] = Convert.ToDouble(climate.MeanTotalRainFallDec);

            // Mean number of wet days (n):
            _rainDays[0] = Min(Convert.ToDouble(climate.MeanRainDaysJan), 30d);
            _rainDays[1] = Min(Convert.ToDouble(climate.MeanRainDaysFeb), 30d);
            _rainDays[2] = Min(Convert.ToDouble(climate.MeanRainDaysMar), 30d);
            _rainDays[3] = Min(Convert.ToDouble(climate.MeanRainDaysApr), 30d);
            _rainDays[4] = Min(Convert.ToDouble(climate.MeanRainDaysMay), 30d);
            _rainDays[5] = Min(Convert.ToDouble(climate.MeanRainDaysJun), 30d);
            _rainDays[6] = Min(Convert.ToDouble(climate.MeanRainDaysJul), 30d);
            _rainDays[7] = Min(Convert.ToDouble(climate.MeanRainDaysAug), 30d);
            _rainDays[8] = Min(Convert.ToDouble(climate.MeanRainDaysSep), 30d);
            _rainDays[9] = Min(Convert.ToDouble(climate.MeanRainDaysOct), 30d);
            _rainDays[10] = Min(Convert.ToDouble(climate.MeanRainDaysNov), 30d);
            _rainDays[11] = Min(Convert.ToDouble(climate.MeanRainDaysDec), 30d);

            // Mean sun hours (hrs):
            _sunHours[0] = Convert.ToDouble(climate.MeanSunHoursJan) / 30d;
            _sunHours[1] = Convert.ToDouble(climate.MeanSunHoursFeb) / 30d;
            _sunHours[2] = Convert.ToDouble(climate.MeanSunHoursMar) / 30d;
            _sunHours[3] = Convert.ToDouble(climate.MeanSunHoursApr) / 30d;
            _sunHours[4] = Convert.ToDouble(climate.MeanSunHoursMay) / 30d;
            _sunHours[5] = Convert.ToDouble(climate.MeanSunHoursJun) / 30d;
            _sunHours[6] = Convert.ToDouble(climate.MeanSunHoursJul) / 30d;
            _sunHours[7] = Convert.ToDouble(climate.MeanSunHoursAug) / 30d;
            _sunHours[8] = Convert.ToDouble(climate.MeanSunHoursSep) / 30d;
            _sunHours[9] = Convert.ToDouble(climate.MeanSunHoursOct) / 30d;
            _sunHours[10] = Convert.ToDouble(climate.MeanSunHoursNov) / 30d;
            _sunHours[11] = Convert.ToDouble(climate.MeanSunHoursDec) / 30d;

            // Mean daily min temp (oC):
            _minTemp[0] = Convert.ToDouble(climate.MeanMinJan);
            _minTemp[1] = Convert.ToDouble(climate.MeanMinFeb);
            _minTemp[2] = Convert.ToDouble(climate.MeanMinMar);
            _minTemp[3] = Convert.ToDouble(climate.MeanMinApr);
            _minTemp[4] = Convert.ToDouble(climate.MeanMinMay);
            _minTemp[5] = Convert.ToDouble(climate.MeanMinJun);
            _minTemp[6] = Convert.ToDouble(climate.MeanMinJul);
            _minTemp[7] = Convert.ToDouble(climate.MeanMinAug);
            _minTemp[8] = Convert.ToDouble(climate.MeanMinSep);
            _minTemp[9] = Convert.ToDouble(climate.MeanMinOct);
            _minTemp[10] = Convert.ToDouble(climate.MeanMinNov);
            _minTemp[11] = Convert.ToDouble(climate.MeanMinDec);

            // Mean daily max temp (oC):
            _maxTemp[0] = Convert.ToDouble(climate.MeanMaxJan);
            _maxTemp[1] = Convert.ToDouble(climate.MeanMaxFeb);
            _maxTemp[2] = Convert.ToDouble(climate.MeanMaxMar);
            _maxTemp[3] = Convert.ToDouble(climate.MeanMaxApr);
            _maxTemp[4] = Convert.ToDouble(climate.MeanMaxMay);
            _maxTemp[5] = Convert.ToDouble(climate.MeanMaxJun);
            _maxTemp[6] = Convert.ToDouble(climate.MeanMaxJul);
            _maxTemp[7] = Convert.ToDouble(climate.MeanMaxAug);
            _maxTemp[8] = Convert.ToDouble(climate.MeanMaxSep);
            _maxTemp[9] = Convert.ToDouble(climate.MeanMaxOct);
            _maxTemp[10] = Convert.ToDouble(climate.MeanMaxNov);
            _maxTemp[11] = Convert.ToDouble(climate.MeanMaxDec);
        }
        catch (Exception ex)
        {
            throw new ArgumentOutOfRangeException(nameof(climate), climate, ex.Message);            
        }
    }

    public void Clear()
    {
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
            SetSunHours(iMonth, d);
        }


        // Default wind speed (m/s)

        var windSpeed = new object[] { 5.1d, 5, 5.3d, 4.8d, 4.6d, 4.2d, 4.1d, 4.2d, 4.3d, 4.4d, 4.8d, 4.9d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            SetWindSpeed(iMonth, Convert.ToDouble(windSpeed[iMonth]));

        // Default min temp (oC)
        var minTemp = new object[] { 0.6d, 0.5d, 1.6d, 3.8d, 6.5d, 9.7d, 11.8d, 11.5d, 9.5d, 6.1d, 3.5d, 1.7d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            SetMinTemp(iMonth, Convert.ToDouble(minTemp[iMonth]));

        // Default max temp (oC)
        var maxTemp = new object[] { 6.3d, 7.2d, 10.5d, 13.7d, 17.2d, 20.5d, 22.2d, 22, 19.2d, 14.6d, 9.9d, 7.3d };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            SetMaxTemp(iMonth, Convert.ToDouble(maxTemp[iMonth]));

        // Default rain days (n)

        var rainDays = new object[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 };
        for (iMonth = 0; iMonth <= 11; iMonth++)
            SetRainDays(iMonth, Convert.ToDouble(rainDays[iMonth]));

        // Default rainfall (mm)
        var rainfall = new object[] { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 };
        for (iMonth = 0; iMonth <= 11; iMonth++)

            SetRainfall(iMonth, Convert.ToDouble(rainfall[iMonth]));

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

    private static void CalculateClimateFit(ref double[] Data, ref double average, ref double amplitude, ref double phase)
    {
        // Fit the first fourier harmonic to monthly mean data
        // and return the coefficients.

        double sum;
        double sumCos;
        double sumSin;
        int index;

        sum = 0d;
        for (index = 0; index <= 11; index++)
        {
            sum = sum + Data[index];
        }

        average = sum / 12d;
        sumCos = 0d;
        sumSin = 0d;

        for (index = 0; index <= 11; index++)
        {
            sumCos = sumCos + (Data[index] - average) * Math.Cos(2d * PI * (index + 0.5d) / 12d);
            sumSin = sumSin + (Data[index] - average) * Math.Sin(2d * PI * (index + 0.5d) / 12d);
        }

        sumCos = sumCos * 2d / 12d;
        sumSin = sumSin * 2d / 12d;
        phase = CalculateATan2(-sumSin, sumCos);
        amplitude = Math.Sqrt(Math.Pow(sumCos, 2d) + Math.Pow(sumSin, 2d));
    }

    private static double CalculateATan2(double opp, double adj)
    {
        double calculateATan2Ret = default;
        double angle;

        if (Math.Abs(adj) < 0.0001d)
        {
            angle = PI / 2d;
        }
        else
        {
            angle = Math.Abs(Math.Atan(opp / adj));
        }

        if (adj < 0d)
        {
            angle = PI - angle;
        }

        if (opp < 0d)
        {
            angle = -1 * angle;
        }

        calculateATan2Ret = angle;
        return calculateATan2Ret;
    }

    private void CalculateSoilTemperature()
    {
        // Calculate the monthly mean soil temperatures at the reference
        // depth and observation hour, by application of the homogenous
        // conductor model with a correction for screen temperature.
                
        var surfaceTemperature = new double[12];
        var meanSurface = default(double);
        var meanAmplitude = default(double);
        var meanPhase = default(double);
        var seasonalAmplitude = default(double);
        var seasonalPhase = default(double);

        for (int month = 0; month < 12; month++)
        {
            surfaceTemperature[month] = CalculateSurfaceTemperature(month);
        }

        // Fit first fourier harmonic to monthly mean soil surface temperatures:
        CalculateClimateFit(ref surfaceTemperature, ref meanSurface, ref meanAmplitude, ref meanPhase);

        // Calculate seasonal depth corrections to phase and amplitude:
        CalculateSeasonalCorrection(_depth, ref seasonalAmplitude, ref seasonalPhase);

        // Calculate monthly mean soil temperatures at depth: 
        for (int month = 0; month < 12; month++)
        {
            _soilTemperature[month] = CalculateSoilTemperature(month, meanSurface, meanAmplitude, seasonalAmplitude, meanPhase, seasonalPhase);
        }
    }

    private double CalculateSoilTemperature(int month, double meanSurface, double meanAmplitude, double seasonalAmplitude, double meanPhase, double seasonalPhase)
    {
        double baseTemp = CalculateBaseSoilTemperature(month, meanSurface, meanAmplitude, seasonalAmplitude, meanPhase, seasonalPhase);

        return HasObservationHour()
            ? baseTemp + GetObservationCorrection(month)
            : baseTemp;
    }

    private static double CalculateBaseSoilTemperature(int month, double meanSurface, double meanAmplitude, double seasonalAmplitude, double meanPhase, double seasonalPhase)
    {
        return meanSurface
            + meanAmplitude * seasonalAmplitude
            * Math.Cos((month + 0.5d) * 2d * PI / 12d + meanPhase + seasonalPhase);
    }

    private const double UnspecifiedHour = -1d;
    private const double FloatingPointTolerance = 0.0001d;

    private bool HasObservationHour() =>
        Math.Abs(_hour - UnspecifiedHour) > FloatingPointTolerance;

    private double GetObservationCorrection(int month)
    {
        return CalculateDayTemp(_depth, month, (int)Math.Round(_hour));
    }

    private double CalculateSurfaceTemperature(int month)
    {
        double meanTemp = 0.5d * (_minTemp[month] + _maxTemp[month]);
        double correction = (2.45d * ((_canopyEvaporation[month] + _actualEvapotranspiration[month]) / 30d) - _radiation[month]) / _delta[month];
        return meanTemp - correction;
    }

    private double CalculateDayTemp(double depth, int indexMonth, int indexHour)
    {
        double calculateDayTempRet = default;

        // Calculate adjustment to monthly mean soil temperature to
        // represent diurnal variation of soil temperature.

        double dampingDepth12hr;
        double dampingDepth24hr;

        dampingDepth12hr = Math.Sqrt(12.0d * 3600.0d * CalculateDiffusivity() / PI);
        dampingDepth24hr = Math.Sqrt(24.0d * 3600.0d * CalculateDiffusivity() / PI);

        if (_landCover == LandCover.ManagedGrass)
        {
            depth = depth + 0.1d;
        }

        calculateDayTempRet = _diurnalAve[indexMonth] + Math.Exp(-1 * depth / dampingDepth24hr) * Math.Cos(indexHour * 2 * PI / 24d + _diurnalPhaseOne[indexMonth] - depth / dampingDepth24hr) * _diurnalAmpOne[indexMonth] + Math.Exp(-1 * depth / dampingDepth12hr) * Math.Cos(indexHour * 2 * PI / 12d + _diurnalPhaseTwo[indexMonth] - depth / dampingDepth24hr) * _diurnalAmpTwo[indexMonth];

        calculateDayTempRet = (calculateDayTempRet - _diurnalMin[indexMonth]) / (_diurnalMax[indexMonth] - _diurnalMin[indexMonth]);

        if (calculateDayTempRet < 0.5d)
        {
            calculateDayTempRet = 0.5d - calculateDayTempRet;
            calculateDayTempRet = -1 * calculateDayTempRet * (_maxTemp[indexMonth] - _minTemp[indexMonth]);
        }
        else
        {
            calculateDayTempRet = (calculateDayTempRet - 0.5d) * (_maxTemp[indexMonth] - _minTemp[indexMonth]);
        }

        return calculateDayTempRet;

    }

    private void CalculateSeasonalCorrection(double depth, ref double amplitude, ref double phase)
    {

        // Calculate seasonal damped amplitude and phase shift
        // of soil temperatures at observation depth.

        double seasonalDampingDepth;
        double profileDiff;

        // Depth correction for ManagedGrass cover:
        if (_landCover == LandCover.ManagedGrass)
        {
            depth = depth + 0.1d;
        }

        profileDiff = CalculateDiffusivity();

        seasonalDampingDepth = Math.Pow(profileDiff * 24.0d * 3600.0d * 365.0d / PI, 0.5d);

        amplitude = Math.Exp(-1 * depth / seasonalDampingDepth);
        phase = -depth / seasonalDampingDepth;

    }

    private const double UndefinedDiffusivity = -1d;    

    public double CalculateDiffusivity()
    {
        double density = CalculateDensity();

        if (Math.Abs(_diffusivity - UndefinedDiffusivity) < FloatingPointTolerance)
        {
            return 0.000001d * CalculateThermalConductivity()
                   / (density * CalculateHeatCapacity());
        }

        return _diffusivity;
    }

    private double CalculateDensity()
    {
        if (_depth > 0.3d)
        {
            return (0.3d * _topDensity + (_depth - 0.3d) * _subDensity) / _depth;
        }

        return _topDensity;
    }

    private double CalculateThermalConductivity()
    {
        double calculateThermalConductivityRet = default;

        // Calculate thermal conductivity of soil profile (J /  s m oC)
        // according to the method of Bristow (2002).

        double sand;
        double clay;
        double silt;

        var soil = default(double);
        var air = default(double);
        var water = default(double);

        double a;
        double b;
        double c;
        double d;
        double e;

        CalculateSoilAirWater(ref soil, ref air, ref water);

        if (_depth > 0.3d)
        {
            sand = 0.01d * (0.3d * _topSand + (_depth - 0.3d) * _subSand) / _depth;
            silt = 0.01d * (0.3d * _topSilt + (_depth - 0.3d) * _subSilt) / _depth;
            clay = 0.01d * (0.3d * _topClay + (_depth - 0.3d) * _subClay) / _depth;
        }
        else
        {
            sand = _topSand * 0.01d;
            silt = _topSilt * 0.01d;
            clay = _topClay * 0.01d;
        }

        a = (0.57d + 1.73d * sand + 0.93d * silt) / (1d - 0.74d * sand - 0.49d * silt) - 2.8d * soil * (1d - soil);
        b = 2.8d * soil;
        c = 1d + 2.6d / Math.Pow(clay, 0.5d);
        d = 0.03d + 0.7d * Math.Pow(soil, 2d);
        e = 4d;

        calculateThermalConductivityRet = a + b * water - (a - d) * Math.Exp(-Math.Pow(c * water, e));
        return calculateThermalConductivityRet;

    }

    private double CalculateHeatCapacity()
    {
        double calculateHeatCapacityRet = default;

        // Calculate heat capacity of soil profile ( MJ / m3 oC), assuming
        // that the soil is at field capacity.

        var air = default(double);
        var soil = default(double);
        var water = default(double);

        CalculateSoilAirWater(ref soil, ref air, ref water);

        calculateHeatCapacityRet = 1.92d * soil + 1d * air + water * 4.18d;
        return calculateHeatCapacityRet;

    }

    private void CalculateLandcover(LandCover landCover)
    {
        // Calculate monthly mean values of plant height, canopy resistance
        // and height, according to the selected land cover type.
        _landCover = landCover;

        switch (landCover)
        {
            case LandCover.BareSoil:
                ApplyLandCover(
                    leafArea: CreateArray(0d),
                    canopy: CreateArray(100d),
                    height: CreateArray(0.05d));
                break;

            case LandCover.ManagedGrass:
                ApplyLandCover(
                    leafArea: new[] { 2d, 2d, 3d, 4d, 5d, 5d, 5d, 5d, 4d, 3d, 2.5d, 2d },
                    canopy: CreateArray(70d),
                    height: CreateArray(0.15d));
                break;

            case LandCover.WinterWheat:
                ApplyLandCover(
                    leafArea: new[] { 0.5d, 0.5d, 0.6d, 1.8d, 3.3d, 4.7d, 5d, 3.2d, 0d, 0.3d, 0.3d, 0.5d },
                    canopy: new[] { 40d, 40d, 40d, 40d, 40d, 40d, 125d, 300d, 100d, 40d, 40d, 40d },
                    height: new[] { 0.08d, 0.08d, 0.08d, 0.14d, 0.35d, 0.7d, 0.8d, 0.5d, 0.05d, 0.08d, 0.08d, 0.08d });
                break;
        }
    }

    private void ApplyLandCover(double[] leafArea, double[] canopy, double[] height)
    {
        for (int i = 0; i < 12; i++)
        {
            _leafAreaIndex[i] = leafArea[i];
            _canopyResistance[i] = canopy[i];
            _height[i] = height[i];
        }
    }

    private static double[] CreateArray(double value)
    {
        return Enumerable.Repeat(value, 12).ToArray();
    }

    private void CalculatePotentialEvapotranspiration(int indexMonth, ref double pet, ref double ratio, ref double delta, ref double radiation)
    {
        // Calculate monthly mean potential evapotranspiration (mm/day) based
        // on Penman-Monteith formula for the currently selected land cover,
        // and the efficiency ratio of evaporation from the plant canopy.

        double relativeDistance;
        double solarDeclination;
        double sunSetHourAngle;
        double julianDay;
        double latitude;
        double dayLength;
        double albedo;

        double actualVapourPressure;
        double extraterrestrialRadiation;
        double shortwaveRadiation;
        double longwaveRadiation;

        double vapourSlope;
        double saturationVapourPressure;
        double correction;
        double airDensity;
        double airPressure;
        double aeroResistance;
        double surfaceResistance;

        double angstromA;
        double angstromB;
        double g;

        // Calculate soil heat loss:
        if (indexMonth == 0)
        {
            g = 0.07d * (_minTemp[1] + _maxTemp[1] - _minTemp[11] - _maxTemp[11]) * 0.5d;
        }
        else if (indexMonth == 11)
        {
            g = 0.07d * (_minTemp[0] + _maxTemp[0] - _minTemp[10] - _maxTemp[10]) * 0.5d;
        }
        else
        {
            g = 0.07d * (_minTemp[indexMonth + 1] + _maxTemp[indexMonth + 1] - _minTemp[indexMonth - 1] - _maxTemp[indexMonth - 1]) * 0.5d;
        }

        // Calculate combined plant and soil albedo, taking account
        // of rainfall effect on soil albedo:
        if (_topSand > 50d)
        {
            albedo = 0.1d;
        }
        else if (_topClay > 50d)
        {
            albedo = 0.3d;
        }
        else
        {
            albedo = 0.2d;
        }

        albedo = _rainDays[indexMonth] * albedo * 0.5d / 30d + (30d - _rainDays[indexMonth]) * albedo / 30d;

        if (_leafAreaIndex[indexMonth] > 4d)
        {
            albedo = 0.25d;
        }
        else
        {
            albedo = albedo + 0.25d * (0.25d - albedo) * _leafAreaIndex[indexMonth];
        }

        // Calculate incident radiation budget:
        latitude = _latitude * 2d * PI / 360d;
        julianDay = 30.42d * (indexMonth + 1) - 15.23d;

        solarDeclination = 0.409d * Math.Sin(0.0172d * julianDay - 1.39d);
        relativeDistance = 1d + 0.033d * Math.Cos(0.0172d * julianDay);

        sunSetHourAngle = -1 * Math.Tan(latitude) * Math.Tan(solarDeclination);
        sunSetHourAngle = Math.Atan(-sunSetHourAngle / Math.Sqrt(-sunSetHourAngle * sunSetHourAngle + 1.0d)) + 2.0d * Math.Atan(1.0d);
        dayLength = 7.64d * sunSetHourAngle;

        extraterrestrialRadiation = 37.6d * relativeDistance * (sunSetHourAngle * Math.Sin(latitude) * Math.Sin(solarDeclination) + Math.Cos(latitude) * Math.Cos(solarDeclination) * Math.Sin(sunSetHourAngle));

        // Seasonal variation of angstrom coefficients:
        angstromA = 0.235d;
        angstromB = 0.5058d + 0.0278d * Math.Cos((julianDay - 140.9d) * 2d * PI / 365d) + 0.0074d * Math.Cos(2d * (julianDay - 84.7d) * 2d * PI / 365d);

        // North south geographical variation of angstrom coefficients:
        angstromB = angstromB - 0.05d + 0.1d * ((_latitude - 49.81d) / 0.00000913d) / 1000000d;

        shortwaveRadiation = (1d - albedo) * extraterrestrialRadiation * (angstromA + angstromB * _sunHours[indexMonth] / dayLength);

        actualVapourPressure = 0.611d * Math.Exp(17.27d * _minTemp[indexMonth] / (_minTemp[indexMonth] + 237.3d));

        longwaveRadiation = -0.0000000049d * (0.2d + 0.8d * _sunHours[indexMonth] / dayLength) * (0.34d - 0.14d * Math.Sqrt(actualVapourPressure)) * (Math.Pow(273d + _maxTemp[indexMonth], 4d) + Math.Pow(273d + _minTemp[indexMonth], 4d)) * 0.5d;

        saturationVapourPressure = Math.Exp(17.27d * _minTemp[indexMonth] / (_minTemp[indexMonth] + 237.3d)) + Math.Exp(17.27d * _maxTemp[indexMonth] / (_maxTemp[indexMonth] + 237.3d));
        saturationVapourPressure = saturationVapourPressure * 0.611d * 0.5d;

        vapourSlope = 4098.0d * saturationVapourPressure / Math.Pow(0.5d * (_minTemp[indexMonth] + _maxTemp[indexMonth]) + 237.3d, 2d);

        correction = 4.0d * 0.98d * 0.0000000049d * Math.Pow(273.1d + 0.5d * (_minTemp[indexMonth] + _maxTemp[indexMonth]), 3d);

        airPressure = 101.3d * Math.Pow((293d - 0.0065d * 0d) / 293d, 5.26d);
        airDensity = 1000.0d * airPressure / (287.0d * 1.01d * (0.5d * (_minTemp[indexMonth] + _maxTemp[indexMonth]) + 273.0d));

        aeroResistance = 6.25d / _windSpeed[indexMonth] * Math.Log(10.0d / (0.1d * _height[indexMonth])) * Math.Log(6.0d / (0.1d * _height[indexMonth]));

        // Calculate surface resistance as a function of crop canopy resistance,
        // leaf area and soil resistance on wet and dry days:
        surfaceResistance = Math.Pow(0.7d, _leafAreaIndex[indexMonth]) / (_rainDays[indexMonth] * 100d / 30d + (30d - _rainDays[indexMonth]) * 600d / 30d);
        surfaceResistance = surfaceResistance + (1d - Math.Pow(0.7d, _leafAreaIndex[indexMonth])) / _canopyResistance[indexMonth];
        surfaceResistance = 1d / surfaceResistance;

        // Screen temperature correction method for calculation of PET:
        pet = vapourSlope * (shortwaveRadiation + longwaveRadiation - g) + 86.4d * airDensity * 1.013d * (saturationVapourPressure - actualVapourPressure) * (1d + correction * aeroResistance / (86.4d * airDensity * 1.013d)) / aeroResistance;
        pet = pet / (vapourSlope + 0.00163d * airPressure / 2.45d * (1d + surfaceResistance / aeroResistance) * (1d + correction * aeroResistance / (86.4d * airDensity * 1.013d)));
        pet = pet / 2.45d;

        // Efficiency of evaporation from plant canopy:
        ratio = vapourSlope + 0.00163d * airPressure / 2.45d * (1.0d + surfaceResistance / aeroResistance);
        ratio = ratio / (vapourSlope + 0.00163d * airPressure / 2.45d);

        // Surface temperature correction data:
        radiation = shortwaveRadiation + longwaveRadiation - g;
        delta = 86.4d * airDensity * 1.013d / aeroResistance + correction;

    }

#pragma warning disable S3241
    private static bool CalculateGammaQuantile(double probability, double beta, double alpha, ref double quantile)
#pragma warning restore S3241
    {
        bool calculateGammaQuantileRet = default;

        // Calculate the quantile corresponding to a probability
        // of non-exceedance for a two-parameter gamma function
        // using the Wilson-Hilferty (1931) transformation or
        // the modified transformation of Kirby (1972).

        // Default return:
        calculateGammaQuantileRet = false;

        double tmpDC;
        var tmpDU = default(double);
        var tmpDK = default(double);
        double tmpDQ;
        double tmpDA;
        double tmpDB;
        double tmpDG;
        double tmpDH;

        try
        {
            tmpDC = alpha / Math.Abs(alpha) * 2d / Math.Pow(beta, 0.5d);

            if (!CalculateNormalVariate(probability, ref tmpDU))
            {
                return calculateGammaQuantileRet;
            }

            if (tmpDC < 1d)
            {
                tmpDK = tmpDC * (tmpDU - tmpDC / 6d) / 6d + 1d;
                tmpDK = Math.Pow(tmpDK, 3d);
                tmpDK = tmpDK - 1d;
                tmpDK = tmpDK * 2d / tmpDC;
            }
            else if (tmpDC < 9.75d)
            {
                tmpDG = -0.00385205d + 1.00426d * tmpDC + 0.00651207d * Math.Pow(tmpDC, 2d) + -0.0149166d * Math.Pow(tmpDC, 3d) + 0.00163945d * Math.Pow(tmpDC, 4d) + -0.0000583804d * Math.Pow(tmpDC, 5d);
                tmpDA = 0.00199447d + 0.48489d * tmpDC + 0.0230935d * Math.Pow(tmpDC, 2d) + -0.0152435d * Math.Pow(tmpDC, 3d) + 0.00160597d * Math.Pow(tmpDC, 4d) + -0.000055869d * Math.Pow(tmpDC, 5d);
                tmpDB = 0.990562d + 0.0319647d * tmpDC + -0.0274231d * Math.Pow(tmpDC, 2d) + 0.00777405d * Math.Pow(tmpDC, 3d) + -0.000571184d * Math.Pow(tmpDC, 4d) + 0.0000142077d * Math.Pow(tmpDC, 5d);
                tmpDA = 1d / tmpDA;
                tmpDH = Math.Pow(tmpDB - 2d / tmpDC / tmpDA, 1d / 3d);
                tmpDK = 1d - Math.Pow(tmpDG / 6d, 2d) + tmpDU * (tmpDG / 6d);

                if (tmpDK < tmpDH)
                {
                    tmpDK = tmpDH;
                }

                tmpDK = Math.Pow(tmpDK, 3d);
                tmpDK = (tmpDK - tmpDB) * tmpDA;
            }
            else
            {
                throw new InvalidOperationException("Algorithm Range Error");
            }

            tmpDQ = alpha * beta + tmpDK * Math.Pow(Math.Pow(alpha, 2d) * beta, 0.5d);
            quantile = tmpDQ;

            // Return value:
            calculateGammaQuantileRet = true;
        }

        catch (Exception)
        {
            // do not throughn exception, but return false to indicate failure of quantile calculation:            
        }

        return calculateGammaQuantileRet;

    }

    private static bool CalculateNormalVariate(double probability, ref double variate)
    {
        bool calculateNormalVariateRet = false;

        // Calculate the standard normal variate corresponding to a
        // probability of non-exceedence, using the approximate
        // solution of Abramowitz and Stegun (1965).


        double tmpDW;
        double tmpDP;
        double tmpDU;
        double tmpDen;
        double tmpNum;

        try
        {
            tmpDP = 1d - probability;
            if (tmpDP > 0.5d)
            {
                tmpDP = 1d - tmpDP;
            }

            tmpDW = Math.Pow(-2.0d * Math.Log(tmpDP), 0.5d);
            tmpDen = 1.0d + 1.432788d * tmpDW + 0.189269d * Math.Pow(tmpDW, 2d) + 0.001308d * Math.Pow(tmpDW, 3d);
            tmpNum = 2.515517d + 0.802853d * tmpDW + 0.010328d * Math.Pow(tmpDW, 2d);
            tmpDU = tmpDW - tmpNum / tmpDen;

            if (1d - probability > 0.5d)
            {
                tmpDU = -tmpDU;
            }

            variate = tmpDU;

            // Return Value:
            calculateNormalVariateRet = true;
        }
        catch (Exception)
        {
            // Do not throw exception, but return false to indicate failure of variate calculation:
        }

        return calculateNormalVariateRet;
    }


#pragma warning disable S1172
    private static double CalculateRunOff(int indexMonth, double rainfall)
#pragma warning restore S1172
    {
        double calculateRunOffRet = default;

        calculateRunOffRet = 0d;
        return calculateRunOffRet;

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

    private void CalculateSoilAirWater(ref double soil, ref double air, ref double water)
    {
        // Calculate the proportional soil, air and water contents
        // of the soil at field capacity, above the depth of soil
        // temperature observation.

        double phi05;

        if (_depth > 0.3d)
        {
            soil = (0.3d * _topDensity + (_depth - 0.3d) * _subDensity) / _depth;
        }
        else
        {
            soil = _topDensity;
        }

        soil = soil / 2.65d;
        air = 1d - soil;

        // Water content at field capacity:
        phi05 = 47d + 0.25d * _topClay + 0.1d * _topSilt + 1.12d * _topCarbon - 16.52d * _topDensity;

        if (_depth > 0.3d)
        {

            water = phi05 * 0.3d;
            phi05 = 37.2d + 0.35d * _subClay + 0.12d * _subSilt - 11.73d * _subDensity;
            water = water + (_depth - 0.3d) * phi05;
            water = water / _depth;
        }
        else
        {
            water = phi05;
        }

        water = water * 0.01d;
        water = Max(0d, water);
        water = Min(air, water);
        air = air - water;
    }

    private void CalculateAvailableWater(ref double am02, ref double am15)
    {
        // Calculate the plant available water held in the soil profile
        // between field capacity and a tension of 2 bar (AM02) and
        // between the tensions of 2 and 15 bar (AM15), using a particle
        // size pedotransfer function and in proportion to the rooting depth
        // of the current landcover.

        var phi05 = default(double);
        var phi15 = default(double);
        var phi02 = default(double);

        // Initialise available water contents:
        am02 = 0d;
        am15 = 0d;

        if (_euniceTexture)
        {
            am02 = am02 + _topSoilAWCEasy * 0.01d * 300d;
            am15 = am15 + (_topSoilAWCHard - _topSoilAWCEasy) * 0.01d * 300d;
        }
        else
        {
            // Top soil water content:
            phi05 = 47d + 0.25d * _topClay + 0.1d * _topSilt + 1.12d * _topCarbon - 16.52d * _topDensity;
            phi02 = 8.7d + 0.45d * _topClay + 0.11d * _topSilt + 1.03d * _topCarbon;
            phi15 = 2.94d + 0.83d * _topClay - 0.0054d * Math.Pow(_topClay, 2d);

            phi05 = Max(0d, phi05);
            phi05 = Min(100d, phi05);

            phi02 = Max(0d, phi02);
            phi02 = Min(phi05, phi02);

            phi15 = Max(0d, phi15);
            phi15 = Min(phi02, phi15);

            // All land cover types can access all the water
            // held in the top soil (0-300mm):
            am02 = am02 + (phi05 - phi02) * 0.01d * 300d;
            am15 = am15 + (phi02 - phi15) * 0.01d * 300d;

            // Sub soil water content:
            phi05 = 37.2d + 0.35d * _subClay + 0.12d * _subSilt - 11.73d * _subDensity;
            phi02 = 7.57d + 0.48d * _subClay + 0.11d * _subSilt;
            phi15 = 1.48d + 0.84d * _subClay - 0.0054d * Math.Pow(_subClay, 2d);

            phi05 = Max(0d, phi05);
            phi05 = Min(100d, phi05);

            phi02 = Max(0d, phi02);
            phi02 = Min(phi05, phi02);

            phi15 = Max(0d, phi15);
            phi15 = Min(phi02, phi15);

        }

        // Sub soil water content is dependent upon the depth
        // of rooting (300-?mm):
        if (_euniceTexture)
        {
            switch (_landCover)
            {
                case LandCover.BareSoil:
                    {
                        am02 = am02 + _subSoilAWCEasy * 0.01d * 0d;
                        am15 = am15 + (_subSoilAWCHard - _subSoilAWCEasy) * 0.01d * 0d;
                        break;
                    }
                case LandCover.ManagedGrass:
                    {
                        am02 = am02 + _subSoilAWCEasy * 0.01d * 700d;
                        am15 = am15 + (_subSoilAWCHard - _subSoilAWCEasy) * 0.01d * 400d;
                        break;
                    }
                case LandCover.WinterWheat:
                    {
                        am02 = am02 + _subSoilAWCEasy * 0.01d * 900d;
                        am15 = am15 + (_subSoilAWCHard - _subSoilAWCEasy) * 0.01d * 200d;
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
                        am02 = am02 + (phi05 - phi02) * 0.01d * 0d;
                        am15 = am15 + (phi02 - phi15) * 0.01d * 0d;
                        break;
                    }
                case LandCover.ManagedGrass:
                    {
                        am02 = am02 + (phi05 - phi02) * 0.01d * 700d;
                        am15 = am15 + (phi02 - phi15) * 0.01d * 400d;
                        break;
                    }
                case LandCover.WinterWheat:
                    {
                        am02 = am02 + (phi05 - phi02) * 0.01d * 900d;
                        am15 = am15 + (phi02 - phi15) * 0.01d * 200d;
                        break;
                    }
            }
        }
    }

    private static double Max(double lhs, double rhs)
    {
        double maxRet = default;
        if (lhs > rhs)
        {
            maxRet = lhs;
        }
        else
        {
            maxRet = rhs;
        }

        return maxRet;
    }

    private static double Min(double lhs, double rhs)
    {
        double MinRet = default;
        if (lhs < rhs)
        {
            MinRet = lhs;
        }
        else
        {
            MinRet = rhs;
        }

        return MinRet;
    }

    // -- Public calculation functions -------------------------------------------------------

    public void Calculate()
    {
        var potentialEvapotranspiration = new double[12];
        var canopyEfficiency = new double[12];

        CalculatePotentialEvapotranspirationForYear(potentialEvapotranspiration, canopyEfficiency);

        var rainfall = new double[12];
        var surfaceRunOff = new double[12];
        CalculateEffectiveRainfallAndRunoff(potentialEvapotranspiration, canopyEfficiency, rainfall, surfaceRunOff);

        var thomasResults = CalculateThomasBookKeeping(rainfall, surfaceRunOff, potentialEvapotranspiration);
        StoreWaterBalanceOutputs(thomasResults);

        CalculateSoilTemperature();
        _dirty = false;
    }

    private sealed record ThomasBookKeepingResult(double[] ActualEvapotranspiration, double[] SoilMoistureDeficit, double[] SoilDrainage, double AM02, double AM15);

    private void CalculatePotentialEvapotranspirationForYear(double[] potentialEvapotranspiration, double[] canopyEfficiency)
    {
        for (int month = 0; month <= 11; month++)
        {
            CalculatePotentialEvapotranspiration(month, ref potentialEvapotranspiration[month], ref canopyEfficiency[month], ref _delta[month], ref _radiation[month]);
            _potentialEvapotranspiration[month] = potentialEvapotranspiration[month] * 30d;
        }
    }

    private void CalculateEffectiveRainfallAndRunoff(double[] potentialEvapotranspiration, double[] canopyEfficiency, double[] rainfall, double[] surfaceRunOff)
    {
        var interception = new double[12];

        for (int month = 0; month <= 11; month++)
        {
            interception[month] = 0d;
            surfaceRunOff[month] = 0d;

            double alpha = 1d + _rainfall[month] / _rainDays[month];
            double beta = _rainfall[month] / _rainDays[month] / alpha;
            double quantile = 0d;

            for (double probability = 0.005d; probability <= 0.995d; probability += 0.01d)
            {
                CalculateGammaQuantile(probability, beta, alpha, ref quantile);
                quantile = Max(quantile, 0.1d);

                double intercept = Min(quantile, _leafAreaIndex[month] * 0.2d);
                intercept = Min(potentialEvapotranspiration[month] * canopyEfficiency[month], intercept);

                interception[month] += intercept;
                quantile -= intercept;

                double runOff = CalculateRunOff(month, quantile);
                surfaceRunOff[month] += runOff;
            }

            double rainDaysFactor = _rainDays[month] / 100d;
            surfaceRunOff[month] *= rainDaysFactor;
            interception[month] *= rainDaysFactor;

            rainfall[month] = _rainfall[month] - surfaceRunOff[month] - interception[month];
            potentialEvapotranspiration[month] = _potentialEvapotranspiration[month] - interception[month] / canopyEfficiency[month];

            _surfaceRunOff[month] = surfaceRunOff[month];
            _canopyEvaporation[month] = interception[month];
        }
    }

    private ThomasBookKeepingResult CalculateThomasBookKeeping(double[] rainfall, double[] surfaceRunOff, double[] potentialEvapotranspiration)
    {
        var am02 = default(double);
        var am15 = default(double);
        CalculateAvailableWater(ref am02, ref am15);

        var actualEvapotranspiration = new double[12];
        var soilMoistureDeficit = new double[12];
        var soilDrainage = new double[12];
        var drainage = default(double);

        var workingSet = new ThomasBookKeepingWorkingSet
        {
            AM02 = am02,
            AM15 = am15,
            Rainfall = rainfall,
            SurfaceRunOff = surfaceRunOff,
            PotentialEvapotranspiration = potentialEvapotranspiration,
            ActualEvapotranspiration = actualEvapotranspiration,
            SoilMoistureDeficit = soilMoistureDeficit,
            SoilDrainage = soilDrainage,
            Drainage = drainage
        };

        for (int month = 0; month <= 11; month++)
        {
            soilMoistureDeficit[month] = 0d;
            soilDrainage[month] = 0d;
        }

        for (int iteration = 0; iteration <= 999; iteration++)
        {
            for (int month = 0; month <= 11; month++)
            {
                ApplyThomasBookKeepingForMonth(month, workingSet);
            }
        }

        return new ThomasBookKeepingResult(actualEvapotranspiration, soilMoistureDeficit, soilDrainage, am02, am15);
    }

    private sealed class ThomasBookKeepingWorkingSet
    {
        public required double AM02 { get; init; }
        public required double AM15 { get; init; }
        public required double[] Rainfall { get; init; }
        public required double[] SurfaceRunOff { get; init; }
        public required double[] PotentialEvapotranspiration { get; init; }
        public required double[] ActualEvapotranspiration { get; init; }
        public required double[] SoilMoistureDeficit { get; init; }
        public required double[] SoilDrainage { get; init; }
        public required double Drainage { get; init; }
    }

    private void ApplyThomasBookKeepingForMonth(
        int month,
        ThomasBookKeepingWorkingSet workingSet)
    {
        double tmp_B = workingSet.AM02 + workingSet.AM15 + 25d;
        double tmp_A = 1d;

        int index = month - 1;
        if (index == -1)
            index = 11;

        double tmp_W = workingSet.Rainfall[month] + workingSet.SoilMoistureDeficit[index] - workingSet.SurfaceRunOff[month];
        double tmp_Y = (tmp_W + tmp_B) / (2d * tmp_A) - Math.Pow(Math.Pow((tmp_W + tmp_B) / (2d * tmp_A), 2d) - tmp_W * tmp_B / tmp_A, 0.5d);

        workingSet.SoilMoistureDeficit[month] = tmp_Y * Math.Exp(-1 * workingSet.PotentialEvapotranspiration[month] / tmp_B);
        workingSet.SoilDrainage[month] = tmp_W - tmp_Y;
        workingSet.ActualEvapotranspiration[month] = tmp_Y - workingSet.SoilMoistureDeficit[month];

        workingSet.SoilMoistureDeficit[month] = workingSet.SoilMoistureDeficit[index] - workingSet.ActualEvapotranspiration[month];

#pragma warning disable S4143
        workingSet.SoilMoistureDeficit[month] = workingSet.SoilMoistureDeficit[month] + workingSet.Rainfall[month] - workingSet.SurfaceRunOff[month];
#pragma warning restore S4143

        if (workingSet.SoilMoistureDeficit[month] > tmp_B)
        {
            workingSet.SoilDrainage[month] = workingSet.SoilMoistureDeficit[month] - tmp_B;
            workingSet.SoilMoistureDeficit[month] = tmp_B;
        }
        else
        {
            workingSet.SoilDrainage[month] = 0d;
        }

        double tmp_Ratio = GetRainDays(month) / 30d;
        workingSet.SoilDrainage[month] = tmp_Ratio * workingSet.SoilDrainage[month] + (1d - tmp_Ratio) * (tmp_W - tmp_Y);

        workingSet.SoilMoistureDeficit[month] = workingSet.SoilMoistureDeficit[index] + workingSet.Rainfall[month] - workingSet.SurfaceRunOff[month] - workingSet.ActualEvapotranspiration[month] - workingSet.SoilDrainage[month];

        if (workingSet.SoilMoistureDeficit[month] > tmp_B)
        {
            workingSet.SoilDrainage[month] = workingSet.SoilDrainage[month] + (workingSet.SoilMoistureDeficit[month] - tmp_B);
            workingSet.SoilMoistureDeficit[month] = tmp_B;
        }

        workingSet.SoilDrainage[month] = workingSet.SoilDrainage[month] + workingSet.SurfaceRunOff[month];

        if (Math.Abs(workingSet.SoilDrainage[month] - workingSet.Drainage) > 1d)
        {
            // need to wright some code here
        }
    }

    private void StoreWaterBalanceOutputs(ThomasBookKeepingResult result)
    {
        for (int month = 0; month <= 11; month++)
        {
            if (result.SoilDrainage[month] < 0d)
            {
                result.SoilDrainage[month] = 0d;
            }

            result.SoilMoistureDeficit[month] = result.AM02 + result.AM15 + 25d - result.SoilMoistureDeficit[month];
#pragma warning disable S4143
            result.SoilMoistureDeficit[month] = Math.Round(result.SoilMoistureDeficit[month], 1);
#pragma warning restore S4143
            _soilMoistureDeficit[month] = result.SoilMoistureDeficit[month];

            result.SoilDrainage[month] = Math.Round(result.SoilDrainage[month], 1);
            _soilDrainage[month] = result.SoilDrainage[month];

            result.ActualEvapotranspiration[month] = Math.Round(result.ActualEvapotranspiration[month], 1);
            _actualEvapotranspiration[month] = result.ActualEvapotranspiration[month];
        }
    }

    // -- Private class constructor / destructor ---------------------------------------------
    private void InitialiseParams()
    {
        InitialiseArray(_diurnalAve, new[] { 0.488, 0.481, 0.474, 0.454, 0.469, 0.481, 0.473, 0.261, 0.453, 0.472, 0.478, 0.5 });
        InitialiseArray(_diurnalAmpOne, new[] { -0.232, -0.305, -0.359, -0.389, -0.416, -0.41, -0.429, -0.416, -0.385, -0.332, -0.278, -0.191 });
        InitialiseArray(_diurnalPhaseOne, new[] { -0.768, -0.821, -0.819, -0.957, -0.977, -0.974, -1.072, -1.052, -0.964, -0.924, -0.663, -0.791 });
        InitialiseArray(_diurnalAmpTwo, new[] { 0.109, 0.129, 0.109, 0.071, 0.034, 0.014, 0.015, 0.053, 0.106, 0.124, 0.129, 0.105 });
        InitialiseArray(_diurnalPhaseTwo, new[] { -0.917, -0.945, -0.966, -1.202, -1.117, -0.886, -1.065, -1.207, -1.223, -0.164, -0.765, -0.864 });

        for (int month = 0; month < 12; month++)
        {
            CalculateDiurnalMinMax(month);
        }
    }

    private static void InitialiseArray(double[] target, double[] source)
    {
        for (int i = 0; i < source.Length; i++)
        {
            target[i] = source[i];
        }
    }

    private void CalculateDiurnalMinMax(int month)
    {
        double min = double.MaxValue;
        double max = double.MinValue;

        for (int hour = 1; hour <= 24; hour++)
        {
            double value = CalculateDiurnalValue(month, hour);

            if (value > max) max = value;
            if (value < min) min = value;
        }

        _diurnalMax[month] = max;
        _diurnalMin[month] = min;
    }

    private double CalculateDiurnalValue(int month, int hour)
    {
        return _diurnalAve[month]
            + Math.Cos(hour * 2d * PI / 24d + _diurnalPhaseOne[month]) * _diurnalAmpOne[month]
            + Math.Cos(hour * 2d * PI / 12d + _diurnalPhaseTwo[month]) * _diurnalAmpTwo[month];
    }

    private void ClassInitializeRenamed()
    {
        InitialiseParams();
        Clear();

    }
    public MeanClimateDrainageModel(Texture texture = default) : base()
    {
        ClassInitializeRenamed();
        _texture = texture;
    }

    private void ClassTerminateRenamed()
    {
        Clear();
    }
    ~MeanClimateDrainageModel()
    {
        ClassTerminateRenamed();
    }
}
