
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject keyPrefab;
    public PathfindingGrid grid;
    public AStarPathfinding pathfinding;
    public Transform player;
    public GameManager gameManager;

    [Header("Spawn Settings")]
    public float minDistanceFromPlayer = 8f;
    public float minDistanceBetweenKeys = 12f;
    public float wallClearance = 0.7f;
    public LayerMask wallLayer;
    public int maxSpawnAttempts = 300;

    private List<Vector2> spawnPositions = new List<Vector2>();

    void Start()
    {
        SpawnKeys();
    }

    void SpawnKeys()
    {
        if (keyPrefab == null || grid == null ||
            pathfinding == null || player == null ||
            gameManager == null)
        {
            Debug.LogError("KeySpawner: Missing references!");
            return;
        }

        spawnPositions.Clear();

        int keysToSpawn = gameManager.totalKeys;
        int attempts = 0;

        while (spawnPositions.Count < keysToSpawn &&
               attempts < maxSpawnAttempts)
        {
            attempts++;

            // Find a random walkable cell inside the mansion.
            if (!grid.TryGetRandomWalkablePosition(
                    out Vector2 candidate))
            {
                break;
            }

            // Keep keys away from the player's starting location.
            if (Vector2.Distance(
                    candidate, player.position) <
                minDistanceFromPlayer)
            {
                continue;
            }

            // Make sure the key is not too close to walls.
            bool nearWall = Physics2D.OverlapCircle(
                candidate,
                wallClearance,
                wallLayer
            ) != null;

            if (nearWall)
                continue;

            // Keep keys separated from one another.
            bool tooClose = false;

            foreach (Vector2 existingPosition in spawnPositions)
            {
                if (Vector2.Distance(
                        candidate, existingPosition) <
                    minDistanceBetweenKeys)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;

            // Verify the player can reach the key.
            List<Vector2> path = pathfinding.FindPath(
                player.position,
                candidate
            );

            if (path.Count < 2)
                continue;

            // Accept the valid spawn location.
            spawnPositions.Add(candidate);
        }

        // Spawn the keys at the chosen positions.
        foreach (Vector2 position in spawnPositions)
        {
            Instantiate(
                keyPrefab,
                new Vector3(position.x, position.y, 0f),
                Quaternion.identity
            );
        }

        Debug.Log(
            "KeySpawner: Spawned " +
            spawnPositions.Count + "/" + keysToSpawn +
            " keys."
        );

        if (spawnPositions.Count < keysToSpawn)
        {
            Debug.LogWarning(
                "Not enough valid key positions found. " +
                "Try reducing minimum spawn distances."
            );
        }
    }
}
