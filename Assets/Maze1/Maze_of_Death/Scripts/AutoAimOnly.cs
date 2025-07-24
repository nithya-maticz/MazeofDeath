using UnityEngine;

public class AutoAimOnly : MonoBehaviour
{
    [Header("References")]
    public Transform weaponPivot;

    [Header("Vision Settings")]
    public float visionAngle = 60f;
    public float visionDistance = 6f;
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;

    public SharedPathFollower currentTarget;

    private void Update()
    {
        FindTargetInCone();
    }

    

    void FindTargetInCone()
    {
        // Turn off lock of previous target
        if (currentTarget != null && currentTarget.TargetLocked != null)
        {
            currentTarget.TargetLocked.SetActive(false);
        }

        currentTarget = null;
        float bestDistance = Mathf.Infinity;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(weaponPivot.position, visionDistance, enemyLayer);

        foreach (var enemyCollider in enemies)
        {
            Vector2 toEnemy = (enemyCollider.transform.position - weaponPivot.position).normalized;
            float angle = Vector2.Angle(weaponPivot.up, toEnemy);

            if (angle <= visionAngle / 2f)
            {
                float dist = Vector2.Distance(weaponPivot.position, enemyCollider.transform.position);
                RaycastHit2D hit = Physics2D.Raycast(weaponPivot.position, toEnemy, dist, obstacleLayer);

                if (!hit && dist < bestDistance)
                {
                    bestDistance = dist;
                    currentTarget = enemyCollider.GetComponent<SharedPathFollower>();
                }
            }
        }

        // Turn on lock of new target
        if (currentTarget != null && currentTarget.TargetLocked != null)
        {
            currentTarget.TargetLocked.SetActive(true);
        }
    }



    private void OnDrawGizmos()
    {
        if (weaponPivot == null) return;

        // Cone lines
        Gizmos.color = Color.red;
        Vector3 left = Quaternion.Euler(0, 0, visionAngle / 2f) * weaponPivot.up;
        Vector3 right = Quaternion.Euler(0, 0, -visionAngle / 2f) * weaponPivot.up;
        Gizmos.DrawLine(weaponPivot.position, weaponPivot.position + left * visionDistance);
        Gizmos.DrawLine(weaponPivot.position, weaponPivot.position + right * visionDistance);

        // Ray to target
        if (currentTarget != null)
        {
            Vector2 dir = (currentTarget.transform.position - weaponPivot.position).normalized;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(weaponPivot.position, dir * visionDistance);
        }
    }
}
