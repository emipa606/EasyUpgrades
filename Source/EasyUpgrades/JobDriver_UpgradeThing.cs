using System.Collections.Generic;
using Verse;

namespace EasyUpgrades;

internal class JobDriver_UpgradeThing : JobDriver_ModifyThing
{
    public override DesignationDef Designation => EasyUpgradesDesignationDefOf.Upgrade;
    public override EffecterDef WorkEffecter => null;

    protected override ThingDef GetModifyToThing(Thing t)
    {
        return t.TryGetComp<CompUpgrade>()?.upgradeTo;
    }

    protected override List<ThingDefCountClass> GetAdditionalRequiredResources(Thing t)
    {
        return t.TryGetComp<CompUpgrade>()?.additionalRequiredResources;
    }
}