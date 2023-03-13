using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace EasyUpgrades;

internal class CompDowngrade : ThingComp
{
    public ThingDef downgradeTo;

    public List<ThingDefCountClass> refundedResources;

    public CompProperties_Upgradable Props => (CompProperties_Upgradable)props;

    private bool HasDowngradeDesignation =>
        parent.Map.designationManager.DesignationOn(parent, EasyUpgradesDesignationDefOf.Downgrade) != null;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        downgradeTo = Props.linkedThing;
        refundedResources = Props.refundedResources;
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (parent.Faction == Faction.OfPlayer && !HasDowngradeDesignation)
        {
            var command = new Command_ModifyThing
            {
                icon = ContentFinder<Texture2D>.Get("UI/Down"),
                defaultLabel = "EU.Downgrade".Translate(),
                defaultDesc = Props.keyedTooltipString.Translate(),
                currentThing = parent,
                def = EasyUpgradesDesignationDefOf.Downgrade
            };

            if (refundedResources.Any())
            {
                command.defaultDesc += "\n" + "EU.DecreaseQualityReturn".Translate(string.Join(", ",
                    refundedResources.Select(thingdefCount => thingdefCount.Summary)));
            }

            yield return command;
        }
    }
}