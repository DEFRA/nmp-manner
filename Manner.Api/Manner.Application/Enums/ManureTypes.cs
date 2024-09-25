namespace Manner.Application.Enums;
public partial class Enumerations
{    
    #region Enumeration 'ManureTypes'

    public enum ManureTypes
    {
        CattleFYMFresh = 0,
        CattleFYMOld = 1,
        PigFYMFresh = 2,
        PigFYMOld = 3,
        SheepFYMFresh = 4,
        SheepFYMOld = 5,
        DuckFYMFresh = 6,
        DuckFYMOld = 7,
        PoultryManure = 8, // Was called LayerManure rename PoultryManure
        BroilerTurkeyLitter = 9,
        DairySlurry = 10,
        BeefSlurry = 11,
        PigSlurry = 12,
        CattleSlurryStrainerBoxLiquid = 13,
        CattleSlurryWeepingWallLiquid = 14,
        CattleSlurryMechanicallySeparatedLiquid = 15,
        CattleSlurrySeparatedSolids = 16,
        PigSlurrySeparatedSolids = 17,
        PigSlurrySeparatedLiquid = 18,
        DirtyWater = 19,
        BiosolidsLiquidDigested = 20,
        BiosolidsDigestedCake = 21,
        BiosolidsThermallyDried = 22,
        BiosolidsLimeStabilised = 23,
        GreenCompost = 24,
        PaperCrumble = 25,
        WasteFoodGeneral = 26,
        WasteFoodDairy = 27,
        WasteFoodSoftDrinks = 28,
        WasteFoodBrewing = 29,
        HorseFYM = 30,
        BiosolidsThermallyHydrolysed = 31,
        GreenFoodCompost = 32,
        PaperCrumbleChemicallyPhysicallyTreated = 33,
        PaperCrumbleBiologicallyTreated = 34,
        MushroomCompost = 35,
        WaterTreatmentCake = 36,
        DistilleryPotale = 37,
        DistilleryEffluentSludge = 38, // Was called DistilleryBioplantSludge rename DistilleryEffluentSludge
        BreweryWashWater = 39, // Was called DistilleryEffluent renamed BreweryWashWater
        StrawMulch = 40,
        GoatFYM = 41,
        BiosolidsComposted = 42,
        OtherSolidMaterials = 43,
        OtherLiquidMaterials = 44,
        CattleSlurry = 45,
        DigestateWholeFoodBased = 46,
        DigestateWholePigSlurryBased = 47,
        DigestateWholeCattleSlurryBased = 48,
        DigestateSeparatedLiquorFoodBased = 49, // New from excel
        DigestateSeparatedFibreFoodBased = 50, // New from excel
        DigestateWholeFarmSourced = 51, // New from excel
        DigestateSeparatedLiquorFarmSourced = 52, // New from excel
        DigestateSeparatedFibreFarmSourced = 53 // New from excel
    }

    #endregion

}
