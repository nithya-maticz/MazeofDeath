using System;
using Unity.VisualScripting;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    [SerializeField] Animator _animator;
    public SharedPathFollower _currentEnemy;
    public bool IsStayEnemy;
    public Knife knife;
    [HideInInspector] public PolygonCollider2D collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_currentEnemy == null)
        {
            if (collision.CompareTag("enemy") || collision.CompareTag("enemyDetected"))
            {
                Debug.Log("Enter...");
                _currentEnemy = collision.GetComponent<SharedPathFollower>();
                IsStayEnemy = true;
                if (Game_Manager.Instance.AutoAttackToogle.isOn)
                {
                    PlayerMovements.Instance.isAttack = true;
                    _animator.SetTrigger("Attack");
                }
                //
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_currentEnemy == null)
        {
            if (collision.CompareTag("enemy"))
            {
                Debug.Log("Stay...");
                _currentEnemy = collision.GetComponent<SharedPathFollower>();
                IsStayEnemy = true;
               // _animator.SetTrigger("Attack");
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
                IsStayEnemy = false;
            }
        }
       
    }
}


[Serializable]
public class Knife
{
    public string name;
    public int demage;
}
