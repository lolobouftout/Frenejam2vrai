using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Visualization")]
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoSize = 0.5f;
    [SerializeField] private bool showGizmo = true;

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);

        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = transform.position + Vector3.up * gizmoSize * 2;
        Gizmos.DrawLine(arrowStart, arrowEnd);

        Gizmos.DrawLine(arrowEnd, arrowEnd + new Vector3(-0.2f, -0.2f, 0));
        Gizmos.DrawLine(arrowEnd, arrowEnd + new Vector3(0.2f, -0.2f, 0));
    }
}