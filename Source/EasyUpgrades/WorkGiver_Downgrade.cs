using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace EasyUpgrades;

internal class WorkGiver_Downgrade : WorkGiver_Scanner
{
    private static DesignationDef DesDown => EasyUpgradesDesignationDefOf.Downgrade;

    private static JobDef JobDowngrade => EasyUpgradesJobDefOf.DowngradeThing;

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Plant);

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!pawn.CanReserve(t, 1, -1, null, forced))
        {
            return null;
        }

        if (t.IsForbidden(pawn) || t.IsBurning())
        {
            return null;
        }

        foreach (var item in pawn.Map.designationManager.AllDesignationsOn(t))
        {
            if (item.def != DesDown)
            {
                continue;
            }

            var construction = WorkTypeDefOf.Construction;
            if (pawn.workSettings.GetPriority(construction) != 0)
            {
                return JobMaker.MakeJob(JobDowngrade, t);
            }

            if (pawn.WorkTypeIsDisabled(construction))
            {
                JobFailReason.Is("EU.WorkTypeDisabled".Translate(construction.gerundLabel));
                return null;
            }

            JobFailReason.Is("EU.WorkTypeNotAssigned".Translate(construction.gerundLabel));
            return null;
        }

        return null;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn p)
    {
        foreach (var item in p.Map.designationManager.SpawnedDesignationsOfDef(EasyUpgradesDesignationDefOf
                     .Downgrade))
        {
            yield return item.target.Thing;
        }
    }
}