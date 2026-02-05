using UnityEngine;

[CreateAssetMenu(fileName = "ExtraCoinsUpgrade", menuName = "Upgrades/Player/Extra Coins")]
public class PlayerExtraCoinsUpgrade : Upgrade
{

    [Header("Extra Coins Upgrade Info")]
    public int coinsIncreaseAmount;

    public override void ApplyUpgrade()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats= player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.extraCoins += coinsIncreaseAmount;
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