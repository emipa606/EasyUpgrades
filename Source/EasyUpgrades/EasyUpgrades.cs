using RimWorld;
using Verse;

namespace EasyUpgrades;

public class EasyUpgrades
{
    public static int baseLevel = 14;

    public static float GetSuccessChance(Pawn pawn, SkillDef activeSkill, Thing thing)
    {
        thing.TryGetQuality(out var qc);
        float num;
        switch (qc)
        {
            case QualityCategory.Awful:
                num = EasyUpgradesSettings.increaseAwfulQualityChance;
                break;
            case QualityCategory.Poor:
                num = EasyUpgradesSettings.increasePoorQualityChance;
                break;
            case QualityCategory.Normal:
                num = EasyUpgradesSettings.increaseNormalQualityChance;
                break;
            case QualityCategory.Good:
                num = EasyUpgradesSettings.increaseGoodQualityChance;
                break;
            case QualityCategory.Excellent:
                num = EasyUpgradesSettings.increaseExcellentQualityChance;
                break;
            case QualityCategory.Masterwork:
                num = EasyUpgradesSettings.increaseMasterworkQualityChance;
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
        var num = 0f;
        switch (qc)
        {
            case QualityCategory.Awful:
                return 0f;
            case QualityCategory.Poor:
                num = EasyUpgradesSettings.decreasePoorQualityChance;
                break;
            case QualityCategory.Normal:
                num = EasyUpgradesSettings.decreaseNormalQualityChance;
                break;
            case QualityCategory.Good:
                num = EasyUpgradesSettings.decreaseGoodQualityChance;
                break;
            case QualityCategory.Excellent:
                num = EasyUpgradesSettings.decreaseExcellentQualityChance;
                break;
            case QualityCategory.Masterwork:
                num = EasyUpgradesSettings.decreaseMasterworkQualityChance;
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