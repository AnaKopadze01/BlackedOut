
using System.Collections.Generic;
using UnityEngine;

public class ExitSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject exitPrefab;
    public PathfindingGrid grid;
    public AStarPathfinding pathfinding;
    public PolygonCollider2D mansionBoundary;
    public Transform player;
    public GameManager gameManager;

    [Header("Spawn Settings")]
    public float minDistanceFromPlayer = 12f;
    public float minDistanceFromWall = 0.8f;
    public float minDistanceFromBoundary = 0.8f;
    public float maxDistanceFromBoundary = 2f;
    public int maxAttempts = 500;

    [Header("Obstacle Detection")]
    public LayerMask wallLayer;

   

    public void SpawnExit()
    {
        if (exitPrefab == null || grid == null ||
            pathfinding == null || mansionBoundary == null ||
            player == null || gameManager == null)
        {
            Debug.LogError("ExitSpawner: Missing references!");
            return;
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (!grid.TryGetRandomWalkablePosition(
                    out Vector2 candidate))
                break;

            // Must be far enough from the player.
            if (Vector2.Distance(
                    candidate, player.position) <
                minDistanceFromPlayer)
                continue;

            // Must be near an exterior boundary wall.
            float boundaryDistance =
                DistanceToBoundary(candidate);

            if (boundaryDistance < minDistanceFromBoundary ||
                boundaryDistance > maxDistanceFromBoundary)
                continue;

            // Leave enough room for the exit and player.
            if (Physics2D.OverlapCircle(
                    candidate, minDistanceFromWall,
                    wallLayer) != null)
                continue;

            // Confirm the player can walk to the exit.
            List<Vector2> path = pathfinding.FindPath(
                player.position, candidate
            );

            if (path == null || path.Count < 2)
                continue;

            // Create exactly one exit.
            GameObject exit = Instantiate(
                exitPrefab,
                new Vector3(candidate.x, candidate.y, 0f),
                Quaternion.identity
            );

            // Ensure the spawned door references GameManager.
            ExitDoor exitDoor = exit.GetComponent<ExitDoor>();

            if (exitDoor != null)
                exitDoor.gameManager = gameManager;

            Debug.Log(
                "Exit spawned near hospital boundary at: " +
                candidate
            );

            return;
        }

        Debug.LogError(
            "ExitSpawner: Could not find a valid exit " +
            "location. Increase Max Attempts or " +
            "adjust boundary distance settings."
        );
    }

    private float DistanceToBoundary(Vector2 point)
    {
        float closestDistance = float.MaxValue;

        // Check every edge of the hospital boundary.
        for (int pathIndex = 0;
             pathIndex < mansionBoundary.pathCount;
             pathIndex++)
        {
            Vector2[] path = mansionBoundary.GetPath(pathIndex);

            for (int i = 0; i < path.Length; i++)
            {
                Vector2 start =
                    mansionBoundary.transform.TransformPoint(
                        path[i]
                    );

                Vector2 end =
                    mansionBoundary.transform.TransformPoint(
                        path[(i + 1) % path.Length]
                    );

                float distance =
                    DistanceToSegment(point, start, end);

                if (distance < closestDistance)
                    closestDistance = distance;
            }
        }

        return closestDistance;
    }

    private float DistanceToSegment(
        Vector2 point,
        Vector2 start,
        Vector2 end)
    {
        Vector2 line = end - start;

        if (line.sqrMagnitude < 0.0001f)
            return Vector2.Distance(point, start);

        float t = Vector2.Dot(
            point - start, line
        ) / line.sqrMagnitude;

        t = Mathf.Clamp01(t);

        Vector2 closest = start + t * line;

        return Vector2.Distance(point, closest);
    }
}
