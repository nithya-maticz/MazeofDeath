using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class Player : MonoBehaviour
{
    public static Player Instance;
    public ManagerMaze manager;
   
    public float speed = 5.0f;

    
  
    public GameObject door;
    public GameObject enemy;
    public GameObject keyRef;
    public bool keyTaken;
    public int keyCount;
    
    public bool openDoor;
    public bool playerDeath;
    public bool reach;
    public GameObject keyHead;
    public Animator animatorRef;
   // public int maxhealth = 10;
   // public int health;
    public int damage;
    public bool attackEnemy;
    public bool spaceClick;
    public Joystick joystick;
    public Rigidbody2D rb;
    public float timer = 10f;
    public float remainTime;
    public TMP_Text playerLifeTxt;
    public healthbarscript healthBar;
    public Animator enemyAnimator;
    public GameObject playerCollider;
    public GameObject gameoverpage;
    public GameObject winpage;
    public GameObject player;
    public ManagerMaze managerRef;
    public bool attackButtonClick;
    public Transform bulletTrnsform;
    public GameObject bullet;
    public float bulletpeed;
    public Sprite boxOpen;
    NavMeshAgent Agent;
     public Transform target;
   
    public GameObject OpenDoorImg;
    // public bool isDeath;

    [Space]
    [Header("HEALTH")]
    public Image playerHealthFill;
    public float playerHealth;

    public Transform TargetMovement;
    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;



        //  health = maxhealth;
        // healthBar = GetComponent<healthbarscript>();
    }

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

          float moveH = joystick.Horizontal;
          float moveV = joystick.Vertical;
          Vector2 moveDir = new Vector2(moveH, moveV);
          rb.linearVelocity = moveDir * speed;
       

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
      /*  else if (collision.gameObject.tag == "door" && manager.isEnemyDoorOpen == false && manager.isPlayerGetKey)
        {
            Debug.Log("LEVEL UP!");
            ManagerMaze.instance.PlayerDoorSprite.sprite = ManagerMaze.instance.DoorOpen;
           // LevelUp();
            Invoke("LevelUp", 1f);
        }*/
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
      
       
           
      
           
    }

    public void PlayerAttackButton()
    {
        if(!playerDeath)
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
}
