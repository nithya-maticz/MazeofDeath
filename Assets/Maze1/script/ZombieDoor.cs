using System.Collections;
using UnityEngine;

public class ZombieDoor : MonoBehaviour
{
    public bool isClosed;
    public Transform SpawnPoint;
    public SpriteRenderer sprite;
    public float waitTime;
    //ZombieDoor instance;

    void Start()
    {
        ManagerMaze.instance.ZombieDoors.Add(this);
    }

    private void Awake()
    {
        
    }
    public void StartGame()
    {
        print("Call 1");
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
                    if (ManagerMaze.instance.Enemies.Count < ManagerMaze.instance.EnemyCount)
                    {
                        GameObject enemyPrefab = Instantiate(ManagerMaze.instance.enemy, SpawnPoint);
                        enemyPrefab.GetComponent<Enemy>().target = Player.Instance.transform;
                        ManagerMaze.instance.Enemies.Add(enemyPrefab.GetComponent<Enemy>());
                    }
                }
            }

            yield return new WaitForSeconds(waitTime);
        }


    }
}
