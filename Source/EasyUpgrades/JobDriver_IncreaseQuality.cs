using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Random = UnityEngine.Random;

namespace EasyUpgrades;

internal class JobDriver_IncreaseQuality : JobDriver
{
    private List<Thing> resourcesPlaced;

    private Thing thingToWorkOn;

    private float totalWorkNeeded;

    private float workLeft;

    private bool IsCraftingJob => job.def == EasyUpgradesJobDefOf.IncreaseQuality_Crafting;

    private bool IsArtisticJob => job.GetTarget(TargetIndex.B).Thing?.def?.defName == "TableSculpting";

    private SkillDef ActiveSkillDef
    {
        get
        {
            if (IsArtisticJob)
            {
                return SkillDefOf.Artistic;
            }

            if (!IsCraftingJob)
            {
                return SkillDefOf.Construction;
            }

            return SkillDefOf.Crafting;
        }
    }

    private StatDef ActiveStatDef => !IsCraftingJob ? StatDefOf.ConstructionSpeed : StatDefOf.WorkSpeedGlobal;

    private float TotalWorkNeeded
    {
        get
        {
            if (IsArtisticJob && thingToWorkOn is MinifiedThing)
            {
                var innerThing = (thingToWorkOn as MinifiedThing)?.InnerThing;
                if (innerThing != null)
                {
                    return Mathf.Clamp(
                        innerThing.def.GetStatValueAbstract(StatDefOf.WorkToMake, innerThing.Stuff) /
                        pawn.skills.GetSkill(ActiveSkill).levelInt, 1f, 200f);
                }
            }

            if (!IsCraftingJob)
            {
                return Mathf.Clamp((TargetA.Thing as Building).GetStatValue(StatDefOf.WorkToBuild), 20f, 200f);
            }

            if (thingToWorkOn == null)
            {
                return 0f;
            }

            return Mathf.Clamp(
                thingToWorkOn.def.GetStatValueAbstract(StatDefOf.WorkToMake, thingToWorkOn.Stuff) /
                pawn.skills.GetSkill(ActiveSkill).levelInt, 1f, 200f);
        }
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (!IsCraftingJob && !IsArtisticJob || !pawn.CanReserve(TargetB, 1, -1, null, true))
        {
            return pawn.CanReserve(TargetA) && pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
        }

        var num = pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
        return num && pawn.Reserve(TargetB, job, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        resourcesPlaced = new List<Thing>();
        var enumerable = IsCraftingJob ? MakeToilsForCrafting() : MakeToilsForBuilding();
        foreach (var item in enumerable)
        {
            yield return item;
        }

        var modify =
            new Toil().FailOnCannotTouch(!IsCraftingJob ? TargetIndex.A : TargetIndex.B, PathEndMode.Touch);
        modify.initAction = delegate
        {
            totalWorkNeeded = TotalWorkNeeded;
            workLeft = totalWorkNeeded;
        };
        modify.tickAction = delegate
        {
            workLeft -= modify.actor.GetStatValue(ActiveStatDef) * 1.3f;
            modify.actor.skills.Learn(ActiveSkillDef,
                0.08f * modify.actor.GetStatValue(StatDefOf.GlobalLearningFactor));
            if (workLeft <= 0f)
            {
                modify.actor.jobs.curDriver.ReadyForNextToil();
            }
        };
        modify.defaultCompleteMode = ToilCompleteMode.Never;
        modify.WithProgressBar(!IsCraftingJob ? TargetIndex.A : TargetIndex.B,
            () => 1f - (workLeft / totalWorkNeeded));
        modify.activeSkill = () => ActiveSkillDef;
        yield return modify;
        yield return new Toil
        {
            initAction = delegate
            {
                if (!NotifyQualityChanged(modify.actor))
                {
                    return;
                }

                DestroyPlacedResources();
                RemoveDesignationsForQualityUpgrade(IsCraftingJob ? thingToWorkOn : TargetA.Thing);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }

    private IEnumerable<Toil> MakeToilsForCrafting()
    {
        this.FailOnForbidden(TargetIndex.A);
        this.FailOnForbidden(TargetIndex.B);
        this.FailOnSomeonePhysicallyInteracting(TargetIndex.B);
        var gotoWorkbench = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B);
        var endGathering = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B);
        var gotoNextHaulThing = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
        yield return gotoNextHaulThing;
        yield return SetThingToWorkOn();
        yield return Toils_Haul.StartCarryThing(TargetIndex.A);
        yield return gotoWorkbench;
        var findPlaceTarget =
            Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.B, TargetIndex.A, TargetIndex.C);
        yield return findPlaceTarget;
        yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget, false);
        yield return Toils_Jump.JumpIf(endGathering, () => job.GetTargetQueue(TargetIndex.A).NullOrEmpty());
        var extract = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A);
        yield return extract;
        yield return gotoNextHaulThing;
        yield return Toils_Haul.StartCarryThing(TargetIndex.A);
        yield return JumpToCollectNextThingForUpgrade(gotoNextHaulThing, TargetIndex.A);
        yield return gotoWorkbench;
        yield return findPlaceTarget;
        yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget, false);
        yield return RecordPlacedResource(TargetIndex.A);
        yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.A, extract);
        yield return endGathering;
    }

    private IEnumerable<Toil> MakeToilsForBuilding()
    {
        this.FailOnForbidden(TargetIndex.A);
        var gotoThingToWorkOn = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A);
        yield return Toils_Jump.JumpIf(gotoThingToWorkOn, () => job.GetTargetQueue(TargetIndex.B).NullOrEmpty());
        var extract = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B);
        yield return extract;
        var gotoNextHaulThing = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
        yield return gotoNextHaulThing;
        yield return Toils_Haul.StartCarryThing(TargetIndex.B, true, false, true);
        yield return JumpToCollectNextThingForUpgrade(gotoNextHaulThing, TargetIndex.B);
        yield return gotoThingToWorkOn;
        yield return Toils_Jump.JumpIf(gotoNextHaulThing, () => pawn.carryTracker.CarriedThing == null);
        var findPlaceTarget =
            Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.C);
        yield return findPlaceTarget;
        yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget, false);
        yield return RecordPlacedResource(TargetIndex.B);
        yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.B, extract);
        yield return gotoThingToWorkOn;
    }

    private Toil SetThingToWorkOn()
    {
        return Toils_General.Do(delegate
        {
            if (IsArtisticJob && job.targetA.Thing is not MinifiedThing)
            {
                var targetA = job.targetA;
                var thing = job.targetA.Thing.TryMakeMinified();
                if (GenPlace.TryPlaceThing(thing, targetA.Cell, Map, ThingPlaceMode.Direct))
                {
                    Map.designationManager.RemoveAllDesignationsOn(job.targetA.Thing);
                    thingToWorkOn = thing;
                    job.SetTarget(TargetIndex.A, thingToWorkOn);
                    Map.designationManager.AddDesignation(new Designation(thingToWorkOn,
                        EasyUpgradesDesignationDefOf.IncreaseQuality_Art));
                }
                else
                {
                    thingToWorkOn = job.targetA.Thing;
                }
            }
            else
            {
                thingToWorkOn = job.targetA.Thing;
            }
        });
    }

    private Toil JumpToCollectNextThingForUpgrade(Toil gotoGetTargetToil, TargetIndex targetIdx)
    {
        return Toils_General.Do(delegate
        {
            var actor = gotoGetTargetToil.actor;
            if (actor.carryTracker.CarriedThing == null)
            {
                Log.Error($"{actor} is not carrying anything");
            }
            else if (!actor.carryTracker.Full)
            {
                var curJob = actor.jobs.curJob;
                var targetQueue = curJob.GetTargetQueue(targetIdx);
                if (targetQueue.NullOrEmpty())
                {
                    return;
                }

                for (var i = 0; i < targetQueue.Count; i++)
                {
                    if (!GenAI.CanUseItemForWork(actor, targetQueue[i].Thing) ||
                        !targetQueue[i].Thing.CanStackWith(actor.carryTracker.CarriedThing))
                    {
                        continue;
                    }

                    var num = actor.carryTracker.CarriedThing?.stackCount ?? 0;
                    var a = curJob.countQueue[i];
                    a = Mathf.Min(a, targetQueue[i].Thing.def.stackLimit - num);
                    a = Mathf.Min(a, actor.carryTracker.AvailableStackSpace(targetQueue[i].Thing.def));
                    if (a <= 0)
                    {
                        continue;
                    }

                    curJob.count = a;
                    curJob.SetTarget(targetIdx, targetQueue[i].Thing);
                    curJob.countQueue[i] -= a;
                    if (curJob.countQueue[i] <= 0)
                    {
                        curJob.countQueue.RemoveAt(i);
                        targetQueue.RemoveAt(i);
                    }

                    actor.jobs.curDriver.JumpToToil(gotoGetTargetToil);
                    break;
                }
            }
        });
    }

    private bool NotifyQualityChanged(Pawn localPawn)
    {
        var thing = IsCraftingJob ? thingToWorkOn : TargetA.Thing;
        if (!thing.TryGetQuality(out var qc))
        {
            return false;
        }

        var successChance = EasyUpgrades.GetSuccessChance(localPawn, ActiveSkillDef, thing);
        var failChance = EasyUpgrades.GetFailChance(localPawn, ActiveSkillDef, thing);
        var num = Random.Range(0f, 1f);
        var labelNoCount = thing.LabelNoCount;
        if (thing is MinifiedThing minifiedThing)
        {
            thing = minifiedThing.InnerThing;
        }

        string key;
        float xp;
        MessageTypeDef def;
        if (num < successChance)
        {
            key = "EU.IncreaseQualityMessage_Success";
            xp = (int)qc * 80f;
            thing.TryGetComp<CompQuality>().SetQuality(qc + 1, ArtGenerationContext.Colony);
            thing.HitPoints = thing.MaxHitPoints;
            def = MessageTypeDefOf.PositiveEvent;
            thing.graphicInt = null;
            thing.styleGraphicInt = null;
            thing.DirtyMapMesh(thing.Map);
        }
        else if (num < successChance + failChance)
        {
            key = "EU.IncreaseQualityMessage_Fail";
            xp = (int)qc * 40f;
            thing.TryGetComp<CompQuality>().SetQuality(qc - 1, ArtGenerationContext.Colony);
            thing.HitPoints -= Mathf.RoundToInt(thing.MaxHitPoints / 10f);
            def = MessageTypeDefOf.NegativeEvent;
            thing.graphicInt = null;
            thing.styleGraphicInt = null;
            thing.DirtyMapMesh(thing.Map);
        }
        else
        {
            xp = (int)qc * 50f;
            if (thing.HitPoints >= thing.MaxHitPoints)
            {
                key = "EU.IncreaseQualityMessage_Neutral_NoRepair";
            }
            else
            {
                key = "EU.IncreaseQualityMessage_Neutral";
                thing.HitPoints += Mathf.RoundToInt(thing.MaxHitPoints / 10f);
                if (thing.HitPoints > thing.MaxHitPoints)
                {
                    thing.HitPoints = thing.MaxHitPoints;
                }
            }

            def = MessageTypeDefOf.NeutralEvent;
        }

        localPawn.skills.Learn(ActiveSkillDef, xp);
        Messages.Message(
            key.Translate(localPawn.NameShortColored,
                labelNoCount.Substring(0, labelNoCount.IndexOf("(", StringComparison.Ordinal) - 1),
                Mathf.Clamp(successChance, 0f, 1f).ToStringPercent()), localPawn, def);
        return true;
    }

    private void DestroyPlacedResources()
    {
        foreach (var item in resourcesPlaced)
        {
            if (!item.Destroyed)
            {
                item.Destroy();
            }
        }
    }

    private Toil RecordPlacedResource(TargetIndex index)
    {
        return Toils_General.Do(delegate { resourcesPlaced.Add(job.GetTarget(index).Thing); });
    }

    private void RemoveDesignationsForQualityUpgrade(Thing t)
    {
        Map.designationManager.RemoveAllDesignationsOn(t);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref thingToWorkOn, "thingToWorkOn");
    }
}