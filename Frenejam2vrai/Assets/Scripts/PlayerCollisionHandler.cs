using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifier si c'est la clé
        if (collision.CompareTag("Key"))
        {
            collision.gameObject.SetActive(false);
            playerMovement.PickupKey();

            if (audioManager != null)
                audioManager.PlayKeyPickupSound();

            Debug.Log("CLÉ RÉCUPÉRÉE - Direction inversée !");
            return;
        }

        // Vérifier si c'est un piège
        if (collision.CompareTag("Trap"))
        {
            // Vérifier si c'est un ReturnTrap
            ReturnTrap returnTrap = collision.GetComponent<ReturnTrap>();

            if (returnTrap != null)
            {
                // Vérifier si le ReturnTrap est actif
                if (!returnTrap.IsActive)
                {
                    // Ne pas mourir si le ReturnTrap n'est pas encore actif
                    return;
                }
            }

            // Mourir
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("MORT - Rechargement du niveau...");

        if (audioManager != null)
            audioManager.PlayDeathSound();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}