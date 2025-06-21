using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace EasyUpgrades;

internal class WorkGiver_Upgrade : WorkGiver_Scanner
{
    private static DesignationDef DesUp => EasyUpgradesDesignationDefOf.Upgrade;

    private static JobDef JobUpgrade => EasyUpgradesJobDefOf.UpgradeThing;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Plant);

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn p)
    {
        foreach (var item in p.Map.designationManager.SpawnedDesignationsOfDef(DesUp))
        {
            yield return item.target.Thing;
        }
    }

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
            if (item.def != DesUp)
            {
                continue;
            }

            var construction = WorkTypeDefOf.Construction;
            if (pawn.workSettings.GetPriority(construction) != 0)
            {
                return makeUpgradeJob(t, pawn);
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

    private Job makeUpgradeJob(Thing thingToUpgrade, Pawn pawn)
    {
        var additionalRequiredResources = thingToUpgrade.TryGetComp<CompUpgrade>().additionalRequiredResources;
        var foundResources =
            findAvailableResources(pawn, thingToUpgrade, additionalRequiredResources, out var missingResources);
        switch (missingResources.Count)
        {
            case 1:
                JobFailReason.Is("EU.LackingResourcesError_1".Translate(missingResources[0].label));
                return null;
            case > 0:
                JobFailReason.Is(
                    "EU.LackingResourcesError_2".Translate(missingResources[0].label, missingResources[1].label));
                return null;
        }

        var dictionary = new Dictionary<ThingDef, int>();
        var job = JobMaker.MakeJob(JobUpgrade, thingToUpgrade);
        job.targetQueueB = [];
        job.countQueue = new List<int>(foundResources.Count);
        int i;
        for (i = 0; i < foundResources.Count; i++)
        {
            job.targetQueueB.Add(foundResources[i]);
            var key = foundResources[i].def;
            var i1 = i;
            var count = additionalRequiredResources.FirstOrDefault(t => t.thingDef == foundResources[i1].def).count;
            int num2;
            if (dictionary.TryGetValue(key, out var value))
            {
                num2 = dictionary[key] = Mathf.Min(foundResources[i].stackCount, count - value);
            }
            else
            {
                num2 = Mathf.Min(foundResources[i].stackCount, count);
                dictionary.Add(key, num2);
            }

            job.countQueue.Add(num2);
        }

        job.haulMode = HaulMode.ToCellNonStorage;
        return job;
    }

    private static List<Thing> findAvailableResources(Pawn pawn, Thing thingToUpgrade,
        List<ThingDefCountClass> neededResources, out List<ThingDef> missingResources)
    {
        missingResources = [];
        var list = new List<Thing>();
        foreach (var neededResource in neededResources)
        {
            var count = neededResource.count;
            var thingDef = neededResource.thingDef;
            var num = 0;
            var hasEnough = false;
            if (!pawn.Map.itemAvailability.ThingsAvailableAnywhere(thingDef, count, pawn))
            {
                missingResources.Add(thingDef);
                continue;
            }

            var centerPoint = thingToUpgrade.Position;
            foreach (var item in from t in pawn.Map.listerThings.ThingsOfDef(thingDef)
                     orderby (centerPoint - t.Position).LengthManhattan
                     select t)
            {
                if (item.IsForbidden(pawn) || !pawn.CanReserve(item) || !pawn.CanReach((LocalTargetInfo)item,
                        PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    continue;
                }

                num += item.stackCount;
                list.Add(item);
                if (num < count)
                {
                    continue;
                }

                hasEnough = true;
                break;
            }

            if (!hasEnough)
            {
                missingResources.Add(thingDef);
            }
        }

        return list;
    }
}