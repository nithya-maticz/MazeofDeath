using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using NavMeshPlus.Components;
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

    public Sprite damage1;
    public Sprite damage2;
    public Sprite damage3;
    public Sprite damage4;

    [Header("TREASURE HANDLE")]
    public Animator TreasureAnimation;
    public GameObject TreasureKey;
    public GameObject TreasureMedikit;

    [Header("WIN PAGE")]
    public GameObject WinPage;

    [Header("GAMEOVER")]
    public GameObject GameOverPage;

    public GameObject PlayerImage;

    [Header("PATROL POINTS")]
    public List<PatrolPoints> PatrolAreas;

    public Animator enemyAnimatorRef;
    public Image bloodImage;

    public Transform[] partolPoints;
    public int targetPoint;
    public float enemySpeed;

    
    [Header("Videoplayer")]
    public List<VideoControl> videoPlayer;

    public int count = 0;
    public  bool loadVideo;

    public Animator fadeAnimation;
    public GameObject FadeImg;
    public GameObject rawImage;
    public bool gameStart;
    public TextMeshProUGUI textMeshPro;
    
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;
    

    void Start()
    {

        isPlayerGetKey = false;
        isGameOver = false;
        
        keyImg.SetActive(false);

        for(int i=0;i<videoPlayer.Count;i++)
        {
            videoPlayer[i].videoPlayer.Pause();
        }

    }


    public void StartTyping()
    {
       if(videoPlayer.Count>count && gameStart==false)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText());
        }
       
        
    }

    IEnumerator TypeText()
    {
        textMeshPro.text = "";
        foreach (char c in videoPlayer[count].fullText)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        Debug.Log("completed");
        if( gameStart == false)
        {
            Invoke("playFun", 5f);
        }
       
        


    }

    public void playFun()
    {
       
        if (count<5&& gameStart==false)
        {
            FadeImg.SetActive(true);
            fadeAnimation.SetTrigger("fade");

        }
        else
        {

            Skipfun();
        }

    }


    public void Skipfun()
    {
        if(gameStart==false)
        {
            gameStart = true;
            Debug.Log("Skifun");
            FadeImg.SetActive(true);
            fadeAnimation.SetTrigger("fade");
            rawImage.SetActive(false);
            StopCoroutine(typingCoroutine);
            textMeshPro.text = "";
            Invoke("startgame", 2f);
        }
        



    }

    public void startgame()
    {
        FadeImg.SetActive(false);
        for (int i = 0; i < ZombieDoors.Count; i++)
        {
            ZombieDoors[i].StartGame();
        }

    }





    public void LoadVideo()
    {
        FadeImg.SetActive(true);
        fadeAnimation.SetTrigger("fade");
        loadVideo = true;
        

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
        attackPlayer = true;
    }
    public void RestartFun()
    {
        SceneManager.LoadScene("Maze1");
    }



    public void GetTreasure(BoxObjects boxObjects)
    {
        if (boxObjects == BoxObjects.Key)
        {
            TreasureAnimation.gameObject.SetActive(true);
            TreasureKey.SetActive(true);
            TreasureAnimation.SetTrigger("Key");
        }
        else if (boxObjects == BoxObjects.MediKit)
        {
            TreasureAnimation.gameObject.SetActive(true);
            TreasureMedikit.SetActive(true);
            TreasureAnimation.SetTrigger("MediKit");
        }
        else if (boxObjects == BoxObjects.Empty)
        {
            return;
        }
    }

    public void CheckLevelUp()
    {
        bool allclosed = true;

        foreach (ZombieDoor door in ZombieDoors)
        {
            if (!door.isClosed)
                allclosed = false;
        }

        if (allclosed)
            Invoke("LevelUP", 1f);
        else
            return;
    }


    void LevelUP()
    {
        foreach (Enemy enemy in Enemies)
        {
            Destroy(enemy);
        }
        WinPage.SetActive(true);
    }

    public void GameOver()
    {
       /* foreach (Enemy enemy in Enemies)
        {
            Debug.Log("GameOver-----------");

            Destroy(enemy.gameObject);
        }
        Invoke("game_over", 1f);*/
    }

    void game_over()
    {

        GameOverPage.SetActive(true);
    }

    public void CheckBoxCount()
    {
        _boxCount = 0;
        foreach (KeyBox box in KeyBoxes)
        {
            if (box.IsOpened)
            {
                _boxCount++;
            }
        }

        OpenedBoxText.text = _boxCount.ToString() + "/" + KeyBoxes.Count.ToString();

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

[Serializable]
public struct PatrolPoints
{
    public string Area;
    public List<Transform> Points;
}

[Serializable]
public class VideoControl
{
  
    public VideoPlayer videoPlayer;
    public RenderTexture videoTexture;

    [TextArea] public string fullText;
}
