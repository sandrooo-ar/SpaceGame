using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour
{
    public static ManaManager Instance { get; private set; }

    public float currentMana; // Changed to float for smooth increment
    [SerializeField] private SliderBar manaBar;
    [SerializeField] private TextMeshProUGUI manaText;
    public NoManaMessageTMP noManaMessage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Initialize mana to full
        currentMana = PlayerStats.Instance.maxMana;

        if (manaBar != null)
        {
            manaBar.SetMaxValue(PlayerStats.Instance.maxMana);
            manaBar.SetCurrentValue(currentMana);
        }
    }

    private void Update()
    {
        // Update mana text
        if (manaText != null)
        {
            manaText.text = Mathf.FloorToInt(currentMana) + " / " + Mathf.FloorToInt(PlayerStats.Instance.maxMana);

        }

        if (manaBar != null)
        {
            manaBar.SetMaxValue(PlayerStats.Instance.maxMana);
        }

        HandleManaRegen();
    }

    private void HandleManaRegen()
    {
        float regenRate = PlayerStats.Instance.manaRegenRate; // mana per second

        // Smoothly regenerate mana
        if (currentMana < PlayerStats.Instance.maxMana)
        {
            currentMana += regenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, PlayerStats.Instance.maxMana);

            manaBar.SetCurrentValue(currentMana);
        }
    }

    public bool UseMana(int amount)
    {
        if (currentMana < amount)
            return false;

        currentMana -= amount;
        manaBar.SetCurrentValue(currentMana);
        return true;
    }

    public void RestoreAllMana()
    {
        currentMana = PlayerStats.Instance.maxMana;
        manaBar.SetCurrentValue(currentMana);
    }
}
