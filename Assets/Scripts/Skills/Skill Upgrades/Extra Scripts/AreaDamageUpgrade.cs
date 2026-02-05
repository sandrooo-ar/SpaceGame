using UnityEngine;

[CreateAssetMenu(fileName = "NewAreaSkillUpgrade", menuName = "Abilities/Upgrades/AreaSkill")]
public class AreaSkillUpgrade : BaseSkillUpgrade
{
    [Header("Per Level Values")]
    public int[] damagePerLevel;
    public float[] durationPerLevel;
    public float[] tickIntervalPerLevel;
    public float[] areaScalePerLevel;

    [Header("Special Behaviors")]
    public UpgradeBehavior[] extraBehaviors;

    public override void ApplyUpgrade(Ability ability, int level)
    {
        if (ability is AreaDamageAbility areaAbility)
        {
            if (level < damagePerLevel.Length)
                areaAbility.baseDamage += damagePerLevel[level];

            if (level < cooldownPerLevel.Length)
                areaAbility.cooldownTime -= cooldownPerLevel[level];

            if (level < durationPerLevel.Length)
                areaAbility.duration += durationPerLevel[level];

            if (level < tickIntervalPerLevel.Length)
                areaAbility.tickInterval -= tickIntervalPerLevel[level];

            if (level < areaScalePerLevel.Length)
                areaAbility.areaScale += areaScalePerLevel[level];

            foreach (var behavior in extraBehaviors)
            {
                if (behavior.unlockLevel == level + 1)
                {
                    behavior.Apply(areaAbility);
                }
            }
        }
    }
}
