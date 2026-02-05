using UnityEngine;
using TMPro;
using System;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    [SerializeField] ShockwaveDistort levelUpEffect;
    [SerializeField] SliderBar expBar;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] ShockWaveEffect shockWaveEffect;
    [SerializeField] private AudioSource levelUpSound;

    // EFFECTS
    [SerializeField] private ParticleSystem expParticles;
    [SerializeField] private MaterialFlash expFlash;


    public bool hasLeveledUp = false;
    public int currentLevel = 0;

    public int currentExp = 0;
    private int expToNextLevel = 83;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        if (currentLevel == 0) LevelUp(false); hasLeveledUp = false;

        expBar.SetMaxValue(expToNextLevel);
        expBar.SetCurrentValue(currentExp);
        levelText.text = System.Convert.ToString(currentLevel);
    }

    public void AddExperience(int amount)
    {

        if (hasLeveledUp) return;

        currentExp += Convert.ToInt32(amount * PlayerStats.Instance.EXPMultiplier);

        if (expParticles != null)
        {
            expParticles.Play();
        }

        if (expFlash != null)
        {
            expFlash.Flash();
        }

        expBar.SetCurrentValue(currentExp);


        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            GameObject expIndicator = ObjectPooler.Instance.SpawnFromPool(
                            "ExpIndicator",
                            player.transform.position,
                            Quaternion.identity
                        );

            if (expIndicator != null)
            {
                expIndicator.GetComponent<FloatingMessage>().ShowNumber(amount * PlayerStats.Instance.EXPMultiplier, true, " EXP");
            }
        }

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

    }

    public void LevelUp(bool playEffects = true)
    {
        currentLevel++;
        RunStats.Instance.playerLevel = currentLevel;
        levelText.text = System.Convert.ToString(currentLevel);
        currentExp = 0;

        // TO CHANGE: OPEN SKILL TREE INSTEAD OF CHEST SELECTION
        //UpgradeSystem.Instance.ShowChests();

        if (playEffects)
        {
            shockWaveEffect.CallShockWave();
            levelUpEffect.Play();
            levelUpSound.Play();
        }
        

        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f); // Increase the required EXP for next level
        expBar.SetMaxValue(expToNextLevel);
        expBar.SetCurrentValueInsta(currentExp);

        hasLeveledUp = true;

        SkillManager.Instance.ShowLevelUp();
    }

    public void NextLevel()
    {        
        hasLeveledUp = false;
    }


}
