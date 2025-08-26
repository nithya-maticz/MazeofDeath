using System.Collections;
using UnityEngine;

public class EnemyDirection : MonoBehaviour
{
    public SharedPathFollower follower;
    public bool playerDetected;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyDirection"))
        {
            if (playerDetected && !swappedRecently)
            {
                // Ensure only one of them does the swap (using instance ID)
                if (gameObject.GetInstanceID() > collision.gameObject.GetInstanceID())
                {
                    Vector3 posA = follower.transform.position;
                    Vector3 posB = collision.GetComponent<EnemyDirection>().follower.transform.position;

                    follower.transform.position = posB;
                    collision.GetComponent<EnemyDirection>().follower.transform.position = posA;

                    Debug.Log("Swapped...");

                    // Start cooldown to avoid repeated swapping
                    StartCoroutine(SwapCooldown());
                }
            }
        }
        
    }

    private bool swappedRecently = false;



    private IEnumerator SwapCooldown()
    {
        swappedRecently = true;
        yield return new WaitForSeconds(0.5f); // half a second delay before next swap
        swappedRecently = false;
    }
}
