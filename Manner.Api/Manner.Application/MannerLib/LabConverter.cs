using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib
{
    public class LabConverter
    {

        private const double ConversionFactorsP = 2.291d;
        private const double ConversionFactorsK = 1.205d;
        private const double ConversionFactorsMg = 1.658d;
        private const double ConversionFactorsS = 2.5d;
        private const double ConversionFactorsNAmmoniumN = 1d;
        private const double ConversionFactorsUricAcid = 1d;
        private const double ConversionFactorsTotalN = 1d;
        private const double ConversionFactorsNitrateN = 1d;

        public enum CurrentUnit
        {
            Kg = 0,
            Percent = 1
        }

        public enum NutrientItems
        {
            DryMatter = 0,
            TotalN = 1,
            AmmoniumN = 2,
            UricAcidN = 3,
            NitrateN = 4,
            TotalP = 5,
            TotalK = 6,
            TotalS = 7,
            TotalMg = 8
        }

        /// <summary>
        /// This routine was developed to enable users of MANNER to automatically convert the nutrient analysis results most commonly reported by laboratories on a dry matter basis, into the fresh weight basis. 
        /// Parameter ManureID - The selected manure type ID, Parameter NutrientVal - the input value of the Nutrient to be converted,  unitFactor - Dry matter expressed a g/kg or %, Nutrient - the nutrient to be converted, DryMatter - value of dry matter (don't need to supply for layer manure, broiler/turkeys or ducks).
        /// </summary>
        /// <param name="runType"></param>
        /// <param name="manureID"></param>
        /// <param name="nutrientValue"></param>
        /// <param name="unitFactor"></param>
        /// <param name="nutrient"></param>
        /// <param name="dryMatter"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public double LabConvertNutrient(int runType, int manureID, double nutrientValue, CurrentUnit unitFactor, NutrientItems nutrient, double dryMatter = 0d)
        {

            try
            {
                int mantype;
                var ObjManData = new MannerLib.MannerData();
                double retVal = 0d;

                mantype = Convert.ToInt32(ObjManData.GetDataField(runType, MannerLib.MannerData.XmlLookups.ManureTypes, "ManureID", "ManureCategory", manureID));

                switch (nutrient)
                {

                    case NutrientItems.DryMatter:
                        {
                            retVal = CalculateDryMatter(mantype, nutrientValue, unitFactor);
                            break;
                        }

                    case NutrientItems.TotalN:
                        {
                            retVal = CalcTotalN(mantype, nutrientValue, unitFactor, dryMatter);
                            break;
                        }

                    case NutrientItems.AmmoniumN:
                    case NutrientItems.NitrateN:
                    case NutrientItems.TotalK:
                    case NutrientItems.TotalMg:
                    case var @case when @case == NutrientItems.TotalN:
                    case NutrientItems.TotalP:
                    case NutrientItems.TotalS:
                    case NutrientItems.UricAcidN:
                        {
                            retVal = CalculateOtherNutrient(mantype, nutrientValue, unitFactor, nutrient, dryMatter);
                            break;
                        }

                    default:
                        {
                            throw new Exception("Unknown enumeration value for MannerLib.LabConverter.NutrientItems [" + nutrient.ToString() + "]");
                        }
                }
                return retVal;
            }
            catch (Exception ex)
            {
                return 0d;
            }
        }

        private double CalculateDryMatter(int mantype, double Nutrientval, CurrentUnit unitfactor)
        {

            try
            {
                double calcVal;

                if (mantype == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry | mantype == (int)MannerLib.Enumerations.ManureCategory.PigSlurry)
                {
                    calcVal = Nutrientval / 10d;
                }
                else if (unitfactor == CurrentUnit.Kg)
                {
                    calcVal = Nutrientval / 10d;
                }
                else
                {
                    calcVal = Nutrientval;
                }

                return calcVal;
            }
            catch (Exception ex)
            {
                throw; // new Exception(ex.ToString());
                //return 0d;
            }
        }

        private double CalcTotalN(int mantype, double Nutrientval, CurrentUnit unitfactor, double drymatter)
        {
            try
            {

                double calcVal;

                if (mantype == (int)MannerLib.Enumerations.ManureCategory.CattleSlurry | mantype == (int)MannerLib.Enumerations.ManureCategory.PigSlurry)
                {
                    calcVal = Nutrientval;
                }
                else if (unitfactor == CurrentUnit.Kg)
                {
                    calcVal = Nutrientval * ConversionFactorsTotalN * drymatter / 1000d;
                }
                else
                {
                    calcVal = Nutrientval * ConversionFactorsTotalN * drymatter / 10d;
                }

                return calcVal;
            }
            catch (Exception ex)
            {
                throw; // new Exception(ex.ToString());
                //return 0d;
            }
        }

        private double CalculateOtherNutrient(int mantype, double Nutrientval, CurrentUnit unitfactor, NutrientItems Nutrient, double drymatter)
        {
            try
            {

                var ConversionFactor = default(double);
                double calcVal;

                switch (Nutrient)
                {
                    case NutrientItems.AmmoniumN:
                    case NutrientItems.UricAcidN:
                    case NutrientItems.NitrateN:
                        {
                            ConversionFactor = ConversionFactorsNAmmoniumN;
                            break;
                        }
                    case NutrientItems.TotalK:
                        {
                            ConversionFactor = ConversionFactorsK;
                            break;
                        }
                    case NutrientItems.TotalMg:
                        {
                            ConversionFactor = ConversionFactorsMg;
                            break;
                        }
                    case NutrientItems.TotalP:
                        {
                            ConversionFactor = ConversionFactorsP;
                            break;
                        }
                    case NutrientItems.TotalS:
                        {
                            ConversionFactor = ConversionFactorsS;
                            break;
                        }
                }

                if (mantype != (int)MannerLib.Enumerations.ManureCategory.CattleSlurry | mantype != (int)MannerLib.Enumerations.ManureCategory.PigSlurry)
                {
                    if (unitfactor == CurrentUnit.Kg)
                    {
                        calcVal = Nutrientval * ConversionFactor * drymatter / 1000000d;
                    }
                    else
                    {
                        calcVal = Nutrientval * ConversionFactor * drymatter / 100000d;
                    }
                }
                else
                {
                    calcVal = Nutrientval * ConversionFactor / 1000d;
                }

                return calcVal;
            }
            catch (Exception ex)
            {
                throw; // new Exception(ex.ToString());
                //return 0d;
            }

        }

        /// <summary>
        /// To covert mg/kg nutrient in Dry Matter to kg/t Fresh Weight
        /// ''' </summary>
        public double MilligramPerKilogramToKillogramPerTonne(double NutrientVal, double DryMatter)
        {

            return NutrientVal / 1000d * (DryMatter / 100d);

        }

        /// <summary>
        /// To convert g/kg nutrient in Dry Matter to kg/t Fresh Weight
        /// </summary>
        public double GramPerKilogramToKillogramPerTon(double NutrientVal, double DryMatter)
        {

            return NutrientVal * (DryMatter / 100d);

        }

        /// <summary>
        /// To convert g/100g nutrient in Dry Matter to kg/t Fresh Weight
        /// </summary>
        public double GramPer100KillogramsToKillogramPerTon(double NutrientVal, double DryMatter)
        {

            return NutrientVal * (DryMatter / 10d);

        }

        /// <summary>
        /// To convert % nutrient in Dry Matter to kg/t Fresh Weight
        /// </summary>
        public double PercentToKillogramPerTon(double NutrientVal, double DryMatter)
        {

            return NutrientVal * (DryMatter / 10d);

        }

        /// <summary>
        /// To convert mg/kg nutrient Fresh Weight to kg/t Fresh Weight
        /// </summary>
        public double MilligramPerKilogramToKillogramPerTonne(double NutrientVal)
        {

            return NutrientVal / 1000d;

        }


        /// <summary>
        /// To convert g/kg nutrient Fresh Weight to kg/t Fresh Weight
        /// </summary>
        public double GramPerKilogramToKillogramPerTon(double NutrientVal)
        {
            // No conversion (g/kg = kg/t)

            return NutrientVal;

        }

        /// <summary>
        /// To convert g/100g nutrient Fresh Weight to kg/t Fresh Weight
        /// </summary>
        public double GramPer100KillogramsToKillogramPerTon(double NutrientVal)
        {

            return NutrientVal * 10d;

        }

        /// <summary>
        /// To convert % nutrient in Fresh Weight to kg/t Fresh Weight
        /// </summary>
        public double PercentToKillogramPerTon(double NutrientVal)
        {

            return NutrientVal / 10d;

        }

        /// <summary>
        /// To convert mg/l nutrient to kg/m3
        /// </summary>
        public double MilligramsPerLitreToKilogramPerCubicMetre(double NutrientVal)
        {

            return NutrientVal / 1000d;

        }

        /// <summary>
        /// To convert g/l nutrient to kg/m3
        /// </summary>
        public double GramPerLitreToKilogramPerCubicMetre(double NutrientVal)
        {
            // No conversion needed (g/l = kg/m3)
            return NutrientVal;

        }
    }
}
