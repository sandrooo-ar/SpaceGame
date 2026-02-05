using UnityEngine;

public enum UpgradeRarity
{
    Normal,
    Rare,
    Epic,
    Legendary,
    SuperNova
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("General Info")]
    public string upgradeName;
    public string description;
    public string statDescription;
    public int price;
    public Sprite icon;
    public UpgradeRarity rarity;

    public virtual void ApplyUpgrade()
    {
    }

}