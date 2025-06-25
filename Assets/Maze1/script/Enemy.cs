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


    


    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        currentHealth = enemyHealth;

    }

    public void damageEnemy(int damange)
    {
        currentHealth -= damange;
        enemyHealthTxt.text = currentHealth.ToString();
    }
    // Update is called once per frame
    void Update()
    {

        playerDeath = FindObjectOfType<Player>().playerDeath;

        if (Door == false && playerDeath == false)
        { 
            if (target != null && Agent.enabled)
            {
                Agent.SetDestination(target.position);
            }

        }
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.tag == "player")
        {
            attackPlayer = true;
            target = null;
            //Agent.enabled = false;
            Debug.Log("playercollider on Tigger Enter ");

            // animator.SetBool("attack", true);

            _enemyAttack = true;
            healthCoroutine = StartCoroutine(ReducePlayerHealth(collision));

        }
        if (collision.gameObject.tag == "attack")
        {
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                healthCoroutine = null;
                Debug.Log("Coroutine stopped.");
            }
            Destroy(this.gameObject, 0.5f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "player")
        {
            target = null;
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



    IEnumerator ReducePlayerHealth(Collision2D _player)
    {
        Player player = _player.gameObject.GetComponent<Player>();
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
}

   

