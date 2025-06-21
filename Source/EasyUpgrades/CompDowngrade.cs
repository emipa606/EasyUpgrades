using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace EasyUpgrades;

internal class CompDowngrade : ThingComp
{
    public ThingDef DowngradeTo;

    public List<ThingDefCountClass> RefundedResources;

    private CompProperties_Upgradable Props => (CompProperties_Upgradable)props;

    private bool HasDowngradeDesignation =>
        parent.Map.designationManager.DesignationOn(parent, EasyUpgradesDesignationDefOf.Downgrade) != null;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        DowngradeTo = Props.linkedThing;
        RefundedResources = Props.refundedResources;
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (parent.Faction != Faction.OfPlayer || HasDowngradeDesignation)
        {
            yield break;
        }

        var command = new Command_ModifyThing
        {
            icon = ContentFinder<Texture2D>.Get("UI/Down"),
            defaultLabel = "EU.Downgrade".Translate(),
            defaultDesc = Props.keyedTooltipString.Translate(),
            CurrentThing = parent,
            Def = EasyUpgradesDesignationDefOf.Downgrade
        };

        if (RefundedResources.Any())
        {
            command.defaultDesc += "\n" + "EU.DecreaseQualityReturn".Translate(string.Join(", ",
                RefundedResources.Select(thingDefCount => thingDefCount.Summary)));
        }

        yield return command;
    }
}