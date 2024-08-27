using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Manner.Application.MannerLib;

public class Climate
{

    public ClimateMonths Rain { get; set; }
    public ClimateMonths SoilMoistureDeficit { get; set; }
    public ClimateMonths ActualEvapotranspiration { get; set; }
    public ClimateMonths PotentialEvapotranspiration { get; set; }
    public ClimateMonths SoilDrainage { get; set; }

    public Climate()
    {
        SoilMoistureDeficit = new ClimateMonths();
        Rain = new ClimateMonths();
        ActualEvapotranspiration = new ClimateMonths();
        PotentialEvapotranspiration = new ClimateMonths();
        SoilDrainage = new ClimateMonths(); // Dec 2012 - Soil drainage code from Eish
    }

    protected internal MCDM MCDMObject { get; private set; } = new MCDM();

    public void GetClimate(int RunType, string postcode, MannerLib.Enumerations.CropTypeEnum croptype, MannerLib.Enumerations.SoilType TopSoil, MannerLib.Enumerations.SoilType SubSoil, bool HasOwnClimate = false)
    {

        bool canproc;
        double dLatitude;
        double TopSoilAWC, SubSoilAWC;
        var objManData = new MannerLib.MannerData();
        int k;
        var ds = new DataSet();
        var northing = default(double);

        canproc = true;

        try
        {

            if (!HasOwnClimate)
            {

                ds = GetClimateDataset("MannerClimateData.xml", postcode, ref northing);

                if (canproc)
                {
                    MCDMObject.RetrieveClimate(ds, postcode);
                }

            }

            // Set default
            double argdepth = 1.22d;
            MCDMObject.SetObservation(ref argdepth, 9);

            // Set default diffusivity:
            MCDMObject.Diffusivity = (double)-1;

            // Estimate latitude from northing
            dLatitude = Math.Round(49.81d + 0.00000913d * northing, 1);

            // Set location attributes:
            MCDMObject.SetLocation(dLatitude, MCDMObject.Altitude);

            // Set the land cover
            if (croptype == MannerLib.Enumerations.CropTypeEnum.Grass)
            {
                MCDMObject.SetLandCover(MCDM.LandCover.ManagedGrass);
            }
            else
            {
                MCDMObject.SetLandCover(MCDM.LandCover.WinterWheat);
            }

            MCDMObject.EuniceTexture = true;

            // Set the topsoil and subsoil AWC


            TopSoilAWC = Convert.ToDouble(objManData.GetDataField(RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "TopsoilAWC", (int)TopSoil));
            SubSoilAWC = Convert.ToDouble(objManData.GetDataField(RunType, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "SubsoilAWC", (int)SubSoil));


            MCDMObject.SetAWC(TopSoilAWC, SubSoilAWC);

            // Set the topsoil percentage AWC for the soil texture
            // Figures from the IRRIGUIDE look up table
            switch (TopSoil)
            {

                case MannerLib.Enumerations.SoilType.Sand:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Sand, (short)12, (short)8);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.LoamySand:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.LoamySand, (short)13, (short)9);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandyLoam, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandySiltLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandySiltLoam, (short)19, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SiltLoam, (short)23, (short)15);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClayLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SiltyClayLoam, (short)19, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClayLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandyClayLoam, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.ClayLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.ClayLoam, (short)18, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClay:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SiltyClay, (short)17, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Clay:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Clay, (short)17, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.FineSandyLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.FineSandyLoam, (short)18, (short)13);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClay:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandyClay, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peat:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Peat, (short)33, (short)26);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peaty:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Peaty, (short)27, (short)18);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Organic:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Organic, (short)28, (short)20);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Chalk:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Chalk, (short)22, (short)14);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.RocknotChalk:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.RocknotChalk, (short)10, (short)5);
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

            switch (SubSoil)
            {
                case MannerLib.Enumerations.SoilType.Sand:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Sand, (short)7, (short)5);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.LoamySand:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.LoamySand, (short)9, (short)6);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandyLoam, (short)15, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandySiltLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandySiltLoam, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SiltLoam, (short)22, (short)14);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClayLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SiltyClayLoam, (short)17, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClayLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandyClayLoam, (short)15, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.ClayLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.ClayLoam, (short)16, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClay:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SiltyClay, (short)15, (short)8);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Clay:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Clay, (short)16, (short)8);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.FineSandyLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.FineSandyLoam, (short)18, (short)13);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClay:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandyClay, (short)15, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peat:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Peat, (short)33, (short)26);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peaty:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Peaty, (short)27, (short)18);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Organic:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Organic, (short)28, (short)20);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Chalk:
                    {
                        // These values are a bit flaky but come out at similar values to Eunice's texture
                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Chalk, (short)27, (short)13);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.RocknotChalk:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.RocknotChalk, (short)10, (short)5);
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
                            SoilMoistureDeficit.January = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.January = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.January = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.January = MCDMObject.get_Rainfall(k);
                            SoilDrainage.January = MCDMObject.get_SoilDrainage(k); // Dec 2012 - Soil drainage code from Eish
                            break;
                        }
                    case 1:
                        {
                            SoilMoistureDeficit.February = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.February = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.February = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.February = MCDMObject.get_Rainfall(k);
                            SoilDrainage.February = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 2:
                        {
                            SoilMoistureDeficit.March = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.March = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.March = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.March = MCDMObject.get_Rainfall(k);
                            SoilDrainage.March = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 3:
                        {
                            SoilMoistureDeficit.April = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.April = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.April = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.April = MCDMObject.get_Rainfall(k);
                            SoilDrainage.April = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 4:
                        {
                            SoilMoistureDeficit.May = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.May = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.May = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.May = MCDMObject.get_Rainfall(k);
                            SoilDrainage.May = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 5:
                        {
                            SoilMoistureDeficit.June = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.June = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.June = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.June = MCDMObject.get_Rainfall(k);
                            SoilDrainage.June = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 6:
                        {
                            SoilMoistureDeficit.July = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.July = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.July = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.July = MCDMObject.get_Rainfall(k);
                            SoilDrainage.July = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 7:
                        {
                            SoilMoistureDeficit.August = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.August = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.August = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.August = MCDMObject.get_Rainfall(k);
                            SoilDrainage.August = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 8:
                        {
                            SoilMoistureDeficit.September = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.September = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.September = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.September = MCDMObject.get_Rainfall(k);
                            SoilDrainage.September = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 9:
                        {
                            SoilMoistureDeficit.October = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.October = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.October = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.October = MCDMObject.get_Rainfall(k);
                            SoilDrainage.October = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 10:
                        {
                            SoilMoistureDeficit.November = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.November = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.November = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.November = MCDMObject.get_Rainfall(k);
                            SoilDrainage.November = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                    case 11:
                        {
                            SoilMoistureDeficit.December = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.December = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.December = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.December = MCDMObject.get_Rainfall(k);
                            SoilDrainage.December = MCDMObject.get_SoilDrainage(k);
                            break;
                        }
                }

            }
        }


        catch (Exception ex)
        {
            throw ex;
        }

    }

    public void GetClimate(int runAs, int easting, int northing, MannerLib.Enumerations.CropTypeEnum croptype, MannerLib.Enumerations.SoilType topsoil, MannerLib.Enumerations.SoilType subsoil, bool hasOwnClimate = false)
    {

        bool canproc;
        double dLatitude;
        double TopsoilAWC, SubsoilAWC;
        var objManData = new MannerLib.MannerData();
        int k;
        var ds = new DataSet();

        canproc = true;

        try
        {
            if (!hasOwnClimate)
            {

                ds = GetClimateDataset("MannerClimateData.xml", easting, northing);

                if (canproc)
                {
                    MCDMObject.RetrieveClimate(ds, easting, northing);
                }
            }

            // Set default
            double argdepth = 1.22d;
            MCDMObject.SetObservation(ref argdepth, 9);

            // Set default diffusivity:
            MCDMObject.Diffusivity = (double)-1;

            // Estimate latitude from northing
            dLatitude = Math.Round(49.81d + 0.00000913d * northing, 1);

            // Set location attributes:
            MCDMObject.SetLocation(dLatitude, MCDMObject.Altitude);

            // Set the land cover
            if (croptype == MannerLib.Enumerations.CropTypeEnum.Grass)
            {
                MCDMObject.SetLandCover(MCDM.LandCover.ManagedGrass);
            }
            else
            {
                MCDMObject.SetLandCover(MCDM.LandCover.WinterWheat);
            }

            MCDMObject.EuniceTexture = true;

            // Set the topsoil and subsoil AWC
            TopsoilAWC = Convert.ToDouble(objManData.GetDataField(runAs, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "TopsoilAWC", (int)topsoil));
            SubsoilAWC = Convert.ToDouble(objManData.GetDataField(runAs, MannerLib.MannerData.XmlLookups.Soil, "SoilID", "SubsoilAWC", (int)subsoil));


            MCDMObject.SetAWC(TopsoilAWC, SubsoilAWC);

            // Set the topsoil percentage AWC for the soil texture
            // Figures from the IRRIGUIDE look up table
            switch (topsoil)
            {

                case MannerLib.Enumerations.SoilType.Sand:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Sand, (short)12, (short)8);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.LoamySand:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.LoamySand, (short)13, (short)9);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandyLoam, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandySiltLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandySiltLoam, (short)19, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SiltLoam, (short)23, (short)15);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClayLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SiltyClayLoam, (short)19, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClayLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandyClayLoam, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.ClayLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.ClayLoam, (short)18, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClay:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SiltyClay, (short)17, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Clay:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Clay, (short)17, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.FineSandyLoam:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.FineSandyLoam, (short)18, (short)13);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClay:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.SandyClay, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peat:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Peat, (short)33, (short)26);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peaty:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Peaty, (short)27, (short)18);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Organic:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Organic, (short)28, (short)20);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Chalk:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.Chalk, (short)22, (short)14);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.RocknotChalk:
                    {
                        MCDMObject.SetPercentTopsoilAWC(MCDM.Texture.RocknotChalk, (short)10, (short)5);
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

            switch (subsoil)
            {
                case MannerLib.Enumerations.SoilType.Sand:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Sand, (short)7, (short)5);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.LoamySand:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.LoamySand, (short)9, (short)6);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandyLoam, (short)15, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandySiltLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandySiltLoam, (short)17, (short)11);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SiltLoam, (short)22, (short)14);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClayLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SiltyClayLoam, (short)17, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClayLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandyClayLoam, (short)15, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.ClayLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.ClayLoam, (short)16, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SiltyClay:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SiltyClay, (short)15, (short)8);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Clay:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Clay, (short)16, (short)8);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.FineSandyLoam:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.FineSandyLoam, (short)18, (short)13);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.SandyClay:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.SandyClay, (short)15, (short)10);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peat:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Peat, (short)33, (short)26);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Peaty:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Peaty, (short)27, (short)18);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Organic:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Organic, (short)28, (short)20);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.Chalk:
                    {
                        // These values are a bit flaky but come out at similar values to Eunice's texture
                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.Chalk, (short)27, (short)13);
                        break;
                    }

                case MannerLib.Enumerations.SoilType.RocknotChalk:
                    {

                        MCDMObject.SetPercentSubsoilAWC(MCDM.Texture.RocknotChalk, (short)10, (short)5);
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
                            SoilMoistureDeficit.January = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.January = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.January = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.January = MCDMObject.get_Rainfall(k);
                            break;
                        }

                    case 1:
                        {
                            SoilMoistureDeficit.February = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.February = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.February = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.February = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 2:
                        {
                            SoilMoistureDeficit.March = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.March = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.March = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.March = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 3:
                        {
                            SoilMoistureDeficit.April = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.April = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.April = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.April = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 4:
                        {
                            SoilMoistureDeficit.May = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.May = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.May = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.May = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 5:
                        {
                            SoilMoistureDeficit.June = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.June = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.June = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.June = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 6:
                        {
                            SoilMoistureDeficit.July = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.July = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.July = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.July = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 7:
                        {
                            SoilMoistureDeficit.August = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.August = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.August = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.August = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 8:
                        {
                            SoilMoistureDeficit.September = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.September = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.September = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.September = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 9:
                        {
                            SoilMoistureDeficit.October = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.October = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.October = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.October = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 10:
                        {
                            SoilMoistureDeficit.November = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.November = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.November = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.November = MCDMObject.get_Rainfall(k);
                            break;
                        }
                    case 11:
                        {
                            SoilMoistureDeficit.December = MCDMObject.get_SoilMoistureDeficit(k);
                            PotentialEvapotranspiration.December = MCDMObject.get_PotentialEvapotranspiration(k);
                            ActualEvapotranspiration.December = MCDMObject.get_ActualEvapotranspiration(k) + MCDMObject.get_CanopyEvaporation(k);
                            Rain.December = MCDMObject.get_Rainfall(k);
                            break;
                        }

                }

            }
        }


        catch (Exception ex)
        {
            throw ex;
        }

    }


    private DataSet GetClimateDataset(string Filename, string postcode, ref double northing)
    {
        var xmlDoc = new XmlDocument();
        XmlNamespaceManager nsMgr;
        XmlNodeList selectedNodes;
        var ds = new DataSet();
        System.IO.StringReader stream;

        xmlDoc.Load(ResourceXML(Filename));

        nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);

        selectedNodes = xmlDoc.SelectNodes("//row[@POSTCODE=\"" + postcode + "\"]", nsMgr);

        if (selectedNodes.Count == 0)
        {
            throw new Exception("Postcode not found. Please enter a valid postcode.");
        }

        northing = Convert.ToDouble(selectedNodes[0].Attributes["NORTH"].Value);

        foreach (XmlNode selectedNode in selectedNodes)
        {
            stream = new System.IO.StringReader(selectedNode.OuterXml);
            ds.ReadXml(stream);
        }

        return ds;
    }

    private DataSet GetClimateDataset(string Filename, int easting, int northing)
    {
        var xmlDoc = new XmlDocument();
        XmlNamespaceManager nsMgr;
        XmlNodeList selectedNodes;
        var ds = new DataSet();
        int ID;
        decimal dEasting, dnorthing;
        System.IO.StringReader stream;

        dEasting = (decimal)(easting / 10000d);
        dnorthing = (decimal)(northing / 10000d);

        easting = (int)Math.Round(Math.Floor(dEasting));
        northing = (int)Math.Round(Math.Floor(dnorthing));

        ID = easting * 1000 + northing;

        xmlDoc.Load(ResourceXML(Filename));

        nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);

        selectedNodes = xmlDoc.SelectNodes("//row[@ID=\"" + ID + "\"]", nsMgr);

        if (selectedNodes.Count == 0)
        {
            throw new Exception("Cant find Easting and Northing in climate file ");
        }

        foreach (XmlNode selectedNode in selectedNodes)
        {
            stream = new System.IO.StringReader(selectedNode.OuterXml);
            ds.ReadXml(stream);
        }

        return ds;
    }

    private System.IO.Stream ResourceXML(string sEmbeddedResourceFile)
    {

        System.Reflection.Assembly thisExe;
        thisExe = System.Reflection.Assembly.GetExecutingAssembly();

        var @file = thisExe.GetManifestResourceStream(sEmbeddedResourceFile);

        return @file;

    }

}
