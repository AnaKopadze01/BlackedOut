
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    [Header("References")]
    public PathfindingGrid grid;

    // A node represents one cell in the navigation grid.
    private class Node
    {
        public Vector2Int position;
        public int gCost;
        public int hCost;
        public Node parent;

        public int FCost => gCost + hCost;

        public Node(Vector2Int position)
        {
            this.position = position;
            gCost = int.MaxValue;
        }
    }

    // Horizontal and vertical movement only.
    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public List<Vector2> FindPath(
        Vector2 startWorld,
        Vector2 targetWorld)
    {
        List<Vector2> path = new List<Vector2>();

        if (grid == null)
        {
            Debug.LogWarning("Pathfinding Grid is not assigned.");
            return path;
        }

        Vector2Int start = grid.GetGridPosition(startWorld);
        Vector2Int target = grid.GetGridPosition(targetWorld);

        // Reject locations outside the walkable grid.
        if (!grid.IsWalkable(start.x, start.y) ||
            !grid.IsWalkable(target.x, target.y))
        {
            return path;
        }

        Dictionary<Vector2Int, Node> nodes =
            new Dictionary<Vector2Int, Node>();

        List<Node> openSet = new List<Node>();
        HashSet<Vector2Int> closedSet =
            new HashSet<Vector2Int>();

        Node startNode = new Node(start);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(start, target);

        nodes[start] = startNode;
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Find the node with the lowest estimated cost.
            Node current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                Node candidate = openSet[i];

                if (candidate.FCost < current.FCost ||
                    (candidate.FCost == current.FCost &&
                     candidate.hCost < current.hCost))
                {
                    current = candidate;
                }
            }

            // Destination reached.
            if (current.position == target)
            {
                return RetracePath(current);
            }

            openSet.Remove(current);
            closedSet.Add(current.position);

            // Explore neighboring grid cells.
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPosition =
                    current.position + direction;

                if (!grid.IsWalkable(
                        neighborPosition.x,
                        neighborPosition.y))
                {
                    continue;
                }

                if (closedSet.Contains(neighborPosition))
                    continue;

                int newCost = current.gCost + 10;

                if (!nodes.TryGetValue(
                        neighborPosition, out Node neighbor))
                {
                    neighbor = new Node(neighborPosition);
                    nodes[neighborPosition] = neighbor;
                }

                if (newCost < neighbor.gCost)
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(
                        neighborPosition, target);

                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No valid route was found.
        return path;
    }

    private int GetDistance(
        Vector2Int a,
        Vector2Int b)
    {
        // Manhattan distance for four-direction movement.
        return 10 * (
            Mathf.Abs(a.x - b.x) +
            Mathf.Abs(a.y - b.y)
        );
    }

    private List<Vector2> RetracePath(Node endNode)
    {
        List<Vector2> path = new List<Vector2>();

        Node current = endNode;

        // Follow the parent nodes back to the start.
        while (current != null)
        {
            path.Add(grid.GetWorldPosition(
                current.position.x,
                current.position.y));

            current = current.parent;
        }

        path.Reverse();
        return path;
    }
}
