using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAroundPlayer : MonoBehaviour
{
    public Transform player;
    public float distanceFromPlayer = 2f;
    public float orbitSmoothness = 5f; // Higher = faster response

    private Camera mainCamera;
    private float currentAngle; // Our smoothed angle

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Get mouse position in world space
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f)
        );

        // Calculate target angle from player to mouse
        Vector2 direction = (mouseWorldPos - player.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly interpolate the angle
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, orbitSmoothness * Time.deltaTime);

        // Convert the angle back to a position around the player
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * distanceFromPlayer;
        transform.position = (Vector2)player.position + offset;

        // Keep sprite from rotating
        transform.rotation = Quaternion.identity;
    }

    public Vector2 GetCurrentDirection()
    {
        float radians = currentAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}
