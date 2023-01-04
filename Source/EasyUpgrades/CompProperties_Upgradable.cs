using System.Collections.Generic;
using Verse;

namespace EasyUpgrades;

internal class CompProperties_Upgradable : CompProperties
{
    public List<ThingDefCountClass> additionalRequiredResources;

    public string keyedTooltipString;

    public ThingDef linkedThing;

    public List<ThingDefCountClass> refundedResources;
    public ResearchProjectDef researchPrerequisite;
    public List<ResearchProjectDef> researchPrerequisites;

    public string linkedThingName => linkedThing.label;
}