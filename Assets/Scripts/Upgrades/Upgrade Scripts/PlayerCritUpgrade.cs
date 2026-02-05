using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCritChanceUpgrade", menuName = "Upgrades/Player/Crit Chance")]
public class PlayerCritChanceUpgrade : Upgrade
{

    [Header("Crit Chance Upgrade Info")]
    public int critChanceIncreaseAmount;

    public override void ApplyUpgrade()
    {
            
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (playerStats.critChance + critChanceIncreaseAmount < 100)
                {
                    playerStats.critChance += critChanceIncreaseAmount;
                }
                else
                {
                    playerStats.critChance = 100;
                }
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
