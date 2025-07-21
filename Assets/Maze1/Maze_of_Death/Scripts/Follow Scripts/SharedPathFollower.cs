using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedPathFollower : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float stopThreshold = 0.1f;
    public float repathThreshold = 0.5f;
    public float pathUpdateInterval = 0.25f;

    private List<Vector3> path;
    private int currentIndex = 0;
    private bool isFollowing = false;

    private Vector3 lastTargetPosition;
    private WaitForSeconds wait;

    private void Start()
    {
        if (target != null)
            lastTargetPosition = target.position;

        wait = new WaitForSeconds(pathUpdateInterval);
        StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            if (target != null && (!isFollowing || Vector3.Distance(target.position, lastTargetPosition) > repathThreshold))
            {
                lastTargetPosition = target.position;
                PathManager.Instance.RequestPath(transform.position, target.position, OnPathFound);
            }
            yield return wait;
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
        Vector3 direction = (targetPoint - transform.position);

        if (direction.sqrMagnitude < stopThreshold * stopThreshold)
        {
            currentIndex++;
            if (currentIndex >= path.Count)
            {
                isFollowing = false;
            }
            return;
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

   /* private void OnDrawGizmos()
    {
        if (path == null || path.Count == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
            Gizmos.DrawSphere(path[i], 0.1f);
        }
    }*/
}