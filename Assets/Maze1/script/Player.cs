using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class Player : MonoBehaviour
{
    public ManagerMaze manager;
    public int keys = 0;
    public float speed = 5.0f;
    public Text keyAmount;
    public Text youWin;
    public Sprite keySprite;
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
    public int maxhealth = 10;
    public int health;
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
    public bool attacks;
    public Transform bulletTrnsform;
    public GameObject bullet;
    public float bulletpeed;
    public Sprite boxOpen;
    NavMeshAgent Agent;
     public Transform target;
    public bool attackButtonClick;
    // public bool isDeath;

    [Space]
    [Header("HEALTH")]
    public Image playerHealthFill;
    public float playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

       

        health = maxhealth;
       // healthBar = GetComponent<healthbarscript>();
    }
    

    // Update is called once per frame
    void Update()
    {
       
        float moveH = joystick.Horizontal;
        float moveV = joystick.Vertical;
        Vector2 moveDir = new Vector2(moveH, moveV);
        rb.linearVelocity = moveDir * speed;

      

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject.name);
        print(manager.isEnemyDoorOpen);
        if (collision.gameObject.tag == "keys")
        {
            Debug.Log("keytaken");
            manager.isPlayerGetKey = true;
            manager.keyImg.SetActive(true);
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(collision.gameObject);
            
            //collision box open sprite
        }
        else  if (collision.gameObject.tag=="box")
        {
            Debug.Log("Box hit!");
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            collision.gameObject.GetComponent<SpriteRenderer>().sprite = boxOpen;
           // Destroy(collision.gameObject);
            //collision box open sprite

        }
        else if (collision.gameObject.tag == "EnemyDoor" && manager.isPlayerGetKey)
        {
            Debug.Log("Enemy Door Closed!");
            manager.isEnemyDoorOpen = false;
        }
        else if(collision.gameObject.tag == "door" && manager.isEnemyDoorOpen==false && manager.isPlayerGetKey)
        {
            Debug.Log("LEVEL UP!");
            Destroy(collision.gameObject);
            LevelUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
           
            Debug.Log("Enemy hit!");
            attackEnemy = true;
          //  animatorRef.SetTrigger("damage");

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        attacks = false;
        attackEnemy = false;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

        Debug.Log("exit collider");
        keyTaken = false;
       
    }
    
   
    public void PlayerAttack()
    {

        ///// knife Function;
        if (manager.isEnemyDoorOpen && !manager.playerRef.playerDeath)

        {
            attacks = true;
            Debug.Log("dfdsf");
            animatorRef.SetTrigger("playerattack");
        }

        //// Gun Function
        ///
       

    }
    public void LevelUp()
    { 
        if(managerRef.enemys.Length>=0)
        {
            for (int i = 0; i <= managerRef.enemys.Length-1; i++)
            {
                Destroy(managerRef.enemys[i].gameObject);
                // managerRef.enemys[i].GetComponent<Enemy>().idleFun();
            }
        }
        player.tag = "unplayer";
        winpage.SetActive(true);


    }

    public void GameOver()
    {
        animatorRef.SetTrigger("death");
        for (int i = 0; i <= managerRef.enemys.Length - 1; i++)
        {
            //  managerRef.enemys[i].GetComponent<Enemy>().idleFun();
            Destroy(managerRef.enemys[i].gameObject);
            // Destory(managerRef.enemys[i]);

        }
        player.tag = "unplayer";
        StartCoroutine(ShowdeathPageWithDelay(1f)); // 1 second delay
    }
    IEnumerator ShowdeathPageWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameoverpage.SetActive(true);
    }
}
