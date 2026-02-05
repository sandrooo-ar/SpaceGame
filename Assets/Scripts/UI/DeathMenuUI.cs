using DG.Tweening;
using TMPro;
using UnityEngine;

public class DeathMenuUI : MonoBehaviour
{
    
    private HealthComponent playerHealth;

    [SerializeField] private GameObject _canvasHolder;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI round;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI enemiesKilled;
    [SerializeField] private TextMeshProUGUI enemiesSpawned;
    [SerializeField] private TextMeshProUGUI goldCollected;
    [SerializeField] private TextMeshProUGUI goldSpawned;

    private void Start()
    {
        // Show the death menu when the player dies
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
        playerHealth.OnDeath += ShowDeathMenu;
    }
    private void ShowDeathMenu(bool obj)
    {
        // Player death functionality
        if (GameObject.FindGameObjectWithTag("LevelManager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("LevelManager"));
        }

        if (GameObject.FindGameObjectWithTag("CoinManager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("CoinManager"));
        }

        
        if (GameObject.FindGameObjectWithTag("UpgradeSystem") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("UpgradeSystem"));
        }
        


        // Update final stats
        round.text = RunStats.Instance.currentRound.ToString();
        playerLevel.text = RunStats.Instance.playerLevel.ToString();
        enemiesKilled.text = RunStats.Instance.enemiesKilled.ToString();
        enemiesSpawned.text = RunStats.Instance.enemiesSpawned.ToString();
        goldCollected.text = RunStats.Instance.coinsCollected.ToString();
        goldSpawned.text = RunStats.Instance.coinsSpawned.ToString();

        // Animate the death menu appearance
        _canvasHolder.transform.localScale = Vector3.zero;
        _canvasHolder.transform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // <-- ignore timeScale

        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LoadLevel("MainMenu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LoadLevel("Grassland");
    }

}
