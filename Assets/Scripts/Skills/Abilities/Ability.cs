using UnityEngine;
using System;

public class Ability : ScriptableObject
{
    public new string name;
    public int manaCost;
    public bool isComboAbility;
    public bool isPassive;
    public float cooldownTime;
    public Sprite icon;
    public AudioClip soundEffect;

    [Header("Upgrade System")]
    public BaseSkillUpgrade upgradeData;
    public int currentLevel = 0;
    public int maxLevel = 5;

    public AbilityHolder abilityHolder;

    // Events
    public event Action<GameObject> OnAbilityActivated;
    public event Action<GameObject> OnDamageDealt;

    public virtual void Activate(GameObject parent)
    {
        // Actives trigger as usual
        OnAbilityActivated?.Invoke(parent);
    }

    public void LevelUp()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            upgradeData.ApplyUpgrade(this, currentLevel - 1);
        }
    }
}
