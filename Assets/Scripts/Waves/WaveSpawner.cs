using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{

    public static WaveSpawner Instance;

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();
    public float timeBetweenWaves = 5f;

    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private float roundCountdown = 0f;
    private float gameTimer = 0f;
    private bool waveActive = false;
    
    public int enemiesInWave = 0;
    public int enemiesDefeatedInWave = 0;

    public Chest.ChestType currentChestType = Chest.ChestType.Normal;

    private Transform player;

    [SerializeField] private TMPro.TextMeshProUGUI waveText;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    [SerializeField] private TMPro.TextMeshProUGUI enemiesKilled;

    [SerializeField] private AudioSource waveEndSound;

    private float roundTimeLeft;

    // Tracks how many enemies are left to spawn in current wave
    private List<int> enemiesLeftToSpawn = new List<int>();
    private List<float> nextSpawnTimes = new List<float>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found by WaveSpawner.");

        StartWave(0);
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;

        if (!waveActive)
        {
            // Countdown until next wave
            roundCountdown -= Time.deltaTime;
            roundTimeLeft = Mathf.Max(0f, roundCountdown);

            if (timerText != null)
                timerText.text = FormatTime(roundTimeLeft);

            if (roundCountdown <= 0f && currentWaveIndex < waves.Count)
            {
                StartWave(currentWaveIndex);
            }
            return;
        }

        // Active wave countdown
        waveTimer += Time.deltaTime;
        roundTimeLeft = Mathf.Max(0f, waves[currentWaveIndex].waveDuration - waveTimer);

        if (timerText != null)
            timerText.text = FormatTime(roundTimeLeft);

        Wave wave = waves[currentWaveIndex];

        for (int i = 0; i < wave.enemies.Count; i++)
        {
            var info = wave.enemies[i];

            if (enemiesLeftToSpawn[i] > 0 &&
                waveTimer >= info.spawnStartTime &&
                waveTimer <= info.spawnEndTime)
            {
                if (waveTimer >= nextSpawnTimes[i])
                {
                    Vector2 spawnPos = GetValidSpawnPosition();
                    StartCoroutine(ShowIndicatorAndSpawn(info.enemyPrefab, spawnPos));

                    enemiesLeftToSpawn[i]--;

                    // Use interval instead of rate
                    nextSpawnTimes[i] += info.spawnInterval;
                }
            }
        }


        // End wave when duration is over
        bool allSpawned = enemiesLeftToSpawn.TrueForAll(x => x <= 0);
        if (waveTimer >= wave.waveDuration)
        {
            EndWave();
        }
    }

    public void OnEnemyDefeated()
    {
        enemiesDefeatedInWave++;
        if (enemiesKilled != null)
            enemiesKilled.text = $"{enemiesDefeatedInWave} / {enemiesInWave}";
    }

    private string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time % 60f);
        int centiseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return $"{seconds:00}:{centiseconds:00}";
    }

    private void StartWave(int index)
    {
        if (waveText != null)
            waveText.text = waves[index].waveName;

        currentChestType = waves[index].rewardChest;
        currentWaveIndex = index;
        enemiesInWave = 0;
        enemiesDefeatedInWave = 0;
        waveTimer = 0f;
        waveActive = true;
        nextSpawnTimes.Clear();
        enemiesLeftToSpawn.Clear();

        foreach (var info in waves[index].enemies)
        {
            nextSpawnTimes.Add(info.spawnStartTime);
            enemiesLeftToSpawn.Add(info.enemyCount);

            enemiesInWave += info.enemyCount;
        }

        RunStats.Instance.currentRound = index + 1;

        if (enemiesKilled != null)
            enemiesKilled.text = $"0 / {enemiesInWave}";
    }


    private void EndWave()
    {

        // Show upgrade chests
        if (UpgradeSystem.Instance != null) 
        {
            UpgradeSystem.Instance.ShowChests();

            if (waveEndSound != null)
                waveEndSound.Play();

            KillAllEnemies();
            UpgradeSystem.Instance.ApplyDiscountToChestsAndItems(enemiesDefeatedInWave, enemiesInWave);

        }

        waveActive = false;
        roundCountdown = timeBetweenWaves;
        currentWaveIndex++;
    }
     
    private void KillAllEnemies()
    {
        foreach (BaseEnemy enemy in FindObjectsOfType<BaseEnemy>())
        {
            enemy.InstantDeath();
        }
    }

    private IEnumerator ShowIndicatorAndSpawn(GameObject enemyPrefab, Vector2 spawnPos)
    {
        GameObject indicator = ObjectPooler.Instance.SpawnFromPool(
            "SpawnIndicator",
            spawnPos,
            Quaternion.Euler(0f, 0f, 45f)
        );

        yield return new WaitForSeconds(0.8f);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        RunStats.Instance.enemiesSpawned++;

        if (indicator != null)
            indicator.SetActive(false);
    }

    private Vector2 GetValidSpawnPosition()
    {
        if (player == null)
        {
            return new Vector2(
                Random.Range(waves[currentWaveIndex].spawnAreaMin.x, waves[currentWaveIndex].spawnAreaMax.x),
                Random.Range(waves[currentWaveIndex].spawnAreaMin.y, waves[currentWaveIndex].spawnAreaMax.y)
            );
        }

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float maxRadius = Vector2.Distance(waves[currentWaveIndex].spawnAreaMax, waves[currentWaveIndex].spawnAreaMin) * 0.5f;
        float distance = Random.Range(waves[currentWaveIndex].minDistanceFromPlayer, maxRadius);

        Vector2 spawnOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        Vector2 spawnPos = (Vector2)player.position + spawnOffset;

        spawnPos.x = Mathf.Clamp(spawnPos.x, waves[currentWaveIndex].spawnAreaMin.x, waves[currentWaveIndex].spawnAreaMax.x);
        spawnPos.y = Mathf.Clamp(spawnPos.y, waves[currentWaveIndex].spawnAreaMin.y, waves[currentWaveIndex].spawnAreaMax.y);

        return spawnPos;
    }
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
    public float waveDuration = 10f;
    public Chest.ChestType rewardChest = Chest.ChestType.Normal;

    [Header("Spawn Area")]
    public Vector2 spawnAreaMin = new Vector2(-5, -5);
    public Vector2 spawnAreaMax = new Vector2(5, 5);

    [Header("Player Safe Zone")]
    public float minDistanceFromPlayer = 2f;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;

    [Header("Timing")]
    public float spawnStartTime = 0f;
    public float spawnEndTime = 10f;

    [Tooltip("Seconds between each spawn of this enemy type.")]
    public float spawnInterval = 1f;

    [Header("Calculated Spawn Count")]
    [Tooltip("Automatically calculated based on timing and interval.")]
    public int enemyCount
    {
        get
        {
            if (spawnInterval <= 0f) return 0;
            return Mathf.FloorToInt((spawnEndTime - spawnStartTime) / spawnInterval);
        }
    }
}