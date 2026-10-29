
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("Navigation References")]
    public AStarPathfinding pathfinding;
    public PathfindingGrid grid;
    public Transform player;
    public PlayerFlashlight playerFlashlight;
    public LayerMask wallLayer;

    [Header("Movement Settings")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float stoppingDistance = 0.1f;

    [Header("Wandering Settings")]
    public float waitTime = 1f;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float flashlightDetectionRadius = 9f;
    public float losePlayerTime = 3f;

    [Header("Pathfinding Settings")]
    public float pathUpdateInterval = 0.5f;

    private Rigidbody2D rb;
    private List<Vector2> currentPath = new List<Vector2>();

    private int currentPathIndex;
    private float waitTimer;
    private float pathUpdateTimer;
    private float lostPlayerTimer;

    private Vector2 currentDestination;
    private bool hasDestination;
    private bool isChasing;

    // Other scripts can check the monster's current behavior.
    public bool IsChasing => isChasing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (grid == null || pathfinding == null || player == null)
        {
            Debug.LogError("Monster AI references are missing!");
            enabled = false;
            return;
        }

        ChooseRandomDestination();
    }

    void FixedUpdate()
    {
        CheckForPlayer();

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }

        FollowPath();
    }

    void CheckForPlayer()
    {
        Vector2 monsterPosition = rb.position;
        Vector2 playerPosition = player.position;

        float distance = Vector2.Distance(
            monsterPosition,
            playerPosition
        );

        // Flashlight makes the player easier to detect.
        float currentDetectionRadius = detectionRadius;

        if (playerFlashlight != null &&
            playerFlashlight.IsFlashlightOn)
        {
            currentDetectionRadius = flashlightDetectionRadius;
        }

        // Check whether a wall blocks the monster's view.
        bool wallBlocking = Physics2D.Linecast(
            monsterPosition,
            playerPosition,
            wallLayer
        ).collider != null;

        bool canSeePlayer =
            distance <= currentDetectionRadius &&
            !wallBlocking;

        if (canSeePlayer)
        {
            // Begin or continue chasing.
            isChasing = true;
            lostPlayerTimer = losePlayerTime;
        }
        else if (isChasing)
        {
            // Give the player time to escape.
            lostPlayerTimer -= Time.fixedDeltaTime;

            if (lostPlayerTimer <= 0f)
            {
                StopChasing();
            }
        }
    }

    void ChasePlayer()
    {
        pathUpdateTimer -= Time.fixedDeltaTime;

        if (pathUpdateTimer <= 0f)
        {
            pathUpdateTimer = Mathf.Max(
                0.1f,
                pathUpdateInterval
            );

            // Recalculate a route toward the player.
            List<Vector2> newPath = pathfinding.FindPath(
                rb.position,
                player.position
            );

            if (newPath.Count > 1)
            {
                currentPath = newPath;
                currentPathIndex = 1;
            }
        }
    }

    void StopChasing()
    {
        isChasing = false;
        hasDestination = false;
        currentPath.Clear();
        currentPathIndex = 0;
        waitTimer = waitTime;
    }

    void Wander()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!hasDestination)
        {
            ChooseRandomDestination();
            return;
        }

        pathUpdateTimer -= Time.fixedDeltaTime;

        if (pathUpdateTimer <= 0f)
        {
            pathUpdateTimer = Mathf.Max(
                0.1f,
                pathUpdateInterval
            );

            UpdateWanderPath();
        }
    }

    void ChooseRandomDestination()
    {
        // Try to find a reachable destination.
        for (int attempt = 0; attempt < 20; attempt++)
        {
            if (!grid.TryGetRandomWalkablePosition(
                out Vector2 destination))
            {
                break;
            }

            List<Vector2> newPath = pathfinding.FindPath(
                rb.position,
                destination
            );

            if (newPath.Count > 1)
            {
                currentDestination = destination;
                currentPath = newPath;
                currentPathIndex = 1;
                hasDestination = true;
                pathUpdateTimer = pathUpdateInterval;
                return;
            }
        }

        hasDestination = false;
        waitTimer = waitTime;
    }

    void UpdateWanderPath()
    {
        List<Vector2> newPath = pathfinding.FindPath(
            rb.position,
            currentDestination
        );

        if (newPath.Count > 1)
        {
            currentPath = newPath;
            currentPathIndex = 1;
        }
    }

    void FollowPath()
    {
        // Don't move while waiting between patrols.
        if (!isChasing && waitTimer > 0f)
            return;

        if (currentPathIndex >= currentPath.Count)
        {
            if (!isChasing && hasDestination)
            {
                hasDestination = false;
                currentPath.Clear();
                currentPathIndex = 0;
                waitTimer = waitTime;
            }

            return;
        }

        Vector2 target = currentPath[currentPathIndex];

        float distance = Vector2.Distance(
            rb.position,
            target
        );

        if (distance <= stoppingDistance)
        {
            currentPathIndex++;
            return;
        }

        // Monster moves faster when chasing.
        float speed = isChasing ? chaseSpeed : wanderSpeed;

        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            target,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }
}
