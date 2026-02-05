using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    public bool showFPS = true;
    public float updateInterval = 0.2f; // how often to update FPS text

    private TextMeshProUGUI fpsText;
    private float timeSinceLastUpdate;
    private float accumulatedTime;
    private int frameCount;

    private void Start()
    {
        fpsText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!showFPS) return;

        // Accumulate frame count and time
        frameCount++;
        accumulatedTime += Time.unscaledDeltaTime;
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        // Update text at fixed interval
        if (timeSinceLastUpdate >= updateInterval)
        {
            float fps = frameCount / accumulatedTime;
            fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";

            // Reset counters
            timeSinceLastUpdate = 0f;
            accumulatedTime = 0f;
            frameCount = 0;
        }
    }
}
