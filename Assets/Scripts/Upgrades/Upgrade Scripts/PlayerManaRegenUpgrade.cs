using UnityEngine;

[CreateAssetMenu(fileName = "PlayerManaRegenUpgrade", menuName = "Upgrades/Player/Mana Regen")]
public class PlayerManaRegenUpgrade : Upgrade
{

    [Header("Mana Regen Upgrade Info")]
    public int manaRegenIncreaseAmount;

    public override void ApplyUpgrade()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats= player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.manaRegenRate += manaRegenIncreaseAmount;
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