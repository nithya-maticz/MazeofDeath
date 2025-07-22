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

        // Check only rows: 0, -1, -2 (behind the enemy)
        for (int depth = 0; depth <= 2; depth++)
        {
            Vector3Int rowOffset = -forward * depth;

            for (int i = -1; i <= 1; i++) // left (-1), center (0), right (+1)
            {
                Vector3Int checkCell = enemyCell + rowOffset + side * i;

                // Skip if blocked (not green)
                if (obstacleTilemap.HasTile(checkCell))
                    continue;

                // ✅ Only detect if player is on green cell
                if (checkCell == playerCell)
                    return true;
            }
        }

        return false;
    }








    private void OnDrawGizmosSelected()
    {
        // Draw path
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
                Gizmos.DrawSphere(path[i], 0.1f);
            }
        }

        // Draw patrol points
        Gizmos.color = Color.green;
        foreach (Transform point in patrolPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.2f);
        }

        // Draw 3 back rows of grid (rows 0, -1, -2)
        if (obstacleTilemap != null)
        {
            Vector3Int enemyCell = obstacleTilemap.WorldToCell(transform.position);
            Vector3 direction = transform.right.normalized;
            Vector3Int forward = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
            Vector3Int side = new Vector3Int(Mathf.RoundToInt(perpendicular.x), Mathf.RoundToInt(perpendicular.y), 0);

            for (int depth = 0; depth <= 2; depth++)
            {
                Vector3Int rowOffset = -forward * depth; // rows: 0, -1, -2

                for (int i = -1; i <= 1; i++)
                {
                    Vector3Int cell = enemyCell + rowOffset + side * i;
                    Vector3 center = obstacleTilemap.GetCellCenterWorld(cell);

                    // Color based on obstacle presence
                    if (obstacleTilemap.HasTile(cell))
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = Color.green;

                    Gizmos.DrawWireCube(center, Vector3.one * 0.9f);

#if UNITY_EDITOR
                    // Draw cell coordinate
                    Handles.color = Color.white;
                    Handles.Label(center + Vector3.up * 0.2f, cell.ToString());
#endif
                }
            }
        }
    }







}
