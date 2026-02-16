using UnityEngine;

public class Marker : MonoBehaviour //I'd like to know where things are in 3D space, thanks.
{
    public Color gizmoColor = Color.red;
    public float gizmoRadius = 0.5f;
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}