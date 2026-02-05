using UnityEngine;
using System.Collections;

public class BurnEffect : MonoBehaviour
{
    public int damagePerTick = 5;
    public float tickInterval = 1f;
    public float duration = 5f;

    private Coroutine burnCoroutine;

    public void ApplyBurn(int damage, float interval, float totalDuration)
    {
        damagePerTick = damage;
        tickInterval = interval;
        duration = totalDuration;

        if (burnCoroutine != null && gameObject.activeSelf)
            StopCoroutine(burnCoroutine);
        
        if (gameObject.activeSelf)
            burnCoroutine = StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        float elapsed = 0f;
        var health = GetComponent<HealthComponent>();

        while (elapsed < duration)
        {
            if (health != null)
            {
                health.TakeDamage(damagePerTick, true, false, true);
            }
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(this); // Remove the burn script after finishing
    }
}
