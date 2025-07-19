using UnityEngine;
using UnityEngine.EventSystems;

public class Game_Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public Joystick movementJoystick; // Assign your movement joystick
    public float moveSpeed = 5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;
    private Vector2 lastTouchPosition;
    private bool isRotating = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleTouchRotation(); // Right-thumb like drag for 360� rotation
    }

    void HandleMovement()
    {
        Vector2 input = Vector2.zero;

        // Joystick input
        if (movementJoystick != null)
        {
            input = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
        }

        // Keyboard fallback
        if (input == Vector2.zero)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        rb.linearVelocity = input.normalized * moveSpeed;
    }

    /* void HandleTouchRotation()
     {
 #if UNITY_EDITOR
         // For testing in editor using right mouse drag
         if (Input.GetMouseButtonDown(1))
         {
             lastTouchPosition = Input.mousePosition;
             isRotating = true;
         }
         else if (Input.GetMouseButton(1) && isRotating)
         {
             RotateToward(Input.mousePosition);
             lastTouchPosition = Input.mousePosition;
         }
         else if (Input.GetMouseButtonUp(1))
         {
             isRotating = false;
         }
 #else
         // Touch input
         foreach (Touch touch in Input.touches)
         {
             if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                 continue; // Ignore if touching joystick or UI

             // Start rotating
             if (touch.phase == TouchPhase.Began)
             {
                 lastTouchPosition = touch.position;
                 isRotating = true;
             }
             else if (touch.phase == TouchPhase.Moved && isRotating)
             {
                 RotateToward(touch.position);
                 lastTouchPosition = touch.position;
             }
             else if (touch.phase == TouchPhase.Ended)
             {
                 isRotating = false;
             }
         }
 #endif
     }*/

    void HandleTouchRotation()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            lastTouchPosition = Input.mousePosition;
            isRotating = true;
        }
        else if (Input.GetMouseButton(1) && isRotating)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
            float rotationDelta = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, -rotationDelta); // Negative for clockwise drag right
            lastTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

#else
    foreach (Touch touch in Input.touches)
    {
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            continue;

        if (touch.position.x < Screen.width / 2f)
            continue; // Ignore left side

        if (touch.phase == TouchPhase.Began)
        {
            lastTouchPosition = touch.position;
            isRotating = true;
        }
        else if (touch.phase == TouchPhase.Moved && isRotating)
        {
            Vector2 delta = touch.position - lastTouchPosition;
            float rotationDelta = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, -rotationDelta); // Negative = right is clockwise
            lastTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            isRotating = false;
        }
    }
#endif
    }


    void RotateToward(Vector2 screenPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 direction = (worldPos - transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

}
