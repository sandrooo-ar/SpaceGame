using System.Collections;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [Header("Resource Settings")]
    public string resourceName;
    public int quantityGiven = 1;

    private bool isCollected = false;

    [SerializeField] private float scaleUpDuration = 0.5f;
    [SerializeField] private Vector3 targetScale = Vector3.one;

    private HealthComponent healthComponent;

    private ResourceSpawner spawner;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.OnDeath += OnDeath;
        }

        transform.localScale = Vector3.zero; // Start scaled down
        StartCoroutine(ScaleUp());
    }

    // Called by spawner when this is instantiated
    public void SetSpawner(ResourceSpawner originSpawner)
    {
        spawner = originSpawner;
    }

    private void OnDeath(bool playerDamage)
    {

        if (isCollected) return; // Prevent double collection

        isCollected = true;

        if (playerDamage)
        {
            // Tell spawner we’ve been destroyed
            if (spawner != null)
            {
                spawner.OnResourceCollected(gameObject);
            }

            GameObject drop = ObjectPooler.Instance.SpawnFromPool(
                resourceName,
                transform.position,
                Quaternion.identity
            );

            if (drop != null)
            {
                drop.GetComponent<BaseDrop>().dropAmount = quantityGiven;
                drop.GetComponent<BaseDrop>().isCoin = false;
                drop.GetComponent<BaseDrop>().resourceName = resourceName;
            }
        }

        // Destroy the prefab after death
        Destroy(gameObject, 1f);
    }


    private IEnumerator ScaleUp()
    {
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        while (elapsed < scaleUpDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / scaleUpDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale; // Ensure final scale is set
    }

}
