using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size = 10;
        public float autoReturnTime = 0f; // 0 = use particle system lifetime if present
        public bool usePooling = true;    // ✅ new toggle: can disable pooling for this pool
    }

    public List<Pool> pools = new List<Pool>();

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, Pool> poolSettings;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolSettings = new Dictionary<string, Pool>();

        foreach (Pool pool in pools)
        {
            poolSettings.Add(pool.tag, pool);

            if (pool.usePooling)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    objectPool.Enqueue(CreatePooledObject(pool));
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
            else
            {
                poolDictionary.Add(pool.tag, new Queue<GameObject>()); // still create key for consistency
            }
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolSettings.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Pool settings = poolSettings[tag];
        GameObject objectToSpawn;

        // ✅ If pooling is disabled, just Instantiate and destroy normally
        if (!settings.usePooling)
        {
            objectToSpawn = Instantiate(settings.prefab, position, rotation, parent);
            objectToSpawn.SetActive(true);

            // Auto-destroy after time if specified
            if (settings.autoReturnTime > 0)
            {
                Destroy(objectToSpawn, settings.autoReturnTime);
            }
            else
            {
                var ps = objectToSpawn.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Destroy(objectToSpawn, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }

            return objectToSpawn;
        }

        // ✅ Pooling enabled (normal path)
        if (poolDictionary[tag].Count == 0)
        {
            objectToSpawn = CreatePooledObject(settings);
        }
        else
        {
            objectToSpawn = poolDictionary[tag].Dequeue();
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        objectToSpawn.transform.SetParent(parent);

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolSettings.ContainsKey(tag))
        {
            Destroy(obj);
            return;
        }

        if (!poolSettings[tag].usePooling)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    private GameObject CreatePooledObject(Pool pool)
    {
        GameObject obj = Instantiate(pool.prefab);
        obj.SetActive(false);

        var returner = obj.AddComponent<PoolReturner>();
        returner.poolTag = pool.tag;
        returner.autoReturnTime = pool.autoReturnTime;

        return obj;
    }

    // Internal helper for auto-return
    private class PoolReturner : MonoBehaviour
    {
        [HideInInspector] public string poolTag;
        [HideInInspector] public float autoReturnTime;

        private ParticleSystem ps;

        private void OnEnable()
        {
            if (ps == null)
                ps = GetComponent<ParticleSystem>();

            float timer = 2f; // fallback

            if (autoReturnTime > 0)
            {
                timer = autoReturnTime;
            }
            else if (ps != null)
            {
                timer = ps.main.duration + ps.main.startLifetime.constantMax;
                ps.Play(true);
            }

            StartCoroutine(ReturnAfterDelay(timer));
        }

        private System.Collections.IEnumerator ReturnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (ObjectPooler.Instance != null && !string.IsNullOrEmpty(poolTag))
            {
                ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
            }
        }
    }

    public GameObject GetPrefabByTag(string tag)
    {
        if (poolSettings.ContainsKey(tag))
            return poolSettings[tag].prefab;
        return null;
    }
}
