using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace EasyUpgrades;

internal class CompUpgrade : ThingComp
{
    public List<ThingDefCountClass> additionalRequiredResources;
    public ThingDef upgradeTo;

    private CompProperties_Upgradable Props => (CompProperties_Upgradable)props;

    private bool HasUpgradeDesignation =>
        parent.Map.designationManager.DesignationOn(parent, EasyUpgradesDesignationDefOf.Upgrade) != null;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        upgradeTo = Props.linkedThing;
        additionalRequiredResources = Props.additionalRequiredResources;
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (parent.Faction != Faction.OfPlayer || HasUpgradeDesignation)
        {
            yield break;
        }

        var disabled = false;
        var unfinishedRequirements = new List<string>();
        if (Props.researchPrerequisite is { IsFinished: false })
        {
            disabled = true;
            unfinishedRequirements.Add(Props.researchPrerequisite.LabelCap);
        }

        if (!disabled && Props.researchPrerequisites?.Any() == true)
        {
            foreach (var propsResearchPrerequisite in Props.researchPrerequisites)
            {
                if (propsResearchPrerequisite.IsFinished)
                {
                    continue;
                }

                unfinishedRequirements.Add(propsResearchPrerequisite.LabelCap);
                disabled = true;
            }
        }

        var command = new Command_ModifyThing
        {
            icon = ContentFinder<Texture2D>.Get("UI/Up"),
            defaultLabel = "EU.Upgrade".Translate(),
            defaultDesc = Props.keyedTooltipString.Translate(),
            disabled = disabled,
            disabledReason = "EU.UnresearchedError".Translate(string.Join(", ", unfinishedRequirements)),
            CurrentThing = parent,
            Def = EasyUpgradesDesignationDefOf.Upgrade
        };

        if (!disabled && additionalRequiredResources.Any())
        {
            command.defaultDesc += "\n" + "EU.IncreaseQualityCost".Translate(string.Join(", ",
                additionalRequiredResources.Select(thingDefCountClass => thingDefCountClass.Summary)));
        }

        yield return command;
    }
}