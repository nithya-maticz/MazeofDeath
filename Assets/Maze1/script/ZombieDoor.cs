using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDoor : MonoBehaviour
{
    public bool isClosed;
    public Transform SpawnPoint;
    public SpriteRenderer sprite;
    public float waitTime;
    //ZombieDoor instance;
    public GameObject light;
    public List<Transform> doorPatrolPoints;
    void Start()
    {
        ManagerMaze.instance.ZombieDoors.Add(this);
        GameObject enemyPrefab = Instantiate(ManagerMaze.instance.enemy, SpawnPoint);
        enemyPrefab.GetComponent<Enemy>().isPatrolDoor = true;
        enemyPrefab.GetComponent<Enemy>().doorPatrolPoints = doorPatrolPoints;

    }

    private void Awake()
    {
        
    }
    public void SpawnEnemyFromDoor()
    {
        StartCoroutine(SpawnEnemys());
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemys()
    {
        print("Call 2");
        while (true)
        {
            if (!isClosed && !Player.Instance.playerDeath)
            {
                if(ManagerMaze.instance.CreateEnemy)
                {
                    GameObject enemyPrefab = Instantiate(ManagerMaze.instance.enemy, SpawnPoint);
                    enemyPrefab.GetComponent<Enemy>().targetPoint = Player.Instance.transform;
                    ManagerMaze.instance.EnemiesCount();
                   // ManagerMaze.instance.Enemies.Add(enemyPrefab.GetComponent<Enemy>());
                    /*if (ManagerMaze.instance.Enemies.Count < ManagerMaze.instance.EnemyCount)
                    {
                       
                    }*/
                }
            }

            yield return new WaitForSeconds(waitTime);
        }


    }
}
