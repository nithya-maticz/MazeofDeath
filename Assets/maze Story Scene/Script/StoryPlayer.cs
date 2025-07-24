using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class StoryPlayer : MonoBehaviour
{
    public static StoryPlayer Instance;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animatorRef;
    public NavMeshAgent agent;
    public SpriteRenderer playerSprite;

    [Header("UI")]
    //public Image playerHealthFill;

    [Header("Gameplay")]
    public GameObject playerCollider;
    public GameObject light;

    [Header("Stats")]
    public float speed = 2f;
    public bool playerDeath;
    public int PlayerHealthCount;

    private Coroutine closeDoorCoroutine;
    private WaitForSeconds waitFor2Sec;

    [Header("Modules")]
    public Joystick joystick;
    public bool attack;
    public GameObject doorLight;
    public GameObject closeDoor;
    public GameObject openDoor;

    void Awake()
    {
        Instance = this;
        //joystick.StartStoryJoystick();
        rb = GetComponent<Rigidbody2D>();
        waitFor2Sec = new WaitForSeconds(2f);
        //  if (agent == null) agent = GetComponent<NavMeshAgent>();
        //  agent.updateRotation = false;
        // agent.updateUpAxis = false;
    }

    void Update()
    {
        HandleMovement();
        //HandleMovement();
    }

    void HandleMovement()
    {
        float moveH = joystick.Horizontal;
        float moveV = joystick.Vertical;


        if (moveH != 0f || moveV != 0f)
        {

            Vector2 moveDir = new Vector2(moveH, moveV).normalized;

            // Rotate
            if (moveDir.x > 0f)
                transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Face left
            else if (moveDir.x < 0f)
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);   // Face right

            // Apply velocity
            rb.linearVelocity = moveDir * speed;

            if (FindObjectOfType<knifeBox>().knifeTaken)
                animatorRef.SetTrigger("knifewalk");
            else
                animatorRef.SetTrigger("walk");
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (FindObjectOfType<knifeBox>().knifeTaken)
            {
                if (attack)
                    animatorRef.SetTrigger("attack");
                else
                    animatorRef.SetTrigger("knifeidle");
            }
            else
            {
                animatorRef.SetTrigger("idle");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyDoor"))
        {
            doorLight.SetActive(true);
            light.SetActive(true);
            Invoke("doorclose", 3f);

        }
        if (collision.CompareTag("enemy"))
        {
            Debug.Log("Enemyyyyyyyyyy");
            attack = true;


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyDoor") )
        {

        }
        if (collision.CompareTag("enemy"))
        {
            Debug.Log("Enemyyyyyyyyyy");
            attack = false;
        }
    }
    public void doorclose()
    {
        doorLight.SetActive(false);
        closeDoor.SetActive(true);
        openDoor.SetActive(false);
    }



}

   
  

