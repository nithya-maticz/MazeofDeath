using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("References")]
    public ManagerMaze manager;
    public Rigidbody2D rb;
    public Joystick joystick;
    public Animator animatorRef;
    public healthbarscript healthBar;
    public NavMeshAgent agent;

    [Header("UI")]
    public TMP_Text playerLifeTxt;
    public Image playerHealthFill;
    public GameObject gameoverPage;
    public GameObject winPage;
    public GameObject openDoorImg;

    [Header("Gameplay")]
    public GameObject door;
    public GameObject enemy;
    public GameObject keyRef;
    public GameObject keyHead;
    public GameObject playerCollider;
    public GameObject bullet;
    public Transform bulletTransform;
    public float bulletSpeed = 10f;
    public Sprite boxOpen;
    public GameObject light;

    [Header("Stats")]
    public float speed = 5f;
    public float playerHealth = 1f;
    public int damage = 1;
    public int playerHealthCount;
    public int keyCount;
    public bool keyTaken;
    public bool openDoor;
    public bool playerDeath;
    public bool attackEnemy;
    public int PlayerHealthCount;

    private Coroutine closeDoorCoroutine;
    private WaitForSeconds waitFor2Sec; // reuse WaitForSeconds to reduce GC

    void Awake()
    {
        Instance = this;
        waitFor2Sec = new WaitForSeconds(2f);
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveH = joystick.Horizontal;
        float moveV = joystick.Vertical;

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

    public void IncreaseHealth()
    {
        playerHealth = 1f;
        playerHealthFill.fillAmount = 1f;
    }
}
