using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "FlashAbility", menuName = "Abilities/FlashAbility")]
public class FlashAbility : Ability
{
    public float flashDistance = 5f;
    public float invincibilityDuration = 0.25f;

    public override void Activate(GameObject parent)
    {
        var playerMovement = parent.GetComponent<PlayerMovement>();
        var rb = parent.GetComponent<Rigidbody2D>();

        if (playerMovement == null || rb == null)
        {
            Debug.LogWarning("Missing required component(s).");
            return;
        }

        Vector2 direction = playerMovement.CurrentDirection.normalized;

        if (direction != Vector2.zero)
        {
            parent.GetComponent<MonoBehaviour>().StartCoroutine(Invincibility(parent));

            // --- FLASH FUNCTIONALITY ---
            Vector2 targetPosition = rb.position + direction * flashDistance;

            // Optional: Raycast to stop flash through walls
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, flashDistance, LayerMask.GetMask("Walls"));
            if (hit.collider != null)
            {
                targetPosition = hit.point; // stop at the wall
            }

            rb.position = targetPosition;

            // --- VFX 1 ---
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            var effect = ObjectPooler.Instance.SpawnFromPool(
                "DashEffectOne",
                parent.transform.position,
                Quaternion.Euler(0, 0, angle)
            );

            effect.transform.SetParent(parent.transform);

            // --- VFX 2 ---
            ObjectPooler.Instance.SpawnFromPool(
                "DashEffectTwo",
                parent.transform.position,
                Quaternion.identity
            );

            // --- SOUND EFFECT ---
            if (abilityHolder.abilitySFX != null)
            {
                abilityHolder.abilitySFX.PlaySoundAtRandomPitch(soundEffect);
            }

            base.Activate(parent);
        }
        else
        {
            Debug.LogWarning("Flash ability failed: Player is not moving.");
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
