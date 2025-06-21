using System.Collections.Generic;
using Verse;

namespace EasyUpgrades;

internal class JobDriver_DowngradeThing : JobDriver_ModifyThing
{
    public override DesignationDef Designation => EasyUpgradesDesignationDefOf.Downgrade;
    public override EffecterDef WorkEffecter => null;

    protected override ThingDef GetModifyToThing(Thing t)
    {
        return t.TryGetComp<CompDowngrade>()?.DowngradeTo;
    }

    protected override List<ThingDefCountClass> GetRefundedResources(Thing t)
    {
        return t.TryGetComp<CompDowngrade>()?.RefundedResources;
    }
}