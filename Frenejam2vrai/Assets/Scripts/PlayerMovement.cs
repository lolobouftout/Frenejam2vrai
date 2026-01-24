using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private ParticleEffect particleEffect;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Spawn Protection")]
    [SerializeField] private float spawnProtectionDuration = 1f;
    [SerializeField] private float spawnUpwardForce = 2f;

    private bool hasKey = false;
    private float spawnProtectionTimer = 0f;
    private bool isSpawnProtected = false;

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
            Debug.LogError("PlayerController: Ground Check Transform is not assigned!");

        EnableSpawnProtection();
    }

    void Update()
    {
        UpdateSpawnProtection();
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
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public void CollectKey()
    {
        hasKey = true;
    }

    public bool IsSpawnProtected()
    {
        return isSpawnProtected;
    }

    public void ResetKey()
    {
        hasKey = false;
    }

    public void ApplySpawnProtection()
    {
        EnableSpawnProtection();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSpawnProtected)
            return;

        if (collision.gameObject.CompareTag("Trap") || collision.gameObject.CompareTag("Enemy"))
        {
            TriggerDeath();
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
            TriggerDeath();
        }
    }

    void TriggerDeath()
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