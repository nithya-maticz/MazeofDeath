using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class StoryPlayer : MonoBehaviour
{
    public static StoryPlayer Instance;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animatorRef;
    public Animator fadeAnimator;
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
    public GameObject fadeImage;
    public GameObject attackImage;
    public bool knifeWalkStart;
    public bool walkStart;

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

            if (StoryManager.Instance.knifeTaken && attack==false)
            {
                if (knifeWalkStart == false)
                {
                    knifeWalkStart = true;
                    animatorRef.SetTrigger("knifewalk");
                }
            }

            else
            {
                if (walkStart == false && attack == false)
                {
                    walkStart = true;
                    animatorRef.SetTrigger("walk");
                }
            }
               


        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (StoryManager.Instance.knifeTaken && attack == false)
            {

                animatorRef.SetTrigger("knifeidle");

            }
            else
            {
                if( attack == false)
                {
                    animatorRef.SetTrigger("idle");
                }
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("---------------"+ StoryManager.Instance.keyTaken);
        Debug.Log(StoryManager.Instance.enemyDestory);
        if (collision.CompareTag("EnemyDoor")  && (StoryManager.Instance.keyTaken) && (StoryManager.Instance.enemyDestory))
        {
            doorLight.SetActive(true);
            light.SetActive(true);
            Invoke("doorclose", 2f);

        }
        if (collision.CompareTag("enemy"))
        {
            
           
            Debug.Log("Enemyyyyyyyyyy");
            


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyDoor") )
        {

        }
        if (collision.CompareTag("enemy"))
        {
            attackImage.SetActive(false);
            Debug.Log("Enemyyyyyyyyyy");
            attack = false;
        }
    }
    public void doorclose()
    {
        doorLight.SetActive(false);
        closeDoor.SetActive(true);
        openDoor.SetActive(false);
        Invoke("fade", 1f);
       
       
    }
    public void GameScene()
    {
        SceneManager.LoadScene("Game");
    }
    public void fade()
    {
        fadeImage.SetActive(true);
        fadeAnimator.SetTrigger("fade");
        GameScene();
    }

    public void AttackFunction()
    {
         attack = true;
        attackImage.SetActive(true);
        animatorRef.SetTrigger("attack");
       
    }


}

   
  

