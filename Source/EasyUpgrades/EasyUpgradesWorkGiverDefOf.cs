using RimWorld;

namespace EasyUpgrades;

[DefOf]
public static class EasyUpgradesWorkGiverDefOf
{
    public static WorkGiverDef UpgradeThing;

    public static WorkGiverDef DowngradeThing;

    public static WorkGiverDef IncreaseQuality_Building;

    public static WorkGiverDef IncreaseQuality_Apparel;

    public static WorkGiverDef IncreaseQuality_Item;

    public static WorkGiverDef IncreaseQuality_Art;

    static EasyUpgradesWorkGiverDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(EasyUpgradesWorkGiverDefOf));
    }
}