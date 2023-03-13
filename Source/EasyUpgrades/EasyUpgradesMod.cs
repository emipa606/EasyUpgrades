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
        var text = EasyUpgrades.baseLevel.ToString();
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
            "EU.Settings.MaxQuality".Translate(listOfQualities[EasyUpgradesSettings.maxUpgradableQuality]), -1f,
            "EU.Settings.MaxQualityTooltip".Translate());
        EasyUpgradesSettings.maxUpgradableQuality =
            (int)Math.Round(listing_Standard.Slider(EasyUpgradesSettings.maxUpgradableQuality, 0f, 6f));
        listing_Standard.Gap(1f);

        if (EasyUpgradesSettings.maxUpgradableQuality > 1)
        {
            listing_Standard.Label("EU.Settings.FailTitle".Translate());
            listing_Standard.GapLine(2f);
            listing_Standard.Gap(6f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(poorLabel, awfulLabel,
                    EasyUpgradesSettings.decreasePoorQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(poorLabel, awfulLabel, text));
            EasyUpgradesSettings.decreasePoorQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.decreasePoorQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 2)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(normalLabel, poorLabel,
                    EasyUpgradesSettings.decreaseNormalQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(normalLabel, poorLabel, text));
            EasyUpgradesSettings.decreaseNormalQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.decreaseNormalQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 3)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(goodLabel, normalLabel,
                    EasyUpgradesSettings.decreaseGoodQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(goodLabel, normalLabel, text));
            EasyUpgradesSettings.decreaseGoodQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.decreaseGoodQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 4)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(excellentLabel, goodLabel,
                    EasyUpgradesSettings.decreaseExcellentQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(excellentLabel, goodLabel, text));
            EasyUpgradesSettings.decreaseExcellentQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.decreaseExcellentQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 5)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(masterworkLabel, excellentLabel,
                    EasyUpgradesSettings.decreaseMasterworkQualityChance.ToStringPercent()), -1f,
                "EU.Settings.DecreaseItemQualityTooltip".Translate(masterworkLabel, excellentLabel, text));
            EasyUpgradesSettings.decreaseMasterworkQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.decreaseMasterworkQualityChance, 0f, 1f);
        }

        listing_Standard.End();
        listing_Standard.Begin(new Rect(inRect.width / 2f, inRect.y, inRect.width / 2f, inRect.height / 1.6f));
        if (EasyUpgradesSettings.maxUpgradableQuality > 0)
        {
            listing_Standard.Label("EU.Settings.SuccessTitle".Translate());
            listing_Standard.GapLine(2f);
            listing_Standard.Gap(6f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(awfulLabel, poorLabel,
                    EasyUpgradesSettings.increaseAwfulQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(awfulLabel, poorLabel, text));
            EasyUpgradesSettings.increaseAwfulQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.increaseAwfulQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 1)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(poorLabel, normalLabel,
                    EasyUpgradesSettings.increasePoorQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(poorLabel, normalLabel, text));
            EasyUpgradesSettings.increasePoorQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.increasePoorQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 2)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(normalLabel, goodLabel,
                    EasyUpgradesSettings.increaseNormalQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(normalLabel, goodLabel, text));
            EasyUpgradesSettings.increaseNormalQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.increaseNormalQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 3)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(goodLabel, excellentLabel,
                    EasyUpgradesSettings.increaseGoodQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(goodLabel, excellentLabel, text));
            EasyUpgradesSettings.increaseGoodQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.increaseGoodQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 4)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(excellentLabel, masterworkLabel,
                    EasyUpgradesSettings.increaseExcellentQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(excellentLabel, masterworkLabel, text));
            EasyUpgradesSettings.increaseExcellentQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.increaseExcellentQualityChance, 0f, 1f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 5)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.XtoY".Translate(masterworkLabel, legendaryLabel,
                    EasyUpgradesSettings.increaseMasterworkQualityChance.ToStringPercent()), -1f,
                "EU.Settings.IncreaseItemQualityTooltip".Translate(masterworkLabel, legendaryLabel, text));
            EasyUpgradesSettings.increaseMasterworkQualityChance =
                listing_Standard.Slider(EasyUpgradesSettings.increaseMasterworkQualityChance, 0f, 1f);
        }

        listing_Standard.End();
        listing_Standard.Begin(new Rect(inRect.x, inRect.height / 1.6f, inRect.width, 35f));
        if (EasyUpgradesSettings.maxUpgradableQuality > 0)
        {
            listing_Standard.Label("EU.Settings.MaterialsTitle".Translate());
            listing_Standard.GapLine(2f);
            listing_Standard.Gap(6f);
            listing_Standard.End();
            listing_Standard.Begin(new Rect(inRect.x, (inRect.height / 1.6f) + 35f, inRect.width / 2.15f, 180f));
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(awfulLabel,
                    EasyUpgradesSettings.neededMaterialsAwfulQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.neededMaterialsAwfulQuality =
                listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsAwfulQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 1)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(poorLabel,
                    EasyUpgradesSettings.neededMaterialsPoorQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.neededMaterialsPoorQuality =
                listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsPoorQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 2)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(normalLabel,
                    EasyUpgradesSettings.neededMaterialsNormalQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.neededMaterialsNormalQuality =
                listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsNormalQuality, 0f, 10f);
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
        if (EasyUpgradesSettings.maxUpgradableQuality > 3)
        {
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(goodLabel,
                    EasyUpgradesSettings.neededMaterialsGoodQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.neededMaterialsGoodQuality =
                listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsGoodQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 4)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(excellentLabel,
                    EasyUpgradesSettings.neededMaterialsExcellentQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.neededMaterialsExcellentQuality =
                listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsExcellentQuality, 0f, 10f);
        }

        if (EasyUpgradesSettings.maxUpgradableQuality > 5)
        {
            listing_Standard.Gap(1f);
            listing_Standard.Label(
                "EU.Settings.MaterialsNeededFor".Translate(masterworkLabel,
                    EasyUpgradesSettings.neededMaterialsMasterworkQuality.ToString()), -1f,
                "EU.Settings.MaterialsNeededTooltip".Translate());
            EasyUpgradesSettings.neededMaterialsMasterworkQuality =
                listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsMasterworkQuality, 0f, 10f);
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