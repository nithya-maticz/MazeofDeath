using UnityEngine;

public class EnemyPatroll : MonoBehaviour
{
    public Transform[] partrolPoints;
    public int targetPoint;
    public float speed;
    public Animator animatorRef;
    public float rotationSpeed = 5f;
    public bool destory;
    
    public float reachThreshold = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPoint = 0;
        animatorRef.SetTrigger("walk");
       
    }

    // Update is called once per frame
    void Update()
    {
     if(destory==false)
        {
            if (transform.position == partrolPoints[targetPoint].position)
            {
                increaseTargetInt();
            }

            transform.position = Vector3.MoveTowards(transform.position, partrolPoints[targetPoint].position, speed * Time.deltaTime);
        }
        
        //Debug.Log(transform.position);
    }
    public void increaseTargetInt()


    {
        targetPoint++;
        
        if (targetPoint >= partrolPoints.Length)
        {
            targetPoint = 0;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Debug.Log("EEEEEEEEEEEEEEEEE");
        }
        if (targetPoint == 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.CompareTag("attack") && (StoryManager.Instance.knifeTaken))
        {
            StoryManager.Instance.enemyDestory = true;
            Debug.Log("Attack function");
          
            Invoke("DestoryEnemy",0.5f);
        }
    }
    

    public void DestoryEnemy()
    {
       
        StoryManager.Instance.content5visiblefun();
        Destroy(gameObject);
    }
}



