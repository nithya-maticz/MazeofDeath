using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animatorRef;
    // NavMeshAgent agent;
    public SpriteRenderer playerSprite;

    [Header("UI")]
    //public Image playerHealthFill;
   
    [Header("Gameplay")]
    public GameObject playerCollider;
    public GameObject light;

    [Header("Stats")]
    public float speed = 5f;
    public bool playerDeath;
    public int PlayerHealthCount;

    private Coroutine closeDoorCoroutine;
    private WaitForSeconds waitFor2Sec; 

    void Awake()
    {
        Instance = this;
       
        rb = GetComponent<Rigidbody2D>();
        waitFor2Sec = new WaitForSeconds(2f);
      /*  if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;*/
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveH = ManagerMaze.instance.joystick.Horizontal;
        float moveV = ManagerMaze.instance.joystick.Vertical;

        // Only apply movement if joystick is not idle
        if (moveH != 0f || moveV != 0f)
        {
            Vector2 moveDir = new Vector2(moveH, moveV);
            rb.linearVelocity = moveDir * speed;

            // Optional: Add facing direction animation
          /*  animatorRef.SetFloat("MoveX", moveH);
            animatorRef.SetFloat("MoveY", moveV);*/
            //animatorRef.SetBool("IsMoving", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            //animatorRef.SetBool("IsMoving", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyDoor") && ManagerMaze.instance.isPlayerGetKey)
        {
            ZombieDoor zombieDoor = collision.GetComponent<ZombieDoor>();
            if (zombieDoor != null)
            {
                zombieDoor.light.SetActive(true);
                closeDoorCoroutine = StartCoroutine(CloseDoorAfterDelay(zombieDoor));
            }
        }
       else  if (collision.CompareTag("enemy"))
        {
            Debug.Log("Inside---------");
            animatorRef.SetTrigger("playerattack");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyDoor") && closeDoorCoroutine != null)
        {
            StopCoroutine(closeDoorCoroutine);
            closeDoorCoroutine = null;

            ZombieDoor zombieDoor = collision.GetComponent<ZombieDoor>();
            if (zombieDoor != null)
            {
                zombieDoor.light.SetActive(false);
                light.SetActive(true);
            }
        }
    }

    private IEnumerator CloseDoorAfterDelay(ZombieDoor zombieDoor)
    {
        yield return waitFor2Sec;
        light.SetActive(false);
        CloseDoor(zombieDoor);
    }

    void CloseDoor(ZombieDoor zombieDoor)
    {
        if (zombieDoor == null) return;

        zombieDoor.isClosed = true;
        zombieDoor.sprite.sprite = ManagerMaze.instance.DoorClose;
        zombieDoor.light.SetActive(false);
        light.SetActive(true);
        zombieDoor.GetComponent<BoxCollider2D>().enabled = false;

        Debug.Log("Door closed!");
        ManagerMaze.instance.DoorClosedCount();
        ManagerMaze.instance.CheckLevelUp();
    }

    public void PlayerAttackButton()
    {
        if (!playerDeath)
        {
            animatorRef.SetTrigger("playerattack");
            // You can handle bullet shooting here if needed
        }
    }

}
