using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public int critChance = 50; // 0 to 100
    public int Damage = 10;
    public float MoveSpeed = 5f;
    public float AttackSpeed = 1f;

    public int extraCoins = 0;

    public float DamageMultiplier = 1f;
    public float AttackSpeedMultiplier = 1f;
    public float MoveSpeedMultiplier = 1f;
    public float CDReductionMultiplier = 1f;
    public float EXPMultiplier = 1f;

    public int maxMana = 100;
    public int manaRegenRate = 5; // Mana per second

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional, keep across scenes
    }

}