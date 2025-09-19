using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovements : MonoBehaviour
{
    public static PlayerMovements Instance;
    [Header("Joystick Movement")]
    private VariableJoystick movementJoystick;
    public float moveSpeed = 5f;
 

    [Header("Rotation Settings")]
    public float rotationSensitivity = 0.2f;

    [Header("Animations")]
    public Animator _Animator;

    private Vector2 moveInput;
    private float rotationInput = 0f;
    private Vector2 lastTouchPosition;
    private bool isRotating = false;
    public bool wasWalking = false;
    private bool wasIdle = false;
    public bool wasGun;
    public bool wasWalking1 = false;
    private bool wasIdle1 = false;
    public bool wasGun1;
    public bool isGun;
    private Rigidbody2D rb;
    public bool isAttack = false;
    [Header("Knife Attack Controller")]
    public KnifeAttack knifeAttack;

    [Header("Door")]
    private Coroutine closeDoorCoroutine;
    private WaitForSeconds waitFor2Sec;

    [Header("Manual Target")]
    public Transform ManualTarget;
    public Transform offSet;
    public Transform ShotGunbulletSpawn;
    public Transform PistolbulletSpawn;
    public float speed = 2f;

    // Bomb
    public GameObject bombSpawnPoint;
    public GameObject bombLight;

    public bool shotGun;
    public bool PistolGun;
    ///
   // public GameObject autoAim;



    private void Awake()
    {
        Instance = this;
        
    }
    private void Start()
    {
        
         movementJoystick = Game_Manager.Instance.movementJoystick;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
       
       
       
    }

    private void FixedUpdate()
    {
        HandleMovementInput();
    }


    private float playerSmoothVel; // field for rotation smoothing

    public void HandleMovementInput()
    {
        float moveH = movementJoystick.Vertical;
        float moveV = movementJoystick.Horizontal;

        bool isWalking = moveH != 0f || moveV != 0f;
        Vector2 moveDir = new Vector2(-moveH, moveV);

        // Movement
        rb.linearVelocity = moveDir * 2f;

        // Rotation
        if (isWalking)
        {
            float targetAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg + 180f;
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.z,
                targetAngle,
                ref playerSmoothVel,
                0.1f // matches your "rotation sensitivity"
            );

            transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
        }

        isGun = Game_Manager.Instance.IsGun;

        // ✅ Skip animation change logic if attacking
        if (isAttack) return;

        //shotGun = true;

       // Debug.Log("GUNN    " +Game_Manager.Instance.PrimaryGun);

        if (Game_Manager.Instance.PrimaryGun== "Shotgun")
        {
           
            if (!isGun && !isWalking && (!wasIdle || wasGun))
            {
                ResetAllTriggers();
                _Animator.SetTrigger("Idle");
                wasIdle = true;
            }
            else if (isGun && !isWalking && (!wasIdle || !wasGun))
            {
                ResetAllTriggers();
                _Animator.SetTrigger("shotgunidle");
                wasIdle = true;
            }
            else if (isGun && isWalking && (!wasWalking || !wasGun))
            {
                ResetAllTriggers();
                _Animator.SetTrigger("shotgunwalk");
                wasIdle = false;
            }
            else if (!isGun && isWalking && (!wasWalking || wasGun))
            {
                ResetAllTriggers();
                _Animator.SetTrigger("Walk");
                wasIdle = false;
            }
        }
        else if (Game_Manager.Instance.PrimaryGun == "Pistol")
        {
           

            if (!isGun && !isWalking && (!wasIdle || wasGun))
            {
                ResetAllTriggers();
                _Animator.SetTrigger("Idle");
                wasIdle = true;
            }
            else if (isGun && !isWalking && (!wasIdle || !wasGun))
            {
               
                    ResetAllTriggers();
                _Animator.SetTrigger("GunIdle");
                wasIdle = true;
            }
            else if (isGun && isWalking && (!wasWalking || !wasGun))
            {
                ResetAllTriggers();
                _Animator.SetTrigger("WalkWithGun");
                wasIdle = false;
            }
            else if (!isGun && isWalking && (!wasWalking || wasGun))
            {
                ResetAllTriggers();
                _Animator.SetTrigger("Walk");
                wasIdle = false;
            }
        }

       

        wasWalking = isWalking;
        wasGun = isGun;
    }


    public void ShorGunIdle()
    {
        _Animator.SetTrigger("shotgunidle");
    }
    public void PistolIdle()
    {
        _Animator.SetTrigger("GunIdle");
    }


    public void ResetAllTriggers()
    {
        _Animator.ResetTrigger("Idle");
        _Animator.ResetTrigger("GunIdle");
        _Animator.ResetTrigger("Walk");
        _Animator.ResetTrigger("WalkWithGun");
        _Animator.ResetTrigger("shotgunidle");
        _Animator.ResetTrigger("shotgunwalk");
    }


   

    void AttackEnd()
    {
       
        if (knifeAttack._currentEnemy != null)
        {
            /* Debug.Log("inside enemy");
             Debug.Log("inside enemy" + knifeAttack.IsStayEnemy);
             if (knifeAttack.IsStayEnemy)
             {
                 Debug.Log("Destroy...");
                 GameObject blood = Instantiate(Game_Manager.Instance.BloodPrefab, knifeAttack._currentEnemy.gameObject.transform.position, Quaternion.identity);
                 Game_Manager.Instance.Enemies.Remove(knifeAttack._currentEnemy);
                 for(int i = 0; i < Game_Manager.Instance.patrolAreas.Count; i++)
                 {
                     if (Game_Manager.Instance.patrolAreas[i]._id == knifeAttack._currentEnemy.patrolArea._id)
                     {
                         Game_Manager.Instance.patrolAreas[i].isOccupied = false;
                         break;
                     }
                 }

                 Destroy(knifeAttack._currentEnemy.gameObject);
                 Game_Manager.Instance.EnemyCount();
             }*/

            knifeAttack._currentEnemy.GetComponent<ZombieHealth>().KnifeDemage(knifeAttack, knifeAttack.knife.demage);
        }    
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyDoor") && Game_Manager.Instance.IsGetKey)
        {
            ZombieDoor zombieDoor = collision.GetComponent<ZombieDoor>();
            if (zombieDoor != null)
            {
                zombieDoor.light.SetActive(true);
                Game_Manager.Instance.IsShowKey = true;
                zombieDoor.keyImage.SetActive(true);
                //closeDoorCoroutine = StartCoroutine(CloseDoorAfterDelay(zombieDoor));
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
                Game_Manager.Instance.IsShowKey = false;
                zombieDoor.keyImage.SetActive(false);
            }
        }
    }

  
    public void CloseDoor(ZombieDoor zombieDoor)
    {
        if (zombieDoor == null) return;

        zombieDoor.isClosed = true;
        zombieDoor.sprite.sprite = Game_Manager.Instance.DoorCloseSprite;
        zombieDoor.light.SetActive(false);
        Game_Manager.Instance.IsShowKey = false;
        zombieDoor.keyImage.SetActive(false);

        zombieDoor.GetComponent<BoxCollider2D>().enabled = false;

        Debug.Log("Door closed!");
        Game_Manager.Instance.ZombieDoorCount();


    }

    void IsAttackOff()
    {
        isAttack = false;
        PlayerMovements.Instance.isAttack = false;
        PlayerMovements.Instance.wasWalking = false;
        PlayerMovements.Instance.wasGun = !PlayerMovements.Instance.isGun;
        PlayerMovements.Instance.HandleMovementInput(); // Resume walk/idle/gunwalk based on current input
    }

    
}
