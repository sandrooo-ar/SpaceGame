using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRegenHPUpgrade", menuName = "Upgrades/Player/Regen HP")]
public class PlayerRegenHPUpgrade : Upgrade
{

    [Header("Regen HP Upgrade Info")]
    public float HPRegenAmount;

    public override void ApplyUpgrade()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            HealthComponent playerHealth = player.GetComponent<HealthComponent>();

            if (playerHealth != null)
            {
                playerHealth.healthRegenRate += HPRegenAmount;
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on the player object.");
            }

        }
        else
        {
            Debug.LogWarning("Player object not found in the scene.");
        }

    }
}