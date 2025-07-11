using System.Collections;
using UnityEngine;
using System;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using System.Collections.Generic;




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
    public GameObject ligtColor;


    [Header("Patrolling")]
    // public Transform[] patrolPoints;  // Set these in the inspector
    public float speed = 3f;
    public float reachThreshold = 0.1f;

    private int currentPointIndex = 0;
    public Transform targetPoint;

    [Header("Door Patrol")]
    public bool isPatrolDoor;
    public List<Transform> doorPatrolPoints;
    public int currentPoint;



    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        currentHealth = enemyHealth;


        if(isPatrolDoor)
        {
            if (doorPatrolPoints.Count != 0)
            {
                currentPoint = 0;
                targetPoint = doorPatrolPoints[currentPoint];
            }
               
        }
        else
        {
            ManagerMaze.instance.targetPoint = 0;
            target = Player.Instance.transform;


            if (ManagerMaze.instance.partolPoints.Length > 0)
            {
                targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
            }
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
        if (targetPoint == Player.Instance.transform)
        {
            Agent.SetDestination(targetPoint.position);

            if (Agent.velocity.sqrMagnitude > 0.01f)
            {
                Vector2 moveDirection = Agent.velocity.normalized;
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 180f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {

            if(isPatrolDoor)
            {
                if (targetPoint == null || doorPatrolPoints.Count == 0) return;

                Agent.SetDestination(targetPoint.position);

                if (Agent.velocity.sqrMagnitude > 0.01f)
                {
                    Vector2 moveDirection = Agent.velocity.normalized;
                    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 180f;
                    Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                if (Vector2.Distance(transform.position, targetPoint.position) < reachThreshold)
                {
                    currentPoint  = (currentPoint + 1) % doorPatrolPoints.Count;
                    targetPoint = doorPatrolPoints[currentPoint];
                }
            }
            else
            {
                if (targetPoint == null || ManagerMaze.instance.partolPoints.Length == 0) return;

                Agent.SetDestination(targetPoint.position);

                if (Agent.velocity.sqrMagnitude > 0.01f)
                {
                    Vector2 moveDirection = Agent.velocity.normalized;
                    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 180f;
                    Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                if (Vector2.Distance(transform.position, targetPoint.position) < reachThreshold)
                {
                    currentPointIndex = (currentPointIndex + 1) % ManagerMaze.instance.partolPoints.Length;
                    targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
                }
            }
        }
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
                // Debug.Log("Name    "+ hit.collider.gameObject.name);

                if (hit.collider.gameObject.name == "Range" || hit.collider.gameObject.name == "player")
                {
                    // Debug.Log("Player detected by raycast!");
                    ligtColor.GetComponent<Light2D>().color = Color.red;
                    // Change target point to player's transform
                    CancelInvoke("ChangeColor");
                    targetPoint = Player.Instance.transform;

                    // Optionally change speed or animation
                    Agent.speed = 10f;
                    // animator.SetTrigger("enemyrun");

                    // Optional: reset the path to force recalculation
                    Agent.ResetPath();
                }

            }
            else
            {
                // Debug.Log("ELSE ------");
                Invoke("ChangeColor", 10f);

                Debug.DrawRay(RaycastParent.position, dir * rayLength, Color.red);
            }
        }
    }

    public void ChangeColor()
    {
       // Debug.Log("ChangeColor function");
        Agent.speed = 5f;
        ligtColor.GetComponent<Light2D>().color = new Color(0f, 0.443f, 0.031f, 1f);

        if (isPatrolDoor)
        {
            targetPoint = doorPatrolPoints[currentPoint];
            Agent.ResetPath();
        }
        else
        {
            targetPoint = ManagerMaze.instance.partolPoints[currentPointIndex];
            Agent.ResetPath();
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
            Debug.Log("TIGGERFUNNNNNNNNNNN");
            target = null;
            animator.SetTrigger("enemyattack");
            ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.red;
            // targetPoint =  Player.Instance.transform;
            Agent.ResetPath();

            if (healthCoroutine == null)
                healthCoroutine = StartCoroutine(ReducePlayerHealth(ManagerMaze.instance.playerRef.GetComponent<Player>()));
        }

      /*else if (collision.gameObject.tag == "enemyrange")
        {
            target = null;
            animator.SetTrigger("enemyrun");
            GetComponent<NavMeshAgent>().speed = 5f;
            targetPoint = Player.Instance.transform;
            Agent.ResetPath();
        }*/


       else  if (collision.gameObject.tag == "attack")
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
            GetComponent<NavMeshAgent>().speed = 3.5f;
            animator.SetTrigger("idle");
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




