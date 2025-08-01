using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public static SmoothFollowCamera Instance;

    [Header("Smooth Settings")]
    [Tooltip("How quickly the camera follows position.")]
    public float positionSmoothTime = 0.1f;
    [Tooltip("How quickly the camera follows rotation.")]
    public float rotationSmoothTime = 0.1f;
    [Tooltip("Rotation offset for aligning the camera with the target's facing direction.")]
    public float rotationOffset = 90f;

    private Vector3 velocity = Vector3.zero;
    private float rotationVelocity = 0f;

    private Rigidbody2D targetRb; // Cache target Rigidbody2D for interpolation check

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
                // Ensure interpolation is enabled to avoid jitter
                targetRb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }
        
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ----- Smooth position -----
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            positionSmoothTime
        );

        // ----- Smooth rotation -----
       /* float targetZ = (target.eulerAngles.z + rotationOffset) % 360f;
        float currentZ = transform.eulerAngles.z;
        float newZ = Mathf.SmoothDampAngle(currentZ, targetZ, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);*/
    }

    
}

