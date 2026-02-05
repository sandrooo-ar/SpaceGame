using DG.Tweening;
using UnityEngine;
using static Chest;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class UpgradeSystem : MonoBehaviour
{

    public static UpgradeSystem Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private TextMeshProUGUI discountText;

    [SerializeField] private GameObject upgradePannel;
    [SerializeField] private GameObject chestPannel;

    [SerializeField] private ParticleSystem normalStarParticles;
    [SerializeField] private ParticleSystem rareStarParticles;
    [SerializeField] private ParticleSystem epicStarParticles;
    [SerializeField] private ParticleSystem legendaryStarParticles;
    [SerializeField] private ParticleSystem superNovaStarParticles;

    public GameObject currentStarParticles;

    [SerializeField] private UpgradeSlot slotOne;
    [SerializeField] private UpgradeSlot slotTwo;
    [SerializeField] private UpgradeSlot slotThree;
    [SerializeField] private UpgradeSlot slotFour;
    [SerializeField] private UpgradeSlot slotFive;

    [SerializeField] private GameObject skipSlotButton;

    [SerializeField] private Chest normalChest;
    [SerializeField] private Chest rareChest;
    [SerializeField] private Chest epicChest;
    [SerializeField] private Chest legendaryChest;

    private int currentUpgrades = 0;

    private int currentPhase = 0; // 0: None, 1: Slot1, 2: Slots 2&3, 3: Slots 4&5
    private bool done1, done2, done3, done4, done5;

    public TextMeshProUGUI coins;

    public Canvas playerUI;

    public WaveTier[] waveTiers;

    [Header("Audio")]
    public AudioClip chestOpenSFX;
    public AudioManager AudioManager;

    public void ShowUpgrades(
    ChestType chestType,
    Upgrade upgradeOne,
    Upgrade upgradeTwo = null,
    Upgrade upgradeThree = null,
    Upgrade upgradeFour = null,
    Upgrade upgradeFive = null)
    {

        AudioManager.PlaySoundAtRandomPitch(chestOpenSFX, 1f, 0.9f, 1.1f);

        // --- ACTIVATE PANEL ---
        upgradePannel.SetActive(true);
        upgradePannel.transform.localScale = Vector3.zero;

        // --- PARTICLES ---
        /*
        switch (chestType)
        {
            case ChestType.Normal:
                currentStarParticles = normalStarParticles;
                break;
            case ChestType.Rare:
                currentStarParticles = rareStarParticles;
                break;
            case ChestType.Epic:
                currentStarParticles = epicStarParticles;
                break;
            case ChestType.Legendary:
                currentStarParticles = legendaryStarParticles;
                break;
        }
        */

        skipSlotButton.SetActive(true);

        // Ensure CanvasGroup exists for fade animation
        CanvasGroup cg = upgradePannel.GetComponent<CanvasGroup>();
        if (cg == null) cg = upgradePannel.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        if (upgradeTwo == null) currentUpgrades = 1;
        else if (upgradeThree == null) currentUpgrades = 2;
        else if (upgradeFour == null) currentUpgrades = 3;
        else if (upgradeFive == null) currentUpgrades = 4;
        else currentUpgrades = 5;

        Debug.Log(currentUpgrades + " upgrades to pick.");

        // --- ANIMATED SHOW ---
        Sequence seq = DOTween.Sequence()
            .SetUpdate(true) // ignore timeScale
            .Append(upgradePannel.transform.DOScale(1.1f, 0.4f).SetEase(Ease.OutBack))
            .Join(cg.DOFade(1f, 0.22f))
            .Append(upgradePannel.transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad));


        // --- ASSIGN UPGRADES (slots hidden initially) ---
        slotOne.SetUpgrade(upgradeOne);
        slotTwo.SetUpgrade(upgradeTwo);
        slotThree.SetUpgrade(upgradeThree);
        slotFour.SetUpgrade(upgradeFour);
        slotFive.SetUpgrade(upgradeFive);

        slotOne.upgradeSystem = this;
        slotTwo.upgradeSystem = this;
        slotThree.upgradeSystem = this;
        slotFour.upgradeSystem = this;
        slotFive.upgradeSystem = this;

        // Hide slots until slot-machine animation begins
        slotOne.gameObject.SetActive(false);
        slotTwo.gameObject.SetActive(false);
        slotThree.gameObject.SetActive(false);
        slotFour.gameObject.SetActive(false);
        slotFive.gameObject.SetActive(false);

        // After panel appears, start your slot reveal animation
        PlaySlotMachineSequential(chestType);
    }

    private void ResetSlots()
    {
        slotOne.ResetSlot();
        slotTwo.ResetSlot();
        slotThree.ResetSlot();
        slotFour.ResetSlot();
        slotFive.ResetSlot();
    }

    public void PickedUpgrade()
    {
        currentUpgrades--;
        if (currentUpgrades <= 0)
        {
            HideUpgradesAndResume();
        }
    }

    public void ApplyDiscountToChestsAndItems(int enemiesDefeatedInWave, int enemiesInWave)
    {
        if (enemiesInWave <= 0) return;

        float completionPercent = ((float)enemiesDefeatedInWave / enemiesInWave) * 100f;
        WaveTier currentTier = null;

        // Find which tier the completion percentage fits
        foreach (var tier in waveTiers)
        {
            /*
            if (completionPercent >= tier.minRange && completionPercent <= tier.maxRange)
            {
                currentTier = tier;
                break;
            }*/

            if (GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>().playerHitTimes <= tier.playerHitTimes)
            {
                currentTier = tier;
                break;
            }
        }

        if (currentTier == null)
        {
            discountText.text = "WAVE INCOMPLETE";
            return;
        }

        // Apply random discount per chest within tier range
        normalChest.ApplyDiscount(RandomDiscount(currentTier));
        rareChest.ApplyDiscount(RandomDiscount(currentTier));
        epicChest.ApplyDiscount(RandomDiscount(currentTier));
        legendaryChest.ApplyDiscount(RandomDiscount(currentTier));

        foreach (var slot in chestPannel.GetComponent<ChestPannel>().items)
        {
            slot.GetComponent<UpgradeSlot>()?.ApplyDiscount(RandomDiscount(currentTier) * 100);
        }

        discountText.text = $"{currentTier.name}!";
    }

    // Helper function to get a random discount in the wave tier range
    private float RandomDiscount(WaveTier tier)
    {
        int discount = Random.Range(tier.minDiscount, tier.maxDiscount + 1);
        return discount / 100f; // convert to 0-1 float for ApplyDiscount
    }


    public void PlaySlotMachineSequential(Chest.ChestType chestType)
    {
        StartCoroutine(SlotSequenceCoroutine(chestType));
    }

    private IEnumerator SlotSequenceCoroutine(Chest.ChestType chestType)
    {
        // --- Center slot first (Phase 1) ---
        currentPhase = 1;
        if (slotOne.upgrade != null)
        {
            slotOne.gameObject.SetActive(true);
            done1 = false;
            slotOne.PlaySlotMachineEffect(chestType, onComplete: () => done1 = true);
            yield return new WaitUntil(() => done1);
        }
        PlayStarParticles(slotOne);

        // --- Lateral slots (Phase 2) ---
        currentPhase = 2;
        if (slotTwo.upgrade != null || slotThree.upgrade != null)
        {
            done2 = true; done3 = true; // Default to true if slot is null

            if (slotTwo.upgrade != null)
            {
                slotTwo.gameObject.SetActive(true);
                done2 = false;
                slotTwo.PlaySlotMachineEffect(chestType, onComplete: () => done2 = true);
            }
            if (slotThree.upgrade != null)
            {
                slotThree.gameObject.SetActive(true);
                done3 = false;
                slotThree.PlaySlotMachineEffect(chestType, onComplete: () => done3 = true);
            }
            yield return new WaitUntil(() => done2 && done3);
        }

        if (slotTwo.upgrade != null)
        {
            PlayStarParticles(slotTwo);
            PlayStarParticles(slotThree);
        }

        // --- Outer slots (Phase 3) ---
        currentPhase = 3;
        if (slotFour.upgrade != null || slotFive.upgrade != null)
        {
            done4 = true; done5 = true;

            if (slotFour.upgrade != null)
            {
                slotFour.gameObject.SetActive(true);
                done4 = false;
                slotFour.PlaySlotMachineEffect(chestType, onComplete: () => done4 = true);
            }
            if (slotFive.upgrade != null)
            {
                slotFive.gameObject.SetActive(true);
                done5 = false;
                slotFive.PlaySlotMachineEffect(chestType, onComplete: () => done5 = true);
            }
            yield return new WaitUntil(() => done4 && done5);
        }

        if (slotFour.upgrade != null)
        {
            PlayStarParticles(slotFour);
            PlayStarParticles(slotFive);
        }

        currentPhase = 0;
        skipSlotButton.SetActive(false);
        ShowSelectButtons();
    }

    public void SkipSlotMachineEffect()
    {
        switch (currentPhase)
        {
            case 1: // Skip Center
                if (slotOne.upgrade != null && !done1)
                {
                    slotOne.SkipSlotMachineEffect();
                    done1 = true; // Breaks the first yield return
                }
                break;

            case 2: // Skip 2 & 3
                if (slotTwo.upgrade != null && !done2) slotTwo.SkipSlotMachineEffect();
                if (slotThree.upgrade != null && !done3) slotThree.SkipSlotMachineEffect();

                done2 = true; // Breaks the second yield return
                done3 = true;
                break;

            case 3: // Skip 4 & 5
                if (slotFour.upgrade != null && !done4) slotFour.SkipSlotMachineEffect();
                if (slotFive.upgrade != null && !done5) slotFive.SkipSlotMachineEffect();

                done4 = true; // Breaks the third yield return
                done5 = true;
                break;
        }
    }

    public void PlayStarParticles(UpgradeSlot slot)
    {
        switch (slot.upgrade.rarity)
        {
            case UpgradeRarity.Normal:
                slot.currentStarParticles = Instantiate(normalStarParticles, slot.transform.position, Quaternion.identity, currentStarParticles.transform);
                break;
            case UpgradeRarity.Rare:
                slot.currentStarParticles = Instantiate(rareStarParticles, slot.transform.position, Quaternion.identity, currentStarParticles.transform);
                break;
            case UpgradeRarity.Epic:
                slot.currentStarParticles = Instantiate(epicStarParticles, slot.transform.position, Quaternion.identity, currentStarParticles.transform);
                break;
            case UpgradeRarity.Legendary:
                slot.currentStarParticles = Instantiate(legendaryStarParticles, slot.transform.position, Quaternion.identity, currentStarParticles.transform);
                break;
            case UpgradeRarity.SuperNova:
                slot.currentStarParticles = Instantiate(superNovaStarParticles, slot.transform.position, Quaternion.identity, currentStarParticles.transform);
                break;
        }

        slot.currentStarParticles.Play();
    }
    private void ShowSelectButtons()
    {
        slotOne.ShowSelectButton();
        slotTwo.ShowSelectButton();
        slotThree.ShowSelectButton();
        slotFour.ShowSelectButton();
        slotFive.ShowSelectButton();
    }

    private void StopHoverParticles()
    {
        foreach (var slot in chestPannel.GetComponent<ChestPannel>().items)
        {
            slot.GetComponent<UpgradeSlot>()?.StopParticles();
        }
    }

    public void HideUpgradesAndResume()
    {
        Vector3 originalPanelScale = upgradePannel.transform.localScale;
        Vector3 originalChestScale = currentStarParticles.transform.localScale;

        // Ensure CanvasGroup exists for fading
        CanvasGroup cg = upgradePannel.GetComponent<CanvasGroup>();
        if (cg == null) cg = upgradePannel.AddComponent<CanvasGroup>();

        foreach (ParticleSystem ps in currentStarParticles.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
            Destroy(ps.gameObject, 1f);
        }


        // Build animation sequence
        Sequence seq = DOTween.Sequence()
            .SetUpdate(true) // ignore timescale
            .Append(currentStarParticles.transform.DOScale(4f, 0.6f).SetEase(Ease.OutBack))
            .Append(upgradePannel.transform.DOScale(0.85f, 0.2f).SetEase(Ease.OutQuad))
            .Join(cg.DOFade(0f, 0.22f))
            // Animate chest particles scaling to 1
            .OnComplete(() =>
            {
                // Reset UI
                upgradePannel.SetActive(false);
                upgradePannel.transform.localScale = originalPanelScale;
                cg.alpha = 1f;

                // Reset chest particles to their original scale
                currentStarParticles.transform.localScale = originalChestScale;

                ResetSlots();
            });

         
    }


    public void HideChestsAndResume()
    {
        Vector3 originalScale = chestPannel.transform.localScale;

        CanvasGroup cg = chestPannel.GetComponent<CanvasGroup>();
        if (cg == null) cg = chestPannel.AddComponent<CanvasGroup>();

        StopHoverParticles();

        playerUI.enabled = true;
        GameManager.Instance.gamePaused = false;
        Time.timeScale = 1f;

        Sequence seq = DOTween.Sequence()
            .SetUpdate(true)
            .Append(chestPannel.transform.DOScale(0.85f, 0.1f).SetEase(Ease.OutQuad))
            .Join(cg.DOFade(0f, 0.1f))
            .Append(chestPannel.transform.DOScale(0f, 0.1f).SetEase(Ease.InBack))
            .OnComplete(() =>
            {
                discountText.text = "";
                chestPannel.SetActive(false);
                chestPannel.transform.localScale = Vector3.one;
                cg.alpha = 1f;

                ResetSlots();
            });
    }


    public void ShowChests()
    {
        GameManager.Instance.gamePaused = true;
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<Canvas>();
        playerUI.enabled = false;


        discountText.text = "";
        chestPannel.SetActive(true);
        chestPannel.transform.localScale = Vector3.zero;
        chestPannel.transform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // <-- ignore timeScale
        Time.timeScale = 0f; // pause game AFTER tween starts
    }

    public void HideUpgrades()
    {
        discountText.text = "";
        upgradePannel.SetActive(false);
    }

    public void HideChests()
    {
        GameManager.Instance.gamePaused = false;

        chestPannel.transform.DOScale(Vector3.zero, 0.15f)
            .SetEase(Ease.InBack)
            .SetUpdate(true) // <-- ignore timeScale
            .OnComplete(() => chestPannel.SetActive(false));

        playerUI.enabled = true;
        Time.timeScale = 1f; // resume game
    }


}

[System.Serializable]
public class WaveTier
{
    public string name;        // "Perfect Wave", "Great Wave", etc.
    public int minRange;       // minimum completion %
    public int maxRange;       // maximum completion %
    public int playerHitTimes; // max times player got hit in wave for this tier
    public int minDiscount;    // minimum discount %
    public int maxDiscount;    // maximum discount %
}
