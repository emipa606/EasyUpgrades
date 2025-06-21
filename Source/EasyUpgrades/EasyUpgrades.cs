using System.Collections.Generic;
using RimWorld;
using Verse;

namespace EasyUpgrades;

public class EasyUpgrades
{
    public const int BaseLevel = 14;

    public static readonly List<QualityCategory> QualityArray =
    [
        QualityCategory.Awful, QualityCategory.Poor, QualityCategory.Normal, QualityCategory.Good,
        QualityCategory.Excellent, QualityCategory.Masterwork, QualityCategory.Legendary
    ];

    public static float GetSuccessChance(Pawn pawn, SkillDef activeSkill, Thing thing)
    {
        thing.TryGetQuality(out var qc);
        if (QualityArray.IndexOf(qc) >= EasyUpgradesSettings.MaxUpgradableQuality)
        {
            return 0f;
        }

        float num;
        switch (qc)
        {
            case QualityCategory.Awful:
                num = EasyUpgradesSettings.IncreaseAwfulQualityChance;
                break;
            case QualityCategory.Poor:
                num = EasyUpgradesSettings.IncreasePoorQualityChance;
                break;
            case QualityCategory.Normal:
                num = EasyUpgradesSettings.IncreaseNormalQualityChance;
                break;
            case QualityCategory.Good:
                num = EasyUpgradesSettings.IncreaseGoodQualityChance;
                break;
            case QualityCategory.Excellent:
                num = EasyUpgradesSettings.IncreaseExcellentQualityChance;
                break;
            case QualityCategory.Masterwork:
                num = EasyUpgradesSettings.IncreaseMasterworkQualityChance;
                break;
            default:
                return 0f;
        }

        var skill = 0;

        if (pawn.RaceProps.mechFixedSkillLevel > 0)
        {
            skill = pawn.RaceProps.mechFixedSkillLevel;
        }

        if (pawn.skills != null)
        {
            skill = pawn.skills.GetSkill(activeSkill).Level;
        }

        return num * (skill / 14f);
    }

    public static float GetFailChance(Pawn pawn, SkillDef activeSkill, Thing thing)
    {
        thing.TryGetQuality(out var qc);
        if (QualityArray.IndexOf(qc) > EasyUpgradesSettings.MaxUpgradableQuality)
        {
            return 1f;
        }

        var num = 0f;
        switch (qc)
        {
            case QualityCategory.Awful:
                return 0f;
            case QualityCategory.Poor:
                num = EasyUpgradesSettings.DecreasePoorQualityChance;
                break;
            case QualityCategory.Normal:
                num = EasyUpgradesSettings.DecreaseNormalQualityChance;
                break;
            case QualityCategory.Good:
                num = EasyUpgradesSettings.DecreaseGoodQualityChance;
                break;
            case QualityCategory.Excellent:
                num = EasyUpgradesSettings.DecreaseExcellentQualityChance;
                break;
            case QualityCategory.Masterwork:
                num = EasyUpgradesSettings.DecreaseMasterworkQualityChance;
                break;
        }

        var skill = 0;

        if (pawn.RaceProps.mechFixedSkillLevel > 0)
        {
            skill = pawn.RaceProps.mechFixedSkillLevel;
        }

        if (pawn.skills != null)
        {
            skill = pawn.skills.GetSkill(activeSkill).Level;
        }

        var num2 = (20 - skill) / 20f;
        return num + (num2 * 0.15f);
    }
}