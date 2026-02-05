using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMaxHPUpgrade", menuName = "Upgrades/Player/Max HP")]
public class PlayerMaxHPUpgrade : Upgrade
{

    [Header("Max HP Upgrade Info")]
    public int HPIncreaseAmount;

    public override void ApplyUpgrade()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            HealthComponent playerHealth = player.GetComponent<HealthComponent>();

            if (playerHealth != null)
            {
                playerHealth.IncreaseMaxHP(HPIncreaseAmount);
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