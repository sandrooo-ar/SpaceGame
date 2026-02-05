using UnityEngine;
using System.Collections.Generic;

public class DecorationSpawner : MonoBehaviour
{
    [Header("Spawn Zone")]
    public Vector2 zoneSize = new Vector2(10, 10);  // X = width, Y = depth
    public float minDistance = 1f;                  // minimum spacing
    public int spawnCount = 30;                     // how many objects
    public GameObject[] prefabs;                    // decoration prefabs

    private List<Vector2> points = new List<Vector2>();

    void Start()
    {
        GeneratePoints();
        SpawnObjects();
    }

    void GeneratePoints()
    {
        int attempts = 0;
        while (points.Count < spawnCount && attempts < spawnCount * 10)
        {
            Vector2 candidate = new Vector2(
                Random.Range(-zoneSize.x / 2f, zoneSize.x / 2f),
                Random.Range(-zoneSize.y / 2f, zoneSize.y / 2f)
            );

            if (IsValid(candidate))
                points.Add(candidate);

            attempts++;
        }
    }

    bool IsValid(Vector2 candidate)
    {
        foreach (Vector2 point in points)
        {
            if (Vector2.Distance(candidate, point) < minDistance)
                return false;
        }
        return true;
    }

    void SpawnObjects()
    {
        foreach (Vector2 point in points)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            Vector3 pos = new Vector3(point.x, point.y, 0) + transform.position;
            Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(zoneSize.x, zoneSize.y));
    }
}
