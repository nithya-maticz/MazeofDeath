using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public ManagerMaze manager;

    [Header("Movement")]
    public float speed = 5.0f;
    [Tooltip("Adjust joystick sensitivity")]
    public float sensitivity = 1f;
    public Joystick joystick;
    public Rigidbody2D rb;

    [Header("Door & Key")]
    public GameObject door;
    public GameObject keyRef;
    public bool keyTaken;
    public int keyCount;
    public bool openDoor;
    public GameObject OpenDoorImg;

    [Header("Enemy")]
    public GameObject enemy;
    public bool attackEnemy;
    public Animator enemyAnimator;
    public bool attackButtonClick;
    public bool spaceClick;
    public Transform target;

    [Header("Health")]
    public float playerHealth = 1f;
    public Image playerHealthFill;
    public TMP_Text playerLifeTxt;
    public healthbarscript healthBar;
    public int damage;

    [Header("Other")]
    public GameObject keyHead;
    public Animator animatorRef;
    public GameObject playerCollider;
    public GameObject gameoverpage;
    public GameObject winpage;
    public GameObject player;
    public ManagerMaze managerRef;
    public Transform bulletTrnsform;
    public GameObject bullet;
    public float bulletSpeed;
    public Sprite boxOpen;
    public NavMeshAgent Agent;
    public Transform TargetMovement;

    [Header("State Flags")]
    public bool playerDeath;
    public bool reach;

    [Header("Timer")]
    public float timer = 10f;
    public float remainTime;

    [Header("Sensitivity")]
    public TMP_Text SensitivityText;

    
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        SensitivityText.text = sensitivity.ToString();
    }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        float moveH = joystick.Horizontal;
        float moveV = joystick.Vertical;

        Vector2 moveDir = new Vector2(moveH, moveV);

       
        if (moveDir.magnitude > 1f)
            moveDir = moveDir.normalized;

        
        rb.linearVelocity = moveDir * speed;

        
        if (moveDir.sqrMagnitude > 0.05f)
        {
            
            float targetAngle = Mathf.Atan2(moveV, moveH) * Mathf.Rad2Deg;

            
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * sensitivity * 10f);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyDoor" && ManagerMaze.instance.isPlayerGetKey)
        {
            Debug.Log("Enemy Door Closed!");
            var zombieDoor = collision.GetComponent<ZombieDoor>();
            zombieDoor.isClosed = true;
            zombieDoor.sprite.sprite = ManagerMaze.instance.DoorClose;
            ManagerMaze.instance.CheckLevelUp();
        }
    }

    public void PlayerAttackButton()
    {
        if (!playerDeath)
        {
            attackButtonClick = true;
            animatorRef.SetTrigger("playerattack");
        }
    }

    public void IncreaseHealth()
    {
        playerHealth = 1;
        playerHealthFill.fillAmount = 1;
    }

    public void IncreaseSensitivity()
    {
        sensitivity += 0.1f;
        SensitivityText.text = sensitivity.ToString("F3");
    }

    public void DecreaseSensitivity()
    {
        sensitivity -= 0.1f;
        SensitivityText.text = sensitivity.ToString("F3");
    }
}
