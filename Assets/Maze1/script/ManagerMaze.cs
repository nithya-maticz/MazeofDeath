using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
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
    public List<KeyBox> KeyBoxes;
    public Blood BloodPrefab;
    public Transform BloodTransform;
    int _boxCount;
    public TMP_Text OpenedBoxText;

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

    public GameObject PlayerImage;
    void Start()
    {
       
        isPlayerGetKey = false;
        isGameOver = false;
        keyImg.SetActive(false);
        
    }

    private void Awake()
    {
        instance = this;
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
  
  
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

    public void CheckBoxCount()
    {
        _boxCount = 0;
        foreach(KeyBox box in KeyBoxes)
        {
           if (box.IsOpened)
           {
                _boxCount++;
           }
        }

        OpenedBoxText.text = _boxCount.ToString() + "/" +KeyBoxes.Count.ToString();
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
