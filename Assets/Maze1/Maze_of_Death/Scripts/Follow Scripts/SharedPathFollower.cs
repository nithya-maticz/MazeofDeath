using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedPathFollower : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float stopThreshold = 0.1f;
    public float repathThreshold = 0.5f;  // Minimum distance target must move to trigger repath
    public float pathUpdateInterval = 0.25f;

    private List<Vector3> path;
    private int currentIndex = 0;
    private bool isFollowing = false;

    private Vector3 lastTargetPosition;

    private void Start()
    {
        if (target != null)
            lastTargetPosition = target.position;

        StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            if (target != null)
            {
                if (!isFollowing || Vector3.Distance(target.position, lastTargetPosition) > repathThreshold)
                {
                    lastTargetPosition = target.position;
                    PathManager.Instance.RequestPath(transform.position, target.position, OnPathFound);
                }
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

        // Move toward current path point
        transform.position += direction * speed * Time.deltaTime;

        // Face movement direction (with smooth rotation)
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Advance to next point if close enough
        if (Vector3.Distance(transform.position, targetPoint) < stopThreshold)
        {
            currentIndex++;

            if (currentIndex >= path.Count)
            {
                isFollowing = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (path == null || path.Count == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
            Gizmos.DrawSphere(path[i], 0.1f);
        }
    }
}
