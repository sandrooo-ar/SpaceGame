using UnityEngine;

[CreateAssetMenu(fileName = "NewAimshotSkillUpgrade", menuName = "Abilities/Upgrades/AimshotSkill")]
public class AimshotSkillUpgrade : BaseSkillUpgrade
{
    [Header("Per Level Values")]
    public int[] damagePerLevel;
    public float[] projectileSpeedPerLevel;
    public float[] projectileScalePerLevel;

    [Header("Special Behaviors")]
    public UpgradeBehavior[] extraBehaviors;

    public override void ApplyUpgrade(Ability ability, int level)
    {
        if (ability is AimshotAbility aimshot)
        {
            if (level < damagePerLevel.Length)
                aimshot.baseDamage += damagePerLevel[level];

            if (level < cooldownPerLevel.Length)
                aimshot.cooldownTime -= cooldownPerLevel[level];

            if (level < projectileScalePerLevel.Length) 
                aimshot.projectileScale += projectileScalePerLevel[level];

            if (level < projectileSpeedPerLevel.Length)
                aimshot.projectileSpeed += projectileSpeedPerLevel[level];

            foreach (var behavior in extraBehaviors)
            {
                if (behavior.unlockLevel == level + 1)
                {
                    behavior.Apply(aimshot);
                }
            }
        }
    }
}


