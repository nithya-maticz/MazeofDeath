using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public static SmoothFollowCamera Instance;

    [Header("Smooth Settings")]
    public float positionSmoothTime = 0.1f;
    public float rotationSmoothTime = 0.1f;
    public float rotationOffset = 90f;

    [Header("Boundary Settings")]
    public SpriteRenderer backgroundBounds; // Assign your background SpriteRenderer here

    private Vector3 velocity = Vector3.zero;
    private float rotationVelocity = 0f;
    private Rigidbody2D targetRb;

    private float camHeight;
    private float camWidth;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }

        // Calculate camera view size in world units
        Camera cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = cam.aspect * camHeight;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        // Clamp position to background bounds
        if (backgroundBounds != null)
        {
            Bounds bounds = backgroundBounds.bounds;

            float minX = bounds.min.x + camWidth;
            float maxX = bounds.max.x - camWidth;
            float minY = bounds.min.y + camHeight;
            float maxY = bounds.max.y - camHeight;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Smooth position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            positionSmoothTime
        );

        // Optional rotation (commented out)
        /*
        float targetZ = (target.eulerAngles.z + rotationOffset) % 360f;
        float currentZ = transform.eulerAngles.z;
        float newZ = Mathf.SmoothDampAngle(currentZ, targetZ, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);
        */
    }
}


