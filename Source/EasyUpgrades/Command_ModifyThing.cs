using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EasyUpgrades;

public class Command_ModifyThing : Command
{
    public ThingWithComps CurrentThing;

    public DesignationDef Def;

    private static DesignationDef UninstallDef => DesignationDefOf.Uninstall;

    private static DesignationDef DeconstructDef => DesignationDefOf.Deconstruct;

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
        ApplyDesignation(CurrentThing, Def);
    }

    public static void ApplyDesignation(Thing thing, DesignationDef designationDef)
    {
        var designationManager = thing.Map.designationManager;
        if (designationManager.DesignationOn(thing, designationDef) != null)
        {
            return;
        }

        designationManager.TryRemoveDesignationOn(thing, UninstallDef);
        designationManager.TryRemoveDesignationOn(thing, DeconstructDef);

        if (designationDef == EasyUpgradesDesignationDefOf.Upgrade)
        {
            designationManager.TryRemoveDesignationOn(thing, EasyUpgradesDesignationDefOf.Downgrade);
        }
        else if (designationDef == EasyUpgradesDesignationDefOf.Downgrade)
        {
            designationManager.TryRemoveDesignationOn(thing, EasyUpgradesDesignationDefOf.Upgrade);
        }

        designationManager.AddDesignation(new Designation(thing, designationDef));
    }
}
