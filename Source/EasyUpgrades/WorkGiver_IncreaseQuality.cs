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

    private bool HasIncreaseQualityDesignation(Designation des)
    {
        return des.def == Designation;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        foreach (var item in pawn.Map.designationManager.SpawnedDesignationsOfDef(Designation))
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

            if (!HasRequiredSkills(pawn, t))
            {
                return null;
            }

            var requiredResources = GetStuffNeededForQualityIncrease(t);
            if (requiredResources == null)
            {
                return null;
            }

            var resources = new List<ThingCountClass>();
            foreach (var requiredResource in requiredResources)
            {
                if (!HasEnoughResourcesOfType(pawn, t, requiredResource, out var foundResources))
                {
                    JobFailReason.Is("EU.LackingQualityResource".Translate(requiredResource.Label));
                    return null;
                }

                resources.AddRange(foundResources);
            }

            return MakeIncreaseQualityJob(t, pawn, resources);
        }

        return null;
    }

    protected abstract Job MakeIncreaseQualityJob(Thing t, Pawn pawn, List<ThingCountClass> resources);

    protected static bool CanDoWorkType(WorkTypeDef def, Pawn pawn)
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

    private static bool HasRequiredSkills(Pawn pawn, Thing t)
    {
        var actualThing = t is MinifiedThing minified ? minified.InnerThing : t;
        if (actualThing is Building)
        {
            foreach (var requirement in new[]
                     {
                         new SkillRequirement
                         {
                             skill = SkillDefOf.Construction, minLevel = actualThing.def.constructionSkillPrerequisite
                         },
                         new SkillRequirement
                             { skill = SkillDefOf.Artistic, minLevel = actualThing.def.artisticSkillPrerequisite }
                     })
            {
                if (requirement.minLevel <= 0 || requirement.PawnSatisfies(pawn))
                {
                    continue;
                }

                JobFailReason.Is("SkillTooLowForConstruction".Translate(requirement.skill.LabelCap));
                return false;
            }
        }

        var requirements = actualThing.def.recipeMaker?.skillRequirements;
        if (requirements == null)
        {
            return true;
        }

        foreach (var requirement in requirements)
        {
            if (requirement.PawnSatisfies(pawn))
            {
                continue;
            }

            JobFailReason.Is("UnderRequiredSkill".Translate(requirement.minLevel), actualThing.LabelShort);
            return false;
        }

        return true;
    }

    protected static Building GetClosestNeededCraftingBuilding(Pawn pawn, Thing t)
    {
        var actualThing = t is MinifiedThing minified ? minified.InnerThing : t;
        var recipeUsers = actualThing.def.recipeMaker?.recipeUsers;
        SkillRequirement missingSkill = null;
        foreach (var building in pawn.Map.listerBuildings.allBuildingsColonist
                     .Where(building => !building.IsForbidden(pawn) && !building.IsBurning() &&
                                        pawn.CanReserveAndReach(building, PathEndMode.InteractionCell, Danger.Some))
                     .OrderBy(building => (building.Position - pawn.Position).LengthManhattan))
        {
            var recipes = building.def.AllRecipes.Where(recipe => recipe.ProducedThingDef == actualThing.def).ToList();
            if (recipes.Count == 0)
            {
                if (recipeUsers?.Contains(building.def) == true)
                {
                    return building;
                }

                continue;
            }

            foreach (var recipe in recipes)
            {
                var requirement = recipe.FirstSkillRequirementPawnDoesntSatisfy(pawn);
                if (requirement == null)
                {
                    return building;
                }

                missingSkill ??= requirement;
            }
        }

        if (missingSkill != null)
        {
            JobFailReason.Is("UnderRequiredSkill".Translate(missingSkill.minLevel), actualThing.LabelShort);
        }
        else
        {
            JobFailReason.Is("EU.MissingWorkStation".Translate(GetNeededCraftingBenchName(t)));
        }

        return null;
    }

    public static string GetNeededCraftingBenchName(Thing t)
    {
        var actualThing = t;
        if (t is MinifiedThing thing)
        {
            actualThing = thing.InnerThing;
        }

        var possibleWorkstations = new HashSet<string>();
        if (actualThing.def.recipeMaker?.recipeUsers != null)
        {
            foreach (var building in actualThing.def.recipeMaker.recipeUsers)
            {
                possibleWorkstations.Add(building.LabelCap);
            }
        }

        foreach (var building in DefDatabase<ThingDef>.AllDefsListForReading.Where(thingDef =>
                     thingDef.AllRecipes.Any(recipeDef => recipeDef.ProducedThingDef == actualThing.def)))
        {
            possibleWorkstations.Add(building.LabelCap);
        }

        return possibleWorkstations.Any() ? string.Join(", ", possibleWorkstations.OrderBy(s => s)) : string.Empty;
    }

    private static bool HasEnoughResourcesOfType(Pawn pawn, Thing t, ThingDefCountClass stuffDef,
        out List<ThingCountClass> resources)
    {
        resources = [];
        if (stuffDef == null)
        {
            return false;
        }

        var num = 0;
        var count = stuffDef.count;
        var thingDef = stuffDef.thingDef;
        if (!pawn.Map.itemAvailability.ThingsAvailableAnywhere(thingDef, count, pawn))
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

    public static List<ThingDefCountClass> GetStuffNeededForQualityIncrease(Thing t)
    {
        if (t is MinifiedThing thing)
        {
            t = thing.InnerThing;
        }

        if (!t.TryGetQuality(out var qc))
        {
            Log.Error($"Couldn't get quality for {t.Label}");
            return null;
        }

        float multiplier;
        switch (qc)
        {
            case QualityCategory.Awful:
                multiplier = EasyUpgradesSettings.NeededMaterialsAwfulQuality;
                break;
            case QualityCategory.Poor:
                multiplier = EasyUpgradesSettings.NeededMaterialsPoorQuality;
                break;
            case QualityCategory.Normal:
                multiplier = EasyUpgradesSettings.NeededMaterialsNormalQuality;
                break;
            case QualityCategory.Good:
                multiplier = EasyUpgradesSettings.NeededMaterialsGoodQuality;
                break;
            case QualityCategory.Excellent:
                multiplier = EasyUpgradesSettings.NeededMaterialsExcellentQuality;
                break;
            case QualityCategory.Masterwork:
                multiplier = EasyUpgradesSettings.NeededMaterialsMasterworkQuality;
                break;
            default:
                Log.Error($"Cannot calculate quality upgrade resources for {t.Label} at quality {qc}");
                return null;
        }

        var costs = new Dictionary<ThingDef, float>();

        if (t.def.MadeFromStuff)
        {
            if (t.Stuff == null)
            {
                Log.Error($"Cannot calculate quality upgrade resources for {t.Label}: missing stuff");
                return null;
            }

            AddCost(t.Stuff, t.def.CostStuffCount);
        }

        if (t.def.CostList != null)
        {
            foreach (var cost in t.def.CostList)
            {
                AddCost(cost.thingDef, cost.count);
            }
        }

        if (costs.Count != 0)
        {
            return costs.Select(cost => new ThingDefCountClass(cost.Key,
                Mathf.Max(1, Mathf.CeilToInt(cost.Value * multiplier)))).ToList();
        }

        var fallback = t.def.techLevel switch
        {
            TechLevel.Industrial => ThingDefOf.Steel,
            TechLevel.Spacer or TechLevel.Ultra or TechLevel.Archotech => ThingDefOf.Plasteel,
            _ => ThingDefOf.WoodLog
        };
        AddCost(fallback, (float)Math.Max(10, Math.Round(t.def.BaseMarketValue / 10 / fallback.BaseMarketValue)));

        return costs.Select(cost => new ThingDefCountClass(cost.Key,
            Mathf.Max(1, Mathf.CeilToInt(cost.Value * multiplier)))).ToList();

        void AddCost(ThingDef def, float count)
        {
            if (count <= 0)
            {
                return;
            }

            def = EasyUpgradesSettings.GetReplacementThingDef(def);
            costs.TryGetValue(def, out var existing);
            costs[def] = existing + count;
        }
    }
}