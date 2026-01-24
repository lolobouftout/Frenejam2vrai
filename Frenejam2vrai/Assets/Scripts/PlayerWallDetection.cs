using UnityEngine;

public class PlayerWallDetection : MonoBehaviour
{
    [Header("Wall Detection")]
    [SerializeField] private float wallCheckDistance = 0.7f;
    [SerializeField] private float minSpeedForWallDeath = 12f;
    [SerializeField] private LayerMask wallDeathLayer;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCollisionHandler collisionHandler;

    private Rigidbody2D rb;
    private float spawnProtectionTimer = 0f;
    private const float SPAWN_PROTECTION_DURATION = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (collisionHandler == null)
            collisionHandler = GetComponent<PlayerCollisionHandler>();
    }

    private void Start()
    {
        // Protection au spawn
        spawnProtectionTimer = SPAWN_PROTECTION_DURATION;
    }

    private void Update()
    {
        // Décompter la protection spawn
        if (spawnProtectionTimer > 0f)
        {
            spawnProtectionTimer -= Time.deltaTime;
            return;
        }

        // Ne vérifier que si le joueur court
        if (!playerMovement.IsRunning || playerMovement.IsWaitingAfterKey)
            return;

        CheckWallCollision();
    }

    private void CheckWallCollision()
    {
        // Obtenir la vitesse horizontale actuelle
        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);

        // Vérifier si la vitesse est suffisante
        if (currentSpeed < minSpeedForWallDeath)
            return;

        // Direction du raycast basée sur la direction du joueur
        Vector2 rayDirection = Vector2.right * playerMovement.Direction;
        Vector2 rayOrigin = transform.position;

        // Lancer le raycast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, wallCheckDistance, wallDeathLayer);

        if (hit.collider != null)
        {
            // Vérifier si c'est une collision frontale
            float dotProduct = Vector2.Dot(rayDirection.normalized, hit.normal);

            // Si dot product < -0.7, c'est une collision frontale (mur face à nous)
            if (dotProduct < -0.7f)
            {
                Debug.Log($"MORT PAR MUR - Vitesse: {currentSpeed:F2}, Dot: {dotProduct:F2}");

                if (collisionHandler != null)
                {
                    collisionHandler.Die();
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (playerMovement == null) return;

        // Dessiner le raycast de détection
        Vector2 rayDirection = Vector2.right * playerMovement.Direction;
        Vector2 rayOrigin = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(rayOrigin, rayDirection * wallCheckDistance);
    }
}