using UnityEngine;

public class MinimapFollowPlayer : MonoBehaviour
{
    // Transform player;          // Assign your player Transform
    public BoxCollider2D boundary;    // Assign your BoxCollider2D in inspector

    private Vector2 minLimits;
    private Vector2 maxLimits;

    private float cameraHalfWidth;
    private float cameraHalfHeight;

    private Camera minimapCam;

    void Start()
    {
        minimapCam = GetComponent<Camera>();
        if (minimapCam.orthographic)
        {
            cameraHalfHeight = minimapCam.orthographicSize;
            cameraHalfWidth = cameraHalfHeight * minimapCam.aspect;
        }

        Bounds bounds = boundary.bounds;
        minLimits = bounds.min;
        maxLimits = bounds.max;
    }

    void LateUpdate()
    {
        
    }

    private void FixedUpdate()
    {
        if(Player.Instance != null)
        {
            Vector3 targetPos = Player.Instance.transform.position;
            // Clamp X and Y (for 2D)
            float clampedX = Mathf.Clamp(targetPos.x, minLimits.x + cameraHalfWidth, maxLimits.x - cameraHalfWidth);
            float clampedY = Mathf.Clamp(targetPos.y, minLimits.y + cameraHalfHeight, maxLimits.y - cameraHalfHeight);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
