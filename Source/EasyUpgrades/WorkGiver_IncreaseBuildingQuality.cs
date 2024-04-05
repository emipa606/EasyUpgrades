using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace EasyUpgrades;

internal class WorkGiver_IncreaseBuildingQuality : WorkGiver_IncreaseQuality
{
    protected override DesignationDef Designation => EasyUpgradesDesignationDefOf.IncreaseQuality_Building;

    protected override Job MakeIncreaseQualityJob(Thing t, Pawn pawn, List<ThingCountClass> resources)
    {
        if (!CanDoWorkType(WorkTypeDefOf.Construction, pawn))
        {
            return null;
        }

        var job = JobMaker.MakeJob(EasyUpgradesJobDefOf.IncreaseQuality_Building, t);
        job.targetQueueB = [];
        job.countQueue = [];
        foreach (var resource in resources)
        {
            job.targetQueueB.Add(resource.thing);
            job.countQueue.Add(resource.Count);
        }

        job.haulMode = HaulMode.ToCellNonStorage;
        job.count = job.countQueue.Count <= 0 ? 1 : job.countQueue[0];
        return job;
    }
}