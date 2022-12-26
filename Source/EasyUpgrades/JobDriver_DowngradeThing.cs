using System.Collections.Generic;
using Verse;

namespace EasyUpgrades;

internal class JobDriver_DowngradeThing : JobDriver_ModifyThing
{
    public override DesignationDef Designation => EasyUpgradesDesignationDefOf.Downgrade;

    protected override ThingDef getModifyToThing(Thing t)
    {
        return t.TryGetComp<CompDowngrade>()?.downgradeTo;
    }

    protected override List<ThingDefCountClass> getRefundedResources(Thing t)
    {
        return t.TryGetComp<CompDowngrade>()?.refundedResources;
    }
}