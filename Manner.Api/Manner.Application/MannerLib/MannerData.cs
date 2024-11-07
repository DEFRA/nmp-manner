using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Manner.Application.MannerLib
{
    public class MannerData
    {
        public MannerData()
        {

        }

        /// <summary>
        /// Enumerations for XML lookup files
        /// </summary>
        /// <remarks></remarks>
        public enum XmlLookups : int
        {
            ManureTypes = 1,
            ApplicationMethod = 2,
            ApplicationDelay = 3,
            IncorporationMethod = 4,
            Crops = 5,
            Soil = 6,
            SoilMoisture = 7,
            WindSpeed = 8,
            Rainfall = 9,
            Climate = 10,
            // ManureTypesOld = 11 Deprecated
            Validation = 12
        }

        /// <summary>
        /// Enumerations for climate attributes.
        /// </summary>
        /// <remarks></remarks>
        public enum ClimateType : int
        {
            Rainfall = 1,
            ActualEvapotranspiration = 2,
            SoilMoistureDefecit = 3,
            PotentialEvapotranspiration = 4
        }

        /// <summary>
        /// Enumerations of how Manner DLL is run, different manure types exist for each runAs setting
        /// </summary>
        /// <remarks></remarks>
        public enum RunAs : int
        {
            MannerEngland = 1,
            MannerScotland = 2,
            PlanetEngland = 3,
            PlanetScotland = 4
        }

        // TODO: create global list on startup and read that send that back each time.

        /// <summary>
        /// This method returns an Array list of manure types. The array list has two items the ID and Description.
        /// </summary>
        /// <param name="runAs"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetManureTypes(int runAs)
        {

            var ds = new DataSet();
            var dv = new DataView();
            var al = new ArrayList();

            try
            {

                ds.ReadXml(GetLookupListName(runAs, XmlLookups.ManureTypes));
                dv = ds.Tables[0].DefaultView;

                // Filters the manure types depending on how they are running manner.
                switch (runAs)
                {

                    case (int)RunAs.MannerEngland:
                        {
                            dv.RowFilter = "MannerEnglandDefaults = 1";
                            break;
                        }
                    case (int)RunAs.MannerScotland:
                        {
                            dv.RowFilter = "MannerScotlandDefaults = 1";
                            break;
                        }
                    case (int)RunAs.PlanetEngland:
                        {
                            dv.RowFilter = "PlanetEnglandDefaults = 1";
                            break;
                        }
                    case (int)RunAs.PlanetScotland:
                        {
                            dv.RowFilter = "PlanetScotlandDefaults = 1";
                            break;
                        }
                }

                // Add types to array list
                al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "ManureID", "ManureName").DefaultView);
            }

            catch (Exception)
            {

            }
            return al;

        }

        /// <summary>
        /// Returns one manure type
        /// </summary>
        /// <param name="runAs"></param>
        /// <param name="manureId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private DataView GetManureTypes(int runAs, int manureId)
        {

            var ds = new DataSet();
            var Dv = new DataView();

            try
            {
                ds.ReadXml(GetLookupListName(runAs, XmlLookups.ManureTypes));
                Dv = ds.Tables[0].DefaultView;
                Dv.RowFilter = "ManureID = " + manureId;
            }
            catch (Exception)
            {

            }
            return Dv;

        }

        /// <summary>
        /// returns the validation limits
        /// </summary>
        /// <param name="runAs"></param>
        /// <param name="manureId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private DataView GetValidationLimits(int runAs, int manureId)
        {

            var ds = new DataSet();
            var Dv = new DataView();

            try
            {
                ds.ReadXml(GetLookupListName(runAs, XmlLookups.Validation));
                Dv = ds.Tables[0].DefaultView;
                Dv.RowFilter = "ManureID = " + manureId;
            }
            catch (Exception)
            {

            }
            return Dv;

        }


        /// <summary>
        /// Returns the list of application methods.
        /// </summary>
        /// <param name="runAs"></param>
        /// <param name="manureCategory"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetApplicationMethodTypes(int runAs, int manureCategory)
        {

            var AppMethodDs = new DataSet();
            string sFilterString;
            var DV = new DataView();
            var al = new ArrayList();

            if (manureCategory == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry | manureCategory == (int)MannerLib.Enumerations.ManureCategory.PigSlurry | manureCategory == (int)MannerLib.Enumerations.ManureCategory.LiquidSludge)
            {
                sFilterString = "ManType = " + ((int)MannerLib.Enumerations.ManureCategory.PigSlurry).ToString();
            }

            else
            {
                sFilterString = "ManType = " + ((int)MannerLib.Enumerations.ManureCategory.Poultry).ToString();
            }

            AppMethodDs.ReadXml(GetLookupListName(runAs, XmlLookups.ApplicationMethod));

            DV = AppMethodDs.Tables[0].DefaultView;
            DV.RowFilter = sFilterString;
            DV.Sort = "OrderBy ASC";

            al = AddItemToArray(DV.ToTable(DV.Table.TableName, false, "ApplicationMethodID", "ApplicationMethodName").DefaultView);

            return al;

        }

        /// <summary>
        /// Returns a list of incorporation methods, this list is dependant on the type of application method that has been selected.
        /// </summary>
        /// <param name="runAs"></param>
        /// <param name="applicationMethod"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetApplicationMethodOfIncorporation(int runAs, int applicationMethod)
        {

            var dv = new DataView();
            var dsIncorporation = new DataSet();
            string sFilterString;
            var al = new ArrayList();

            try
            {

                switch (runAs)
                {

                    case (int)RunAs.MannerEngland:
                    case (int)RunAs.MannerScotland:
                    case (int)RunAs.PlanetEngland:
                    case (int)RunAs.PlanetScotland:
                        {

                            if (applicationMethod == (int)MannerLib.Enumerations.ApplicationMethodEnum.DeepInjection)
                            {

                                sFilterString = "IncorporationID IN (" + ((int)MannerLib.Enumerations.MethodOfIncorporationEnum.DeepInjection).ToString() + ")"; // 1 2
                            }

                            else if (applicationMethod == (int)MannerLib.Enumerations.ApplicationMethodEnum.ShallowInjection)
                            {

                                sFilterString = "IncorporationID IN (" + ((int)MannerLib.Enumerations.MethodOfIncorporationEnum.ShallowInjection).ToString() + ")"; // 1 2
                            }

                            else
                            {

                                sFilterString = "IncorporationID IN (" + ((int)MannerLib.Enumerations.MethodOfIncorporationEnum.Discs).ToString() + "," + ((int)MannerLib.Enumerations.MethodOfIncorporationEnum.MouldboardPlough).ToString() + "," + ((int)MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated).ToString() + "," + ((int)MannerLib.Enumerations.MethodOfIncorporationEnum.RotaryCultivator).ToString() + "," + ((int)MannerLib.Enumerations.MethodOfIncorporationEnum.TineCultivator).ToString() + ")";

                            } // (3, 4, 5, 6, 7)"

                            dsIncorporation.ReadXml(GetLookupListName(runAs, XmlLookups.IncorporationMethod));
                            dv = dsIncorporation.Tables[0].DefaultView;
                            dv.RowFilter = sFilterString;
                            dv.Sort = "OrderBy ASC";
                            break;
                        }
                }

                al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "IncorporationID", "IncorportaionName").DefaultView);
            }

            catch (Exception)
            {

            }
            return al;

        }

        /// <summary>
        /// Returns a list of Incorporation Delay,  this list is dependant on the type of application method that has been selected.
        /// </summary>
        /// <param name="runAs"></param>
        /// <param name="applicationMethod"></param>
        /// <param name="incorporationMethod"></param>
        /// <param name="manureEnum"></param>
        /// <param name="isManureLiquid"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetApplicationIncorporationDelay(int runAs, int applicationMethod, int incorporationMethod, int manureEnum, bool isManureLiquid = false)
        {
            var Dv = new DataView();
            var dsDelay = new DataSet();
            string sFilterString = "";
            var al = new ArrayList();

            try
            {

                if (incorporationMethod == (int)MannerLib.Enumerations.MethodOfIncorporationEnum.NotIncorporated)
                {
                    sFilterString = "ID IN (" + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.NotIncorporated).ToString() + ")";
                }

                else if (isManureLiquid)
                {
                    sFilterString = "ID IN (" + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.LessThan2Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n2To4Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n4To6Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n6To12Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n12To24Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n1To2Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n3To7Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.GreaterThan7Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.NotIncorporated).ToString() + ")";
                }
                else if (manureEnum == (int)MannerLib.Enumerations.ManureTypes.BroilerTurkeyLitter | manureEnum == (int)MannerLib.Enumerations.ManureTypes.PoultryManure)
                {
                    sFilterString = "ID IN (" + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.LessThan2Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n2To4Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n4To6Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n6To12Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n12To24Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n1To2Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n3To5Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n6To12Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n12To32Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.GreaterThan32Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.NotIncorporated).ToString() + ")";
                }
                else
                {
                    sFilterString = "ID IN (" + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.LessThan2Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n2To4Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n4To6Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n6To12Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n12To24Hours).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n1To2Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n3To5Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.n6To12Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.GreaterThan12Days).ToString() + ",";
                    sFilterString = sFilterString + ((int)MannerLib.Enumerations.DelayToIncorporationEnum.NotIncorporated).ToString() + ")";


                }

                dsDelay.ReadXml(GetLookupListName(runAs, XmlLookups.ApplicationDelay));
                Dv = dsDelay.Tables[0].DefaultView;
                Dv.RowFilter = sFilterString;
                al = AddItemToArray(Dv.ToTable(Dv.Table.TableName, false, "ID", "Delay").DefaultView);
            }

            catch (Exception ex)
            {

            }

            return al;


        }

        /// <summary>
        /// Returns a list of crop types
        /// </summary>
        /// <param name="runAs"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetCropTypes(int runAs)
        {
            var dv = new DataView();
            var dsCrops = new DataSet();
            var al = new ArrayList();

            dsCrops.ReadXml(GetLookupListName(runAs, XmlLookups.Crops));
            dv = dsCrops.Tables[0].DefaultView;
            al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "CropID", "CropName").DefaultView);

            return al;
        }

        /// <summary>
        /// Returns a list of top soils.
        /// </summary>
        /// <param name="runAs"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetTopsoil(int runAs)
        {
            var dv = new DataView();
            var DsTopSoil = new DataSet();
            string sFilterString = "";
            var al = new ArrayList();

            switch (runAs)
            {

                case (int)RunAs.MannerEngland:
                case (int)RunAs.MannerScotland:
                case (int)RunAs.PlanetEngland:
                case (int)RunAs.PlanetScotland:
                    {
                        sFilterString = "SoilID NOT IN (" + ((int)MannerLib.Enumerations.SoilType.RocknotChalk).ToString() + "," + ((int)MannerLib.Enumerations.SoilType.Chalk).ToString() + ")";
                        break;
                    }

            }

            DsTopSoil.ReadXml(GetLookupListName(runAs, XmlLookups.Soil));
            dv = DsTopSoil.Tables[0].DefaultView;
            dv.RowFilter = sFilterString;
            al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "SoilID", "Name").DefaultView);


            return al;
        }

        /// <summary>
        /// returns a list of sub soils.
        /// </summary>
        /// <param name="runAs"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetSubsoil(int runAs)
        {
            var dv = new DataView();
            var dsSubsoil = new DataSet();
            var al = new ArrayList();

            switch (runAs)
            {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        break;
                    }
            }

            dsSubsoil.ReadXml(GetLookupListName(runAs, XmlLookups.Soil));
            dv = dsSubsoil.Tables[0].DefaultView;
            al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "SoilID", "Name").DefaultView);

            return al;

        }

        /// <summary>
        /// returns a list of soil moisture
        /// </summary>
        /// <param name="runAs"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetSoilMoisture(int runAs)
        {
            var dv = new DataView();
            var dsSoilmoisture = new DataSet();
            var al = new ArrayList();

            switch (runAs)
            {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        break;
                    }
            }

            dsSoilmoisture.ReadXml(GetLookupListName(runAs, XmlLookups.SoilMoisture));
            dv = dsSoilmoisture.Tables[0].DefaultView;
            al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "ID", "Name").DefaultView);
            return al;

        }

        /// <summary>
        /// Returns a list of wind speed.
        /// </summary>
        /// <param name="runAs"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetWindSpeed(int runAs)
        {
            var dv = new DataView();
            var dsWindspeed = new DataSet();
            var al = new ArrayList();

            switch (runAs)
            {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        break;
                    }
            }

            dsWindspeed.ReadXml(GetLookupListName(runAs, XmlLookups.WindSpeed));
            dv = dsWindspeed.Tables[0].DefaultView;
            al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "WeatherConditionID", "WeatherConditionName").DefaultView);

            return al;

        }

        /// <summary>
        /// Returns a list of default rainfall conditions.
        /// </summary>
        /// <param name="runAs"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList GetRainfall(int runAs)
        {
            var dv = new DataView();
            var dsRainfall = new DataSet();
            var al = new ArrayList();

            switch (runAs)
            {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        break;
                    }
            }

            dsRainfall.ReadXml(GetLookupListName(runAs, XmlLookups.Rainfall));
            dv = dsRainfall.Tables[0].DefaultView;
            al = AddItemToArray(dv.ToTable(dv.Table.TableName, false, "RainfallID", "RainfallConditionName").DefaultView);

            return al;

        }

        /// <summary>
        /// This function returns the Manner Defaults from an XML file as an IO.Stream
        /// </summary>
        /// <param name="runAs"> </param>
        /// <param name="xmlLookupList"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private System.IO.Stream GetLookupListName(int runAs, XmlLookups xmlLookupList)
        {

            string lookuplistname = "";
            System.IO.Stream LookupFilestream;

            switch (runAs)
            {
                case (int)RunAs.MannerEngland:
                case (int)RunAs.MannerScotland:
                case (int)RunAs.PlanetEngland:
                case (int)RunAs.PlanetScotland:
                    {

                        switch (xmlLookupList)
                        {
                            case XmlLookups.ManureTypes:
                                {
                                    lookuplistname = "ManureTypesEngland.xml";
                                    break;
                                }
                            case XmlLookups.ApplicationMethod:
                                {
                                    lookuplistname = "ApplicationMethodEngland.xml";
                                    break;
                                }
                            case XmlLookups.ApplicationDelay:
                                {
                                    lookuplistname = "IncorporationDelayEngland.xml";
                                    break;
                                }
                            case XmlLookups.IncorporationMethod:
                                {
                                    lookuplistname = "IncorporationMethodsEngland.xml";
                                    break;
                                }
                            case XmlLookups.Crops:
                                {
                                    lookuplistname = "CropTypesEngland.xml";
                                    break;
                                }
                            case XmlLookups.Soil:
                                {
                                    lookuplistname = "SoilsEngland.xml";
                                    break;
                                }
                            case XmlLookups.SoilMoisture:
                                {
                                    lookuplistname = "SoilMoistureStatusEngland.xml";
                                    break;
                                }
                            case XmlLookups.WindSpeed:
                                {
                                    lookuplistname = "WindSpeedEngland.xml";
                                    break;
                                }
                            case XmlLookups.Rainfall:
                                {
                                    lookuplistname = "RainfallConditionsEngland.xml";
                                    break;
                                }
                            case XmlLookups.Climate:
                                {
                                    lookuplistname = "ClimateEngland.xml";
                                    break;
                                }
                            case XmlLookups.Validation:
                                {
                                    lookuplistname = "ValidationLimits.xml";
                                    break;
                                }
                        }

                        break;
                    }

                default:
                    {
                        throw new Exception("RunAs type not recognised");
                    }
            }

            LookupFilestream = ResourceXML(lookuplistname);

            return LookupFilestream;
        }

        /// <summary>
        /// This method accepts a manureID of a manure type and populates the Manner object that is passed in with the relevant manure type nutrient values.
        /// This method is likely to be called when a new manure type is selected.
        /// </summary>
        /// <param name="manureId"></param>
        /// <param name="manner"></param>
        /// <remarks></remarks>
        internal void PopulateManureType(int manureId, ref MannerLib.Manner manner)
        {

            var dv = new DataView();
            var ValidationLimits = new DataView();
            var ObjMantype = new MannerLib.ManureType();

            // Get default values from XML Files
            // Get manure types and properties
            dv = this.GetManureTypes(manner.RunType, manureId);

            // Get validation limits
            ValidationLimits = this.GetValidationLimits(manner.RunType, manureId);

            // Populate Manure properties
            ObjMantype.ManureID = Convert.ToInt32(dv[0]["ManureID"]);
            ObjMantype.LiquidOrSolid = Convert.ToString(dv[0]["LiquidOrSolid"]);
            ObjMantype.NMaxConst = Convert.ToDouble(dv[0]["NmaxConst"]);
            ObjMantype.ManureCategory = (MannerLib.Enumerations.ManureCategory)Convert.ToInt32(dv[0]["ManureCategory"]);
            ObjMantype.ManureCategoryUserInterface = (MannerLib.Enumerations.ManureCategoryUserInterface)Convert.ToInt32(dv[0]["ManureCategoryUserInterface"]);
            ObjMantype.ManureCategoryUserInterfaceText = Convert.ToString(dv[0]["ManureCategoryUserInterfaceText"]);
            ObjMantype.HighRan = Convert.ToString(dv[0]["HighRAN"]);

            // Set Dry Matter
            ObjMantype.DryMatter.Value = Convert.ToDouble(dv[0]["DryMatter"]);
            ObjMantype.DryMatter.Min = Convert.ToDouble(ValidationLimits[0]["DryMatterMin"]);
            ObjMantype.DryMatter.Max = Convert.ToDouble(ValidationLimits[0]["DryMatterMax"]);

            // Set Total N
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["TotalN"])))
            {
                ObjMantype.TotalN.Value = 0d;
                ObjMantype.TotalN.DisplayValue = "";
            }
            else
            {
                ObjMantype.TotalN.Value = Convert.ToDouble(dv[0]["TotalN"]);
                ObjMantype.TotalN.DisplayValue = Convert.ToString(dv[0]["TotalN"]);
            }
            ObjMantype.TotalN.Min = Convert.ToDouble(ValidationLimits[0]["TotalNMin"]);
            ObjMantype.TotalN.Max = Convert.ToDouble(ValidationLimits[0]["TotalNMax"]);

            // Set NH4-N
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["NH4-N"])))
            {
                ObjMantype.NH4N.Value = 0d;
                ObjMantype.NH4N.DisplayValue = "";
            }
            else
            {
                ObjMantype.NH4N.Value = Convert.ToDouble(dv[0]["NH4-N"]);
                ObjMantype.NH4N.DisplayValue = Convert.ToString(dv[0]["NH4-N"]);
            }
            ObjMantype.NH4N.AvailablePercent = Convert.ToInt32(dv[0]["NH4-N_Available"]);
            ObjMantype.NH4N.Min = Convert.ToDouble(ValidationLimits[0]["NH4-NMin"]);
            ObjMantype.NH4N.Max = Convert.ToDouble(ValidationLimits[0]["NH4-NMax"]);

            // set Uric Acid
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["Uric"])))
            {
                ObjMantype.UricAcidN.Value = 0d;
                ObjMantype.UricAcidN.DisplayValue = "";
            }
            else
            {
                ObjMantype.UricAcidN.Value = Convert.ToDouble(dv[0]["Uric"]);
                ObjMantype.UricAcidN.DisplayValue = Convert.ToString(dv[0]["Uric"]);
            }
            ObjMantype.UricAcidN.Min = Convert.ToDouble(ValidationLimits[0]["UricMin"]);
            ObjMantype.UricAcidN.Max = Convert.ToDouble(ValidationLimits[0]["UricMax"]);

            // Set NO3-N
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["NO3-N"])))
            {
                ObjMantype.NitrateN.Value = 0d;
                ObjMantype.NitrateN.DisplayValue = "";
            }
            else
            {
                ObjMantype.NitrateN.Value = Convert.ToDouble(dv[0]["NO3-N"]);
                ObjMantype.NitrateN.DisplayValue = Convert.ToString(dv[0]["NO3-N"]);
            }
            ObjMantype.NitrateN.Min = Convert.ToDouble(ValidationLimits[0]["NO3-NMin"]);
            ObjMantype.NitrateN.Max = Convert.ToDouble(ValidationLimits[0]["NO3-NMax"]);

            // set P2O5
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["P2O5"])))
            {
                ObjMantype.P2O5.Value = 0d;
                ObjMantype.P2O5.DisplayValue = "";
            }
            else
            {
                ObjMantype.P2O5.Value = Convert.ToDouble(dv[0]["P2O5"]);
                ObjMantype.P2O5.DisplayValue = Convert.ToString(dv[0]["P2O5"]);
            }
            ObjMantype.P2O5.AvailablePercent = Convert.ToInt32(dv[0]["P2O5_Available"]);
            ObjMantype.P2O5.Min = Convert.ToDouble(ValidationLimits[0]["P2O5Min"]);
            ObjMantype.P2O5.Max = Convert.ToDouble(ValidationLimits[0]["P2O5Max"]);

            // Set K2O
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["K2O"])))
            {
                ObjMantype.K2O.Value = 0d;
                ObjMantype.K2O.DisplayValue = "";
            }
            else
            {
                ObjMantype.K2O.Value = Convert.ToDouble(dv[0]["K2O"]);
                ObjMantype.K2O.DisplayValue = Convert.ToString(dv[0]["K2O"]);
            }
            ObjMantype.K2O.AvailablePercent = Convert.ToInt32(dv[0]["K2O_Available"]);
            ObjMantype.K2O.Min = Convert.ToDouble(ValidationLimits[0]["K2OMin"]);
            ObjMantype.K2O.Max = Convert.ToDouble(ValidationLimits[0]["K2OMax"]);


            // Set SO3
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["SO3"])))
            {
                ObjMantype.SO3.Value = 0d;
                ObjMantype.SO3.DisplayValue = "";
            }
            else
            {
                ObjMantype.SO3.Value = Convert.ToDouble(dv[0]["SO3"]);
                ObjMantype.SO3.DisplayValue = Convert.ToString(dv[0]["SO3"]);
            }
            ObjMantype.SO3.AvailablePercent = Convert.ToInt32(dv[0]["SO3_Available"]);
            ObjMantype.SO3.Min = Convert.ToDouble(ValidationLimits[0]["SO3Min"]);
            ObjMantype.SO3.Max = Convert.ToDouble(ValidationLimits[0]["SO3Max"]);
            ObjMantype.SO3.AvailableAutumnOsrGrassPercentage = Convert.ToDouble(dv[0]["SO3_AvaiableAutumnOsrGrass"]);
            ObjMantype.SO3.AvailableSpringPercentage = Convert.ToDouble(dv[0]["SO3_AvailableSpring"]);
            ObjMantype.SO3.AvailableAutumnOtherPercentage = Convert.ToDouble(dv[0]["SO3_AvaiableAutumnOther"]);



            // Set MgO
            if (string.IsNullOrEmpty(Convert.ToString(dv[0]["MgO"])))
            {
                ObjMantype.MgO.Value = 0d;
                ObjMantype.MgO.DisplayValue = "";
            }
            else
            {
                ObjMantype.MgO.Value = Convert.ToDouble(dv[0]["MgO"]);
                ObjMantype.MgO.DisplayValue = Convert.ToString(dv[0]["MgO"]);
            }
            ObjMantype.MgO.AvailablePercent = Convert.ToInt32(dv[0]["MgO_Available"]);
            ObjMantype.MgO.Min = Convert.ToDouble(ValidationLimits[0]["MgOMin"]);
            ObjMantype.MgO.Max = Convert.ToDouble(ValidationLimits[0]["MgOMax"]);

            ObjMantype.ManureNameEnum = (MannerLib.Enumerations.ManureTypes)ObjMantype.ManureID;
            ObjMantype.ManureNameString = MannerLib.Enumerations.ManureTypesToString((MannerLib.Enumerations.ManureTypes)ObjMantype.ManureID);

            manner.ManureType = ObjMantype;
            manner.Application.Rate = Convert.ToDouble(dv[0]["ApplicationRate"]);

            // C Lam: added Oct 2012
            manner.Application.RateArable = Convert.ToDouble(dv[0]["ApplicationRate"]);
            manner.Application.RateGrass = Convert.ToDouble(dv[0]["ApplicationRateGrass"]);

        }

        // Aug 2012 - Christopher Lam
        public int GetCropUptakeFactorDefault(MannerLib.Enumerations.CropTypeEnum cropType)
        {

            int CropuptakeFactor;

            switch (cropType) // Base it on the selected crop type
            {

                case MannerLib.Enumerations.CropTypeEnum.Grass:                                                         // Grass
                    {
                        CropuptakeFactor = 20;
                        break;
                    }
                case MannerLib.Enumerations.CropTypeEnum.EarlySownWinterCereal:                          // Early sown winter cereal
                    {
                        CropuptakeFactor = 10;
                        break;
                    }
                case MannerLib.Enumerations.CropTypeEnum.LateSownWinterCereal:                           // Late sown winter cereal
                    {
                        CropuptakeFactor = 5;
                        break;
                    }
                case MannerLib.Enumerations.CropTypeEnum.EarlyEstablishedWinterOilseedRape:         // Early established winter oilseed rape
                    {
                        CropuptakeFactor = 20; // Aug 2012 - Lizzie says change from 30 to 20
                        break;
                    }
                case MannerLib.Enumerations.CropTypeEnum.LateEstablishedWinterOilseedRape:          // Late established winter oilseed rape
                    {
                        CropuptakeFactor = 10;
                        break;
                    }
                case MannerLib.Enumerations.CropTypeEnum.Other:
                case MannerLib.Enumerations.CropTypeEnum.Potatoes:
                case MannerLib.Enumerations.CropTypeEnum.Sugarbeet:
                case MannerLib.Enumerations.CropTypeEnum.SpringCerealOilseedRape: // Spring cereal/oilseed rape; potatoes; sugar beet; other
                    {
                        CropuptakeFactor = 0;                                                                                       // Anything else - catch all
                        break;
                    }

                default:
                    {
                        CropuptakeFactor = 0;
                        break;
                    }
            }

            return CropuptakeFactor;

        }

        public string GetDataField(int RunType, XmlLookups lookupTable, string FilterFieldName, string LookupFieldName, int LookupID)
        {
            var dv = new DataView();
            var ds = new DataSet();

            ds.ReadXml(GetLookupListName(RunType, lookupTable));
            dv = ds.Tables[0].DefaultView;
            dv.RowFilter = FilterFieldName + " = " + LookupID;
            return Convert.ToString(dv[0][LookupFieldName]);
        }

        /// <summary>
        /// This function returns a climate attribute for a particular month. You can get one of the following values (Soil Moisture Deficit,  rainfall, Potential Evapotranspiration, Actual Evapotranspiration)
        /// </summary>
        /// <param name="month"></param>
        /// <param name="climate"></param>
        /// <param name="climateType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public double GetClimateType(int month, Climate climate, ClimateType climateType)
        {

            var climateMonths = new ClimateMonths();
            var retVal = default(double);

            switch (climateType)
            {
                case ClimateType.SoilMoistureDefecit:
                    {
                        climateMonths = climate.SoilMoistureDeficit;
                        break;
                    }
                case ClimateType.Rainfall:
                    {
                        climateMonths = climate.Rain;
                        break;
                    }
                case ClimateType.PotentialEvapotranspiration:
                    {
                        climateMonths = climate.PotentialEvapotranspiration;
                        break;
                    }
                case ClimateType.ActualEvapotranspiration:
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
        private System.IO.Stream ResourceXML(string sEmbeddedResourceFile)
        {

            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();

            var file = thisExe.GetManifestResourceStream(sEmbeddedResourceFile);

            return file;

        }

        private ArrayList AddItemToArray(DataView dv)
        {
            var Al = new ArrayList();

            foreach (DataRowView drv in dv)
                Al.Add(new MannerLib.Item(drv[0].ToString(), drv[1].ToString()));

            return Al;
        }

        // Aug 2012 - C Lam
        public int GetAverageAnnualRainfall(string postcode, ref bool isEnglandWalesOrScotland)
        {
            double avAnnualRainfall;
            isEnglandWalesOrScotland = true; // avoid NVZ NI warning pop-up if postcode is not found

            try
            {
                var xml = new XmlDocument();
                xml.Load(MiscLib.GetResourceStream("MannerClimateData.xml"));
                string xpath = "//xml/data/row[@POSTCODE='{postcode}']".Replace("{postcode}", postcode);
                var node = xml.SelectSingleNode(xpath);

                // sum 12 months' rainfall
                var monthlyRainfall = new double[13];
                monthlyRainfall[0] = Convert.ToDouble(node.Attributes["MeanPR_JAN"].Value);
                monthlyRainfall[1] = Convert.ToDouble(node.Attributes["MeanPR_FEB"].Value);
                monthlyRainfall[2] = Convert.ToDouble(node.Attributes["MeanPR_MAR"].Value);
                monthlyRainfall[3] = Convert.ToDouble(node.Attributes["MeanPR_APR"].Value);
                monthlyRainfall[4] = Convert.ToDouble(node.Attributes["MeanPR_MAY"].Value);
                monthlyRainfall[5] = Convert.ToDouble(node.Attributes["MeanPR_JUN"].Value);
                monthlyRainfall[6] = Convert.ToDouble(node.Attributes["MeanPR_JUL"].Value);
                monthlyRainfall[7] = Convert.ToDouble(node.Attributes["MeanPR_AUG"].Value);
                monthlyRainfall[8] = Convert.ToDouble(node.Attributes["MeanPR_SEP"].Value);
                monthlyRainfall[9] = Convert.ToDouble(node.Attributes["MeanPR_OCT"].Value);
                monthlyRainfall[10] = Convert.ToDouble(node.Attributes["MeanPR_NOV"].Value);
                monthlyRainfall[11] = Convert.ToDouble(node.Attributes["MeanPR_DEC"].Value);

                avAnnualRainfall = 0d;
                for (int i = 0; i <= 11; i++)
                    avAnnualRainfall += monthlyRainfall[i];

                isEnglandWalesOrScotland = node.Attributes["TERRITORY"].Value == "EnglandWalesScotland";
            }

            catch (Exception ex)
            {
                avAnnualRainfall = -1; // signal problem
            }

            return Convert.ToInt32(avAnnualRainfall);

        }

    }

}
