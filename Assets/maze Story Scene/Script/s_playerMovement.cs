using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
public class s_playerMovement : MonoBehaviour
{
    public static s_playerMovement Instance;
    [Header("Joystick Movement")]
    public VariableJoystick movementJoystick;
    public float moveSpeed = 5f;

    [Header("Rotation Settings")]
    public float rotationSensitivity = 0.2f;

    [Header("Animations")]
    public Animator _Animator;
    private Rigidbody2D rb;

    private Vector2 moveInput;
    private float rotationInput = 0f;
    private Vector2 lastTouchPosition;
    private bool isRotating = false;
    private bool wasWalking = false;
    
   


    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {

       // movementJoystick = StoryManager.Instance.joystick;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
      
        _Animator.ResetTrigger("walk");
        moveInput = Vector2.zero;

        if (movementJoystick != null &&
            (Mathf.Abs(movementJoystick.Horizontal) > 0.1f || Mathf.Abs(movementJoystick.Vertical) > 0.1f))
        {
            moveInput = new Vector2(movementJoystick.Vertical, movementJoystick.Horizontal);
        }

        bool isWalking = moveInput.sqrMagnitude > 0.01f;

        Debug.Log(isWalking);
        if (!isWalking && wasWalking)
        {
            _Animator.ResetTrigger("walk");
           
        }

        if (isWalking && (!wasWalking))
        {
            _Animator.ResetTrigger("idle");
          //  _Animator.SetTrigger(isGun ? "WalkWithGun" : "Walk");
        }

        wasWalking = isWalking;
       



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
            rotationInput = delta.x;
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



}
