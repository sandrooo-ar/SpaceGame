using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{

    public static UpgradeDatabase Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public Upgrade[] normalUpgrades;
    public Upgrade[] rareUpgrades;
    public Upgrade[] epicUpgrades;
    public Upgrade[] legendaryUpgrades;
    public Upgrade[] superNovaUpgrades;

    public Upgrade GetRandomUpgrade(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Normal:
                return normalUpgrades[Random.Range(0, normalUpgrades.Length)];
            case UpgradeRarity.Rare:
                return rareUpgrades[Random.Range(0, rareUpgrades.Length)];
            case UpgradeRarity.Epic:
                return epicUpgrades[Random.Range(0, epicUpgrades.Length)];
            case UpgradeRarity.Legendary:
                return legendaryUpgrades[Random.Range(0, legendaryUpgrades.Length)];
            case UpgradeRarity.SuperNova:
                return superNovaUpgrades[Random.Range(0, superNovaUpgrades.Length)];
            default:
                Debug.LogWarning("Unknown UpgradeRarity: " + rarity);
                return null;
        }
    }
}
