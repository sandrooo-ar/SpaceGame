using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 targetPosition;
    public float minSpeed = 5f;
    public float maxSpeed = 10f;
    public float stopDistance = 0.2f;

    private float speed;
    private bool hasLanded = false;

    private void Start()
    {
        // Pick random speed at start
        speed = Random.Range(minSpeed, maxSpeed);
    }

    private void Update()
    {
        if (hasLanded) return;

        // Move toward target every frame
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Check if we've arrived
        if (Vector2.Distance(transform.position, targetPosition) <= stopDistance)
        {
            hasLanded = true;
            transform.position = targetPosition; // snap to final position
        }
    }
}
