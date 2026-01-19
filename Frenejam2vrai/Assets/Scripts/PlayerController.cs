using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fastFallSpeed = 15f;
    [SerializeField] private float airDrag = 0.98f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchScaleY = 0.5f;
    [SerializeField] private float crouchColliderHeight = 0.5f;

    // Components
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    // State
    private bool isGrounded;
    private bool isMoving = false;
    private bool isCrouching = false;
    private int movementDirection = 1; // 1 = droite, -1 = gauche
    private bool hasKey = false;

    // Original values
    private float originalScaleY;
    private float originalColliderHeight;
    private Vector2 originalColliderOffset;

    // Input
    private bool spacePressed;
    private bool spaceHeld;
    private bool wasSpacePressedLastFrame;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = transform;
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
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        HandleMovementToggle();
        HandleCrouch();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJumpAndFall();
    }

    void HandleInput()
    {
        // Detect space press (one frame only)
        spacePressed = Input.GetKeyDown(KeyCode.Space);
        spaceHeld = Input.GetKey(KeyCode.Space);

        // Track if space was pressed last frame
        if (spacePressed)
        {
            wasSpacePressedLastFrame = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            wasSpacePressedLastFrame = false;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleMovementToggle()
    {
        // Toggle entre déplacement et saut avec la touche espace
        if (spacePressed && isGrounded && !isCrouching)
        {
            if (!isMoving)
            {
                // Active le déplacement
                isMoving = true;
            }
            else
            {
                // Saute
                Jump();
                isMoving = false; // On arrête le déplacement quand on saute
            }
        }
    }

    void HandleMovement()
    {
        if (isMoving && isGrounded && !isCrouching)
        {
            // Déplacement horizontal
            rb.linearVelocity = new Vector2(moveSpeed * movementDirection, rb.linearVelocity.y);
        }
        else if (!isGrounded)
        {
            // Dans les airs, on conserve la vélocité mais avec un drag
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * airDrag, rb.linearVelocity.y);
        }
        else if (!isMoving && isGrounded)
        {
            // Arrêt progressif au sol
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.8f, rb.linearVelocity.y);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void HandleJumpAndFall()
    {
        // Fast fall : maintenir espace en l'air fait tomber plus vite
        if (spaceHeld && !isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
        }
    }

    void HandleCrouch()
    {
        // S'accroupir : maintenir espace au sol
        if (spaceHeld && isGrounded && !isMoving)
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

        // Réduit la taille du joueur
        Vector3 scale = transform.localScale;
        scale.y = originalScaleY * crouchScaleY;
        transform.localScale = scale;

        // Ajuste le collider
        Vector2 size = boxCollider.size;
        size.y = crouchColliderHeight;
        boxCollider.size = size;

        Vector2 offset = boxCollider.offset;
        offset.y = -(originalColliderHeight - crouchColliderHeight) / 2f;
        boxCollider.offset = offset;

        // Stop le mouvement
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void StandUp()
    {
        // Vérifie s'il y a de la place pour se relever
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

            // Restaure la taille
            Vector3 scale = transform.localScale;
            scale.y = originalScaleY;
            transform.localScale = scale;

            // Restaure le collider
            boxCollider.size = new Vector2(boxCollider.size.x, originalColliderHeight);
            boxCollider.offset = originalColliderOffset;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ramasse la clé
        if (other.CompareTag("Key") && !hasKey)
        {
            hasKey = true;
            movementDirection = -1; // Inverse la direction (retour)
            other.gameObject.SetActive(false);

            // Change la couleur du joueur pour indiquer qu'il a la clé
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.yellow;
            }

            Debug.Log("Clé récupérée ! Retour à la base... Bonne chance !");
        }

        // Mort sur piège
        if (other.CompareTag("Trap"))
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Mort ! Rechargement du niveau...");
        // Recharge la scène
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    // Visualisation du ground check
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // Getters publics pour le GameManager
    public bool HasKey() => hasKey;
    public int GetDirection() => movementDirection;
}