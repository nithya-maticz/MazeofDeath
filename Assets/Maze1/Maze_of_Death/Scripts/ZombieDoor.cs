using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieDoor : MonoBehaviour
{
    public bool isClosed;
    public Transform SpawnPoint;
    public SpriteRenderer sprite;
    public float waitTime;
    public GameObject light;
    public List<Transform> doorPatrolPoints;
    public bool isPlayerDoor;
    public SpriteRenderer bg;
    public GameObject keyImage;
    void Start()
    {
        if(GridLoader.Instance.doorBgSprite != null)
            bg.sprite = GridLoader.Instance.doorBgSprite;

        if (!isPlayerDoor)
        {
            GameObject enemyPrefab = Instantiate(Game_Manager.Instance.EnemyPrefab, SpawnPoint);
            Game_Manager.Instance.ZombieDoors.Add(this);
            Game_Manager.Instance.ZombieDoorCount();
            enemyPrefab.GetComponent<SharedPathFollower>().isPatrolDoor = true;
            enemyPrefab.GetComponent<SharedPathFollower>().patrolPoints = doorPatrolPoints;
            Game_Manager.Instance.Enemies.Add(enemyPrefab.GetComponent<SharedPathFollower>());
            Game_Manager.Instance.EnemyCount();
           // SpawnEnemyFromDoor();
        }
        
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
        while (true)
        {
            yield return new WaitForSeconds(waitTime / 2);
           
            if (!isClosed)
            {
                if(Game_Manager.Instance.Enemies.Count <= 8)
                {
                    GameObject enemyPrefab = Instantiate(Game_Manager.Instance.EnemyPrefab, SpawnPoint);
                    Game_Manager.Instance.Enemies.Add(enemyPrefab.GetComponent<SharedPathFollower>());
                    Game_Manager.Instance.EnemyCount();
                }
            }
            yield return new WaitForSeconds(waitTime/2);
        }
    }
}
