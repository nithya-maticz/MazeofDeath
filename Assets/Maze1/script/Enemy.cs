using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
   // public Transform target;
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
    public float reachThreshold = 0.1f;
    private int currentPointIndex = 0;
    public Transform targetPoint;

    [Header("Normal Patrol")]
    public Transform[] partolPoints;

    [Header("Door Patrol")]
    public bool isPatrolDoor;
    public List<Transform> doorPatrolPoints;
    private int currentPoint;
   

    private Coroutine healthCoroutine;
    private Coroutine resetColorCoroutine;


    private void Start()
    {
        animator.SetTrigger("idle");
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = 5f;
        }

        if (ManagerMaze.instance == null || ManagerMaze.instance.partolPoints == null)
        {
            Debug.LogError("ManagerMaze or partolPoints not initialized");
            return;
        }

        partolPoints = new Transform[ManagerMaze.instance.partolPoints.Length];
        ManagerMaze.instance.partolPoints.CopyTo(partolPoints, 0);
        ShuffleArray(partolPoints);

        if (isPatrolDoor && doorPatrolPoints.Count > 0)
        {
            currentPoint = 0;
            targetPoint = doorPatrolPoints[currentPoint];
        }
        else if (partolPoints.Length > 0)
        {
            currentPointIndex = 0;
            targetPoint = partolPoints[currentPointIndex];
        }

        StartCoroutine(CheckRaycastRoutine());


    }

    private void Update()
    {
       
    }

    private void FixedUpdate()
    {
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0f && targetPoint != null)
        {
            agent.SetDestination(targetPoint.position);
            pathUpdateTimer = pathUpdateInterval;
        }

        //RotateTowardsMoveDirection();
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

    void ShuffleArray(Transform[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randIndex = Random.Range(i, array.Length);
            Transform temp = array[i];
            array[i] = array[randIndex];
            array[randIndex] = temp;
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
                targetPoint = Player.Instance.transform;
                agent.speed = 13f;
                
                agent.SetDestination(targetPoint.position);
                ligtColor.GetComponent<Light2D>().color = Color.red;
                //animator.SetTrigger("enemyrun");

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

        agent.speed = 5f;
       
        
        ligtColor.GetComponent<Light2D>().color = new Color(0f, 0.443f, 0.031f, 1f);

        if (isPatrolDoor && doorPatrolPoints.Count > 0)
        {
            targetPoint = doorPatrolPoints[currentPoint];
        }
        else if (partolPoints.Length > 0)
        {
            targetPoint = partolPoints[currentPointIndex];
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
            else if (partolPoints.Length > 0)
            {
                currentPointIndex = (currentPointIndex + 1) % partolPoints.Length;
                targetPoint = partolPoints[currentPointIndex];
            }
        }
    }

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerRange"))
        {
            //Debug.Log("RANGE-----------------");
            
            animator.SetTrigger("enemyattack");
            Vector2 directionToEnemy = (transform.position - collision.transform.position).normalized;
            float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
            collision.transform.rotation = transform.rotation;
           
            Player.Instance.playerSprite.color = Color.red;
            

           
        }
        else if (collision.CompareTag("attack"))
        {
            Player.Instance.playerSprite.color = Color.white;
            Invoke(nameof(DestroySelf), 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerRange"))
        {
            animator.SetTrigger("idle");
            

            Player.Instance.playerSprite.color = Color.white;
          
          
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
