using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedPathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    public float speed = 3f;
    public float stopThreshold = 0.1f;

    [Header("Patrol Settings")]
    public List<Transform> patrolPoints;
    public float rotateDuration = 1f;

    [Header("Debug Info (Read Only)")]
    [SerializeField] private Transform currentPatrolTarget;
    [SerializeField] private Vector3 currentTargetPosition;

    private int currentPointIndex = 0;
    private List<Vector3> path;
    private int currentIndex = 0;
    private bool isFollowing = false;

    private void Start()
    {
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.3f)); // Staggered start

        while (true)
        {
            currentPatrolTarget = patrolPoints[currentPointIndex];

            Vector3 start = transform.position;
            Vector3 end = currentPatrolTarget.position;

            bool pathSet = false;

            PathManager.Instance.RequestPath(start, end, (result) =>
            {
                if (result != null && result.Count > 0)
                {
                    SetSharedPath(result);
                    pathSet = true;
                }
                else
                {
                    Debug.LogWarning($"Path not found from {start} to {end}.");
                }
            });

            yield return new WaitUntil(() => pathSet);

            // Wait until movement completes
            yield return new WaitUntil(() => !isFollowing);

            // Rotate 180°
            yield return Rotate180();

            // Next patrol point
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
        }
    }

    private IEnumerator Rotate180()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 180f);
        float elapsed = 0f;

        while (elapsed < rotateDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
    }

    public void SetSharedPath(List<Vector3> sharedPath)
    {
        path = sharedPath;
        currentIndex = 0;
        isFollowing = true;
        currentTargetPosition = (path != null && path.Count > 0) ? path[0] : Vector3.zero;
    }

    private void Update()
    {
        if (!isFollowing || path == null || currentIndex >= path.Count)
            return;

        Vector3 targetPoint = path[currentIndex];
        currentTargetPosition = targetPoint; // Update debug info

        Vector3 direction = targetPoint - transform.position;

        if (direction.sqrMagnitude < stopThreshold * stopThreshold)
        {
            currentIndex++;
            if (currentIndex >= path.Count)
            {
                isFollowing = false;
                return;
            }
        }

        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
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
