using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform target;

    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float tweenDuration = 0.2f;

    [Header("Pulsing Scale")]
    [SerializeField] private float pulseStrength = 0f; // Amount to grow/shrink repeatedly
    [SerializeField] private float pulseDuration = 0f;  // Duration of one pulse cycle

    [Header("Floating Movement")]
    [SerializeField] private float hoverFloatStrength = 5f;
    [SerializeField] private float hoverFloatDuration = 1.2f;

    public event Action OnHoverEnter;
    public event Action OnHoverExit;

    private Vector3 originalScale;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        if (target == null) target = GetComponent<RectTransform>();
        originalScale = target.localScale;
        originalLocalPosition = target.localPosition;
    }

    private void OnDisable()
    {
        target.DOKill();
        target.localScale = originalScale;
        target.localPosition = originalLocalPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter?.Invoke();
        target.DOKill();

        if (target.gameObject.GetComponent<Animator>() != null)
        {
            target.gameObject.GetComponent<Animator>().SetTrigger("Hover");
        }

        // Scale up once
        target.DOScale(originalScale * hoverScale, tweenDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Start pulsing scale loop
                target.DOScale((originalScale * hoverScale) + Vector3.one * pulseStrength, pulseDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true);
            })
            .SetUpdate(true);

        // Floating up/down
        target.DOLocalMoveY(originalLocalPosition.y + hoverFloatStrength, hoverFloatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke();
        target.DOKill();

        if (target.gameObject.GetComponent<Animator>() != null)
        {
            target.gameObject.GetComponent<Animator>().SetTrigger("Unhover");
        }

        // Reset scale
        target.DOScale(originalScale, tweenDuration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);

        // Reset position
        target.DOLocalMove(originalLocalPosition, tweenDuration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }
}
