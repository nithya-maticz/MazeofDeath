using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f); // Keep Z offset as needed
    public static SmoothFollowCamera Instance;

    [Header("Smooth Settings")]
    public float positionSmoothTime = 0.1f;

    [Header("Boundary Settings")]
    public SpriteRenderer boundsRenderer; // The red boundary sprite

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    private float camHalfWidth;
    private float camHalfHeight;

    private Vector2 minBound;
    private Vector2 maxBound;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    private void Start()
    {
        if (boundsRenderer == null)
        {
            Debug.LogWarning("No bounds renderer assigned.");
            return;
        }

        Bounds bounds = boundsRenderer.bounds;
        minBound = bounds.min;
        maxBound = bounds.max;

        // Swap height/width due to 90-degree rotation
        camHalfWidth = cam.orthographicSize;
        camHalfHeight = camHalfWidth * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null || boundsRenderer == null) return;

        Vector3 targetPosition = target.position + offset;

        float clampedX = Mathf.Clamp(
            targetPosition.x,
            minBound.x + camHalfWidth,
            maxBound.x - camHalfWidth
        );

        float clampedY = Mathf.Clamp(
            targetPosition.y,
            minBound.y + camHalfHeight,
            maxBound.y - camHalfHeight
        );

        Vector3 clampedPos = new Vector3(clampedX, clampedY, offset.z);

        transform.position = Vector3.SmoothDamp(transform.position, clampedPos, ref velocity, positionSmoothTime);
    }

    private void OnDrawGizmos()
    {
        if (boundsRenderer == null) return;

        if (cam == null)
            cam = Camera.main;

        // Swap width/height because of Z rotation = 90
        float cameraWidth = cam.orthographicSize * 2;
        float cameraHeight = cameraWidth * cam.aspect;

        Bounds bounds = boundsRenderer.bounds;

        float minX = bounds.min.x + cameraWidth / 2f;
        float maxX = bounds.max.x - cameraWidth / 2f;
        float minY = bounds.min.y + cameraHeight / 2f;
        float maxY = bounds.max.y - cameraHeight / 2f;

        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}
