using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))] // ✅ Changed from CircleCollider2D
public class SharedPathFollower : MonoBehaviour, IGetBlastTank
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float stopThreshold = 0.1f;
    public float repathThreshold = 0.5f;
    public float pathUpdateInterval = 0.25f;
    public float rotationSpeed = 720f;
    public bool isAllowRot;

    [Header("Patrol Settings")]
    public List<Transform> patrolPoints;
    public int patrolIndex = 0;

    [Header("Detection Settings")]
    public Transform player;
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
    public bool playerDetected = false;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isCollidingWithPlayer = false;
    private CapsuleCollider2D myCollider; // ✅ Changed

    public bool isPatrolDoor;
    public GameObject TargetLocked;

    [Header("Health")]
    public int Health = 3;
    public int maxHealth = 3;

    public GameObject HealthParent;
    public Image FillHealth;
    private Coroutine hideHealthCoroutine;

    Vector3 destination;

    public float defaultSpeed;
    public float maxSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider2D>();

        // ✅ Setup Capsule Collider
        myCollider.direction = CapsuleDirection2D.Vertical;
        myCollider.isTrigger = false;

        // ✅ Reset Rigidbody
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        defaultSpeed = Random.Range(0.25f, 0.75f);
    }

    private void Start()
    {
        UpdateHealthUI();
        HealthParent.SetActive(false);

        if (PlayerMovements.Instance != null)
            player = PlayerMovements.Instance.transform;

        if (!isPatrolDoor)
        {
            patrolPoints = new List<Transform>(Game_Manager.Instance.PatrolPoints);
            ShuffleList(patrolPoints);
        }

        lastTargetPosition = patrolPoints[patrolIndex].position;
        StartCoroutine(UpdatePathRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
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
                //speed = 1.5f;
                speed = maxSpeed;
                pathUpdateInterval = 0.15f;
            }
            else
            {
                isChasingPlayer = false;
                destination = patrolPoints[patrolIndex].position;
                //speed = 0.5f;
                speed = defaultSpeed;
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

    private void FixedUpdate()
    {
        if (isCollidingWithPlayer) return;
        if (!isFollowing || path == null || currentIndex >= path.Count) return;

        Vector3 targetPoint = path[currentIndex];
        Vector3 direction = (targetPoint - transform.position).normalized;

        rb.MovePosition(rb.position + (Vector2)(direction * speed * Time.fixedDeltaTime));

        if (direction != Vector3.zero && isAllowRot)
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

            Vector3 dirToPlayer = (collision.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0, 0, angle + 180f);

            StartCoroutine(RotateThenAttack(targetRot));

            if (!playerDetected)
            {
                playerDetected = true;
                playerLostTime = Time.time + 10f;
            }
        }
        else if (collision.collider.CompareTag("enemy"))
        {
            if(!playerDetected)
            {
                SharedPathFollower otherEnemy = collision.collider.GetComponent<SharedPathFollower>();
                if (otherEnemy == null) return;

                // ✅ Check if facing opposite
                Vector2 myDir = transform.right;
                Vector2 otherDir = otherEnemy.transform.right;

                if (Vector2.Dot(myDir, otherDir) < -0.8f)
                {
                    // Shuffle patrols for both enemies
                    ShuffleList(patrolPoints);
                    patrolIndex = 0;

                    otherEnemy.ShuffleList(otherEnemy.patrolPoints);
                    otherEnemy.patrolIndex = 0;
                }

                // ✅ Push slightly apart to avoid overlap
                Vector2 pushDir = (transform.position - collision.transform.position).normalized;
                rb.MovePosition(rb.position + pushDir * 0.2f);
                CancelInvoke("Freeze");
                Invoke("Freeze", 4f);
            }
            
            
        }
    }

    void Freeze()
    {
        rb.freezeRotation = true;
        rb.freezeRotation = false;
    }

    private IEnumerator RotateThenAttack(Quaternion targetRot)
    {
        while (Quaternion.Angle(transform.rotation, targetRot) > 5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetTrigger("Attack");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bomb"))
        {
            GameObject blood = Instantiate(Game_Manager.Instance.BloodPrefab, transform.position, Quaternion.identity);
            Game_Manager.Instance.Enemies.Remove(this);
            Destroy(gameObject);
            Game_Manager.Instance.EnemyCount();
        }
        else if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            Health--;

            if (Health <= 0)
            {
                GameObject blood = Instantiate(Game_Manager.Instance.BloodPrefab, transform.position, Quaternion.identity);
                Game_Manager.Instance.Enemies.Remove(this);
                Destroy(gameObject);
                Game_Manager.Instance.EnemyCount();
                return;
            }

            UpdateHealthUI();
            HealthParent.SetActive(true);

            if (hideHealthCoroutine != null)
                StopCoroutine(hideHealthCoroutine);

            hideHealthCoroutine = StartCoroutine(HideHealthAfterDelay());

            if (!playerDetected)
            {
                playerDetected = true;
                playerLostTime = Time.time + 10f;
            }
        }
    }

    void UpdateHealthUI()
    {
        FillHealth.fillAmount = (float)Health / maxHealth;
    }

    IEnumerator HideHealthAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        HealthParent.SetActive(false);
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

    public void ShuffleList(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Transform temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void Attack()
    {
        if (isCollidingWithPlayer)
        {
            Game_Manager.Instance.PlayerHealthCount -= 1;
            Game_Manager.Instance.UpdatePlayerHealth();
        }
    }

    public void TankBlastUpdate()
    {
        Health -= 3;

        if (Health <= 0)
        {
            GameObject blood = Instantiate(Game_Manager.Instance.BloodPrefab, transform.position, Quaternion.identity);
            Game_Manager.Instance.Enemies.Remove(this);
            Destroy(gameObject);
            Game_Manager.Instance.EnemyCount();
            return;
        }

        UpdateHealthUI();
        HealthParent.SetActive(true);

        if (hideHealthCoroutine != null)
            StopCoroutine(hideHealthCoroutine);

        hideHealthCoroutine = StartCoroutine(HideHealthAfterDelay());
    }
}
