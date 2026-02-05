using Coffee.UIEffects;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [Header("Dependencies")]
    public UpgradeSystem upgradeSystem;
    public Upgrade upgrade;
    public bool isShopUpgrade = false;

    public bool alreadyBought = false;

    public Image cantBuyEffect;

    public GameObject chestPannel;
    public GameObject originalCanvas;

    private Chest.ChestType currentChestType;
    private System.Action currentOnComplete;

    private Coroutine notEnoughCoinsCoroutine;

    // Discount properties
    public float discountPercentage = 0f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private Coroutine slotCoroutine;
    private UIEffect currentBorderEffect;

    public ParticleSystem currentStarParticles;

    // AUDIO
    [Header("Audio")]
    public AudioManager audioManager;
    public AudioClip brightPickUpSFX;
    public AudioClip coinSpendSFX;
    public AudioClip hoverStartSFX;
    public AudioClip hoverEndSFX;
    public AudioClip cardExplosionSFX;


    [Header("UI Components")]
    [SerializeField] private ButtonHoverScale hoverEffect;
    [SerializeField] private ButtonHoverScale selectButtonHoverEffect;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image itemRarityImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI itemStatDescription;
    [SerializeField] private Image selectButtonImage;
    [SerializeField] private TextMeshProUGUI selectButtonText;
    [SerializeField] private Image priceImage;
    [SerializeField] private GameObject priceButton;
    [SerializeField] private TextMeshProUGUI priceButtonText;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private ParticleSystem hoverParticles;
    [SerializeField] private ParticleSystem selectParticles;
    [SerializeField] private ParticleSystem selectExtraParticles;
    [SerializeField] private UIEffect hoverBorderEffect;
    [SerializeField] private GameObject discountTag;

    // Rarity data container
    [System.Serializable]
    public struct RarityData
    {
        public UpgradeRarity rarity;
        public GameObject header, border, particles, selectParticles, selectExtraParticles;
        public UIEffect borderEffect;
        public Sprite raritySprite;
        public Material material;
        public TMP_FontAsset font;
        public Color mainColor;
        public Color textColor;
    }

    [Header("Rarity Data Settings")]
    [SerializeField] private RarityData[] rarities;

    // Probabilities for each chest type
    private readonly float[] normalChest = { 0.70f, 0.20f, 0.08f, 0.019f, 0.001f };
    private readonly float[] rareChest = { 0.20f, 0.40f, 0.30f, 0.095f, 0.005f };
    private readonly float[] epicChest = { 0.05f, 0.20f, 0.40f, 0.33f, 0.02f };
    private readonly float[] legendaryChest = { 0.01f, 0.04f, 0.15f, 0.75f, 0.05f };

    private void OnEnable()
    {
        selectButton.SetActive(false);
        BindHoverEvents();

        if (isShopUpgrade)
        {
            ShowRandomShopUpgrade();
            priceButtonText.text = upgrade.price.ToString();
            alreadyBought = false;
            priceButton.SetActive(true);
            discountTag.SetActive(false);
        }
        else
        {
            discountTag.SetActive(false);
        }

        if (originalPosition == Vector3.zero)
            originalPosition = transform.localPosition;
        else
            transform.localPosition = originalPosition;

        if (originalScale == Vector3.zero)
            originalScale = transform.localScale;
        
        if (isShopUpgrade)
        {
            transform.DOKill();

            transform.localScale = Vector3.zero;

            transform.DOScale(originalScale, 0.3f)
                     .SetEase(Ease.OutBack)
                     .SetUpdate(true);
        }
        else 
        {             
            transform.localScale = originalScale;
        }

    }

    private void OnDisable()
    {
        hoverEffect.OnHoverEnter -= ActivateHover;
        hoverEffect.OnHoverExit -= DeactivateHover;
        selectButtonHoverEffect.OnHoverEnter -= ActivateHover;
        selectButtonHoverEffect.OnHoverExit -= DeactivateHover;
    }

    public void StopParticles()
    { 
        if (hoverParticles != null)
        {
            hoverParticles.gameObject.SetActive(false);
        }
    }

    public void ResetSlot()
    {
        upgrade = null;
        itemImage.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
        itemStatDescription.text = "";
        alreadyBought = false;
        selectButton.SetActive(false);
        priceButton.SetActive(false);
        discountTag.SetActive(false);
    }

    private void BindHoverEvents()
    {
        hoverEffect.OnHoverEnter += ActivateHover;
        hoverEffect.OnHoverExit += DeactivateHover;
        selectButtonHoverEffect.OnHoverEnter += ActivateHover;
        selectButtonHoverEffect.OnHoverExit += DeactivateHover;
    }

    private void ActivateHover()
    {
        if (!currentBorderEffect) return;
        currentBorderEffect.enabled = true;

        audioManager.PlaySoundAtRandomPitch(hoverStartSFX, volume: 0.05f, 0.9f, 1.1f);

        // Increase particles per burst cycle (NOT cycle count)
        var emission = hoverParticles.emission;
        ParticleSystem.Burst burst = emission.GetBurst(0);

        burst.count = new ParticleSystem.MinMaxCurve(
            burst.count.constant * 5f
        );

        emission.SetBurst(0, burst);

        // ⚡ Speed up the entire particle simulation
        var main = hoverParticles.main;
        main.simulationSpeed = main.simulationSpeed * 1.5f;
    }

    private void DeactivateHover()
    {
        if (!currentBorderEffect) return;
        currentBorderEffect.enabled = false;

        audioManager.PlaySoundAtRandomPitch(hoverEndSFX, volume: 0.05f, 0.9f, 1.1f);

        // Reset particles per burst cycle
        var emission = hoverParticles.emission;
        ParticleSystem.Burst burst = emission.GetBurst(0);

        burst.count = new ParticleSystem.MinMaxCurve(
            burst.count.constant / 5f
        );

        emission.SetBurst(0, burst);

        // 🐢 Restore simulation speed
        var main = hoverParticles.main;
        main.simulationSpeed = main.simulationSpeed / 1.5f;
    }



    public void ApplyDiscount(float discount)
    {
        discountPercentage = discount;
        priceButtonText.text = Mathf.CeilToInt(upgrade.price * (1 - discountPercentage / 100f)).ToString();
        discountTag.SetActive(true);
        discountTag.GetComponentInChildren<TextMeshProUGUI>().text = $"-{Mathf.RoundToInt(discountPercentage)}%";
    }

    public void ShowRandomShopUpgrade()
    {
        var randomUpgrade = GetRandomUpgradeInstant(WaveSpawner.Instance.currentChestType);
        SetUpgrade(randomUpgrade);
    }

    public Upgrade GetRandomUpgradeInstant(Chest.ChestType chestType)
    {
        var rarity = GetRandomRarity(chestType);
        return UpgradeDatabase.Instance.GetRandomUpgrade(rarity);
    }

    public void SetUpgrade(Upgrade newUpgrade)
    {
        if (!newUpgrade) return;

        upgrade = newUpgrade;
        itemImage.sprite = upgrade.icon;
        itemName.text = upgrade.upgradeName;
        itemDescription.text = upgrade.description;
        itemStatDescription.text = upgrade.statDescription;

        // Reset all rarity visuals first
        foreach (var r in rarities)
        {
            r.header?.SetActive(false);
            r.border?.SetActive(false);
            r.particles?.SetActive(false);
        }

        // Apply the correct rarity visuals
        var data = GetRarityData(upgrade.rarity);
        if (data == null) return;

        data.Value.header?.SetActive(true);
        data.Value.border?.SetActive(true);
        data.Value.particles?.SetActive(true);
        hoverParticles = data.Value.particles?.GetComponent<ParticleSystem>();
        selectParticles = data.Value.selectParticles?.GetComponent<ParticleSystem>();
        selectExtraParticles = data.Value.selectExtraParticles?.GetComponent<ParticleSystem>();
        currentBorderEffect = data.Value.borderEffect;

        // Apply visuals
        itemRarityImage.sprite = data.Value.raritySprite;
        ApplyFontAndMaterial(data.Value.font, data.Value.material);
        ApplyButtonAndTextColor(data.Value.mainColor, data.Value.textColor);
    }

    private void ApplyFontAndMaterial(TMP_FontAsset font, Material mat)
    {
        foreach (var text in new[] { itemName, itemDescription, itemStatDescription })
        {
            if (!text) continue;
            text.font = font;
            text.fontSharedMaterial = font.material;
            text.ForceMeshUpdate();
        }
        if (selectButtonImage) selectButtonImage.material = mat;
        if (priceImage) priceImage.material = mat;
        if (discountTag) discountTag.GetComponent<Image>().material = mat;
    }

    private void ApplyButtonAndTextColor(Color mainColor, Color textColor)
    {
        selectButtonText.color = textColor;
        var spriteColor = mainColor; spriteColor.a = 0.1f;
        //itemImage.color = spriteColor;
    }

    private RarityData? GetRarityData(UpgradeRarity rarity)
    {
        foreach (var r in rarities)
            if (r.rarity == rarity) return r;
        return null;
    }

    public void OnSelectButtonPressed()
    {
        DeactivateHover();
        upgrade.ApplyUpgrade();
        selectButton.SetActive(false);
        upgradeSystem.PickedUpgrade();
        currentStarParticles?.Stop();
        currentStarParticles = null;

        audioManager.PlaySoundAtRandomPitch(hoverEndSFX, volume: 0.05f, 0.9f, 1.1f);

        if (brightPickUpSFX)
            switch (upgrade.rarity)
            {
                case UpgradeRarity.Normal:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.5f, pitch: 0.9f);
                    break;
                case UpgradeRarity.Rare:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.5f, pitch: 1f);
                    break;
                case UpgradeRarity.Epic:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.5f, pitch: 1.1f);
                    break;
                case UpgradeRarity.Legendary:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.5f, pitch: 1.2f);
                    break;
                case UpgradeRarity.SuperNova:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.5f, pitch: 1.3f);
                    break;
            }

        StartCoroutine(ChosenItemEffect());
    }

    public void OnPriceButtonPressed()
    {
        if (CoinManager.Instance.GetCoinCount() < Mathf.CeilToInt(upgrade.price * (1 - discountPercentage / 100f)))
        {
            if(notEnoughCoinsCoroutine == null) notEnoughCoinsCoroutine = StartCoroutine(NotEnoughCoinsEffect());
            return;
        }

        DeactivateHover();
        alreadyBought = true;
        CoinManager.Instance.SpendCoins(Mathf.CeilToInt(upgrade.price * (1 - discountPercentage / 100f)));
        upgradeSystem.coins.text = CoinManager.Instance.GetCoinCount().ToString();
        chestPannel.GetComponent<ChestPannel>().ItemBought();

        audioManager.PlaySoundAtRandomPitch(hoverEndSFX, volume: 0.05f, 0.9f, 1.1f);
        audioManager.PlaySoundAtRandomPitch(cardExplosionSFX, volume: 0.2f, 0.9f, 1.1f);

        if (coinSpendSFX)
            audioManager.PlaySoundAtRandomPitch(coinSpendSFX, volume: 0.1f);

        if (brightPickUpSFX)
            switch (upgrade.rarity)
            {
                case UpgradeRarity.Normal:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.4f, pitch: 0.9f);
                    break;
                case UpgradeRarity.Rare:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.5f, pitch: 1f);
                    break;
                case UpgradeRarity.Epic:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.6f, pitch: 1.1f);
                    break;
                case UpgradeRarity.Legendary:
                    audioManager.PlaySound(brightPickUpSFX, volume: 0.8f, pitch: 1.2f);
                    break;
                case UpgradeRarity.SuperNova:
                    audioManager.PlaySound(brightPickUpSFX, volume: 1.2f, pitch: 1.3f);
                    break;
            }

        upgrade.ApplyUpgrade();
        priceButton.SetActive(false);
        StartCoroutine(ChosenItemEffect());
    }

    public IEnumerator NotEnoughCoinsEffect()
    {
        // NEW: force opacity to 100 immediately
        cantBuyEffect.DOKill();
        cantBuyEffect.color = new Color(
            cantBuyEffect.color.r,
            cantBuyEffect.color.g,
            cantBuyEffect.color.b,
            1f
        );

        // NEW: fade back to 0 slowly
        cantBuyEffect.DOFade(0f, 0.8f).SetUpdate(true);

        // EXISTING CODE (unchanged)
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(priceButton.transform
            .DOShakePosition(0.5f, strength: 10f, vibrato: 20, randomness: 90, fadeOut: true)
            .SetEase(Ease.OutQuad));
        seq.Play();

        yield return seq.WaitForCompletion();
        notEnoughCoinsCoroutine = null;
    }


    public IEnumerator ChosenItemEffect()
    {
        originalScale = transform.localScale;

        // --- Disable hover visuals to avoid overlap ---
        if (hoverParticles) hoverParticles.gameObject.SetActive(false);
        if (currentBorderEffect) currentBorderEffect.enabled = false;
        if (hoverBorderEffect) hoverBorderEffect.enabled = false;

        // --- Play initial "selection" burst ---
        if (selectParticles) selectParticles.Play();
        if (selectExtraParticles) selectExtraParticles.Play();
        //if (audioSource && selectSfx) audioSource.PlayOneShot(selectSfx, 0.9f);

        // --- Create juicy animation sequence ---
        Sequence seq = DOTween.Sequence().SetUpdate(true);

        // 1. Anticipation squish (ease in before expanding)
        seq.Append(transform
            .DOScale(originalScale * 0.9f, 0.08f)
            .SetEase(Ease.OutQuad));

        // 2. Satisfying pop + subtle shake + flash
        seq.Append(transform
            .DOScale(originalScale * 1.25f, 0.18f)
            .SetEase(Ease.OutBack, 1.8f));
        seq.Join(transform
            .DOShakePosition(0.25f, strength: 0.1f, vibrato: 12, randomness: 45, fadeOut: true));
        seq.Join(transform
            .DOShakeRotation(0.3f, strength: new Vector3(0, 0, 7f), vibrato: 10, randomness: 25, fadeOut: true));

        // 5. Satisfying smooth scale down and fade out
        seq.AppendInterval(0.05f);
        seq.Append(transform
            .DOScale(Vector3.zero, 0.4f)
            .SetEase(Ease.InOutQuad));


        // --- Execute and wait ---
        seq.Play();
        yield return seq.WaitForCompletion();

        // --- Cleanup ---
        gameObject.SetActive(false);
        transform.localScale = originalScale;
    }
    public void PlaySlotMachineEffect(Chest.ChestType chestType, float duration = 2f, float interval = 0.2f, System.Action onComplete = null)
    {
        // Store these so Skip can access them
        currentChestType = chestType;
        currentOnComplete = onComplete;

        if (slotCoroutine != null)
            StopCoroutine(slotCoroutine);

        slotCoroutine = StartCoroutine(SlotMachineRollingCoroutine(chestType, duration, interval, (finalUpgrade) =>
        {
            FinalizeUpgradeReveal(finalUpgrade, onComplete);
        }));
    }

    private IEnumerator SlotMachineRollingCoroutine(Chest.ChestType chestType, float duration, float interval, System.Action<Upgrade> onRollFinished)
    {
        float elapsed = 0f;
        Upgrade lastUpgrade = null;

        Vector3 baseScale = transform.localScale;
        Vector3 basePosition = transform.localPosition;

        while (elapsed < duration)
        {
            lastUpgrade = GetRandomUpgradeInstant(chestType);
            SetUpgrade(lastUpgrade);

            selectExtraParticles?.Play();

            // --- Animation Logic ---
            itemImage.transform.localScale = Vector3.one * 0.7f;
            itemImage.transform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-15f, 15f));

            itemImage.transform.DOScale(0.9f, interval * 0.5f).SetEase(Ease.OutBack).SetUpdate(true)
                .OnComplete(() => {
                    itemImage.transform.DOScale(1f, interval * 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
                    itemImage.transform.DORotate(Vector3.zero, interval * 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
                });

            transform.DOScale(baseScale * 1.05f, interval * 0.5f).SetEase(Ease.OutBack).SetUpdate(true)
                .OnComplete(() => transform.DOScale(baseScale, interval * 0.5f).SetEase(Ease.OutCubic).SetUpdate(true));

            transform.DOLocalMove(basePosition + new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-6f, 6f), 0f), interval * 0.5f)
                .SetEase(Ease.OutQuad).SetUpdate(true)
                .OnComplete(() => transform.DOLocalMove(basePosition, interval * 0.5f).SetEase(Ease.OutCubic).SetUpdate(true));
            // -----------------------

            elapsed += interval;
            yield return new WaitForSecondsRealtime(interval);
        }

        onRollFinished?.Invoke(lastUpgrade);
    }
    private void FinalizeUpgradeReveal(Upgrade finalUpgrade, System.Action onComplete)
    {
        // 1. Kill any running tweens on these objects to prevent "scale stacking"
        itemImage.transform.DOKill();
        transform.DOKill();

        // 2. Set the final data
        SetUpgrade(finalUpgrade);

        // 3. Reset to a clean starting state (0.8f is what your original code used for the start of the punch)
        itemImage.transform.localScale = Vector3.one * 0.8f;
        itemImage.transform.localRotation = Quaternion.identity;

        // 4. Final punch animation for emphasis
        itemImage.transform.DOPunchScale(Vector3.one * 0.3f, 0.6f, 2, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        itemImage.transform.DORotate(Vector3.zero, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        // 5. Final punch on container (using Vector3.one or your specific baseScale)
        // If this script is on the container, use Vector3.one or a cached 'baseScale' variable
        transform.DOPunchScale(Vector3.one * 0.15f, 0.4f, 2, 0.6f)
            .SetUpdate(true);
        
        transform.DOPunchPosition(Vector3.up * 20f, 0.4f, 2, 0.6f)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void SkipSlotMachineEffect()
    {
        // 1. If no coroutine is running, there's nothing to skip
        if (slotCoroutine == null) return;

        // 2. Stop the rolling sequence immediately
        StopCoroutine(slotCoroutine);
        slotCoroutine = null;

        // 3. Kill all DOTweens on the UI elements to prevent "jumping"
        itemImage.transform.DOKill();
        transform.DOKill();

        // 4. Get a final result immediately and jump to the reveal
        // We use the stored currentChestType
        Upgrade skippedUpgrade = GetRandomUpgradeInstant(currentChestType);
        FinalizeUpgradeReveal(skippedUpgrade, currentOnComplete);
    }

    private UpgradeRarity GetRandomRarity(Chest.ChestType chestType)
    {
        float roll = Random.value;
        float[] table = chestType switch
        {
            Chest.ChestType.Normal => normalChest,
            Chest.ChestType.Rare => rareChest,
            Chest.ChestType.Epic => epicChest,
            Chest.ChestType.Legendary => legendaryChest,
            _ => normalChest
        };

        float cumulative = 0f;
        for (int i = 0; i < table.Length; i++)
        {
            cumulative += table[i];
            if (roll < cumulative)
                return (UpgradeRarity)i;
        }
        return UpgradeRarity.Normal;
    }

    public void ShowSelectButton()
    {
        selectButton.SetActive(true);
    }

}
