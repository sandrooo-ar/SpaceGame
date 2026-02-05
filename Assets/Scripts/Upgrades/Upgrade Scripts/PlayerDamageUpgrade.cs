using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDamageUpgrade", menuName = "Upgrades/Player/Damage")]
public class PlayerDamageUpgrade : Upgrade
{

    [Header("Damage Upgrade Info")]
    public int damageIncreaseAmount;

    public override void ApplyUpgrade()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats= player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.Damage += damageIncreaseAmount;
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