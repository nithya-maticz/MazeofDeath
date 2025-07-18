using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPathfinding : MonoBehaviour
{
    [Header("References")]
    public Grid grid;
    public Tilemap wallTilemap;

    [Header("Debug")]
    public bool drawGizmos = true;
    public List<Vector3> lastPath;

    // Internal Node class
    public class Node
    {
        public Vector3Int cellPosition;
        public bool walkable;
        public int gCost, hCost;
        public Node parent;

        public int fCost => gCost + hCost;

        public Node(Vector3Int cellPosition, bool walkable)
        {
            this.cellPosition = cellPosition;
            this.walkable = walkable;
        }
    }

    /// <summary>
    /// Coroutine-based pathfinding using A*
    /// </summary>
    public IEnumerator FindPathAsync(Vector3 startWorld, Vector3 targetWorld, System.Action<List<Vector3>> callback)
    {
        Vector3Int startCell = grid.WorldToCell(startWorld);
        Vector3Int targetCell = grid.WorldToCell(targetWorld);

        Dictionary<Vector3Int, Node> allNodes = new();
        HashSet<Vector3Int> closedSet = new();
        PriorityQueue<Node> openSet = new();

        Node startNode = new Node(startCell, true);
        Node targetNode = new Node(targetCell, true);

        allNodes[startCell] = startNode;
        openSet.Enqueue(startNode, 0);

        int nodesChecked = 0;

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Dequeue();
            closedSet.Add(currentNode.cellPosition);

            if (currentNode.cellPosition == targetCell)
            {
                var path = RetracePath(startNode, currentNode);
                lastPath = path;
                callback?.Invoke(path);
                yield break;
            }

            foreach (Vector3Int dir in GetNeighbourDirections())
            {
                Vector3Int neighbourPos = currentNode.cellPosition + dir;
                if (closedSet.Contains(neighbourPos) || !IsWalkable(neighbourPos)) continue;

                int tentativeG = currentNode.gCost + GetDistance(currentNode.cellPosition, neighbourPos);

                if (!allNodes.TryGetValue(neighbourPos, out Node neighbour))
                {
                    neighbour = new Node(neighbourPos, true);
                    allNodes[neighbourPos] = neighbour;
                }

                if (tentativeG < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = tentativeG;
                    neighbour.hCost = GetDistance(neighbourPos, targetCell);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Enqueue(neighbour, neighbour.fCost);
                }
            }

            if (++nodesChecked % 10 == 0)
                yield return null;
        }

        callback?.Invoke(null);
    }

    /// <summary>
    /// Returns the final world-space path
    /// </summary>
    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(grid.GetCellCenterWorld(current.cellPosition));
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Returns true if the tile is walkable
    /// </summary>
    private bool IsWalkable(Vector3Int cellPos)
    {
        return !wallTilemap.HasTile(cellPos);
    }

    /// <summary>
    /// Returns 8-directional movement offsets
    /// </summary>
    private List<Vector3Int> GetNeighbourDirections()
    {
        return new List<Vector3Int>
        {
            new Vector3Int( 0,  1, 0), // Up
            new Vector3Int( 1,  0, 0), // Right
            new Vector3Int( 0, -1, 0), // Down
            new Vector3Int(-1,  0, 0), // Left
            new Vector3Int( 1,  1, 0), // Up-Right
            new Vector3Int( 1, -1, 0), // Down-Right
            new Vector3Int(-1, -1, 0), // Down-Left
            new Vector3Int(-1,  1, 0), // Up-Left
        };
    }

    /// <summary>
    /// Returns diagonal-aware cost between two cells
    /// </summary>
    private int GetDistance(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy);
    }

    /// <summary>
    /// Draw last calculated path in scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawGizmos || lastPath == null) return;

        Gizmos.color = Color.green;
        foreach (Vector3 point in lastPath)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
