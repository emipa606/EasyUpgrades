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
        var label = QualityCategory.Awful.GetLabel();
        var label2 = QualityCategory.Poor.GetLabel();
        var label3 = QualityCategory.Normal.GetLabel();
        var label4 = QualityCategory.Good.GetLabel();
        var label5 = QualityCategory.Excellent.GetLabel();
        var label6 = QualityCategory.Masterwork.GetLabel();
        var label7 = QualityCategory.Legendary.GetLabel();
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(new Rect(inRect.x, inRect.y, inRect.width / 2.15f, inRect.height / 1.5f));
        listing_Standard.Label("EU.Settings.SuccessTitle".Translate());
        listing_Standard.GapLine(2f);
        listing_Standard.Gap(6f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label, label2,
                EasyUpgradesSettings.increaseAwfulQualityChance.ToStringPercent()), -1f,
            "EU.Settings.IncreaseItemQualityTooltip".Translate(label, label2, text));
        EasyUpgradesSettings.increaseAwfulQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.increaseAwfulQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label2, label3,
                EasyUpgradesSettings.increasePoorQualityChance.ToStringPercent()), -1f,
            "EU.Settings.IncreaseItemQualityTooltip".Translate(label2, label3, text));
        EasyUpgradesSettings.increasePoorQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.increasePoorQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label3, label4,
                EasyUpgradesSettings.increaseNormalQualityChance.ToStringPercent()), -1f,
            "EU.Settings.IncreaseItemQualityTooltip".Translate(label3, label4, text));
        EasyUpgradesSettings.increaseNormalQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.increaseNormalQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label4, label5,
                EasyUpgradesSettings.increaseGoodQualityChance.ToStringPercent()), -1f,
            "EU.Settings.IncreaseItemQualityTooltip".Translate(label4, label5, text));
        EasyUpgradesSettings.increaseGoodQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.increaseGoodQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label5, label6,
                EasyUpgradesSettings.increaseExcellentQualityChance.ToStringPercent()), -1f,
            "EU.Settings.IncreaseItemQualityTooltip".Translate(label5, label6, text));
        EasyUpgradesSettings.increaseExcellentQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.increaseExcellentQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label6, label7,
                EasyUpgradesSettings.increaseMasterworkQualityChance.ToStringPercent()), -1f,
            "EU.Settings.IncreaseItemQualityTooltip".Translate(label6, label7, text));
        EasyUpgradesSettings.increaseMasterworkQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.increaseMasterworkQualityChance, 0f, 1f);
        listing_Standard.End();
        listing_Standard.Begin(new Rect(inRect.width / 2f, inRect.y, inRect.width / 2f, inRect.height / 1.5f));
        listing_Standard.Label("EU.Settings.FailTitle".Translate());
        listing_Standard.GapLine(2f);
        listing_Standard.Gap(6f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label2, label,
                EasyUpgradesSettings.decreasePoorQualityChance.ToStringPercent()), -1f,
            "EU.Settings.DecreaseItemQualityTooltip".Translate(label2, label, text));
        EasyUpgradesSettings.decreasePoorQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.decreasePoorQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label3, label2,
                EasyUpgradesSettings.decreaseNormalQualityChance.ToStringPercent()), -1f,
            "EU.Settings.DecreaseItemQualityTooltip".Translate(label3, label2, text));
        EasyUpgradesSettings.decreaseNormalQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.decreaseNormalQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label4, label3,
                EasyUpgradesSettings.decreaseGoodQualityChance.ToStringPercent()), -1f,
            "EU.Settings.DecreaseItemQualityTooltip".Translate(label4, label3, text));
        EasyUpgradesSettings.decreaseGoodQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.decreaseGoodQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label5, label4,
                EasyUpgradesSettings.decreaseExcellentQualityChance.ToStringPercent()), -1f,
            "EU.Settings.DecreaseItemQualityTooltip".Translate(label5, label4, text));
        EasyUpgradesSettings.decreaseExcellentQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.decreaseExcellentQualityChance, 0f, 1f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.XtoY".Translate(label6, label5,
                EasyUpgradesSettings.decreaseMasterworkQualityChance.ToStringPercent()), -1f,
            "EU.Settings.DecreaseItemQualityTooltip".Translate(label6, label5, text));
        EasyUpgradesSettings.decreaseMasterworkQualityChance =
            listing_Standard.Slider(EasyUpgradesSettings.decreaseMasterworkQualityChance, 0f, 1f);

        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("EU.Settings.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        listing_Standard.Begin(new Rect(inRect.x, inRect.height / 1.5f, inRect.width, 35f));
        listing_Standard.Label("EU.Settings.MaterialsTitle".Translate());
        listing_Standard.GapLine(2f);
        listing_Standard.Gap(6f);
        listing_Standard.End();
        listing_Standard.Begin(new Rect(inRect.x, (inRect.height / 1.5f) + 35f, inRect.width / 2.15f, 150f));
        listing_Standard.Label(
            "EU.Settings.MaterialsNeededFor".Translate(label,
                EasyUpgradesSettings.neededMaterialsAwfulQuality.ToString()), -1f,
            "EU.Settings.MaterialsNeededTooltip".Translate());
        EasyUpgradesSettings.neededMaterialsAwfulQuality =
            listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsAwfulQuality, 0f, 10f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.MaterialsNeededFor".Translate(label2,
                EasyUpgradesSettings.neededMaterialsPoorQuality.ToString()), -1f,
            "EU.Settings.MaterialsNeededTooltip".Translate());
        EasyUpgradesSettings.neededMaterialsPoorQuality =
            listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsPoorQuality, 0f, 10f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.MaterialsNeededFor".Translate(label3,
                EasyUpgradesSettings.neededMaterialsNormalQuality.ToString()), -1f,
            "EU.Settings.MaterialsNeededTooltip".Translate());
        EasyUpgradesSettings.neededMaterialsNormalQuality =
            listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsNormalQuality, 0f, 10f);
        listing_Standard.End();
        listing_Standard.Begin(
            new Rect(inRect.width / 2f, (inRect.height / 1.5f) + 35f, inRect.width / 2.15f, 150f));
        listing_Standard.Label(
            "EU.Settings.MaterialsNeededFor".Translate(label4,
                EasyUpgradesSettings.neededMaterialsGoodQuality.ToString()), -1f,
            "EU.Settings.MaterialsNeededTooltip".Translate());
        EasyUpgradesSettings.neededMaterialsGoodQuality =
            listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsGoodQuality, 0f, 10f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.MaterialsNeededFor".Translate(label5,
                EasyUpgradesSettings.neededMaterialsExcellentQuality.ToString()), -1f,
            "EU.Settings.MaterialsNeededTooltip".Translate());
        EasyUpgradesSettings.neededMaterialsExcellentQuality =
            listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsExcellentQuality, 0f, 10f);
        listing_Standard.Gap(1f);
        listing_Standard.Label(
            "EU.Settings.MaterialsNeededFor".Translate(label6,
                EasyUpgradesSettings.neededMaterialsMasterworkQuality.ToString()), -1f,
            "EU.Settings.MaterialsNeededTooltip".Translate());
        EasyUpgradesSettings.neededMaterialsMasterworkQuality =
            listing_Standard.Slider(EasyUpgradesSettings.neededMaterialsMasterworkQuality, 0f, 10f);

        listing_Standard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "EU.Settings.Title".Translate();
    }
}