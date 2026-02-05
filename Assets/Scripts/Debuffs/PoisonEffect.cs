using UnityEngine;
using System.Collections;

public class PoisonEffect : MonoBehaviour
{
    public int damagePerTick = 5;
    public int decayAmount = 1;
    public float tickInterval = 1f;

    private Coroutine poisonCoroutine;

    public void ApplyPoison(int damage, int decay, float interval)
    {
        damagePerTick = damage;
        decayAmount = decay;
        tickInterval = interval;

        if (poisonCoroutine != null && gameObject.activeSelf)
            StopCoroutine(poisonCoroutine);

        if (gameObject.activeSelf)
            poisonCoroutine = StartCoroutine(PoisonRoutine());
    }

    private IEnumerator PoisonRoutine()
    {
        var health = GetComponent<HealthComponent>();

        // Continue until damagePerTick decays to 0
        while (damagePerTick > 0f)
        {
            if (health != null)
            {
                health.TakeDamage(damagePerTick, true, false, false, true);
                damagePerTick -= decayAmount;
            }
            yield return new WaitForSeconds(tickInterval);
        }

        Destroy(this);
    }
}
