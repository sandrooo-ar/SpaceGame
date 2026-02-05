using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 0.5f;  // Height variation
    public float floatFrequency = 1f;    // Speed of floating

    private Vector3 startLocalPos;
    private bool isFloating = true;

    [Header("Effect Settings")]
    [SerializeField] private string floorTouchEffectTag; // Tag for ObjectPooler
    [SerializeField] private Vector3 effectOffset = new Vector3(0, -0.1f, 0);

    [SerializeField] private BaseDrop parentDrop; 

    private bool reachedBottom = false;

    void OnEnable()
    {
        startLocalPos = transform.localPosition;
        isFloating = true;
        reachedBottom = false;
    }

    void Update()
    {
        if (!isFloating) return;

        float sinValue = Mathf.Sin(Time.time * floatFrequency);
        float newY = startLocalPos.y + sinValue * floatAmplitude;
        transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);

        // Detect bottom of sine wave
        if (sinValue <= -0.7f && !reachedBottom)
        {
            reachedBottom = true;
            PlayFloorTouchEffect();
        }
        else if (sinValue > -0.5f)
        {
            // Reset when moving upward, so effect can trigger again
            reachedBottom = false;
        }
    }

    private void PlayFloorTouchEffect()
    {
        if (!string.IsNullOrEmpty(floorTouchEffectTag) && ObjectPooler.Instance != null)
        {
            Vector3 effectPos = transform.position + effectOffset;
            GameObject floorTouch = ObjectPooler.Instance.SpawnFromPool(floorTouchEffectTag, effectPos, Quaternion.identity);
            floorTouch.transform.SetParent(null);
            floorTouch.GetComponent<Animator>().Play("FloorTouch");
        }
    }

    public void StopFloating() => isFloating = false;

    public void ResumeFloating()
    {
        startLocalPos = transform.localPosition;
        isFloating = true;
    }

    public void Deactivate()
    {
        if (parentDrop != null)
        {
            parentDrop.Deactivate();
        }
    }
}
