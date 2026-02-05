using UnityEngine;

[CreateAssetMenu(fileName = "NewPoisonSkillUpgrade", menuName = "Abilities/Upgrades/PoisonSkill")]
public class PoisonUpgrade : BaseSkillUpgrade
{

    [Header("Per Level Values")]
    public int[] poisonDamagePerLevel;
    public int[] poisonDecayPerLevel;
    public float[] poisonTickIntervalPerLevel;

    [Header("Special Behaviors")]
    public UpgradeBehavior[] extraBehaviors;

    public override void ApplyUpgrade(Ability ability, int level)
    {
        if (ability is PoisonAbility poisonAbility)
        {
            // Upgrade poison damage
            if (level < poisonDamagePerLevel.Length)
                poisonAbility.poisonDamageImprovement += poisonDamagePerLevel[level];

            // Upgrade invulnerability duration
            if (level < poisonDecayPerLevel.Length)
                poisonAbility.poisonDecayImprovement -= poisonDecayPerLevel[level];

            if (level < poisonTickIntervalPerLevel.Length)
                poisonAbility.poisonTickIntervalImprovement -= poisonTickIntervalPerLevel[level];

            poisonAbility.ImprovePoisons();


            // Unlock extra behaviors
            foreach (var behavior in extraBehaviors)
            {
                if (behavior.unlockLevel == level + 1)
                {
                    behavior.Apply(poisonAbility);
                }
            }
        }
    }

}
