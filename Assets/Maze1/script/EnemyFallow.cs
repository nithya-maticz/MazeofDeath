using UnityEngine;

public class EnemyFallow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed;
    private Transform target;
    public Player playerRef;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("distance" +Vector2.Distance(transform.position, target.position));
        if(Vector2.Distance(transform.position,target.position)>7 )
        {
            Debug.Log("stoping");
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
