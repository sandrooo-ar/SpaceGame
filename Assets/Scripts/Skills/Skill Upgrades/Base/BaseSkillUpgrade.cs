using UnityEngine;

public abstract class BaseSkillUpgrade : ScriptableObject
{
    public string upgradeName;

    [Header("Generic Per Level Values")]
    public float[] cooldownPerLevel;

    /// <summary>
    /// Each derived upgrade decides how to apply its own stats.
    /// </summary>
    public abstract void ApplyUpgrade(Ability ability, int level);
}
public abstract class UpgradeBehavior : ScriptableObject
{
    public int unlockLevel;
    public abstract void Apply(Ability ability);
}
