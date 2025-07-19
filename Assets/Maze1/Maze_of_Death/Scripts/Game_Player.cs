using UnityEngine;
using UnityEngine.EventSystems;

public class Game_Player : MonoBehaviour
{
    [Header("Joystick Movement")]
    public VariableJoystick movementJoystick;
    public float moveSpeed = 5f;

    [Header("Rotation Settings")]
    public float rotationSensitivity = 0.2f;

    private Vector2 moveInput;
    private float rotationInput = 0f;
    private Vector2 lastTouchPosition;
    private bool isRotating = false;

    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleMovement();     // Manual transform-based movement
        HandleRotation();     // Manual rotation
    }

    void HandleMovementInput()
    {
        moveInput = Vector2.zero;

        if (movementJoystick != null &&
            (Mathf.Abs(movementJoystick.Horizontal) > 0.1f || Mathf.Abs(movementJoystick.Vertical) > 0.1f))
        {
            moveInput = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
        }
#if UNITY_EDITOR
        else
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
#endif
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
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                continue;

            if (touch.position.x < Screen.width / 2f)
                continue;

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
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        // Move in local space (forward = transform.up)
        Vector3 moveDir = transform.up * moveInput.y + transform.right * moveInput.x;
        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (Mathf.Abs(rotationInput) < 0.1f)
            return;

        float rotationDelta = rotationInput * rotationSensitivity;
        transform.Rotate(Vector3.forward, rotationDelta);
    }
}
