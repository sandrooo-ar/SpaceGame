using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OpacityModifier : MonoBehaviour
{
    [SerializeField] private float targetOpacity = 0.5f;       // Desired opacity level 
    [SerializeField] private float transitionDuration = 1f;    // Duration of the transition in seconds
    private Image imageComponent;

    [Tooltip("If true, the image will pulse forever between original and target opacity.")]
    public bool forthAndBack = false;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("OpacityModifier requires an Image component on the same GameObject.");
            return;
        }

        if (forthAndBack)
        {
            StartCoroutine(OpacityForthAndBack());
        }
    }

    public IEnumerator OpacityForthAndBack()
    {
        if (imageComponent == null) yield break;

        float initialOpacity = imageComponent.color.a;

        while (true) 
        {
            // Fade to target
            yield return Fade(initialOpacity, targetOpacity);

            // Fade back to initial
            yield return Fade(targetOpacity, initialOpacity);
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(from, to, elapsedTime / transitionDuration);
            SetImageAlpha(newAlpha);
            yield return null;
        }

        SetImageAlpha(to); // Ensure exact final value
    }

    private void SetImageAlpha(float alpha)
    {
        if (imageComponent != null)
        {
            Color color = imageComponent.color;
            color.a = alpha;
            imageComponent.color = color;
        }
    }
}
