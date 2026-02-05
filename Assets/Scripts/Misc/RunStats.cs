using UnityEngine;

public class RunStats : MonoBehaviour
{
    public static RunStats Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int currentRound = 1;

    public int playerLevel = 1;

    public int enemiesKilled = 0;
    public int enemiesSpawned = 0;

    public int coinsCollected = 0;
    public int coinsSpawned = 0;

}
