
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTester : MonoBehaviour
{
    [Header("References")]
    public AStarPathfinding pathfinding;
    public Transform monster;
    public Transform player;

    [Header("Testing Settings")]
    public bool showPath = true;
    public float updateInterval = 0.5f;

    private List<Vector2> currentPath = new List<Vector2>();
    private float updateTimer;

    void Update()
    {
        updateTimer -= Time.deltaTime;

        if (updateTimer <= 0f)
        {
            updateTimer = Mathf.Max(0.1f, updateInterval);
            CalculatePath();
        }
    }

    void CalculatePath()
    {
        if (pathfinding == null ||
            monster == null ||
            player == null)
        {
            return;
        }

        // Calculate a route from the monster to the player.
        currentPath = pathfinding.FindPath(
            monster.position,
            player.position
        );
    }

    void OnDrawGizmos()
    {
        if (!showPath || currentPath == null ||
            currentPath.Count < 2)
        {
            return;
        }

        // Draw the calculated route in the Scene view.
        Gizmos.color = Color.yellow;

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(
                currentPath[i],
                currentPath[i + 1]
            );

            Gizmos.DrawSphere(
                currentPath[i],
                0.12f
            );
        }

        Gizmos.DrawSphere(
            currentPath[currentPath.Count - 1],
            0.12f
        );
    }
}
