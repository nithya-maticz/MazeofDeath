
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Animator Animator1;
    public bool playerMovement;
   
 
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }
    public GameObject Animator
    {
        get { return Animator; }
        //set { Animator = Animator }
    }

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
    public bool SnapX { get { return snapX; } set { snapX = value; } }
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;

    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    private RectTransform baseRect = null;

    private Canvas canvas;
    private Camera cam;

    private Vector2 input = Vector2.zero;

    protected virtual void Start()
    {

        Animator1 = FindObjectOfType<Player>().animatorRef;
       

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
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
       
        playerMovement = FindObjectOfType<Player>().playerDeath;
        Debug.Log("Player Move " + playerMovement);
        if (playerMovement==false)
        {
            Animator1.SetBool("run", true);
            Debug.Log("Down");
            OnDrag(eventData);
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (playerMovement == false)
        {
            var manager = FindObjectOfType<ManagerMaze>();
            cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            input = (eventData.position - position) / (radius * canvas.scaleFactor);

            FormatInput();
            HandleInput(input.magnitude, input.normalized, radius, cam);
            handle.anchoredPosition = input * radius * handleRange;

            Debug.Log("Joystick Input: " + handle.anchoredPosition);

            Vector2 dir = handle.anchoredPosition;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                // Horizontal movement is dominant
                if (dir.x > 0)
                {
                    manager.playerRef.transform.localRotation = Quaternion.Euler(0f, 0f, 180f); // Right
                    Debug.Log("Moving Right");
                }
                else
                {
                    manager.playerRef.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Left
                    Debug.Log("Moving Left");
                }
            }
            else
            {
                // Vertical movement is dominant
                if (dir.y > 0)
                {
                    manager.playerRef.transform.localRotation = Quaternion.Euler(0f, 0f, 270f); // Up
                    Debug.Log("Moving Up");
                }
                else
                {
                    manager.playerRef.transform.localRotation = Quaternion.Euler(0f, 0f, 90f); // Down
                    Debug.Log("Moving Down");
                }
            }
        }
    }


    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
         
            input = Vector2.zero;
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
        {
            Debug.Log("vertical"); ; ;
            input = new Vector2(input.x, 0f);
        }
           
        else if (axisOptions == AxisOptions.Vertical)
        {
            Debug.Log("vertical");;;
            input = new Vector2(0f, input.y);
        }
          
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
       
        if (value == 0)
        {
            Debug.Log("horizontal");
            return value;
        }
           

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {

                if (angle < 22.5f || angle > 157.5f)
                {
                    Debug.Log("horizontal");
                    return 0;
                }

                else
                {
                    Debug.Log("horizontal");
                    return (value > 0) ? 1 : -1;
                }

            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                {
                    Debug.Log("horizontal");
                    return 0;
                }

                else
                {
                    Debug.Log("horizontal");
                    return (value > 0) ? 1 : -1;
                }

            }
            return value;
           
        }
        else
        {
            if (value > 0)
            {
                Debug.Log("horizontal");
                return 1;
            }
               
            if (value < 0)
            {
                Debug.Log("horizontal");
                return -1;
            }
               
        }
        return 0;

       
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("up");
        Animator1.SetBool("run", false);
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
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