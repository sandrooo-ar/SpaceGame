using TMPro;
using UnityEngine;


public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI coinText;

    private GameObject player;

    [SerializeField] private int coinCount = 0;

    public int extraCoinGain = 0;

    [Header("Effects")]
    [SerializeField] private ParticleSystem coinCollectEffect;
    [SerializeField] private MaterialFlash coinFlashEffect;


    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persists across scenes
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void AddCoin(int amount = 1)
    {
        amount += PlayerStats.Instance.extraCoins;
        coinCount += amount;
        coinText.text = coinCount.ToString();

        if (coinCollectEffect != null)
        {
            coinCollectEffect.Play();
        }

        if (coinFlashEffect != null)
        {
            coinFlashEffect.Flash();
        }

        if (player != null)
        {
            GameObject coinIndicator = ObjectPooler.Instance.SpawnFromPool(
                            "CoinIndicator",
                            player.transform.position,
                            Quaternion.identity
                        );

            if (coinIndicator != null)
            {
                coinIndicator.GetComponent<FloatingMessage>().ShowNumber(amount, true);
                coinIndicator.transform.SetParent(player.transform);
            }
        }
    }

    public void SpendCoins(int amount)
    {
        coinCount -= amount;
        if (coinCount < 0) coinCount = 0; // Prevent negative coins
        coinText.text = coinCount.ToString();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}
