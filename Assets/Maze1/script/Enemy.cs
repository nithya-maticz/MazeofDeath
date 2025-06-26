using System.Collections;
using UnityEngine;
using System;
using UnityEngine.AI;
using TMPro;




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




    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        currentHealth = enemyHealth;
        target = ManagerMaze.instance.playerRef.transform;

    }

    public void damageEnemy(int damange)
    {
        currentHealth -= damange;
        enemyHealthTxt.text = currentHealth.ToString();
    }
    
    void Update()
    {
        /*timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            FindNearestTarget();
            timer = 0f;
        }*/

        if (target != null)
        {
            Agent.SetDestination(target.position);

            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                {
                    if(healthCoroutine==null)
                    {
                        healthCoroutine = StartCoroutine(ReducePlayerHealth(ManagerMaze.instance.playerRef));
                    }
                }
            }
            else
            {
                if (healthCoroutine!=null)
                {
                    StopCoroutine(healthCoroutine);
                    healthCoroutine = null;
                }
            }

        }

        if (ManagerMaze.instance.playerRef != null)
        {
            Vector2 direction = ManagerMaze.instance.playerRef.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation only on Z-axis for 2D
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
    }

    void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject target in targets)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = target.transform;
            }
        }

        target = closest;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.tag == "player")
        {

            attackPlayer = true;

            //Agent.enabled = false;
            Debug.Log("playercollider on Tigger Enter ");

            // animator.SetBool("attack", true);

            _enemyAttack = true;
           // healthCoroutine = StartCoroutine(ReducePlayerHealth(collision));

        }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "player")
        {

            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                healthCoroutine = null;
                Debug.Log("Coroutine stopped.");
                _enemyAttack = false;
            }

            attackPlayer = false;
            // animator.SetBool("attack", false);

        }

    }

    public void idleFun()
    {
        animator.SetTrigger("idle");
    }



    IEnumerator ReducePlayerHealth(Player _player)
    {
        Player player = _player;
        while (true)
        {
            yield return new WaitForSeconds(2f);
            player.playerHealth = player.playerHealth - 0.1f;
            if (player.playerHealth <= 0)
            {
                player.playerHealthFill.fillAmount = 0;
                ManagerMaze.instance.GameOver();
                //
            }
            else
            {
                player.playerHealthFill.fillAmount = player.playerHealth;
            }



        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerRange")
        {
            target = null;
            Agent.ResetPath();
          //  Invoke("DeleyPathReset", 0.5f);
           /* if(healthCoroutine == null)
                healthCoroutine = StartCoroutine(ReducePlayerHealth(ManagerMaze.instance.playerRef.GetComponent<Player>()));*/
        }

        if (collision.gameObject.tag == "attack")
        {
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                healthCoroutine = null;
                Debug.Log("Coroutine stopped.");
            }
            Destroy(this.gameObject, 1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerRange")
        {
            target = ManagerMaze.instance.playerRef.transform;
          /*  if (healthCoroutine != null)
                StopCoroutine(healthCoroutine);*/
        }
    }

    void DeleyPathReset()
    {
        Agent.ResetPath();
    }
}




