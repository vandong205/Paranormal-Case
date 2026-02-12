using UnityEngine;

public class TrackTargetGizmoDrawer : MonoBehaviour
{
    public float range = 5f;
    public Collider selfCollider; // Assign the collider of the mutant (self)

    private void OnDrawGizmosSelected()
    {
        if (selfCollider == null)
            return;
        var bounds = selfCollider.bounds;
        var minY = bounds.min.y;
        var maxY = bounds.max.y;
        var center = transform.position;

        // Draw the vision box
        Gizmos.color = Color.cyan;
        Vector3 boxCenter = new Vector3(center.x, (minY + maxY) * 0.5f, center.z);
        float visionDepth = 8f; // Increase this value for a wider Z vision area
        Vector3 boxSize = new Vector3(range * 2, maxY - minY, visionDepth);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
