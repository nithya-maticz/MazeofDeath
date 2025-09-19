using UnityEngine;

public class EnemyRange : MonoBehaviour,IGetBulletDemage
{
    [SerializeField] private SharedPathFollower enemy;
    [SerializeField] private ZombieHealth health;
    private bool lastInRange = false; // track previous state

    private void UpdateAnimState(bool inRange)
    {
        if (lastInRange == inRange) return; // 🔑 prevent jitter
        lastInRange = inRange;

        enemy.animator.SetBool("IsAttacking", inRange);
        enemy.animator.SetBool("IsWalking", !inRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.playerInRange = true;
            UpdateAnimState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.playerInRange = false;
            UpdateAnimState(false);
        }
    }

    public void GetBullet(int demage)
    {
        health.GetBullet(demage);
    }
}
