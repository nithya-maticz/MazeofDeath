using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform firePoint;
    public GameObject ammoType;
    public float shotSpeed;
    public float shotCounter, fireRate;
   // public Animator playerAni;

    void Start()
    {
        //playerAni = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetMouseButton(0))
        {
            shotCounter -= Time.deltaTime;
            if(shotCounter<=0)
            {
                shotCounter = fireRate;
                shoot();
            }

        }*/
    }
    public void shoot()
    {
        GameObject shot = Instantiate(ammoType, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = shot.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * shotSpeed, ForceMode2D.Impulse);
        Destroy(shot.gameObject, 1f);
    }
}
