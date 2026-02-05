using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    private Dictionary<string, int> resources = new Dictionary<string, int>();

    public void AddResource(string resource, int amount = 1)
    {
        if (resources.ContainsKey(resource))
        {
            resources[resource] += amount;
        }
        else
        {
            resources[resource] = amount;
        }

        Debug.Log($"Added {amount} of {resource}. Total now: {resources[resource]}");

        GameObject resourceIndicator = ObjectPooler.Instance.SpawnFromPool(
                           resource + "Indicator",
                           gameObject.transform.position,
                           Quaternion.identity
                       );

        if (resourceIndicator != null)
        {
            resourceIndicator.GetComponent<FloatingMessage>().ShowNumber(amount, true);
            resourceIndicator.transform.SetParent(gameObject.transform);
        }

    }

    public bool SpendResource(string resource, int amount = 1)
    {
        if (resources.ContainsKey(resource) && resources[resource] >= amount)
        {
            resources[resource] -= amount;
            return true;
        }
        return false;
    }

    public int GetResourceAmount(string resource)
    {
        return resources.ContainsKey(resource) ? resources[resource] : 0;
    }
}