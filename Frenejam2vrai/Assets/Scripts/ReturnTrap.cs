using UnityEngine;

public class ReturnTrap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;

    [Header("Settings")]
    [SerializeField] private bool activeOnlyOnReturn = true;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Start()
    {
        // Trouve le joueur si non assigné
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Désactive au départ si c'est un piège de retour
        if (activeOnlyOnReturn)
        {
            SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        // Active le piège uniquement quand le joueur a la clé
        if (activeOnlyOnReturn)
        {
            if (player.HasKey())
            {
                SetActive(true);
            }
        }
    }

    void SetActive(bool active)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = active;
        }

        if (col != null)
        {
            col.enabled = active;
        }
    }
}