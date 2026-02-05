using UnityEngine;

public class StarRandomizer : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer squareRenderer;

    [Header("Size Settings")]
    public Vector2 scaleRange = new Vector2(0.1f, 0.5f);

    void Start()
    {
        // Random scale
        float scale = Random.Range(scaleRange.x, scaleRange.y);
        transform.localScale = Vector3.one * scale;

        // Force color to white
        if (squareRenderer != null)
        {
            squareRenderer.color = Color.white;
        }
    }
}
