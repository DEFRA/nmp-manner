using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Manner.Application.Calculators;

public class ClimateCalculator
{
    public ClimateMonths Rain { get; set; }
    public ClimateMonths SoilMoistureDeficit { get; set; }
    public ClimateMonths ActualEvapotranspiration { get; set; }
    public ClimateMonths PotentialEvapotranspiration { get; set; }
    public ClimateMonths SoilDrainage { get; set; }

    public ClimateCalculator()
    {
        SoilMoistureDeficit = new ClimateMonths();
        Rain = new ClimateMonths();
        ActualEvapotranspiration = new ClimateMonths();
        PotentialEvapotranspiration = new ClimateMonths();
        SoilDrainage = new ClimateMonths(); // Dec 2012 - Soil drainage code from Eish
        mcdm = new MeanClimateDrainageModel();
    }
    protected internal MeanClimateDrainageModel mcdm { get; private set; } 

    public void GetClimate(ClimateDto climate, int cropTypeID, int topSoilID, int subSoilID, double topSoilAWC, double subSoilAWC, bool hasOwnClimate = false)
    {

        bool canproc;
        double latitude;
        //double TopSoilAWC, SubSoilAWC;
        //var objManData = new MannerLib.MannerData();
        int k;
        //var ds = new DataSet();
        var northing = default(double);
        northing = climate.North;
        canproc = true;

        try
        {

            if (!hasOwnClimate)
            {

                // ds = GetClimateDataset("MannerClimateData.xml", postcode, ref northing);

                if (canproc)
                {
                    mcdm.RetrieveClimate(climate);
                }

            }

            // Set default
            double argdepth = 1.22d;
            mcdm.SetObservation(ref argdepth, 9);

            // Set default diffusivity:
            mcdm.Diffusivity = (double)-1d;

            // Estimate latitude from northing
            latitude = Math.Round(49.81d + 0.00000913d * northing, 1);

            // Set location attributes:
            mcdm.SetLocation(latitude, mcdm.Altitude);

            // Set the land cover
            if (cropTypeID == (int)Enums.Enumerations.CropTypeEnum.Grass)
            {
                mcdm.SetLandCover(MeanClimateDrainageModel.LandCover.ManagedGrass);
            }
            else
            {
                mcdm.SetLandCover(MeanClimateDrainageModel.LandCover.WinterWheat);
            }

            mcdm.EuniceTexture = true;

            // Set the topsoil and subsoil AWC


            //TopSoilAWC = Convert.ToDouble(objManData.GetDataField(RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "TopsoilAWC", (int)TopSoil));
            //SubSoilAWC =  Convert.ToDouble(objManData.GetDataField(RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "SubsoilAWC", (int)SubSoil));


            mcdm.SetAWC(topSoilAWC, subSoilAWC);

            // Set the topsoil percentage AWC for the soil texture
            // Figures from the IRRIGUIDE look up table
            switch (topSoilID)
            {

                case (int)Enums.Enumerations.SoilType.Sand:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.Sand, (short)12, (short)8);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.LoamySand:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.LoamySand, (short)13, (short)9);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandyLoam:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.SandyLoam, (short)17, (short)11);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandySiltLoam:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.SandySiltLoam, (short)19, (short)11);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SiltLoam:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.SiltLoam, (short)23, (short)15);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SiltyClayLoam:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.SiltyClayLoam, (short)19, (short)10);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandyClayLoam:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.SandyClayLoam, (short)17, (short)11);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.ClayLoam:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.ClayLoam, (short)18, (short)11);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SiltyClay:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.SiltyClay, (short)17, (short)10);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Clay:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.Clay, (short)17, (short)10);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.FineSandyLoam:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.FineSandyLoam, (short)18, (short)13);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandyClay:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.SandyClay, (short)17, (short)11);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Peat:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.Peat, (short)33, (short)26);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Peaty:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.Peaty, (short)27, (short)18);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Organic:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.Organic, (short)28, (short)20);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Chalk:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.Chalk, (short)22, (short)14);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.RocknotChalk:
                    {
                        mcdm.SetPercentTopsoilAWC(MeanClimateDrainageModel.Texture.RocknotChalk, (short)10, (short)5);
                        break;
                    }

                default:
                    {
                        break;
                    }
                    // shouldn't get here but use the default values if we do

            }


            // Set the subsoil percentage AWC for the soil texture 
            // Figures from the IRRIGUIDE look up table

            switch (subSoilID)
            {
                case (int)Enums.Enumerations.SoilType.Sand:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.Sand, (short)7, (short)5);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.LoamySand:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.LoamySand, (short)9, (short)6);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandyLoam:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.SandyLoam, (short)15, (short)11);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandySiltLoam:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.SandySiltLoam, (short)17, (short)11);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SiltLoam:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.SiltLoam, (short)22, (short)14);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SiltyClayLoam:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.SiltyClayLoam, (short)17, (short)10);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandyClayLoam:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.SandyClayLoam, (short)15, (short)10);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.ClayLoam:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.ClayLoam, (short)16, (short)10);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SiltyClay:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.SiltyClay, (short)15, (short)8);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Clay:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.Clay, (short)16, (short)8);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.FineSandyLoam:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.FineSandyLoam, (short)18, (short)13);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.SandyClay:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.SandyClay, (short)15, (short)10);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Peat:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.Peat, (short)33, (short)26);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Peaty:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.Peaty, (short)27, (short)18);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Organic:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.Organic, (short)28, (short)20);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.Chalk:
                    {
                        // These values are a bit flaky but come out at similar values to Eunice's texture
                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.Chalk, (short)27, (short)13);
                        break;
                    }

                case (int)Enums.Enumerations.SoilType.RocknotChalk:
                    {

                        mcdm.SetPercentSubsoilAWC(MeanClimateDrainageModel.Texture.RocknotChalk, (short)10, (short)5);
                        break;
                    }

                default:
                    {
                        break;
                    }
                    // shouldn't get here but use the default values if we do

            }

            for (k = 0; k <= 11; k++)
            {

                switch (k)
                {

                    case 0:
                        {
                            SoilMoistureDeficit.January = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.January = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.January = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.January = mcdm.GetRainfall(k);
                            SoilDrainage.January = mcdm.GetSoilDrainage(k); // Dec 2012 - Soil drainage code from Eish
                            break;
                        }
                    case 1:
                        {
                            SoilMoistureDeficit.February = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.February = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.February = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.February = mcdm.GetRainfall(k);
                            SoilDrainage.February = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 2:
                        {
                            SoilMoistureDeficit.March = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.March = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.March = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.March = mcdm.GetRainfall(k);
                            SoilDrainage.March = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 3:
                        {
                            SoilMoistureDeficit.April = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.April = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.April = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.April = mcdm.GetRainfall(k);
                            SoilDrainage.April = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 4:
                        {
                            SoilMoistureDeficit.May = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.May = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.May = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.May = mcdm.GetRainfall(k);
                            SoilDrainage.May = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 5:
                        {
                            SoilMoistureDeficit.June = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.June = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.June = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.June = mcdm.GetRainfall(k);
                            SoilDrainage.June = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 6:
                        {
                            SoilMoistureDeficit.July = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.July = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.July = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.July = mcdm.GetRainfall(k);
                            SoilDrainage.July = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 7:
                        {
                            SoilMoistureDeficit.August = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.August = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.August = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.August = mcdm.GetRainfall(k);
                            SoilDrainage.August = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 8:
                        {
                            SoilMoistureDeficit.September = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.September = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.September = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.September = mcdm.GetRainfall(k);
                            SoilDrainage.September = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 9:
                        {
                            SoilMoistureDeficit.October = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.October = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.October = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.October = mcdm.GetRainfall(k);
                            SoilDrainage.October = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 10:
                        {
                            SoilMoistureDeficit.November = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.November = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.November = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.November = mcdm.GetRainfall(k);
                            SoilDrainage.November = mcdm.GetSoilDrainage(k);
                            break;
                        }
                    case 11:
                        {
                            SoilMoistureDeficit.December = mcdm.GetSoilMoistureDeficit(k);
                            PotentialEvapotranspiration.December = mcdm.GetPotentialEvapotranspiration(k);
                            ActualEvapotranspiration.December = mcdm.GetActualEvapotranspiration(k) + mcdm.GetCanopyEvaporation(k);
                            Rain.December = mcdm.GetRainfall(k);
                            SoilDrainage.December = mcdm.GetSoilDrainage(k);
                            break;
                        }
                }

            }
        }


        catch (Exception)
        {
            throw;
        }

    }

}
