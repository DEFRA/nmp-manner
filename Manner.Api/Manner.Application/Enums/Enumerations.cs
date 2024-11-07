using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Enums;
public partial class Enumerations
{

#region Enumeration 'CropTypeRb209'

    /// <summary>
    /// 	Converts a string to a CropTypeRb209 enumeration
    /// </summary>
    public static string CropTypeRb209ToString(CropTypeRb209 enumValue)
    {
        switch (enumValue)
        {
            case CropTypeRb209.WinterWheat:
                {
                    return "Winter Wheat";
                }
            case CropTypeRb209.WinterBarley:
                {
                    return "Winter Barley";
                }
            case CropTypeRb209.SpringWheat:
                {
                    return "Spring Wheat";
                }
            case CropTypeRb209.SpringBarley:
                {
                    return "Spring Barley";
                }
            case CropTypeRb209.WinterOats:
                {
                    return "Winter Oats";
                }
            case CropTypeRb209.SpringOats:
                {
                    return "Spring Oats";
                }
            case CropTypeRb209.WinterRye:
                {
                    return "Winter Rye";
                }
            case CropTypeRb209.SpringRye:
                {
                    return "Spring Rye";
                }
            case CropTypeRb209.WinterTriticale:
                {
                    return "Winter Triticale";
                }
            case CropTypeRb209.SpringTriticale:
                {
                    return "Spring Triticale";
                }
            case CropTypeRb209.WinterOilseedRape:
                {
                    return "Winter Oilseed Rape";
                }
            case CropTypeRb209.SpringOilseedRape:
                {
                    return "Spring Oilseed Rape";
                }
            case CropTypeRb209.Linseed:
                {
                    return "Linseed";
                }
            case CropTypeRb209.Peas:
                {
                    return "Peas";
                }
            case CropTypeRb209.WinterBeans:
                {
                    return "Winter Beans";
                }
            case CropTypeRb209.SpringBeans:
                {
                    return "Spring Beans";
                }
            case CropTypeRb209.SugarBeet:
                {
                    return "Sugar Beet";
                }
            case CropTypeRb209.RyegrassGrownForSeed:
                {
                    return "Ryegrass Grown For Seed";
                }
            case CropTypeRb209.UncroppedOrFallow:
                {
                    return "Uncropped or Fallow";
                }
            case CropTypeRb209.ForageMaize:
                {
                    return "Forage Maize";
                }
            case CropTypeRb209.ForageSwedes:
                {
                    return "Forage Swedes";
                }
            case CropTypeRb209.ForageRape:
                {
                    return "Forage Rape";
                }
            case CropTypeRb209.StubbleTurnips:
                {
                    return "Stubble Turnips";
                }
            case CropTypeRb209.FodderBeet:
                {
                    return "Fodder Beet";
                }
            case CropTypeRb209.WholecropSpringBarley:
                {
                    return "Wholecrop Spring Barley";
                }
            case CropTypeRb209.WholecropSpringWheat:
                {
                    return "Wholecrop Spring Wheat";
                }
            case CropTypeRb209.WholecropWinterBarley:
                {
                    return "Wholecrop Winter Barley";
                }
            case CropTypeRb209.WholecropWinterWheat:
                {
                    return "Wholecrop Winter Wheat";
                }
            case CropTypeRb209.ForageSpringOats:
                {
                    return "Forage Spring Oats";
                }
            case CropTypeRb209.ForageSpringRye:
                {
                    return "Forage Spring Rye";
                }
            case CropTypeRb209.ForageSpringTriticale:
                {
                    return "Forage Spring Triticale";
                }
            case CropTypeRb209.ForageWinterOats:
                {
                    return "Forage Winter Oats";
                }
            case CropTypeRb209.ForageWinterRye:
                {
                    return "Forage Winter Rye";
                }
            case CropTypeRb209.ForageWinterTriticale:
                {
                    return "Forage Winter Triticale";
                }
            case CropTypeRb209.Asparagus:
                {
                    return "Asparagus";
                }
            case CropTypeRb209.BrusselSprouts:
                {
                    return "Brussel Sprouts";
                }
            case CropTypeRb209.Cabbage:
                {
                    return "Cabbage";
                }
            case CropTypeRb209.Cauliflower:
                {
                    return "Cauliflower";
                }
            case CropTypeRb209.Calabrese:
                {
                    return "Calabrese";
                }
            case CropTypeRb209.CelerySelfBlanching:
                {
                    return "Celery Self-blanching";
                }
            case CropTypeRb209.BroadBeans:
                {
                    return "Broad Beans";
                }
            case CropTypeRb209.DwarfBeans:
                {
                    return "Dwarf Beans";
                }
            case CropTypeRb209.RunnerBeans:
                {
                    return "Runner Beans";
                }
            case CropTypeRb209.Lettuce:
                {
                    return "Lettuce";
                }
            case CropTypeRb209.Radish:
                {
                    return "Radish";
                }
            case CropTypeRb209.Sweetcorn:
                {
                    return "Sweetcorn";
                }
            case CropTypeRb209.Courgettes:
                {
                    return "Courgettes";
                }
            case CropTypeRb209.BulbOnions:
                {
                    return "Bulb Onions";
                }
            case CropTypeRb209.SaladOnions:
                {
                    return "Salad Onions";
                }
            case CropTypeRb209.Leeks:
                {
                    return "Leeks";
                }
            case CropTypeRb209.BulbsAndBulbFlowers:
                {
                    return "Bulbs and Bulb Flowers";
                }
            case CropTypeRb209.BabyLeafLettuce:
                {
                    return "Baby Leaf Lettuce";
                }
            case CropTypeRb209.WildRocket:
                {
                    return "Wild Rocket";
                }
            case CropTypeRb209.Pumpkin:
                {
                    return "Pumpkin";
                }
            case CropTypeRb209.Beetroot:
                {
                    return "Beetroot";
                }
            case CropTypeRb209.Parsnips:
                {
                    return "Parsnips";
                }
            case CropTypeRb209.Swedes:
                {
                    return "Swedes";
                }
            case CropTypeRb209.Turnips:
                {
                    return "Turnips";
                }
            case CropTypeRb209.Carrots:
                {
                    return "Carrots";
                }
            case CropTypeRb209.DessertApples:
                {
                    return "Dessert Apples";
                }
            case CropTypeRb209.CulinaryApples:
                {
                    return "Culinary Apples";
                }
            case CropTypeRb209.CiderApples:
                {
                    return "Cider Apples";
                }
            case CropTypeRb209.Pears:
                {
                    return "Pears";
                }
            case CropTypeRb209.Cherries:
                {
                    return "Cherries";
                }
            case CropTypeRb209.Plums:
                {
                    return "Plums";
                }
            case CropTypeRb209.Blackcurrants:
                {
                    return "Blackcurrants";
                }
            case CropTypeRb209.Redcurrants:
                {
                    return "Redcurrants";
                }
            case CropTypeRb209.Gooseberries:
                {
                    return "Gooseberries";
                }
            case CropTypeRb209.Raspberries:
                {
                    return "Raspberries";
                }
            case CropTypeRb209.Loganberries:
                {
                    return "Loganberries";
                }
            case CropTypeRb209.Tayberries:
                {
                    return "Tayberries";
                }
            case CropTypeRb209.Blackberries:
                {
                    return "Blackberries";
                }
            case CropTypeRb209.Strawberries:
                {
                    return "Strawberries";
                }
            case CropTypeRb209.Vines:
                {
                    return "Vines";
                }
            case CropTypeRb209.Hops:
                {
                    return "Hops";
                }
            case CropTypeRb209.Grass:
                {
                    return "Grass";
                }
            case CropTypeRb209.PotatoVarietyGroup1:
                {
                    return "Potato Variety Group 1";
                }
            case CropTypeRb209.PotatoVarietyGroup2:
                {
                    return "Potato Variety Group 2";
                }
            case CropTypeRb209.PotatoVarietyGroup3:
                {
                    return "Potato Variety Group 3";
                }
            case CropTypeRb209.PotatoVarietyGroup4:
                {
                    return "Potato Variety Group 4";
                }
            case CropTypeRb209.Other:
                {
                    return "Other";
                }
            case CropTypeRb209.BarleySpringUndersown:
                {
                    return "Barley, spring (undersown)";
                }
            case CropTypeRb209.OatsSpringUndersown:
                {
                    return "Oats, spring (undersown)";
                }
            case CropTypeRb209.TriticaleSpringUndersown:
                {
                    return "Triticale, spring (undersown)";
                }
            case CropTypeRb209.WheatSpringUndersown:
                {
                    return "Wheat, spring (undersown)";
                }
            case CropTypeRb209.Lupins:
                {
                    return "Lupins";
                }
            case CropTypeRb209.Hemp:
                {
                    return "Hemp";
                }
            case CropTypeRb209.Blueberries:
                {
                    return "Blueberries";
                }
            case CropTypeRb209.Rhubarb:
                {
                    return "Rhubarb";
                }
            case CropTypeRb209.Narcissus:
                {
                    return "Narcissus";
                }
            case CropTypeRb209.Tulip:
                {
                    return "Tulip";
                }
            case CropTypeRb209.MarketPickPeas:
                {
                    return "Market Pick Peas";
                }
            case CropTypeRb209.SwedesShopping:
                {
                    return "Swedes, shopping";
                }
            case CropTypeRb209.Miscanthus:
                {
                    return "Miscanthus";
                }
            case CropTypeRb209.Willow:
                {
                    return "Willow";
                }
            case CropTypeRb209.HerbageForSeed:
                {
                    return "Herbage for seed";
                }
            case CropTypeRb209.KaleCut:
                {
                    return "Kale (cut)";
                }
            case CropTypeRb209.KaleGrazed:
                {
                    return "Kale Grazed";
                }
            case CropTypeRb209.ChicoryPureStand:
                {
                    return "Chicory, pure stand";
                }
            case CropTypeRb209.Coriander:
                {
                    return "Coriander";
                }
            case CropTypeRb209.Mint:
                {
                    return "Mint";
                }
            case CropTypeRb209.SwedesForage:
                {
                    return "Swedes";
                }
            case CropTypeRb209.TurnipsForage:
                {
                    return "Turnips";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.CropTypeRb209 [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a CropTypeRb209 enumeration to a string (suitable for the XML document)
    /// </summary>
    public static CropTypeRb209 CropTypeRb209FromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Winter Wheat":
                {
                    return CropTypeRb209.WinterWheat;
                }
            case "Winter Barley":
                {
                    return CropTypeRb209.WinterBarley;
                }
            case "Spring Wheat":
                {
                    return CropTypeRb209.SpringWheat;
                }
            case "Spring Barley":
                {
                    return CropTypeRb209.SpringBarley;
                }
            case "Winter Oats":
                {
                    return CropTypeRb209.WinterOats;
                }
            case "Spring Oats":
                {
                    return CropTypeRb209.SpringOats;
                }
            case "Winter Rye":
                {
                    return CropTypeRb209.WinterRye;
                }
            case "Spring Rye":
                {
                    return CropTypeRb209.SpringRye;
                }
            case "Winter Triticale":
                {
                    return CropTypeRb209.WinterTriticale;
                }
            case "Spring Triticale":
                {
                    return CropTypeRb209.SpringTriticale;
                }
            case "Winter Oilseed Rape":
                {
                    return CropTypeRb209.WinterOilseedRape;
                }
            case "Spring Oilseed Rape":
                {
                    return CropTypeRb209.SpringOilseedRape;
                }
            case "Linseed":
                {
                    return CropTypeRb209.Linseed;
                }
            case "Peas":
                {
                    return CropTypeRb209.Peas;
                }
            case "Winter Beans":
                {
                    return CropTypeRb209.WinterBeans;
                }
            case "Spring Beans":
                {
                    return CropTypeRb209.SpringBeans;
                }
            case "Sugar Beet":
                {
                    return CropTypeRb209.SugarBeet;
                }
            case "Ryegrass Grown For Seed":
                {
                    return CropTypeRb209.RyegrassGrownForSeed;
                }
            case "Uncropped or Fallow":
                {
                    return CropTypeRb209.UncroppedOrFallow;
                }
            case "Forage Maize":
                {
                    return CropTypeRb209.ForageMaize;
                }
            case "Forage Swedes":
                {
                    return CropTypeRb209.ForageSwedes;
                }
            case "Forage Rape":
                {
                    return CropTypeRb209.ForageRape;
                }
            case "Stubble Turnips":
                {
                    return CropTypeRb209.StubbleTurnips;
                }
            case "Fodder Beet":
                {
                    return CropTypeRb209.FodderBeet;
                }
            case "Wholecrop Spring Barley":
                {
                    return CropTypeRb209.WholecropSpringBarley;
                }
            case "Wholecrop Spring Wheat":
                {
                    return CropTypeRb209.WholecropSpringWheat;
                }
            case "Wholecrop Winter Barley":
                {
                    return CropTypeRb209.WholecropWinterBarley;
                }
            case "Wholecrop Winter Wheat":
                {
                    return CropTypeRb209.WholecropWinterWheat;
                }
            case "Forage Spring Oats":
                {
                    return CropTypeRb209.ForageSpringOats;
                }
            case "Forage Spring Rye":
                {
                    return CropTypeRb209.ForageSpringRye;
                }
            case "Forage Spring Triticale":
                {
                    return CropTypeRb209.ForageSpringTriticale;
                }
            case "Forage Winter Oats":
                {
                    return CropTypeRb209.ForageWinterOats;
                }
            case "Forage Winter Rye":
                {
                    return CropTypeRb209.ForageWinterRye;
                }
            case "Forage Winter Triticale":
                {
                    return CropTypeRb209.ForageWinterTriticale;
                }
            case "Asparagus":
                {
                    return CropTypeRb209.Asparagus;
                }
            case "Brussel Sprouts":
                {
                    return CropTypeRb209.BrusselSprouts;
                }
            case "Cabbage":
                {
                    return CropTypeRb209.Cabbage;
                }
            case "Cauliflower":
                {
                    return CropTypeRb209.Cauliflower;
                }
            case "Calabrese":
                {
                    return CropTypeRb209.Calabrese;
                }
            case "Celery Self-blanching":
                {
                    return CropTypeRb209.CelerySelfBlanching;
                }
            case "Broad Beans":
                {
                    return CropTypeRb209.BroadBeans;
                }
            case "Dwarf Beans":
                {
                    return CropTypeRb209.DwarfBeans;
                }
            case "Runner Beans":
                {
                    return CropTypeRb209.RunnerBeans;
                }
            case "Lettuce":
                {
                    return CropTypeRb209.Lettuce;
                }
            case "Radish":
                {
                    return CropTypeRb209.Radish;
                }
            case "Sweetcorn":
                {
                    return CropTypeRb209.Sweetcorn;
                }
            case "Courgettes":
                {
                    return CropTypeRb209.Courgettes;
                }
            case "Bulb Onions":
                {
                    return CropTypeRb209.BulbOnions;
                }
            case "Salad Onions":
                {
                    return CropTypeRb209.SaladOnions;
                }
            case "Leeks":
                {
                    return CropTypeRb209.Leeks;
                }
            case "Bulbs and Bulb Flowers":
                {
                    return CropTypeRb209.BulbsAndBulbFlowers;
                }
            case "Baby Leaf Lettuce":
                {
                    return CropTypeRb209.BabyLeafLettuce;
                }
            case "Wild Rocket":
                {
                    return CropTypeRb209.WildRocket;
                }
            case "Pumpkin":
                {
                    return CropTypeRb209.Pumpkin;
                }
            case "Beetroot":
                {
                    return CropTypeRb209.Beetroot;
                }
            case "Parsnips":
                {
                    return CropTypeRb209.Parsnips;
                }
            case "Swedes":
                {
                    return CropTypeRb209.Swedes;
                }
            case "Turnips":
                {
                    return CropTypeRb209.Turnips;
                }
            case "Carrots":
                {
                    return CropTypeRb209.Carrots;
                }
            case "Dessert Apples":
                {
                    return CropTypeRb209.DessertApples;
                }
            case "Culinary Apples":
                {
                    return CropTypeRb209.CulinaryApples;
                }
            case "Cider Apples":
                {
                    return CropTypeRb209.CiderApples;
                }
            case "Pears":
                {
                    return CropTypeRb209.Pears;
                }
            case "Cherries":
                {
                    return CropTypeRb209.Cherries;
                }
            case "Plums":
                {
                    return CropTypeRb209.Plums;
                }
            case "Blackcurrants":
                {
                    return CropTypeRb209.Blackcurrants;
                }
            case "Redcurrants":
                {
                    return CropTypeRb209.Redcurrants;
                }
            case "Gooseberries":
                {
                    return CropTypeRb209.Gooseberries;
                }
            case "Raspberries":
                {
                    return CropTypeRb209.Raspberries;
                }
            case "Loganberries":
                {
                    return CropTypeRb209.Loganberries;
                }
            case "Tayberries":
                {
                    return CropTypeRb209.Tayberries;
                }
            case "Blackberries":
                {
                    return CropTypeRb209.Blackberries;
                }
            case "Strawberries":
                {
                    return CropTypeRb209.Strawberries;
                }
            case "Vines":
                {
                    return CropTypeRb209.Vines;
                }
            case "Hops":
                {
                    return CropTypeRb209.Hops;
                }
            case "Grass":
                {
                    return CropTypeRb209.Grass;
                }
            case "Potato Variety Group 1":
                {
                    return CropTypeRb209.PotatoVarietyGroup1;
                }
            case "Potato Variety Group 2":
                {
                    return CropTypeRb209.PotatoVarietyGroup2;
                }
            case "Potato Variety Group 3":
                {
                    return CropTypeRb209.PotatoVarietyGroup3;
                }
            case "Potato Variety Group 4":
                {
                    return CropTypeRb209.PotatoVarietyGroup4;
                }
            case "Other":
                {
                    return CropTypeRb209.Other;
                }
            case "Barley, spring (undersown)":
                {
                    return CropTypeRb209.BarleySpringUndersown;
                }
            case "Oats, spring (undersown)":
                {
                    return CropTypeRb209.OatsSpringUndersown;
                }
            case "Triticale, spring (undersown)":
                {
                    return CropTypeRb209.TriticaleSpringUndersown;
                }
            case "Wheat, spring (undersown)":
                {
                    return CropTypeRb209.WheatSpringUndersown;
                }
            case "Lupins":
                {
                    return CropTypeRb209.Lupins;
                }
            case "Hemp":
                {
                    return CropTypeRb209.Hemp;
                }
            case "Blueberries":
                {
                    return CropTypeRb209.Blueberries;
                }
            case "Rhubarb":
                {
                    return CropTypeRb209.Rhubarb;
                }
            case "Narcissus":
                {
                    return CropTypeRb209.Narcissus;
                }
            case "Tulip":
                {
                    return CropTypeRb209.Tulip;
                }
            case "Market Pick Peas":
                {
                    return CropTypeRb209.MarketPickPeas;
                }
            case "Swedes, shopping":
                {
                    return CropTypeRb209.SwedesShopping;
                }
            case "Miscanthus":
                {
                    return CropTypeRb209.Miscanthus;
                }
            case "Willow":
                {
                    return CropTypeRb209.Willow;
                }
            case "Herbage for seed":
                {
                    return CropTypeRb209.HerbageForSeed;
                }
            case "Kale (cut)":
                {
                    return CropTypeRb209.KaleCut;
                }
            case "Kale Grazed":
                {
                    return CropTypeRb209.KaleGrazed;
                }
            case "Chicory, pure stand":
                {
                    return CropTypeRb209.ChicoryPureStand;
                }
            case "Coriander":
                {
                    return CropTypeRb209.Coriander;
                }
            case "Mint":
                {
                    return CropTypeRb209.Mint;
                }
            case "Swedes Forage":
                {
                    return CropTypeRb209.SwedesForage;
                }
            case "Turnips Forage":
                {
                    return CropTypeRb209.TurnipsForage;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.CropTypeRb209"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.CropTypeRb209 [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.CropTypeRb209"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection CropTypeRb209Names()
    {
        var ret = new StringCollection();
        var t = typeof(CropTypeRb209);
        foreach (CropTypeRb209 e in Enum.GetValues(t))
            ret.Add(CropTypeRb209ToString(e));
        return ret;
    }

#endregion
#region Enumeration 'ManureTypesRb209'

    /// <summary>
    /// 	Converts a string to a ManureTypes enumeration
    /// </summary>
    public static string ManureTypesRB209ToString(ManureTypesRb209 enumValue)
    {
        switch (enumValue)
        {
            case ManureTypesRb209.CattleFYMOld:
                {
                    return "Cattle FYM - Old";
                }
            case ManureTypesRb209.CattleFYMFresh:
                {
                    return "Cattle FYM - Fresh";
                }
            case ManureTypesRb209.PigFYMOld:
                {
                    return "Pig FYM - Old";
                }
            case ManureTypesRb209.PigFYMFresh:
                {
                    return "Pig FYM - Fresh";
                }
            case ManureTypesRb209.SheepFYMOld:
                {
                    return "Sheep FYM - Old";
                }
            case ManureTypesRb209.SheepFYMFresh:
                {
                    return "Sheep FYM - Fresh";
                }
            case ManureTypesRb209.DuckFYMOld:
                {
                    return "Duck FYM - Old";
                }
            case ManureTypesRb209.DuckFYMFresh:
                {
                    return "Duck FYM - Fresh";
                }
            case ManureTypesRb209.HorseFYMOld:
                {
                    return "Horse FYM - Old";
                }
            case ManureTypesRb209.HorseFYMFresh:
                {
                    return "Horse FYM - Fresh";
                }
            case ManureTypesRb209.GoatFYMOld:
                {
                    return "Goat FYM - Old";
                }
            case ManureTypesRb209.GoatFYMFresh:
                {
                    return "Goat FYM - Fresh";
                }
            case ManureTypesRb209.PoultryManure20Percent:
                {
                    return "Poultry Manure 20%";
                }
            case ManureTypesRb209.PoultryManure40Percent:
                {
                    return "Poultry Manure 40%";
                }
            case ManureTypesRb209.PoultryManure60Percent:
                {
                    return "Poultry Manure 60%";
                }
            case ManureTypesRb209.PoultryManure80Percent:
                {
                    return "Poultry Manure 80%";
                }
            case ManureTypesRb209.CattleSlurry2Percent:
                {
                    return "Cattle Slurry 2%";
                }
            case ManureTypesRb209.CattleSlurry6Percent:
                {
                    return "Cattle Slurry 6%";
                }
            case ManureTypesRb209.CattleSlurry10Percent:
                {
                    return "Cattle Slurry 10%";
                }
            case ManureTypesRb209.DirtyWater:
                {
                    return "Dirty Water";
                }
            case ManureTypesRb209.SeparatedCattleSlurryStrainerBox:
                {
                    return "Separated Cattle Slurry - Strainer Box";
                }
            case ManureTypesRb209.SeparatedCattleSlurryWeepingWall:
                {
                    return "Separated Cattle Slurry - Weeping Wall";
                }
            case ManureTypesRb209.SeparatedCattleSlurryMechanicalSeparator:
                {
                    return "Separated Cattle Slurry - Mechanical Separator";
                }
            case ManureTypesRb209.SeparatedCattleSlurrySolidPortion:
                {
                    return "Separated Cattle Slurry - Solid Portion";
                }
            case ManureTypesRb209.PigSlurry2Percent:
                {
                    return "Pig Slurry 2%";
                }
            case ManureTypesRb209.PigSlurry4Percent:
                {
                    return "Pig Slurry 4%";
                }
            case ManureTypesRb209.PigSlurry6Percent:
                {
                    return "Pig Slurry 6%";
                }
            case ManureTypesRb209.SeparatedPigSlurryLiquidPortion:
                {
                    return "Separated Pig Slurry - Liquid Portion";
                }
            case ManureTypesRb209.SeparatedPigSlurrySolidPortion:
                {
                    return "Separated Pig Slurry - Solid Portion";
                }
            case ManureTypesRb209.BiosolidsDigestedCake:
                {
                    return "Biosolids - Digested Cake";
                }
            case ManureTypesRb209.BiosolidsThermallyDried:
                {
                    return "Biosolids - Thermally Dried";
                }
            case ManureTypesRb209.BiosolidsLimeStabilised:
                {
                    return "Biosolids - Lime Stabilised";
                }
            case ManureTypesRb209.BiosolidsComposted:
                {
                    return "Biosolids - Composted";
                }
            case ManureTypesRb209.GreenCompost:
                {
                    return "Green Compost";
                }
            case ManureTypesRb209.GreenFoodCompost:
                {
                    return "Green/Food Compost";
                }
            case ManureTypesRb209.DigestateWholeFoodBased:
                {
                    return "Digestate - Whole Food Based";
                }
            case ManureTypesRb209.DigestateSeparatedLiquorFoodBased:
                {
                    return "Digestate - Separated Liquor Food Based";
                }
            case ManureTypesRb209.DigestateSeparatedFibreFoodBased:
                {
                    return "Digestate - Separated Fibre Food Based";
                }
            case ManureTypesRb209.DigestateWholeFarmSourced:
                {
                    return "Digestate - Whole Farm Sourced";
                }
            case ManureTypesRb209.DigestateSeparatedLiquorFarmSourced:
                {
                    return "Digestate - Separated Liquor Farm Sourced";
                }
            case ManureTypesRb209.DigestateSeparatedFibreFarmSourced:
                {
                    return "Digestate - Separated Fibre Farm Sourced";
                }
            case ManureTypesRb209.PaperCrumbleChemicallyPhysicallyTreated:
                {
                    return "Paper Crumble - Chemically/Physically Treated";
                }
            case ManureTypesRb209.PaperCrumbleBiologicallyTreated:
                {
                    return "Paper Crumble - Biologically Treated";
                }
            case ManureTypesRb209.SpentMushroomCompost:
                {
                    return "Spent Mushroom Compost";
                }
            case ManureTypesRb209.WaterTreatmentCake:
                {
                    return "Water Treatment Cake";
                }
            case ManureTypesRb209.FoodWasteDairy:
                {
                    return "Food Waste - Dairy";
                }
            case ManureTypesRb209.FoodWasteSoftDrinks:
                {
                    return "Food Waste - Soft Drinks";
                }
            case ManureTypesRb209.FoodWasteBrewing:
                {
                    return "Food Waste - Brewing";
                }
            case ManureTypesRb209.FoodWasteGeneral:
                {
                    return "Food Waste - General";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ManureTypesRb209 [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a ManureTypes enumeration to a string (suitable for the XML document)
    /// </summary>
    public static ManureTypesRb209 ManureTypesRB209FromString(string enumValue)
    {
        switch (enumValue)
        {
            case "cattle fym - old":
                {
                    return ManureTypesRb209.CattleFYMOld;
                }
            case "cattle fym - fresh":
                {
                    return ManureTypesRb209.CattleFYMFresh;
                }
            case "pig fym - old":
                {
                    return ManureTypesRb209.PigFYMOld;
                }
            case "pig fym - fresh":
                {
                    return ManureTypesRb209.PigFYMFresh;
                }
            case "sheep fym - old":
                {
                    return ManureTypesRb209.SheepFYMOld;
                }
            case "sheep fym - fresh":
                {
                    return ManureTypesRb209.SheepFYMFresh;
                }
            case "duck fym - old":
                {
                    return ManureTypesRb209.DuckFYMOld;
                }
            case "duck fym - fresh":
                {
                    return ManureTypesRb209.DuckFYMFresh;
                }
            case "horse fym - old":
                {
                    return ManureTypesRb209.HorseFYMOld;
                }
            case "horse fym - fresh":
                {
                    return ManureTypesRb209.HorseFYMFresh;
                }
            case "goat fym - old":
                {
                    return ManureTypesRb209.GoatFYMOld;
                }
            case "goat fym - fresh":
                {
                    return ManureTypesRb209.GoatFYMFresh;
                }
            case "poultry manure 20%":
                {
                    return ManureTypesRb209.PoultryManure20Percent;
                }
            case "poultry manure 40%":
                {
                    return ManureTypesRb209.PoultryManure40Percent;
                }
            case "poultry manure 60%":
                {
                    return ManureTypesRb209.PoultryManure60Percent;
                }
            case "poultry manure 80%":
                {
                    return ManureTypesRb209.PoultryManure80Percent;
                }
            case "cattle slurry 2%":
                {
                    return ManureTypesRb209.CattleSlurry2Percent;
                }
            case "cattle slurry 6%":
                {
                    return ManureTypesRb209.CattleSlurry6Percent;
                }
            case "cattle slurry 10%":
                {
                    return ManureTypesRb209.CattleSlurry10Percent;
                }
            case "dirty water":
                {
                    return ManureTypesRb209.DirtyWater;
                }
            case "separated cattle slurry - strainer box":
                {
                    return ManureTypesRb209.SeparatedCattleSlurryStrainerBox;
                }
            case "separated cattle slurry - weeping wall":
                {
                    return ManureTypesRb209.SeparatedCattleSlurryWeepingWall;
                }
            case "separated cattle slurry - mechanical separator":
                {
                    return ManureTypesRb209.SeparatedCattleSlurryMechanicalSeparator;
                }
            case "separated cattle slurry - solid portion":
                {
                    return ManureTypesRb209.SeparatedCattleSlurrySolidPortion;
                }
            case "pig slurry 2%":
                {
                    return ManureTypesRb209.PigSlurry2Percent;
                }
            case "pig slurry 4%":
                {
                    return ManureTypesRb209.PigSlurry4Percent;
                }
            case "pig slurry 6%":
                {
                    return ManureTypesRb209.PigSlurry6Percent;
                }
            case "separated pig slurry - liquid portion":
                {
                    return ManureTypesRb209.SeparatedPigSlurryLiquidPortion;
                }
            case "separated pig slurry - solid portion":
                {
                    return ManureTypesRb209.SeparatedPigSlurrySolidPortion;
                }
            case "biosolids - digested cake":
                {
                    return ManureTypesRb209.BiosolidsDigestedCake;
                }
            case "biosolids - thermally dried":
                {
                    return ManureTypesRb209.BiosolidsThermallyDried;
                }
            case "biosolids - lime stabilised":
                {
                    return ManureTypesRb209.BiosolidsLimeStabilised;
                }
            case "biosolids - composted":
                {
                    return ManureTypesRb209.BiosolidsComposted;
                }
            case "green compost":
                {
                    return ManureTypesRb209.GreenCompost;
                }
            case "green/food compost":
                {
                    return ManureTypesRb209.GreenFoodCompost;
                }
            case "digestate - whole food based":
                {
                    return ManureTypesRb209.DigestateWholeFoodBased;
                }
            case "digestate - separated liquor food based":
                {
                    return ManureTypesRb209.DigestateSeparatedLiquorFoodBased;
                }
            case "digestate - separated fibre food based":
                {
                    return ManureTypesRb209.DigestateSeparatedFibreFoodBased;
                }
            case "digestate - whole farm sourced":
                {
                    return ManureTypesRb209.DigestateWholeFarmSourced;
                }
            case "digestate - separated liquor farm sourced":
                {
                    return ManureTypesRb209.DigestateSeparatedLiquorFarmSourced;
                }
            case "digestate - separated fibre farm sourced":
                {
                    return ManureTypesRb209.DigestateSeparatedFibreFarmSourced;
                }
            case "paper crumble - chemically/physically treated":
                {
                    return ManureTypesRb209.PaperCrumbleChemicallyPhysicallyTreated;
                }
            case "paper crumble - biologically treated":
                {
                    return ManureTypesRb209.PaperCrumbleBiologicallyTreated;
                }
            case "spent mushroom compost":
                {
                    return ManureTypesRb209.SpentMushroomCompost;
                }
            case "water treatment cake":
                {
                    return ManureTypesRb209.WaterTreatmentCake;
                }
            case "food waste - dairy":
                {
                    return ManureTypesRb209.FoodWasteDairy;
                }
            case "food waste - soft drinks":
                {
                    return ManureTypesRb209.FoodWasteSoftDrinks;
                }
            case "food waste - brewing":
                {
                    return ManureTypesRb209.FoodWasteBrewing;
                }
            case "food waste - general":
                {
                    return ManureTypesRb209.FoodWasteGeneral;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.ManureTypesRb209"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ManureTypesRb209 [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.ManureTypesRb209"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection ManureTypesRB209Names()
    {
        var ret = new StringCollection();
        var t = typeof(ManureTypes);
        foreach (ManureTypes e in Enum.GetValues(t))
            ret.Add(ManureTypesToString(e));
        return ret;
    }

#endregion
#region Enumeration 'ManureTypes'

    /// <summary>
    /// 	Converts a string to a ManureTypes enumeration
    /// </summary>
    public static string ManureTypesToString(ManureTypes enumValue)
    {
        switch (enumValue)
        {

            case ManureTypes.CattleFYMFresh:
                {
                    return "Cattle FYM - fresh";
                }
            case ManureTypes.CattleFYMOld:
                {
                    return "Cattle FYM - old";
                }
            case ManureTypes.PigFYMFresh:
                {
                    return "Pig FYM - fresh";
                }
            case ManureTypes.PigFYMOld:
                {
                    return "Pig FYM - old";
                }
            case ManureTypes.SheepFYMFresh:
                {
                    return "Sheep FYM - fresh";
                }
            case ManureTypes.SheepFYMOld:
                {
                    return "Sheep FYM - old";
                }
            case ManureTypes.DuckFYMFresh:
                {
                    return "Duck FYM - fresh";
                }
            case ManureTypes.DuckFYMOld:
                {
                    return "Duck FYM - old";
                }
            case ManureTypes.PoultryManure:
                {
                    return "Poultry manure"; // Use to return "Layer manure"
                }
            case ManureTypes.BroilerTurkeyLitter:
                {
                    return "Broiler / Turkey litter";
                }
            case ManureTypes.DairySlurry:
                {
                    return "Dairy slurry";
                }
            case ManureTypes.BeefSlurry:
                {
                    return "Beef slurry";
                }
            case ManureTypes.PigSlurry:
                {
                    return "Pig slurry";
                }
            case ManureTypes.CattleSlurryStrainerBoxLiquid:
                {
                    return "Cattle slurry, strainer box liquid";
                }
            case ManureTypes.CattleSlurryWeepingWallLiquid:
                {
                    return "Cattle slurry, weeping wall liquid";
                }
            case ManureTypes.CattleSlurryMechanicallySeparatedLiquid:
                {
                    return "Cattle slurry, mechanically separated liquid";
                }
            case ManureTypes.CattleSlurrySeparatedSolids:
                {
                    return "Cattle slurry, separated solids";
                }
            case ManureTypes.PigSlurrySeparatedSolids:
                {
                    return "Pig slurry, separated solids";
                }
            case ManureTypes.PigSlurrySeparatedLiquid:
                {
                    return "Pig slurry, separated liquid";
                }
            case ManureTypes.DirtyWater:
                {
                    return "Dirty water";
                }
            case ManureTypes.BiosolidsLiquidDigested:
                {
                    return "Biosolids, liquid digested";
                }
            case ManureTypes.BiosolidsDigestedCake:
                {
                    return "Biosolids, digested cake";
                }
            case ManureTypes.BiosolidsThermallyDried:
                {
                    return "Biosolids, thermally dried";
                }
            case ManureTypes.BiosolidsLimeStabilised:
                {
                    return "Biosolids, lime stabilised";
                }
            case ManureTypes.GreenCompost:
                {
                    return "Green compost";
                }
            case ManureTypes.PaperCrumble:
                {
                    return "Paper Crumble";
                }
            case ManureTypes.WasteFoodGeneral:
                {
                    return "Waste food, general";
                }
            case ManureTypes.WasteFoodDairy:
                {
                    return "Waste food, dairy";
                }
            case ManureTypes.WasteFoodSoftDrinks:
                {
                    return "Waste food, soft drinks";
                }
            case ManureTypes.WasteFoodBrewing:
                {
                    return "Waste food, brewing";
                }
            case ManureTypes.HorseFYM:
                {
                    return "Horse FYM";
                }
            case ManureTypes.BiosolidsThermallyHydrolysed:
                {
                    return "Biosolids, thermally hydrolysed";
                }
            case ManureTypes.GreenFoodCompost:
                {
                    return "Green / Food compost";
                }
            case ManureTypes.PaperCrumbleChemicallyPhysicallyTreated:
                {
                    return "Paper crumble, chemically / physically treated";
                }
            case ManureTypes.PaperCrumbleBiologicallyTreated:
                {
                    return "Paper crumble, biologically treated";
                }
            case ManureTypes.MushroomCompost:
                {
                    return "Mushroom compost";
                }
            case ManureTypes.WaterTreatmentCake:
                {
                    return "Water treatment cake";
                }
            case ManureTypes.DistilleryPotale:
                {
                    return "Distillery pot ale";
                }
            case ManureTypes.DistilleryEffluentSludge:
                {
                    return "Distillery bioplant sludge";
                }
            case ManureTypes.BreweryWashWater:
                {
                    return "Distillery effluent";
                }
            case ManureTypes.StrawMulch:
                {
                    return "Straw mulch";
                }
            case ManureTypes.GoatFYM:
                {
                    return "Goat FYM";
                }
            case ManureTypes.BiosolidsComposted:
                {
                    return "Biosolids, composted";
                }
            case ManureTypes.OtherSolidMaterials:
                {
                    return "Other - solid materials";
                }
            case ManureTypes.OtherLiquidMaterials:
                {
                    return "Other - liquid materials";
                }
            case ManureTypes.CattleSlurry:
                {
                    return "Cattle slurry";
                }
            case ManureTypes.DigestateWholeFoodBased:
                {
                    return "Digestate (whole), food-based";
                }
            case ManureTypes.DigestateWholePigSlurryBased:
                {
                    return "Digestate (whole), pig slurry-based";
                }
            case ManureTypes.DigestateWholeCattleSlurryBased:
                {
                    return "Digestate (whole), cattle slurry-based";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ManureTypes [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a ManureTypes enumeration to a string (suitable for the XML document)
    /// </summary>
    public static ManureTypes ManureTypesFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Cattle FYM - fresh":
                {
                    return ManureTypes.CattleFYMFresh;
                }
            case "Cattle FYM - old":
                {
                    return ManureTypes.CattleFYMOld;
                }
            case "Pig FYM - fresh":
                {
                    return ManureTypes.PigFYMFresh;
                }
            case "Pig FYM - old":
                {
                    return ManureTypes.PigFYMOld;
                }
            case "Sheep FYM - fresh":
                {
                    return ManureTypes.SheepFYMFresh;
                }
            case "Sheep FYM - old":
                {
                    return ManureTypes.SheepFYMOld;
                }
            case "Duck FYM - fresh":
                {
                    return ManureTypes.DuckFYMFresh;
                }
            case "Duck FYM - old":
                {
                    return ManureTypes.DuckFYMOld;
                }
            case "Poultry manure": // Case use was "Layer manure"
                {
                    return ManureTypes.PoultryManure;
                }
            case "Broiler / Turkey litter":
                {
                    return ManureTypes.BroilerTurkeyLitter;
                }
            case "Dairy slurry":
                {
                    return ManureTypes.DairySlurry;
                }
            case "Beef slurry":
                {
                    return ManureTypes.BeefSlurry;
                }
            case "Pig slurry":
                {
                    return ManureTypes.PigSlurry;
                }
            case "Cattle slurry, strainer box liquid":
                {
                    return ManureTypes.CattleSlurryStrainerBoxLiquid;
                }
            case "Cattle slurry, weeping wall liquid":
                {
                    return ManureTypes.CattleSlurryWeepingWallLiquid;
                }
            case "Cattle slurry, mechanically separated liquid":
                {
                    return ManureTypes.CattleSlurryMechanicallySeparatedLiquid;
                }
            case "Cattle slurry, separated solids":
                {
                    return ManureTypes.CattleSlurrySeparatedSolids;
                }
            case "Pig slurry, separated solids":
                {
                    return ManureTypes.PigSlurrySeparatedSolids;
                }
            case "Pig slurry, separated liquid":
                {
                    return ManureTypes.PigSlurrySeparatedLiquid;
                }
            case "Dirty water":
                {
                    return ManureTypes.DirtyWater;
                }
            case "Biosolids, liquid digested":
                {
                    return ManureTypes.BiosolidsLiquidDigested;
                }
            case "Biosolids, digested cake":
                {
                    return ManureTypes.BiosolidsDigestedCake;
                }
            case "Biosolids, thermally dried":
                {
                    return ManureTypes.BiosolidsThermallyDried;
                }
            case "Biosolids, lime stabilised":
                {
                    return ManureTypes.BiosolidsLimeStabilised;
                }
            case "Green compost":
                {
                    return ManureTypes.GreenCompost;
                }
            case "Paper Crumble":
                {
                    return ManureTypes.PaperCrumble;
                }
            case "Waste food, general":
                {
                    return ManureTypes.WasteFoodGeneral;
                }
            case "Waste food, dairy":
                {
                    return ManureTypes.WasteFoodDairy;
                }
            case "Waste food, soft drinks":
                {
                    return ManureTypes.WasteFoodSoftDrinks;
                }
            case "Waste food, brewing":
                {
                    return ManureTypes.WasteFoodBrewing;
                }
            case "Horse FYM":
                {
                    return ManureTypes.HorseFYM;
                }
            case "Biosolids, thermally hydrolysed":
                {
                    return ManureTypes.BiosolidsThermallyHydrolysed;
                }
            case "Green / Food compost":
                {
                    return ManureTypes.GreenFoodCompost;
                }
            case "Paper crumble, chemically / physically treated":
                {
                    return ManureTypes.PaperCrumbleChemicallyPhysicallyTreated;
                }
            case "Paper crumble, biologically treated":
                {
                    return ManureTypes.PaperCrumbleBiologicallyTreated;
                }
            case "Mushroom compost":
                {
                    return ManureTypes.MushroomCompost;
                }
            case "Water treatment cake":
                {
                    return ManureTypes.WaterTreatmentCake;
                }
            case "Distillery pot ale":
                {
                    return ManureTypes.DistilleryPotale;
                }
            case "Distillery bioplant sludge":
                {
                    return ManureTypes.DistilleryEffluentSludge;
                }
            case "Distillery effluent":
                {
                    return ManureTypes.BreweryWashWater;
                }
            case "Straw mulch":
                {
                    return ManureTypes.StrawMulch;
                }
            case "Goat FYM":
                {
                    return ManureTypes.GoatFYM;
                }
            case "Biosolids, composted":
                {
                    return ManureTypes.BiosolidsComposted;
                }
            case "Other - solid materials":
                {
                    return ManureTypes.OtherSolidMaterials;
                }
            case "Other - liquid materials":
                {
                    return ManureTypes.OtherLiquidMaterials;
                }
            case "Cattle slurry":
                {
                    return ManureTypes.CattleSlurry;
                }
            case "Digestate (whole), food-based":
                {
                    return ManureTypes.DigestateWholeFoodBased;
                }
            case "Digestate (whole), pig slurry-based":
                {
                    return ManureTypes.DigestateWholePigSlurryBased;
                }
            case "Digestate (whole), cattle slurry-based":
                {
                    return ManureTypes.DigestateWholeCattleSlurryBased;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.ManureTypes"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ManureTypes [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.ManureTypes"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection ManureTypesNames()
    {
        var ret = new StringCollection();
        var t = typeof(ManureTypes);
        foreach (ManureTypes e in Enum.GetValues(t))
            ret.Add(ManureTypesToString(e));
        return ret;
    }

#endregion
#region Enumeration 'ApplicationMethod'

    /// <summary>
    /// 	Converts a string to a ApplicationMethod enumeration
    /// </summary>
    public static string ApplicationMethodToString(ApplicationMethodEnum enumValue)
    {
        switch (enumValue)
        {

            case ApplicationMethodEnum.DischargeSpreader:
                {
                    return "Discharge spreader";
                }
            case ApplicationMethodEnum.BroadcastSpreader:
                {
                    return "Broadcast spreader";
                }
            case ApplicationMethodEnum.DeepInjection:
                {
                    return "Deep injection";
                }
            case ApplicationMethodEnum.ShallowInjection:
                {
                    return "Shallow injection";
                }
            case ApplicationMethodEnum.BandSpreaderTrailingHose:
                {
                    return "Band spreader - trailing hose";
                }
            case ApplicationMethodEnum.BandSpreaderTrailingShoeShortGrass:
                {
                    return "Band spreader - trailing shoe (short grass)";
                }
            case ApplicationMethodEnum.BandSpreaderTrailingShoeLongGrass:
                {
                    return "Band spreader - trailing shoe (long grass)";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ApplicationMethod [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a ApplicationMethod enumeration to a string (suitable for the XML document)
    /// </summary>
    public static ApplicationMethodEnum ApplicationMethodFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Discharge spreader":
                {
                    return ApplicationMethodEnum.DischargeSpreader;
                }
            case "Broadcast spreader":
                {
                    return ApplicationMethodEnum.BroadcastSpreader;
                }
            case "Deep injection":
                {
                    return ApplicationMethodEnum.DeepInjection;
                }
            case "Shallow injection":
                {
                    return ApplicationMethodEnum.ShallowInjection;
                }
            case "Band spreader - trailing hose":
                {
                    return ApplicationMethodEnum.BandSpreaderTrailingHose;
                }
            case "Band spreader - trailing shoe (short grass)":
                {
                    return ApplicationMethodEnum.BandSpreaderTrailingShoeShortGrass;
                }
            case "Band spreader - trailing shoe (long grass)":
                {
                    return ApplicationMethodEnum.BandSpreaderTrailingShoeLongGrass;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.ApplicationMethod"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ApplicationMethod [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.ApplicationMethod"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }
        }
    }

    public static StringCollection ApplicationMethodNames()
    {
        var ret = new StringCollection();
        var t = typeof(ApplicationMethodEnum);
        foreach (ApplicationMethodEnum e in Enum.GetValues(t))
            ret.Add(ApplicationMethodToString(e));
        return ret;
    }

#endregion
#region Enumeration 'WindSpeed'

    /// <summary>
    /// 	Converts a string to a WindSpeed enumeration
    /// </summary>
    public static string WindSpeedToString(WindSpeed enumValue)
    {
        switch (enumValue)
        {

            case WindSpeed.CalmGentle0To3BeaufortScale:
                {
                    return "Calm/Gentle (0-3 Beaufort Scale)";
                }
            case WindSpeed.Moderate4to5BeaufortScale:
                {
                    return "Moderate (4-5 Beaufort Scale)";
                }
            case WindSpeed.StrongBreeze6to7BeaufortScale:
                {
                    return "Strong Breeze (6-7 Beaufort Scale)";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.WindSpeed [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a WindSpeed enumeration to a string (suitable for the XML document)
    /// </summary>
    public static WindSpeed WindSpeedFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Calm/Gentle (0-3 Beaufort Scale)":
                {
                    return WindSpeed.CalmGentle0To3BeaufortScale;
                }
            case "Moderate (4-5 Beaufort Scale)":
                {
                    return WindSpeed.Moderate4to5BeaufortScale;
                }
            case "Strong Breeze (6-7 Beaufort Scale)":
                {
                    return WindSpeed.StrongBreeze6to7BeaufortScale;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.WindSpeed"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.WindSpeed [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.WindSpeed"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection WindSpeedNames()
    {
        var ret = new StringCollection();
        var t = typeof(WindSpeed);
        foreach (WindSpeed e in Enum.GetValues(t))
            ret.Add(WindSpeedToString(e));
        return ret;
    }

#endregion
#region Enumeration 'MethodOfIncorporation'

    /// <summary>
    /// 	Converts a string to a MethodOfIncorporation enumeration
    /// </summary>
    public static string MethodOfIncorporationToString(MethodOfIncorporationEnum enumValue)
    {
        switch (enumValue)
        {

            case MethodOfIncorporationEnum.ShallowInjection:
                {
                    return "Shallow injection";
                }
            case MethodOfIncorporationEnum.DeepInjection:
                {
                    return "Deep injection";
                }
            case MethodOfIncorporationEnum.TineCultivator:
                {
                    return "Tine cultivator";
                }
            case MethodOfIncorporationEnum.Discs:
                {
                    return "Discs";
                }
            case MethodOfIncorporationEnum.RotaryCultivator:
                {
                    return "Rotary cultivator";
                }
            case MethodOfIncorporationEnum.MouldboardPlough:
                {
                    return "Mouldboard plough";
                }
            case MethodOfIncorporationEnum.NotIncorporated:
                {
                    return "Not incorporated";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.MethodOfIncorporation [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a MethodOfIncorporation enumeration to a string (suitable for the XML document)
    /// </summary>
    public static MethodOfIncorporationEnum MethodOfIncorporationFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Shallow injection":
                {
                    return MethodOfIncorporationEnum.ShallowInjection;
                }
            case "Deep injection":
                {
                    return MethodOfIncorporationEnum.DeepInjection;
                }
            case "Tine cultivator":
                {
                    return MethodOfIncorporationEnum.TineCultivator;
                }
            case "Discs":
                {
                    return MethodOfIncorporationEnum.Discs;
                }
            case "Rotary cultivator":
                {
                    return MethodOfIncorporationEnum.RotaryCultivator;
                }
            case "Mouldboard plough":
                {
                    return MethodOfIncorporationEnum.MouldboardPlough;
                }
            case "Not incorporated":
                {
                    return MethodOfIncorporationEnum.NotIncorporated;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.MethodOfIncorporation"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.MethodOfIncorporation [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.MethodOfIncorporation"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }
        }
    }

    public static StringCollection MethodOfIncorporationNames()
    {
        var ret = new StringCollection();
        var t = typeof(MethodOfIncorporationEnum);
        foreach (MethodOfIncorporationEnum e in Enum.GetValues(t))
            ret.Add(MethodOfIncorporationToString(e));
        return ret;
    }

#endregion
#region Enumeration 'Rainfall'

    /// <summary>
    /// 	Converts a string to a Rainfall enumeration
    /// </summary>
    public static string RainfallToString(Rainfall enumValue)
    {
        switch (enumValue)
        {

            case Rainfall.NoRainfallWithin6HoursOfSpreading:
                {
                    return "No rainfall within 6 hours of spreading";
                }
            case Rainfall.LightRainLessthan5mmWithin6Hours:
                {
                    return "Light rain (less than 5 mm) within 6 hours";
                }
            case Rainfall.HeavyRainGreaterThan5mmWithin6hours:
                {
                    return "Heavy rain (>5 mm) within 6 hours";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.Rainfall [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a Rainfall enumeration to a string (suitable for the XML document)
    /// </summary>
    public static Rainfall RainfallFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "No rainfall within 6 hours of spreading":
                {
                    return Rainfall.NoRainfallWithin6HoursOfSpreading;
                }
            case "Light rain (less than 5 mm) within 6 hours":
                {
                    return Rainfall.LightRainLessthan5mmWithin6Hours;
                }
            case "Heavy rain (>=5 mm) within 6 hours":
                {
                    return Rainfall.HeavyRainGreaterThan5mmWithin6hours;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.Rainfall"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.Rainfall [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.Rainfall"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection RainfallNames()
    {
        var ret = new StringCollection();
        var t = typeof(Rainfall);
        foreach (Rainfall e in Enum.GetValues(t))
            ret.Add(RainfallToString(e));
        return ret;
    }

#endregion
#region Enumeration 'ManureCategory'

    /// <summary>
    /// 	Converts a string to a ManureCategory enumeration
    /// </summary>
    public static string ManureCategoryToString(ManureCategory enumValue)
    {
        switch (enumValue)
        {

            case ManureCategory.FYM:
                {
                    return "FYM";
                }
            case ManureCategory.Poultry:
                {
                    return "Poultry";
                }
            case ManureCategory.CattleSlurry:
                {
                    return "Cattle slurry";
                }
            case ManureCategory.PigSlurry:
                {
                    return "Pig slurry";
                }
            case ManureCategory.SolidSludge:
                {
                    return "Solid sludge";
                }
            case ManureCategory.LiquidSludge:
                {
                    return "Liquid sludge";
                }
            case ManureCategory.None:
                {
                    return "None";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ManureCategory [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a ManureCategory enumeration to a string (suitable for the XML document)
    /// </summary>
    public static ManureCategory ManureCategoryFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "FYM":
                {
                    return ManureCategory.FYM;
                }
            case "Poultry":
                {
                    return ManureCategory.Poultry;
                }
            case "Cattle slurry":
                {
                    return ManureCategory.CattleSlurry;
                }
            case "Pig slurry":
                {
                    return ManureCategory.PigSlurry;
                }
            case "Solid sludge":
                {
                    return ManureCategory.SolidSludge;
                }
            case "Liquid sludge":
                {
                    return ManureCategory.LiquidSludge;
                }
            case "None":
                {
                    return ManureCategory.None;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.ManureCategory"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.ManureCategory [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.ManureCategory"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection ManureCategoryNames()
    {
        var ret = new StringCollection();
        var t = typeof(ManureCategory);
        foreach (ManureCategory e in Enum.GetValues(t))
            ret.Add(ManureCategoryToString(e));
        return ret;
    }

#endregion
#region Enumeration 'CropTypeEnum'

    /// <summary>
    /// 	Converts a string to a CropTypeEngland enumeration
    /// </summary>
    public static string CropTypeEnumToString(CropTypeEnum enumValue)
    {
        switch (enumValue)
        {

            case CropTypeEnum.Grass:
                {
                    return "Grass";
                }
            case CropTypeEnum.EarlySownWinterCereal:
                {
                    return "Early sown winter cereal";
                }
            case CropTypeEnum.LateSownWinterCereal:
                {
                    return "Late sown winter cereal";
                }
            case CropTypeEnum.EarlyEstablishedWinterOilseedRape:
                {
                    return "Early established winter oilseed rape";
                }
            case CropTypeEnum.LateEstablishedWinterOilseedRape:
                {
                    return "Late established winter oilseed rape";
                }
            case CropTypeEnum.SpringCerealOilseedRape:
                {
                    return "Spring cereal/oilseed rape";
                }
            case CropTypeEnum.Potatoes:
                {
                    return "Potatoes";
                }
            case CropTypeEnum.Sugarbeet:
                {
                    return "Sugar beet";
                }
            case CropTypeEnum.Other:
                {
                    return "Other";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.CropTypeEngland [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a CropTypeEngland enumeration to a string (suitable for the XML document)
    /// </summary>
    public static CropTypeEnum CropTypeEnumFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Grass":
                {
                    return CropTypeEnum.Grass;
                }
            case "Early sown winter cereal":
                {
                    return CropTypeEnum.EarlySownWinterCereal;
                }
            case "Late sown winter cereal":
                {
                    return CropTypeEnum.LateSownWinterCereal;
                }
            case "Early established winter oilseed rape":
                {
                    return CropTypeEnum.EarlyEstablishedWinterOilseedRape;
                }
            case "Late established winter oilseed rape":
                {
                    return CropTypeEnum.LateEstablishedWinterOilseedRape;
                }
            case "Spring cereal/oilseed rape":
                {
                    return CropTypeEnum.SpringCerealOilseedRape;
                }
            case "Potatoes":
                {
                    return CropTypeEnum.Potatoes;
                }
            case "Sugar beet":
                {
                    return CropTypeEnum.Sugarbeet;
                }
            case "Other":
                {
                    return CropTypeEnum.Other;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.CropTypeEngland"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.CropTypeEngland [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.CropTypeEngland"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection CropTypeEnumNames()
    {
        var ret = new StringCollection();
        var t = typeof(CropTypeEnum);
        foreach (CropTypeEnum e in Enum.GetValues(t))
            ret.Add(CropTypeEnumToString(e));
        return ret;
    }

#endregion
#region Enumeration 'DelayToIncorporation'        

    /// <summary>
    /// 	Converts a string to a DelayToIncorporation enumeration
    /// </summary>
    public static string DelayToIncorporationToString(DelayToIncorporationEnum enumValue)
    {
        switch (enumValue)
        {

            case DelayToIncorporationEnum.Injection:
                {
                    return "Injection";
                }
            case DelayToIncorporationEnum.LessThan2Hours:
                {
                    return "Less than 2 hours";
                }
            case DelayToIncorporationEnum.n2To4Hours:
                {
                    return "2-4 hours";
                }
            case DelayToIncorporationEnum.n4To6Hours:
                {
                    return "4-6 hours";
                }
            case DelayToIncorporationEnum.n6To12Hours:
                {
                    return "6-12 hours";
                }
            case DelayToIncorporationEnum.n12To24Hours:
                {
                    return "12-24 hours";
                }
            case DelayToIncorporationEnum.n1To2Days:
                {
                    return "1-2 days";
                }
            case DelayToIncorporationEnum.n3To5Days:
                {
                    return "3-5 days";
                }
            case DelayToIncorporationEnum.n3To7Days:
                {
                    return "3-7 days";
                }
            case DelayToIncorporationEnum.n6To12Days:
                {
                    return "6-12 days";
                }
            case DelayToIncorporationEnum.GreaterThan7Days:
                {
                    return ">7 days";
                }
            case DelayToIncorporationEnum.GreaterThan12Days:
                {
                    return ">12 days";
                }
            case DelayToIncorporationEnum.n12To32Days:
                {
                    return "12-32 days";
                }
            case DelayToIncorporationEnum.GreaterThan32Days:
                {
                    return ">32 days";
                }
            case DelayToIncorporationEnum.NotIncorporated:
                {
                    return "Not incorporated";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.DelayToIncorporation [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a DelayToIncorporation enumeration to a string (suitable for the XML document)
    /// </summary>
    public static DelayToIncorporationEnum DelayToIncorporationFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Injection":
                {
                    return DelayToIncorporationEnum.Injection;
                }
            case "Less than 2 hours":
                {
                    return DelayToIncorporationEnum.LessThan2Hours;
                }
            case "2-4 hours":
                {
                    return DelayToIncorporationEnum.n2To4Hours;
                }
            case "4-6 hours":
                {
                    return DelayToIncorporationEnum.n4To6Hours;
                }
            case "6-12 hours":
                {
                    return DelayToIncorporationEnum.n6To12Hours;
                }
            case "12-24 hours":
                {
                    return DelayToIncorporationEnum.n12To24Hours;
                }
            case "1-2 days":
                {
                    return DelayToIncorporationEnum.n1To2Days;
                }
            case "3-5 days":
                {
                    return DelayToIncorporationEnum.n3To5Days;
                }
            case "3-7 days":
                {
                    return DelayToIncorporationEnum.n3To7Days;
                }
            case "6-12 days":
                {
                    return DelayToIncorporationEnum.n6To12Days;
                }
            case ">7 days":
                {
                    return DelayToIncorporationEnum.GreaterThan7Days;
                }
            case ">12 days":
                {
                    return DelayToIncorporationEnum.GreaterThan12Days;
                }
            case "12-32 days":
                {
                    return DelayToIncorporationEnum.n12To32Days;
                }
            case ">32 days":
                {
                    return DelayToIncorporationEnum.GreaterThan32Days;
                }
            case "Not incorporated":
                {
                    return DelayToIncorporationEnum.NotIncorporated;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.DelayToIncorporation"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.DelayToIncorporation [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.DelayToIncorporation"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection DelayToIncorporationNames()
    {
        var ret = new StringCollection();
        var t = typeof(DelayToIncorporationEnum);
        foreach (DelayToIncorporationEnum e in Enum.GetValues(t))
            ret.Add(DelayToIncorporationToString(e));
        return ret;
    }

#endregion
#region Enumeration 'TopsoilMoistureEnum'

    /// <summary>
    /// 	Converts a string to a TopSoilMoisture enumeration
    /// </summary>
    public static string TopsoilMoistureToString(TopsoilMoistureEnum enumValue)
    {
        switch (enumValue)
        {

            case TopsoilMoistureEnum.Dry:
                {
                    return "Dry";
                }
            case TopsoilMoistureEnum.Moist:
                {
                    return "Moist";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.TopSoilMoisture [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a TopSoilMoisture enumeration to a string (suitable for the XML document)
    /// </summary>
    public static TopsoilMoistureEnum TopsoilMoistureFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Dry":
                {
                    return TopsoilMoistureEnum.Dry;
                }
            case "Moist":
                {
                    return TopsoilMoistureEnum.Moist;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.TopSoilMoisture"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.TopSoilMoisture [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.TopSoilMoisture"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection TopsoilMoistureNames()
    {
        var ret = new StringCollection();
        var t = typeof(TopsoilMoistureEnum);
        foreach (TopsoilMoistureEnum e in Enum.GetValues(t))
            ret.Add(TopsoilMoistureToString(e));
        return ret;
    }

#endregion
#region Enumeration 'SoilType'

    /// <summary>
    /// 	Converts a string to a SoilEngland enumeration
    /// </summary>
    public static string SoilTypeToString(SoilType enumValue)
    {
        switch (enumValue)
        {

            case SoilType.Sand:
                {
                    return "Sand";
                }
            case SoilType.LoamySand:
                {
                    return "Loamy Sand";
                }
            case SoilType.SandyLoam:
                {
                    return "Sandy Loam";
                }
            case SoilType.FineSandyLoam:
                {
                    return "Fine Sandy Loam";
                }
            case SoilType.SandySiltLoam:
                {
                    return "Sandy Silt Loam";
                }
            case SoilType.SiltLoam:
                {
                    return "Silt Loam";
                }
            case SoilType.SiltyClayLoam:
                {
                    return "Silty Clay Loam";
                }
            case SoilType.SandyClayLoam:
                {
                    return "Sandy Clay Loam";
                }
            case SoilType.ClayLoam:
                {
                    return "Clay Loam";
                }
            case SoilType.SandyClay:
                {
                    return "Sandy Clay";
                }
            case SoilType.SiltyClay:
                {
                    return "Silty Clay";
                }
            case SoilType.Clay:
                {
                    return "Clay";
                }
            case SoilType.Organic:
                {
                    return "Organic";
                }
            case SoilType.Peaty:
                {
                    return "Peaty";
                }
            case SoilType.Peat:
                {
                    return "Peat";
                }
            case SoilType.Chalk:
                {
                    return "Chalk";
                }
            case SoilType.RocknotChalk:
                {
                    return "Rock (not Chalk)";
                }

            default:
                {
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.SoilEngland [" + enumValue.ToString() + "]");
                }
        }
    }

    /// <summary>
    /// 	Converts a SoilEngland enumeration to a string (suitable for the XML document)
    /// </summary>
    public static SoilType SoilTypeFromString(string enumValue)
    {
        switch (enumValue)
        {
            case "Sand":
                {
                    return SoilType.Sand;
                }
            case "Loamy Sand":
                {
                    return SoilType.LoamySand;
                }
            case "Sandy Loam":
                {
                    return SoilType.SandyLoam;
                }
            case "Fine Sandy Loam":
                {
                    return SoilType.FineSandyLoam;
                }
            case "Sandy Silt Loam":
                {
                    return SoilType.SandySiltLoam;
                }
            case "Silt Loam":
                {
                    return SoilType.SiltLoam;
                }
            case "Silty Clay Loam":
                {
                    return SoilType.SiltyClayLoam;
                }
            case "Sandy Clay Loam":
                {
                    return SoilType.SandyClayLoam;
                }
            case "Clay Loam":
                {
                    return SoilType.ClayLoam;
                }
            case "Sandy Clay":
                {
                    return SoilType.SandyClay;
                }
            case "Silty Clay":
                {
                    return SoilType.SiltyClay;
                }
            case "Clay":
                {
                    return SoilType.Clay;
                }
            case "Organic":
                {
                    return SoilType.Organic;
                }
            case "Peaty":
                {
                    return SoilType.Peaty;
                }
            case "Peat":
                {
                    return SoilType.Peat;
                }
            case "Chalk":
                {
                    return SoilType.Chalk;
                }
            case "Rock (not Chalk)":
                {
                    return SoilType.RocknotChalk;
                }

            default:
                {
                    // ##HAND_CODED_BLOCK_START ID="Default Enum Enums.Enumerations.SoilEngland"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                    throw new Exception("Unknown enumeration value for Enums.Enumerations.SoilEngland [" + enumValue + "]");
                    // ##HAND_CODED_BLOCK_END ID="Default Enum Enums.Enumerations.SoilEngland"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
                }

        }
    }

    public static StringCollection SoilTypeNames()
    {
        var ret = new StringCollection();
        var t = typeof(SoilType);
        foreach (SoilType e in Enum.GetValues(t))
            ret.Add(SoilTypeToString(e));
        return ret;
    }

    #endregion

}
