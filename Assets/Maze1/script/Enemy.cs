using System.Collections;
using UnityEngine;
using System;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;




public class Enemy : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Data")]
    public Transform target;
    public TMP_Text enemyHealthTxt;
    public int enemyHealth;
    private int currentHealth;
    NavMeshAgent Agent;
    public Animator animator;
    public bool attackPlayer;
    public bool Door;
    public bool playerDeath;
    public bool temp;
    private bool _enemyAttack;
    private Coroutine healthCoroutine;

    private Quaternion targetRotation;
    public float rotationSpeed = 360f; // Degrees per second
    private Transform playerTransform;

    public float checkInterval = 1f; // How often to check for nearest target
    private float timer;

    public Sprite newSprite;
    public float newAlpha = 0.5f;


    [Header("RAYCAST")]
    [SerializeField] Transform RaycastParent;
    [SerializeField] float rayLength;
    [SerializeField] int rayCount;
    [SerializeField] float coneAngle;
    [SerializeField] LayerMask raycastLayerMask;
    public int count = 1;


    [Header("Patrolling")]
    // public Transform[] patrolPoints;  // Set these in the inspector
    public float speed = 3f;
    public float reachThreshold = 0.1f;

    private int currentPointIndex = 0;
    private Transform targetPoint;



    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        currentHealth = enemyHealth;




        ManagerMaze.instance.targetPoint = 0;
        target = Player.Instance.transform;


        if (ManagerMaze.instance.partolPoints.Length > 0)
        {
            targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
        }

        // target = ManagerMaze.instance.partolPoints[ManagerMaze.instance.targetPoint];

    }

    public void damageEnemy(int damange)
    {
        currentHealth -= damange;
        enemyHealthTxt.text = currentHealth.ToString();
    }

    void Update()
    {
        if (Player.Instance.playerDeath)
        {
            animator.SetTrigger("playerdeath");
            return;
        }


        /*if(targetPoint== Player.Instance.transform)
        {

        }
        else*/
        {
            /*if (targetPoint == null || ManagerMaze.instance.partolPoints.Length == 0) return;

            // Set NavMesh destination
            Agent.SetDestination(targetPoint.position);

            // Rotate enemy smoothly toward movement direction
            Vector2 direction = targetPoint.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 180);

            // Smoothly rotate toward the target
            float rotationSpeed = 5f; // Adjust for faster/slower turning
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if enemy reached patrol point
            if (Vector2.Distance(transform.position, targetPoint.position) < reachThreshold)
            {
                currentPointIndex = (currentPointIndex + 1) % ManagerMaze.instance.partolPoints.Length;
                targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
            }*/

            if (targetPoint == null || ManagerMaze.instance.partolPoints.Length == 0) return;

            // Set NavMesh destination
            Agent.SetDestination(targetPoint.position);

            // Calculate angle toward target
            Vector2 direction = targetPoint.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            // Smoothly rotate toward the target
            float rotationSpeed = 200f; // degrees per second, adjust as needed
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if enemy reached patrol point
            if (Vector2.Distance(transform.position, targetPoint.position) < reachThreshold)
            {
                currentPointIndex = (currentPointIndex + 1) % ManagerMaze.instance.partolPoints.Length;
                targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
            }
        }



        /* if (target != null)
         {
             Agent.SetDestination(target.position);

             if (target ==Player.Instance.transform)
             {
                 Agent.SetDestination(target.position);

                 if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
                 {
                     if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                     {
                         Debug.Log("inside ");
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

                 if (ManagerMaze.instance.playerRef != null)
                 {
                     Vector2 direction = ManagerMaze.instance.playerRef.transform.position - transform.position;
                     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                     // Apply rotation only on Z-axis for 2D
                     transform.rotation = Quaternion.Euler(0, 0, angle + 180);
                 }
             }*/


        //  }


        // }

    }


    private void FixedUpdate()
    {
        Vector2 centerDir = -RaycastParent.up;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = Mathf.Lerp(-coneAngle / 2, coneAngle / 2, (float)i / (rayCount - 1));
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * centerDir;

            RaycastHit2D hit = Physics2D.Raycast(RaycastParent.position, dir, rayLength, raycastLayerMask);

            if (hit.collider != null)
            {

                Debug.DrawRay(RaycastParent.position, dir * hit.distance, Color.green);
                Debug.Log(hit.collider.gameObject.name);
            }
            else
            {

                Debug.DrawRay(RaycastParent.position, dir * rayLength, Color.red);
            }
        }
    }




    public void idleFun()
    {
        animator.SetTrigger("idle");
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

            // Clamp to avoid negative health
            targetHealth = Mathf.Max(0, targetHealth);




            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                player.playerHealth = Mathf.Lerp(startHealth, targetHealth, t);
                // Debug.Log(player.playerHealth);
                player.playerHealthFill.fillAmount = player.playerHealth;
                yield return null;
            }

            if (player.playerHealth < 0.1)
            {
                player.playerHealth = 0;
                player.playerHealthFill.fillAmount = 0;
                ManagerMaze.instance.GameOver();
                yield break;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerRange")
        {
            target = null;
            animator.SetTrigger("enemyattack");
            ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.red;
            // targetPoint =  Player.Instance.transform;
            Agent.ResetPath();


            if (healthCoroutine == null)
                healthCoroutine = StartCoroutine(ReducePlayerHealth(ManagerMaze.instance.playerRef.GetComponent<Player>()));
        }

        if (collision.gameObject.tag == "enemyrange")
        {
            target = null;
             animator.SetTrigger("enemyrun");
            GetComponent<NavMeshAgent>().speed = 5f;
            targetPoint = Player.Instance.transform;
            Agent.ResetPath();
        }


        if (collision.gameObject.tag == "attack")
        {
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.white;
                healthCoroutine = null;
                Debug.Log("Coroutine stopped.");
            }
            Invoke("DestryInstance", .5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerRange")
        {
            target = ManagerMaze.instance.playerRef.transform;
            animator.SetTrigger("idle");

            if (healthCoroutine != null)
            {
                ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.white;
                StopCoroutine(healthCoroutine);
            }

        }
        else if (collision.gameObject.tag == "enemyrange")
        {
            targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
            animator.SetTrigger("idle");
            GetComponent<NavMeshAgent>().speed = 3.5f;
            Agent.ResetPath();
        }

    }

    void DeleyPathReset()
    {
        Agent.ResetPath();
    }

    void DestryInstance()
    {
        GameObject blood = Instantiate(ManagerMaze.instance.BloodPrefab.gameObject, transform);
        blood.transform.SetParent(ManagerMaze.instance.BloodTransform);
        blood.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
    }
}




