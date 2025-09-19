/*using UnityEngine;

public class Bullet : MonoBehaviour
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

          // transform.Translate(Vector3.up * speed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, Target.position, speed * Time.deltaTime);


            *//* Vector2 direction = Target.position - transform.position;
             float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
             transform.rotation = Quaternion.Euler(0f, 0f, angle);*//*

            Vector2 direction = Target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }


        *//*// Destroy if very close (arrived)
        if (Vector3.Distance(transform.position, Target.position) < 0.1f)
        {
            Destroy(gameObject);
            Destroy(Target.gameObject);
        }*//*

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
*/

using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    private Vector2 targetPos;
    private int damage;
    public int bulletDamage;

    public void Initialize(Vector2 start, Vector2 end, int damage)
    {
        transform.position = start;
        targetPos = end;
        this.damage = damage;

        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, targetPos);
        float travelTime = distance / speed;
        float t = 0f;
        Vector2 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / travelTime;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Handle hit
        RaycastHit2D hit = Physics2D.Raycast(startPos, (targetPos - startPos).normalized, distance, LayerMask.GetMask("Enemy", "Wall"));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("enemy") || hit.collider.CompareTag("enemyDetected"))
            {
                Debug.Log("Bullet Detected : " +  hit.collider.name);
                hit.collider.GetComponent<IGetBulletDemage>().GetBullet(bulletDamage);
            /*    SharedPathFollower enemy = hit.collider.GetComponent<SharedPathFollower>();
                enemy.Health = enemy.Health - BulletDamage;
                Debug.Log("Enemy Health" + enemy.Health);
                enemy.HealthUpdate();
                Destroy(gameObject);*/
            }
        }

        Destroy(gameObject);
    }
}
