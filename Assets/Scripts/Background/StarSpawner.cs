using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject starPrefab;
    public int starCount = 100;
    public Vector2 areaSize = new Vector2(20f, 10f);

    public void Start()
    {
        // Asegúrate de que el prefab de estrella esté asignado
        if (starPrefab == null)
        {
            Debug.LogError("Star Prefab is not assigned in the StarSpawner.");
            return;
        }
        // Genera las estrellas al iniciar
        RegenerateStars();
    }

    [ContextMenu("Regenerate Stars")]
    public void RegenerateStars()
    {
        // Borra todas las estrellas anteriores
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Crea nuevas estrellas
        for (int i = 0; i < starCount; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            GameObject star = Instantiate(starPrefab, spawnPos, Quaternion.identity, transform);
            star.name = $"Star_{i}";
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
