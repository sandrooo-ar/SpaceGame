using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class BaseDrop : MonoBehaviour
{
    [SerializeField] private Collider2D playerDetection;
    [SerializeField] private Collider2D playerCollision;
    [SerializeField] private FloatingObject floatingVisual;

    public float lifeTime = 2.0f;
    public int dropAmount = 1;

    public bool isCoin = false;
    public bool isHealth = false;
    public string resourceName = "";

    [Header("Magnet Settings")]
    public float magnetForce = 10f; // Fuerza máxima aplicada
    public float magnetTransitionTime = 0.5f; // Tiempo para cambiar de dirección opuesta a dirección al jugador

    [SerializeField] private Animator anim;

    private Coroutine disableCoroutine;
    private Coroutine blinkingCoroutine;

    private Rigidbody2D rb;
    private Transform targetPlayer;

    private bool isPickedUp = false;
    private bool isMagnetActive = false;

    private Vector2 initialAwayDir;
    private float magnetTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (floatingVisual == null)
            floatingVisual = GetComponentInChildren<FloatingObject>();

        if (playerDetection != null)
            playerDetection.enabled = true;
    }

    private void OnEnable()
    {
        disableCoroutine = StartCoroutine(DisableDrop());
        blinkingCoroutine = StartCoroutine(BlinkOnHalfLife());

        floatingVisual?.ResumeFloating();
        isMagnetActive = false;
        magnetTimer = 0f;
    }

    private IEnumerator DisableDrop()
    {
        yield return new WaitForSeconds(lifeTime);
        ResetDrop();
        disableCoroutine = null;
    }

    private IEnumerator BlinkOnHalfLife()
    {
        yield return new WaitForSeconds(lifeTime / 1.5f);

        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();

        if (sprite != null )
        {
            float blinkInterval = 0.1f;
            float endTime = Time.time + (lifeTime / 1.5f);

            while (Time.time < endTime)
            {
                sprite.enabled = !sprite.enabled;
                yield return new WaitForSeconds(blinkInterval);
            }

            sprite.enabled = true; // aseguramos que quede visible
        }

        blinkingCoroutine = null;
    }

    void Update()
    {
        if (isMagnetActive && targetPlayer != null)
        {
            floatingVisual?.StopFloating();

            magnetTimer += Time.deltaTime;
            float t = Mathf.Clamp01(magnetTimer / magnetTransitionTime);

            // Dirección opuesta al jugador (inicio) y dirección al jugador (final)
            Vector2 toPlayerDir = ((Vector2)targetPlayer.position - (Vector2)transform.position).normalized;
            Vector2 currentDir = Vector2.Lerp(initialAwayDir, toPlayerDir, t);

            // Aplica fuerza en la dirección interpolada
            rb.linearVelocity = currentDir * magnetForce;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPickedUp) return;

        if (other.CompareTag("Player"))
        {
            if (!isMagnetActive)
            {
                targetPlayer = other.transform;
                playerDetection.enabled = false;
                playerCollision.enabled = true;

                if (disableCoroutine != null) StopCoroutine(disableCoroutine);
                if (blinkingCoroutine != null) StopCoroutine(blinkingCoroutine);

                SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
                if (sprite != null)
                    sprite.enabled = true;

                // Calcula la dirección opuesta al jugador
                initialAwayDir = ((Vector2)transform.position - (Vector2)targetPlayer.position).normalized;
                magnetTimer = 0f;
                isMagnetActive = true;
            }
            else
            {
                playerDetection.enabled = true;
                playerCollision.enabled = false;
                PickUp();
            }
        }
    }

    void ResetDrop()
    {
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
            sprite.enabled = true;

        playerCollision.enabled = false;
        playerDetection.enabled = true;
        anim.Play("Idle");
        isMagnetActive = false;
        targetPlayer = null;
        rb.linearVelocity = Vector2.zero;
        magnetTimer = 0f;
    }

    void PickUp()
    {

        if (isCoin && !isPickedUp)
        {
            CoinManager.Instance.AddCoin(dropAmount);
            RunStats.Instance.coinsCollected += dropAmount;
        }
        else if (isHealth && !isPickedUp)
        {
            GameObject player = targetPlayer.gameObject;
            HealthComponent healthManager = player.GetComponent<HealthComponent>();
            if (healthManager != null)
            {
                healthManager.Heal(dropAmount);
            }
        }

        isPickedUp = true;

        rb.linearVelocity = Vector3.zero;
        anim.Play("Idle");
        floatingVisual?.StopFloating();
    }

    public void Deactivate()
    {
        playerCollision.enabled = false;
        playerDetection.enabled = true;

        isMagnetActive = false;
        targetPlayer = null;
        rb.linearVelocity = Vector2.zero;
        magnetTimer = 0f;

        gameObject.SetActive(false);
        isPickedUp = false;
        floatingVisual?.ResumeFloating();

        if (disableCoroutine != null)
            StopCoroutine(disableCoroutine);

        if (blinkingCoroutine != null)
            StopCoroutine(blinkingCoroutine);
    }
}
