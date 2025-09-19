using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class SharedPathFollower : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float stopThreshold = 0.1f;
    public float repathThreshold = 0.5f;
    public float pathUpdateInterval = 0.25f;
    public float rotationSpeed = 720f;
    //public bool isAllowRot;

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
    public float playerLostTime;
    public bool playerDetected = false;

    public Animator animator;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CapsuleCollider2D myCollider;

    public bool isCollidingWithPlayer = false;

    [Header("Patrol Door")]
    public bool isPatrolDoor;


    public GameObject TargetLocked;

    private Vector3 destination;

    public float defaultSpeed;
    public float maxSpeed;

    public PatrolAreas patrolArea;
    public bool playerInRange;

    public float rotationSampleInterval = 0.06f;
    public float rotationAngleThreshold = 1f;
    public float predictionTime = 0.08f;
   
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider2D>();

        if (isPatrolDoor)
            defaultSpeed = 0.5f;
        else
            defaultSpeed = 0.5f;
    }

    private void Start()
    {
       

        if (PlayerMovements.Instance != null)
            player = PlayerMovements.Instance.transform;

        lastTargetPosition = patrolPoints[patrolIndex].position;

        StartCoroutine(UpdatePathRoutine());
    }

    private void OnDisable() => StopAllCoroutines();
    private void OnDestroy() => StopAllCoroutines();

    private IEnumerator UpdatePathRoutine()
    {
        var wait = new WaitForSeconds(pathUpdateInterval);

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
                speed = maxSpeed;
                pathUpdateInterval = 0.15f;
                SetEnemyTagAndCollision(true);
                wait = new WaitForSeconds(pathUpdateInterval);

            }
            else
            {
                isChasingPlayer = false;
                destination = patrolPoints[patrolIndex].position;
                speed = defaultSpeed;
                pathUpdateInterval = 0.25f;
                SetEnemyTagAndCollision(false);
                wait = new WaitForSeconds(pathUpdateInterval);
            }

            if (!isFollowing || Vector3.Distance(destination, lastTargetPosition) > repathThreshold)
            {
                lastTargetPosition = destination;
                PathManager.Instance.RequestPath(transform.position, destination, OnPathFound);
            }

            yield return wait;
        }
    }

    private void OnPathFound(List<Vector3> newPath)
    {
        if (newPath == null || newPath.Count == 0) return;
        path = newPath;
        currentIndex = 0;
        isFollowing = true;
    }

 /*   private void FixedUpdate()
    {
        if (isCollidingWithPlayer || !isFollowing || path == null || currentIndex >= path.Count)
            return;

        // --- Movement ---
        Vector3 targetPoint = path[currentIndex];
        moveDir = (targetPoint - transform.position).normalized;
        cachedMoveDir = moveDir;

        rb.MovePosition(rb.position + (Vector2)(moveDir * speed * Time.fixedDeltaTime));

        // --- Path check ---
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

   



    private void Update()
    {
        // --- Rotation Sampling ---

        if(playerRb==null)
        {
            playerRb = PlayerMovements.Instance.gameObject.GetComponent<Rigidbody2D>();
        }

        rotationSampleTimer += Time.deltaTime;
        if (rotationSampleTimer >= rotationSampleInterval)
        {
            rotationSampleTimer = 0f;

            float desiredAngle;

            if (isChasingPlayer && player != null)
            {
                // Predict player pos if needed
                Vector3 targetPos = player.position;
                if (predictionTime > 0f && playerRb != null)
                    targetPos = (Vector2)player.position + playerRb.linearVelocity * predictionTime;

                Vector3 dirToPlayer = (targetPos - transform.position);
                desiredAngle = dirToPlayer.sqrMagnitude > 0.0001f
                    ? Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg + 180f
                    : transform.eulerAngles.z;
            }
            else
            {
                desiredAngle = cachedMoveDir.sqrMagnitude > 0.0001f
                    ? Mathf.Atan2(cachedMoveDir.y, cachedMoveDir.x) * Mathf.Rad2Deg + 180f
                    : transform.eulerAngles.z;
            }

            // Update cached target only if significant
            if (!hasCachedAngle)
            {
                cachedTargetAngle = desiredAngle;
                hasCachedAngle = true;
            }
            else
            {
                float delta = Mathf.DeltaAngle(cachedTargetAngle, desiredAngle);
                if (Mathf.Abs(delta) > rotationAngleThreshold)
                    cachedTargetAngle = desiredAngle;
            }
        }

        // --- Rotate deterministically ---
        if (hasCachedAngle)
        {
            Quaternion targetRot = Quaternion.Euler(0f, 0f, cachedTargetAngle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
*/


    private void LateUpdate()
    {
        if (isCollidingWithPlayer || !isFollowing || path == null || currentIndex >= path.Count)
            return;

        Vector3 targetPoint = path[currentIndex];
        Vector3 direction = (targetPoint - transform.position).normalized;

        // Move enemy
        rb.MovePosition(rb.position + (Vector2)(direction * speed * Time.fixedDeltaTime));

        // Smooth rotation only if moving
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            // Multiply by Time.fixedDeltaTime to keep rotation consistent with physics update
            float step = rotationSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }

        // Check if reached current path point
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

    /* private void FixedUpdate()
     {
         if (isCollidingWithPlayer || !isFollowing || path == null || currentIndex >= path.Count)
             return;

         Vector3 targetPoint = path[currentIndex];
         Vector3 moveVector = targetPoint - transform.position;
         float distance = moveVector.magnitude;

         if (distance > 0.001f)
         {
             // Normalize movement direction
             Vector3 moveDir = moveVector / distance;

             // Move enemy
             Vector2 moveDelta = (Vector2)(moveDir * speed * Time.fixedDeltaTime);
             rb.MovePosition(rb.position + moveDelta);

             // Rotate to match actual velocity
             if (moveDelta.sqrMagnitude > 0.0001f)
             {
                 float angle = Mathf.Atan2(moveDelta.y, moveDelta.x) * Mathf.Rad2Deg + 180f;
                 transform.rotation = Quaternion.Euler(0f, 0f, angle);
             }
         }

         // Check if reached current path point
         if (distance < stopThreshold)
         {
             currentIndex++;
             if (currentIndex >= path.Count)
             {
                 isFollowing = false;
                 if (!isChasingPlayer)
                     patrolIndex = (patrolIndex + 1) % patrolPoints.Count;
             }
         }
     }*/



    public bool IsPlayerVisibleInBack()
    {
        if (!player) return false;

        Vector3 origin = transform.position;
        Vector3 toPlayer = player.position - origin;

        if (toPlayer.magnitude > detectionRange) return false;

        Vector3 dirToPlayer = toPlayer.normalized;
        Vector3 back = -transform.right;

        if (Vector3.Angle(back, dirToPlayer) > rearViewAngle * 0.5f) return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, dirToPlayer, detectionRange, playerMask | obstacleMask);
        return hit.collider && hit.collider.CompareTag("Player");
    }

    // ---------------- PLAYER COLLISION ----------------
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
        else if(this.gameObject.tag == "enemy" && collision.gameObject.tag == "enemy")
        {
            SharedPathFollower otherEnemy = collision.collider.GetComponent<SharedPathFollower>();
            if (otherEnemy == null) return;

            if (!playerDetected && !otherEnemy.playerDetected)
            {
                Physics2D.IgnoreCollision(myCollider, otherEnemy.myCollider, true);
            }
        }
        else if(this.gameObject.tag == "enemy" && collision.gameObject.tag == "enemyDetected")
        {
            SharedPathFollower otherEnemy = collision.collider.GetComponent<SharedPathFollower>();
            if (otherEnemy == null) return;

            if (!playerDetected && !otherEnemy.playerDetected)
            {
                Physics2D.IgnoreCollision(myCollider, otherEnemy.myCollider, false);
            }
        }
        else if(this.gameObject.tag == "enemyDetected" && collision.gameObject.tag == "enemy")
        {
            SharedPathFollower otherEnemy = collision.collider.GetComponent<SharedPathFollower>();
            if (otherEnemy == null) return;

            if (!playerDetected && !otherEnemy.playerDetected)
            {
                Physics2D.IgnoreCollision(myCollider, otherEnemy.myCollider, false);
            }
        }
        else if (this.gameObject.tag == "enemyDetected" && collision.gameObject.tag == "enemyDetected")
        {
            SharedPathFollower otherEnemy = collision.collider.GetComponent<SharedPathFollower>();
            if (otherEnemy == null) return;

            if (!playerDetected && !otherEnemy.playerDetected)
            {
                Physics2D.IgnoreCollision(myCollider, otherEnemy.myCollider, false);
            }
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bomb"))
        {
            Instantiate(Game_Manager.Instance.BloodPrefab, transform.position, Quaternion.identity);
            Game_Manager.Instance.Enemies.Remove(this);
            Destroy(gameObject);
            Game_Manager.Instance.EnemyCount();
        }
        /*else if (collision.CompareTag("Bullet"))
        {
           
           


            
        }*/
    }


  



    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            //animator.SetTrigger("Walk");
        }

    }

    private IEnumerator RotateThenAttack(Quaternion targetRot)
    {
        while (Quaternion.Angle(transform.rotation, targetRot) > 5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
       // animator.SetTrigger("Attack");
    }

    // ---------------- HEALTH ----------------
   
    // ---------------- UTILS ----------------
    private void OnDrawGizmosSelected()
    {
        if (!player) return;

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
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void Attack()
    {
       // if (!isCollidingWithPlayer) return;
       if(playerInRange)
       {
            Game_Manager.Instance.playerHealth.GetAttack(1);
       }
       
    }

    

    private void SetEnemyTagAndCollision(bool detected)
    {
        if (detected)
        {
            gameObject.tag = "enemyDetected";

            foreach (var enemy in Game_Manager.Instance.Enemies)
            {
                if (enemy == null || enemy == this) continue;

                Physics2D.IgnoreCollision(myCollider, enemy.myCollider, false);

                /*if (gameObject.tag == "enemy" && enemy.gameObject.tag == "enemy")
                {
                    // Both normal enemies -> allow collision
                    Physics2D.IgnoreCollision(myCollider, enemy.myCollider, true);
                }
                else
                {
                    // At least one is "enemyDetected" -> ignore collision
                    Physics2D.IgnoreCollision(myCollider, enemy.myCollider, false);
                }*/
            }
        }     
        else
        {
            gameObject.tag = "enemy";
        }
    }

    
}


