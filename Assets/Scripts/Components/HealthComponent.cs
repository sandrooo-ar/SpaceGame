using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] public SliderBar healthBar;
    [SerializeField] public SliderBar secondBar;

    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] Collider2D hitCollider;

    // Damage Effects
    private MaterialFlash damageFlash;
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] ShockwaveDistort playerHitEffect;
    [SerializeField] private ParticleSystem playerHealParticles;

    [Header("Health Settings")]
    public float maxHealth = 100;
    public float currentHealth;
    public float invencibilityTime = 0.5f;
    public float healthRegenRate = 0f;    // HP per second

    [Header("Farming Settings")]
    public bool hasNaturalDecay = false;   // Toggle farming-style decay

    public float decayRate = 1f;           // HP lost per second if natural decay is active
    public float decayDamage = 1f;         // Amount of HP lost each decay tick

    public int playerHitTimes = 0;

    public bool takesDamage = true;
    public bool isPlayer = false;

    public event Action<int> OnHealthChanged;
    public event Action<bool> OnDeath;
    public event Action OnDamageTaken;

    public bool diedFromExternalDamage = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxValue(maxHealth);
            healthBar.SetCurrentValue(currentHealth);
        }

        if (secondBar != null)
        {
            secondBar.SetMaxValue(maxHealth);
            secondBar.SetCurrentValue(currentHealth);
        }

        if (damageFlash == null)
        {
            damageFlash = GetComponent<MaterialFlash>();
        }

        if (healthText != null) healthText.text = currentHealth.ToString("0") + " / " + maxHealth;

        // Start farming decay if enabled
        if (hasNaturalDecay)
        {
            StartCoroutine(NaturalDecayRoutine());
        }

        StartCoroutine(RegenHP());

    }

    public IEnumerator RegenHP()
    {
        float healAccumulator = 0f;

        while (true) // runs permanently
        {
            if (currentHealth < maxHealth)
            {
                healAccumulator += healthRegenRate * Time.deltaTime;

                if (healAccumulator >= 1f)
                {
                    int healAmount = Mathf.FloorToInt(healAccumulator);
                    Heal(healAmount);
                    healAccumulator -= healAmount;
                }
            }
            else
            {
                // reset accumulator to avoid leftover fractional healing
                healAccumulator = 0f;
            }

            yield return null; // every frame
        }
    }

    public void TakeDamage(float damage, bool isAbilityDamage = false, bool isResourceDamage = false, bool isBurnDamage = false, bool isPoisonDamage = false)
    {
        if (!takesDamage) return;

        bool tookCrit = false;

        if (!isPlayer && !isAbilityDamage)
        {
            // Crit chance for enemies
            float critRoll = UnityEngine.Random.Range(0f, 100f);
            if (critRoll <= PlayerStats.Instance.critChance) 
            {
                tookCrit = true;
                damage *= 2f;
            }
        }

        currentHealth -= damage;
        diedFromExternalDamage = true;

        if (healthText != null)
            healthText.text = currentHealth.ToString("0") + " / " + maxHealth;

        if (isPlayer)
        {
            playerHitTimes++;
            CameraShakeManager.Instance.CameraShake(0.25f);
            if (playerHitEffect != null)
                playerHitEffect.Play();
            //ComboManager.Instance.ResetAllCombos();
        }
        else
        {
            CameraShakeManager.Instance.CameraShake(0.05f);
        }

        if (invencibilityTime > 0)
        {
            StartCoroutine(InvincibilityCoroutine());
        }

        if (!isResourceDamage)
        {
            // Determine indicator pool
            string indicatorPool;
            if (isBurnDamage)
            {
                indicatorPool = "BurnDamageIndicator";
            }
            else if (isPoisonDamage)
            {
                indicatorPool = "PoisonDamageIndicator";
            }
            else
            {
                if (tookCrit) 
                    indicatorPool = "CritDamageIndicator";
                else
                    indicatorPool = isAbilityDamage ? "AbilityDamageIndicator" : "DamageIndicator";
            }

            GameObject damageIndicator = ObjectPooler.Instance.SpawnFromPool(indicatorPool, transform.position, Quaternion.identity);

            if (damageIndicator != null)
            {
                damageIndicator.GetComponent<FloatingMessage>().ShowNumber(damage, false);
            }
        }
        else
        {
            GameObject damageIndicator = ObjectPooler.Instance.SpawnFromPool("ResourceDamageIndicator", transform.position, Quaternion.identity);

            if (damageIndicator != null)
            {
                damageIndicator.GetComponent<FloatingMessage>().ShowNumber(damage, false);
            }
        }

        OnDamageTaken?.Invoke();

        if (healthBar != null) healthBar.SetCurrentValue(currentHealth);
        if (secondBar != null) secondBar.SetCurrentValue(currentHealth);

        if (damageParticles != null) damageParticles.Play();
        if (damageFlash != null) damageFlash.Flash();

        if (currentHealth <= 0)
        {
            if (healthText != null) healthText.text = "0/" + maxHealth;
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        if (playerHealParticles != null) playerHealParticles.Play();

        ObjectPooler.Instance.SpawnFromPool("HealIndicator", transform.position, Quaternion.identity)
            .GetComponent<FloatingMessage>().ShowNumber(healAmount, false);

        if (healthText != null)
            healthText.text = currentHealth.ToString("0") + " / " + maxHealth;
        if (healthBar != null) healthBar.SetCurrentValue(currentHealth);
        if (secondBar != null) secondBar.SetCurrentValue(currentHealth);
        OnHealthChanged?.Invoke(healAmount);
    }


    private IEnumerator InvincibilityCoroutine()
    {
        takesDamage = false;

        if (hitCollider != null)
            hitCollider.enabled = false;

        yield return new WaitForSeconds(invencibilityTime);

        if (hitCollider != null)
            hitCollider.enabled = true;

        takesDamage = true;
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        if (healthBar != null)
        {
            healthBar.SetMaxValue(maxHealth);
            healthBar.SetCurrentValue(currentHealth);
        }
        if (secondBar != null)
        {
            secondBar.SetMaxValue(maxHealth);
            secondBar.SetCurrentValue(currentHealth);
        }
        if (healthText != null)
            healthText.text = currentHealth.ToString("0") + " / " + maxHealth;
    }

    private IEnumerator NaturalDecayRoutine()
    {
        while (currentHealth > 0)
        {
            yield return new WaitForSeconds(decayRate);
            currentHealth -= decayDamage;

            if (healthBar != null) healthBar.SetCurrentValue(currentHealth);
            if (secondBar != null) secondBar.SetCurrentValue(currentHealth);

            if (currentHealth <= 0)
            {
                diedFromExternalDamage = false;
                Die();
                yield break;
            }
        }
    }

    public void Die()
    {
        OnDeath?.Invoke(diedFromExternalDamage);

        if (diedFromExternalDamage)
            CameraShakeManager.Instance.CameraShake(0.1f);

        if (deathEffect != null)
        {
            deathEffect.GetComponent<DeathEffect>().PlayDeathEffect();
        }

        if (isPlayer) Destroy(this.gameObject);
        else
            gameObject.SetActive(false);
    }
}
