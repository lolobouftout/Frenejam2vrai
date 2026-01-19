using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
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
        if (target == null)
        {
            Debug.LogError("CameraFollow: Target is not assigned! Please assign the Player transform in the Inspector.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        Vector3 finalPosition = transform.position;

        if (followX)
        {
            finalPosition.x = smoothedPosition.x;
        }

        if (followY)
        {
            finalPosition.y = smoothedPosition.y;
        }

        if (useBounds)
        {
            finalPosition.x = Mathf.Clamp(finalPosition.x, minX, maxX);
            finalPosition.y = Mathf.Clamp(finalPosition.y, minY, maxY);
        }

        transform.position = finalPosition;
    }
}