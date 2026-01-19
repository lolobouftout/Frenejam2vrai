using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem landParticles;
    [SerializeField] private ParticleSystem deathParticles;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool wasGrounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
        }

        if (groundCheck == null)
        {
            Debug.LogWarning("ParticleEffect: Ground Check is not assigned. Effects may not work properly.");
        }
    }

    void Update()
    {
        if (groundCheck == null || rb == null) return;

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded && landParticles != null)
        {
            landParticles.Play();
        }

        if (!isGrounded && wasGrounded && rb.linearVelocity.y > 0 && jumpParticles != null)
        {
            jumpParticles.Play();
        }

        wasGrounded = isGrounded;
    }

    public void PlayDeathEffect()
    {
        if (deathParticles != null)
        {
            deathParticles.Play();
        }
    }
}