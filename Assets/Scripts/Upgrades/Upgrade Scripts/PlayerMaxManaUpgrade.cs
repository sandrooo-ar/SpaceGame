using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMaxManaUpgrade", menuName = "Upgrades/Player/Max Mana")]
public class PlayerMaxManaUpgrade : Upgrade
{

    [Header("Max Mana Upgrade Info")]
    public int manaIncreaseAmount;

    public override void ApplyUpgrade()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats= player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.maxMana += manaIncreaseAmount;
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