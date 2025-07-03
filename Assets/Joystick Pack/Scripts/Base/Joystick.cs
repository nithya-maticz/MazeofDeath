using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Animator Animator1;
    public bool playerMovement;

    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    public float HandleRange { get => handleRange; set => handleRange = Mathf.Abs(value); }
    public float DeadZone { get => deadZone; set => deadZone = Mathf.Abs(value); }
    public AxisOptions AxisOptions { get => axisOptions; set => axisOptions = value; }
    public bool SnapX { get => snapX; set => snapX = value; }
    public bool SnapY { get => snapY; set => snapY = value; }

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;
    [SerializeField] private float sensitivity = 1.5f;

    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;

    private RectTransform baseRect;
    private Canvas canvas;
    private Camera cam;
    private Vector2 input = Vector2.zero;

    // For smooth rotation
    private Quaternion targetRotation;
    public float rotationSpeed = 360f; // Degrees per second
    private Transform playerTransform;

    private float currentZ;
    private float rotationVelocity;


    protected virtual void Start()
    {
        Animator1 = FindObjectOfType<Player>().animatorRef;
        playerTransform = FindObjectOfType<ManagerMaze>().playerRef.transform;

        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;

        targetRotation = playerTransform.localRotation;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        playerMovement = FindObjectOfType<Player>().playerDeath;
        if (!playerMovement)
        {
            Animator1.SetBool("run", true);
            OnDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!playerMovement)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            input = (eventData.position - position) / (radius * canvas.scaleFactor);

            FormatInput();
            HandleInput(input.magnitude, input.normalized, radius, cam);
            handle.anchoredPosition = input * radius * handleRange;

            Vector2 dir = handle.anchoredPosition;

            /* if (dir.magnitude > 0.1f)
             {
                 if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                 {
                     // Horizontal rotation
                     targetRotation = dir.x > 0 ?
                         Quaternion.Euler(0f, 0f, 180f) : // Right
                         Quaternion.Euler(0f, 0f, 0f);   // Left
                 }
                 else
                 {
                     // Vertical rotation
                     targetRotation = dir.y > 0 ?
                         Quaternion.Euler(0f, 0f, 270f) : // Up
                         Quaternion.Euler(0f, 0f, 90f);   // Down
                 }
             }*/

            if (dir.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;


                // Normalize angle to 0–360
                if (angle < 0) angle += 360f;

                float snappedAngle = 0f;

                // Snap to 8 directions (every 45 degrees)
                if (angle >= 337.5f || angle < 22.5f)
                    snappedAngle = 180f; // Right
                else if (angle >= 22.5f && angle < 67.5f)
                    snappedAngle = 225f; // Up-Right
                else if (angle >= 67.5f && angle < 112.5f)
                    snappedAngle = 270f; // Up
                else if (angle >= 112.5f && angle < 157.5f)
                    snappedAngle = 315f; // Up-Left
                else if (angle >= 157.5f && angle < 202.5f)
                    snappedAngle = 0f;   // Left
                else if (angle >= 202.5f && angle < 247.5f)
                    snappedAngle = 45f;  // Down-Left
                else if (angle >= 247.5f && angle < 292.5f)
                    snappedAngle = 90f;  // Down
                else if (angle >= 292.5f && angle < 337.5f)
                    snappedAngle = 135f; // Down-Right


                targetRotation = Quaternion.Euler(0f, 0f, snappedAngle);
            }
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Animator1.SetBool("run", false);
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        /*if (playerTransform != null && !playerMovement)
        {
             playerTransform.localRotation = Quaternion.RotateTowards(
               playerTransform.localRotation,
               targetRotation,
             rotationSpeed * Time.deltaTime
             );
 
           *//* playerTransform.localRotation = Quaternion.Slerp(
             playerTransform.localRotation,
             targetRotation,
            rotationSpeed * Time.deltaTime / 360f
            );*//*
        }*/
        if (playerTransform != null && !playerMovement)
        {
            float targetZ = targetRotation.eulerAngles.z;
            float smoothedZ = Mathf.SmoothDampAngle(
                playerTransform.localEulerAngles.z,
                targetZ,
                ref rotationVelocity,
                0.2f // Smooth time (lower is faster)
            );

            playerTransform.localRotation = Quaternion.Euler(0f, 0f, smoothedZ);
        }
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        input = (magnitude > deadZone) ? (magnitude > 1 ? normalised : input * sensitivity) : Vector2.zero;
        input = Vector2.ClampMagnitude(input, 1f);
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0) return 0;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
                return (angle < 22.5f || angle > 157.5f) ? 0 : (value > 0 ? 1 : -1);
            else if (snapAxis == AxisOptions.Vertical)
                return (angle > 67.5f && angle < 112.5f) ? 0 : (value > 0 ? 1 : -1);
        }
        else
        {
            return value > 0 ? 1 : -1;
        }

        return 0;
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }