using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private SpawnPoint spawnPoint;
    [SerializeField] private Vector3 defaultSpawnOffset = new Vector3(0, 2, 0);
    [SerializeField] private float respawnDelay = 0.1f;

    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Vector3 initialPosition;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("PlayerRespawner: No SpawnPoint assigned. Using initial position as spawn.");
        }
    }

    public void Respawn()
    {
        Invoke(nameof(ExecuteRespawn), respawnDelay);
    }

    void ExecuteRespawn()
    {
        Vector3 spawnPosition;

        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.GetPosition();
        }
        else
        {
            spawnPosition = initialPosition + defaultSpawnOffset;
        }

        transform.position = spawnPosition;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (playerMovement != null)
        {
            playerMovement.ResetKey();
            playerMovement.ApplySpawnProtection();
        }
    }
}