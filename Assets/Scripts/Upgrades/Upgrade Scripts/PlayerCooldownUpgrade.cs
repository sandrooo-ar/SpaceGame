using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCooldownUpgrade", menuName = "Upgrades/Player/CooldownReduction")]
public class PlayerCooldownUpgrade : Upgrade
{

    [Header("Cooldown Upgrade Info")]
    public float cooldownMultiplierAmount;

    public override void ApplyUpgrade()
    {
            
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.CDReductionMultiplier += cooldownMultiplierAmount;
            }
            else
            {
                Debug.LogWarning("PlayerStats component not found on the player object.");
            }

        }
        else
        {
            Debug.LogWarning("Player object not found in the scene.");
        }

    }

}
