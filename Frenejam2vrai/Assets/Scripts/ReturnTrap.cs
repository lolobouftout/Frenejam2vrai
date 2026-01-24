using UnityEngine;

public class ReturnTrap : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool activeOnlyOnReturn = true;

    [Header("References")]
    [SerializeField] private PlayerMovement player;

    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D col;
    private bool isActive = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<PolygonCollider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError("ReturnTrap nécessite un SpriteRenderer !", this);
        }

        if (col == null)
        {
            Debug.LogError("ReturnTrap nécessite un PolygonCollider2D !", this);
        }

        // Vérifier le tag
        if (!gameObject.CompareTag("Trap"))
        {
            Debug.LogWarning("ReturnTrap devrait avoir le tag 'Trap' !", this);
        }
    }

    private void Start()
    {
        if (activeOnlyOnReturn)
        {
            // Désactiver complètement au début
            SetActive(false);
        }
        else
        {
            SetActive(true);
        }
    }

    private void Update()
    {
        // Activer uniquement si le joueur a la clé
        if (activeOnlyOnReturn && player != null && player.HasKey && !isActive)
        {
            SetActive(true);
            Debug.Log($"ReturnTrap {gameObject.name} ACTIVÉ car le joueur a la clé !");
        }
    }

    private void SetActive(bool active)
    {
        isActive = active;

        if (spriteRenderer != null)
            spriteRenderer.enabled = active;

        if (col != null)
            col.enabled = active;
    }

    public bool IsActive => isActive;
}