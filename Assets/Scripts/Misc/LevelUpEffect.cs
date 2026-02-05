using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShockwaveDistort : MonoBehaviour
{
    public Material effectMaterial;
    public float duration = 1.2f;
    public float maxRadius = 1.5f;
    public float waveWidth = 0.2f;
    public float waveStrength = 0.05f;
    public float intensity = 1f;

    private Coroutine running;

    void Awake()
    {
        if (effectMaterial == null) return;

        // Set defaults
        effectMaterial.SetFloat("_WaveRadius", 0f);
        effectMaterial.SetFloat("_WaveWidth", waveWidth);
        effectMaterial.SetFloat("_WaveStrength", waveStrength);
        effectMaterial.SetFloat("_Intensity", 0f);
        effectMaterial.SetVector("_WaveCenter", new Vector4(0.5f, 0.5f, 0f, 0f));
    }

    public void Play(Vector2? center = null)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(DoShockwave(center ?? new Vector2(0.5f, 0.5f)));
    }

    IEnumerator DoShockwave(Vector2 center)
    {
        effectMaterial.SetVector("_WaveCenter", new Vector4(center.x, center.y, 0, 0));

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float norm = Mathf.Clamp01(t / duration);

            float radius = Mathf.Lerp(0f, maxRadius, norm);
            effectMaterial.SetFloat("_WaveRadius", radius);
            effectMaterial.SetFloat("_Intensity", (1f - norm) * intensity);

            yield return null;
        }

        effectMaterial.SetFloat("_WaveRadius", 0f);
        effectMaterial.SetFloat("_Intensity", 0f);
        running = null;
    }

    [ContextMenu("Test Play")]
    private void TestPlayButton()
    {
        Play();
    }
}
