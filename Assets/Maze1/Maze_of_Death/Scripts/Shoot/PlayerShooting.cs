/*using UnityEngine;

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
}
*/

using UnityEngine;

[System.Serializable]
public class Gun
{
    public string gunName;
    public Transform firePoint;       // Fire point for this gun
    public float fireRate = 0.25f;
    public float range = 15f;
    public int pellets = 1;           // 1 = pistol, >1 = shotgun
    public float spread = 0f;         // Shotgun spread in degrees
    public int damage = 10;
    public GameObject bulletPrefab;   // Bullet sprite prefab
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

    [Header("Aim Pointer")]
    public GameObject aimDotPrefab;   // Red dot prefab
    private GameObject aimDotInstance;
    private SpriteRenderer aimDotRenderer;

    private float nextFireTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Game_Manager.Instance.GunShootBtn.onClick.AddListener(OnShootButton);
        GunSetting();

        // Instantiate red dot pointer
        if (aimDotPrefab != null)
        {
            aimDotInstance = Instantiate(aimDotPrefab);
            aimDotInstance.SetActive(false); // Hide until needed
            aimDotRenderer = aimDotInstance.GetComponent<SpriteRenderer>();
            if (aimDotRenderer == null)
            {
                Debug.LogWarning("AimDotPrefab needs a SpriteRenderer component!");
            }
        }
    }

    public void GunSetting()
    {
        Debug.Log("Gun Name: " + Game_Manager.Instance.PrimaryGun);
        if (Game_Manager.Instance.PrimaryGun == "Pistol")
            currentGun = pistol;
        else if (Game_Manager.Instance.PrimaryGun == "Shotgun")
            currentGun = shotgun;
    }

    void Update()
    {
        if (currentGun == null || currentGun.firePoint == null || aimDotInstance == null)
            return;

        Vector2 start = currentGun.firePoint.position;
        Vector2 dir = currentGun.firePoint.right;

        // Raycast to detect hit
        RaycastHit2D hit = Physics2D.Raycast(start, dir, currentGun.range, hitLayers);
        Vector2 hitPoint = (hit.collider != null) ? hit.point : start + dir * currentGun.range;

        // Show/hide dot based on toggle
        bool showDot = Game_Manager.Instance.AutoAimAndManualShootToggle.isOn && Game_Manager.Instance.IsGun;
        aimDotInstance.SetActive(showDot);

        if (showDot)
        {
            aimDotInstance.transform.position = hitPoint;

            // Change color based on hit
            if (hit.collider != null &&
                (hit.collider.CompareTag("enemy") || hit.collider.CompareTag("enemyDetected")))
            {
                aimDotRenderer.color = Color.red; // Enemy detected
            }
            else
            {
                aimDotRenderer.color = Color.green; // Default
            }
        }

        // Optional: debug line
        Color lineColor = (hit.collider != null) ? Color.red : Color.green;
        Debug.DrawLine(start, hitPoint, lineColor);
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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
}

