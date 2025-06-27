using System.Collections;
using UnityEngine;

public class ZombieDoor : MonoBehaviour
{
    public bool isClosed;
    public Transform SpawnPoint;
    public SpriteRenderer sprite;
   
    void Start()
    {
        ManagerMaze.instance.ZombieDoors.Add(this);
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

            yield return new WaitForSeconds(10f);
        }


    }
}
