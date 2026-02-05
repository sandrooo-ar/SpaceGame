using UnityEngine;

public class DashingEnemy : BaseEnemy
{

    [SerializeField] private AudioClip jumpSFX;

    public float dashForce = 500f; // fuerza del dash
    private Transform player;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start(); // mantiene el Spawn (escalado)
        if(GameObject.FindGameObjectWithTag("Player") != null) player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody attached to DashingEnemy!");
        }
    }

    // Dash instantáneo hacia el jugador
    public void DashTowardsPlayer()
    {
        if (player == null || rb == null) return;

        Vector3 dashDirection = (player.position - transform.position).normalized;
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

        if (audioManager != null && jumpSFX != null)
        {
            audioManager.PlaySoundAtRandomPitch(jumpSFX, 0.3f);
        }
    }

    public void StopDash()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Si el enemigo está colisionando con el jugador, infligir daño
            HealthComponent playerHealth = collision.gameObject.GetComponent<HealthComponent>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
