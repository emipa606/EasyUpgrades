using RimWorld;
using UnityEngine;
using Verse;

namespace EasyUpgrades;

public class Designator_IncreaseQuality : Designator
{
    public Designator_IncreaseQuality()
    {
        defaultLabel = "EU.IncreaseQuality".Translate();
        defaultDesc = "EU.IncreaseQualityTooltip".Translate();
        icon = ContentFinder<Texture2D>.Get("UI/QualityUp");
        soundDragSustain = SoundDefOf.Designate_DragStandard;
        soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
        useMouseIcon = true;
        soundSucceeded = SoundDefOf.Checkbox_TurnedOff;
        hotKey = KeyBindingDefOf.Command_ItemForbid;
        hasDesignateAllFloatMenuOption = true;
        designateAllLabel = "EU.IncreaseQualityOfAll".Translate();
    }

    public override int DraggableDimensions => 2;

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
        if (!c.InBounds(Map) || c.Fogged(Map))
        {
            return false;
        }

        if (!c.GetThingList(Map).Any(t => CanDesignateThing(t).Accepted))
        {
            return "EU.MustDesignateItemOfQuality".Translate();
        }

        return true;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        var thingList = c.GetThingList(Map);
        for (var i = 0; i < thingList.Count; i++)
        {
            if (CanDesignateThing(thingList[i]).Accepted)
            {
                DesignateThing(thingList[i]);
            }
        }
    }

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        if (!t.def.HasComp(typeof(CompQuality)))
        {
            return false;
        }

        if (t.Faction == Faction.OfPlayer)
        {
            if (t.def.IsArt || t is Building)
            {
                return true;
            }
        }

        if (t.Faction != null)
        {
            return false;
        }

        if ((t.def.IsApparel || t.def.IsWeapon) &&
            WorkGiver_IncreaseQuality.GetNeededCraftingBenchName(t) == string.Empty)
        {
            return false;
        }

        return t.def.IsApparel || t.def.IsWeapon;
    }

    public override void DesignateThing(Thing t)
    {
        var designator = EasyUpgradesDesignationDefOf.IncreaseQuality_Building;

        if (t.def.IsArt)
        {
            designator = EasyUpgradesDesignationDefOf.IncreaseQuality_Art;
        }

        if (t.def.IsApparel)
        {
            designator = EasyUpgradesDesignationDefOf.IncreaseQuality_Apparel;
        }

        if (t.def.IsWeapon)
        {
            designator = EasyUpgradesDesignationDefOf.IncreaseQuality_Item;
        }

        if (t.Map.designationManager.DesignationOn(t, designator) == null)
        {
            t.Map.designationManager.AddDesignation(new Designation(t, designator));
        }
    }
}