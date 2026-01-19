using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float airDashCooldown = 0.8f;

    [Header("Jump Settings")]
    [SerializeField] private float normalJumpForce = 13f;
    [SerializeField] private float dashJumpForceMultiplier = 1.7f;
    [SerializeField] private float dashJumpHorizontalBoost = 8f;

    [Header("Movement Settings")]
    [SerializeField] private float airDrag = 0.95f;
    [SerializeField] private float groundDrag = 0.85f;
    [SerializeField] private float fastFallSpeed = 18f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchScaleY = 0.5f;
    [SerializeField] private float crouchColliderHeight = 0.5f;
    [SerializeField] private float crouchHoldTime = 0.2f;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color dashColor = Color.white;
    [SerializeField] private Color keyColor = Color.yellow;

    // Components (cached in Awake)
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

    // State
    private bool isGrounded;
    private bool isDashing = false;
    private bool isCrouching = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private int movementDirection = 1; // 1 = droite, -1 = gauche
    private bool hasKey = false;

    // Original values
    private float originalScaleY;
    private float originalColliderHeight;
    private Vector2 originalColliderOffset;

    // Input tracking
    private bool spacePressed;
    private bool spaceHeld;
    private float spaceHeldTime = 0f;

    // Dash jump detection
    private bool canDashJump = false;

    void Awake()
    {
        // Cache all components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void Start()
    {
        // Create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform, false);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // Store original values
        originalScaleY = transform.localScale.y;
        originalColliderHeight = boxCollider.size.y;
        originalColliderOffset = boxCollider.offset;

        // Set ground layer if not set
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
        }

        // Set initial color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }

        // Configure trail
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        UpdateTimers();
        HandleCrouch();
        HandleDashInput();
        UpdateVisuals();
    }

    void FixedUpdate()
    {
        HandleDash();
        HandleMovement();
        HandleFastFall();
    }

    void HandleInput()
    {
        spacePressed = Input.GetKeyDown(KeyCode.Space);
        spaceHeld = Input.GetKey(KeyCode.Space);

        if (spaceHeld)
        {
            spaceHeldTime += Time.deltaTime;
        }
        else
        {
            spaceHeldTime = 0f;
        }
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            dashCooldownTimer = 0f;
        }
    }

    void UpdateTimers()
    {
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                EndDash();
            }
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void HandleDashInput()
    {
        if (spaceHeldTime >= crouchHoldTime && isGrounded && !isDashing)
        {
            return;
        }

        if (spacePressed && !isCrouching)
        {
            if (isDashing && isGrounded && canDashJump)
            {
                DashJump();
            }
            else if (CanDash())
            {
                StartDash();
            }
        }
    }

    bool CanDash()
    {
        return dashCooldownTimer <= 0f;
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        canDashJump = isGrounded;

        dashCooldownTimer = isGrounded ? dashCooldown : airDashCooldown;

        float currentYVelocity = rb.linearVelocity.y;
        rb.linearVelocity = new Vector2(dashSpeed * movementDirection, currentYVelocity);

        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
        }
    }

    void HandleDash()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashSpeed * movementDirection, rb.linearVelocity.y);
        }
    }

    void EndDash()
    {
        isDashing = false;
        canDashJump = false;

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
    }

    void DashJump()
    {
        float jumpForce = normalJumpForce * dashJumpForceMultiplier;
        float horizontalBoost = dashJumpHorizontalBoost * movementDirection;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x + horizontalBoost, jumpForce);

        EndDash();
    }

    void HandleMovement()
    {
        if (!isDashing && !isCrouching)
        {
            float drag = isGrounded ? groundDrag : airDrag;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * drag, rb.linearVelocity.y);
        }
    }

    void HandleFastFall()
    {
        if (spaceHeld && !isGrounded && rb.linearVelocity.y < 0 && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
        }
    }

    void HandleCrouch()
    {
        if (spaceHeldTime >= crouchHoldTime && isGrounded && !isDashing)
        {
            if (!isCrouching)
            {
                Crouch();
            }
        }
        else
        {
            if (isCrouching)
            {
                StandUp();
            }
        }
    }

    void Crouch()
    {
        isCrouching = true;

        Vector3 scale = transform.localScale;
        scale.y = originalScaleY * crouchScaleY;
        transform.localScale = scale;

        Vector2 size = boxCollider.size;
        size.y = crouchColliderHeight;
        boxCollider.size = size;

        Vector2 offset = boxCollider.offset;
        offset.y = -(originalColliderHeight - crouchColliderHeight) / 2f;
        boxCollider.offset = offset;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void StandUp()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            new Vector2(boxCollider.size.x, originalColliderHeight),
            0f,
            Vector2.up,
            0.1f,
            groundLayer
        );

        if (hit.collider == null)
        {
            isCrouching = false;

            Vector3 scale = transform.localScale;
            scale.y = originalScaleY;
            transform.localScale = scale;

            boxCollider.size = new Vector2(boxCollider.size.x, originalColliderHeight);
            boxCollider.offset = originalColliderOffset;
        }
    }

    void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        if (hasKey)
        {
            spriteRenderer.color = keyColor;
        }
        else if (isDashing)
        {
            spriteRenderer.color = dashColor;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key") && !hasKey)
        {
            hasKey = true;
            movementDirection = -1;
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Trap"))
        {
            Die();
        }
    }

    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // Public getters
    public bool HasKey() => hasKey;
    public int GetDirection() => movementDirection;
    public bool IsDashing() => isDashing;
    public float GetDashCooldown() => dashCooldownTimer;
}