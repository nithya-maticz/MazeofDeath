using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using NUnit.Framework;
using System.Collections.Generic;
public class ManagerMaze : MonoBehaviour
{
    public GameObject enemy;
    public Transform spawnPosition;
    //public Transform player;
    public Player playerRef;
    public bool attackPlayer;
    //public GameObject[] enemys;
    public bool isEnemyDoorOpen;
    public bool isPlayerGetKey;
    public GameObject keyImg;
    public GameObject Playerweapon;
    public bool isGameOver;

    public static ManagerMaze instance;
    public bool CreateEnemy;
    public List<Enemy> Enemies;
    public int EnemyCount;
    public SpriteRenderer EnemyDoorSprite;
    public SpriteRenderer PlayerDoorSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("SPRITES")]
    public Sprite SpriteBoxOpen;
    public Sprite SpriteBoxClose;
    public Sprite DoorOpen;
    public Sprite DoorClose;

    [Header("TREASURE HANDLE")]
    public Animator TreasureAnimation;
    public GameObject TreasureKey;
    public GameObject TreasureMedikit;
    void Start()
    {
        
        SpawnEnemy();
        isEnemyDoorOpen = true;
        isPlayerGetKey = false;
        isGameOver = false;
        keyImg.SetActive(false);
        
    }

    private void Awake()
    {
        instance = this;
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
                if(CreateEnemy)
                {
                    if(Enemies.Count < EnemyCount)
                    {
                        GameObject enemyPrefab = Instantiate(enemy, spawnPosition.position, spawnPosition.rotation);
                        enemyPrefab.GetComponent<Enemy>().target = playerRef.transform;
                        Enemies.Add(enemyPrefab.GetComponent<Enemy>());
                    }
                   
                   // enemys = GameObject.FindGameObjectsWithTag("enemy");

                }
               
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

   public void GetTreasure(BoxObjects boxObjects)
   {
        if(boxObjects == BoxObjects.Key)
        {
            TreasureAnimation.gameObject.SetActive(true);
            TreasureKey.SetActive(true);
            TreasureAnimation.SetTrigger("Key");
        }
        else if(boxObjects == BoxObjects.MediKit)
        {
            TreasureAnimation.gameObject.SetActive(true);
            TreasureMedikit.SetActive(true);
            TreasureAnimation.SetTrigger("MediKit");
        }
        else if(boxObjects == BoxObjects.Empty)
        {
            return;
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
