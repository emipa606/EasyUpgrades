using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace EasyUpgrades;

internal abstract class WorkGiver_IncreaseQuality : WorkGiver_Scanner
{
    protected virtual DesignationDef Designation => null;

    protected bool HasIncreaseQualityDesignation(Designation des)
    {
        return des.def == Designation;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn p)
    {
        foreach (var item in p.Map.designationManager.SpawnedDesignationsOfDef(Designation))
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
            if (!HasIncreaseQualityDesignation(item))
            {
                continue;
            }

            var stuffNeededForQualityIncrease = GetStuffNeededForQualityIncrease(t);
            if (HasEnoughResourcesOfType(pawn, t, stuffNeededForQualityIncrease, out var resources))
            {
                return MakeIncreaseQualityJob(t, pawn, resources);
            }

            JobFailReason.Is("EU.LackingQualityResource".Translate(stuffNeededForQualityIncrease.Label));
            return null;
        }

        return null;
    }

    protected abstract Job MakeIncreaseQualityJob(Thing t, Pawn pawn, List<ThingCountClass> resources);

    protected bool CanDoWorkType(WorkTypeDef def, Pawn pawn)
    {
        if (pawn.workSettings.GetPriority(def) != 0)
        {
            return true;
        }

        if (pawn.WorkTypeIsDisabled(def))
        {
            JobFailReason.Is("EU.WorkTypeDisabled".Translate(def.gerundLabel));
            return false;
        }

        JobFailReason.Is("EU.WorkTypeNotAssigned".Translate(def.gerundLabel));
        return false;
    }

    protected Building GetClosestNeededCraftingBuilding(Pawn pawn, Thing t)
    {
        var actualThing = t;
        if (t is MinifiedThing thing)
        {
            actualThing = thing.InnerThing;
        }

        var defNames = actualThing.def.recipeMaker?.recipeUsers?.ConvertAll(thingDef => thingDef.defName);

        if (defNames == null)
        {
            return null;
        }

        return (from b in pawn.Map.listerBuildings.allBuildingsColonist
            where defNames.Contains(b.def.defName) && !b.IsForbidden(pawn) && !b.IsBurning()
            orderby (b.Position - pawn.Position).LengthManhattan
            select b).FirstOrDefault();
    }

    protected string GetNeededCraftingBenchName(Thing t)
    {
        try
        {
            if (t is MinifiedThing thing)
            {
                return thing.InnerThing.def.recipeMaker.recipeUsers.ConvertAll(thingDef => thingDef.label)
                    .First();
            }

            return t.def.recipeMaker.recipeUsers.ConvertAll(thingDef => thingDef.label).First();
        }
        catch
        {
            return "";
        }
    }

    private bool HasEnoughResourcesOfType(Pawn pawn, Thing t, ThingDefCountClass stuffDef,
        out List<ThingCountClass> resources)
    {
        resources = new List<ThingCountClass>();
        if (stuffDef == null)
        {
            return false;
        }

        var num = 0;
        var count = stuffDef.count;
        var thingDef = stuffDef.thingDef;
        if (!pawn.Map.itemAvailability.ThingsAvailableAnywhere(stuffDef, pawn))
        {
            return false;
        }

        var centerPoint = t.Position;
        foreach (var item in from resource in pawn.Map.listerThings.ThingsOfDef(thingDef)
                 orderby (centerPoint - resource.Position).LengthManhattan
                 select resource)
        {
            if (item.IsForbidden(pawn) || !pawn.CanReserve(item) ||
                !pawn.CanReach((LocalTargetInfo)item, PathEndMode.ClosestTouch, Danger.Deadly))
            {
                continue;
            }

            resources.Add(new ThingCountClass(item, Mathf.Min(item.stackCount, count - num)));
            num += item.stackCount;
            if (num >= count)
            {
                break;
            }
        }

        return num >= count;
    }

    public static ThingDefCountClass GetStuffNeededForQualityIncrease(Thing t)
    {
        if (t is MinifiedThing thing)
        {
            t = thing.InnerThing;
        }

        var num = 1;
        var stuff = t.Stuff;
        if (!t.def.MadeFromStuff || stuff == null)
        {
            var stuffToUse = ThingDefOf.WoodLog;

            switch (t.def.techLevel)
            {
                case TechLevel.Industrial:
                    stuffToUse = ThingDefOf.Steel;
                    break;
                case TechLevel.Spacer:
                case TechLevel.Ultra:
                case TechLevel.Archotech:
                    stuffToUse = ThingDefOf.Plasteel;
                    break;
            }

            var value = t.def.BaseMarketValue / 10;
            var amountToUse = (int)Math.Max(10, Math.Round(value / stuffToUse.BaseMarketValue));

            return new ThingDefCountClass(stuffToUse, amountToUse);
        }

        if (!t.TryGetQuality(out var qc))
        {
            Log.Error($"Couldn't get quality for {t.Label}");
            return null;
        }

        var recipeMaker = t.def.recipeMaker;
        if (recipeMaker is { bulkRecipeCount: > 0 })
        {
            num = recipeMaker.bulkRecipeCount;
        }

        ThingDef thingDef2;
        float num3;
        if (t.def.costList is { Count: > 0 })
        {
            ThingDef thingDef = null;
            var num2 = 0;
            foreach (var cost in t.def.costList)
            {
                if (cost.count <= num2)
                {
                    continue;
                }

                num2 = cost.count;
                thingDef = cost.thingDef;
            }

            thingDef2 = thingDef;
            num3 = num2;
        }
        else
        {
            thingDef2 = stuff;
            num3 = t.def.costStuffCount;
        }

        switch (qc)
        {
            case QualityCategory.Awful:
                num3 *= EasyUpgradesSettings.neededMaterialsAwfulQuality;
                break;
            case QualityCategory.Poor:
                num3 *= EasyUpgradesSettings.neededMaterialsPoorQuality;
                break;
            case QualityCategory.Normal:
                num3 *= EasyUpgradesSettings.neededMaterialsNormalQuality;
                break;
            case QualityCategory.Good:
                num3 *= EasyUpgradesSettings.neededMaterialsGoodQuality;
                break;
            case QualityCategory.Excellent:
                num3 *= EasyUpgradesSettings.neededMaterialsExcellentQuality;
                break;
            case QualityCategory.Masterwork:
                num3 *= EasyUpgradesSettings.neededMaterialsMasterworkQuality;
                break;
            default:
                return null;
        }

        thingDef2 = EasyUpgradesSettings.GetReplacementThingDef(thingDef2);
        return new ThingDefCountClass(thingDef2, Mathf.Max(1, Mathf.CeilToInt(num3 * num)));
    }
}