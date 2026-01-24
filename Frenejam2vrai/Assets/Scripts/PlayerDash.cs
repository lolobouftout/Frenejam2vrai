using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float airDashCooldown = 0.8f;

    [Header("Dash Jump Settings")]
    [SerializeField] private float normalJumpForce = 13f;
    [SerializeField] private float dashJumpForceMultiplier = 1.7f;
    [SerializeField] private float dashJumpHorizontalBoost = 10f;

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
    private bool wasGroundedOnDash = false;
    private Color originalColor;

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
        if (!playerMovement.IsRunning && Input.GetKeyDown(KeyCode.Space))
        {
            PerformDash();
            playerMovement.StartRunning();
            return;
        }

        if (playerMovement.IsWaitingAfterKey && Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.ResumeAfterKey();
            return;
        }

        if (!playerMovement.IsRunning || playerMovement.IsWaitingAfterKey) return;

        if (isDashing && wasGroundedOnDash && Input.GetKeyDown(KeyCode.Space))
        {
            PerformDashJump();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0f && !isDashing)
        {
            PerformDash();
        }
    }

    private void PerformDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        wasGroundedOnDash = playerMovement.CheckIsGrounded();

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
        float jumpForce = normalJumpForce * dashJumpForceMultiplier;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x + (dashJumpHorizontalBoost * playerMovement.Direction),
            rb.linearVelocity.y
        );

        EndDash();

        if (audioManager != null)
            audioManager.PlayJumpSound();
    }

    private void EndDash()
    {
        isDashing = false;

        cooldownTimer = wasGroundedOnDash ? dashCooldown : airDashCooldown;

        if (trailRenderer != null)
            trailRenderer.emitting = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (!wasGroundedOnDash)
        {
            playerMovement.ApplyJumpBoost();
        }
    }

    public bool IsDashing => isDashing;
    public float CooldownTimer => cooldownTimer;
    public float MaxCooldown => wasGroundedOnDash ? dashCooldown : airDashCooldown;
}