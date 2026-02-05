using UnityEngine;
using UnityEngine.InputSystem; // <-- required for Mouse.current

[CreateAssetMenu(fileName = "AreaDamageAbility", menuName = "Abilities/AreaDamageAbility")]
public class AreaDamageAbility : Ability
{
    [Header("General Settings")]
    public float areaScale = 1f;
    public int baseDamage = 5;
    public float duration = 2f;
    public float tickInterval = 0.5f;

    public bool followPlayer = false;

    [Header("Pooling")]
    public string areaEffectPoolTag = "AreaDamageEffect";

    [Header("Execution")]
    public int executionCount = 1;

    [Header("Explosion Settings")]
    public bool explosion = false;
    public float explosionForce = 10f;
    public float explosionRadius = 1f;
    public ForceMode2D explosionForceMode = ForceMode2D.Impulse;

    public override void Activate(GameObject parent)
    {
        // Get mouse position from the new Input System
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0f; // reset z for 2D

        GameObject areaEffect = null;

        if (followPlayer)
        {
            areaEffect = ObjectPooler.Instance.SpawnFromPool(
                areaEffectPoolTag,
                Vector3.zero,
                Quaternion.identity
            );
            areaEffect.transform.SetParent(parent.transform);
            areaEffect.transform.localPosition = Vector3.zero;
        }
        else
        {
            areaEffect = ObjectPooler.Instance.SpawnFromPool(
                areaEffectPoolTag,
                mousePos,
                Quaternion.identity
            );
        }

        if (areaEffect != null)
        {
            // Set scale
            areaEffect.transform.localScale = Vector3.one * areaScale;

            // Configure AreaDamage script
            AreaDamage dmg = areaEffect.GetComponent<AreaDamage>();
            if (dmg != null)
            {
                dmg.Initialize(
                    (int)(PlayerStats.Instance.Damage * PlayerStats.Instance.DamageMultiplier),
                    baseDamage,
                    duration,
                    tickInterval,
                    abilityHolder.abilitySFX
                );

                dmg.executionCount = executionCount;

                // 🔹 Apply explosion settings
                dmg.explosion = explosion;
                dmg.explosionForce = explosionForce;
                dmg.explosionRadius = explosionRadius;
                dmg.forceMode = explosionForceMode;
            }

            base.Activate(parent);

            // - SOUND EFFECT
            if (abilityHolder.abilitySFX != null)
            {
                abilityHolder.abilitySFX.PlaySoundAtRandomPitch(soundEffect, 0.1f);
            }
        }
    }
}
