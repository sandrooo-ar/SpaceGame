using UnityEngine.InputSystem;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public SkillSlot skillSlot;
    public AudioManager abilitySFX;
    public Ability abilityOriginal;
    public Ability ability;
    public InputActionReference abilityAction;
    public UIAbilityCooldown uiAbilityCooldown;

    [Header("Hold-to-Cast Settings")]
    public bool allowHoldToCast = true;

    private float cooldownTime;

    private enum AbilityState { ready, cooldown }
    private AbilityState state = AbilityState.ready;

    private void Awake()
    {
        if (AbilitiesData.Instance != null)
        {
            switch (skillSlot)
            {
                case SkillSlot.SkillOne:
                    abilityOriginal = AbilitiesData.Instance.abilityOne;
                    break;
                case SkillSlot.SkillTwo:
                    abilityOriginal = AbilitiesData.Instance.abilityTwo;
                    break;
                case SkillSlot.SkillThree:
                    abilityOriginal = AbilitiesData.Instance.abilityThree;
                    break;
                case SkillSlot.SkillFour:
                    abilityOriginal = AbilitiesData.Instance.abilityFour;
                    break;
            }
        }

        ability = Instantiate(abilityOriginal);
        ability.abilityHolder = this;

        if (ability.isPassive)
        {
            ability.Activate(gameObject);
        }
    }

    private void OnEnable()
    {
        if (!ability.isPassive && abilityAction != null)
            abilityAction.action.Enable();

        if (uiAbilityCooldown != null)
        {
            uiAbilityCooldown.SetAbilityImage(ability.icon);
            uiAbilityCooldown.SetTransparency(ability.isPassive ? 0.6f : 1f);
        }
    }

    private void OnDisable()
    {
        if (!ability.isPassive && abilityAction != null)
            abilityAction.action.Disable();
    }

    private void Update()
    {
        if (ability.isPassive) return;
        if (GameManager.Instance.gamePaused) return;

        // Skip if not unlocked
        if (ability.currentLevel < 1)
        {
            uiAbilityCooldown?.SetTransparency(0.2f);
            return;
        }

        uiAbilityCooldown?.SetManaCost(ability.manaCost);
        uiAbilityCooldown?.SetManaIcon(ManaManager.Instance.currentMana < ability.manaCost);

        switch (state)
        {
            case AbilityState.ready:
                uiAbilityCooldown?.SetTransparency(1f);

                if (abilityAction != null &&
                    (abilityAction.action.WasPressedThisFrame() ||
                    (allowHoldToCast && abilityAction.action.IsPressed())))
                {
                    // 🧙‍♂️ Check if player has enough mana
                    if (ManaManager.Instance.currentMana < ability.manaCost)
                    {
                        // Optionally: play “not enough mana” sound or flash UI
                        ManaManager.Instance.noManaMessage?.ShowNoManaMessage();
                        return;
                    }

                    // ✅ Spend mana
                    bool manaUsed = ManaManager.Instance.UseMana(ability.manaCost);
                    if (!manaUsed) return;

                    // Begin cooldown
                    cooldownTime = ability.isComboAbility
                        ? PlayerStats.Instance.AttackSpeed / PlayerStats.Instance.AttackSpeedMultiplier
                        : ability.cooldownTime / PlayerStats.Instance.CDReductionMultiplier;

                    state = AbilityState.cooldown;

                    // Activate ability
                    ability.Activate(gameObject);
                }
                break;

            case AbilityState.cooldown:
                uiAbilityCooldown?.SetTransparency(0.4f);
                uiAbilityCooldown?.SetCooldown(cooldownTime, ability.cooldownTime, true);

                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.ready;

                    // Handle hold-to-cast immediately after cooldown
                    if (allowHoldToCast && abilityAction.action.IsPressed())
                    {
                        if (ManaManager.Instance.currentMana >= ability.manaCost)
                        {
                            ManaManager.Instance.UseMana(ability.manaCost);

                            cooldownTime = ability.isComboAbility
                                ? PlayerStats.Instance.AttackSpeed / PlayerStats.Instance.AttackSpeedMultiplier
                                : ability.cooldownTime / PlayerStats.Instance.CDReductionMultiplier;

                            state = AbilityState.cooldown;
                            ability.Activate(gameObject);
                        }
                        else
                        {
                            Debug.Log("❌ Not enough mana to recast " + ability.name);
                        }
                    }
                }
                break;
        }
    }

    public void ResetAbility()
    {
        state = AbilityState.ready;
        cooldownTime = 0f;
    }
}
