using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private ParticleEffect particleEffect;

    [Header("Movement")]
    [SerializeField] private float runSpeed = 8f;
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
    private float direction = 1f; // 1 = droite, -1 = gauche
    private bool isRunning = false;
    private bool isWaitingAfterKey = false;

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
        HandleMovement();
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
        // Si en attente après la clé, bloquer tout mouvement
        if (isWaitingAfterKey)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Si le joueur court, avancer automatiquement dans la direction
        if (isRunning)
        {
            rb.linearVelocity = new Vector2(direction * runSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Si pas encore commencé, rester immobile
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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

    public void Jump()
    {
        if (CheckIsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public void CollectKey()
    {
        hasKey = true;
        isWaitingAfterKey = true;
        isRunning = false;
        rb.linearVelocity = Vector2.zero;
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
        direction = 1f; // Retour à droite
        isRunning = false;
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
        isRunning = true;
        // Inverser la direction
        direction = -direction;
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

        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            CollectKey();
            other.gameObject.SetActive(false);
            return;
        }

        if (isSpawnProtected)
            return;

        if (other.CompareTag("Trap"))
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