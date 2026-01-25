using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Visual")]
    [SerializeField] private Color dashColor = Color.white;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private AudioManager audioManager;

    private Rigidbody2D rb;
    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteRenderer;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private Color originalColor;
    private bool canDashJump = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (trailRenderer != null)
            trailRenderer.emitting = false;

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }

        HandleInput();
    }

    private void HandleInput()
    {
        // Si pas encore commencé, premier espace = démarrer
        if (!playerMovement.IsRunning && Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.StartRunning();
            return;
        }

        // Si en attente après la clé, espace = reprendre
        if (playerMovement.IsWaitingAfterKey && Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.ResumeAfterKey();
            return;
        }

        // Si pas en train de courir, ignorer
        if (!playerMovement.IsRunning || playerMovement.IsWaitingAfterKey)
            return;

        // Pendant un dash au sol, espace = saut
        if (isDashing && canDashJump && Input.GetKeyDown(KeyCode.Space))
        {
            PerformDashJump();
            return;
        }

        // Espace = dash (si pas en cooldown et pas déjà en dash)
        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0f && !isDashing)
        {
            PerformDash();
        }
    }

    private void PerformDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        // On peut seulement dash jump si on est au sol au moment du dash
        canDashJump = playerMovement.CheckIsGrounded();

        // Dash horizontal rapide
        rb.linearVelocity = new Vector2(dashSpeed * playerMovement.Direction, rb.linearVelocity.y);

        if (trailRenderer != null)
            trailRenderer.emitting = true;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = dashColor;
        }

        if (audioManager != null)
            audioManager.PlayDashSound();
    }

    private void PerformDashJump()
    {
        playerMovement.Jump();
        EndDash();

        if (audioManager != null)
            audioManager.PlayJumpSound();
    }

    private void EndDash()
    {
        isDashing = false;
        canDashJump = false;
        cooldownTimer = dashCooldown;

        if (trailRenderer != null)
            trailRenderer.emitting = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    public bool IsDashing => isDashing;
    public float CooldownTimer => cooldownTimer;
    public float MaxCooldown => dashCooldown;
}