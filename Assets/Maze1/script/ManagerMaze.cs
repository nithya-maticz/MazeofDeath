using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class ManagerMaze : MonoBehaviour
{
    public GameObject enemy;
    public Transform spawnPosition;
    //public Transform player;
    public Player playerRef;
    public bool attackPlayer;
    public GameObject[] enemys;
    public bool isEnemyDoorOpen;
    public bool isPlayerGetKey;
    public GameObject keyImg;
    public GameObject Playerweapon;
    public bool isGameOver;

    public static ManagerMaze instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        SpawnEnemy();
        isEnemyDoorOpen = true;
        isPlayerGetKey = false;
        isGameOver = false;
        keyImg.SetActive(false);
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
           
            if (isEnemyDoorOpen && !playerRef.playerDeath)
            {
                Debug.Log("inside true");
                GameObject enemyPrefab = Instantiate(enemy, spawnPosition.position, spawnPosition.rotation);
                enemyPrefab.GetComponent<Enemy>().target = playerRef.transform;
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

    public void GameOver()
    {
        if(!isGameOver)
        {
            //action
            playerRef.GameOver();
            isGameOver = true;
        }

    }

   

}


[Serializable]
public class CharacterObj
{
    public ObjectDatas rightObj;
    public ObjectDatas leftObj;
}

[Serializable]
public class ObjectDatas
{
    public GameObject obj;

}
