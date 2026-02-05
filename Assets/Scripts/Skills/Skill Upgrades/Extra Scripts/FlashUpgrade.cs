using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFlashSkillUpgrade", menuName = "Abilities/Upgrades/FlashSkill")]
public class FlashSkillUpgrade : BaseSkillUpgrade
{
    [Header("Per Level Values")]
    public float[] flashDistancePerLevel;
    public float[] invulnerabilityDurationPerLevel;

    [Header("Special Behaviors")]
    public UpgradeBehavior[] extraBehaviors;

    public override void ApplyUpgrade(Ability ability, int level)
    {
        if (ability is FlashAbility flashAbility)
        {
            // Upgrade flash distance
            if (level < flashDistancePerLevel.Length)
                flashAbility.flashDistance += flashDistancePerLevel[level];

            // Upgrade invulnerability duration
            if (level < invulnerabilityDurationPerLevel.Length)
                flashAbility.invincibilityDuration += invulnerabilityDurationPerLevel[level];

            // Unlock extra behaviors
            foreach (var behavior in extraBehaviors)
            {
                if (behavior.unlockLevel == level + 1)
                {
                    behavior.Apply(flashAbility);
                }
            }
        }
        else
        {
            Debug.LogWarning("FlashSkillUpgrade applied to non-FlashAbility.");
        }
    }
}
