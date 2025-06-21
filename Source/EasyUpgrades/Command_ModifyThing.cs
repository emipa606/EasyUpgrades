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
        var designationManager = CurrentThing.Map.designationManager;
        var designation = designationManager.DesignationOn(CurrentThing, Def);
        var designation2 = designationManager.DesignationOn(CurrentThing, UninstallDef);
        var designation3 = designationManager.DesignationOn(CurrentThing, DeconstructDef);
        if (designation != null)
        {
            return;
        }

        if (designation2 != null)
        {
            designationManager.TryRemoveDesignationOn(CurrentThing, UninstallDef);
        }

        if (designation3 != null)
        {
            designationManager.TryRemoveDesignationOn(CurrentThing, DeconstructDef);
        }

        designationManager.AddDesignation(new Designation(CurrentThing, Def));
    }
}