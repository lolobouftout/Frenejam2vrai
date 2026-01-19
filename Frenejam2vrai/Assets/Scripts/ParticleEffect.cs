using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [Header("Jump Particles")]
    [SerializeField] private ParticleSystem jumpParticles;

    [Header("Land Particles")]
    [SerializeField] private ParticleSystem landParticles;

    [Header("Death Particles")]
    [SerializeField] private ParticleSystem deathParticles;

    private Rigidbody2D rb;
    private bool wasGrounded = true;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
        }
    }

    void Update()
    {
        if (groundCheck == null) return;

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Détecte l'atterrissage
        if (isGrounded && !wasGrounded && landParticles != null)
        {
            landParticles.Play();
        }

        // Détecte le saut
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