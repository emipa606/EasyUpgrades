using RimWorld;
using Verse;

namespace EasyUpgrades;

[DefOf]
public static class EasyUpgradesJobDefOf
{
    public static JobDef UpgradeThing;

    public static JobDef DowngradeThing;

    public static JobDef IncreaseQuality_Building;

    public static JobDef IncreaseQuality_Crafting;

    public static JobDef IncreaseQuality_Artistic;

    static EasyUpgradesJobDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(EasyUpgradesJobDefOf));
    }
}