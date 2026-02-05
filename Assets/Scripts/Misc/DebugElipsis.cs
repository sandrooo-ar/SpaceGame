using UnityEngine;

[ExecuteAlways]
public class DebugEllipseDrawer : MonoBehaviour
{
    [Header("Ellipse Settings")]
    public float baseRadius = 2f;       // main radius
    [Range(0f, 90f)]
    public float tiltAngle = 60f;       // tilt in degrees
    public Vector3 offset = Vector3.zero;
    public int segments = 32;
    public Color gizmoColor = Color.cyan;

    private void OnDrawGizmos()
    {
        if (segments < 4) segments = 4;

        // Calculate ellipse radii
        float radiusX = baseRadius;
        float radiusY = baseRadius * Mathf.Cos(tiltAngle * Mathf.Deg2Rad);

        Gizmos.color = gizmoColor;

        Vector3 center = transform.position + offset;
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(radiusX, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            float x = Mathf.Cos(angle) * radiusX;
            float y = Mathf.Sin(angle) * radiusY;

            Vector3 nextPoint = center + new Vector3(x, y, 0f);

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
