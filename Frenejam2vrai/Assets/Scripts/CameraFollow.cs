using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculer la position désirée
        Vector3 desiredPosition = target.position + offset;

        // Appliquer les contraintes de suivi
        float newX = followX ? desiredPosition.x : transform.position.x;
        float newY = followY ? desiredPosition.y : transform.position.y;
        float newZ = offset.z;

        Vector3 targetPosition = new Vector3(newX, newY, newZ);

        // Appliquer les limites si activées
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Interpolation fluide
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        // Dessiner les limites
        Gizmos.color = Color.green;

        Vector3 topLeft = new Vector3(minX, maxY, 0f);
        Vector3 topRight = new Vector3(maxX, maxY, 0f);
        Vector3 bottomLeft = new Vector3(minX, minY, 0f);
        Vector3 bottomRight = new Vector3(maxX, minY, 0f);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}