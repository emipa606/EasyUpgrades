using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace EasyUpgrades;

internal class WorkGiver_IncreaseArtQuality : WorkGiver_IncreaseQuality
{
    protected override DesignationDef Designation => EasyUpgradesDesignationDefOf.IncreaseQuality_Art;

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn p)
    {
        foreach (var item in p.Map.designationManager.SpawnedDesignationsOfDef(Designation))
        {
            yield return item.target.Thing;
        }
    }

    protected override Job MakeIncreaseQualityJob(Thing t, Pawn pawn, List<ThingCountClass> resources)
    {
        if (!CanDoWorkType(WorkTypeDefOf.Crafting, pawn))
        {
            return null;
        }

        var closestNeededCraftingBuilding = GetClosestNeededCraftingBuilding(pawn, t);
        if (closestNeededCraftingBuilding == null)
        {
            JobFailReason.Is("EU.MissingWorkStation".Translate(GetNeededCraftingBenchName(t)));
            return null;
        }

        var job = JobMaker.MakeJob(EasyUpgradesJobDefOf.IncreaseQuality_Artistic, t, closestNeededCraftingBuilding);
        job.targetQueueA = [];
        job.countQueue = [];
        foreach (var resource in resources)
        {
            job.targetQueueA.Add(resource.thing);
            job.countQueue.Add(resource.Count);
        }

        job.haulMode = HaulMode.ToCellNonStorage;
        job.count = 1;
        return job;
    }
}