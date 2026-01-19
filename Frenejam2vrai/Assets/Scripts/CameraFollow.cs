using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    void Start()
    {
        // Trouve le joueur si non assigné
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Position désirée
        Vector3 desiredPosition = target.position + offset;

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply suivant les axes activés
        Vector3 finalPosition = transform.position;

        if (followX)
        {
            finalPosition.x = smoothedPosition.x;
        }

        if (followY)
        {
            finalPosition.y = smoothedPosition.y;
        }

        // Applique les limites si activées
        if (useBounds)
        {
            finalPosition.x = Mathf.Clamp(finalPosition.x, minX, maxX);
            finalPosition.y = Mathf.Clamp(finalPosition.y, minY, maxY);
        }

        transform.position = finalPosition;
    }
}