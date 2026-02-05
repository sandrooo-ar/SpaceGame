using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    protected bool isSpawned = false;
    protected bool isDead = false;

    [SerializeField] protected string dropName;
    [SerializeField] protected int healthDropChance = 10;

    [SerializeField] protected AudioManager audioManager;
    [SerializeField] protected AudioClip hurtSFX;
    [SerializeField] protected AudioClip deathSFX;

    [SerializeField] protected Vector3 targetScale = Vector3.one;

    public int damage = 10;
    public int expAward = 1;

    public int minCoinAward = 10;
    public int maxCoinAward = 15;

    public float spawnTime;

    private HealthComponent healthComponent;

    [SerializeField] private GameObject healthSlider;
    [SerializeField] private GameObject backgroundSlider;

    protected virtual void Start()
    {
        //transform.localScale = Vector3.zero;
        StartCoroutine(Spawn());

        isDead = false;

        healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDeath += Death;
        healthComponent.OnDamageTaken += Damaged;
    }

    private void Damaged()
    {
        if (audioManager != null)
        {
            audioManager.PlaySoundAtRandomPitch(hurtSFX, 1f, 0.95f, 1.1f);
        }
    }

    private IEnumerator Spawn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < spawnTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / spawnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Asegura la escala final
        isSpawned = true;

        
        if (healthSlider != null)
        {
            healthSlider.transform.localScale = Vector3.one;
        }

        if (backgroundSlider != null)
        {
            backgroundSlider.transform.localScale = Vector3.one;
        }

    }

    public void InstantDeath()
    {
        isDead = true;
        Destroy(gameObject);
    }

    protected virtual void Death(bool diedFromExternalDamage)
    { 

        if (isDead) return; 

        isDead = true;

        if (diedFromExternalDamage)
        {

            int healthRoll = Random.Range(0, 100);
            int dropAmount = Random.Range(minCoinAward, maxCoinAward + 1);

            if (healthRoll < healthDropChance)
            {
                GameObject healthDrop = ObjectPooler.Instance.SpawnFromPool(
                    "HealthDrop",
                    transform.position,
                    Quaternion.identity
                );
                healthDrop.GetComponent<BaseDrop>().isHealth = true;
                healthDrop.GetComponent<BaseDrop>().dropAmount = dropAmount;
            }
            else
            {
                GameObject drop = ObjectPooler.Instance.SpawnFromPool(
                    dropName,
                    transform.position,
                    Quaternion.identity
                );
                RunStats.Instance.coinsSpawned += dropAmount;
                drop.GetComponent<BaseDrop>().dropAmount = dropAmount;
                drop.GetComponent<BaseDrop>().isCoin = true;
            }

            RunStats.Instance.enemiesKilled++;

            //drop.GetComponent<BaseDrop>().startPos = transform.position;
            LevelManager.Instance.AddExperience(expAward);

            WaveSpawner.Instance.OnEnemyDefeated();

        }

        if (audioManager != null && deathSFX != null)
        {
            audioManager.PlaySoundAtRandomPitch(deathSFX, 0.5f, 0.95f, 1.1f);
        }

        Destroy(gameObject, 1f);
    }
}