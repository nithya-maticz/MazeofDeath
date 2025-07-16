using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    public Transform target;
    private NavMeshAgent agent;
    public Animator animator;
    public float rotationSpeed = 360f;
    public float checkInterval;
    public float pathUpdateInterval = 0.2f;
    private float pathUpdateTimer;

    public GameObject ligtColor;
    public Sprite newSprite;
    public float newAlpha = 0.5f;

    [Header("RAYCAST")]
    [SerializeField] Transform RaycastParent;
    [SerializeField] float rayLength;
    [SerializeField] int rayCount;
    [SerializeField] float coneAngle;
    [SerializeField] LayerMask raycastLayerMask;

    private bool isChangingColorBack = false;

    [Header("Patrolling")]
    public float speed = 3f;
    public float reachThreshold = 0.1f;
    private int currentPointIndex = 0;
    public Transform targetPoint;

    [Header("Door Patrol")]
    public bool isPatrolDoor;
    public List<Transform> doorPatrolPoints;
    private int currentPoint;

    private Coroutine healthCoroutine;
    private Coroutine resetColorCoroutine;

    private void Awake()
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = speed;
        }

        if (isPatrolDoor && doorPatrolPoints.Count > 0)
        {
            currentPoint = 0;
            targetPoint = doorPatrolPoints[currentPoint];
        }
        else if (ManagerMaze.instance.partolPoints.Length > 0)
        {
            currentPointIndex = 0;
            targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
        }

        StartCoroutine(CheckRaycastRoutine());
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;

        

        if (isPatrolDoor && doorPatrolPoints.Count > 0)
        {
            currentPoint = 0;
            targetPoint = doorPatrolPoints[currentPoint];
        }
        else if (ManagerMaze.instance.partolPoints.Length > 0)
        {
            currentPointIndex = 0;
            targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
        }

        
    }

    private void Update()
    {
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0f && targetPoint != null)
        {
            agent.SetDestination(targetPoint.position);
            pathUpdateTimer = pathUpdateInterval;
        }

        RotateTowardsMoveDirection();
        CheckReachTarget();
    }

    private IEnumerator CheckRaycastRoutine()
    {
        var wait = new WaitForSeconds(checkInterval);
        while (true)
        {
            DoRaycastLogic();
            yield return wait;
        }
    }

    private void DoRaycastLogic()
    {
        Vector2 centerDir = -RaycastParent.up;
        bool playerDetected = false;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = Mathf.Lerp(-coneAngle / 2, coneAngle / 2, (float)i / (rayCount - 1));
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * centerDir;

            RaycastHit2D hit = Physics2D.Raycast(RaycastParent.position, dir, rayLength, raycastLayerMask);

            Color debugColor = (hit.collider != null) ? Color.red : Color.green;
            Debug.DrawLine(RaycastParent.position, RaycastParent.position + (Vector3)(dir * rayLength), debugColor);

            if (hit.collider != null && (hit.collider.CompareTag("Player") || hit.collider.name == "Range"))
            {
                playerDetected = true;
                ligtColor.GetComponent<Light2D>().color = Color.red;

                if (targetPoint != Player.Instance.transform)
                {
                    targetPoint = Player.Instance.transform;
                    agent.speed = 10f;
                    agent.SetDestination(targetPoint.position);
                }

                // Reset and restart the 10-second timer
                if (resetColorCoroutine != null)
                    StopCoroutine(resetColorCoroutine);
                resetColorCoroutine = StartCoroutine(ResetColorAfterDelay(10f));

                break;
            }
        }

        // Only start timer if player was NOT detected and there is no running coroutine
        if (!playerDetected && resetColorCoroutine == null)
        {
            resetColorCoroutine = StartCoroutine(ResetColorAfterDelay(10f));
        }
    }


    private IEnumerator ResetColorAfterDelay(float delay)
    {
        isChangingColorBack = true;
        yield return new WaitForSeconds(delay);

        agent.speed = speed;
        ligtColor.GetComponent<Light2D>().color = new Color(0f, 0.443f, 0.031f, 1f);

        if (isPatrolDoor && doorPatrolPoints.Count > 0)
        {
            targetPoint = doorPatrolPoints[currentPoint];
        }
        else if (ManagerMaze.instance.partolPoints.Length > 0)
        {
            targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
        }

        agent.SetDestination(targetPoint.position);

        isChangingColorBack = false;
        resetColorCoroutine = null; // mark coroutine finished
    }


    private void RotateTowardsMoveDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 moveDir = agent.velocity.normalized;
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 180f;
            Quaternion targetRot = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void CheckReachTarget()
    {
        if (targetPoint == null) return;

        if (Vector2.Distance(transform.position, targetPoint.position) < reachThreshold)
        {
            if (isPatrolDoor && doorPatrolPoints.Count > 0)
            {
                currentPoint = (currentPoint + 1) % doorPatrolPoints.Count;
                targetPoint = doorPatrolPoints[currentPoint];
            }
            else if (ManagerMaze.instance.partolPoints.Length > 0)
            {
                currentPointIndex = (currentPointIndex + 1) % ManagerMaze.instance.partolPoints.Length;
                targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
            }
        }
    }

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerRange"))
        {
            Vector2 directionToEnemy = (transform.position - collision.transform.position).normalized;
            float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
            collision.transform.rotation = Quaternion.Euler(0, 0, angle);
            animator.SetTrigger("enemyattack");
            ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.red;
            

            if (healthCoroutine == null)
                healthCoroutine = StartCoroutine(ReducePlayerHealth(ManagerMaze.instance.playerRef.GetComponent<Player>()));
        }
        else if (collision.CompareTag("attack"))
        {
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.white;
                healthCoroutine = null;
            }
            Invoke(nameof(DestroySelf), 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerRange"))
        {
            animator.SetTrigger("idle");

            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.white;
                healthCoroutine = null;
            }

            if (isPatrolDoor && doorPatrolPoints.Count > 0)
            {
                targetPoint = doorPatrolPoints[currentPoint];
            }
            else if (ManagerMaze.instance.partolPoints.Length > 0)
            {
                targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
            }
            agent.speed = speed;
        }
    }

    private IEnumerator ReducePlayerHealth(Player player)
    {
        while (player.playerHealth > 0)
        {
            float duration = 2f;
            float elapsed = 0f;
            float startHealth = player.playerHealth;
            float targetHealth = Mathf.Max(0, startHealth - 0.1f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                player.playerHealth = Mathf.Lerp(startHealth, targetHealth, t);
                player.playerHealthFill.fillAmount = player.playerHealth;
                yield return null;
            }

            if (player.playerHealth <= 0.1f)
            {
                player.playerHealth = 0;
                player.playerHealthFill.fillAmount = 0;
                ManagerMaze.instance.GameOver();
                yield break;
            }
        }
    }

    private void DestroySelf()
    {
        GameObject blood = Instantiate(ManagerMaze.instance.BloodPrefab.gameObject, transform);
        blood.transform.SetParent(ManagerMaze.instance.BloodTransform);
        blood.transform.localScale = Vector3.one;
        ManagerMaze.instance.DelayCountEnemies();
        Destroy(gameObject);
    }
}
