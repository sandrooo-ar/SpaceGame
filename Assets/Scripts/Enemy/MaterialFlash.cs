using System.Collections;
using UnityEngine;

public class MaterialFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.25f;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashCoroutine;
    private Color originalBaseColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;

            // Store the initial base color (make sure your shader uses "_NormalColor")
            if (spriteRenderer.material.HasProperty("_NormalColor"))
                originalBaseColor = spriteRenderer.material.GetColor("_NormalColor");
            else
                originalBaseColor = Color.white;
        }
    }

    /// <summary>
    /// Triggers a temporary flash effect.
    /// </summary>
    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if (gameObject.activeSelf)
            flashCoroutine = StartCoroutine(FlashRoutine());
    }

    /// <summary>
    /// Gradually changes the material's base color to the specified one.
    /// </summary>
    public void ChangeBaseColor(Color targetColor, float duration = 0.5f)
    {
        StartCoroutine(ChangeBaseColorCoroutine(targetColor, duration));
    }

    /// <summary>
    /// Restores the original base color over time.
    /// </summary>
    public void ResetBaseColor(float duration = 0.5f)
    {
        ChangeBaseColor(originalBaseColor, duration);
    }

    private IEnumerator FlashRoutine()
    {
        SetFlashColor();

        spriteRenderer.material.SetFloat("_FlashAmount", 1f);

        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            float currentFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / flashDuration);
            spriteRenderer.material.SetFloat("_FlashAmount", currentFlashAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
    }

    private void SetFlashColor()
    {
        originalMaterial.SetColor("_FlashColor", flashColor);
    }

    private IEnumerator ChangeBaseColorCoroutine(Color targetColor, float duration)
    {
        if (spriteRenderer == null || spriteRenderer.material == null)
            yield break;

        Color startColor = spriteRenderer.material.GetColor("_NormalColor");
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Color newColor = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            spriteRenderer.material.SetColor("_NormalColor", newColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.material.SetColor("_NormalColor", targetColor);
    }
}
