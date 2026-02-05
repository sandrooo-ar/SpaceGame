using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "DashAbility", menuName = "Abilities/DashAbility")]
public class DashAbility : Ability
{

    public float dashVelocity = 20f;
    public float invincibilityDuration = 0.5f;

    public override void Activate(GameObject parent)
    {
        var playerMovement = parent.GetComponent<PlayerMovement>();
        var rb = parent.GetComponent<Rigidbody2D>();

        if (playerMovement == null || rb == null)
        {
            Debug.LogWarning("Missing required component(s).");
            return;
        }

        Vector2 direction = playerMovement.CurrentDirection;

        if (direction != Vector2.zero)
        {

            parent.GetComponent<MonoBehaviour>().StartCoroutine(Invincibility(parent));

            // - FUNCTIONALITY
            rb.AddForce(direction * dashVelocity, ForceMode2D.Impulse);

            // - VFX 1
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (angle < 0)
                angle += 360f;

            var effect = ObjectPooler.Instance.SpawnFromPool(
                "DashEffectOne",
                parent.transform.position,
                Quaternion.Euler(0, 0, angle)
            );

            // - VFX 2
            effect.transform.SetParent(parent.transform);

            ObjectPooler.Instance.SpawnFromPool(
                "DashEffectTwo",
                parent.transform.position,
                Quaternion.Euler(0, 0, 0)
            );


            // - SOUND EFFECT
            if (abilityHolder.abilitySFX != null)
            {
                abilityHolder.abilitySFX.PlaySoundAtRandomPitch(soundEffect);
            }

            base.Activate(parent);
        }
        // The ability didn't activate because the player is not moving 
        else
        {
            Debug.LogWarning("Dash ability failed: Player is not moving.");
            abilityHolder.ResetAbility();
        }
    }

    private IEnumerator Invincibility(GameObject parent)
    {

        var playerHealth = parent.GetComponent<HealthComponent>();

        if (playerHealth != null)
        {
            playerHealth.takesDamage = false;
            yield return new WaitForSeconds(invincibilityDuration);
            playerHealth.takesDamage = true;
        }
        else
        {
            Debug.LogWarning("No HealthComponent found on the player.");
        }

    }

}