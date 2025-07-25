using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform Target;
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


            transform.position = Vector3.MoveTowards(transform.position, Target.position, speed * Time.deltaTime);


            Vector2 direction = Target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
}
