using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace EasyUpgrades;

public abstract class JobDriver_ModifyThing : JobDriver_RemoveBuilding
{
    private List<Thing> resourcesPlaced;
    private float totalNeededWork;

    private float workLeft;

    public override float TotalNeededWork =>
        Mathf.Clamp(Building.GetStatValue(StatDefOf.WorkToBuild), 20f, 3000f);

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        if (getModifyToThing(Target) == null)
        {
            yield break;
        }

        this.FailOnForbidden(TargetIndex.A);
        var gotoThingToUpgrade = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A);
        resourcesPlaced = new List<Thing>();
        if (getAdditionalRequiredResources(Target) != null)
        {
            yield return Toils_Jump.JumpIf(gotoThingToUpgrade,
                () => job.GetTargetQueue(TargetIndex.B).NullOrEmpty());
            var extract = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B);
            yield return extract;
            var gotoNextHaulThing = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return gotoNextHaulThing;
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, true, false, true);
            yield return JumpToCollectNextThingForUpgrade(gotoNextHaulThing, TargetIndex.B);
            yield return gotoThingToUpgrade;
            yield return Toils_Jump.JumpIf(gotoNextHaulThing, () => pawn.carryTracker.CarriedThing == null);
            var findPlaceTarget =
                Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.C);
            yield return findPlaceTarget;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget, false);
            yield return RecordPlacedResource();
            yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.B, extract);
        }

        yield return gotoThingToUpgrade;
        var modify = new Toil().FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        modify.initAction = delegate
        {
            totalNeededWork = TotalNeededWork;
            workLeft = totalNeededWork;
        };
        modify.tickAction = delegate
        {
            workLeft -= modify.actor.GetStatValue(StatDefOf.ConstructionSpeed) * 1.3f;
            modify.actor.skills.Learn(SkillDefOf.Construction,
                0.08f * modify.actor.GetStatValue(StatDefOf.GlobalLearningFactor));
            if (workLeft <= 0f)
            {
                modify.actor.jobs.curDriver.ReadyForNextToil();
            }
        };
        modify.defaultCompleteMode = ToilCompleteMode.Never;
        modify.WithProgressBar(TargetIndex.A, () => 1f - (workLeft / totalNeededWork));
        modify.activeSkill = () => SkillDefOf.Construction;
        modify.PlaySoundAtEnd(SoundDefOf.TinyBell);
        yield return modify;
        yield return new Toil
        {
            initAction = delegate
            {
                DestroyPlacedResources();
                RemoveAndReplace();
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }

    private Toil JumpToCollectNextThingForUpgrade(Toil gotoGetTargetToil, TargetIndex targetIdx)
    {
        return Toils_General.Do(delegate
        {
            var actor = gotoGetTargetToil.actor;
            if (actor.carryTracker.CarriedThing == null)
            {
                Log.Error($"{actor} is not carrying anything");
                return;
            }

            if (actor.carryTracker.Full)
            {
                return;
            }

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
        });
    }

    private void RemoveAndReplace()
    {
        var position = Building.Position;
        var rotation = Building.Rotation;
        BillStack billStack = null;
        var refundedResources = getRefundedResources(Target);
        var modifyToThing = getModifyToThing(Target);
        var stuff = Target.Stuff;
        if (Building is Building_WorkTable)
        {
            billStack = (Building as Building_WorkTable)?.BillStack;
        }

        var compRefuelable = Building.TryGetComp<CompRefuelable>();
        if (compRefuelable != null)
        {
            var num = Mathf.CeilToInt(compRefuelable.Fuel);
            var thingDef = compRefuelable.Props.fuelFilter.AllowedThingDefs.First();
            if (thingDef != null && num > 0)
            {
                var thing = ThingMaker.MakeThing(thingDef);
                thing.stackCount = num;
                GenPlace.TryPlaceThing(thing, position, Map, ThingPlaceMode.Near);
            }
        }

        Map.designationManager.RemoveAllDesignationsOn(Building);
        Building.Destroy(DestroyMode.WillReplace);
        var thing2 = ThingMaker.MakeThing(modifyToThing, stuff);
        thing2.SetFactionDirect(Faction.OfPlayer);
        thing2.HitPoints = thing2.MaxHitPoints;
        if (billStack != null)
        {
            foreach (var item in billStack)
            {
                (thing2 as Building_WorkTable)?.BillStack.AddBill(item);
            }
        }

        var compPower = thing2.TryGetComp<CompPower>();
        if (compPower != null)
        {
            var compPower2 = PowerConnectionMaker.BestTransmitterForConnector(position, Map);
            if (compPower2 != null)
            {
                compPower.ConnectToTransmitter(compPower2);
                for (var i = 0; i < 5; i++)
                {
                    FleckMaker.ThrowMetaPuff(position.ToVector3Shifted(), Map);
                }

                Map.mapDrawer.MapMeshDirty(position, MapMeshFlag.PowerGrid);
                Map.mapDrawer.MapMeshDirty(position, MapMeshFlag.Things);
            }
        }

        GenSpawn.Spawn(thing2, position, Map, rotation);
        if (refundedResources == null)
        {
            return;
        }

        foreach (var item2 in refundedResources)
        {
            var thing3 = ThingMaker.MakeThing(item2.thingDef);
            thing3.stackCount = item2.count;
            GenPlace.TryPlaceThing(thing3, position, Map, ThingPlaceMode.Near);
        }
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

    private Toil RecordPlacedResource()
    {
        return Toils_General.Do(delegate { resourcesPlaced.Add(TargetB.Thing); });
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref resourcesPlaced, "resourcesPlaced", LookMode.Reference, new List<Thing>());
    }

    protected abstract ThingDef getModifyToThing(Thing t);

    protected virtual List<ThingDefCountClass> getRefundedResources(Thing t)
    {
        return null;
    }

    protected virtual List<ThingDefCountClass> getAdditionalRequiredResources(Thing t)
    {
        return null;
    }
}