using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<Vector3> path;
    private int currentIndex = 0;
    private bool isFollowing = false;

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
            Transform destination = patrolPoints[patrolIndex];

            if (!isFollowing || Vector3.Distance(destination.position, lastTargetPosition) > repathThreshold)
            {
                lastTargetPosition = destination.position;
                PathManager.Instance.RequestPath(transform.position, destination.position, OnPathFound);
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

                // Move to next patrol point
                patrolIndex = (patrolIndex + 1) % patrolPoints.Count;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
                Gizmos.DrawSphere(path[i], 0.1f);
            }
        }

        Gizmos.color = Color.green;
        foreach (Transform point in patrolPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.2f);
        }
    }
}
