using System.Collections;
using UnityEngine;

public class DirectionEnemy : MonoBehaviour
{
    [SerializeField] SharedPathFollower enemy;
    SharedPathFollower otherEnmy;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* private void OnTriggerEnter2D(Collider2D collision)
     {
         if (collision.CompareTag("Direction"))
         {
             DirectionEnemy dirEnemy = collision.GetComponent<DirectionEnemy>();
             if (dirEnemy == null) return;

             SharedPathFollower otherEnemy = dirEnemy.enemy;
             otherEnmy = dirEnemy.enemy;
             if (otherEnemy == null) return;

             if (!enemy.playerDetected && !swappedRecently)
             {
                 if (gameObject.GetInstanceID() > otherEnemy.gameObject.GetInstanceID())
                 {
                     enemy.isAllowRot = false;
                     otherEnmy.isAllowRot = false;
                      Vector3 posA = enemy.transform.position;
                     Vector3 posB = otherEnemy.transform.position;

                     enemy.transform.position = posB;
                     otherEnemy.transform.position = posA;

                    // Debug.Log("Swapped due to opposite facing...");
                     StartCoroutine(SwapCooldown());
                 }


             }
         }
     }*/

   

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Direction"))
        {
            DirectionEnemy dirEnemy = collision.GetComponent<DirectionEnemy>();
            if (dirEnemy == null) return;

            SharedPathFollower otherEnemy = dirEnemy.enemy;
            otherEnmy = dirEnemy.enemy;
            if (otherEnemy == null) return;

            if (!enemy.playerDetected && !swappedRecently)
            {
                if (gameObject.GetInstanceID() > otherEnemy.gameObject.GetInstanceID())
                {
                    enemy.isAllowRot = false;
                    otherEnmy.isAllowRot = false;
                    Vector3 posA = enemy.transform.position;
                    Vector3 posB = otherEnemy.transform.position;

                    enemy.transform.position = posB;
                    otherEnemy.transform.position = posA;

                    // Debug.Log("Swapped due to opposite facing...");
                    StartCoroutine(SwapCooldown());
                    
                }


            }
        }
    }


    private bool swappedRecently = false;

    private IEnumerator SwapCooldown()
    {
        swappedRecently = true;
        yield return new WaitForSeconds(.5f); // cooldown before next swap
        swappedRecently = false;
        enemy.isAllowRot = true;
        otherEnmy.isAllowRot = true;
    }

}
