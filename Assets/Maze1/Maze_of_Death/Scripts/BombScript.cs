using UnityEngine;

public class BombScript : MonoBehaviour
{
    
    public SharedPathFollower _currentEnemy;
    public bool IsStayBomb;
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
        if (_currentEnemy == null)
        {
            if (collision.CompareTag("enemy"))
            {
                Debug.Log("Enter... Bomb Script");
                 _currentEnemy = collision.GetComponent<SharedPathFollower>();
                IsStayBomb = true;
            }
        }
    }

   

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            if (collision.GetComponent<SharedPathFollower>() == _currentEnemy)
            {
                Debug.Log("Exit...");
                _currentEnemy = null;
                IsStayBomb = false;
            }
        }

    }
}
