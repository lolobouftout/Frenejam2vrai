using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeedMultiplier = 0.6f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float airDashCooldown = 0.8f;

    [Header("Jump Settings")]
    [SerializeField] private float normalJumpForce = 13f;
    [SerializeField] private float dashJumpForceMultiplier = 1.7f;
    [SerializeField] private float dashJumpHorizontalBoost = 8f;

    [Header("Air Control")]
    [SerializeField] private float airDrag = 0.95f;
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
    [SerializeField] private Camera mainCamera;

    // Components
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
    private int movementDirection = 1;
    private bool hasKey = false;

    // Original values
    private float originalScaleY;
    private float originalColliderHeight;
    private Vector2 originalColliderOffset;

    // Input
    private bool spacePressed;
    private bool spaceReleased;
    private bool spaceHeld;
    private float spaceHeldTime = 0f;
    private bool canDashJump = false;

    // Pour éviter dash après crouch
    private bool hasTriedToCrouch = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Start()
    {
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform, false);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        originalScaleY = transform.localScale.y;
        originalColliderHeight = boxCollider.size.y;
        originalColliderOffset = boxCollider.offset;

        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }

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
        spaceReleased = Input.GetKeyUp(KeyCode.Space);
        spaceHeld = Input.GetKey(KeyCode.Space);

        // Incrément du timer seulement si maintenu
        if (spaceHeld)
        {
            spaceHeldTime += Time.deltaTime;
        }

        // Reset complet quand on relâche
        if (spaceReleased)
        {
            spaceHeldTime = 0f;
            hasTriedToCrouch = false;
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

    void HandleCrouch()
    {
        // Détecte si on veut crouch
        if (spaceHeldTime >= crouchHoldTime && isGrounded && !isDashing)
        {
            hasTriedToCrouch = true;

            if (!isCrouching)
            {
                Crouch();
            }
        }
        else
        {
            // Sort du crouch seulement si on relâche ou qu'on n'est plus au sol
            if (isCrouching && (!spaceHeld || !isGrounded))
            {
                StandUp();
            }
        }
    }

    void HandleDashInput()
    {
        // Si on maintient pour crouch, pas de dash
        if (hasTriedToCrouch || spaceHeldTime >= crouchHoldTime)
        {
            return;
        }

        // Dash seulement sur appui court (pas maintenu)
        if (spacePressed && !isCrouching)
        {
            // Dash jump si on dash au sol
            if (isDashing && isGrounded && canDashJump)
            {
                DashJump();
            }
            // Dash normal
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
        if (isDashing)
        {
            return;
        }

        // Course constante au sol
        if (isGrounded)
        {
            float currentSpeed = isCrouching ? runSpeed * crouchSpeedMultiplier : runSpeed;
            float targetX = currentSpeed * movementDirection;

            // Applique directement sans interpolation
            rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);
        }
        else
        {
            // En l'air : conserve la vélocité avec drag
            float newX = rb.linearVelocity.x * airDrag;
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }
    }

    void HandleFastFall()
    {
        if (spaceHeld && !isGrounded && rb.linearVelocity.y < 0 && !isDashing && !hasTriedToCrouch)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
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
    }

    void StandUp()
    {
        // Vérifie s'il y a de la place
        Vector2 checkSize = new Vector2(boxCollider.size.x * 0.9f, originalColliderHeight);
        Vector2 checkPosition = new Vector2(transform.position.x, transform.position.y);

        RaycastHit2D hit = Physics2D.BoxCast(
            checkPosition,
            checkSize,
            0f,
            Vector2.up,
            0.01f,
            groundLayer
        );

        // Se relève seulement s'il n'y a pas d'obstacle
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

            if (mainCamera != null)
            {
                mainCamera.backgroundColor = new Color(0.3f, 0f, 0f);
            }

            Debug.Log("Clé récupérée ! Direction inversée !");
        }

        if (other.CompareTag("Trap"))
        {
            Debug.Log("Piège touché ! Rechargement...");
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

    public bool HasKey() => hasKey;
    public int GetDirection() => movementDirection;
    public bool IsDashing() => isDashing;
    public float GetDashCooldown() => dashCooldownTimer;
}