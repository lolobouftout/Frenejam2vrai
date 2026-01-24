using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private ParticleEffect particleEffect;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpBoostMultiplier = 1.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Spawn Protection")]
    [SerializeField] private float spawnProtectionDuration = 1f;
    [SerializeField] private float spawnUpwardForce = 2f;

    private bool hasKey = false;
    private float spawnProtectionTimer = 0f;
    private bool isSpawnProtected = false;
    private float direction = 0f;
    private bool isRunning = false;
    private bool isWaitingAfterKey = false;
    private float waitTimer = 0f;
    private float waitDuration = 0.5f;
    private bool hasJumpBoost = false;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (groundLayer == 0)
            groundLayer = LayerMask.GetMask("Default");

        if (groundCheck == null)
            Debug.LogError("PlayerMovement: Ground Check Transform is not assigned!");

        EnableSpawnProtection();
    }

    void Update()
    {
        UpdateSpawnProtection();
        UpdateWaitAfterKey();
        HandleMovement();
        HandleJump();
    }

    void UpdateSpawnProtection()
    {
        if (isSpawnProtected)
        {
            spawnProtectionTimer -= Time.deltaTime;
            if (spawnProtectionTimer <= 0f)
            {
                isSpawnProtected = false;
            }
        }
    }

    void UpdateWaitAfterKey()
    {
        if (isWaitingAfterKey)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaitingAfterKey = false;
            }
        }
    }

    void EnableSpawnProtection()
    {
        isSpawnProtected = true;
        spawnProtectionTimer = spawnProtectionDuration;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, spawnUpwardForce);
        }
    }

    void HandleMovement()
    {
        // Si en attente après la clé, bloquer tout mouvement
        if (isWaitingAfterKey)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            direction = 0f;
            return;
        }

        // Mouvement horizontal normal (ZQSD ou flèches)
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Toujours permettre le mouvement horizontal
        if (horizontalInput != 0)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
            direction = horizontalInput;
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && CheckIsGrounded())
        {
            float currentJumpForce = hasJumpBoost ? jumpForce * jumpBoostMultiplier : jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
        }
    }

    bool IsGroundedInternal()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool CheckIsGrounded()
    {
        return IsGroundedInternal();
    }

    public bool IsGrounded
    {
        get { return IsGroundedInternal(); }
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public void CollectKey()
    {
        hasKey = true;
        isWaitingAfterKey = true;
        waitTimer = waitDuration;
    }

    public void PickupKey()
    {
        CollectKey();
    }

    public bool IsSpawnProtected()
    {
        return isSpawnProtected;
    }

    public void ResetKey()
    {
        hasKey = false;
        isWaitingAfterKey = false;
        waitTimer = 0f;
    }

    public void ApplySpawnProtection()
    {
        EnableSpawnProtection();
    }

    public void StartRunning()
    {
        isRunning = true;
    }

    public void ResumeAfterKey()
    {
        isWaitingAfterKey = false;
        waitTimer = 0f;
    }

    public void ApplyJumpBoost()
    {
        hasJumpBoost = true;
    }

    public void RemoveJumpBoost()
    {
        hasJumpBoost = false;
    }

    public bool HasJumpBoost()
    {
        return hasJumpBoost;
    }

    public float Direction
    {
        get { return direction; }
    }

    public bool IsRunning
    {
        get { return isRunning; }
    }

    public bool IsWaitingAfterKey
    {
        get { return isWaitingAfterKey; }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSpawnProtected)
            return;

        if (collision.gameObject.CompareTag("Trap") || collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            CollectKey();
            Destroy(other.gameObject);
            return;
        }

        if (isSpawnProtected)
            return;

        if (other.CompareTag("Trap") || other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (particleEffect != null)
            particleEffect.PlayDeathEffect();

        PlayerRespawner respawner = GetComponent<PlayerRespawner>();
        if (respawner != null)
        {
            respawner.Respawn();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}