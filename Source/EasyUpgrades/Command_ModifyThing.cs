using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EasyUpgrades;

public class Command_ModifyThing : Command
{
    public ThingWithComps currentThing;

    public DesignationDef def;

    private DesignationDef uninstallDef => DesignationDefOf.Uninstall;

    private DesignationDef deconstructDef => DesignationDefOf.Deconstruct;

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
        var designationManager = currentThing.Map.designationManager;
        var designation = designationManager.DesignationOn(currentThing, def);
        var designation2 = designationManager.DesignationOn(currentThing, uninstallDef);
        var designation3 = designationManager.DesignationOn(currentThing, deconstructDef);
        if (designation != null)
        {
            return;
        }

        if (designation2 != null)
        {
            designationManager.TryRemoveDesignationOn(currentThing, uninstallDef);
        }

        if (designation3 != null)
        {
            designationManager.TryRemoveDesignationOn(currentThing, deconstructDef);
        }

        designationManager.AddDesignation(new Designation(currentThing, def));
    }
}