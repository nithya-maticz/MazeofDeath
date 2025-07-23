using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Animator))]
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
    public float detectionRange = 5f;
    public float rearViewAngle = 90f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    private List<Vector3> path;
    private int currentIndex = 0;
    private bool isFollowing = false;
    private bool isChasingPlayer = false;
    private Vector3 lastTargetPosition;
    private float playerLostTime;
    [SerializeField] bool playerDetected = false;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isCollidingWithPlayer = false;

    private void Start()
    {
        patrolPoints = new List<Transform>(Game_Manager.Instance.PatrolPoints);
        ShuffleList(patrolPoints);

        lastTargetPosition = patrolPoints[patrolIndex].position;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            Vector3 destination;
            bool visible = IsPlayerVisibleInBack();

            if (visible)
            {
                playerDetected = true;
                playerLostTime = Time.time + 10f;
            }
            else if (Time.time > playerLostTime)
            {
                playerDetected = false;
            }

            if (isCollidingWithPlayer)
            {
                destination = transform.position;
                isFollowing = false;
                speed = 0f;
            }
            else if (playerDetected)
            {
                isChasingPlayer = true;
                destination = player.position;
                speed = 1f;
                pathUpdateInterval = 0.15f;
            }
            else
            {
                isChasingPlayer = false;
                destination = patrolPoints[patrolIndex].position;
                speed = 0.5f;
                pathUpdateInterval = 0.25f;
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
        if (newPath == null || newPath.Count == 0) return;
        path = newPath;
        currentIndex = 0;
        isFollowing = true;
    }

    private void Update()
    {
        if (isCollidingWithPlayer) return;
        if (!isFollowing || path == null || currentIndex >= path.Count) return;

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

    public bool IsPlayerVisibleInBack()
    {
        if (player == null) return false;

        Vector3 origin = transform.position;
        Vector3 toPlayer = player.position - origin;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > detectionRange) return false;

        Vector3 dirToPlayer = toPlayer.normalized;
        Vector3 back = -transform.right;
        float angle = Vector3.Angle(back, dirToPlayer);
        if (angle > rearViewAngle * 0.5f) return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, dirToPlayer, detectionRange, playerMask | obstacleMask);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }


    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.SetTrigger("Attack");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            animator.SetTrigger("Walk");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector3 origin = transform.position;
        Vector3 back = -transform.right;

        Gizmos.color = Color.yellow;
        Quaternion leftLimit = Quaternion.Euler(0, 0, rearViewAngle * 0.5f);
        Quaternion rightLimit = Quaternion.Euler(0, 0, -rearViewAngle * 0.5f);

        Vector3 leftDir = leftLimit * back;
        Vector3 rightDir = rightLimit * back;

        Gizmos.DrawRay(origin, leftDir * detectionRange);
        Gizmos.DrawRay(origin, rightDir * detectionRange);

        Gizmos.color = Color.cyan;
        Vector3 toPlayer = player.position - origin;
        if (toPlayer.magnitude <= detectionRange)
            Gizmos.DrawRay(origin, toPlayer.normalized * toPlayer.magnitude);
    }

    void ShuffleList(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Transform temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
