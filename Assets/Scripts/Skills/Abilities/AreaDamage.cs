using System.Collections;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    public int playerDamage;
    public int abilityDamage;
    public float duration;
    public float tickInterval;

    public float areaMultiplier = 1f;

    public bool managedByAnimation = false;
    public int executionCount = 1;

    [Header("Explosion Settings")]
    public bool explosion = false;     // Toggle for explosion knockback
    public float explosionForce = 10f; // Strength of the push
    public float explosionRadius = 1f; // Override if different from scale
    public ForceMode2D forceMode = ForceMode2D.Impulse;

    [Header("Animation Settings")]
    public Animator animator;
    public string animationTrigger = "Play"; // trigger name in Animator

    private AudioManager sfxManager;
    private Coroutine damageRoutine;

    
    public void Initialize(int pDmg, int aDmg, float dur, float interval, AudioManager audioManager)
    {
        playerDamage = pDmg;
        abilityDamage = aDmg;
        duration = dur;
        tickInterval = interval;
        sfxManager = audioManager;

        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        if (!managedByAnimation)
        {
            damageRoutine = StartCoroutine(DamageOverTime());
        }
        else
        {
            damageRoutine = StartCoroutine(PlayAnimationExecutions());
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw Explosion Area (yellow), only if enabled
        if (explosion)
        {
            float explosionRadiusVisual = transform.localScale.x * areaMultiplier * explosionRadius;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadiusVisual);
        }
    }


    /// <summary>
    /// Applies area damage once.
    /// </summary>
    public void ApplyDamage()
    {

        // 🔹 Explosion effect on spawn
        if (explosion)
        {
            ApplyExplosionForce();
        }

        float radius = transform.localScale.x * areaMultiplier;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                HealthComponent health = hit.GetComponent<HealthComponent>();
                if (health != null)
                {
                    health.TakeDamage(playerDamage, false);
                    health.TakeDamage(abilityDamage, true);
                }
            }
        }
    }

    /// <summary>
    /// Explosion knockback on spawn.
    /// </summary>
    private void ApplyExplosionForce()
    {
        float radius = transform.localScale.x * areaMultiplier * explosionRadius;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (hit.transform.position - transform.position).normalized;
                    rb.linearVelocity = Vector2.zero; // reset current velocity
                    rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    private IEnumerator DamageOverTime()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ApplyDamage();
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }

        gameObject.SetActive(false);
    }

    private IEnumerator PlayAnimationExecutions()
    {
        if (animator == null || string.IsNullOrEmpty(animationTrigger))
        {
            Debug.LogWarning("Animator or AnimationTrigger not set on AreaDamage.");
            yield break;
        }

        for (int i = 0; i < executionCount; i++)
        {
            animator.ResetTrigger(animationTrigger);
            animator.SetTrigger(animationTrigger);

            yield return null; // Wait one frame

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float clipLength = stateInfo.length;

            yield return new WaitForSeconds(clipLength - 0.05f);
        }

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }
}
