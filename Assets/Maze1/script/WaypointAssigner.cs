using System.Collections.Generic;
using UnityEngine;

public class WaypointAssigner : MonoBehaviour
{
    public MeshWaypointGenerator2D waypointGenerator;
    public float minWaypointSpacing = 3f; 

    public static WaypointAssigner instance;

    private void Awake()
    {
        instance = this;
    }

    public void AssignWaypoints(List<Enemy> enemies)
    {
        List<Vector3> availableWaypoints = new List<Vector3>(waypointGenerator.waypoints);
        int totalWaypoints = availableWaypoints.Count;
        int enemyCount = enemies.Count;

        if (enemyCount == 0 || totalWaypoints == 0)
        {
            Debug.LogWarning("No enemies or waypoints to assign.");
            return;
        }

        List<Vector3> usedWaypoints = new List<Vector3>();
        int waypointsPerEnemy = Mathf.Max(1, totalWaypoints / enemyCount);

        foreach (var enemy in enemies)
        {
            enemy.assignedWaypoints.Clear();

            int attempts = 0;
            while (enemy.assignedWaypoints.Count < waypointsPerEnemy && availableWaypoints.Count > 0 && attempts < 100)
            {
                int index = Random.Range(0, availableWaypoints.Count);
                Vector3 candidate = availableWaypoints[index];

                bool tooClose = false;
                foreach (var used in usedWaypoints)
                {
                    if (Vector3.Distance(candidate, used) < minWaypointSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    enemy.assignedWaypoints.Add(candidate);
                    usedWaypoints.Add(candidate);
                    availableWaypoints.RemoveAt(index);
                }
                else
                {
                    attempts++;
                }
            }

            if (enemy.assignedWaypoints.Count == 0)
            {
                Debug.LogWarning($"Enemy {enemy.name} did not get any valid spaced-out waypoints.");
            }
        }

        Debug.Log("Spaced random waypoints assigned to all enemies.");
    }
}
