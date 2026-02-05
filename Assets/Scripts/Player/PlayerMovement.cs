using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float friction = 10f;

    private Rigidbody2D _rigidbody;
    private bool _isRunning;
    private Vector2 _movementInput;
    private Vector2 _currentVelocity;

    public Vector2 CurrentDirection => _rigidbody.linearVelocity.sqrMagnitude > 0.01f ? _rigidbody.linearVelocity.normalized : Vector2.zero;

    private Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        // Calculate target velocity
        Vector2 targetVelocity = _movementInput * PlayerStats.Instance.MoveSpeed * PlayerStats.Instance.MoveSpeedMultiplier;

        // Smoothly move current velocity toward target
        Vector2 velocityChange;
        if (_movementInput.sqrMagnitude > 0.01f)
        {
            velocityChange = Vector2.MoveTowards(_rigidbody.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _isRunning = true;
        }
        else
        {
            // Apply friction to reduce velocity when no input
            velocityChange = Vector2.MoveTowards(_rigidbody.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
            _isRunning = false;
        }

        _rigidbody.linearVelocity = velocityChange;

        // Update animator
        if (_animator != null)
        {
            _animator.SetBool("isRunning", _isRunning);

            if (_isRunning)
            {
                _animator.SetFloat("X", _movementInput.x);
                _animator.SetFloat("Y", _movementInput.y);
            }
        }
    }
}

