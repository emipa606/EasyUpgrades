using Verse;

namespace EasyUpgrades;

internal class EasyUpgradesSettings : ModSettings
{
    public static float IncreaseAwfulQualityChance = 0.95f;

    public static float IncreasePoorQualityChance = 0.9f;

    public static float IncreaseNormalQualityChance = 0.85f;

    public static float IncreaseGoodQualityChance = 0.6f;

    public static float IncreaseExcellentQualityChance = 0.25f;

    public static float IncreaseMasterworkQualityChance = 0.15f;

    public static float DecreasePoorQualityChance = 0.02f;

    public static float DecreaseNormalQualityChance = 0.07f;

    public static float DecreaseGoodQualityChance = 0.12f;

    public static float DecreaseExcellentQualityChance = 0.19f;

    public static float DecreaseMasterworkQualityChance = 0.25f;

    public static float NeededMaterialsAwfulQuality = 0.2f;

    public static float NeededMaterialsPoorQuality = 0.6f;

    public static float NeededMaterialsNormalQuality = 0.9f;

    public static float NeededMaterialsGoodQuality = 1.25f;

    public static float NeededMaterialsExcellentQuality = 2f;

    public static float NeededMaterialsMasterworkQuality = 3f;

    public static int MaxUpgradableQuality = 6;


    public static void Reset()
    {
        IncreaseAwfulQualityChance = 0.95f;
        IncreasePoorQualityChance = 0.9f;
        IncreaseNormalQualityChance = 0.85f;
        IncreaseGoodQualityChance = 0.6f;
        IncreaseExcellentQualityChance = 0.25f;
        IncreaseMasterworkQualityChance = 0.15f;
        DecreasePoorQualityChance = 0.02f;
        DecreaseNormalQualityChance = 0.07f;
        DecreaseGoodQualityChance = 0.12f;
        DecreaseExcellentQualityChance = 0.19f;
        DecreaseMasterworkQualityChance = 0.25f;
        NeededMaterialsAwfulQuality = 0.2f;
        NeededMaterialsPoorQuality = 0.6f;
        NeededMaterialsNormalQuality = 0.9f;
        NeededMaterialsGoodQuality = 1.25f;
        NeededMaterialsExcellentQuality = 2f;
        NeededMaterialsMasterworkQuality = 3f;
        MaxUpgradableQuality = 6;
    }


    public override void ExposeData()
    {
        Scribe_Values.Look(ref IncreaseAwfulQualityChance, "increaseAwfulQualityChance", 0.95f);
        Scribe_Values.Look(ref IncreasePoorQualityChance, "increasePoorQualityChance", 0.9f);
        Scribe_Values.Look(ref IncreaseNormalQualityChance, "increaseNormalQualityChance", 0.85f);
        Scribe_Values.Look(ref IncreaseGoodQualityChance, "increaseGoodQualityChance", 0.6f);
        Scribe_Values.Look(ref IncreaseExcellentQualityChance, "increaseExcellentQualityChance", 0.25f);
        Scribe_Values.Look(ref IncreaseMasterworkQualityChance, "increaseMasterworkQualityChance", 0.15f);
        Scribe_Values.Look(ref DecreasePoorQualityChance, "decreasePoorQualityChance", 0.02f);
        Scribe_Values.Look(ref DecreaseNormalQualityChance, "decreaseNormalQualityChance", 0.07f);
        Scribe_Values.Look(ref DecreaseGoodQualityChance, "decreaseGoodQualityChance", 0.12f);
        Scribe_Values.Look(ref DecreaseExcellentQualityChance, "decreaseExcellentQualityChance", 0.19f);
        Scribe_Values.Look(ref DecreaseMasterworkQualityChance, "decreaseMasterworkQualityChance", 0.25f);
        Scribe_Values.Look(ref NeededMaterialsAwfulQuality, "neededMaterialsAwfulQuality", 0.2f);
        Scribe_Values.Look(ref NeededMaterialsPoorQuality, "neededMaterialsPoorQuality", 0.6f);
        Scribe_Values.Look(ref NeededMaterialsNormalQuality, "neededMaterialsNormalQuality", 0.9f);
        Scribe_Values.Look(ref NeededMaterialsGoodQuality, "neededMaterialsGoodQuality", 1.25f);
        Scribe_Values.Look(ref NeededMaterialsExcellentQuality, "neededMaterialsExcellentQuality", 2f);
        Scribe_Values.Look(ref NeededMaterialsMasterworkQuality, "neededMaterialsMasterworkQuality", 3f);
        Scribe_Values.Look(ref MaxUpgradableQuality, "maxUpgradableQuality", 6);
        base.ExposeData();
    }


    public static ThingDef GetReplacementThingDef(ThingDef def)
    {
        if (def.modExtensions == null || !def.modExtensions.Any())
        {
            return def;
        }

        var possibleVfeExtension =
            def.modExtensions.FirstOrDefault(extension => extension.GetType().Name == "StuffExtension_Cost");

        if (possibleVfeExtension == null)
        {
            return def;
        }

        try
        {
            var replacementDef = (ThingDef)possibleVfeExtension.GetType().GetField("thingDef")
                ?.GetValue(possibleVfeExtension);
            return replacementDef ?? def;
        }
        catch
        {
            return def;
        }
    }
}