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
        target = Player.Instance.transform;

    }

    public void damageEnemy(int damange)
    {
        currentHealth -= damange;
        enemyHealthTxt.text = currentHealth.ToString();
    }
    
    void Update()
    {
       

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
            ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color= Color.red;
            
            // Clamp to avoid negative health
            targetHealth = Mathf.Max(0, targetHealth);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                player.playerHealth = Mathf.Lerp(startHealth, targetHealth, t);
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
        if(collision.gameObject.tag == "PlayerRange")
        {
            target = null;
            Agent.ResetPath();
            if (healthCoroutine == null)
                healthCoroutine = StartCoroutine(ReducePlayerHealth(ManagerMaze.instance.playerRef.GetComponent<Player>()));
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
           
            if (healthCoroutine != null)
            {
                ManagerMaze.instance.PlayerImage.GetComponent<SpriteRenderer>().color = Color.white;
                StopCoroutine(healthCoroutine);
            }
                
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




