using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDashSkillUpgrade", menuName = "Abilities/Upgrades/DashSkill")]
public class DashSkillUpgrade : BaseSkillUpgrade
{
    [Header("Per Level Values")]
    public float[] dashVelocityPerLevel;
    public float[] invulnerabilityDurationPerLevel;

    [Header("Special Behaviors")]
    public UpgradeBehavior[] extraBehaviors;

    public override void ApplyUpgrade(Ability ability, int level)
    {
        if (ability is DashAbility dashAbility)
        {
            // Upgrade dash velocity
            if (level < dashVelocityPerLevel.Length)
                dashAbility.dashVelocity += dashVelocityPerLevel[level];

            // Add optional invulnerability duration
            if (level < invulnerabilityDurationPerLevel.Length)
            {
                dashAbility.invincibilityDuration += invulnerabilityDurationPerLevel[level];
            }

            // Unlock extra behaviors
            foreach (var behavior in extraBehaviors)
            {
                if (behavior.unlockLevel == level + 1)
                {
                    behavior.Apply(dashAbility);
                }
            }
        }
        else
        {
            Debug.LogWarning("DashSkillUpgrade applied to non-DashAbility.");
        }
    }
}
