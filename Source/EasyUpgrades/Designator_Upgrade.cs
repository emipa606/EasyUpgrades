using RimWorld;
using UnityEngine;
using Verse;

namespace EasyUpgrades;

public class Designator_Upgrade : Designator
{
    public Designator_Upgrade()
    {
        defaultLabel = "EU.Upgrade".Translate();
        defaultDesc = "EU.UpgradeTooltip".Translate();
        icon = ContentFinder<Texture2D>.Get("UI/Up");
        soundDragSustain = SoundDefOf.Designate_DragStandard;
        soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
        soundSucceeded = SoundDefOf.Checkbox_TurnedOff;
        useMouseIcon = true;
        hasDesignateAllFloatMenuOption = true;
        designateAllLabel = "EU.UpgradeOfAll".Translate();
    }

    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Orders;

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
        if (!c.InBounds(Map) || c.Fogged(Map))
        {
            return false;
        }

        if (!c.GetThingList(Map).Any(t => CanDesignateThing(t).Accepted))
        {
            return "EU.MustDesignateUpgradeableThing".Translate();
        }

        return true;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        foreach (var thing in c.GetThingList(Map))
        {
            if (CanDesignateThing(thing).Accepted)
            {
                DesignateThing(thing);
            }
        }
    }

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        if (t.Faction != Faction.OfPlayer)
        {
            return false;
        }

        var compUpgrade = t.TryGetComp<CompUpgrade>();
        if (compUpgrade?.upgradeTo == null)
        {
            return false;
        }

        if (t.Map.designationManager.DesignationOn(t, EasyUpgradesDesignationDefOf.Upgrade) != null)
        {
            return false;
        }

        var props = compUpgrade.props as CompProperties_Upgradable;
        if (props?.researchPrerequisite is { IsFinished: false })
        {
            return false;
        }

        return props?.researchPrerequisites?.Any(project => !project.IsFinished) != true;
    }

    public override void DesignateThing(Thing t)
    {
        Command_ModifyThing.ApplyDesignation(t, EasyUpgradesDesignationDefOf.Upgrade);
    }
}