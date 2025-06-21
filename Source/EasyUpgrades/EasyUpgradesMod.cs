using System;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;

namespace EasyUpgrades;

internal class EasyUpgradesMod : Mod
{
    private static string currentVersion;

    public EasyUpgradesMod(ModContentPack content)
        : base(content)
    {
        GetSettings<EasyUpgradesSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var text = EasyUpgrades.BaseLevel.ToString();
        var awfulLabel = QualityCategory.Awful.GetLabel();
        var poorLabel = QualityCategory.Poor.GetLabel();
        var normalLabel = QualityCategory.Normal.GetLabel();
        var goodLabel = QualityCategory.Good.GetLabel();
        var excellentLabel = QualityCategory.Excellent.GetLabel();
        var masterworkLabel = QualityCategory.Masterwork.GetLabel();
        var legendaryLabel = QualityCategory.Legendary.GetLabel();
        var listOfQualities = new[]
            { awfulLabel, poorLabel, normalLabel, goodLabel, excellentLabel, masterworkLabel, legendaryLabel };
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(new Rect(inRect.x, inRect.y, inRect.width / 2.15f, inRect.height / 1.6f));

        listing_Standard.Label(
            "EU.Settings.MaxQuality".Translate(listOfQualities[EasyUpgradesSettings.MaxUpgradableQuality]), -1f,
            "EU.Settings.MaxQualityTooltip".Translate());
        EasyUpgradesSettings.MaxUpgradableQuality =
            (int)Math.Round(listing_Standard.Slider(EasyUpgradesSettings.MaxUpgradableQuality, 0f, 6f));
        listing_Standard.Gap(1f);

        if (EasyUpgradesSettings.MaxUpgradableQuality > 1)
        {
            listing_Standard.Label("EU.Settings.FailTitle".Translate());
            listing_Standard.GapLine(2f);
            listing_Standard.Gap(6f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(poorLabel, awfulLabel,
                    EasyUpgradesSettings.DecreasePoorQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(poorLabel, awfulLabel, text));
            EasyUpgradesSettings.DecreasePoorQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.DecreasePoorQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 2)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(normalLabel, poorLabel,
                    EasyUpgradesSettings.DecreaseNormalQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(normalLabel, poorLabel, text));
            EasyUpgradesSettings.DecreaseNormalQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.DecreaseNormalQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 3)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(goodLabel, normalLabel,
                    EasyUpgradesSettings.DecreaseGoodQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(goodLabel, normalLabel, text));
            EasyUpgradesSettings.DecreaseGoodQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.DecreaseGoodQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 4)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(excellentLabel, goodLabel,
                    EasyUpgradesSettings.DecreaseExcellentQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(excellentLabel, goodLabel, text));
            EasyUpgradesSettings.DecreaseExcellentQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.DecreaseExcellentQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 5)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(masterworkLabel, excellentLabel,
                    EasyUpgradesSettings.DecreaseMasterworkQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(masterworkLabel, excellentLabel, text));
            EasyUpgradesSettings.DecreaseMasterworkQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.DecreaseMasterworkQualityChance, 0f, 1f);
        }

        listing_Standard.End();
        listing_Standard.Begin(new Rect(inRect.width / 2f, inRect.y, inRect.width / 2f, inRect.height / 1.6f));
        if (EasyUpgradesSettings.MaxUpgradableQuality > 0)
        {
            listing_Standard.Label("EU.Settings.SuccessTitle".Translate());
            listing_Standard.GapLine(2f);
            listing_Standard.Gap(6f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(awfulLabel, poorLabel,
                    EasyUpgradesSettings.IncreaseAwfulQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(awfulLabel, poorLabel, text));
            EasyUpgradesSettings.IncreaseAwfulQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.IncreaseAwfulQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 1)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(poorLabel, normalLabel,
                    EasyUpgradesSettings.IncreasePoorQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(poorLabel, normalLabel, text));
            EasyUpgradesSettings.IncreasePoorQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.IncreasePoorQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 2)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(normalLabel, goodLabel,
                    EasyUpgradesSettings.IncreaseNormalQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(normalLabel, goodLabel, text));
            EasyUpgradesSettings.IncreaseNormalQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.IncreaseNormalQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 3)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(goodLabel, excellentLabel,
                    EasyUpgradesSettings.IncreaseGoodQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(goodLabel, excellentLabel, text));
            EasyUpgradesSettings.IncreaseGoodQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.IncreaseGoodQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 4)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(excellentLabel, masterworkLabel,
                    EasyUpgradesSettings.IncreaseExcellentQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(excellentLabel, masterworkLabel, text));
            EasyUpgradesSettings.IncreaseExcellentQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.IncreaseExcellentQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 5)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(masterworkLabel, legendaryLabel,
                    EasyUpgradesSettings.IncreaseMasterworkQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(masterworkLabel, legendaryLabel, text));
            EasyUpgradesSettings.IncreaseMasterworkQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.IncreaseMasterworkQualityChance, 0f, 1f);
        }

        listing_Standard.End();
        listing_Standard.Begin(new Rect(inRect.x, inRect.height / 1.6f, inRect.width, 35f));
        if (EasyUpgradesSettings.MaxUpgradableQuality > 0)
        {
            listing_Standard.Label("EU.Settings.MaterialsTitle".Translate());
            listing_Standard.GapLine(2f);
            listing_Standard.Gap(6f);
            listing_Standard.End();
            listing_Standard.Begin(new Rect(inRect.x, (inRect.height / 1.6f) + 35f, inRect.width / 2.15f, 180f));
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(awfulLabel,
                    EasyUpgradesSettings.NeededMaterialsAwfulQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.NeededMaterialsAwfulQuality =
                listing_Standard.Slider(EasyUpgradesSettings.NeededMaterialsAwfulQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 1)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(poorLabel,
                    EasyUpgradesSettings.NeededMaterialsPoorQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.NeededMaterialsPoorQuality =
                listing_Standard.Slider(EasyUpgradesSettings.NeededMaterialsPoorQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 2)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(normalLabel,
                    EasyUpgradesSettings.NeededMaterialsNormalQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.NeededMaterialsNormalQuality =
                listing_Standard.Slider(EasyUpgradesSettings.NeededMaterialsNormalQuality, 0f, 10f);
        }

        if (currentVersion != null)
        {
            listing_Standard.Gap(1f);
            GUI.contentColor = Color.gray;
            listing_Standard.Label("EU.Settings.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }


        listing_Standard.End();
        listing_Standard.Begin(
            new Rect(inRect.width / 2f, (inRect.height / 1.6f) + 35f, inRect.width / 2.15f, 180f));
        if (EasyUpgradesSettings.MaxUpgradableQuality > 3)
        {
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(goodLabel,
                    EasyUpgradesSettings.NeededMaterialsGoodQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.NeededMaterialsGoodQuality =
                listing_Standard.Slider(EasyUpgradesSettings.NeededMaterialsGoodQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 4)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(excellentLabel,
                    EasyUpgradesSettings.NeededMaterialsExcellentQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.NeededMaterialsExcellentQuality =
                listing_Standard.Slider(EasyUpgradesSettings.NeededMaterialsExcellentQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.MaxUpgradableQuality > 5)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(masterworkLabel,
                    EasyUpgradesSettings.NeededMaterialsMasterworkQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.NeededMaterialsMasterworkQuality =
                listing_Standard.Slider(EasyUpgradesSettings.NeededMaterialsMasterworkQuality, 0f, 10f);
        }

        if (listing_Standard.ButtonText("ResetButton".Translate()))
        {
            EasyUpgradesSettings.Reset();
        }

        listing_Standard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "EU.Settings.Title".Translate();
    }
}