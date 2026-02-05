using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    private HealthComponent playerHealth;

    private void Start()
    {
        // Destroy the UI when the player dies
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
        playerHealth.OnDeath += DestroyUI;
    }

    private void DestroyUI(bool obj)
    {
        Destroy(gameObject);
    }

}
