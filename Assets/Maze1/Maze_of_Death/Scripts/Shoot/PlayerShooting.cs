using UnityEngine;

[System.Serializable]
public class Gun
{
    public string gunName;
    public Transform firePoint;         // Fire point for this gun
    public float fireRate = 0.25f;
    public float range = 15f;
    public int pellets = 1;             // 1 = pistol, >1 = shotgun
    public float spread = 0f;           // Shotgun spread in degrees
    public int damage = 10;
    public GameObject bulletPrefab;     // Bullet sprite prefab
}

public class PlayerShooting : MonoBehaviour
{
    [Header("Guns")]
    public Gun pistol;
    public Gun shotgun;
    public static PlayerShooting Instance;

    private Gun currentGun;

    [Header("Options")]
    public LayerMask hitLayers;

    private float nextFireTime;

    void Start()
    {
        Game_Manager.Instance.GunShootBtn.onClick.AddListener(OnShootButton);
        GunSetting();

    }
    private void Awake()
    {

        Instance = this;

    }
    public void GunSetting()
    {
        Debug.Log("Gun NAme " + Game_Manager.Instance.PrimaryGun);
        if (Game_Manager.Instance.PrimaryGun == "Pistol")
        {
            currentGun = pistol; // Default gun
        }
        else if (Game_Manager.Instance.PrimaryGun == "Shotgun")
        {
            currentGun = shotgun; // Default gun
        }
    }

    void Update()
    {
        if (currentGun != null && currentGun.firePoint != null)
        {
            Vector2 start = currentGun.firePoint.position;
            Vector2 dir = currentGun.firePoint.right; // FirePoint rotation

            // Debug ray for Scene view
            RaycastHit2D hit = Physics2D.Raycast(start, dir, currentGun.range, hitLayers);
            Vector2 end = (hit.collider != null) ? hit.point : start + dir * currentGun.range;

            Color lineColor = (hit.collider != null) ? Color.red : Color.green;
            Debug.DrawLine(start, end, lineColor);


        }
    }

    // ----------------- Shoot Button -----------------
    public void OnShootButton()
    {
        if (Time.time >= nextFireTime && currentGun != null)
        {
            nextFireTime = Time.time + currentGun.fireRate;
            Shoot();
        }
    }

    // ----------------- Switch Guns -----------------
    public void EquipPistol() => currentGun = pistol;
    public void EquipShotgun() => currentGun = shotgun;

    // ----------------- Shoot Logic -----------------
    void Shoot()
    {
        for (int i = 0; i < currentGun.pellets; i++)
        {
            // Spread angle around firePoint rotation
            float angleOffset = -currentGun.spread * (currentGun.pellets - 1) / 2f + i * currentGun.spread;
            float finalAngle = currentGun.firePoint.eulerAngles.z + angleOffset;
            Vector2 dir = Quaternion.Euler(0, 0, finalAngle) * Vector2.right;

            FireBullet(dir, currentGun);
        }
    }

    // ----------------- Fire Bullet -----------------
    void FireBullet(Vector2 direction, Gun gun)
    {
        if (gun.bulletPrefab == null) return;

        RaycastHit2D hit = Physics2D.Raycast(gun.firePoint.position, direction, gun.range, hitLayers);
        Vector2 endPoint = (hit.collider != null) ? hit.point : (Vector2)gun.firePoint.position + direction * gun.range;

        // Correct bullet rotation (for sprites facing up)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180f;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject bulletObj = Instantiate(gun.bulletPrefab, gun.firePoint.position, rot);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(gun.firePoint.position, endPoint, gun.damage);
        }

        // Optional: log hit
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("enemy") || hit.collider.CompareTag("enemyDetected"))
                Debug.Log("Hit enemy: " + hit.collider.name);
            else if (hit.collider.CompareTag("Wall"))
                Debug.Log("Hit wall!");
        }
    }

    // ----------------- Find Closest Enemy -----------------
    GameObject FindClosestEnemy(float maxRange)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDist && dist <= maxRange)
            {
                minDist = dist;
                closest = e;
            }
        }
        return closest;
    }



}


