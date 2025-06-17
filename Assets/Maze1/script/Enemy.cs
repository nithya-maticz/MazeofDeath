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


        /* if (Door == false && playerDeath == false)
         {
             Agent.SetDestination(target.position);

             // Rotate enemy to face the player
             Vector3 direction = (target.position - transform.position).normalized;
             if (direction != Vector3.zero)
             {
                 Quaternion lookRotation = Quaternion.LookRotation(direction);
                 transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
             }
         }*/

        /* if (Door == false && playerDeath == false)
         {
             Agent.SetDestination(target.position);

             // 2D Facing: Rotate on Z axis only
             Vector2 direction = (target.position - transform.position);
             float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
             transform.rotation = Quaternion.Euler(0, 0, angle);
         }*/

        if (Door == false && playerDeath == false)
        {
            Agent.SetDestination(target.position);
            Agent.transform.rotation = target.rotation;
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
