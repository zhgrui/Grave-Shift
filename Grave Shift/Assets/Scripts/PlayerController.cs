using UnityEngine;

/// <summary>
/// Handles player collision with walls and gravity-based falling after gravity shifts.
/// Attach to the XR Origin GameObject.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Gravity Fall Settings")]
    [SerializeField] private float gravityMultiplier = 1.5f;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask groundLayer = ~0;

    private CharacterController cc;
    private Vector3 velocity;
    private bool isFalling;
    private bool gravityActive;

    public bool IsFalling => isFalling;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        cc.slopeLimit = 80f;
        cc.stepOffset = 0.3f;
    }

    private void OnEnable()
    {
        if (GravityManipulation.Instance != null)
            GravityManipulation.Instance.OnGravityShiftEnd += HandleGravityShiftEnd;
    }

    private void OnDisable()
    {
        if (GravityManipulation.Instance != null)
            GravityManipulation.Instance.OnGravityShiftEnd -= HandleGravityShiftEnd;
    }

    private void HandleGravityShiftEnd()
    {
        gravityActive = true;
        isFalling = true;
        velocity = Vector3.zero;
    }

    private void Update()
    {
        if (gravityActive)
        {
            // Apply gravity and move player toward new floor
            velocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
            CollisionFlags flags = cc.Move(velocity * Time.deltaTime);

            // Landed when CC reports ground contact or we hit something below
            if (cc.isGrounded || (flags & CollisionFlags.Below) != 0)
            {
                gravityActive = false;
                isFalling = false;
                velocity = Vector3.zero;
            }
        }
        else
        {
            // Keep player stuck to ground when not falling
            if (!cc.isGrounded)
                cc.Move(Vector3.down * 0.05f);
        }
    }
}
