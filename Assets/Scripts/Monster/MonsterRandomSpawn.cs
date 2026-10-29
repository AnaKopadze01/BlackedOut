
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class MonsterRandomSpawn : MonoBehaviour
{
    [Header("References")]
    public PathfindingGrid grid;
    public AStarPathfinding pathfinding;
    public Transform player;

    [Header("Spawn Settings")]
    public float minimumDistance = 15f;
    public float wallClearance = 0.8f;
    public int maxAttempts = 500;
    public LayerMask wallLayer;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        SpawnMonster();
    }

    void SpawnMonster()
    {
        if (grid == null || pathfinding == null || player == null)
        {
            Debug.LogError("MonsterRandomSpawn: Missing references!");
            return;
        }

        for (int i = 0; i < maxAttempts; i++)
        {
            if (!grid.TryGetRandomWalkablePosition(out Vector2 position))
                continue;

            if (Vector2.Distance(position, player.position) < minimumDistance)
                continue;

            if (Physics2D.OverlapCircle(position, wallClearance, wallLayer) != null)
                continue;

            List<Vector2> path =
                pathfinding.FindPath(position, player.position);

            if (path == null || path.Count < 2)
                continue;

            // Teleport the existing monster to the new location.
            if (rb != null)
            {
                rb.position = position;
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                transform.position = new Vector3(
                    position.x, position.y, transform.position.z
                );
            }

            Debug.Log(
                "Monster successfully spawned at: " + position
            );

            return;
        }

        Debug.LogError(
            "MonsterRandomSpawn: No valid spawn position found. " +
            "Monster remains at its original location."
        );
    }
}
