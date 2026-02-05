using UnityEngine;

[CreateAssetMenu(fileName = "EXPMultiplierUpgrade", menuName = "Upgrades/Player/EXP Multiplier")]
public class PlayerEXPMultiplierUpgrade : Upgrade
{

    [Header("EXP Multiplier Upgrade Info")]
    public float expMultiplierIncreaseAmount;

    public override void ApplyUpgrade()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats= player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.EXPMultiplier += expMultiplierIncreaseAmount;
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