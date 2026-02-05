using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class FloatingMessage : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private TMP_Text _numberValue;

    [SerializeField] private Image icon;

    public bool isCoin;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip coinSFX;

    [Header("Physics Settings")]
    public float InitialYVelocity = 7f;
    public float InitialXVelocityRange = 3f;

    [Header("Animation Settings")]
    public float baseScale = 1f;        // resting scale
    public float popDuration = 0.2f;
    public float settleDuration = 0.1f;
    public float pauseDuration = 0.5f;  // ⏸ how long to stay static
    public float lifetime = 1.0f;
    public float fadeDuration = 0.5f;

    [Header("Number → Scale Mapping")]
    public float minNum = 5f;
    public float maxNum = 100f;
    public float minScale = 1.05f;  // scale for very small hits
    public float maxScale = 2.0f;   // scale for very big hits

    [Header("Behavior Settings")]
    public bool holdBeforePhysics = true; // 🔑 whether to pause before moving

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _numberValue = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        // Reset for pooling
        transform.localScale = Vector3.zero;
        Color c = _numberValue.color;
        c.a = 1f;
        _numberValue.color = c;

        if (icon != null)
        {
            Color iconColor = icon.color;
            iconColor.a = 1f;
            icon.color = iconColor;
        }

        transform.DOKill();
        _numberValue.DOKill();

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector2.zero;

            if (holdBeforePhysics)
                _rigidbody.isKinematic = true; // stop physics until we release it
            else
                ApplyInitialForce(); // normal behavior
        }

    }

    public void ShowNumber(float number, bool isPositive, string extra = "")
    {

        if (isPositive)
            _numberValue.SetText("+" + Mathf.RoundToInt(number).ToString() + extra);
        else 
            _numberValue.SetText(Mathf.RoundToInt(number).ToString() + extra);

        float normalized = Mathf.InverseLerp(minNum, maxNum, number);
        float targetPopScale = Mathf.Lerp(minScale, maxScale, normalized);

        if (isCoin && audioManager != null && coinSFX != null)
        {
            audioManager.PlaySound(coinSFX, 0.8f);
        }

        // Build a tween sequence
        Sequence seq = DOTween.Sequence();

        // 1. Pop up
        transform.localScale = Vector3.zero;
        seq.Append(transform.DOScale(targetPopScale, popDuration).SetEase(Ease.OutBack));

        // 2. Settle to base scale
        seq.Append(transform.DOScale(baseScale, settleDuration).SetEase(Ease.InOutSine));

        // 3. Pause (if enabled) and then release physics
        if (holdBeforePhysics && _rigidbody != null)
        {
            seq.AppendInterval(pauseDuration);
            seq.AppendCallback(() =>
            {
                _rigidbody.isKinematic = false;
                ApplyInitialForce();
            });
        }

        // Fade out after lifetime
        _numberValue.DOFade(0f, fadeDuration)
            .SetDelay(lifetime)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        if (icon != null)
        {
            icon.DOFade(0f, fadeDuration)
                .SetDelay(lifetime);
        }

        
    }

    private void ApplyInitialForce()
    {
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = new Vector2(
                Random.Range(-InitialXVelocityRange, InitialXVelocityRange),
                InitialYVelocity
            );
        }
    }
}
