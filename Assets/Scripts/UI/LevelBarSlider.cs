using UnityEngine;
using UnityEngine.UI;

public class LevelBarSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetValue;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    void Start()
    {
        if (slider != null)
        {
            slider.value = 0f;
            targetValue = 0f;
        }
    }

    void Update()
    {
        if (slider != null)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
    }

    public void SetMaxValue(float maxValue)
    {
        if (slider != null)
        {
            slider.maxValue = maxValue;
        }
    }

    public void SetValue(float value)
    {
        if (slider != null)
        {
            value = Mathf.Clamp(value, 0, slider.maxValue);
            slider.value = value;
            targetValue = value;
        }
    }

    public void SetTargetValue(float value)
    {
        if (slider != null)
        {
            targetValue = Mathf.Clamp(value, 0, slider.maxValue);
        }
    }
}
