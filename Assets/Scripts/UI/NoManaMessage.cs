using UnityEngine;
using TMPro;
using System.Collections;

public class NoManaMessageTMP : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_Text messageText;         // Assign your TMP text in the inspector
    public float fadeDuration = 1.5f;    // How long it takes to fade out
    public string defaultMessage = "No Mana!";

    private Coroutine fadeCoroutine;
    private Color originalColor;

    void Start()
    {
        if (messageText == null)
        {
            Debug.LogError("NoManaMessageTMP: Please assign a TMP_Text component!");
            enabled = false;
            return;
        }

        originalColor = messageText.color;
        messageText.alpha = 0f; // Start hidden
    }

    // Call this when trying to use a skill but have no mana
    public void ShowNoManaMessage()
    {
        // Stop current fade if one is already running
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        // Reset text and alpha
        messageText.text = defaultMessage;
        messageText.alpha = 1f;

        // Start fade out
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            messageText.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        messageText.alpha = 0f;
        fadeCoroutine = null;
    }
}
