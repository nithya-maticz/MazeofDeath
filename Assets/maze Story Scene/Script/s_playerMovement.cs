using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class s_playerMovement : MonoBehaviour
{
    public static s_playerMovement Instance;
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

    public GameObject closeDoor;

    [Header("Stats")]
    public float speed = 5f;
    

    
    public bool IsGetKnife;
    public bool attackEnemy;
    public GameObject fadeImage;
    public Animator fadeAnimator;
    public bool reach;

    void Awake()
    {
        Instance = this;
        IsGetKnife = false;
        rb = GetComponent<Rigidbody2D>();
        
        
    }

    void Update()
    {
        HandleMovement();
    }

    bool wasMovingLastFrame = false;
    private bool wasKeyTakenLastFrame = false;

    void HandleMovement()
{
    float moveH = StoryManager.Instance.joystick.Vertical;
    float moveV = StoryManager.Instance.joystick.Horizontal;

    bool isMoving = moveH != 0f || moveV != 0f;

    if (isMoving)
    {
        Vector2 moveDir = new Vector2(moveH, -moveV);
        rb.linearVelocity = moveDir * speed;

        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 180f);
        float rotationSpeed = 720f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        /*animatorRef.SetFloat("MoveX", moveH);
        animatorRef.SetFloat("MoveY", -moveV);*/

        // Trigger only once when movement starts
        if (!wasMovingLastFrame)
        {
                if(StoryManager.Instance.knifeTaken)
                {
                  
                    animatorRef.SetTrigger("knifewalk");
                }
                else
                {
                    animatorRef.SetTrigger("walk");
                }
                    
        }
    }
    else
    {
        rb.linearVelocity = Vector2.zero;

        // Trigger only once when stopping
        if (wasMovingLastFrame)
        {
                if (StoryManager.Instance.knifeTaken)
                {
                    animatorRef.SetTrigger("knifeidle");
                }
                else
                {
                    animatorRef.SetTrigger("idle");
                }
                
        }
    }

    wasMovingLastFrame = isMoving;
}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("enemy"))
        {
            attackEnemy = true;
            Debug.Log("Inside---------");
           // animatorRef.SetTrigger("playerattack");
        }
          else if (collision.CompareTag("EnemyDoor") && StoryManager.Instance.keyTaken)
        {
            light.SetActive(true);
            reach = true;
            Invoke("LightInvisible", 2f);

        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            attackEnemy = false;
            Debug.Log("Inside---------");
           // animatorRef.SetTrigger("playerattack");
        }
        else if (collision.CompareTag("EnemyDoor"))
        {
            reach = false;
            light.SetActive(false);
          

        }

    }
    public void LightInvisible()
    {
        if(reach)
        {
            light.SetActive(false);
            closeDoor.SetActive(true);
            fade();
        }
       
    }
    public void GameScene()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void fade()
    {
        fadeImage.SetActive(true);
        fadeAnimator.SetTrigger("fade");
        
    }



    public void AttackFun()
    {
        animatorRef.SetTrigger("attack");
    }

   

}
