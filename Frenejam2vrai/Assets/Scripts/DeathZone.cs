using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool respectSpawnProtection = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleDeath(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleDeath(collision.gameObject);
    }

    void HandleDeath(GameObject target)
    {
        PlayerMovement player = target.GetComponent<PlayerMovement>();

        if (player == null)
            return;

        if (respectSpawnProtection && player.IsSpawnProtected())
            return;

        player.Die();
    }
}