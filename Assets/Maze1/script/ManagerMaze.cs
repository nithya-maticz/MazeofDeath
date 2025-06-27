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

    public Player playerRef;
    public bool attackPlayer;
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
    public List<ZombieDoor> ZombieDoors;
    public Blood BloodPrefab;
    public Transform BloodTransform;

    [Header("SPRITES")]
    public Sprite SpriteBoxOpen;
    public Sprite SpriteBoxClose;
    public Sprite DoorOpen;
    public Sprite DoorClose;

    [Header("TREASURE HANDLE")]
    public Animator TreasureAnimation;
    public GameObject TreasureKey;
    public GameObject TreasureMedikit;

    [Header("WIN PAGE")]
    public GameObject WinPage;

    [Header("GAMEOVER")]
    public GameObject GameOverPage;
    void Start()
    {
        
        SpawnEnemy();
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
      //  StartCoroutine(SpawnEnemys());

    }
    // Update is called once per frame
    void Update()
    {
        
    }
  
  /*  IEnumerator SpawnEnemys()
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
       
        
    }*/

    public void AttackEnemy()
    {
        attackPlayer=true;
    }
    public void RestartFun()
    {
        SceneManager.LoadScene("Maze1");
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

    public void CheckLevelUp()
    {
        bool allclosed = true;

        foreach(ZombieDoor door in ZombieDoors)
        {
            if(!door.isClosed)
                allclosed = false;
        }

        if (allclosed)
            Invoke("LevelUP", 1f);
        else
            return;
    }


    void LevelUP()
    {
        foreach(Enemy enemy in Enemies)
        {
            Destroy(enemy);
        }
        WinPage.SetActive(true);
    }

    public void GameOver()
    {
        Invoke("game_over", 1f);
    }

    void game_over()
    {
        GameOverPage.SetActive(true);

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
