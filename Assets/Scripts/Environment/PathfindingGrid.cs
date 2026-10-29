using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2 gridWorldSize = new Vector2(60f, 40f);
    public float cellSize = 0.5f;

    [Header("Obstacle Detection")]
    public LayerMask wallLayer;
    public float obstacleCheckRadius = 0.35f;

    [Header("Mansion Boundary")]
    public PolygonCollider2D mansionCollider;

    [Header("Visualization")]
    public bool showGrid = true;

    private bool[,] walkableGrid;
    private int gridWidth;
    private int gridHeight;

    // Allow the pathfinding algorithm to access grid dimensions.
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    void Awake()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        // Prevent invalid grid settings.
        if (cellSize <= 0f)
        {
            Debug.LogError("Cell Size must be greater than zero.");
            return;
        }

        // Make sure the mansion boundary is assigned.
        if (mansionCollider == null)
        {
            Debug.LogError("Mansion Collider is not assigned!");
            return;
        }

        // Calculate the number of cells.
        gridWidth = Mathf.CeilToInt(gridWorldSize.x / cellSize);
        gridHeight = Mathf.CeilToInt(gridWorldSize.y / cellSize);

        walkableGrid = new bool[gridWidth, gridHeight];

        // Check every cell in the grid.
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPosition = GetWorldPosition(x, y);

                // Check if the cell is inside the mansion.
                bool insideMansion =
                    mansionCollider.OverlapPoint(worldPosition);

                // Check if the cell overlaps a wall.
                bool blockedByWall = Physics2D.OverlapCircle(
                    worldPosition,
                    obstacleCheckRadius,
                    wallLayer
                ) != null;

                // A cell is walkable only when both
                // conditions are satisfied.
                walkableGrid[x, y] =
                    insideMansion && !blockedByWall;
            }
        }

        Debug.Log(
            "Pathfinding grid generated: " +
            gridWidth + " x " + gridHeight
        );
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        // Calculate the bottom-left corner of the grid.
        Vector2 bottomLeft =
            (Vector2)transform.position - gridWorldSize / 2f;

        // Return the center of the requested cell.
        return bottomLeft + new Vector2(
            x * cellSize + cellSize / 2f,
            y * cellSize + cellSize / 2f
        );
    }


    public Vector2Int GetGridPosition(Vector2 worldPosition)
    {
        // Find the bottom-left corner of the grid.
        Vector2 bottomLeft =
            (Vector2)transform.position - gridWorldSize / 2f;

        // Convert a world position into grid coordinates.
        int x = Mathf.FloorToInt(
            (worldPosition.x - bottomLeft.x) / cellSize
        );

        int y = Mathf.FloorToInt(
            (worldPosition.y - bottomLeft.y) / cellSize
        );

        return new Vector2Int(x, y);
    }


    public bool IsWalkable(int x, int y)
    {
        if (walkableGrid == null)
            return false;

        // Check whether coordinates are inside the grid.
        if (x < 0 || y < 0 ||
            x >= gridWidth || y >= gridHeight)
        {
            return false;
        }

        return walkableGrid[x, y];
    }

    public bool TryGetRandomWalkablePosition(out Vector2 position)
    {
        position = Vector2.zero;

        if (walkableGrid == null)
            return false;

        // Try random cells until we find a walkable one.
        for (int attempt = 0; attempt < 100; attempt++)
        {
            int x = Random.Range(0, gridWidth);
            int y = Random.Range(0, gridHeight);

            if (IsWalkable(x, y))
            {
                position = GetWorldPosition(x, y);
                return true;
            }
        }

    return false;
}

    
    void OnDrawGizmosSelected()
    {
        if (!showGrid || cellSize <= 0f)
            return;

        if (mansionCollider == null)
            return;

        int width = Mathf.CeilToInt(
            gridWorldSize.x / cellSize
        );

        int height = Mathf.CeilToInt(
            gridWorldSize.y / cellSize
        );

        // Visualize every cell in the Scene view.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = GetWorldPosition(x, y);

                // Check the mansion boundary.
                bool insideMansion =
                    mansionCollider.OverlapPoint(position);

                // Check for wall collisions.
                bool blockedByWall = Physics2D.OverlapCircle(
                    position,
                    obstacleCheckRadius,
                    wallLayer
                ) != null;

                bool walkable =
                    insideMansion && !blockedByWall;

                // Green = walkable, Red = blocked.
                Gizmos.color = walkable
                    ? new Color(0f, 1f, 0f, 0.2f)
                    : new Color(1f, 0f, 0f, 0.6f);

                Gizmos.DrawCube(
                    position,
                    Vector3.one * (cellSize * 0.85f)
                );
            }
        }
    }
}
