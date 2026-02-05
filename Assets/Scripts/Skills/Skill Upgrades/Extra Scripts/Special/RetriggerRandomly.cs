using UnityEngine;

[CreateAssetMenu(fileName = "RetriggerRandomly", menuName = "Abilities/SpecialUpgrades/RetriggerRandomly")]
public class RetriggerRandomly : UpgradeBehavior
{
    public int retriggerCount = 1;

    [Header("Spawn Area (World Bounds)")]
    public Vector2 minSpawnBounds; // Bottom-left corner of map
    public Vector2 maxSpawnBounds; // Top-right corner of map

    public override void Apply(Ability ability)
    {
        // Subscribe once: whenever this ability activates, also spawn extra effects
        ability.OnAbilityActivated += (parent) =>
        {
            for (int i = 0; i < retriggerCount; i++)
            {
                float randX = Random.Range(minSpawnBounds.x, maxSpawnBounds.x);
                float randY = Random.Range(minSpawnBounds.y, maxSpawnBounds.y);
                Vector3 spawnPos = new Vector3(randX, randY, 0f);

                // We only handle AreaDamageAbility here
                if (ability is AreaDamageAbility areaAbility)
                {
                    SpawnExtra(areaAbility, spawnPos, parent);
                }
            }
        };
    }

    private void SpawnExtra(AreaDamageAbility areaAbility, Vector3 position, GameObject parent)
    {
        GameObject areaEffect = ObjectPooler.Instance.SpawnFromPool(areaAbility.areaEffectPoolTag, position, Quaternion.identity);

        if (areaEffect != null)
        {
            areaEffect.transform.localScale = Vector3.one * areaAbility.areaScale;

            AreaDamage dmg = areaEffect.GetComponent<AreaDamage>();
            if (dmg != null)
            {
                dmg.Initialize(
                    (int)(PlayerStats.Instance.Damage * PlayerStats.Instance.DamageMultiplier),
                    areaAbility.baseDamage,
                    areaAbility.duration,
                    areaAbility.tickInterval,
                    areaAbility.abilityHolder.abilitySFX
                );
                dmg.executionCount = areaAbility.executionCount;
            }
        }
    }
}
