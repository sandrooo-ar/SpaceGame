using UnityEngine;

[CreateAssetMenu(fileName = "AimshotAbility", menuName = "Abilities/AimshotAbility")]
public class AimshotAbility : Ability
{

    // BASE ABILITY STATS
    [Header("Aimshot Ability Stats")]
    public int baseDamage = 5;
    public float projectileSpeed = 10f;
    public float projectileScale = 1f;
    public float projectileLifetime = 5f;

    public string effectPoolTag = "BaseProjectileEffectOne";

    public Material projectileMaterial;

    /*
    private int bounceCount = 0;
    private bool pierceEnabled = false;
    */

    public override void Activate(GameObject parent)
    {

        // Spawn projectile from pool
        GameObject skillSpawner = GameObject.FindGameObjectWithTag("SkillSpawner");
        if (skillSpawner == null) return;

        Vector2 direction = skillSpawner.GetComponent<RotateAroundPlayer>().GetCurrentDirection().normalized;

        GameObject projectile = ObjectPooler.Instance.SpawnFromPool(
            "BaseProjectile",
            skillSpawner.transform.position,
            Quaternion.identity
        );

        GameObject effect = ObjectPooler.Instance.SpawnFromPool(
            effectPoolTag,
            skillSpawner.transform.position,
            Quaternion.identity
        );

        effect.transform.SetParent(skillSpawner.transform);
        effect.transform.localPosition = Vector3.zero;


        if (projectile != null)
        {
            var proj = projectile.GetComponent<AbilityProjectile>();

            // These stats are set on projectile spawn
            proj.SetDirection(direction);
            proj.SetAudioManager(abilityHolder.abilitySFX);
            proj.SetProjectileScale(projectileScale);
            proj.SetProjectileMaterial(projectileMaterial);
            proj.abilityDamage = baseDamage;
            proj.lifeTime = projectileLifetime;

            /*
            if (bounceCount > 0)
                proj.EnableBounce(bounceCount);
            */

            if (abilityHolder.abilitySFX != null)
                abilityHolder.abilitySFX.PlaySoundAtRandomPitch(soundEffect, 0.1f);

            base.Activate(parent);
        }
    }

    /*
    public void EnableBounce(int count)
    {
        bounceCount = count;
    }

    public void EnablePierce()
    {
        pierceEnabled = true;
    }
    */
}
