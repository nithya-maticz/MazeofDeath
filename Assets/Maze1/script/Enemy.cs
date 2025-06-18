using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.AI;



public class Enemy : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Data")]
    public CharacterObj charcter;
    public Transform target;
    public int enemyHealth;
    NavMeshAgent Agent;
    public Animator animator;
    public bool attackPlayer;
    public bool Door;
    public bool playerDeath;
    public bool temp;
    private Coroutine healthCoroutine;


    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        Door = FindObjectOfType<Player>().openDoor;
        playerDeath = FindObjectOfType<Player>().playerDeath;

        if (Door == false && playerDeath == false)
        {
            Agent.SetDestination(target.position);
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("playercollider");
        if (collision.gameObject.tag == "player")
        {
            attackPlayer = true;
            Debug.Log("playercollider");
            animator.SetBool("attack", true);
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
            
            Destroy(this.gameObject,0.5f);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "player")
        {
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                healthCoroutine = null;
                Debug.Log("Coroutine stopped.");
            }

            attackPlayer = false;
            animator.SetBool("attack", false);
        }
    }
    public void idleFun()
    {
        animator.SetTrigger("idle");
    }



    IEnumerator ReducePlayerHealth(Collider2D _player)
    {
        Player player = _player.GetComponent<Player>();
        while (true)
        {
            yield return new WaitForSeconds(2f);
            player.playerHealth = player.playerHealth - 0.1f;
            if(player.playerHealth <= 0)
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

