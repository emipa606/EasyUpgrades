using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace EasyUpgrades;

internal class CompIncreaseQuality : ThingComp
{
    private readonly DesignationDef apparelDes = EasyUpgradesDesignationDefOf.IncreaseQuality_Apparel;

    private readonly DesignationDef artDes = EasyUpgradesDesignationDefOf.IncreaseQuality_Art;
    private readonly DesignationDef buildingDes = EasyUpgradesDesignationDefOf.IncreaseQuality_Building;

    private readonly DesignationDef itemDes = EasyUpgradesDesignationDefOf.IncreaseQuality_Item;

    private bool HasIncreaseBuildingQualityDes =>
        parent.Map.designationManager.DesignationOn(parent, buildingDes) != null;

    private bool HasIncreaseArtQualityDes => parent.Map.designationManager.DesignationOn(parent, artDes) != null;

    private bool HasIncreaseItemQualityDes => parent.Map.designationManager.DesignationOn(parent, itemDes) != null;

    private bool HasIncreaseApparelQualityDes =>
        parent.Map.designationManager.DesignationOn(parent, apparelDes) != null;

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (!parent.def.HasComp(typeof(CompQuality)))
        {
            yield break;
        }

        if (parent.Faction == Faction.OfPlayer)
        {
            if (parent.def.IsArt && !HasIncreaseArtQualityDes)
            {
                yield return CreateCommandForDesignation(new Designation(parent, artDes));
                yield break;
            }

            if (parent is Building && !HasIncreaseBuildingQualityDes)
            {
                yield return CreateCommandForDesignation(new Designation(parent, buildingDes));
            }

            yield break;
        }

        if (parent.Faction != null)
        {
            yield break;
        }

        if ((parent.def.IsApparel || parent.def.IsWeapon) &&
            WorkGiver_IncreaseQuality.GetNeededCraftingBenchName(parent) == string.Empty)
        {
            yield break;
        }

        if (parent.def.IsApparel && !HasIncreaseApparelQualityDes)
        {
            yield return CreateCommandForDesignation(new Designation(parent, apparelDes));
            yield break;
        }

        if (!parent.def.IsWeapon || HasIncreaseItemQualityDes)
        {
            yield break;
        }

        yield return CreateCommandForDesignation(new Designation(parent, itemDes));
    }

    private Command CreateCommandForDesignation(Designation des)
    {
        var gizmo = new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("UI/QualityUp"),
            defaultLabel = "EU.IncreaseQuality".Translate(),
            defaultDesc = "EU.TryIncreaseQualityTooltip".Translate()
        };
        var compQuality = parent.TryGetComp<CompQuality>();
        if (compQuality != null)
        {
            if (EasyUpgrades.QualityArray.IndexOf(compQuality.Quality) >=
                EasyUpgradesSettings.maxUpgradableQuality)
            {
                gizmo.disabled = true;
                gizmo.disabledReason = "EU.CannotIncreaseQuality".Translate();
            }
            else
            {
                gizmo.defaultDesc += "\n" +
                                     "EU.IncreaseQualityCost".Translate(WorkGiver_IncreaseQuality
                                         .GetStuffNeededForQualityIncrease(parent).Summary);
            }
        }

        gizmo.action = delegate { parent.Map.designationManager.AddDesignation(des); };
        return gizmo;
    }
}