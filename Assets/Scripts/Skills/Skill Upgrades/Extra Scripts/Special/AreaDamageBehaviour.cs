using UnityEngine;

[CreateAssetMenu(fileName = "AreaDamageBehavior", menuName = "Abilities/SpecialUpgrades/AreaDamage")]
public class AreaDamageBehavior : UpgradeBehavior
{
    [Header("Damage Settings")]
    public int damage = 10;
    public LayerMask enemyLayers;

    [Header("Visual Settings")]
    public string areaEffectPoolTag = "DashAreaDamage";

    [Header("Oval Settings")]
    public float baseRadius = 2f;
    public float tiltAngle = 60f; // degrees
    public float yOffset = 0f;    // vertical offset of the oval
    public int ellipseSegments = 32; // smoothness of gizmo ellipse

    public override void Apply(Ability ability)
    {
        ability.OnAbilityActivated += (parent) =>
        {
            if (parent == null) return;

            // Compute ellipse radii
            float radiusX = baseRadius;
            float radiusY = baseRadius * Mathf.Cos(tiltAngle * Mathf.Deg2Rad);

            // Compute center with Y offset
            Vector3 center = parent.transform.position + new Vector3(0f, yOffset, 0f);

            // Bounding circle to avoid missing targets
            float maxRadius = Mathf.Max(radiusX, radiusY);
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(center, maxRadius, enemyLayers);

            // Apply tilt along the X axis
            Quaternion rotation = Quaternion.Euler(tiltAngle, 0f, 0f);

            // Spawn the AoE effect with tilt applied
            ObjectPooler.Instance.SpawnFromPool(
                areaEffectPoolTag,
                center,
                rotation
            );

            foreach (var enemy in hitEnemies)
            {

                Vector2 offset = enemy.transform.position - center;

                // Ellipse check: (x² / a²) + (y² / b²) <= 1
                float normX = offset.x / radiusX;
                float normY = offset.y / radiusY;
                float ellipseCheck = (normX * normX) + (normY * normY);

                if (ellipseCheck <= 1f)
                {
                    HealthComponent health = enemy.GetComponent<HealthComponent>();
                    if (health != null)
                    {
                        health.TakeDamage(damage, true);
                    }
                }
            }
        };
    }
}
