using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SharedPathFollower : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float stopThreshold = 0.1f;
    public float repathThreshold = 0.5f;
    public float pathUpdateInterval = 0.25f;
    public float rotationSpeed = 720f;

    [Header("Patrol Settings")]
    public List<Transform> patrolPoints;
    private int patrolIndex = 0;

    [Header("Detection Settings")]
    public Transform player;
    public Tilemap obstacleTilemap;

    private List<Vector3> path;
    private int currentIndex = 0;
    private bool isFollowing = false;
    private bool isChasingPlayer = false;

    private Vector3 lastTargetPosition;

    private void Start()
    {
        lastTargetPosition = patrolPoints[patrolIndex].position;
        StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            Vector3 destination;

            if (IsPlayerVisibleInBack())
            {
                isChasingPlayer = true;
                destination = player.position;
            }
            else
            {
                isChasingPlayer = false;
                destination = patrolPoints[patrolIndex].position;
            }

            if (!isFollowing || Vector3.Distance(destination, lastTargetPosition) > repathThreshold)
            {
                lastTargetPosition = destination;
                PathManager.Instance.RequestPath(transform.position, destination, OnPathFound);
            }

            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

   






    private void OnPathFound(List<Vector3> newPath)
    {
        if (newPath == null || newPath.Count == 0)
            return;

        path = newPath;
        currentIndex = 0;
        isFollowing = true;
    }

    private void Update()
    {
        if (!isFollowing || path == null || currentIndex >= path.Count)
            return;

        Vector3 targetPoint = path[currentIndex];
        Vector3 direction = (targetPoint - transform.position).normalized;

        // Move
        transform.position += direction * speed * Time.deltaTime;

        // Rotate (2D)
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 180f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Reached current path point
        if (Vector3.Distance(transform.position, targetPoint) < stopThreshold)
        {
            currentIndex++;
            if (currentIndex >= path.Count)
            {
                isFollowing = false;

                if (!isChasingPlayer)
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Count;
            }
        }
    }

    private bool IsPlayerVisibleInBack()
    {
        if (player == null || obstacleTilemap == null)
            return false;

        Vector3Int enemyCell = obstacleTilemap.WorldToCell(transform.position);
        Vector3 direction = transform.right.normalized;
        Vector3Int forward = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
        Vector3Int side = new Vector3Int(Mathf.RoundToInt(perpendicular.x), Mathf.RoundToInt(perpendicular.y), 0);

        Vector3Int playerCell = obstacleTilemap.WorldToCell(player.position);

        // Build 3x3 grid from top (row 0) to bottom (row 2)
        Vector3Int[] grid = new Vector3Int[9];
        int index = 0;
        for (int depth = 0; depth <= 2; depth++)
        {
            Vector3Int rowOffset = -forward * depth;
            for (int i = -1; i <= 1; i++)
            {
                grid[index++] = enemyCell + rowOffset + side * i;
            }
        }

        // Build blocked list based on updated rules
        HashSet<Vector3Int> blocked = new HashSet<Vector3Int>();

        if (obstacleTilemap.HasTile(grid[0])) // 1
            blocked.Add(grid[0]);

        if (obstacleTilemap.HasTile(grid[1])) // 2 (player cell)
            blocked.Add(grid[1]);

        if (obstacleTilemap.HasTile(grid[2])) // 3
            blocked.Add(grid[2]);

        if (obstacleTilemap.HasTile(grid[3])) // 4
        {
            blocked.Add(grid[3]);
            blocked.Add(grid[6]); // 7
        }

        if (obstacleTilemap.HasTile(grid[4])) // 5
        {
            blocked.Add(grid[4]);
            blocked.Add(grid[6]); // 7
            blocked.Add(grid[7]); // 8
            blocked.Add(grid[8]); // 9
        }

        if (obstacleTilemap.HasTile(grid[5])) // 6
        {
            blocked.Add(grid[5]);
            blocked.Add(grid[8]); // 9
        }

        if (obstacleTilemap.HasTile(grid[6])) // 7
            blocked.Add(grid[6]);

        if (obstacleTilemap.HasTile(grid[7])) // 8
            blocked.Add(grid[7]);

        if (obstacleTilemap.HasTile(grid[8])) // 9
            blocked.Add(grid[8]);

        // Check if player is on a visible tile
        for (int i = 0; i < 9; i++)
        {
            if (grid[i] == playerCell && !blocked.Contains(grid[i]))
                return true;
        }

        return false;
    }










    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (obstacleTilemap == null)
            return;

        Vector3Int enemyCell = obstacleTilemap.WorldToCell(transform.position);
        Vector3 direction = transform.right.normalized;
        Vector3Int forward = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
        Vector3Int side = new Vector3Int(Mathf.RoundToInt(perpendicular.x), Mathf.RoundToInt(perpendicular.y), 0);

        Vector3Int playerCell = player != null ? obstacleTilemap.WorldToCell(player.position) : Vector3Int.zero;

        // Build 3x3 grid (row-wise)
        Vector3Int[] grid = new Vector3Int[9];
        int index = 0;
        for (int depth = 0; depth <= 2; depth++)
        {
            Vector3Int rowOffset = -forward * depth;
            for (int i = -1; i <= 1; i++)
            {
                Vector3Int checkCell = enemyCell + rowOffset + side * i;
                grid[index++] = checkCell;
            }
        }

        // Blocking propagation based on new logic
        HashSet<Vector3Int> blocked = new HashSet<Vector3Int>();

        if (obstacleTilemap.HasTile(grid[0])) // 1
            blocked.Add(grid[0]);

        if (obstacleTilemap.HasTile(grid[1])) // 2 (player cell)
            blocked.Add(grid[1]);

        if (obstacleTilemap.HasTile(grid[2])) // 3
            blocked.Add(grid[2]);

        if (obstacleTilemap.HasTile(grid[3])) // 4
        {
            blocked.Add(grid[3]);
            blocked.Add(grid[6]); // 7
        }

        if (obstacleTilemap.HasTile(grid[4])) // 5
        {
            blocked.Add(grid[4]);
            blocked.Add(grid[6]); // 7
            blocked.Add(grid[7]); // 8
            blocked.Add(grid[8]); // 9
        }

        if (obstacleTilemap.HasTile(grid[5])) // 6
        {
            blocked.Add(grid[5]);
            blocked.Add(grid[8]); // 9
        }

        if (obstacleTilemap.HasTile(grid[6])) // 7
            blocked.Add(grid[6]);

        if (obstacleTilemap.HasTile(grid[7])) // 8
            blocked.Add(grid[7]);

        if (obstacleTilemap.HasTile(grid[8])) // 9
            blocked.Add(grid[8]);

        // Draw each cell
        for (int i = 0; i < 9; i++)
        {
            Vector3Int cell = grid[i];
            Vector3 center = obstacleTilemap.GetCellCenterWorld(cell);

            if (cell == playerCell)
                Gizmos.color = Color.blue;
            else if (blocked.Contains(cell))
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawWireCube(center, Vector3.one * 0.9f);
            Handles.Label(center + Vector3.up * 0.2f, $"[{i + 1}] {cell}");
        }
#endif
    }










}
