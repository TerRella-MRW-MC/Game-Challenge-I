using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 4f;
    [SerializeField] private float sprintModifier = 1.5f;
    [SerializeField] private float jumpForce = 50f;

    [Header("Gravity Settings")]
    [SerializeField] private float jumpGravity = -150f;
    [SerializeField] private float walkGravity = -100f;

    [Header("Ground Check")]
    [SerializeField] private float maxSlopeAngle = 65;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float frontCheckDistance = 0.1f;
    [SerializeField] private float rearCheckDistance = 0.1f;
    [SerializeField] private Transform raycastOriginFront;
    [SerializeField] private Transform raycastOriginRear;
    [SerializeField] private LayerMask groundMask;

    [Header("Scene Settings")]
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Using custom gravity
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        RaycastChecks();
        HandleMovement();
        ApplyCustomGravity();

    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool jumpPressed = Input.GetKey(KeyCode.Space);

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        float currentForce = (isSprinting && isGrounded) ? moveForce * sprintModifier : moveForce;

        // Handle jumping
        if (jumpPressed && isGrounded) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset Y velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Handle movement on slopes
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.1f, groundMask)) {
            Vector3 slopeDirection = Vector3.ProjectOnPlane(inputDirection, hit.normal).normalized;
            rb.AddForce(slopeDirection * currentForce, ForceMode.Impulse);
        }
        else {
            rb.AddForce(inputDirection * currentForce, ForceMode.Impulse);
        }
    }

    private void RaycastChecks()
    {
        bool isGroundedFront, isGroundedRear, forwardSlope, rearSlope;

        isGroundedFront = Physics.Raycast(raycastOriginFront.position, Vector3.down, groundCheckDistance, groundMask);
        isGroundedRear = Physics.Raycast(raycastOriginRear.position, Vector3.down, groundCheckDistance, groundMask);
        forwardSlope = Physics.Raycast(raycastOriginFront.position, raycastOriginFront.forward, out RaycastHit frontHit, frontCheckDistance, groundMask);
        rearSlope = Physics.Raycast(raycastOriginRear.position, raycastOriginRear.forward, out RaycastHit rearHit, rearCheckDistance, groundMask);

        isGrounded = isGroundedFront || isGroundedRear;

        if (isGroundedFront && forwardSlope) {
            float frontSlopeAngle = Vector3.Angle(frontHit.normal, Vector3.up);
            Debug.Log($"Front Slope Angle: {frontSlopeAngle}");

            if (frontSlopeAngle < maxSlopeAngle) {

            }
        }

        if (isGroundedRear && rearSlope) {
            float rearSlopeAngle = Vector3.Angle(rearHit.normal, Vector3.up);
            Debug.Log($"Rear Slope Angle: {rearSlopeAngle}");
        }

        Debug.DrawRay(raycastOriginFront.position, Vector3.down * groundCheckDistance, Color.green);
        Debug.DrawRay(raycastOriginRear.position, Vector3.down * groundCheckDistance, Color.green);
        Debug.DrawRay(raycastOriginFront.position, raycastOriginFront.forward * frontCheckDistance, Color.green);
        Debug.DrawRay(raycastOriginRear.position, raycastOriginRear.forward * rearCheckDistance, Color.green);


    }

    private void ApplyCustomGravity()
    {
        Vector3 gravity = new Vector3(0f, jumpGravity, 0f);
        Vector3 groundedGravity = new Vector3(0f, walkGravity, 0f);

        if (isGrounded) {
            rb.AddForce(groundedGravity, ForceMode.Acceleration);
        }
        else {
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    public void ResetToStart()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = startPosition;
    }

}
