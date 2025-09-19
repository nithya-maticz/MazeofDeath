/*
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
*/


using UnityEngine;
using System.Collections;


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
    public int magazineSize = 10;     // bullets per magazine
    public int totalBullets = 50;     // reserve bullets
    public float reloadTime = 1f;     // reload duration
    public GameObject bulletPrefab;   // Bullet sprite prefab
}



public class PlayerShooting : MonoBehaviour
{
    [Header("Guns")]
    public Gun pistol;
    public Gun shotgun;
   

    public static PlayerShooting Instance;

    private Gun currentGun;
    private int currentAmmo;      // bullets in current magazine
    private int currentReserve;   // reserve bullets
    private bool isReloading = false;

    [Header("Options")]
    public LayerMask hitLayers;

    [Header("Aim Pointer")]
    public GameObject aimDotPrefab;   // Red dot prefab
    private GameObject aimDotInstance;
    private SpriteRenderer aimDotRenderer;

   // [Header("UI")]
  //  public TMP_Text ammoText;         // Assign in Inspector

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

        // Red dot pointer setup
        if (aimDotPrefab != null)
        {
            aimDotInstance = Instantiate(aimDotPrefab);
            aimDotInstance.SetActive(false);
            aimDotRenderer = aimDotInstance.GetComponent<SpriteRenderer>();
            if (aimDotRenderer == null)
                Debug.LogWarning("AimDotPrefab needs a SpriteRenderer component!");
        }

        Game_Manager.Instance.reloadBtn.onClick.AddListener(ReloadGun);
    }


    void ReloadGun()
    {
        StartCoroutine(Reload());
    }
    // ----------------- Gun Settings -----------------
    public void GunSetting()
    {
        if(Game_Manager.Instance.PrimaryGun == "Pistol")
        {
            EquipGun(pistol);
        }
        else if(Game_Manager.Instance.PrimaryGun == "Shotgun")
        {
            EquipGun(shotgun);
        }
       
    }


    
    public void EquipGun(Gun gun)
    {
        currentGun = gun;

        // Initialize ammo
        currentAmmo = Mathf.Min(currentGun.magazineSize, currentGun.totalBullets);
        currentReserve = currentGun.totalBullets - currentAmmo;

        UpdateAmmoUI();
    }

    public void EquipPistol() => EquipGun(pistol);
    public void EquipShotgun() => EquipGun(shotgun);

    // ----------------- Update Aim -----------------
    void Update()
    {
        if (currentGun == null || currentGun.firePoint == null || aimDotInstance == null) return;

        Vector2 start = currentGun.firePoint.position;
        Vector2 dir = currentGun.firePoint.right;

        RaycastHit2D hit = Physics2D.Raycast(start, dir, currentGun.range, hitLayers);
        Vector2 hitPoint = (hit.collider != null) ? hit.point : start + dir * currentGun.range;

        // Show/hide dot
        bool showDot = Game_Manager.Instance.AutoAimAndManualShootToggle.isOn && Game_Manager.Instance.IsGun;
        aimDotInstance.SetActive(showDot);

        if (showDot)
        {
            aimDotInstance.transform.position = hitPoint;

            // Change color based on hit
            if (hit.collider != null && (hit.collider.CompareTag("enemy") || hit.collider.CompareTag("enemyDetected")))
                aimDotRenderer.color = Color.red;
            else
                aimDotRenderer.color = Color.green;
        }

        // Optional debug line
        Color lineColor = (hit.collider != null) ? Color.red : Color.green;
        Debug.DrawLine(start, hitPoint, lineColor);
    }

    // ----------------- Shooting -----------------
    public void OnShootButton()
    {
        if (isReloading) return;

        if (Time.time >= nextFireTime && currentGun != null)
        {
            if (currentAmmo <= 0)
            {
                if (currentReserve > 0)
                    StartCoroutine(Reload());
                else
                    Debug.Log("Out of bullets!");
                return;
            }

            nextFireTime = Time.time + currentGun.fireRate;
            Shoot();
            currentAmmo--;
            UpdateAmmoUI(); // Update ammo display
        }
    }

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

    IEnumerator Reload()
    {
        if (currentReserve <= 0) yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        float elapsed = 0f;
        if (Game_Manager.Instance.reloadFillImage != null)
            Game_Manager.Instance.reloadFillImage.fillAmount = 0f; // reset fill

        while (elapsed < currentGun.reloadTime)
        {
            elapsed += Time.deltaTime;
            if (Game_Manager.Instance.reloadFillImage != null)
                Game_Manager.Instance.reloadFillImage.fillAmount = Mathf.Clamp01(elapsed / currentGun.reloadTime);
            yield return null;
        }

        // Add leftover bullets back to reserve
        currentReserve += currentAmmo;

        // Reload bullets
        int bulletsToReload = Mathf.Min(currentGun.magazineSize, currentReserve);
        currentAmmo = bulletsToReload;
        currentReserve -= bulletsToReload;

        isReloading = false;

        UpdateAmmoUI(); // Update UI after reload
        if (Game_Manager.Instance.reloadFillImage != null)
            Game_Manager.Instance.reloadFillImage.fillAmount = 0f; // Reset after complete

        Debug.Log("Reloaded! Ammo: " + currentAmmo + " | Reserve: " + currentReserve);
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
            bullet.Initialize(gun.firePoint.position, endPoint, gun.damage);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("enemy") || hit.collider.CompareTag("enemyDetected"))
                Debug.Log("Hit enemy: " + hit.collider.name);
            else if (hit.collider.CompareTag("Wall"))
                Debug.Log("Hit wall!");
        }
    }

    // ----------------- Ammo UI -----------------
    private void UpdateAmmoUI()
    {
        if (Game_Manager.Instance.bulletText != null && currentGun != null)
        {
            Game_Manager.Instance.bulletText.text = $"{currentAmmo} / {currentReserve}";
        }
    }
}
