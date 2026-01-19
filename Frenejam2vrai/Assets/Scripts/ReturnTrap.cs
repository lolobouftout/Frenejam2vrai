using UnityEngine;

public class ReturnTrap : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private PlayerController player;

    [Header("Settings")]
    [SerializeField] private bool activeOnlyOnReturn = true;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("ReturnTrap: Player reference is missing! Please assign it in the Inspector.");
        }

        if (activeOnlyOnReturn)
        {
            SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        if (activeOnlyOnReturn && player.HasKey())
        {
            SetActive(true);
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