using RimWorld;
using Verse;

namespace EasyUpgrades;

[DefOf]
public static class EasyUpgradesDesignationDefOf
{
    public static DesignationDef Upgrade;

    public static DesignationDef Downgrade;

    public static DesignationDef IncreaseQuality_Building;

    public static DesignationDef IncreaseQuality_Apparel;

    public static DesignationDef IncreaseQuality_Item;

    public static DesignationDef IncreaseQuality_Art;

    static EasyUpgradesDesignationDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(EasyUpgradesDesignationDefOf));
    }
}