using UnityEngine;
using DG.Tweening;

public class PopUpFromZero : MonoBehaviour
{
    [SerializeField] private float popScale = 1.2f;   // overshoot size
    [SerializeField] private float popTime = 0.2f;    // grow duration
    [SerializeField] private float settleTime = 0.15f; // settle duration

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;

        transform.DOKill();

        Sequence popSequence = DOTween.Sequence();
        popSequence.Append(transform.DOScale(popScale, popTime).SetEase(Ease.OutBack));
        popSequence.Append(transform.DOScale(1f, settleTime).SetEase(Ease.OutQuad));
    }
}
