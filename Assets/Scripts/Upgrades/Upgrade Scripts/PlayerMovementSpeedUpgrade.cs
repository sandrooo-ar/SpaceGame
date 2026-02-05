using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementSpeedUpgrade", menuName = "Upgrades/Player/Movement Speed")]
public class PlayerMovementSpeedUpgrade : Upgrade
{

    [Header("Movement Speed Upgrade Info")]
    public float movementSpeedIncreaseAmount;

    public override void ApplyUpgrade()
    {
            
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (playerStats.MoveSpeed + movementSpeedIncreaseAmount < 10)
                {
                    playerStats.MoveSpeed += movementSpeedIncreaseAmount;
                }
                else
                {
                    playerStats.MoveSpeed = 10;
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
