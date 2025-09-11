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
            StartCoroutine(SpawnEnemyFromDoor());
        }
        
    }

    private void Awake()
    {
        
    }
    public IEnumerator SpawnEnemyFromDoor()
    {
        yield return new WaitForSeconds(Random.Range(1f, 8f));
        //wait for 1 to 8 seconds random and than start
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
                if(Game_Manager.Instance.Enemies.Count <= 9)
                {
                    bool isget = false;
                    GameObject enemyPrefab = Instantiate(Game_Manager.Instance.EnemyPrefab, SpawnPoint);
                    Game_Manager.Instance.Enemies.Add(enemyPrefab.GetComponent<SharedPathFollower>());
                    Game_Manager.Instance.EnemyCount();
                    for(int i = 0; i < Game_Manager.Instance.patrolAreas.Count; i++)
                    {
                        if (Game_Manager.Instance.patrolAreas[i]._id == Game_Manager.Instance.currentPatrolId)
                        {
                            if(!Game_Manager.Instance.patrolAreas[i].isOccupied)
                            {
                                enemyPrefab.GetComponent<SharedPathFollower>().patrolPoints = Game_Manager.Instance.patrolAreas[i].patrolPoints;
                                enemyPrefab.GetComponent<SharedPathFollower>().patrolArea = Game_Manager.Instance.patrolAreas[i];
                                Game_Manager.Instance.patrolAreas[i].isOccupied = true;
                                isget = true;
                                Game_Manager.Instance.currentPatrolId++;
                                break;
                            }
                            
                        }
                    }

                    if(!isget)
                    {
                        for (int i = 0; i < Game_Manager.Instance.patrolAreas.Count; i++)
                        {
                            if (!Game_Manager.Instance.patrolAreas[i].isOccupied)
                            {
                                enemyPrefab.GetComponent<SharedPathFollower>().patrolPoints = Game_Manager.Instance.patrolAreas[i].patrolPoints;
                                enemyPrefab.GetComponent<SharedPathFollower>().patrolArea = Game_Manager.Instance.patrolAreas[i];
                                Game_Manager.Instance.patrolAreas[i].isOccupied = true;
                                break;
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(waitTime/2);
        }
    }


}
