using UnityEngine;

public class DashableWall : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color passedColor = new Color(0.5f, 1f, 1f, 0.3f);

    [Header("References")]
    [SerializeField] private PlayerDash playerDash;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool hasPassed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    void Start()
    {
        if (playerDash == null)
        {
            playerDash = FindFirstObjectByType<PlayerDash>();
            if (playerDash == null)
            {
                Debug.LogError("DashableWall: PlayerDash not found! Assign it manually in the Inspector.");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

        if (player == null)
            return;

        // Si le joueur dash, il peut passer
        if (playerDash != null && playerDash.IsDashing)
        {
            // Désactiver temporairement le collider pour laisser passer
            if (col != null)
            {
                col.enabled = false;
            }

            // Changer l'apparence
            if (spriteRenderer != null && !hasPassed)
            {
                spriteRenderer.color = passedColor;
                hasPassed = true;
            }

            // Réactiver après un court délai
            Invoke(nameof(ReenableCollider), 0.5f);
        }
        else
        {
            // Sans dash, le joueur meurt
            player.Die();
        }
    }

    void ReenableCollider()
    {
        if (col != null)
        {
            col.enabled = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (GetComponent<Collider2D>() != null)
        {
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}