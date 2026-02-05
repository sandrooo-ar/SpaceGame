using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackSpeedUpgrade", menuName = "Upgrades/Player/Attack Speed")]
public class PlayerAttackSpeedUpgrade : Upgrade
{

    [Header("Attack Speed Upgrade Info")]
    public float attackSpeedIncreaseAmount;

    public override void ApplyUpgrade()
    {
            
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (playerStats.AttackSpeed - attackSpeedIncreaseAmount > 0.1f)
                {
                    playerStats.AttackSpeed -= attackSpeedIncreaseAmount;
                }
                else
                {
                    playerStats.AttackSpeed = 0.1f;
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
