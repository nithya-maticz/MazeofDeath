using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    public Transform target;
    public TMP_Text enemyHealthTxt;
    public int enemyHealth;
    private int currentHealth;

    [Header("References")]
    NavMeshAgent Agent;
    public Animator animator;

    [Header("Player Interaction")]
    public bool attackPlayer;
    public bool Door;
    public bool playerDeath;
    public bool temp;
    private bool _enemyAttack;
    private Coroutine healthCoroutine;

    private Quaternion targetRotation;
    public float rotationSpeed = 360f;
    private bool playerInRange = false;

    [Header("Patrol Settings")]
    public List<Vector3> assignedWaypoints = new List<Vector3>();
    private int currentWaypointIndex = 0;
    public float patrolWaitTime = 1f;
    public float checkInterval = 1f;
    private bool waitingAtWaypoint = false;
    private bool isPatrolling = true;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        currentHealth = enemyHealth;

        if (assignedWaypoints.Count > 0)
        {
            isPatrolling = true;
            currentWaypointIndex = 0;
            Agent.SetDestination(assignedWaypoints[currentWaypointIndex]);
            StartCoroutine(PatrolWaypoints());
        }
    }

    void Update()
    {
        if (playerInRange && target != null)
        {
            Agent.SetDestination(target.position);

            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                {
                    if (healthCoroutine == null)
                    {
                        healthCoroutine = StartCoroutine(ReducePlayerHealth(ManagerMaze.instance.playerRef));
                    }
                }
            }
            else
            {
                if (healthCoroutine != null)
                {
                    StopCoroutine(healthCoroutine);
                    healthCoroutine = null;
                }
            }

            FaceTarget();
        }
        else if (isPatrolling && assignedWaypoints.Count > 0)
        {
            FacePatrolDirection();
        }
    }

    IEnumerator PatrolWaypoints()
    {
        while (isPatrolling && assignedWaypoints.Count > 0)
        {
            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!waitingAtWaypoint)
                {
                    waitingAtWaypoint = true;
                    yield return new WaitForSeconds(patrolWaitTime);

                    currentWaypointIndex = (currentWaypointIndex + 1) % assignedWaypoints.Count;
                    Agent.SetDestination(assignedWaypoints[currentWaypointIndex]);
                    waitingAtWaypoint = false;
                }
            }

            yield return null;
        }
    }

    void FaceTarget()
    {
        if (ManagerMaze.instance.playerRef != null)
        {
            Vector2 direction = ManagerMaze.instance.playerRef.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
    }

    void FacePatrolDirection()
    {
        if (Agent.hasPath && Agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = Agent.velocity;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
    }

    public void damageEnemy(int damage)
    {
        currentHealth -= damage;
        enemyHealthTxt.text = currentHealth.ToString();
    }

    IEnumerator ReducePlayerHealth(Player _player)
    {
        Player player = _player;

        while (player.playerHealth > 0)
        {
            float duration = 2f;
            float elapsed = 0f;
            float startHealth = player.playerHealth;
            float targetHealth = player.playerHealth - 0.1f;
            ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.red;

            targetHealth = Mathf.Max(0, targetHealth);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                player.playerHealth = Mathf.Lerp(startHealth, targetHealth, t);
                player.playerHealthFill.fillAmount = player.playerHealth;
                yield return null;
            }

            if (player.playerHealth < 0.1f)
            {
                player.playerHealth = 0;
                player.playerHealthFill.fillAmount = 0;
                playerInRange = false;
                target = null;
                isPatrolling = true;

                Agent.ResetPath();
                if (assignedWaypoints.Count > 0)
                {
                    Agent.SetDestination(assignedWaypoints[currentWaypointIndex]);
                    StartCoroutine(PatrolWaypoints());
                }

                ManagerMaze.instance.GameOver();
                yield break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerRange"))
        {
            playerInRange = true;
            target = Player.Instance.transform;
            isPatrolling = false;
            Agent.ResetPath();
            StopCoroutine(PatrolWaypoints());
        }

        if (collision.CompareTag("attack"))
        {
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.white;
                healthCoroutine = null;
            }
            Invoke(nameof(DestryInstance), 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerRange"))
        {
            playerInRange = false;
            target = null;

            isPatrolling = true;
            currentWaypointIndex = 0;

            if (assignedWaypoints.Count > 0)
            {
                Agent.SetDestination(assignedWaypoints[currentWaypointIndex]);
                StartCoroutine(PatrolWaypoints());
            }

            if (healthCoroutine != null)
            {
                ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.white;
                StopCoroutine(healthCoroutine);
                healthCoroutine = null;
            }
        }
    }

    void DestryInstance()
    {
        GameObject blood = Instantiate(ManagerMaze.instance.BloodPrefab.gameObject, transform);
        blood.transform.SetParent(ManagerMaze.instance.BloodTransform);
        blood.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
    }

    public void idleFun()
    {
        animator.SetTrigger("idle");
    }
}
