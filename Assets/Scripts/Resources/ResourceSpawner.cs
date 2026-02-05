using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public static ResourceSpawner Instance { get; set; }

    [Header("References")]
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private Transform player; // assign Player Transform in Inspector

    [Header("Spawn Settings")]
    [SerializeField] private int maxResources = 3;
    [SerializeField] private float minDistanceFromPlayer = 5f; // must be outside this radius
    [SerializeField] private float minDistanceBetweenResources = 3f; // NEW: spacing between resources
    [SerializeField] private Vector2 areaSize = new Vector2(20f, 20f); // area centered on spawner

    public int resourceQuantity = 1; // amount each resource gives when collected

    private List<GameObject> activeResources = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void SpawnResources()
    {
        for (int i = 0; i < maxResources; i++)
        {
            SpawnResource();
        }
    }

    public void SpawnResource()
    {
        if (resourcePrefab == null || player == null) return;

        Vector3 spawnPos = GetValidSpawnPosition();

        GameObject resourceObj = Instantiate(resourcePrefab, spawnPos, Quaternion.identity);

        Resource resource = resourceObj.GetComponent<Resource>();
        if (resource != null)
        {
            resource.SetSpawner(this);
            resource.quantityGiven = resourceQuantity;
        }

        activeResources.Add(resourceObj);
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        int safety = 0;

        do
        {
            float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float z = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
            spawnPos = transform.position + new Vector3(x, z, 0); // Z axis for depth

            safety++;
            if (safety > 100) // raise safety cap since now we check two conditions
            {
                Debug.LogWarning("Could not find valid spawn position.");
                break;
            }

        }
        while (
            Vector3.Distance(player.position, spawnPos) < minDistanceFromPlayer ||
            !IsFarEnoughFromOtherResources(spawnPos)
        );

        return spawnPos;
    }

    private bool IsFarEnoughFromOtherResources(Vector3 pos)
    {
        foreach (var res in activeResources)
        {
            if (res == null) continue;
            if (Vector3.Distance(res.transform.position, pos) < minDistanceBetweenResources)
            {
                return false;
            }
        }
        return true;
    }

    public void OnResourceCollected(GameObject collectedObj)
    {
        if (activeResources.Contains(collectedObj))
        {
            activeResources.Remove(collectedObj);
            SpawnResource();
        }
    }

    public void DestroyResources()
    {
        // Loop backwards so removal won't break the list
        for (int i = activeResources.Count - 1; i >= 0; i--)
        {
            GameObject res = activeResources[i];
            if (res != null)
            {
                var health = res.GetComponent<HealthComponent>();
                if (health != null)
                {
                    health.diedFromExternalDamage = false;
                    health.Die();
                    health.healthBar.gameObject.SetActive(false);
                }
                else
                {
                    Destroy(res);
                }
            }
            activeResources.RemoveAt(i); // manually remove from list
        }
    }

}

