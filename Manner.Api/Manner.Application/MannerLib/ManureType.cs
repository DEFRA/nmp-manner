using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Manner.Application.MannerLib.Enumerations;

namespace Manner.Application.MannerLib;

public class ManureType
{
    public ManureType()            
    {
        Element = "ManureType";
        Registration.RegistrationIndicator = 0; // causes registration to take place
        ManureID = 0;
        ManureNameEnum = ManureTypes.CattleFYMFresh;
        ManureNameString = "";
        ManureCategory = ManureCategory;
        LiquidOrSolid = "";
        DryMatter = new DryMatterType();
        TotalN = new Nutrient("TotalN");
        NH4N = new Nutrient("NH4-N");
        P2O5 = new Nutrient("P2O5");
        K2O = new Nutrient("K2O");
        SO3 = new Nutrient("SO3");
        MgO = new Nutrient("MgO");
        UricAcidN = new Nutrient("UricAcidN");
        NitrateN = new Nutrient("NitrateN");
        NMaxConst = 0d;
        ManureCategoryUserInterface = ManureCategoryUserInterface.LiveStock;
        ManureCategoryUserInterfaceText = "";
        HighRan = "";
    }

    public ManureType(string element) 
    {
        Element = element;
        Registration.RegistrationIndicator = 0;
        ManureID = 0;
        ManureNameEnum = ManureTypes.CattleFYMFresh;
        ManureNameString = "";
        ManureCategory = ManureCategory;
        LiquidOrSolid = "";
        DryMatter = new DryMatterType();
        TotalN = new Nutrient("TotalN");
        NH4N = new Nutrient("NH4-N");
        P2O5 = new Nutrient("P2O5");
        K2O = new Nutrient("K2O");
        SO3 = new Nutrient("SO3");
        MgO = new Nutrient("MgO");
        UricAcidN = new Nutrient("UricAcidN");
        NitrateN = new Nutrient("NitrateN");
        NMaxConst = 0d;
        ManureCategoryUserInterface = ManureCategoryUserInterface;
        ManureCategoryUserInterfaceText = "";
        HighRan = "";
    }

    public string Element { get; set; }

    public int ManureID {  get; set; }

    public ManureTypes ManureNameEnum { get; set; }

    public string ManureNameString {  get; set; }
    public ManureCategory ManureCategory { get; set; }

    public string LiquidOrSolid {  get; set; }
    public  DryMatterType DryMatter {  get; set; }
    public Nutrient TotalN {  get; set; }
    public Nutrient NH4N {  get; set; }
    public Nutrient P2O5 {  get; set; }
    public Nutrient K2O {  get; set; }
    public Nutrient SO3 {  get; set; }
    public Nutrient MgO {  get; set; }
    public Nutrient UricAcidN {  get; set; }
    public Nutrient NitrateN {  get; set; }
    public double NMaxConst {  get; set; }
    public ManureCategoryUserInterface ManureCategoryUserInterface {  get; set; }
    public string ManureCategoryUserInterfaceText {  get; set; }
    public string HighRan {  get; set; }

}
