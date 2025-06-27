using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class MeshWaypointGenerator2D : MonoBehaviour
{
    public float sampleRadius = 0.3f;
    public float bufferDistance = 1.0f;               // Space required from NavMesh edges
    public float minDistanceBetweenWaypoints = 2.0f;  // Minimum distance between two waypoints
    public int maxWaypoints = 25;
    public int maxAttempts = 500;

    [ReadOnly] public int generatedCount;
    public List<Vector3> waypoints = new List<Vector3>();

    private void Awake()
    {
        GenerateWaypoints();
    }

    public void GenerateWaypoints()
    {
        waypoints.Clear();

        var navMeshData = NavMesh.CalculateTriangulation();
        if (navMeshData.vertices.Length == 0)
        {
            Debug.LogWarning("NavMesh not available.");
            return;
        }

        // Calculate bounds of baked navmesh
        Bounds bounds = new Bounds(navMeshData.vertices[0], Vector3.zero);
        foreach (var v in navMeshData.vertices)
            bounds.Encapsulate(v);

        int attempts = 0;
        int count = 0;

        while (count < maxWaypoints && attempts < maxAttempts)
        {
            attempts++;

            // Random point in NavMesh bounds
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                0f
            );

            // Sample to find actual NavMesh point
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
            {
                Vector3 candidate = hit.position;

                // Check buffer from edges and from other waypoints
                if (HasClearBufferAround(candidate, bufferDistance) && !IsTooCloseToOtherPoints(candidate, minDistanceBetweenWaypoints))
                {
                    waypoints.Add(candidate);
                    count++;
                }
            }
        }

        generatedCount = count;
        Debug.Log($"Generated {count} waypoints with buffer & spacing.");
    }

    private bool HasClearBufferAround(Vector3 point, float buffer)
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.right, Vector3.left, Vector3.up, Vector3.down,
            (Vector3.right + Vector3.up).normalized,
            (Vector3.right + Vector3.down).normalized,
            (Vector3.left + Vector3.up).normalized,
            (Vector3.left + Vector3.down).normalized
        };

        foreach (var dir in directions)
        {
            if (NavMesh.Raycast(point, point + dir * buffer, out NavMeshHit hit, NavMesh.AllAreas))
                return false; // Edge is too close
        }

        return true;
    }

    private bool IsTooCloseToOtherPoints(Vector3 candidate, float minDistance)
    {
        foreach (var wp in waypoints)
        {
            if (Vector3.Distance(candidate, wp) < minDistance)
                return true; // Too close to another waypoint
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = Color.cyan;
        foreach (var point in waypoints)
            Gizmos.DrawSphere(point, 0.25f);

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);

        if (waypoints.Count > 2)
            Gizmos.DrawLine(waypoints[waypoints.Count - 1], waypoints[0]);
    }
}
