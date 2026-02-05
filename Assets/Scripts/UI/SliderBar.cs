using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderBar : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private Slider _slider;
    private Coroutine smoothCoroutine;

    public float smoothTime = 0.2f;
    public bool backgroundSlider = false;

    [Header("Display Settings")]
    public bool worldSpace = false;
    public Vector2 offset = new Vector2(0, 2f);

    public float currentValue = 0f; // track the logical value

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        if (_slider == null)
        {
            Debug.LogError("SliderBar requires a Slider component.");
        }
    }

    private void Update()
    {
        if (worldSpace && targetTransform != null && _slider != null)
        {
            UpdateSliderPosition();
        }
    }

    private void UpdateSliderPosition()
    {
        Vector3 worldPosition = targetTransform.position + new Vector3(offset.x, offset.y, 0f);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        _slider.transform.position = screenPosition;
    }

    public void SetMaxValue(float maxValue)
    {
        if (_slider != null)
        {
            _slider.maxValue = maxValue;
        }
    }

    public void SetCurrentValueInsta(float value)
    {
        if (smoothCoroutine != null)
        {
            StopCoroutine(smoothCoroutine);
            smoothCoroutine = null;
        }

        currentValue = value;
        if (_slider != null)
        {
            _slider.value = currentValue;
        }
    }

    public void SetCurrentValue(float value)
    {
        if (smoothCoroutine != null)
        {
            StopCoroutine(smoothCoroutine);
        }

        smoothCoroutine = StartCoroutine(SmoothValueChange(value));
    }

    private IEnumerator SmoothValueChange(float targetValue)
    {
        float start = currentValue;
        float elapsed = 0f;

        if (backgroundSlider)
        {
            // delay the "background bar" catchup
            yield return new WaitForSeconds(smoothTime);
            elapsed = 0f;
        }

        while (elapsed < smoothTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / smoothTime;
            // Easing curve: ease-out (slows down near the end)
            t = 1f - Mathf.Pow(1f - t, 3f); // cubic ease-out
                                            // Alternative: t = Mathf.SmoothStep(0f, 1f, t);

            currentValue = Mathf.Lerp(start, targetValue, t);
            _slider.value = currentValue;
            yield return null;
        }

        if (backgroundSlider && currentValue <= 0)
        {
            gameObject.SetActive(false);
        }

        currentValue = targetValue;
        _slider.value = currentValue;
        smoothCoroutine = null;
    }


    public void SetBarColor(Color color)
    {
        if (_slider != null && _slider.fillRect != null)
        {
            Image fillImage = _slider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = color;
            }
        }
    }
}
