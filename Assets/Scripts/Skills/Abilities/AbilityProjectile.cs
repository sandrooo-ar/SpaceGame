using UnityEngine;
using System.Collections;
using System;
public class AbilityProjectile : MonoBehaviour
{

    [SerializeField] private Animator anim;
    [SerializeField] private AudioClip projectileCrashSFX;

    public event Action<GameObject> OnDamageDealt;

    private AudioManager sfxManager;

    private Vector2 direction;

    private Coroutine disableCoroutine;

    // These are set when spawned from the ability
    [HideInInspector] public int abilityDamage;
    [HideInInspector] public float projectileScale = 1f;
    [HideInInspector] public float projectileSpeed = 10f;
    [HideInInspector] public float lifeTime = 1f;
    [HideInInspector] public Material projectileMaterial;

    [Header("EXTRA ABILITY STATS")]

    public int extraAbilityDamage = 0;
    public float extraProjectileSpeed = 0f;
    public float extraProjectileScale = 0f;
    public float extraLifeTime = 0f;

    [Header("SPECIAL BEHAVIOURS")]

    [Header("Pierce")]
    public bool passEnemies = false;

    [Header("Bounce")]
    public bool canBounce;
    public int remainingBounces;

    private void OnEnable()
    {
        disableCoroutine = StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(lifeTime + extraLifeTime);
        StartDisabling(true);
    }

    // This is called to set up the projectile when spawned from the ability
    public void SetAudioManager(AudioManager audioManager) => sfxManager = audioManager;
    public void SetDirection(Vector2 currentDirection) => direction = currentDirection;
    public void SetProjectileScale(float scale)
    {
        projectileScale = scale;
        transform.localScale = Vector3.one * (projectileScale + extraProjectileScale);
    }
    public void SetProjectileMaterial(Material mat)
    {
        if (mat != null)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.material = mat;
            }
        }
    }

    /*
    public void SetPoison(bool apply, int damage, int decay, float tickInterval)
    {
        applyPoison = apply;
        poisonDamage = damage;
        poisonDecay = decay;
        poisonTickInterval = tickInterval;
    }
    public void SetBurn(bool apply, int damage, float tickInterval, float duration)
    {
        applyBurn = apply;
        burnDamage = damage;
        burnTickInterval = tickInterval;
        burnDuration = duration;
    }
    public void EnableBounce(int bounceCount)
    {
        canBounce = true;
        remainingBounces = bounceCount;
    }
    public void EnablePierce(bool value) => passEnemies = value;

    */

    void Update()
    {
        transform.Translate(direction * (projectileSpeed + extraProjectileSpeed) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {

                // PLAYER DAMAGE - The player's damage is based on their current stats on hit
                int playerDamage = (int)(PlayerStats.Instance.Damage * PlayerStats.Instance.DamageMultiplier);
                healthComponent.TakeDamage(playerDamage, false);

                // ABILITY DAMAGE - The ability damage is based on the ability's set damage plus extra damage from upgrades or items
                healthComponent.TakeDamage(abilityDamage + extraAbilityDamage, true);

                OnDamageDealt?.Invoke(collision.gameObject);
            }

            if (passEnemies)
            {
                // Ignore collision with enemy completely
                Physics2D.IgnoreCollision(
                    GetComponent<Collider2D>(),
                    collision.collider
                );
                return;
            }

            if (canBounce && remainingBounces > 0)
            {
                remainingBounces--;

                // Reflect projectile off enemy surface normal
                Vector2 normal = collision.contacts[0].normal;
                direction = Vector2.Reflect(direction, normal);

                // Optionally reduce speed slightly each bounce
                direction *= 0.9f;
            }
            else
            {
                StartDisabling(false);
            }
        }
        else
        {
            StartDisabling(true);
        }
    }

    private void StartDisabling(bool lostCombo)
    {
        StopCoroutine(disableCoroutine);

        direction = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        if (sfxManager != null)
            sfxManager.PlaySoundAtRandomPitch(projectileCrashSFX, 0.2f);

        if (anim != null)
            anim.Play("ProjectileCrash");
    }

    // Executed from animation projectile crash
    private void DisableProjectile()
    {
        gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = true;
        transform.position = Vector3.zero;
        canBounce = false; // reset bounce state
    }

}
