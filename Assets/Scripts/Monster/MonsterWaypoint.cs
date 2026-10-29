
using UnityEngine;

public class MonsterWaypoint : MonoBehaviour
{
    [Header("Connected Waypoints")]
    public MonsterWaypoint[] neighbors;

    // Draw connections between waypoints in the Scene view.
    void OnDrawGizmos()
    {
        if (neighbors == null)
            return;

        Gizmos.color = Color.yellow;

        foreach (MonsterWaypoint neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(
                    transform.position,
                    neighbor.transform.position
                );
            }
        }

        Gizmos.DrawSphere(transform.position, 0.15f);
    }
}
