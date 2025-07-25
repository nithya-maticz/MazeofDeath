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
    private bool wasWalking = false;
    private Rigidbody2D rb;

    [Header("Knife Attack Controller")]
    public KnifeAttack knifeAttack;

    [Header("Door")]
    private Coroutine closeDoorCoroutine;
    private WaitForSeconds waitFor2Sec;

    [Header("Manual Target")]
    public Transform ManualTarget;
    

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
        HandleMovementInput();
        HandleRotationInput();
       
    }

    private void FixedUpdate()
    {
        HandleMovement(); // Do movement in FixedUpdate for better physics stability
        HandleRotation();
    }

    void HandleMovementInput()
    {
        moveInput = Vector2.zero;

        if (movementJoystick != null &&
            (Mathf.Abs(movementJoystick.Horizontal) > 0.1f || Mathf.Abs(movementJoystick.Vertical) > 0.1f))
        {
            moveInput = new Vector2(movementJoystick.Vertical, movementJoystick.Horizontal);
        }

        bool isWalking = moveInput.sqrMagnitude > 0.01f;
        if (isWalking != wasWalking)
        {
            _Animator.SetBool("isWalking", isWalking);
            wasWalking = isWalking;
        }
    }

    void HandleRotationInput()
    {
        rotationInput = 0f;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            lastTouchPosition = Input.mousePosition;
            isRotating = true;
        }
        else if (Input.GetMouseButton(1) && isRotating)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
            rotationInput = -delta.x;
            lastTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }
#else
        foreach (Touch touch in Input.touches)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId)) continue;
            if (touch.position.x < Screen.width / 2f) continue;

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isRotating = true;
            }
            else if (touch.phase == TouchPhase.Moved && isRotating)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                rotationInput = -delta.x;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isRotating = false;
            }
        }
#endif
    }

    void HandleMovement()
    {
        if (moveInput.sqrMagnitude < 0.01f) return;

        Vector3 moveDir = new Vector3(-moveInput.x, moveInput.y, 0f);
        moveDir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * moveDir;

        rb.MovePosition(rb.position + (Vector2)(moveDir.normalized * moveSpeed * Time.fixedDeltaTime));
    }

    void HandleRotation()
    {
        if (Mathf.Abs(rotationInput) < 0.1f) return;

        float rotationDelta = rotationInput * rotationSensitivity;
        transform.Rotate(Vector3.forward, rotationDelta);
    }

    void AttackEnd()
    {
        if(knifeAttack._currentEnemy != null)
        {
            if (knifeAttack.IsStayEnemy)
            {
                Debug.Log("Destroy...");
                GameObject blood = Instantiate(Game_Manager.Instance.BloodPrefab, knifeAttack._currentEnemy.gameObject.transform.position, Quaternion.identity);
                Game_Manager.Instance.Enemies.Remove(knifeAttack._currentEnemy);
                Destroy(knifeAttack._currentEnemy.gameObject);
                Game_Manager.Instance.EnemyCount();
            }
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
            }
        }
    }

    private IEnumerator CloseDoorAfterDelay(ZombieDoor zombieDoor)
    {
        yield return new WaitForSeconds(2f);
        
        CloseDoor(zombieDoor);
    }

    void CloseDoor(ZombieDoor zombieDoor)
    {
        if (zombieDoor == null) return;

        zombieDoor.isClosed = true;
        zombieDoor.sprite.sprite = Game_Manager.Instance.DoorCloseSprite;
        zombieDoor.light.SetActive(false);
        
        zombieDoor.GetComponent<BoxCollider2D>().enabled = false;

        Debug.Log("Door closed!");
        Game_Manager.Instance.ZombieDoorCount();


    }

   
}
