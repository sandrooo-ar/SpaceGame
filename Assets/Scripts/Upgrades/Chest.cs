using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class Chest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private GameObject discountTag;
    [SerializeField] private TextMeshProUGUI discountText;

    public ChestPannel chestPannel;
    public UpgradeSystem upgradeSystem;

    public AudioSource hoverSFX;
    public AudioManager audioManager;
    public AudioClip openChestSFX;
    public AudioClip closeChestSFX;



    private bool isHovering = false;

    public int upgradeCount = 1;

    public bool isDiscounted = false;

    public enum ChestType { Normal, Rare, Epic, Legendary }
    public ChestType chestType;

    [Header("Chest Cost")]
    public int baseCost;   // original cost (set in inspector)
    private int cost;      // current cost (calculated)

    // Probabilities for each chest type
    private float[] normalChest = { 0.70f, 0.20f, 0.08f, 0.019f, 0.001f };
    private float[] rareChest = { 0.20f, 0.40f, 0.30f, 0.095f, 0.005f };
    private float[] epicChest = { 0.05f, 0.20f, 0.40f, 0.33f, 0.02f };
    private float[] legendaryChest = { 0.01f, 0.04f, 0.15f, 0.75f, 0.05f };

    private void OnEnable()
    {
        // Reset to base cost when chest is shown
        cost = baseCost;
        price.text = cost.ToString();
        discountTag.SetActive(false);
        isDiscounted = false;
    }

    public void ApplyDiscount(float discount)
    {
        if (isDiscounted) return;

        // Calculate discount from base cost
        cost = Mathf.CeilToInt(baseCost * (1 - discount));

        price.text = cost.ToString();
        discountTag.SetActive(true);
        discountText.text = $"{Mathf.RoundToInt(discount * 100)}%";
        isDiscounted = true;
    }

    public void StartHover()
    {

        if (isHovering) return;
        isHovering = true;
        audioManager.PlaySound(openChestSFX, 0.1f);
        StopAllCoroutines(); // Stop any ongoing fade to prevent conflicts
        StartCoroutine(FadeIn());
    }

    public void EndHover()
    {
        if (!isHovering) return;
        isHovering = false;
        audioManager.PlaySound(closeChestSFX, 0.1f);
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        hoverSFX.volume = 0f;

        float fadeDuration = 0.3f; // seconds   

        if (!hoverSFX.isPlaying)
            hoverSFX.Play();

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            hoverSFX.volume = Mathf.Lerp(0f, 0.05f, elapsed / fadeDuration);
            yield return null;
        }

        Debug.Log("Hover SFX faded in");

        hoverSFX.volume = 0.05f;
    }

    private IEnumerator FadeOut()
    {
        float fadeDuration = 0.3f; // seconds
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            hoverSFX.volume = Mathf.Lerp(0.05f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        hoverSFX.Stop();
        hoverSFX.volume = 0f;
        Debug.Log("Hover SFX faded out");
    }

    public void OpenChestMultipleRewards()
    {
        if (CoinManager.Instance.GetCoinCount() < cost)
        {
            Debug.Log("Not enough coins to open chest!");
            return;
        }

        if (chestPannel == null)
        {
            Debug.LogError("ChestPannel or UpgradeSystem reference is missing!");
            return;
        }

        CoinManager.Instance.SpendCoins(cost);
        chestPannel.ChangeCoins();



        upgradeCount = 1;

        // Always at least 1 upgrade
        Upgrade upgrade1 = OpenChest();
        Upgrade upgrade2 = null;
        Upgrade upgrade3 = null;
        Upgrade upgrade4 = null;
        Upgrade upgrade5 = null;

        float roll = UnityEngine.Random.value; // 0.0 - 1.0

        // 5 Upgrades - 5% chance
        if (roll < 0.05f)
        {
            upgrade2 = OpenChest();
            upgrade3 = OpenChest();
            upgrade4 = OpenChest();
            upgrade5 = OpenChest();

            upgradeCount = 5;
        }
        // 3 Upgrades - 20% chance
        else if (roll < 0.2f)
        {
            upgrade2 = OpenChest();
            upgrade3 = OpenChest();

            upgradeCount = 3;
        }

        upgradeSystem.ShowUpgrades(chestType, upgrade1, upgrade2, upgrade3, upgrade4, upgrade5);
    }

    public Upgrade OpenChest()
    {
        float roll = Random.value; // random float 0.0 - 1.0
        float cumulative = 0f;
        float[] table = GetProbabilityTable();

        for (int i = 0; i < table.Length; i++)
        {
            cumulative += table[i];
            if (roll < cumulative)
            {
                return GetUpgrade((UpgradeRarity)i);
            }
        }

        // fallback safety
        return GetUpgrade(UpgradeRarity.Normal);
    }

    private float[] GetProbabilityTable()
    {
        switch (chestType)
        {
            case ChestType.Normal: return normalChest;
            case ChestType.Rare: return rareChest;
            case ChestType.Epic: return epicChest;
            case ChestType.Legendary: return legendaryChest;
            default: return normalChest;
        }
    }

    private Upgrade GetUpgrade(UpgradeRarity rarity)
    {
        return UpgradeDatabase.Instance.GetRandomUpgrade(rarity);
    }
}
