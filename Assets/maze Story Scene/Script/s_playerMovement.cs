using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [Header("Stats")]
    public float speed = 5f;
    

    private Coroutine closeDoorCoroutine;
    private WaitForSeconds waitFor2Sec;
    public bool IsGetKnife;

    void Awake()
    {
        Instance = this;
        IsGetKnife = false;
        rb = GetComponent<Rigidbody2D>();
        waitFor2Sec = new WaitForSeconds(2f);
        
    }

    void Update()
    {
        HandleMovement();
    }

    bool wasMovingLastFrame = false;

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

        animatorRef.SetFloat("MoveX", moveH);
        animatorRef.SetFloat("MoveY", -moveV);

        // Trigger only once when movement starts
        if (!wasMovingLastFrame)
        {
            animatorRef.SetTrigger("walk");
        }
    }
    else
    {
        rb.linearVelocity = Vector2.zero;

        // Trigger only once when stopping
        if (wasMovingLastFrame)
        {
            animatorRef.SetTrigger("idle");
        }
    }

    wasMovingLastFrame = isMoving;
}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.CompareTag("EnemyDoor") && ManagerMaze.instance.isPlayerGetKey)
        {
            ZombieDoor zombieDoor = collision.GetComponent<ZombieDoor>();
            if (zombieDoor != null)
            {
                zombieDoor.light.SetActive(true);
                closeDoorCoroutine = StartCoroutine(CloseDoorAfterDelay(zombieDoor));
            }
        }
        else if (collision.CompareTag("enemy"))
        {
            Debug.Log("Inside---------");
            animatorRef.SetTrigger("playerattack");
        }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.CompareTag("EnemyDoor") && closeDoorCoroutine != null)
        {
            StopCoroutine(closeDoorCoroutine);
            closeDoorCoroutine = null;

            ZombieDoor zombieDoor = collision.GetComponent<ZombieDoor>();
            if (zombieDoor != null)
            {
                zombieDoor.light.SetActive(false);
                light.SetActive(true);
            }
        }*/
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

    

}
