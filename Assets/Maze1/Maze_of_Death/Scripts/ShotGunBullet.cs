using UnityEngine;

public class ShotGunBullet : MonoBehaviour
{
    
   public Transform Target;
    public int BulletDamage;

    public float speed = 10f;
    public bool GO;

    void Update()
    {
        if(GO)
        {
            if (Target == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.Translate(Vector3.up * speed * Time.deltaTime);
            // transform.position = Vector3.MoveTowards(transform.position, Target.position, speed * Time.deltaTime);


            /* Vector2 direction = Target.position - transform.position;
             float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
             transform.rotation = Quaternion.Euler(0f, 0f, angle);*/

           /* Vector2 direction = Target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);*/
        }


        /*// Destroy if very close (arrived)
        if (Vector3.Distance(transform.position, Target.position) < 0.1f)
        {
            Destroy(gameObject);
            Destroy(Target.gameObject);
        }*/

        if(this.transform.position == Target.position)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("enemy") || collision.CompareTag("enemyDetected"))
        {
            
            SharedPathFollower enemy = collision.GetComponent<SharedPathFollower>();
            enemy.Health = enemy.Health - BulletDamage;
            Debug.Log("Enemy Health" + enemy.Health);
            enemy.HealthUpdate();
            Destroy(gameObject);
        }
       
       
    }
}

