using UnityEngine;

[ExecuteAlways]
public class ParallaxUI : MonoBehaviour
{
    public RectTransform uiElement;
    public Transform target; // Usually the main camera
    public float parallaxX = 0.5f;
    public float parallaxY = 0.5f;
    public bool smooth = true;
    [Range(0f, 10f)] public float smoothFactor = 5f;

    private Vector3 previousTargetPosition;
    private Vector2 initialAnchoredPosition;

    void Start()
    {
        if (target == null) target = Camera.main.transform;
        if (uiElement == null) uiElement = GetComponent<RectTransform>();

        previousTargetPosition = target.position;
        initialAnchoredPosition = uiElement.anchoredPosition;
    }

    void LateUpdate()
    {
        Vector3 delta = target.position - previousTargetPosition;
        Vector2 targetPos = uiElement.anchoredPosition + new Vector2(delta.x * parallaxX, delta.y * parallaxY);

        if (smooth)
            uiElement.anchoredPosition = Vector2.Lerp(uiElement.anchoredPosition, targetPos, smoothFactor * Time.deltaTime);
        else
            uiElement.anchoredPosition = targetPos;

        previousTargetPosition = target.position;
    }
}
