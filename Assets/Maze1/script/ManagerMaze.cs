using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerMaze : MonoBehaviour
{
    public GameObject enemy;
    public Transform spawnPosition;
    public Transform player;
    public Player playerRef;
    public bool attackPlayer;
    public GameObject[] enemys;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnEnemy();
    }
    public void SpawnEnemy()
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
            yield return new WaitForSeconds(10f);
            Debug.Log("outside true");
            if (playerRef.openDoor == false && playerRef.playerDeath==false)
            {
                Debug.Log("inside true");
                GameObject enemyPrefab = Instantiate(enemy, spawnPosition.position, spawnPosition.rotation);
                enemyPrefab.GetComponent<Enemy>().target = player;
                enemys = GameObject.FindGameObjectsWithTag("enemy");
            }
            }
       
        
    }

    public void AttackEnemy()
    {
        attackPlayer=true;
    }
    public void RestartFun()
    {
        SceneManager.LoadScene("Maze1");
    }

}
