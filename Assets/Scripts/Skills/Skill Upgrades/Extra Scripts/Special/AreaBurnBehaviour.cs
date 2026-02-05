using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "BurnUpgrade", menuName = "Abilities/SpecialUpgrades/BurnUpgrade")]
public class BurnUpgrade : UpgradeBehavior
{
    [Header("Burn Settings")]
    public int damagePerTick = 5;
    public float tickInterval = 1f;
    public float duration = 5f;

    public override void Apply(Ability ability)
    {
        // Only apply to AreaDamageAbility
        if (ability is AreaDamageAbility areaAbility)
        {
            areaAbility.OnAbilityActivated += (parent) =>
            {
                // Find all AreaDamage components spawned by this ability
                AreaDamage[] areas = GameObject.FindObjectsOfType<AreaDamage>();

                foreach (var area in areas)
                {
                    // Subscribe to each area’s damage ticks
                    area.StartCoroutine(ApplyBurnToEnemies(area));
                }
            };
        }
    }

    private IEnumerator ApplyBurnToEnemies(AreaDamage area)
    {
        float elapsed = 0f;
        float radius = area.transform.localScale.x;

        while (elapsed < area.duration)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(area.transform.position, radius);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    // Add BurnEffect if not already present
                    BurnEffect burn = hit.GetComponent<BurnEffect>();
                    if (burn == null)
                    {
                        burn = hit.gameObject.AddComponent<BurnEffect>();
                    }
                    burn.ApplyBurn(damagePerTick, tickInterval, duration);
                }
            }

            elapsed += area.tickInterval;
            yield return new WaitForSeconds(area.tickInterval);
        }
    }
}
