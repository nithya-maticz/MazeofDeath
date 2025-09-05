/*using UnityEngine;

public class EnemyRange : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] SharedPathFollower enemy;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    private string currentAnimState = "";

    void Update()
    {
        if (enemy.playerInRange)
        {
            ChangeAnimationState("Attack");
        }
        else
        {
            ChangeAnimationState("Walk");
        }
    }

    void ChangeAnimationState(string newState)
    {
        // Don’t restart if it’s the same state
        if (currentAnimState == newState) return;

        enemy.animator.ResetTrigger(currentAnimState); // optional, if using triggers
        enemy.animator.SetTrigger(newState);

        currentAnimState = newState;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            enemy.playerInRange = true;
            //enemy.isCollidingWithPlayer = true;
            enemy.animator.SetTrigger("Attack");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.playerInRange = false;
            //enemy.isCollidingWithPlayer = false;
            enemy.animator.SetTrigger("Walk");
        }
    }
}
*/

using UnityEngine;

public class EnemyRange : MonoBehaviour
{
    [SerializeField] private SharedPathFollower enemy;
    private string currentAnimState = "";

    private void SetAnimState(string newState)
    {
        if (currentAnimState == newState) return; // avoid restart
        enemy.animator.ResetTrigger(currentAnimState);
        enemy.animator.SetTrigger(newState);
        currentAnimState = newState;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.playerInRange = true;
            SetAnimState("Attack");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.playerInRange = false;
            SetAnimState("Walk");
        }
    }
}
