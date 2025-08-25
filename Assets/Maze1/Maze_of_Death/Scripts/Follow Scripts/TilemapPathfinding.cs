using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPathfinding : MonoBehaviour
{
    public static TilemapPathfinding instance;
    [Header("References")]
    public Grid grid;
    public Tilemap wallTilemap;

    [Header("Blocking Data")]
    // 👇 assign prefab-blocked positions here (e.g. from your LevelController)
    public List<Vector2Int> blockedPrefabCells = new();

    [Header("Debug")]
    public bool drawGizmos = true;
    public List<Vector3> lastPath;

    private void Awake()
    {
        instance = this;
    }

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

                // Skip if not walkable or already evaluated
                if (closedSet.Contains(neighbourPos) || !IsWalkable(neighbourPos))
                    continue;

                // ❌ Prevent corner cutting
                if (Mathf.Abs(dir.x) == 1 && Mathf.Abs(dir.y) == 1)
                {
                    Vector3Int horizontal = new Vector3Int(currentNode.cellPosition.x + dir.x, currentNode.cellPosition.y, 0);
                    Vector3Int vertical = new Vector3Int(currentNode.cellPosition.x, currentNode.cellPosition.y + dir.y, 0);

                    if (!IsWalkable(horizontal) || !IsWalkable(vertical))
                        continue;
                }

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
    /// (Checks both wall tiles and prefab-blocked cells)
    /// </summary>
    private bool IsWalkable(Vector3Int cellPos)
    {
        // 1. Walls block
        if (wallTilemap.HasTile(cellPos))
            return false;

        // 2. Prefab-blocked grid positions block
        Vector2Int pos2D = new Vector2Int(cellPos.x, cellPos.y);
        if (blockedPrefabCells.Contains(pos2D))
            return false;

        return true;
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

      /*  // 🔴 Draw blocked prefab cells too
        // Gizmos.color = Color.red;
        foreach (var cell in blockedPrefabCells)
        {
            Vector3 world = grid.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
            Gizmos.DrawCube(world, Vector3.one * 0.9f);
        }*/
    }

    /// <summary>
    /// Loads placed prefab positions from LevelController into blockedPrefabCells
    /// Handles multi-cell prefabs (size + rotation)
    /// </summary>
    public void LoadBlockedPrefabs()
    {
        blockedPrefabCells.Clear();

        if (ApiManager.Instance != null && ApiManager.Instance.levelData != null)
        {
            foreach (var prefab in ApiManager.Instance.levelData.data.prefabs)
            {
                // if no size info, fallback to 1x1
                Vector2Int size = prefab.size == Vector2Int.zero ? Vector2Int.one : prefab.size;

                // handle prefab rotation (only multiples of 90° supported)
                int rot = prefab.rotation % 360;

                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        Vector2Int local = new Vector2Int(x, y);
                        Vector2Int rotated = local;

                        // rotate local coords
                        switch (rot)
                        {
                            case 90: rotated = new Vector2Int(-local.y, local.x); break;
                            case 180: rotated = new Vector2Int(-local.x, -local.y); break;
                            case 270: rotated = new Vector2Int(local.y, -local.x); break;
                        }

                        Vector2Int pos = prefab.position + rotated;
                        if (!blockedPrefabCells.Contains(pos))
                            blockedPrefabCells.Add(pos);
                    }
                }
            }
        }
    }
}
