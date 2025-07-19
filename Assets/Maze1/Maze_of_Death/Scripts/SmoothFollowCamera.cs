using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float positionSmoothTime = 0.1f;
    public float rotationSmoothTime = 0.1f;
    public float rotationOffset = 90f; // Try 0f, 90f, or -90f depending on your sprite direction

    private Vector3 velocity = Vector3.zero;
    private float rotationVelocity = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // ----- Smooth position -----
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, positionSmoothTime);

        // ----- Smooth rotation -----
        float targetZ = target.eulerAngles.z + rotationOffset;
        float currentZ = transform.eulerAngles.z;

        float newZ = Mathf.SmoothDampAngle(currentZ, targetZ, ref rotationVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0f, 0f, newZ);
    }
}
