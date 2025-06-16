using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform target;
    public int enemyHealth;
    NavMeshAgent Agent;
    public Animator animator;
    public bool attackPlayer;
    public bool Door;
    public bool playerDeath;
   
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
        if (Door==false && playerDeath==false)
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
        }
        if (collision.gameObject.tag == "attack")
        {
            Destroy(this.gameObject,0.5f);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        attackPlayer = false;
        animator.SetBool("attack", false);
    }
    public void idleFun()
    {
        animator.SetTrigger("idle");
    }

}
