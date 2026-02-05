using UnityEngine;
using System.Collections;

public class MeleeHitbox : MonoBehaviour
{
    private int damage;
    private Ability sourceAbility;
    private Coroutine disableRoutine;

    [SerializeField] private Collider2D hitCollider; // assign in prefab

    public void Initialize(int dmg, float activeTime, Ability ability)
    {
        damage = dmg;
        sourceAbility = ability;

        if (hitCollider != null)
            hitCollider.enabled = true;

        if (disableRoutine != null)
            StopCoroutine(disableRoutine);

        disableRoutine = StartCoroutine(DisableAfterTime(activeTime));
    }

    private IEnumerator DisableAfterTime(float t)
    {
        yield return new WaitForSeconds(t);

        if (hitCollider != null)
            hitCollider.enabled = false;

        transform.SetParent(null); // detach from spawner
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(damage, true);
            }
        }
    }
}
