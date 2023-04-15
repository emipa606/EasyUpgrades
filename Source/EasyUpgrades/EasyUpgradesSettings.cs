using Verse;

namespace EasyUpgrades;

internal class EasyUpgradesSettings : ModSettings
{
    public static float increaseAwfulQualityChance = 0.95f;

    public static float increasePoorQualityChance = 0.9f;

    public static float increaseNormalQualityChance = 0.85f;

    public static float increaseGoodQualityChance = 0.6f;

    public static float increaseExcellentQualityChance = 0.25f;

    public static float increaseMasterworkQualityChance = 0.15f;

    public static float decreasePoorQualityChance = 0.02f;

    public static float decreaseNormalQualityChance = 0.07f;

    public static float decreaseGoodQualityChance = 0.12f;

    public static float decreaseExcellentQualityChance = 0.19f;

    public static float decreaseMasterworkQualityChance = 0.25f;

    public static float neededMaterialsAwfulQuality = 0.2f;

    public static float neededMaterialsPoorQuality = 0.6f;

    public static float neededMaterialsNormalQuality = 0.9f;

    public static float neededMaterialsGoodQuality = 1.25f;

    public static float neededMaterialsExcellentQuality = 2f;

    public static float neededMaterialsMasterworkQuality = 3f;

    public static int maxUpgradableQuality = 6;


    public static void Reset()
    {
        increaseAwfulQualityChance = 0.95f;
        increasePoorQualityChance = 0.9f;
        increaseNormalQualityChance = 0.85f;
        increaseGoodQualityChance = 0.6f;
        increaseExcellentQualityChance = 0.25f;
        increaseMasterworkQualityChance = 0.15f;
        decreasePoorQualityChance = 0.02f;
        decreaseNormalQualityChance = 0.07f;
        decreaseGoodQualityChance = 0.12f;
        decreaseExcellentQualityChance = 0.19f;
        decreaseMasterworkQualityChance = 0.25f;
        neededMaterialsAwfulQuality = 0.2f;
        neededMaterialsPoorQuality = 0.6f;
        neededMaterialsNormalQuality = 0.9f;
        neededMaterialsGoodQuality = 1.25f;
        neededMaterialsExcellentQuality = 2f;
        neededMaterialsMasterworkQuality = 3f;
        maxUpgradableQuality = 6;
    }


    public override void ExposeData()
    {
        Scribe_Values.Look(ref increaseAwfulQualityChance, "increaseAwfulQualityChance", 0.95f);
        Scribe_Values.Look(ref increasePoorQualityChance, "increasePoorQualityChance", 0.9f);
        Scribe_Values.Look(ref increaseNormalQualityChance, "increaseNormalQualityChance", 0.85f);
        Scribe_Values.Look(ref increaseGoodQualityChance, "increaseGoodQualityChance", 0.6f);
        Scribe_Values.Look(ref increaseExcellentQualityChance, "increaseExcellentQualityChance", 0.25f);
        Scribe_Values.Look(ref increaseMasterworkQualityChance, "increaseMasterworkQualityChance", 0.15f);
        Scribe_Values.Look(ref decreasePoorQualityChance, "decreasePoorQualityChance", 0.02f);
        Scribe_Values.Look(ref decreaseNormalQualityChance, "decreaseNormalQualityChance", 0.07f);
        Scribe_Values.Look(ref decreaseGoodQualityChance, "decreaseGoodQualityChance", 0.12f);
        Scribe_Values.Look(ref decreaseExcellentQualityChance, "decreaseExcellentQualityChance", 0.19f);
        Scribe_Values.Look(ref decreaseMasterworkQualityChance, "decreaseMasterworkQualityChance", 0.25f);
        Scribe_Values.Look(ref neededMaterialsAwfulQuality, "neededMaterialsAwfulQuality", 0.2f);
        Scribe_Values.Look(ref neededMaterialsPoorQuality, "neededMaterialsPoorQuality", 0.6f);
        Scribe_Values.Look(ref neededMaterialsNormalQuality, "neededMaterialsNormalQuality", 0.9f);
        Scribe_Values.Look(ref neededMaterialsGoodQuality, "neededMaterialsGoodQuality", 1.25f);
        Scribe_Values.Look(ref neededMaterialsExcellentQuality, "neededMaterialsExcellentQuality", 2f);
        Scribe_Values.Look(ref neededMaterialsMasterworkQuality, "neededMaterialsMasterworkQuality", 3f);
        Scribe_Values.Look(ref maxUpgradableQuality, "maxUpgradableQuality", 6);
        base.ExposeData();
    }


    public static ThingDef GetReplacementThingDef(ThingDef def)
    {
        if (def.modExtensions == null)
        {
            return def;
        }

        if (!def.modExtensions.Any())
        {
            return def;
        }

        var possibleVFEExtension =
            def.modExtensions.FirstOrDefault(
                extension => extension.GetType().Name == "StuffExtension_Cost");

        if (possibleVFEExtension == null)
        {
            return def;
        }

        try
        {
            var replacementDef = (ThingDef)possibleVFEExtension.GetType().GetField("thingDef")
                ?.GetValue(possibleVFEExtension);
            return replacementDef ?? def;
        }
        catch
        {
            return def;
        }
    }
}