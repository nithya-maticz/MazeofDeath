using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int bulletDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="enemy")
        {
            Debug.Log("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
            collision.gameObject.GetComponent<Enemy>().damageEnemy(bulletDamage);
            Destroy(gameObject);
        }
    }
}
